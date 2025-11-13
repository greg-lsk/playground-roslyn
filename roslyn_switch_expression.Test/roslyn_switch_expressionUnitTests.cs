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
        var test = _expressionBodyCode;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }


    [TestMethod]
    public async Task BlockBody_Diagnostic()
    {
        var test = _methodBodyCode;
        var expected = VerifyCS.Diagnostic().WithLocation(5, 9).WithArguments("static", "void", "Main", "()");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task BlockBody_DiagnosticAndFix()
    {
        var testCode = _methodBodyCode;
        var fixedCode = _expressionBodyCode;
        var expected = VerifyCS.Diagnostic().WithLocation(5, 9).WithArguments("static", "void", "Main", "()");

        await VerifyCS.VerifyCodeFixAsync(testCode, expected, fixedCode);
    }

    private static readonly string _expressionBodyCode =
    @"using System;

    class Program
    {
        static void Main() => Console.WriteLine(4);
    }";

    private static readonly string _methodBodyCode =
    @"using System;

    class Program
    {
        static void Main() { Console.WriteLine(4); }
    }";
}
