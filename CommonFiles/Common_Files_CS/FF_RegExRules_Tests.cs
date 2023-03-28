using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Regex_Rules
{
    [TestClass]
    public class R
    {
        const string multipleFunctions = $@"^[\n\s]*(?:static.*=>(?:[^;{{}}]|\n)*;\s*)*$"; //use of double braces to make literal for this test only
       
        [TestMethod]
        public void TestMultipleFunctions()
        {
            AssertMultiLineMatch("static int Foo(int a) => a*3;", multipleFunctions);
            AssertMultiLineMatch("static int Foo(int a)  =>  a * 3  ;  \n  ", multipleFunctions);
            AssertMultiLineMatch("static int Foo(int a) => a*3;\n  \nstatic int Bar(int a) => a*4;\n  \nstatic int Yon(int a) => a*4;", multipleFunctions);

            AssertMultiLineDoesNotMatch("static int Foo(int a) => a*3;\nxxx", multipleFunctions);
        }      
        
#region Help
        private void AssertMultiLineMatch(string input, string pattern )
        {
            var match = Regex.Match(input, pattern, RegexOptions.Multiline);
            Assert.IsTrue(match.Success);
            Assert.AreEqual(input, match.Value);
        }

        private void AssertMultiLineDoesNotMatch(string input, string pattern)
        {
            //var whole = $"^{pattern}$";
            var match = Regex.Match(input, pattern, RegexOptions.Multiline);
            Assert.AreNotEqual(input, match.Value);
        }
        #endregion
    }
}