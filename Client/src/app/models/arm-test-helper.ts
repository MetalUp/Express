import { Run, GetRegister, GetMemory } from 'armlite_service';
import { EmptyRunResult, RunResult } from './run-result';

export class ArmTestHelper {

    static runTests(testCode: string) {
        var rr = { ...EmptyRunResult };
        var helper = new ArmTestHelper();
    
        try {
            const runTestsFunction = new Function('helper', testCode);
            runTestsFunction(helper);
            rr.stdout = "all tests passed";
            rr.outcome = 15;
        }
        catch (e) {
            if (e instanceof Error) {
                rr.stdout = e.message;
            }
            else {
                rr.stderr = `unexpected error: ${e}`;
            }
            rr.outcome = 12;
        }

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
            throw new Error(`${expected}: Expected ${expected} Actual: ${actual}`);
    }

    AssertMemory(address: number, expected: number) {
        var actual = this.GetMemory(address);
        if (actual != expected)
            throw new Error(`Memory address ${address}: Expected ${expected} Actual ${actual}`);
    }

}