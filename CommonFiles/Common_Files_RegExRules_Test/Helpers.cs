using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace RegExRules_Test;

internal static class Helpers
{
    public const string CsFuncSignatureMsg = "All functions should be: static <ReturnType> <NameStartingInUpperCase>(<parametersStartingLowerCase>) => <expression>;";
    public static void IsValidExp(string lang, string exp)
    {
        Assert.IsNull(ValidateExpression(lang, exp));
    }

    public static void IsInvalidExp(string lang, string exp, string msg)
    {
        Assert.AreEqual(msg, ValidateExpression(lang, exp));
    }

    public static void IsValidCode(string lang, string code)
    {
        Assert.IsNull(ValidateCode(lang, code));
    }

    public static  void IsInvalidCode(string lang, string code, string msg)
    {
        Assert.AreEqual(msg, (ValidateCode(lang, code)));
    }

    //Applies appropriate RegExRules to code. If all tests pass, returns null, else returns failure message
    private static string? ValidateCode(string regexFile, string code)
    {
        var (mustMatchRules, mustNotContainRules) = GetRules(regexFile, "functions");
        return ApplyRules(code, mustMatchRules, mustNotContainRules);
    }

    //Applies appropriate RegExRules to expression. If all tests pass, returns null, else returns failure message
    private static string? ValidateExpression(string regexFile, string expression)
    {
        var (mustMatchRules, mustNotContainRules) = GetRules(regexFile, "expressions");
        return ApplyRules(expression, mustMatchRules, mustNotContainRules);
    }

    private static string? ApplyRules(string code, IEnumerable<JArray> mustMatchRules, IEnumerable<JArray> mustNotContainRules)
    {
        foreach (var rule in mustMatchRules)
        {
            var pattern = rule[0].Value<string>();
            var msg = rule[1].Value<string>();

            if (!Regex.Match(code, pattern).Success)
            {
                return msg;
            }
        }

        foreach (var rule in mustNotContainRules)
        {
            var pattern = rule[0].Value<string>();
            var msg = rule[1].Value<string>();

            var matches = Regex.Match(code, pattern);

            if (matches.Success)
            {
                for (var i = 0; i < matches.Groups.Count; i++)
                {
                    msg = msg.Replace($"{{{i}}}", matches.Groups[i].ToString());
                }

                return msg;
            }
        }

        return null;
    }

    private static (IEnumerable<JArray>, IEnumerable<JArray>) GetRules(string regexFile, string ruleType)
    {
        var rootDir = Directory.GetCurrentDirectory().Replace(@"\Common_Files_RegExRules_Test\bin\Debug\net6.0", "");
        var path = Path.Combine(rootDir, regexFile);

        var json = File.ReadAllText(path);
        var asJson = JObject.Parse(json);
        var mustMatch = asJson["CodeMustMatch"];
        var mustNotContain = asJson["CodeMustNotContain"];

        var mustMatchRules = GetRules(ruleType, mustMatch).Union(GetRules("both", mustMatch));
        var mustNotContainRules = GetRules(ruleType, mustNotContain).Union(GetRules("both", mustNotContain));
        return (mustMatchRules, mustNotContainRules);
    }

    private static IEnumerable<JArray> GetRules(string ruleType, JToken mustMatch) => ((JArray)mustMatch[ruleType]).Cast<JArray>();
}