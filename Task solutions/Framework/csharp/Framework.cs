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


        public static void TestFunction(string functionName, object expected, object actual, params object[] arguments)
        {
            var argString = arguments.Aggregate("", (s, a) => s + Display(a) + ", ").TrimEnd(' ', ',');
            if (Display(actual) != Display(expected))
            {
                Console.WriteLine($"Test failed calling function {functionName}({argString}) Expected: {Display(expected)}  Actual: {Display(actual)}");
                throw new TestException();
            }
        }

        public static void AssertTrue(string functionName, bool actual, string message)
        {
            if (actual != true)
            {
                Console.WriteLine($"Test failed calling function {functionName} {message}");
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