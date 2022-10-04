import { wrapCSharpExpression, wrapCSharpFunctions, wrapCSharpTests } from "./csharp-helpers";
import { wrapJavaExpression, wrapJavaFunctions, wrapJavaTests } from "./java-helpers";
import { wrapPythonExpression, wrapPythonFunctions, wrapPythonTests } from "./python-helpers";
import { wrapVBExpression, wrapVBFunctions, wrapVBTests } from "./vb-helpers";

export const UserDefinedFunctionPlaceholder = "<<UserDefinedFunctionPlaceholder>>";
export const ReadyMadeFunctionsPlaceholder = "<<ReadyMadeFunctionsPlaceholder>>";

export function wrapExpression(language: string, expression: string) {
    switch (language) {
        case 'csharp': return wrapCSharpExpression(expression);
        case 'python': return wrapPythonExpression(expression);
        case 'vb': return wrapVBExpression(expression);
        case 'java': return wrapJavaExpression(expression);
        default: return expression;
    }
}

export function wrapFunctions(language: string, expression: string) {
    switch (language) {
        case 'csharp': return wrapCSharpFunctions(expression);
        case 'python': return wrapPythonFunctions(expression);
        case 'vb': return wrapVBFunctions(expression);
        case 'java': return wrapJavaFunctions(expression);
        default: return expression;
    }
}

export function wrapTests(language: string, tests: string) {
    switch (language) {
        case 'csharp': return wrapCSharpTests(tests);
        case 'python': return wrapPythonTests(tests);
        case 'vb': return wrapVBTests(tests);
        case 'java': return wrapJavaTests(tests);
        default: return tests;
    }
}