open System
open System.IO
open System.Text.RegularExpressions

// Domain types matching our JSON schema
type Platform = {
    Name: string
    Url: string
}

type Streamer = {
    Id: string
    Name: string
    Aka: string option
    Handle: string option
    Description: string
    Topics: string list
    Platforms: Platform list
    Links: Map<string, string>
    Languages: string list
    AlphabeticalSection: string
}

type StreamerData = {
    Version: string
    LastUpdated: string
    Streamers: Streamer list
}

// Helper functions
let createId (name: string) =
    name.ToLowerInvariant()
        .Replace(" ", "-")
        .Replace("(", "")
        .Replace(")", "")

let extractName (line: string) =
    let pattern = @"^###\s+(.+?)(?:\s+\(([^)]+)\))?$"
    let m = Regex.Match(line.Trim(), pattern)
    if m.Success then
        let name = m.Groups.[1].Value.Trim()
        let aka = if m.Groups.[2].Success then Some(m.Groups.[2].Value.Trim()) else None
        Some(name, aka)
    else
        None

let extractTopics (line: string) =
    line.Split(',')
    |> Array.map (fun s -> s.Trim())
    |> Array.filter (fun s -> s.Length > 0)
    |> Array.toList

let extractLink (line: string) =
    let pattern = @"\[([^\]]+)\]\(([^)]+)\)"
    let m = Regex.Match(line, pattern)
    if m.Success then
        Some(m.Groups.[1].Value.Trim(), m.Groups.[2].Value.Trim())
    else
        None

let parseStreamerBlock (section: string) (lines: string list) : Streamer option =
    try
        // Find the name (### header)
        let nameInfo = 
            lines 
            |> List.tryPick (fun line -> 
                if line.StartsWith("###") then extractName line else None)
        
        match nameInfo with
        | None -> None
        | Some(name, aka) ->
            // Extract description (What X streams:)
            let descIdx = lines |> List.tryFindIndex (fun l -> l.Contains("What") && l.Contains("streams"))
            let description = 
                match descIdx with
                | Some idx when idx + 1 < lines.Length ->
                    let desc = lines.[idx + 1].Trim()
                    if desc.StartsWith("-") then desc.Substring(1).Trim() else desc
                | _ -> ""
            
            // Extract topics from description
            let topics = extractTopics description
            
            // Extract platforms (Streaming on:)
            let platformStartIdx = lines |> List.tryFindIndex (fun l -> l.Contains("Streaming on"))
            let platforms = 
                match platformStartIdx with
                | Some startIdx ->
                    lines
                    |> List.skip (startIdx + 1)
                    |> List.takeWhile (fun l -> l.StartsWith("-") && l.Contains("["))
                    |> List.choose extractLink
                    |> List.map (fun (name, url) -> { Name = name; Url = url })
                | None -> []
            
            // Extract links (Links:)
            let linksStartIdx = lines |> List.tryFindIndex (fun l -> l.Trim() = "#### Links:")
            let links = 
                match linksStartIdx with
                | Some startIdx ->
                    lines
                    |> List.skip (startIdx + 1)
                    |> List.takeWhile (fun l -> l.StartsWith("-") && l.Contains("["))
                    |> List.choose extractLink
                    |> List.map (fun (name, url) -> (name.ToLowerInvariant(), url))
                    |> Map.ofList
                | None -> Map.empty
            
            // For now, default to English
            let languages = ["English"]
            
            Some {
                Id = createId name
                Name = name
                Aka = aka
                Handle = aka
                Description = description
                Topics = topics
                Platforms = platforms
                Links = links
                Languages = languages
                AlphabeticalSection = section
            }
    with
    | ex -> 
        printfn "Error parsing streamer block: %s" ex.Message
        None

[<EntryPoint>]
let main argv =
    printfn "Awesome Developer Streams - Markdown Parser"
    printfn "=========================================="
    
    let readmePath = "../../../README.md"
    
    if not (File.Exists(readmePath)) then
        printfn "ERROR: README.md not found at %s" readmePath
        1
    else
        printfn "Reading README.md..."
        let lines = File.ReadAllLines(readmePath) |> Array.toList
        
        printfn "Parsing streamers..."
        printfn "TODO: Implement full parsing logic"
        
        // For now, just test with a simple output
        printfn "\nParser prototype ready!"
        printfn "Next: Implement section-based parsing to extract all streamers"
        
        0
