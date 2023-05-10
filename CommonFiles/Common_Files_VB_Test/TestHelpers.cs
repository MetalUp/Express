using static MetalUp.Express.Helpers;

namespace CommonFiles_VB_Test
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
            var actual = Display(new List<int> { 1, 2, 3 });
            Assert.AreEqual("\n{1, 2, 3}", actual );
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
            Assert.AreEqual("3, \"foo\", (1, \"bar\"), \n{1, 2, 3}", ArgString(3, "foo", (1, "bar"), new List<int> { 1, 2, 3 }));
        }
        #endregion
    }
}