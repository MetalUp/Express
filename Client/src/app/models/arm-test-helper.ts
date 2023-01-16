import { Run, GetRegister, GetMemory } from 'armlite_service';
import { EmptyRunResult, RunResult } from './run-result';

class TestError extends Error {
    constructor(message : string) {
        super(message);
    }
}


export class ArmTestHelper {

    static runTests(testCode: string) {
        var rr = { ...EmptyRunResult };
        var helper = new ArmTestHelper();
    
        try {
            const runTestsFunction = new Function('helper', testCode);
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