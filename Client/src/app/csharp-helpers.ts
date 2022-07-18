import { defaultRegExp, filterCmpinfoWithRegex, findFunctionsWithRegex, validateExpressionWithRegex } from "./language-helpers";

export function wrapCSharpExpression(expression : string) {
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

const cmpInfoRegex = /CS.*/

export function filterCSharpCmpinfo(cmpinfo : string) {
    return filterCmpinfoWithRegex(cmpinfo, cmpInfoRegex);
}

export function findCSharpFunctions(expression : string) {
    const fMatch = /([A-Z]\w*\s*)\(/g;
    return findFunctionsWithRegex(expression, fMatch);
}