using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace SwitchExpressionAnalyzer;


[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SwitchExpressionCodeFixProvider)), Shared]
public class SwitchExpressionCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(SwitchExpressionAnalyzer.DiagnosticId); }
    }

    public sealed override FixAllProvider GetFixAllProvider()
    {
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the type declaration identified by the diagnostic.
        var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

        context.RegisterCodeFix
        (
            CodeAction.Create
            (
                title:                 CodeFixResources.CodeFixTitle,
                createChangedDocument: c => ToExpressionBodyAsync(context.Document, declaration, c),
                equivalenceKey:        nameof(CodeFixResources.CodeFixTitle)
            ),
            diagnostic
        );
    }

    private async Task<Document> ToExpressionBodyAsync(Document document, MethodDeclarationSyntax methodDecl, CancellationToken cancellationToken)
    {
        var invocationExp = methodDecl.Body.DescendantNodes().Where(n => n.IsKind(SyntaxKind.InvocationExpression)).FirstOrDefault();

        var expressionBody = SyntaxFactory.ArrowExpressionClause(invocationExp as InvocationExpressionSyntax)
                                          .WithArrowToken(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken)
                                                                       .WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(" "))));

        var newMethodDecl = methodDecl.RemoveNode(methodDecl.Body, SyntaxRemoveOptions.KeepNoTrivia)
                                      .WithExpressionBody(expressionBody)
                                      .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                                                                       .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed));

        var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newRoot = oldRoot.ReplaceNode(methodDecl, newMethodDecl);

        return document.WithSyntaxRoot(newRoot);
    }
}
