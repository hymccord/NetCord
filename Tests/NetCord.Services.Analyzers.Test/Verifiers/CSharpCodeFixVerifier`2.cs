using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Analyzer1.Test;
public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic()"/>
    public static DiagnosticResult Diagnostic()
        => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, DefaultVerifier>.Diagnostic();

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(string)"/>
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, DefaultVerifier>.Diagnostic(diagnosticId);

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(DiagnosticDescriptor)"/>
    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
        => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, DefaultVerifier>.Diagnostic(descriptor);

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
    public static async Task VerifyAnalyzerAsync([StringSyntax("c#-test")] string source, params DiagnosticResult[] expected)
    {
        var test = new Test
        {
            TestCode = source,
        };

        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, string)"/>
    public static async Task VerifyCodeFixAsync([StringSyntax("c#-test")] string source, [StringSyntax("c#-test")] string fixedSource)
        => await VerifyCodeFixAsync(source, DiagnosticResult.EmptyDiagnosticResults, fixedSource).ConfigureAwait(false);

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult, string)"/>
    public static async Task VerifyCodeFixAsync([StringSyntax("c#-test")] string source, DiagnosticResult expected, [StringSyntax("c#-test")] string fixedSource)
        => await VerifyCodeFixAsync(source, [expected], fixedSource).ConfigureAwait(false);

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult[], string)"/>
    public static async Task VerifyCodeFixAsync([StringSyntax("c#-test")] string source, DiagnosticResult[] expected, [StringSyntax("c#-test")] string fixedSource)
    {
        var test = new Test
        {
            TestCode = source,
            FixedCode = fixedSource,
        };

        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }
}
