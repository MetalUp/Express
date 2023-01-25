// Extended AQA Instruction set simulator, ARMlite
// Copyright 2019-23 Peter Higginson (plh256 at hotmail.com)

// Jan 2023 Partition the code into a Service module for Richard's Angular service
// Pass 2 - this is just the Service Interface Routines

/**
 * @typedef {Object} RunResult 
 * @property {string} run_id - Unused by ARMlite, so always set an empty string
 * @property {number} outcome - A standard integer code
 * @property {string} cmpinfo - Relevant only to `SubmitProgram ` command, otherwise empty. Means 'assemblyinfo' in ARMlite context
 * @property {string} formattedSource - allows compiler/assembler to return source reformatted
 * @property {string} stdout - Last _system_ message generated by run
 * @property {string} stderr - Applies only when program is run, or attempted to run, and an error occurs
 * @property {string} progout - textual output generated by the program. Scrolling but limited by fixed system definition
 */

/** @type {RunResult} */
var RunResult = {};

// First API for ARMlite Service
/**
 * @param {string}  codeString - A string param.
 * @return {RunResult} This is the result
 */
export function SubmitProgram(codeString)	// SubmitProgram(code : string) : RunResult
{
	RunResult.run_id = "";		// Unused by ARMlite, so always set an empty string
	RunResult.stdout = "";		// unused by assemble
	if (waitingForInput) {		// bad sequence
		RunResult.stderr = "Waiting for input";
		RunResult.outcome = 35;		// Invalid
		return RunResult;
	}
	programText = codeString;
	textToHtml();
	assemble();
	RunResult.formattedsource = programHTML;	// source reformatted
	RunResult.stderr = "";					// unused by assemble
	RunResult.progout = "";					// unused by assemble
	if (address[0] != 0) {				// assembly worked
		RunResult.outcome = 15;
		RunResult.cmpinfo = "Assembled "+byteCount+" bytes OK";
	} else {
		RunResult.outcome = 11;		// note errorLineNum set if we need to return it
		RunResult.cmpinfo = lastMessage;
	}
	return RunResult;
}

// Produced by analysis of stand-alone setup code and URL options
// Excluded display features: memOpt, profile, dynamicProgramWidth, binarySize
// ConfigureSystem(slowSpeed, delayTime, maxUsableMem, debug, comment_align);
// comment_align (int 6-300) included because can pass back formatted assembly listing
/**
 * @param {number} slow_speed
 * @param {number} delayTime
 * @param {number} maxUsableMem
 * @param {boolean} debug_mode
 * @param {number} comment_alignment - (int 6-300)
 * @return {RunResult}
 */
export function ConfigureSystem(slow_speed, delay_time, max_usable_mem, debug_mode, comment_alignment)
{
	slowSpeed = slow_speed;
	delayTime = delay_time;
	maxUsableMem = max_usable_mem;
	debug = debug_mode;
	comment_align = comment_alignment;
	return ClearSystem();
}

/**
 * @param {number} maxSteps
 * @return {RunResult}
 */
