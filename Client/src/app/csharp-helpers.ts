import { defaultRegExp, validateExpressionWithRegex } from "./language-helpers";

export function wrapCSharpEXpression(expression : string) {
    return `
    class Hello {
        static void Main(string[] args) {
           System.Console.WriteLine(${expression});
       }
    }`;
}

export function validateCSharpExpression(expression : string) {
    return validateExpressionWithRegex(expression, defaultRegExp);
}