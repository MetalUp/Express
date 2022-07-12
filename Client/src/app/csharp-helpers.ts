import { defaultRegExp, validateExpressionWithRegex } from "./language-helpers";

export function wrapCSharp(code : string) {
    return `
    class Hello {
        static void Main(string[] args) {
           System.Console.WriteLine(${code});
       }
    }`;
}

export function validateCSharpExpression(code : string) {
    return validateExpressionWithRegex(code, defaultRegExp);
}