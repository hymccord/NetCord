using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

using NetCord.Services;
using NetCord.Services.Analyzers.Test.Verifiers;

namespace Analyzer1.Test;
public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    public class Test : CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
    {
        public Test()
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90;
            TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(IService).Assembly.Location));
            SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId)!.CompilationOptions!;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
                solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                return solution;
            });
        }
    }
}
