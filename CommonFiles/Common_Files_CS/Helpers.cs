﻿
public static class Helpers
{
    public static string Display(object obj)
    {
        if (obj == null) return null;
        if (obj is string) return $"{obj}";
        if (obj is Boolean) return (Boolean)obj ? "true" : "false";
        if (obj is IEnumerable)
        {
            var display = ((IEnumerable)obj).Cast<object>().Select(o => Display(o));
            return $@"{{{string.Join(',', display)}}}";
        }
        return obj.ToString();
    }

    public static string ArgString(params object[] arguments) => arguments.Aggregate("", (s, a) => s + Display(a) + ", ").TrimEnd(' ', ',');

    public static string FailMessage(string functionName, object expected, object actual, params object[] args) =>
         $"xxxTest failed calling {functionName}({ArgString(args)}) Expected: {Display(expected)} Actual: {Display(actual)}xxx";

    public static string QAFailMessage(string wrongAnswers) =>
        $"xxxWrong or missing answer(s) to question number(s): {wrongAnswers}.xxx";

    public static string WrongAnswers(string student, string answers)
    {
        var studentArr = AsArray(student);
        var answersArr = AsArray(answers);
        var result = "";
        for (int n = 0; n < answersArr.Length; n++)
        {
            if (answersArr[n] != null && studentArr[n] != answersArr[n]) result += $"{n} ";
        }
        return result;
    }

    public static string[] AsArray(string answers) {
        var arr = new string[20];
        foreach (var a in answers.Split('\n').Where(a => a.Length > 0))
        {
            var split = a.Split(')');
            var n = AsInt(split[0].Trim());
            arr[n] = split[1].Trim().ToUpper();
        }
        return arr;
    }

    private static int AsInt(string input) =>
         Convert.ToInt32(Encoding.Default.GetString(Encoding.ASCII.GetBytes(input).Where(b => b > 47 && b < 58).ToArray()));

}

