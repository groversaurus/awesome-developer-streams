# Awesome Developer Streams - Blazor Spike Roadmap

## Project Vision
Transform the awesome-developer-streams list into an interactive, searchable Blazor web application while maintaining the existing README.md as the community-editable source of truth.

---

## Status: Planning & Discussion Phase
**Branch**: `spike/blazor`  
**Started**: November 11, 2025  
**Last Updated**: November 11, 2025

---

## Decisions to Make

### 1. Blazor Hosting Model
**Options:**
- [x] **Blazor WebAssembly** ✅ **SELECTED**
  - ✅ Static hosting (GitHub Pages, Azure Static Web Apps)
  - ✅ No server costs
  - ✅ Offline-capable
  - ❌ Larger initial download
  
- [ ] **Blazor Server**
  - ✅ Smaller client payload
  - ✅ Fast initial load
  - ❌ Requires hosting infrastructure
  - ❌ Websocket connection required

**Decision**: ✅ **Blazor WebAssembly** - Enables static hosting with no server infrastructure requirements

---

### 2. Data Management Strategy
**Options:**
- [x] **README as Source** ✅ **SELECTED**
  - Keep README.md as primary
  - Generate JSON on-demand or via CI/CD
  - Easy community contributions via PR
  - Future: Consider contribution web UI if maintenance burden grows
  
- [ ] **JSON as Source**
  - Make JSON primary data source
  - Generate README from JSON
  - More structured but harder to contribute

**Decision**: ✅ **README as Source** - Maintains community-friendly contribution model. Will evaluate creating a web-based contribution UI in the future if manual editing becomes a bottleneck.

---

### 3. Parsing Approach
**Options:**
- [ ] **Regex-based parser**
  - Quick to implement
  - Fragile to format changes
  
- [ ] **Markdown AST parser**
  - More robust
  - Handles variations better
  
- [x] **Hybrid approach** ✅ **SELECTED**
  - Section-based parsing with targeted regex
  - Balance of robustness and simplicity

**Decision**: ✅ **Hybrid F# Parser** - Use markdown structure to identify sections and streamer blocks, then apply targeted regex/pattern matching within each block to extract fields. Combines structural robustness with implementation simplicity.

---

### 4. Link Validation Strategy
**Options:**
- [x] **One-time validation** ✅ **SELECTED** - Manual run, generate report
- [ ] **CI/CD automated** - Check on every PR
- [ ] **Scheduled checks** - Weekly cron job
- [ ] **Lazy validation** - Check on user request in UI

**Decision**: ✅ **Manual validation tool** - F# console application run locally to generate validation reports. Simple, no infrastructure required, and a fun way to explore F#!

---

### 5. Additional Features
**Priority Features (Phase 5):**
- [x] Twitch API integration for live status ✅ **PLANNED**
- [x] YouTube API for subscriber counts ✅ **PLANNED**
- [x] Statistics/analytics dashboard ✅ **PLANNED**
- [x] Dark mode ✅ **PLANNED**

**Future Consideration:**
- [ ] RSS feed generation
- [ ] Export to CSV/JSON

**Decision**: ✅ First four features planned for Phase 5 implementation after core functionality is complete.

---

## Phase 1: Foundation (Week 1-2)

### ✅ Completed
- [x] Create spike branch
- [x] Initial planning and documentation
- [x] Created copilot-instructions.md
- [x] Created roadmap.md

### 🚧 In Progress
- [x] Finalize architectural decisions ✅
- [ ] Design JSON schema
- [ ] Set up project structure

### 📋 To Do
- [ ] Create markdown parser prototype
- [ ] Test parsing on subset of streamers
- [ ] Validate JSON output
- [ ] Set up basic Blazor project
- [ ] Create sample components

**Deliverables:**
- `data/streamers.json` - Initial parsed data
- `tools/parser/` - Working parser
- Basic Blazor app scaffold

---

## Phase 2: Core Functionality (Week 3-4)

### 📋 Planned Tasks
- [ ] Implement search functionality
  - [ ] Technology/topic search
  - [ ] Streamer name search
  - [ ] Full-text search
  
