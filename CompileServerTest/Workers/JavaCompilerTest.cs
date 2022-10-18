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
        var runSpec = Helpers.JavaRunSpec(SimpleCode);
        var (rr, code) = JavaCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.Ok);

        Assert.AreEqual("temp", code);
    }

    [TestMethod]
    public void TestCompileFailMissingSemiColon()
    {
        var runSpec = Helpers.JavaRunSpec(MissingSC);
        var (rr, code) = JavaCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.CompilationError, "C:\\Users\\scasc\\AppData\\Local\\Temp\\temp.java:3: error: ';' expected\r\n           int a = 1 \r\n                    ^\r\n1 error\r\n");

        Assert.AreEqual("temp", code);
    }
}