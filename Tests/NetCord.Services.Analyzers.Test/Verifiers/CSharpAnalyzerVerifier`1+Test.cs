using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

using NetCord.Services;
using NetCord.Services.Analyzers.Test.Verifiers;

namespace Analyzer1.Test;
public static partial class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public class Test : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
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