- [ ] Implement filters
  - [ ] Platform filter (Twitch, YouTube, etc.)
  - [ ] Language filter
  - [ ] Topic/technology filter
  
- [ ] Create UI components
  - [ ] Streamer card component
  - [ ] Search bar component
  - [ ] Filter panel component
  - [ ] Alphabetical index
  
- [ ] Responsive design
  - [ ] Mobile layout
  - [ ] Tablet layout
  - [ ] Desktop layout

**Deliverables:**
- Working search and filter
- Responsive UI
- Basic styling

---

## Phase 3: Validation & Quality (Week 5)

### 📋 Planned Tasks
- [ ] Implement link checker
  - [ ] HTTP status validation
  - [ ] Retry logic for failures
  - [ ] Generate validation report
  
- [ ] Data quality checks
  - [ ] Duplicate detection
  - [ ] Format validation
  - [ ] Missing field detection
  
- [ ] Testing
  - [ ] Unit tests for parser
  - [ ] Component tests
  - [ ] Integration tests

**Deliverables:**
- `tools/link-checker/` - Working validator
- `data/validation-report.json` - Link status
- Test coverage report

---

## Phase 4: Enhancement & Polish (Week 6)

### 📋 Planned Tasks
- [ ] Dark mode support
- [ ] Performance optimization
- [ ] Accessibility audit
- [ ] SEO optimization
- [ ] Documentation
- [ ] Deployment setup

**Deliverables:**
- Production-ready application
- Deployment pipeline
- User documentation

---

## Phase 5: Advanced Features (Future)

### 🔮 Future Enhancements
- [ ] Twitch API integration
  - [ ] Live status indicators
  - [ ] Viewer counts
  - [ ] Stream schedules
  
- [ ] Statistics dashboard
  - [ ] Technology trends
  - [ ] Platform distribution
  - [ ] Language distribution
  
- [ ] Community features
  - [ ] Favorites/bookmarks
  - [ ] Stream notifications
  - [ ] Calendar integration

---

## Technical Debt & Considerations

### Current Concerns
- Performance with 200+ streamers
- Markdown format variations in README
- Link rot over time
- API rate limits (if using external APIs)
- Maintenance burden

### Mitigation Strategies
- Implement caching for parsed data
- Version JSON schema
- Regular automated link checking
- Rate limiting and error handling
- Clear contribution guidelines

---

## Success Metrics

### Spike Success Criteria
- [ ] Successfully parse 100% of streamers from README
- [ ] Search returns results in < 100ms
- [ ] Mobile-responsive design works on all devices
- [ ] Link validation runs successfully
- [ ] Can filter by at least 3 criteria simultaneously

### Production Readiness Criteria
- [ ] 95%+ link validity
- [ ] Accessibility score 90+
- [ ] Lighthouse performance score 90+
- [ ] Zero critical security issues
- [ ] Documentation complete

---

## Notes & Decisions Log

### 2025-11-11
- **Created**: Initial roadmap and copilot instructions
- **Status**: ✅ All architectural decisions finalized
- **Decision**: Blazor WebAssembly for static hosting
- **Decision**: README.md as source of truth (with future consideration for web-based contribution UI)
- **Decision**: Link validation tool in F# - manual local execution
- **Decision**: Hybrid F# parser - structural sections + targeted regex
- **Decision**: Phase 5 priorities - Twitch API, YouTube API, Statistics dashboard, Dark mode
- **Next Steps**: Begin Phase 1 implementation - Design JSON schema and set up project structure

---

## Questions for Consideration

1. **Performance**: How to handle 500+ streamers in the future?
2. **Maintenance**: Who maintains the Blazor app vs README?
3. **Contributions**: How do contributors test their additions?
4. **Deployment**: Where will this be hosted?
5. **Analytics**: Do we track usage/popular streamers?
6. **Monetization**: Any plans for sponsors/featured streamers?
7. **Internationalization**: Support for non-English interfaces?

---

## References & Resources

- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
- [Original awesome-developer-streams](https://github.com/groversaurus/awesome-developer-streams)
- [Awesome List Guidelines](https://github.com/sindresorhus/awesome/blob/main/contributing.md)

---

**Last Review**: 2025-11-11  
**Next Review**: TBD  
**Owner**: @groversaurus
