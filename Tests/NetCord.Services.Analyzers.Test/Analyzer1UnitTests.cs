using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = Analyzer1.Test.CSharpCodeFixVerifier<
    NetCord.Services.Analyzers.Analyzer1Analyzer,
    NetCord.Services.Analyzers.Analyzer1CodeFixProvider>;

namespace NetCord.Services.Analyzers.Test;
[TestClass]
public class Analyzer1UnitTest
{
    //No diagnostics expected to show up
    [TestMethod]
    public async Task TestMethod1()
    {
        var code = """

        class CLASSNAME
        {   
        }

        """;

        await VerifyCS.VerifyAnalyzerAsync(code).ConfigureAwait(false);
    }

    //Diagnostic and CodeFix both triggered and checked for
    [TestMethod]
    public async Task TestMethod2()
    {
        var code = """

        class {|#0:TypeName|}
        {   
        }

        """;

        var codefix = """

        class TYPENAME
        {   
        }

        """;

        var expected = VerifyCS.Diagnostic("Analyzer1").WithLocation(0).WithArguments("TypeName");
        await VerifyCS.VerifyCodeFixAsync(code, expected, codefix).ConfigureAwait(false);
    }
}
