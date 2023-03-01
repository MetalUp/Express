namespace Test;

public static class ExtensionMethods {
    public static Menus AssertMenuOrder(this Menus? menus, params string[] menusNames) {
        Assert.IsNotNull(menus);
        var names = menus.GetLinks().Select(l => l.GetTitle()).ToArray();

        Assert.AreEqual(names.Count(), menusNames.Length, "an error");

        var zip = names.Zip(menusNames);

        foreach (var (first, second) in zip) {
            Assert.AreEqual(first, second);
        }

        return menus;
    }

    public static Menus AssertMemberOrder(this DomainObject? obj, params string[] memberNames) {
        Assert.IsNotNull(obj);
        var names = menus.GetLinks().Select(l => l.GetTitle()).ToArray();

        Assert.AreEqual(names.Count(), menusNames.Length, "an error");

        var zip = names.Zip(menusNames);

        foreach (var (first, second) in zip) {
            Assert.AreEqual(first, second);
        }

        return obj;
    }
}