using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;


namespace SwitchExpressionAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SwitchExpressionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "switch_expression";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle),
                                                                                        Resources.ResourceManager,
                                                                                        typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat),
                                                                                                Resources.ResourceManager,
                                                                                                typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription),
                                                                                              Resources.ResourceManager,
                                                                                              typeof(Resources));

        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId,
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


            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var methodDeclarationNode = (MethodDeclarationSyntax)context.Node;

            var hasExpressionBody = methodDeclarationNode.ExpressionBody != null;

            if (!hasExpressionBody)
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclarationNode.GetLocation(), methodDeclarationNode.Identifier);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
