using CompileServer.Controllers;
using CompileServer.Models;
using CompileServer.Workers;

namespace CompileServerTest.Workers;

[TestClass]
public class JavaCompilerTest {

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
        CompileServerController.JavaPath = "C:\\Program Files\\Java\\jdk-17.0.4.1";
    }


    private const string SimpleCode =
        @"public class temp {
        public static void main(String[] args) {
           int a = 1;
        }
        }";

    private const string MissingSC =
        @"public class temp {
        public static void main(String[] args) {
           int a = 1 
        }
        }";

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = TestHelpers.JavaRunSpec(SimpleCode);
        var (rr, code) = JavaCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.Ok);

        Assert.AreEqual("temp", code);
    }

    [TestMethod]
    public void TestCompileFailMissingSemiColon()
    {
        var runSpec = TestHelpers.JavaRunSpec(MissingSC);
        var (rr, code) = JavaCompiler.Compile(runSpec);

        rr.cmpinfo = TestHelpers.ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"{Path.GetTempPath()}temp.java:3:error:';'expectedinta=1^1error");

        Assert.AreEqual("temp", code);
    }
}