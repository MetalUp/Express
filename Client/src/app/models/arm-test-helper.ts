import { Run, Reset, ClearSystem, GetRegister, GetN, GetZ, GetC, GetV, GetMemory, GetPixel, GetConsoleOutput, InputText, HitKey, GetPixels, GetMemoryRange } from 'armlite_service';
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

    private isActiveInstruction(line: string) {
        const trimmed = line.trim();
        const emptyOrComment = trimmed === "" || trimmed.startsWith("/");
        return !emptyOrComment;
    }

    GetInstructions() {
       const codeArray = this.studentCode.split('\n');
       return codeArray.filter(l => this.isActiveInstruction(l));
    }

    AssertAreEqual(expected: string, actual: string, context: string) {
        if(actual != expected)
        throw new TestError(`{context}: Expected ${expected} Actual ${actual}`);
    }

    AssertLineOfCodeContains(snippet: string, loc: string) {
        if (!loc.includes(snippet)) {
            throw new TestError(`Line of code: ${loc} should contain ${snippet}`)
        }
    }

    AssertLineOfCodeDoesNotContains(snippet: string, loc: string) {
        if (loc.includes(snippet)) {
            throw new TestError(`Line of code: ${loc} should not contain ${snippet}`)
        }
    }

    AssertLineOfCode(snippet: string, loc: string, contains: boolean) {
        if (contains) {
            this.AssertLineOfCodeContains(snippet, loc);
        }
        else {
            this.AssertLineOfCodeDoesNotContains(snippet, loc);
        }
    }

    AssertCode(snippet: string, contains: boolean, atInstrNo?: number) {
        const instr = this.GetInstructions();

        if (atInstrNo != null) {
            if (instr.length > atInstrNo) {
                const loc = instr[atInstrNo];
                this.AssertLineOfCode(snippet, loc, contains)
            }
            else {
                throw new TestError(`No code at offset ${atInstrNo} code length: ${instr.length}`)
            }
        }
        else {
            for (const loc of instr) {
                this.AssertLineOfCode(snippet, loc, contains)
            }
        }
    }

    AssertCodeContains(snippet: string, atInstrNo?: number) {
        this.AssertCode(snippet, true, atInstrNo);
    }
 
    AssertCodeDoesNotContain(snippet: string, atInstrNo?: number) {
        this.AssertCode(snippet, false, atInstrNo);
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