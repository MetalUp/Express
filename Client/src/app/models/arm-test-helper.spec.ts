import { ArmTestHelper } from "./arm-test-helper";

describe('ArmTestHelper', () => {

    const testInstruction = "MOV R0, #1";


    beforeEach(() => {

    });

    it('should get instructions', () => {
        
        const ath = new ArmTestHelper(testInstruction);
        const insts = ath.GetInstructions();
        
        expect(insts.length).toBe(1);
        expect(insts[0]).toEqual(testInstruction);
    });

    it('should get multiple instructions', () => {
        const testCode = "MOV R0, #1\nMOV R0, #2\nMOV R0, #3";
        const ath = new ArmTestHelper(testCode);
        const insts = ath.GetInstructions();
        
        expect(insts.length).toBe(3);
        expect(insts[0]).toEqual(testInstruction);
        expect(insts[1]).toEqual("MOV R0, #2");
        expect(insts[2]).toEqual("MOV R0, #3");
    });

    it('should get instructions and ignore blank lines', () => {
        const testCode = "MOV R0, #1\n ";
        const ath = new ArmTestHelper(testCode);
        const insts = ath.GetInstructions();
        
        expect(insts.length).toBe(1);
        expect(insts[0]).toEqual(testInstruction);
    });

    it('should get multiple instructions and ignore blank lines', () => {
        const testCode = "MOV R0, #1\n \nMOV R0, #2\nMOV R0, #3";
        const ath = new ArmTestHelper(testCode);
        const insts = ath.GetInstructions();
        
        expect(insts.length).toBe(3);
        expect(insts[0]).toEqual(testInstruction);
        expect(insts[1]).toEqual("MOV R0, #2");
        expect(insts[2]).toEqual("MOV R0, #3");
    });

    it('should get instructions and ignore comments', () => {
        const testCode = "MOV R0, #1\n//comment";
        const ath = new ArmTestHelper(testCode);
        const insts = ath.GetInstructions();
        
        expect(insts.length).toBe(1);
        expect(insts[0]).toEqual(testInstruction);
    });

    it('should get multiple instructions and ignore comments', () => {
        const testCode = "MOV R0, #1\n// comment\nMOV R0, #2\nMOV R0, #3";
        const ath = new ArmTestHelper(testCode);
        const insts = ath.GetInstructions();
        
        expect(insts.length).toBe(3);
        expect(insts[0]).toEqual(testInstruction);
        expect(insts[1]).toEqual("MOV R0, #2");
        expect(insts[2]).toEqual("MOV R0, #3");
    });

    it('should get instructions and not ignore comments after instructions', () => {
        const testCode = "MOV R0, #1 //comment";
        const ath = new ArmTestHelper(testCode);
        const insts = ath.GetInstructions();
        
        expect(insts.length).toBe(1);
        expect(insts[0]).toEqual(testCode);
    });
})