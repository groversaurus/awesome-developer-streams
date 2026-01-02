open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open System.Text.RegularExpressions

// Domain types matching our JSON schema
type Platform = {
    [<JsonPropertyName("name")>]
    Name: string
    [<JsonPropertyName("url")>]
    Url: string
}

type Streamer = {
    [<JsonPropertyName("id")>]
    Id: string
    [<JsonPropertyName("name")>]
    Name: string
    [<JsonPropertyName("aka")>]
    [<JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)>]
    Aka: string option
    [<JsonPropertyName("handle")>]
    [<JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)>]
    Handle: string option
    [<JsonPropertyName("description")>]
    Description: string
    [<JsonPropertyName("topics")>]
    Topics: string list
    [<JsonPropertyName("platforms")>]
    Platforms: Platform list
    [<JsonPropertyName("links")>]
    Links: Map<string, string>
    [<JsonPropertyName("languages")>]
    Languages: string list
    [<JsonPropertyName("alphabeticalSection")>]
    AlphabeticalSection: string
}

type TopicCount = {
    [<JsonPropertyName("topic")>]
    Topic: string
    [<JsonPropertyName("count")>]
    Count: int
}

type Statistics = {
    [<JsonPropertyName("totalStreamers")>]
    TotalStreamers: int
    [<JsonPropertyName("totalTopics")>]
    TotalTopics: int
    [<JsonPropertyName("totalPlatforms")>]
    TotalPlatforms: int
    [<JsonPropertyName("totalLanguages")>]
    TotalLanguages: int
    [<JsonPropertyName("platformBreakdown")>]
    PlatformBreakdown: Map<string, int>
    [<JsonPropertyName("languageBreakdown")>]
    LanguageBreakdown: Map<string, int>
    [<JsonPropertyName("topTopics")>]
    TopTopics: TopicCount list
}

