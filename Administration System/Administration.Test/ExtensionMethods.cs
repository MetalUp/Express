namespace Test;

public static class ExtensionMethods {
    public static Menus AssertMenuOrder(this Menus? menus, params string[] menusNames) {
        Assert.IsNotNull(menus);
        var names = menus.GetValue().Select(l => l.GetTitle()).ToArray();

        Assert.AreEqual(menusNames.Length, names.Count(), "Menu count does not match names count");

        var zip = menusNames.Zip(names);

        foreach (var (first, second) in zip) {
            Assert.AreEqual(first, second);
        }

        return menus;
    }

    public static DomainObject AssertMemberOrder(this DomainObject? obj, params string[] memberNames) {
        Assert.IsNotNull(obj);
        var names = obj.GetActions().Select(a => a.GetId()).ToArray();

        Assert.AreEqual(memberNames.Length, names.Count(), "member count does not match names count");

        var zip = memberNames.Zip(names);

        foreach (var (first, second) in zip) {
            Assert.AreEqual(first, second);
        }

        return obj;
    }

    public static async Task<ActionMember> AssertNumberOfParameters(this ActionMember? am, int numberOfParameters) {
        Assert.IsNotNull(am);
        Assert.AreEqual(numberOfParameters, (await am.GetParameters()).Parameters().Count(), "Unexpected number of parameters");
        return am;
    }

    public static ActionMember AssertReturnsList(this ActionMember? am) {
        Assert.IsNotNull(am);
        Assert.AreEqual("list", am.SafeGetExtension(ExtensionsApi.ExtensionKeys.returnType));
        return am;
    }
}