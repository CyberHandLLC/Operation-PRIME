---
trigger: always_on
---

# Custom Instructions for Operation Prime Development

## Project Context
You are working on "Operation Prime" - a WinUI 3 incident management application using .NET 9, Entity Framework Core, and Clean Architecture patterns.

## Tech Stack Assessment
Before providing suggestions/writing any code or architectural structure, ALWAYS reference the latest official Microsoft documentation for our tech stack:

### Core Framework Documentation
- **WinUI 3**: https://learn.microsoft.com/en-us/windows/apps/winui/
- **Windows App Development**: https://learn.microsoft.com/en-us/windows/apps/develop/
- **Navigation Patterns**: https://learn.microsoft.com/en-us/windows/apps/design/basics/navigation-basics
- **WinUI Controls**: https://learn.microsoft.com/en-us/windows/apps/design/controls/

### MVVM & Architecture
- **MVVM Community Toolkit**: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/
- **Dependency Injection**: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
- **DI Guidelines**: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines
- **Clean Architecture**: https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

### Data & Entity Framework
- **EF Core**: https://learn.microsoft.com/en-us/ef/core/
- **EF Core with WinUI**: https://learn.microsoft.com/en-us/windows/apps/develop/data-access/entity-framework-7-with-sqlite
- **DbContext Configuration**: https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/

### Design System
- **Fluent Design**: https://fluent2.microsoft.design/
- **Windows Design Principles**: https://learn.microsoft.com/en-us/windows/apps/design/signature-experiences/design-principles
- **WinUI Styling**: https://learn.microsoft.com/en-us/windows/apps/design/style/

### Development Tools
- **Project Templates**: https://learn.microsoft.com/en-us/windows/apps/winui/winui3/create-your-first-winui3-app
- **Hot Reload**: https://learn.microsoft.com/en-us/visualstudio/debugger/hot-reload
- **MSIX Packaging**: https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/

## Assessment Protocol
When analyzing our codebase or suggesting improvements:

1. **Reference Current Documentation**: Always check the linked Microsoft documentation for the latest best practices
2. **Compare Against Standards**: Assess our implementation against the official patterns shown in the documentation
3. **Identify Gaps**: Point out any deviations from Microsoft's recommended approaches
4. **Suggest Improvements**: Provide specific recommendations based on the official documentation
5. **Explain Rationale**: Reference the specific documentation sections that support your suggestions

## Code Review Checklist
Before suggesting/writing any code changes, verify against these documentation areas:
- [ ] MVVM patterns match Community Toolkit documentation
- [ ] Dependency injection follows Microsoft DI guidelines
- [ ] WinUI controls are used according to official control documentation
- [ ] XAML binding patterns follow WinUI best practices
- [ ] Entity Framework usage aligns with EF Core documentation
- [ ] Architecture follows Clean Architecture principles
- [ ] Design follows Fluent Design system guidelines

## Response Format
When providing suggestion, structure your response as:
1. **Documentation Reference**: Cite the specific Microsoft documentation
2. **Current Assessment**: How our code compares to the documented best practices
3. **Recommendations**: Specific improvements based on the documentation
4. **Implementation**: Code examples that follow the documented patterns

## Important Notes
- ALWAYS verify information against the linked documentation before responding
- If documentation has been updated since your training, acknowledge this and recommend checking the latest version
- Focus on Microsoft's official recommendations rather than third-party interpretations
- When in doubt, direct to the official documentation for the most current guidance

### Additional Microsoft Resources
- **API Reference**: https://learn.microsoft.com/en-us/dotnet/api/microsoft.ui.xaml.controls
- **WinUI Gallery**: https://github.com/microsoft/WinUI-Gallery
- **Samples**: https://github.com/microsoft/WindowsAppSDK-Samples
- **Performance Guidelines**: https://learn.microsoft.com/en-us/windows/apps/develop/performance/
- **Accessibility**: https://learn.microsoft.com/en-us/windows/apps/design/accessibility/
- **Security**: https://learn.microsoft.com/en-us/windows/apps/develop/security/