namespace CommonFiles_CS_Test
{
    [TestClass]
    public class RegExRules_CS_Test
    {
        const string CS = @"..\Common_Files_CS\Functions_regex.json"; //Appropriate relative path

        [TestMethod]
        public void ValidExpression()
        {
           //Assert.IsNull(ValidateExpression(CS, "3+4"));
        }

        [TestMethod]
        public void ExpressionUsingVar()
        {
           // Assert.AreEqual("Use of of var not allowed", 
               // ValidateExpression(CS, "var a = 1")); //Illustration only - Not real message
        }

        [TestMethod]
        public void ValidCode()
        {
           // Assert.IsNull(ValidateCode(CS, "static int Foo(int x) => x*x;"));
        }

        [TestMethod]
        public void NonStaticFunction()
        { 
           // Assert.AreEqual("Function definitions must start with 'static'",
            //    ValidateCode(CS,"int Foo(int x) => x*x;")); //Illustration only - Not real message
        }

    }
}