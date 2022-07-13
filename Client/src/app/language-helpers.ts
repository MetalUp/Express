import { filterCSharpCmpinfo, validateCSharpExpression, wrapCSharpEXpression as wrapCSharpExpression } from "./csharp-helpers";
import { filterJavaCmpinfo, validateJavaExpression, wrapJavaEXpression as wrapJavaExpression } from "./java-helpers";
import { filterPython3Cmpinfo, validatePython3Expression, wrapPython3Expression } from "./python3-helpers";
import { filterVBNetCmpinfo, validateVBNetExpression, wrapVBNetExpression } from "./vbnet-helpers";


export function wrapExpression(language : string, expression : string) {
    switch (language) {
        case 'csharp' : return wrapCSharpExpression(expression);
        case 'java' : return wrapJavaExpression(expression);
        case 'python3' : return wrapPython3Expression(expression);
        case 'vbnet' : return wrapVBNetExpression(expression);
        default : return expression;
    }
}

export function validateExpression(language : string, expression : string) {
    switch (language) {
        case 'csharp' : return validateCSharpExpression(expression);
        case 'java' : return validateJavaExpression(expression);
        case 'python3' : return validatePython3Expression(expression);
        case 'vbnet' : return validateVBNetExpression(expression);
        default : return false;
    }
}

export function filterCmpinfo(language : string, cmpinfo : string) {
    switch (language) {
        case 'csharp' : return filterCSharpCmpinfo(cmpinfo);
        case 'java' : return filterJavaCmpinfo(cmpinfo);
        case 'python3' : return filterPython3Cmpinfo(cmpinfo);
        case 'vbnet' : return filterVBNetCmpinfo(cmpinfo);
        default : return cmpinfo;
    }
}


export const defaultRegExp = /^.*$/;

export function validateExpressionWithRegex(expression : string, re : RegExp) {
    return re.test(expression);
}

export function filterCmpinfoWithRegex(cmpinfo : string, re : RegExp) {
    var m = re.exec(cmpinfo);
    return  m ? m[0] : cmpinfo;
}