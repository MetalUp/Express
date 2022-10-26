using CompileServer.Controllers;
using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using static CompileServerTest.TestHelpers;

namespace CompileServerTest.Workers;

[TestClass]
public class JavaCompilerTest {
    private readonly ILogger testLogger = NullLogger.Instance;


    private const string SimpleCode =
        @"public class temp {
        public static void main(String[] args) {
           int a = 1;
           System.out.print(a);
        }
        }";

    private const string MissingSC =
        @"public class temp {
        public static void main(String[] args) {
           int a = 1 
        }
        }";

    private const string RunTimeFail =
        @"public class temp {
        public static void main(String[] args) {
           int a = Integer.parseInt(""invalid"");
           System.out.print(a);
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
    public void TestCompileAndRunOk() {
        var runSpec = JavaRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1");
    }

    [TestMethod]
    public void TestCompileFailMissingSemiColon() {
        var runSpec = JavaRunSpec(MissingSC);
        var (rr, file) = JavaCompiler.Compile(runSpec);

        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"{Path.GetTempPath()}temp.java:3:error:';'expectedinta=1^1error");

        Assert.AreEqual("temp", file);
    }

    [TestMethod]
    public void TestCompileAndRunFail()
    {
        var runSpec = JavaRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value;

        Assert.IsNotNull(rr);
        rr.stderr = ClearWhiteSpace(rr.stderr);
        rr.AssertRunResult(Outcome.RunTimeError, "", "", @$"Exceptioninthread""main""java.lang.NumberFormatException:Forinputstring:""invalid""	atjava.base/java.lang.NumberFormatException.forInputString(NumberFormatException.java:67)	atjava.base/java.lang.Integer.parseInt(Integer.java:668)	atjava.base/java.lang.Integer.parseInt(Integer.java:786)	attemp.main(temp.java:3)");
    }
}