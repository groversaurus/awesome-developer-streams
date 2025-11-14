open System
open System.IO
open System.Text.Json

// Simple structure validator - loads and checks JSON structure
[<EntryPoint>]
let main argv =
    printfn "============================================================"
    printfn "JSON Structure Validator"
    printfn "============================================================"
    
    let jsonPath = 
        if argv.Length > 0 then argv.[0]
        else "../../data/streamers.json"
    
    if not (File.Exists(jsonPath)) then
        printfn "❌ ERROR: %s not found" jsonPath
        1
    else
        try
            printfn "Loading JSON from %s..." jsonPath
            let jsonText = File.ReadAllText(jsonPath)
            let doc = JsonDocument.Parse(jsonText)
            let root = doc.RootElement
            
            let mutable errors = []
            let mutable warnings = []
            
            // Check top-level structure
            match root.TryGetProperty("version") with
            | (false, _) -> errors <- "Missing 'version' field" :: errors
            | _ -> ()
            
            match root.TryGetProperty("lastUpdated") with
            | (false, _) -> errors <- "Missing 'lastUpdated' field" :: errors
            | _ -> ()
            
            match root.TryGetProperty("streamers") with
            | (true, streamersElement) when streamersElement.ValueKind = JsonValueKind.Array ->
                let streamers = streamersElement.EnumerateArray() |> Seq.toList
                printfn "Found %d streamers" streamers.Length
                
                // Validate each streamer
                streamers |> List.iteri (fun idx streamer ->
                    let getName () =
                        match streamer.TryGetProperty("name") with
                        | (true, name) -> name.GetString()
                        | _ -> sprintf "#%d" (idx + 1)
                    
                    // Required fields
                    match streamer.TryGetProperty("id") with
                    | (false, _) -> errors <- sprintf "Streamer %s: Missing 'id'" (getName()) :: errors
                    | _ -> ()
                    
                    match streamer.TryGetProperty("name") with
                    | (false, _) -> errors <- sprintf "Streamer #%d: Missing 'name'" (idx + 1) :: errors
                    | _ -> ()
                    
                    match streamer.TryGetProperty("topics") with
                    | (false, _) -> errors <- sprintf "Streamer %s: Missing 'topics'" (getName()) :: errors
                    | _ -> ()
                    
                    match streamer.TryGetProperty("platforms") with
                    | (false, _) -> errors <- sprintf "Streamer %s: Missing 'platforms'" (getName()) :: errors
                    | _ -> ()
                    
                    match streamer.TryGetProperty("alphabeticalSection") with
                    | (false, _) -> errors <- sprintf "Streamer %s: Missing 'alphabeticalSection'" (getName()) :: errors
                    | _ -> ()
                    
                    // Optional but expected
                    match streamer.TryGetProperty("description") with
                    | (false, _) -> warnings <- sprintf "Streamer %s: Missing 'description'" (getName()) :: warnings
                    | _ -> ()
                    
                    match streamer.TryGetProperty("languages") with
                    | (false, _) -> warnings <- sprintf "Streamer %s: Missing 'languages'" (getName()) :: warnings
                    | _ -> ()
                )
                
            | _ ->
                errors <- "Missing or invalid 'streamers' array" :: errors
            
            printfn ""
            printfn "============================================================"
            printfn "VALIDATION RESULTS"
            printfn "============================================================"
            
            if not errors.IsEmpty then
                printfn "\n❌ Found %d error(s):" errors.Length
                errors |> List.rev |> List.take (min 10 errors.Length) |> List.iter (printfn "  • %s")
                if errors.Length > 10 then
                    printfn "  ... and %d more errors" (errors.Length - 10)
            else
                printfn "\n✅ No structural errors found!"
            
            if not warnings.IsEmpty then
                printfn "\n⚠️  Found %d warning(s):" warnings.Length
                warnings |> List.rev |> List.take (min 5 warnings.Length) |> List.iter (printfn "  • %s")
                if warnings.Length > 5 then
                    printfn "  ... and %d more warnings" (warnings.Length - 5)
            
            printfn "\n============================================================"
            
            if errors.IsEmpty then
                printfn "✓ Validation passed!"
                0
            else
                1
                
        with
        | :? JsonException as ex ->
            printfn "\n❌ JSON PARSE ERROR: %s" ex.Message
            1
        | ex ->
            printfn "\n❌ ERROR: %s" ex.Message
            1
