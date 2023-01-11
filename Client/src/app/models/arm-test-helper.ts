import { Run, GetRegister, GetMemory } from 'armlite_service';
import { RunResult } from './run-result';

export class ArmTestHelper {

    // temp
    static RunTests(helper: ArmTestHelper) {
        //This assumes that it can only be called once the code has been submitted to ARMliteServer and assembled OK
        helper.Run(1); //specifies number of steps
        helper.AssertRegister(0, 114); //Can use helpers to reduce duplication here
        helper.Run(1); //specifies number of steps
        helper.AssertRegister(0, 75); //Can use helpers to reduce duplication here
        helper.Run(1); //specifies number of steps
        helper.AssertMemory(2000, 75); //Can use helpers to reduce duplication here
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