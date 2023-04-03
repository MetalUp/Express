import * as ARMlite from 'armlite_service';
import { EmptyRunResult, RunResult } from './run-result';

class TestError extends Error {
    constructor(message : string) {
        super(message);
    }
}

export class ArmTestHelper {

    constructor(private studentCode: string) { }

    static runTests(tests: string, studentCode: string) {
        const rr = { ...EmptyRunResult };
        const helper = new ArmTestHelper(studentCode);
    
        try {
            const runTestsFunction = new Function('helper', tests);
            runTestsFunction(helper);
            rr.stdout = "all tests passed";
        }
        catch (e) {
            if (e instanceof TestError) {
                rr.stdout = e.message;
            }
            else if (e instanceof Error) {
                rr.stderr = `unexpected error: ${e.message}`;
            }
            else {
                rr.stderr = `unexpected error: ${e}`;
            }
        }
        rr.outcome = rr.stderr ? 12 : 15;
        return rr;
    }

    Run(maxSteps: number): RunResult {
        return ARMlite.Run(maxSteps) as RunResult;
    }

    InputText(text: string) {
        return ARMlite.InputText(text);
    }

    HitKey(keyValue: number) {
        return ARMlite.HitKey(keyValue);
    }

    Reset(): RunResult {
        return ARMlite.Reset() as RunResult;
    }

    ClearSystem(): RunResult {
        return ARMlite.ClearSystem() as RunResult;
    }

    GetRegister(n: number) : number {
        return ARMlite.GetRegister(n);
    }

    GetN(): boolean {
        return ARMlite.GetN() as boolean;
    }

    GetZ(): boolean {
        return ARMlite.GetZ() as boolean;
    }

    GetC(): boolean {
        return ARMlite.GetC() as boolean;
    }

    GetV(): boolean {
        return ARMlite.GetV() as boolean;
    }

    GetMemory(loc: number) : number {
        return ARMlite.GetMemory(loc);
    }

    GetMemoryRange(low: number, high: number) : number []{
        return ARMlite.GetMemoryRange(low, high);
    }

    GetPixel(addr: number) : number {
        return ARMlite.GetPixel(addr);
    }

    GetPixels(low: number, high: number) : number[] {
        return ARMlite.GetPixels(low, high);
    }

    GetPixelAreaSize(): number {
        return ARMlite.GetPixelAreaSize();
    }

    GetPixelScreen(): number {
        return ARMlite.GetPixelScreen();
    }

    GetCharScreen(): number {
        return ARMlite.GetCharScreen();
    }

    GetConsoleOutput(): string {
        return ARMlite.GetConsoleOutput() as string;
    }

    GetProgram(): RunResult {
        return ARMlite.GetProgram();
    }

    GetNonEmptyLinesOfCode() : string[] {
       const codeArray = this.studentCode.split('\n');
       return codeArray.filter(l => !(l.trim() === ""));
    }

    AssertAreEqual(expected: string, actual: string, context: string) {
        if(actual != expected)
        throw new TestError(`${context}: Expected ${expected} Actual ${actual}`);
    }

    private AssertLineOfCodeContains(snippet: string, atLineNo: number) {
        const loc = this.GetNonEmptyLinesOfCode()[atLineNo -1];
        if (!loc.toUpperCase().includes(snippet.toUpperCase())) {
            throw new TestError(`Line of code: ${atLineNo} should contain '${snippet}'`);
        }
    }

    private AssertLineOfCodeDoesNotContains(snippet: string, atLineNo: number) {
        const loc = this.GetNonEmptyLinesOfCode()[atLineNo -1];
        if (loc.toUpperCase().includes(snippet.toUpperCase())) {
            throw new TestError(`Line of code: ${atLineNo} should not contain '${snippet}'`);
        }
    }

    private AssertLineOfCode(snippet: string, atLineNo: number, contains: boolean) {
        if (contains) {
            this.AssertLineOfCodeContains(snippet, atLineNo);
        }
        else {
            this.AssertLineOfCodeDoesNotContains(snippet, atLineNo);
        }
    }

    private AssertCode(snippet: string, contains: boolean, atLineNo?: number) {
        const instr = this.GetNonEmptyLinesOfCode();
        if (atLineNo != null) {
            if (instr.length >= atLineNo) {
                this.AssertLineOfCode(snippet, atLineNo, contains);
            }
            else {
                throw new TestError(`No code at offset ${atLineNo} code length: ${instr.length}`);
            }
        }
        else {
            for (let n = 1; n <= instr.length; n++) {
                this.AssertLineOfCode(snippet, n, contains);
            }
        }
    }

    AssertCodeContains(snippet: string, atLineNo?: number) {
        this.AssertCode(snippet, true, atLineNo);
    }
 
    AssertCodeDoesNotContain(snippet: string, atLineNo?: number) {
        this.AssertCode(snippet, false, atLineNo);
    }

    AssertRegister(number: number, expected: number) {
        const actual = this.GetRegister(number);
        if (actual != expected)
            throw new TestError(`R${number}: Expected ${expected} Actual ${actual}`);
    }

    AssertPixel(addr: number, expected: number) {
        const actual = this.GetPixel(addr);
        if(actual != expected)
            throw new TestError(`Pixel ${addr}: Expected ${expected} Actual ${actual}`);
    }

    AssertMemory(address: number, expected: number) {
        const actual = this.GetMemory(address);
        if (actual != expected)
            throw new TestError(`Memory Location ${address}: Expected ${expected} Actual ${actual}`);
    }
}