namespace CommonFiles_CS_Test
{
    [TestClass]
    public class RegExRules_TypedPY_Test
    {
        const string PY = @"..\Common_Files_PY\Functions_regex.json"; //Appropriate relative path

        [TestMethod]
        public void ValidExpression()
        {
          //  Assert.IsNull(ValidateExpression(PY, "3+4"));
        }

        [TestMethod]
        public void ExpressionUsingAssignment()
        {
           // Assert.AreEqual("Assignment not allowed",//Illustration only - Not real message
           // ValidateExpression(PY, " a = 1"));
        }

        [TestMethod]
        public void ValidCode()
        {
           // Assert.IsNull(ValidateCode(PY, "def foo(x: int) -> int : return x*x"));
        }

        [TestMethod]
        public void FunctionWithoutDef()
        {
          //  Assert.AreEqual("Function definition must start with 'def'",//Illustration only - Not real message
          //  ValidateCode(PY, "foo(x: int)-> int : return x * x"));
        }
    }
}