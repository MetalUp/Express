import { defaultRegExp, filterCmpinfoWithRegex, filterStderrWithRegex, findFunctionsWithRegex, validateExpressionWithRegex } from "./language-helpers";

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

const cmpInfoRegex = /\s\w*Error:.*/

export function filterPythonCmpinfo(cmpinfo : string) {
    return filterCmpinfoWithRegex(cmpinfo, cmpInfoRegex);
}

const stderrRegex = /\s\w*Error:.*/

export function filterPythonStderr(stderr : string) {
    return filterStderrWithRegex(stderr, stderrRegex);
}

export function findPythonFunctions(expression : string) {
    const fMatch = /([A-Za-z]\w*\s*)\(/g;
    return findFunctionsWithRegex(expression, fMatch);
}