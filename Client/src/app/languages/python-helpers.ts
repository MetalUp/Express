import { FunctionPlaceholder } from "./language-helpers";

export function wrapPythonExpression(expression : string) {
    return `
from math import *
from datetime import date, datetime

${FunctionPlaceholder}

def display(x):
  if isinstance(x, str):
    return str(x)
  else:
    try:
         return list(iter(x))
    except TypeError:
         return str(x)

print (display(${expression}))
`;
}

export function wrapPythonFunctions(functions : string) {
    return functions;
}
