using CompileServer.Models;
using CompileServer.Workers;

namespace CompileServerTest.Workers;

[TestClass]
public class VisualBasicCompilerTest {
    private const string SimpleCode =
        @"Module Program
          Sub Main(args As String())       
            Dim a = 1 
            End Sub 
          End Module";

    private const string DivZero =
        @"Module Program
          Sub Main(args As String())       
            Dim a = 1 \ 0
            End Sub 
          End Module";

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = TestHelpers.VisualBasicRunSpec(SimpleCode);
        var (rr, code) = VisualBasicCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.Ok);

        Assert.AreEqual(2560, code.Length);
    }

    [TestMethod]
    public void TestCompileFailDivisionByZero()
    {
        var runSpec = TestHelpers.VisualBasicRunSpec(DivZero);
        var (rr, code) = VisualBasicCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.CompilationError, "(3) : error BC30542: Division by zero occurred while evaluating this expression.");
        Assert.AreEqual(0, code.Length);
    }
}