import { defaultRegExp, filterCmpinfoWithRegex, validateExpressionWithRegex } from "./language-helpers";

export function wrapVBNetExpression(expression : string) {
    return `
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
