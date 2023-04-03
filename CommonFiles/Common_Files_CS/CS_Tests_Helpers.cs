using Microsoft.VisualStudio.TestTools.UnitTesting;
using static MetalUp.Express.Wrapper_CS;
using System.Collections.Immutable;

[TestClass]
public class CS_Tests_Helpers
{

    [TestMethod]
    public void TestDisplay()
    {
        Assert.AreEqual("", Display(null));
        Assert.AreEqual(@"Hello", Display("Hello"));
        Assert.AreEqual("true", Display(true));
        Assert.AreEqual("false", Display(false));
        var l1 = ImmutableList.Create(1, 2, 3, 4, 5);
        Assert.AreEqual("1,2,3,4,5", Display(l1));
        var l2 = ImmutableList.Create("foo","bar");
        Assert.AreEqual(@"foo","bar", Display(l2));
        var l3 = new List<int> { 1, 2, 3, 4, 5 };
        Assert.AreEqual("Result is an ordinary List. Use ImmutableList only.", Display(l3));
        var en = l3.Select(x => x);
        Assert.AreEqual("Result is an IEnumerable. Convert to ImmutableList to display contents", Display(en));
    }


    //        if (obj == null) return null;
    //if (obj is string) return $"{obj}";
    //if (obj is Boolean) return (Boolean) obj ? "True" : "False";
    //if (obj.GetType().GetGenericTypeDefinition() == typeof(ImmutableList<>))
    //{
    //    var toDisplay = ((IEnumerable)obj).Cast<object>().Select(o => Display(o));
    //    return $@"{string.Join(',', toDisplay)}";
    //}
    //if (obj.GetType().GetGenericTypeDefinition() == typeof(List<>))
    //{
    //    return "Object is an ordinary List. Use ImmutableList only.";
    //}
}
