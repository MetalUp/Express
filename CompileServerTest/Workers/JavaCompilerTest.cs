using CompileServer.Controllers;
using CompileServer.Models;
using CompileServer.Workers;

namespace CompileServerTest.Workers;

[TestClass]
public class JavaCompilerTest {
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

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
        const string localDir = "C:\\Program Files\\Java\\jdk-17.0.4.1";
        const string appveyorDir = "C:\\Program Files\\Java\\jdk17";

        CompileServerController.JavaPath = Directory.Exists(appveyorDir) ? appveyorDir : localDir;
    }

    [TestMethod]
    public void TestVersion() {
        var csv = JavaCompiler.GetNameAndVersion();

        Assert.AreEqual("java", csv[0]);
        Assert.AreEqual("17.0.4.1", csv[1]);
    }

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = TestHelpers.JavaRunSpec(SimpleCode);
        var (rr, code) = JavaCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.Ok);

        Assert.AreEqual("temp", code);
    }

    [TestMethod]
    public void TestCompileFailMissingSemiColon() {
        var runSpec = TestHelpers.JavaRunSpec(MissingSC);
        var (rr, code) = JavaCompiler.Compile(runSpec);

        rr.cmpinfo = TestHelpers.ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"{Path.GetTempPath()}temp.java:3:error:';'expectedinta=1^1error");

        Assert.AreEqual("temp", code);
    }
}