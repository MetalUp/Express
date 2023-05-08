using static MetalUp.Express.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static System.Math;
using System.Runtime.CompilerServices;
using System.Text;

namespace MetalUp.Express
{
    public class Wrapper
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine(Display("<Expression>"));
        }
//COMMENTS BELOW MUST BE OUTDENTED TO MAX

// StudentCode - this comment is needed error line number offset
//<StudentCode>
//<HiddenCode>
//<Tests>
    }

    public static class Helpers
    {
        public static string Display(object obj)
        {
            if (obj is null) return "";
            if (obj is string) return $@"""{obj}""";
            if (obj is Boolean) return (Boolean)obj ? "true" : "false";
            if (obj is ITuple)
            {
                var tuple = (ITuple)obj;
                return Enumerable.Range(0, tuple.Length)
                    .Aggregate("(", (s, i) => s + Display(tuple[i]) + (i < (tuple.Length - 1) ? ", " : ")"));
            }
            var type = obj.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var toDisplay = ((IEnumerable)obj).Cast<object>().Select(o => Display(o)).ToList();
                return $"\n{{{string.Join(", ", toDisplay)}}}";
            }
            if (obj is IEnumerable)
            {
                return "Result is an IEnumerable. Convert to List to display contents";
            }
            return obj.ToString();
        }

        public static string ArgString(params object[] arguments) => arguments.Aggregate("", (s, a) => s + Display(a) + ", ").TrimEnd(' ', ',');

        public static string FailMessage(string functionName, object expected, object actual, params object[] args) =>
             $"xxxTest failed calling {functionName}({ArgString(args)}) Expected: {Display(expected)} Actual: {Display(actual)}xxx";

        public static bool EqualIfRounded(double expected, double actual) =>
            expected == Round(actual, expected.ToString().Length - expected.ToString().IndexOf(".") - 1);
        public static List<T> SetItem<T>(this List<T> list, int i, T value) =>
                list.Take(i).Append(value).Concat(list.Skip(i + 1)).ToList();

            public static List<T> InsertItem<T>(this List<T> list, int i, T value) =>
                list.Take(i).Append(value).Concat(list.Skip(i)).ToList();

            public static List<T> RemoveItem<T>(this List<T> list, int i) =>
                list.Take(i).Concat(list.Skip(i+1)).ToList();
    }
}