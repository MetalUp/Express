import { Run, Reset, ClearSystem, GetRegister, GetN, GetZ, GetC, GetV, GetMemory, GetPixel, GetPixels, GetConsoleOutput, InputText, HitKey, GetMemoryRange } from 'armlite_service';
import { EmptyRunResult, RunResult } from './run-result';

class TestError extends Error {
    constructor(message : string) {
        super(message);
    }
}

export class ArmTestHelper {

    constructor(private studentCode: string) { }

    static runTests(tests: string, studentCode: string) {
        var rr = { ...EmptyRunResult };
        var helper = new ArmTestHelper(studentCode);
    
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
        return Run(maxSteps) as RunResult;
    }

    Reset(): RunResult {
        return Reset() as RunResult;
    }

    ClearSystem(): RunResult {
        return ClearSystem() as RunResult;
    }

    GetRegister(n: number) {
        return GetRegister(n);
    }

    GetN(): boolean {
        return GetN() as boolean;
    }

    GetZ(): boolean {
        return GetZ() as boolean;
    }

    GetC(): boolean {
        return GetC() as boolean;
    }

    GetV(): boolean {
        return GetV() as boolean;
    }

    GetMemory(loc: number) {
        return GetMemory(loc);
    }

    GetPixel(addr: number) {
        return GetPixel(addr);
    }

    InputText(text: string) {
        return InputText(text);
    }

    HitKey(keyValue: number) {
        return HitKey(keyValue);
    }

    GetPixels(low: number, high: number) {
        return GetPixels(low, high);
    }

    GetMemoryRange(low: number, high: number) {
        return GetMemoryRange(low, high);
    }

    GetConsoleOutput(): string {
        return GetConsoleOutput() as string;
    }

    GetNonEmptyLinesOfCode() {
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
            throw new TestError(`Line of code: ${atLineNo} should contain '${snippet}'`)
        }
    }

    private AssertLineOfCodeDoesNotContains(snippet: string, atLineNo: number) {
        const loc = this.GetNonEmptyLinesOfCode()[atLineNo -1];
        if (loc.toUpperCase().includes(snippet.toUpperCase())) {
            throw new TestError(`Line of code: ${atLineNo} should not contain '${snippet}'`)
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
                this.AssertLineOfCode(snippet, atLineNo, contains)
            }
            else {
                throw new TestError(`No code at offset ${atLineNo} code length: ${instr.length}`)
            }
        }
        else {
            for (let n = 1; n <= instr.length; n++) {
                this.AssertLineOfCode(snippet, n, contains)
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
        var actual = this.GetRegister(number);
        if (actual != expected)
            throw new TestError(`R${number}: Expected ${expected} Actual ${actual}`);
    }

    AssertPixel(addr: number, expected: number) {
        var actual = this.GetPixel(addr);
        if(actual != expected)
            throw new TestError(`Pixel ${addr}: Expected ${expected} Actual ${actual}`);
    }

    AssertMemory(address: number, expected: number) {
        var actual = this.GetMemory(address);
        if (actual != expected)
            throw new TestError(`Memory Location ${address}: Expected ${expected} Actual ${actual}`);
    }
}