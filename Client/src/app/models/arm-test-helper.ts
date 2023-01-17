import { Run, GetRegister, GetMemory } from 'armlite_service';
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

    Run(maxSteps: number) {
        return Run(maxSteps) as RunResult;
    }

    GetRegister(n: number) {
        return GetRegister(n);
    }

    GetMemory(loc: number) {
        return GetMemory(loc);
    }

    private isActiveInstruction(line: string) {
        const trimmed = line.trim();
        if (trimmed === "") {
            return false;
        }
        if (trimmed.startsWith("/")) {
            return false;
        }
        return true;
    }

    GetInstructions() {
       const codeArray = this.studentCode.split('\n');
       return codeArray.filter(l => this.isActiveInstruction(l));
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
            throw new TestError(`Expected: ${expected} Actual: ${actual}`);
    }

    AssertMemory(address: number, expected: number) {
        var actual = this.GetMemory(address);
        if (actual != expected)
            throw new TestError(`Memory address ${address}: Expected ${expected} Actual ${actual}`);
    }
}