import { defaultRegExp, filterCmpinfoWithRegex, findFunctionsWithRegex, validateExpressionWithRegex } from "./language-helpers";

export function wrapVBNetExpression(expression : string) {
    return `
    Imports System
    Imports System.Linq
    
    Module Hello  
    Sub Main()  
        System.Console.WriteLine(${expression})
    End Sub  
    End Module`;
}

export function validateVBNetExpression(expression : string) {
    return validateExpressionWithRegex(expression, defaultRegExp);
}

const cmpInfoRegex = /VBNC.*/

export function filterVBNetCmpinfo(cmpinfo : string) {
    return filterCmpinfoWithRegex(cmpinfo, cmpInfoRegex);
}

export function findVBNetFunctions(expression : string) {
    const fMatch = /([A-Z]\w*\s*)\(/g;
    return findFunctionsWithRegex(expression, fMatch);
}
