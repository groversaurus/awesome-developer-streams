# GitHub Copilot Instructions

## Project Overview
This is a Blazor spike project for the awesome-developer-streams repository. The goal is to create a searchable, interactive web application to browse developer streamers while maintaining the existing README.md as the community-editable source of truth.

## Architecture Decisions

### Data Flow
1. **Source of Truth**: README.md remains the primary data source
2. **JSON Generation**: Parse README.md → generate `data/streamers.json`
3. **Blazor App**: Consumes JSON for search/filter functionality

### Technology Stack
- **Frontend**: Blazor WebAssembly (static hosting)
- **Data Format**: JSON schema for streamer data
- **Validation**: F# console application for link checking (manual execution)
- **Parser**: TBD (C# or F#)
- **Languages**: C# for Blazor, F# for tooling

## Code Style & Conventions

### C# / Blazor
- Use nullable reference types
- Follow Microsoft's C# coding conventions
- Component-based architecture
- Services for data access and API calls

### F# / Tooling
- Functional-first approach
- Immutable data structures
- Use Railway-Oriented Programming for error handling
- Pipeline operators for data transformation

### JSON Schema
```json
{
  "streamers": [
    {
      "id": "string",
      "name": "string",
      "handle": "string",
      "topics": ["string"],
      "platforms": [
        {
          "name": "string",
          "url": "string"
        }
      ],
      "links": {
        "twitter": "string",
        "github": "string",
        "website": "string",
        "youtube": "string"
      },
      "languages": ["string"]
    }
  ]
}
```

## Project Structure
```
/
├── README.md                    # Community-editable source
├── data/
│   ├── streamers.json          # Generated from README
│   └── validation-report.json  # Link validation results
├── tools/
│   ├── parser/                 # Markdown → JSON converter
│   └── link-checker/           # URL validation
├── src/
│   └── BlazorApp/              # Blazor application
├── .github/
│   └── copilot-instructions.md # This file
└── roadmap.md                  # Project roadmap
```

## Development Workflow

1. **Parsing**: Create robust markdown parser to extract streamer data
2. **Validation**: Implement link checker with retry logic
3. **Blazor App**: Build search/filter UI with responsive design
4. **CI/CD**: Automate JSON generation and validation

## Key Features to Implement

### Phase 1: Data Extraction
- Markdown parser (README → JSON)
- Link validation tool
- Generate structured data

### Phase 2: Blazor Application
- Search by technology/topic
- Filter by platform (Twitch, YouTube, etc.)
- Filter by spoken language
- Alphabetical browsing
- Responsive design
- Dark mode support

### Phase 3: Enhancements (Future)
- Twitch API integration for live status
- Statistics dashboard
- Contribution guidelines integration
- Export functionality

## Open Questions (To be refined)
1. ✅ Blazor hosting model: **WebAssembly for static hosting**
2. ✅ Parsing strategy: **Hybrid approach - F# structural parsing with targeted regex**
3. ✅ Link validation: **Manual F# tool**
4. API integrations: Twitch live status?
5. Deployment target: GitHub Pages, Azure, other?
## Open Questions (To be refined)
1. Blazor hosting model: WebAssembly vs Server?
2. Parsing strategy: Regex vs structured parser?
3. Link validation: One-time vs CI/CD automated?
4. API integrations: Twitch live status?
5. Deployment target: GitHub Pages, Azure, other?

## Contributing
When working on this project:
- Keep README.md format backward compatible
- JSON schema changes require parser updates
- Test with full dataset
- Consider performance with 100+ streamers
- Maintain accessibility standards

## Notes
- This is a spike/proof-of-concept project
- Focus on validating the approach
- Gather feedback before full implementation
- Consider community maintenance burden
