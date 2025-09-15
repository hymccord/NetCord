using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = Analyzer1.Test.CSharpAnalyzerVerifier<NetCord.Services.Analyzers.NetCordFirstAnalyzer>;

namespace NetCord.Services.Analyzers.Test;

[TestClass]
public class SlashCommandGroupAnalyzerTests
{
    [TestMethod]
    public async Task ClassWithoutSlashCommandAttribute_ShouldNotReportDiagnostic()
    {
        var code = """
            using NetCord.Services.ApplicationCommands;
            
            public class TestModule
            {
                public void SomeMethod() { }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ClassWithSlashCommandAndSubSlashCommand_ShouldNotReportDiagnostic()
    {
        var code = """
            using NetCord.Services.ApplicationCommands;
            
            [SlashCommand("test", "Test command")]
            public class TestModule
            {
                [SubSlashCommand("sub", "Sub command")]
                public void SubCommand() { }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ClassWithSlashCommandButNoSubSlashCommand_ShouldReportDiagnostic()
    {
        var code = """
            using NetCord.Services.ApplicationCommands;
            
            [SlashCommand("test", "Test command")]
            public class {|#0:TestModule|}
            {
                public void RegularMethod() { }
            }
            """;

        var expected = VerifyCS.Diagnostic("NCS0003").WithLocation(0).WithArguments("TestModule");
        await VerifyCS.VerifyAnalyzerAsync(code, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ClassWithMultipleSlashCommandsAndOneSubSlashCommand_ShouldNotReportDiagnostic()
    {
        var code = """
            using NetCord.Services.ApplicationCommands;
            
            [SlashCommand("test1", "Test command 1")]
            [SlashCommand("test2", "Test command 2")]
            public class TestModule
            {
                [SubSlashCommand("sub", "Sub command")]
                public void SubCommand() { }
                
                public void RegularMethod() { }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ClassWithSlashCommandAndMultipleSubSlashCommands_ShouldNotReportDiagnostic()
    {
        var code = """
            using NetCord.Services.ApplicationCommands;
            
            [SlashCommand("test", "Test command")]
            public class TestModule
            {
                [SubSlashCommand("sub1", "Sub command 1")]
                public void SubCommand1() { }
                
                [SubSlashCommand("sub2", "Sub command 2")]
                public void SubCommand2() { }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code).ConfigureAwait(false);
    }
}