type StreamerData = {
    [<JsonPropertyName("version")>]
    Version: string
    [<JsonPropertyName("lastUpdated")>]
    LastUpdated: string
    [<JsonPropertyName("statistics")>]
    Statistics: Statistics
    [<JsonPropertyName("streamers")>]
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

let parseStreamerBlock (section: string) (block: string) : Streamer option =
    let lines = block.Split([|'\n'; '\r'|], StringSplitOptions.RemoveEmptyEntries)
    
    if lines.Length = 0 then None
    else
        // First line is name (starts with ###)
        match extractName lines[0] with
        | None -> None
        | Some(name, aka) ->
            let id = createId name
            
            // Find "What X streams:" section
            let streamingLine = 
                lines 
                |> Array.tryFindIndex (fun line -> line.StartsWith("####") && line.ToLower().Contains("what") && line.ToLower().Contains(("stream")))
            
            let description, topics = 
                match streamingLine with
                | Some idx when idx + 1 < lines.Length ->
                    let desc = lines[idx + 1].TrimStart([|'-'; ' '|])
                    (desc, extractTopics desc)
                | _ -> ("", [])
            
            // Find "Streaming on:" section for platform links
            let platformsStartIdx = 
                lines 
                |> Array.tryFindIndex (fun line -> line.StartsWith("####") && line.ToLower().Contains("streaming on"))
            
            let platforms = 
                match platformsStartIdx with
                | Some idx ->
                    lines
                    |> Array.skip (idx + 1)
                    |> Array.takeWhile (fun line -> not (line.StartsWith("####") || line.StartsWith("---")))
                    |> Array.filter (fun line -> line.StartsWith("-") && line.Contains("[") && line.Contains("]"))
                    |> Array.choose extractLink
                    |> Array.map (fun (text, url) -> { Platform.Name = text; Url = url })
                    |> Array.toList
                | None -> []
            
            // Find "Links:" section for social links
            let linksStartIdx = 
                lines 
                |> Array.tryFindIndex (fun line -> line.StartsWith("#### Links:"))
            
            let links = 
                match linksStartIdx with
                | Some idx ->
                    let linkLines = 
                        lines
                        |> Array.skip (idx + 1)
                        |> Array.takeWhile (fun line -> not (line.StartsWith("####") || line.StartsWith("---") || line.StartsWith("###")))
                        |> Array.filter (fun line -> line.StartsWith("-") && line.Contains("["))
                    
                    linkLines
                    |> Array.choose extractLink
                    |> Array.map (fun (name, url) -> 
                        // Normalize common link names to lowercase keys
                        let key = 
                            match name.ToLowerInvariant() with
                            | "twitter" | "x" -> "twitter"
                            | "github" -> "github"
                            | "website" | "web" | "blog" -> "website"
                            | "youtube" | "youtube channel" | "video playlist" -> "youtube"
                            | other -> other.ToLowerInvariant().Replace(" ", "_")
                        (key, url))
                    |> Map.ofArray
                | None -> Map.empty
            
            // Extract languages - for now default to English
            let languages = ["English"]
            
            Some {
                Id = id
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

// Section parsing functions
let parseSections (lines: string list) : (string * string) list =
    // Find all section headers (## A, ## B, etc.)
    let sectionIndices = 
        lines
        |> List.indexed
        |> List.filter (fun (_, line) -> Regex.IsMatch(line, @"^##\s+[A-Z]$"))
        |> List.map fst
    
    // Extract section name and text for each section
    sectionIndices
    |> List.mapi (fun i idx ->
        let sectionName = lines.[idx].Substring(3).Trim()
        let endIdx = 
            if i + 1 < sectionIndices.Length then
                sectionIndices.[i + 1]
            else
                lines.Length
        let sectionLines = lines.[idx + 1 .. endIdx - 1]
        let sectionText = String.Join("\n", sectionLines)
        (sectionName, sectionText))

let splitIntoStreamerBlocks (sectionText: string) : string array =
    sectionText.Split([|"---"|], StringSplitOptions.RemoveEmptyEntries)
    |> Array.filter (fun block -> block.Trim().StartsWith("###"))

let calculateStatistics (streamers: Streamer list) : Statistics =
    // Count unique topics
    let allTopics = 
        streamers 
        |> List.collect (fun s -> s.Topics)
    
    let uniqueTopics = allTopics |> List.distinct
    
    // Count topics frequency for top topics
    let topicCounts = 
        allTopics
        |> List.groupBy id
        |> List.map (fun (topic, instances) -> { Topic = topic; Count = instances.Length })
        |> List.sortByDescending (fun tc -> tc.Count)
        |> List.take (min 20 (List.length (List.groupBy id allTopics)))
    
    // Count unique platforms and create breakdown
    let allPlatforms = 
        streamers 
        |> List.collect (fun s -> s.Platforms |> List.map (fun p -> p.Name))
    
    let uniquePlatforms = allPlatforms |> List.distinct
    
    let platformBreakdown = 
        allPlatforms
        |> List.groupBy id
        |> List.map (fun (platform, instances) -> (platform, instances.Length))
        |> Map.ofList
    
    // Count unique languages and create breakdown
    let allLanguages = 
        streamers 
        |> List.collect (fun s -> s.Languages)
    
    let uniqueLanguages = allLanguages |> List.distinct
    
    let languageBreakdown = 
        allLanguages
        |> List.groupBy id
        |> List.map (fun (language, instances) -> (language, instances.Length))
        |> Map.ofList
    
    {
        TotalStreamers = streamers.Length
        TotalTopics = uniqueTopics.Length
        TotalPlatforms = uniquePlatforms.Length
        TotalLanguages = uniqueLanguages.Length
        PlatformBreakdown = platformBreakdown
        LanguageBreakdown = languageBreakdown
        TopTopics = topicCounts
    }



[<EntryPoint>]
let main argv =
    printfn "Awesome Developer Streams - Markdown Parser"
    printfn "=========================================="
    
    let readmePath = "../../../README.md"
    let outputPath = "../../../data/streamers.json"
    
    if not (File.Exists(readmePath)) then
        printfn "ERROR: README.md not found at %s" readmePath
        1
    else
        printfn "Reading README.md..."
        let allLines = File.ReadAllLines(readmePath)
        
        // Find where actual streamer content starts (after the TOC)
        let startIdx = 
            allLines
            |> Array.tryFindIndex (fun line -> line.Trim() = "# Developers That Stream")
        
        let content = 
            match startIdx with
            | Some idx -> String.Join("\n", allLines |> Array.skip (idx + 1))
            | None -> String.Join("\n", allLines)
        
        printfn "Parsing streamers..."
        
        // Split directly into streamer blocks
        let streamerBlocks = splitIntoStreamerBlocks content
        
        printfn "Found %d streamer blocks" streamerBlocks.Length
        
        // Parse each streamer and determine section by first letter of name
        let streamers = 
            streamerBlocks
            |> Array.choose (fun block ->
                match extractName (block.Split([|'\n'; '\r'|], StringSplitOptions.RemoveEmptyEntries) |> Array.head) with
                | Some(name, _) ->
                    let section = name.[0].ToString().ToUpperInvariant()
                    parseStreamerBlock section block
                | None -> None)
            |> Array.toList
        
        // Group by section for reporting
        let sections = 
            streamers
            |> List.groupBy (fun s -> s.AlphabeticalSection)
            |> List.sortBy fst
        
        for (section, sectionStreamers) in sections do
            printfn "  Section %s: %d streamers" section sectionStreamers.Length
        
        printfn "\nTotal streamers parsed: %d" streamers.Length
        
        // Calculate statistics
        printfn "\nCalculating statistics..."
        let stats = calculateStatistics streamers
        
        printfn "  Total unique topics: %d" stats.TotalTopics
        printfn "  Total unique platforms: %d" stats.TotalPlatforms
        printfn "  Total unique languages: %d" stats.TotalLanguages
        
        // Create output data
        let data = {
            Version = "1.0.0"
            LastUpdated = DateTime.UtcNow.ToString("yyyy-MM-dd")
            Statistics = stats
            Streamers = streamers
        }
        
        // Serialize to JSON
        printfn "\nWriting JSON to %s..." outputPath
        let options = JsonSerializerOptions()
        options.WriteIndented <- true
        options.Converters.Add(JsonFSharpConverter())
        
        let json = JsonSerializer.Serialize(data, options)
        
        // Ensure output directory exists
        let outputDir = Path.GetDirectoryName(outputPath)
        if not (Directory.Exists(outputDir)) then
            Directory.CreateDirectory(outputDir) |> ignore
        
        File.WriteAllText(outputPath, json)
        
        printfn "✓ Successfully generated streamers.json"
        printfn "  %d streamers across %d sections" streamers.Length sections.Length
        
        0
