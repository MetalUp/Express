import { wrapCSharpExpression, wrapCSharpFunctions, wrapCSharpTests } from "./csharp-helpers";
import { wrapPythonExpression, wrapPythonFunctions, wrapPythonTests } from "./python-helpers";

export const FunctionPlaceholder = "<<FunctionsPlaceholder>>";
export const ReadyMadeFunctionsPlaceholder = "<<ReadyMadeFunctionsPlaceholder>>";

export function wrapExpression(language: string, expression: string) {
    switch (language) {
        case 'csharp': return wrapCSharpExpression(expression);
        case 'python': return wrapPythonExpression(expression);
        default: return expression;
    }
}

export function wrapFunctions(language: string, expression: string) {
    switch (language) {
        case 'csharp': return wrapCSharpFunctions(expression);
        case 'python': return wrapPythonFunctions(expression);
        default: return expression;
    }
}

export function wrapTests(language: string, tests: string) {
    switch (language) {
        case 'csharp': return wrapCSharpTests(tests);
        case 'python': return wrapPythonTests(tests);
        default: return tests;
    }
}