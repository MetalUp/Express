using static System.Math;
namespace CommonFiles_CS_Test
{
    [TestClass]
    public class TestHelpers
    {
        #region Display
        [TestMethod]
        public void TestDisplayString()
        {
            Assert.AreEqual("\"Foo\"", Display("Foo"));
        }

        [TestMethod]
        public void TestDisplayInt()
        {
            Assert.AreEqual("1", Display(1));
        }

        [TestMethod]
        public void TestDisplayDouble()
        {
            Assert.AreEqual("3.3", Display(3.3));
        }

        [TestMethod]
        public void TestDisplayBool()
        {
            Assert.AreEqual("true", Display(true));
        }
 
        [TestMethod]
        public void TestDisplayListOfInt()
        {
            Assert.AreEqual("\n{1, 2, 3}", Display(new List<int> { 1, 2, 3 }));
        }
 
        [TestMethod]
        public void TestDisplayListOfString()
        {
            Assert.AreEqual("\n{\"foo\", \"bar\"}", Display(new List<string> { "foo", "bar" }));
        }
 
        [TestMethod]
        public void TestDisplayListOfLists()
        {
            Assert.AreEqual("\n{\n{1, 2, 3}, \n{4, 5}}", Display(new List<List<int>> { new List<int> { 1, 2, 3 }, new List<int> { 4, 5 } }));
        }

        [TestMethod]
        public void TestDisplayTuplet()
        {
            Assert.AreEqual("(\"foo\", \"bar\")", Display(("foo", "bar")));
        }
        #endregion

        #region ArgString
        [TestMethod]
        public void TestArgString()
        {
            Assert.AreEqual("3, \"foo\", (1, \"bar\"), \n{1, 2, 3}", ArgString(3, "foo", (1, "bar"), new List<int> {1,2,3 }));
        }
        #endregion

        #region FailMessage
        [TestMethod]
        public void FailMessageTest()
        {
            Assert.AreEqual("xxxTest failed calling Foo(3, 4) Expected: 1 Actual: 2xxx", FailMessage("Foo", 1, 2, 3, 4));
        }
        #endregion

        #region EqualIfRounded
        [TestMethod]
        public void EqualIfRoundedTest()
        {
            Assert.IsTrue(EqualIfRounded(3.456, 3.4562789));
            Assert.IsFalse(EqualIfRounded(3.4562, 3.4562789));
        }
        #endregion

        #region non-mutating list extension methods
        [TestMethod]
        public void SetItem()
        {
            var input = new List<int> { 1, 2, 3, 4, 5 };
            var expected = new List<int> { 1, 2, 6, 4, 5 };
            CollectionAssert.AreEqual(expected, input.SetItem(2, 6));
        }

        [TestMethod]
        public void InsertItem()
        {
            var input = new List<int> { 1, 2, 3, 4, 5 };
            var expected = new List<int> { 1, 2, 6, 3, 4, 5 };
            CollectionAssert.AreEqual(expected, input.InsertItem(2, 6));
        }

        [TestMethod]
        public void RemoveItem()
        {
            var input = new List<int> { 1, 2, 3, 4, 5 };
            var expected = new List<int> { 1, 2, 4, 5 };
            CollectionAssert.AreEqual(expected, input.RemoveItem(2));
        }
        #endregion
    }
}