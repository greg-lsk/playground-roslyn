using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = SwitchExpressionAnalyzer.Test.CSharpCodeFixVerifier<
    SwitchExpressionAnalyzer.SwitchExpressionAnalyzer,
    SwitchExpressionAnalyzer.SwitchExpressionCodeFixProvider>;

namespace SwitchExpressionAnalyzer.Test
{
    [TestClass]
    public class RoslynSwitchExpressionUnitTest
    {
        [TestMethod]
        public async Task ExpressionBody_NoDiagnostic()
        {
            var test = 
            @"public class StreetValidationStrategy : EvaluationStrategy<string>
            {
                public override Delegate Run(EvaluationAttribute evaluation) => evaluation switch 
                {
                  NotNull<string> => async Task (string subject) => await Console.WriteLine($""{subject} is null""),
                  IsNotWhitespace => WhenWhitespace,
                  ArbitraryMaxLength => OnLengthExceed
                };
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
