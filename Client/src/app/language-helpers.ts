import { validateCSharpExpression, wrapCSharp } from "./csharp-helpers";
import { validateJavaExpression, wrapJava } from "./java-helpers";
import { validatePython3Expression, wrapPython3 } from "./python3-helpers";
import { validateVBNetExpression, wrapVBNet } from "./vbnet-helpers";


export function wrap(language : string, code : string) {
    switch (language) {
        case 'csharp' : return wrapCSharp(code);
        case 'java' : return wrapJava(code);
        case 'python3' : return wrapPython3(code);
        case 'vbnet' : return wrapVBNet(code);
        default : return code;
    }
}

export function validateExpression(language : string, code : string) {
    switch (language) {
        case 'csharp' : return validateCSharpExpression(code);
        case 'java' : return validateJavaExpression(code);
        case 'python3' : return validatePython3Expression(code);
        case 'vbnet' : return validateVBNetExpression(code);
        default : return false;
    }
}

export const defaultRegExp = /^.*[^+-]$/;

export function validateExpressionWithRegex(code : string, re : RegExp) {
    return re.test(code);
}