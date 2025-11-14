# GitHub Copilot Instructions

## Project Overview
This is a Blazor spike project for the awesome-developer-streams repository. The goal is to create a searchable, interactive web application to browse developer streamers while maintaining the existing README.md as the community-editable source of truth.

## User Preferences

### Working Style
- **Iterative Development**: User prefers to pause and review before moving to next steps
- **Visual Verification**: User wants to inspect generated files (like JSON) before proceeding
- **Deliberate Commits**: User commits changes themselves after review, not automatically
- **Discussion First**: User likes to discuss and validate approach before implementation
- **Technology Consistency**: Prefer using existing project tools (F#) over introducing new ones (Python)

### Communication Preferences
- Be concise and direct
- Wait for explicit approval before starting Blazor app work
- Provide summaries and statistics for validation
- Show concrete examples of parsed data
- Don't create unnecessary documentation files

## Architecture Decisions

### Data Flow
1. **Source of Truth**: README.md remains the primary data source
2. **JSON Generation**: Parse README.md → generate `data/streamers.json`
3. **Blazor App**: Consumes JSON for search/filter functionality

### Technology Stack
- **Frontend**: Blazor WebAssembly (static hosting)
- **Data Format**: JSON schema for streamer data
- **Parser**: F# console application with hybrid parsing approach
- **Validator**: F# console application for JSON structure validation
- **Link Checker**: F# console application (to be implemented)
- **Languages**: C# for Blazor, F# for all tooling

## Code Style & Conventions

### C# / Blazor
- Use nullable reference types
- Follow Microsoft's C# coding conventions
- Component-based architecture
- Services for data access and API calls

### F# / Tooling
- Functional-first approach
- Immutable data structures
- Use pattern matching extensively
- Pipeline operators for data transformation
- Console apps for all tooling (parser, validator, link-checker)
- Clear, informative console output with progress indicators

### Parsing Strategy
- **Hybrid Approach**: Structural navigation + targeted regex
- Find main content section: "# Developers That Stream"
- Split by `---` separators to get individual streamer blocks
- Parse each block for structured data (name, description, topics, platforms, links)
- Determine alphabetical section from first letter of name
- Flexible link extraction to handle any link type in README

### JSON Schema
```json
{
  "version": "1.0.0",
  "lastUpdated": "YYYY-MM-DD",
  "streamers": [
    {
      "id": "string (slug format)",
      "name": "string",
      "aka": "string (optional)",
      "handle": "string (optional)",
      "description": "string",
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
        "youtube": "string",
        "playlist": "string"
      },
      "languages": ["string"],
      "alphabeticalSection": "string (A-Z)"
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
│   ├── schema.json             # JSON Schema definition
│   └── example.json            # Sample data
├── tools/
│   ├── parser/                 # F# Markdown → JSON converter
│   ├── validator/              # F# JSON structure validator
│   └── link-checker/           # F# URL validation (to be implemented)
├── src/
│   └── AwesomeDevStreams/      # Blazor WebAssembly application
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

## Contributing
When working on this project:
- Keep README.md format backward compatible
- JSON schema changes require parser updates
- Test with full dataset (232 streamers)
- Consider performance with 200+ streamers
- Maintain accessibility standards

## Notes
- This is a spike/proof-of-concept project
- Focus on validating the approach
- Gather feedback before full implementation
- Consider community maintenance burden
