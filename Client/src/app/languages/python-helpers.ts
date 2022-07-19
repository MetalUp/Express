import { defaultRegExp, filterCmpinfoWithRegex, findFunctionsWithRegex, validateExpressionWithRegex } from "./language-helpers";

export function wrapPythonExpression(expression : string) {
    return `
from math import *

def display(x):
  if isinstance(x, str):
    return x
  else:
    try:
         return list(iter(x))
    except TypeError:
         return x

print (display(${expression}))
`;
}

export function validatePythonExpression(expression : string) {
    return validateExpressionWithRegex(expression, defaultRegExp);
}

const cmpInfoRegex = /SyntaxError.*/

export function filterPythonCmpinfo(cmpinfo : string) {
    return filterCmpinfoWithRegex(cmpinfo, cmpInfoRegex);
}

export function findPythonFunctions(expression : string) {
    const fMatch = /([A-Za-z]\w*\s*)\(/g;
    return findFunctionsWithRegex(expression, fMatch);
}