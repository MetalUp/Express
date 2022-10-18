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
        var runSpec = new RunSpec { language_id = "vb", sourcecode = SimpleCode };
        var (rr, code) = VisualBasicCompiler.Compile(runSpec);

        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.AreEqual("", rr.run_id);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.stdout);

        Assert.AreEqual(2560, code.Length);
    }

    [TestMethod]
    public void TestCompileFailDivisionByZero()
    {
        var runSpec = new RunSpec { language_id = "vb", sourcecode = DivZero };
        var (rr, code) = VisualBasicCompiler.Compile(runSpec);

        Assert.AreEqual(Outcome.CompilationError, rr.outcome);
        Assert.AreEqual("BC30542: Division by zero occurred while evaluating this expression.", rr.cmpinfo);
        Assert.AreEqual("", rr.run_id);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.stdout);

        Assert.AreEqual(0, code.Length);
    }
}