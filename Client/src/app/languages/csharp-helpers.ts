import {  FunctionPlaceholder, TaskCodePlaceholder } from "./language-helpers";

export function wrapCSharpExpression(expression : string) {
    return `
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    class MainWrapper {
        ${TaskCodePlaceholder}

        private static string Display(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is string)
            {
                return $"{obj}";
            }

            if (obj is IEnumerable)
            {
                var display = ((IEnumerable)obj).Cast<object>().Select(o => Display(o));
                return $@"{{{string.Join(',', display)}}}";
            }

            return obj.ToString();
        }

        static void Main(string[] args) {
           System.Console.WriteLine(Display(${(expression)}));
        }

       ${FunctionPlaceholder}

       static void TestFunction(string functionName, object expected, object actual, params object[] arguments)
       {
           var argString = arguments.Aggregate("", (s, a) => s + Display(a) + ", ").TrimEnd(' ', ',');
           if (Display(actual) != Display(expected))
           {
               Console.WriteLine($"Test Failed calling function {functionName}");
               if (arguments.Length > 0) Console.WriteLine($"with arguments: {argString}");
               Console.WriteLine($"Expected: {Display(expected)}  Actual: {Display(actual)}");
               throw new TestException();
           }
       }

       static void AssertTrue(string functionName, bool actual, string message)
       {
           if (actual != true)
           {
               Console.WriteLine($"Test Failed calling function {functionName}");
               Console.WriteLine(message);
               throw new TestException();
           }
       }

       static void AllTestsPassed(string function)
       {
           Console.WriteLine($"All tests passed on function: {function}");
       }

       class TestException : Exception { }
    }`;
}

export function wrapCSharpFunctions(functions : string) {
    return `
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    
    class MainWrapper{
        ${TaskCodePlaceholder}

        static void Main(string[] args) {}
    }

    static class UserDefinedFunctions {
        ${functions}
    }
    `;
}