export function Run(maxSteps)		// Service interface
{
	RunResult.run_id = "";		// Unused by ARMlite, so always set an empty string
	RunResult.stdout = "";
	if (waitingForInput) {		// bad sequence
		RunResult.stderr = "Waiting for input";
		RunResult.outcome = 35;		// Invalid
		return RunResult;
	}
	if (address[0] == 0) {				// assembly failed
		RunResult.stderr = "No program";
		RunResult.outcome = 35;		// Invalid
		return RunResult;
	}		
	// use the single step code so we can count
	dontDisplay = 1;	// so selects fast path options
	step1Code = 0;				// set non-zero for RFE, HALT and some other cases
		// 0=error, 1=HALT, 2=Breakpoint, 3=interrupt, 4=input, 5=NOP (MOV r0,r0)
	lastMessage = "";	// otherwise we often get the last assembly message
	
	stopping = false;	// Not sure whether we really need these
	noKeyEffects = true;

	RunResult.formattedsource = programHTML;	// source reformatted	
	RunResult.cmpinfo = "";
	RunResult.instructions = 0;

	if ((interruptMask&1)!=0 && (interruptRequest!=0)) {
		// clock interrupts later maybe || (xTime && (xTime < Date.now())))) {

		if (badStack()) { 			// badStack() returns true if error
			RunResult.stderr = lastMessage;
			RunResult.outcome = 35;
			return RunResult;			
		}
		doInterrupt();						// usually changes PC
	}
	while (--maxSteps >= 0) {
		++RunResult.instructions;
		if (step1(1) && step1Code != 5) {		// got an error
			RunResult.stderr = lastMessage;
			RunResult.progout = output1;
			switch (step1Code) {
			case 0:
				RunResult.outcome = 34;
				break;
			case 1:
				RunResult.outcome = 32;
				break;
			case 2:
				RunResult.outcome = 33;		// use STOPPED code
				break;
			case 3:
				RunResult.outcome = 35;		// Interrupt only comes here for fast mode
				break;
			case 4:
				RunResult.outcome = 31;		// Input - but continue not yet supported
				RunResult.stderr = "";
				RunResult.stdout = "Input expected";
				// see comments on InputText() if need type of input
				break;
			default:
				RunResult.outcome = 20;		// Should not happen
				break;
			}
			return RunResult;
		}
		// NOTE INTERRUPTS cannot happen while running currently
	}
	// here if done instructions requested
	RunResult.stderr = "";
	RunResult.stdout = lastMessage;
	RunResult.progout = output1;
	RunResult.outcome = 30;
	return RunResult;	
}

// Note that we saved registerToInput as the dest register and
// inst (historical!!) as the type of input (0 num, -1 string, +1 FP)
/**
 * @param {string} val
 * @return {RunResult}
 */
export function InputText(val)
{
	RunResult.stderr = "";
	RunResult.run_id = "";		// Unused by ARMlite, so always set an empty string
	RunResult.stdout = "";
	RunResult.outcome = 35;		// Invalid is the default
	if (val === false || val == "") {
		RunResult.stderr = "Bad input - must not be empty";
		return RunResult;
	}
	if (!waitingForInput) {
		RunResult.stderr = "Bad input - not waiting for input";
		return RunResult;
	}
	if (processInput(val)) {	// returns true if OK, else false and (maybe) message
		RunResult.outcome = 15;	// OK
		return RunResult;
	}
	RunResult.stderr = lastMessage;
	return RunResult;
}

// Note iclick() does just what we need, so no need to add formal interface unless Richard adds it to the official spec
// function HitPin()
/**
 * @param {number} keyval
 * @return {RunResult}
 */
export function HitKey(keyval)
{
	lastKey = keyval;
	testKeyInterrupt();
	RunResult.run_id = "";					// Unused by ARMlite, so always set an empty string
	RunResult.formattedsource = "";
	RunResult.stdout = "";
	RunResult.stderr = "";
	RunResult.progout = "";
	RunResult.outcome = 15;
	RunResult.cmpinfo = "";
	return RunResult;
}

/**
 * @return {RunResult}
 */
export function Reset()	// interface Reset call
{
	running = false;
	noKeyEffects = false;
	fileResult = -1;			// cancel waiting for file read
	dontDisplay = 1;
	serviceMode = true;
	reset3();
	RunResult.run_id = "";					// Unused by ARMlite, so always set an empty string
	RunResult.formattedsource = "";
	RunResult.stdout = "";
	RunResult.stderr = "";
	RunResult.progout = "";
	RunResult.outcome = 15;
	RunResult.cmpinfo = "";
	return RunResult;
}

/**
 * @return {RunResult}
 */
export function ClearSystem()
{
	// clear all the memory
	for (var i=0; i<maxUsableMem; i+=4) {
		setAddress(i,0);
	}
	byteCount = 0;		// prevent false program highlighting
	programText = "";
	textToHtml();		// better than setting all the resets
	return Reset();
}

/**
 * @param {number} n
 * @return {number}
 */
export function GetRegister(n)
{
	if (n >=0 && n <15) return register[n];
	if (n == 15) return pCounter;
	throw new Error('Illegal register number');
}

// flags	// N Z C V
/**
 * @return {boolean}
 */
export function GetN()
{
	if (flags&8) return true;
	return false;
}
/**
 * @return {boolean}
 */
