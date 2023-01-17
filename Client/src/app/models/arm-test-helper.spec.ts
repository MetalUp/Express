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

    it('should assert contains ok', () => {
        const ath = new ArmTestHelper(testInstruction);
        try {
            ath.AssertCodeContains("MOV", 0);
        }
        catch (e : any) {
           fail();
        }
    });

    it('should assert contains fail', () => {
        const ath = new ArmTestHelper(testInstruction);
        try {
            ath.AssertCodeContains("MOT", 0);
            fail(); // expect exception
        }
        catch (e : any) {
            expect(e.message).toBe("Line of code: MOV R0, #1 should contain MOT")
        }
    });

    it('should assert not contains ok', () => {
        const ath = new ArmTestHelper(testInstruction);
        try {
            ath.AssertCodeDoesNotContain("MOT", 0);
        }
        catch (e : any) {
           fail();
        }
    });

    it('should assert not contains fail', () => {
        const ath = new ArmTestHelper(testInstruction);
        try {
            ath.AssertCodeDoesNotContain("MOV", 0);
            fail(); // expect exception
        }
        catch (e : any) {
            expect(e.message).toBe("Line of code: MOV R0, #1 should not contain MOV")
        }
    });

    it('should assert contains ok no line', () => {
        const ath = new ArmTestHelper(testInstruction);
        try {
            ath.AssertCodeContains("MOV");
        }
        catch (e : any) {
           fail();
        }
    });

    it('should assert contains fail no line', () => {
        const ath = new ArmTestHelper(testInstruction);
        try {
            ath.AssertCodeContains("MOT");
            fail(); // expect exception
        }
        catch (e : any) {
            expect(e.message).toBe("Line of code: MOV R0, #1 should contain MOT")
        }
    });

    it('should assert not contains ok no line', () => {
        const ath = new ArmTestHelper(testInstruction);
        try {
            ath.AssertCodeDoesNotContain("MOT");
        }
        catch (e : any) {
           fail();
        }
    });

    it('should assert not contains fail no line', () => {
        const ath = new ArmTestHelper(testInstruction);
        try {
            ath.AssertCodeDoesNotContain("MOV");
            fail(); // expect exception
        }
        catch (e : any) {
            expect(e.message).toBe("Line of code: MOV R0, #1 should not contain MOV")
        }
    });
})