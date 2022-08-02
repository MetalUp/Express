using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public static class Framework
    {
        public static string Display(object obj)
        {
            if (obj == null)  return null;
            if (obj is string) return $"{obj}";
            if (obj is Boolean) return (Boolean) obj ? "true" : "false";
            if (obj is IEnumerable)
            {
                var display = ((IEnumerable)obj).Cast<object>().Select(o => Display(o));
                return $@"{{{string.Join(',', display)}}}";
            }
            return obj.ToString();
        }

        public static string fail = "Test failed calling ";

        public static string ArgString(params object[] arguments) => arguments.Aggregate("", (s, a) => s + Display(a) + ", ").TrimEnd(' ', ',');

        public static void TestFunction(string functionName, object expected, object actual, params object[] args)
        {
            if (Display(actual) != Display(expected))
            {
                Console.WriteLine(fail + $"{functionName}({ArgString(args)}) Expected: {Display(expected)}  Actual: {Display(actual)}");
                throw new TestException();
            }
        }

        public static void AssertTrue(string functionName, string args, bool actual, string message)
        {
            if (actual != true)
            {
                Console.WriteLine(fail +$"{functionName}({ArgString(args)}) {message}");
                throw new TestException();
            }
        }

        public static void AllTestsPassed(string function)
        {
            Console.WriteLine($"All tests passed on function: {function}");
        }

        public class TestException : Exception { }

    }
}