export function GetZ()
{
	if (flags&4) return true;
	return false;
}
/**
 * @return {boolean}
 */
export function GetC()
{
	if (flags&2) return true;
	return false;
}
/**
 * @return {boolean}
 */
export function GetV()
{
	if (flags&1) return true;
	return false;
}

/**
 * @param {number} loc
 * @return {number}
 */
export function GetMemory(loc)
{
	// note might extend to lowLim later
	// lowLim = vaddressBase-0x100000000;
	if (loc < 0 || loc >= maxUsableMem || (loc & 3) != 0) throw new Error('bad address');
	return address[loc/4];
}

/**
 * @param {number} low
 * @param {number} high
 * @return {number[]}
 */
export function GetMemoryRange(low,high)
{
	// note might extend to lowLim later
	// lowLim = vaddressBase-0x100000000;
	if (low < 0 || high >= maxUsableMem || low > high || (low & 3) != 0 || (high & 3) != 0) throw new Error('bad address range');
	var ret = [];
	var i = 0;
	while (low <= high) {
		ret[i++] = address[low];
		low += 4;
	}
	return ret;
}

// v1address[] starts at vaddressBase and is pixelAreaSize (dynamic) long
// and v2address[] starts at charBase and goes to IOBase (fixed size)
// var v1address = [];						// mid-res or hi-res pixel/video memory
// var v2address = [];						// char map and low-res pixel memory

// GetPixel(n) gives you what LDR Rd,n would give you (and this is a copy of that code!!)
/**
 * @param {number} addr
 * @return {number}
 */
export function GetPixel(addr)
{
	if ((addr & 3) != 0) throw new Error('address not multiple of 4');
	// assume given a 32 bit unsigned address
	if (addr < (pixelBase+4*pixelAreaSize) && addr >= pixelBase) // is pixel memory in I/O area
		return v1address[(addr-pixelBase)/4];

	if (addr < IOBase && addr >= charBase) 					// is char or low-res pixel area
		return v2address[(addr-charBase)/4];

	// note the gap between v1address and v2address just drops through to error
	// but the gap between the char and low-res pixel areas just reads white
	throw new Error('Bad pixel or char address');
}

/**
 * @param {number} low
 * @param {number} high
 * @return {number[]}
 */
export function GetPixels(low,high)
{
	if (low > high || (low & 3) != 0 || (high & 3) != 0) throw new Error('bad address range');
	var ret = [];
	var i = 0;
	while (low <= high) {
		ret[i++] = GetPixel(low);
		low += 4;
	}
	return ret;
}
	
// I have added function GetpixelAreaSize() : int because otherwise you cannot work out whether
// the program changed the resolution or what range is valid for GetPixel(n).
// pixelAreaSize defaults to 3072 but the program can change to 12288 (words and pixels)
/**
 * @return {number}
 */
export function GetPixelAreaSize()
{
	return pixelAreaSize;
}

/**
 * @return {number}
 */
export function GetPixelScreen()
{
	return pixelBase;
}

/**
 * @return {number}
 */
export function GetCharScreen()
{
	return charBase;
}

/**
 * @return {string}
 */
export function GetConsoleOutput()
{
	return output1;
}

// This was added by Peter because his test programs loaded a program from a file
// which auto-assembled. So a function to get the assembly results was needed

/**
 * @return {RunResult}
 */
export function GetProgram()
{
	// just the return part of SubmitProgram()
	RunResult.run_id = "";		// Unused by ARMlite, so always set an empty string
	RunResult.formattedsource = programHTML;	// source reformatted
	RunResult.stderr = "";					// unused by assemble
	RunResult.progout = "";					// unused by assemble
	if (address[0] != 0) {				// assembly worked
		RunResult.outcome = 15;
		RunResult.cmpinfo = "Assembled "+byteCount+" bytes OK";
		RunResult.stdout = lastMessage;	// incase called after Run()
	} else {
		RunResult.outcome = 11;		// note errorLineNum set if we need to return it
		RunResult.cmpinfo = lastMessage;
		RunResult.stdout = "";		// unused by assemble
	}
	return RunResult;
}
