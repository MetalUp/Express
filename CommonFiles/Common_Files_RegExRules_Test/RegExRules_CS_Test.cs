namespace CommonFiles_CS_Test;

[TestClass]
public class RegExRules_CS_Test
{
    private const string CS = @"Common_Files_CS\Functions_regex.json"; //Appropriate relative path

    [TestMethod]
    public void ValidExpression()
    {
        Assert.IsNull(ValidateExpression(CS, "3+4"));
    }

    [TestMethod]
    public void ExpressionUsingVar()
    {
        Assert.AreEqual("Use of the keyword 'var' is not permitted", ValidateExpression(CS, "var a = 1"));
    }

    [TestMethod]
    public void ValidCode()
    {
        Assert.IsNull(ValidateCode(CS, "static int Foo(int x) => x*x;"));
    }

    [TestMethod]
    public void CodeUsingReturn()
    {
        Assert.AreEqual("Use of the keyword 'return' is not permitted", ValidateCode(CS, "static int Foo(int x) { return x*x; }"));
    }

    [TestMethod]
    public void NonStaticFunction()
    {
        Assert.AreEqual("All functions should be: static <ReturnType> <NameStartingInUpperCase>(<parametersStartingLowerCase>) => <expression>;", ValidateCode(CS, "int Foo(int x) => x*x;"));
    }
}