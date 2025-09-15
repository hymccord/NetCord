using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NetCord.Services.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NetCordFirstAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NCS0003";

    private static readonly LocalizableString Title = "Classes with SlashCommand attribute must have SubSlashCommand methods";
    private static readonly LocalizableString MessageFormat = "Class '{0}' has SlashCommand attribute but no methods with SubSlashCommand attribute";
    private static readonly LocalizableString Description = "Classes decorated with SlashCommandAttribute must contain at least one method decorated with SubSlashCommandAttribute to form valid slash command groups.";
    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId, 
        Title, 
        MessageFormat, 
        Category, 
        DiagnosticSeverity.Warning, 
        isEnabledByDefault: true, 
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        // Try to get the well-known attribute types from the compilation
        var slashCommandAttributeType = context.Compilation.GetTypeByMetadataName(WellKnownTypeNames.NetCordServicesApplicationCommandSlashCommandAttribute);
        var subSlashCommandAttributeType = context.Compilation.GetTypeByMetadataName(WellKnownTypeNames.NetCordServicesApplicationCommandSubSlashCommandAttribute);

        // If either attribute type is not available in this compilation, skip analysis
        // This means the code being analyzed doesn't reference NetCord.Services
        if (slashCommandAttributeType == null || subSlashCommandAttributeType == null)
            return;

        // Register symbol action only when we know the required types are available
        context.RegisterSymbolAction(symbolContext => AnalyzeNamedType(symbolContext, slashCommandAttributeType, subSlashCommandAttributeType), SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context, INamedTypeSymbol slashCommandAttributeType, INamedTypeSymbol subSlashCommandAttributeType)
    {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        // Check if the class has SlashCommandAttribute using actual symbol comparison
        var hasSlashCommandAttribute = namedTypeSymbol.GetAttributes()
            .Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, slashCommandAttributeType));

        if (!hasSlashCommandAttribute)
            return;

        // Check if any method in the class has SubSlashCommandAttribute using actual symbol comparison
        var hasSubSlashCommandMethod = namedTypeSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Any(method => method.GetAttributes()
                .Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, subSlashCommandAttributeType)));

        if (!hasSubSlashCommandMethod)
        {
            var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
