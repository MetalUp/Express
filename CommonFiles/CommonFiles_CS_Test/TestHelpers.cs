namespace CommonFiles_CS_Test
{
    [TestClass]
    public class TestHelpers
    {
        [TestMethod]
        public void TestDisplay()
        {
            Assert.AreEqual("\"Foo\"", Display("Foo"));
            Assert.AreEqual("1", Display(1));
            Assert.AreEqual("3.3", Display(3.3));
            Assert.AreEqual("true", Display(true));
            Assert.AreEqual("\n{1, 2, 3}", Display(new List<int> {1,2,3}));
            Assert.AreEqual("\n{\"foo\", \"bar\"}", Display(new List<string> { "foo", "bar" }));
            Assert.AreEqual("\n{\n{1, 2, 3}, \n{4, 5}}", Display(new List<List<int>> { new List<int> { 1, 2, 3 }, new List<int> { 4,5 } }));
            Assert.AreEqual("(\"foo\", \"bar\")", Display(("foo","bar")));
        }
    }
}