using CompileServer.Controllers;
using CompileServer.Models;
using CompileServer.Workers;
using static CompileServerTest.TestHelpers;

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

    private static string JavaVersion = "17.0.4.1";

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
        const string localDir = @"C:\Program Files\Java\jdk-17.0.4.1";
        const string appveyorDir = @"C:\Program Files\Java\jdk17";

        if (Directory.Exists(appveyorDir)) {
            CompileServerController.JavaPath = appveyorDir;
            JavaVersion = "17.0.1";
        }
        else {
            CompileServerController.JavaPath = localDir;
        }
    }

    [TestMethod]
    public void TestVersion() {
        var csv = JavaCompiler.GetNameAndVersion();

        Assert.AreEqual("java", csv[0]);
        Assert.AreEqual(JavaVersion, csv[1]);
    }

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = JavaRunSpec(SimpleCode);
        var (rr, file) = JavaCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.Ok);

        Assert.AreEqual("temp", file);
    }

    [TestMethod]
    public void TestCompileFailMissingSemiColon() {
        var runSpec = JavaRunSpec(MissingSC);
        var (rr, file) = JavaCompiler.Compile(runSpec);

        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"{Path.GetTempPath()}temp.java:3:error:';'expectedinta=1^1error");

        Assert.AreEqual("temp", file);
    }
}