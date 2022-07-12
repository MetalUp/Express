import { defaultRegExp, validateExpressionWithRegex } from "./language-helpers";

export function wrapVBNet(code : string) {
    return `
    Module Hello  
    Sub Main()  
        System.Console.WriteLine(${code})
    End Sub  
    End Module`;
}

export function validateVBNetExpression(code : string) {
    return validateExpressionWithRegex(code, defaultRegExp);
}
