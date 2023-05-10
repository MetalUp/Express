namespace CommonFiles_CS_Test
{
    [TestClass]
    public class RegExRules_VB_Test
    {
        const string VB = @"..\Common_Files_VB\Functions_regex.json"; //Appropriate relative path to RegEx rules .json file

        [TestMethod]
        public void ValidExpression()
        {
           // Assert.IsNull(VB, "3+4");
        }

        [TestMethod]
        public void ExpressionUsingDim()
        {
          //  Assert.AreEqual("Use of var not allowed", ValidateExpression(VB, "Dim a = 1") ); //Illustration only - Not real message
        }

        [TestMethod]
        public void ValidCode()
        {
          //  Assert.IsNull(ValidateCode(VB,
//@"Function Foo(x As Integer) As Integer
//  Return x
//End Function"));
        }

        [TestMethod]
        public void FunctionWithoutKeyword()
        {
          //  Assert.AreEqual("Functions definitions must start with 'Function'", //Illustrative only - message not correct
//                ValidateCode(VB,
//@"Foo(x As Integer) As Integer
//  Return x
//End Function"));
        }
    }
}