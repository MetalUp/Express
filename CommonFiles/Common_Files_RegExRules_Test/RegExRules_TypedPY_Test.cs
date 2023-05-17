namespace CommonFiles_CS_Test
{
    [TestClass]
    public class RegExRules_PY_Typed_Test
    {
        //C:\MetalUp\Express\CommonFiles\Common_Files_PY
        const string PY = @"Common_Files_PY\Typed_Functions_regex.json"; //Appropriate relative path

        [TestMethod]
        public void ValidExpression()
        {
            IsValidExp(PY, "3+4");
        }

        [TestMethod]
        public void AssigningVariable()
        {
            IsInvalidExp(PY, "a = 1", "Use of single '=', signifying assignment, is not permitted. (To test for equality, use '==')");
        }
        #region Valid code
        [TestMethod]
        public void CanonicalFunction()
        {
            IsValidCode(PY, "def foo(x: int) -> int: return x*x");
        }

        [TestMethod]
        public void WithSpacesAndLineBreaksBeforeAfter()
        {
            IsValidCode(PY, "\n def foo(x: int) -> int: return x*x\n  ");
        }

        [TestMethod]
        public void BreaksOver2Lines()
        {
            IsValidCode(PY, "def foo(x: int) -> int:\n  return x*x");
        }

        [TestMethod]
        public void BreaksOver3LinesWithBackslash()
        {
            IsValidCode(PY, "def foo(x: int)-> int:\n  return \\x * x");
        }

        [TestMethod]
        public void TwoFunctionsWithSpaces()
        {
            IsValidCode(PY, "def foo(x: int) -> int: return x*x\ndef bar(x: int) -> int: return x*x");
        }
        #endregion

        #region Invalid code

        [TestMethod]
        public void NotPermitted()
        {
            IsInvalidCode(PY, "def foo(x: int) -> int: return print(x)", "Messages.NotPermitted");
        }
        #endregion
    }
}