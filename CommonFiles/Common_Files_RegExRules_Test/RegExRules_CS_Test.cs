namespace CommonFiles_CS_Test;

[TestClass]
public class RegExRules_CS_Test
{
    private const string CS = @"Common_Files_CS\Functions_regex.json"; //Appropriate relative path

    [TestMethod]
    public void ValidExpression()
    {
        IsValidExp(CS, "3+4");
    }

    [TestMethod]
    public void AssigningVariable()
    {
        IsInvalidExp(CS, "var a = 1", "Use of the keyword 'var' is not permitted");
    }
    #region Valid code
    [TestMethod]
    public void CanonicalFunction()
    {
        IsValidCode(CS, "static int Foo(int x) => x*x;");
    }

    [TestMethod]
    public void WithSpaces()
    {
        IsValidCode(CS, "  static  int Foo (int x)   => x*x;\n  ");
    }

    [TestMethod]
    public void BreaksOverLine1()
    {
        IsValidCode(CS, "static int Foo(int x) \n=> x*x;");
    }

    [TestMethod]
    public void BreaksOverLine2()
    {
        IsValidCode(CS, "static int Foo(int x) \n=> \nx*x;");
    }

    [TestMethod]
    public void TwoFunctionsWithSpaces()
    {
        IsValidCode(CS, "static int Foo(int x) => x*x;\n  static int Bar(int x) => x*x;\n");
    }
    #endregion

    #region Invalid Code
    [TestMethod]
    public void CodeUsingReturn()
    {
        IsInvalidCode(CS, "static int Foo(int x) { return x*x; }", "Use of the keyword 'return' is not permitted");
    }

    //[TestMethod]
    //public void NonStaticFunction()
    //{
    //    IsInvalidCode(CS, "int Foo(int x) => x*x;", CsFuncSignatureMsg);
    //}

    //[TestMethod]
    //public void UseOfBraces()
    //{
    //    IsInvalidCode(CS, "int Foo(int x) { return x*x;}", CsFuncSignatureMsg);
    //}

    //[TestMethod]
    //public void SecondFunctionInvalid()
    //{
    //    IsInvalidCode(CS, "static int Foo(int x) => x*x;\n int Bar(int x) => x*x;\n", CsFuncSignatureMsg);
    //}
    #endregion
}