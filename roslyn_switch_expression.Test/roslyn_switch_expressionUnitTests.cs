using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = SwitchExpressionAnalyzer.Test.CSharpCodeFixVerifier<
    SwitchExpressionAnalyzer.SwitchExpressionAnalyzer,
    SwitchExpressionAnalyzer.SwitchExpressionCodeFixProvider>;

namespace SwitchExpressionAnalyzer.Test;


[TestClass]
public class RoslynSwitchExpressionUnitTest
{
    [TestMethod]
    public async Task ExpressionBody_NoDiagnostic()
    {
        var test = 
        @"using System;

class Program
{
    static void Main() => Console.WriteLine(4);
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }


    [TestMethod]
    public async Task BlockBody_Diagnostic()
    {
        var test =
@"using System;

class Program
{
    [|static void Main() { Console.WriteLine(4); }|]
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task BlockBody_DiagnosticAndFix()
    {
        var testCode =
        @"using System;

class Program
{
    [|static void Main() { Console.WriteLine(4); }|]
}";

        var fixedCode =
        @"using System;

class Program
{
    static void Main() => Console.WriteLine(4);
}";

        await VerifyCS.VerifyCodeFixAsync(testCode, fixedCode);
    }
}
