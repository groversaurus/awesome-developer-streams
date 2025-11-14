# GitHub Copilot Instructions

## User Preferences

### Working Style
- **Iterative Development**: Pause and review before moving to next steps
- **Visual Verification**: Inspect generated files before proceeding
- **Deliberate Commits**: User commits changes after review, never automatically
- **Discussion First**: Discuss and validate approach before implementation
- **Technology Consistency**: Prefer using existing project tools over introducing new dependencies
- **Minimal Documentation**: Don't create unnecessary documentation files

### Communication Preferences
- Be concise and direct
- Wait for explicit approval before starting major work
- Provide summaries and statistics for validation
- Show concrete examples when relevant
- Skip unnecessary introductions or conclusions

## Code Style & Conventions

### C# 
- Use nullable reference types
- Follow Microsoft's C# coding conventions
- Component-based architecture (for Blazor/UI)
- Services for data access and API calls
- Meaningful variable and method names
- Prefer explicit over implicit when it aids clarity

### F#
- Functional-first approach
- Immutable data structures by default
- Use pattern matching extensively
- Pipeline operators for data transformation
- Console apps should have clear, informative output with progress indicators
- Prefer pure functions and composition

### General Principles
- Write idiomatic code for the language being used
- Prioritize readability and maintainability
- Include helpful error messages
- Avoid premature optimization
- Use existing project patterns and conventions
