// Extended AQA Instruction set simulator, ARMlite
// Copyright 2019-23 Peter Higginson (plh256 at hotmail.com)

// Jan 2023 Partition the code into a Service module for Richard's Angular service
// for now keep the outer layer so I can test it, whether this becomes ARMlite is TBD
// First pass, keep the global definitions here and the setups in mainxxx.js
// Later separate the screen driving parts somewhere else, maybe key handling stays in mainxxx.js

// For historical comments see last V1 mainxxx.js file
// BEWARE you cannot do == NaN (at least not to get the result you expect)

// global variables
var myTimeout = false; 					// to hold return from setTimeOut()
var scrollTimeout = false;
var register = [];						// register file
// note SP treated internally as R13 and LR as R14 (but not PC)
var flags = 0;							// N Z C V
var pCounter = 0;
var programText = "";					// loaded or selected file
var programHTML = "";					// display version of programText
var programEdit = "";					// formatted version to edit
var programSave = "";					// ditto without line numbers
var output1 = "";						// last line of scrolling number/character output
var fileRead = "";						// contents read from input file reading
var fileResult = -1;					// register to read into
var address = [];						// main memory
// v1address[] starts at vaddressBase and is pixelAreaSize (dynamic) long
// and v2address[] starts at charBase and goes to IOBase (fixed size)
var v1address = [];						// mid-res or hi-res pixel/video memory
var v2address = [];						// char map and low-res pixel memory
var instructionTxt = [];
var addrToLine = [];
var lineToByteAddress = [];				// now each code line might not be aligned or a multiple of 4
var speed;
var slowSpeed = 40;					// default delay for SLOW mode - is a calling parameter to change it
var delayTime = 4;					// default parallel delay in FAST mode - is a calling parameter to change it
var comment_align = 24;				// comment alignment default
var dynamicProgramWidth = false;	// default is fixed width	
var fileWriteBuffer = [];			// for file writes

var inst;							// made global for FETCH/EXECUTE use
var addr;							// made global for FETCH/EXECUTE use
var waitingForInput = false;
var registerToInput = 0;
var modifyingProgram = false;
var stopping;
var oneStep;
var running = false;
var lastCodeHighlight = -1;
var lastMemHighlight = -1;
var breakpointAddr = -1;
var errorLineNum = -1;
var memOpt = 2;					// 0 = 2's comp, 1 = unsigned, 2 = hex, 3 = binary
var maxUsableMem = 1048576;		// first byte we cannot use (currently query string can change)
var overlay = 0;				// the base page being displayed (two pages = 512 bytes displayed)
var lastPixelClick = -1;

var waitingForOverlay = false;	// NEED TO DO ALL THE RESET CASES FOR THIS

var byteCount = 0;				// as lineCount but allows for multi-word directives (e.g. .asciz)
var instructionCount = 0;		// for .InstructionCount I/O Read
var measureSpeed = 0;
var breakPoint = -1;			// no Break Point set initially
var runTimer;
var hex = ['0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f'];
var debug = 0;					// set to 1 by url query string - all current tests are true/false
								// but values might be used for special purposes so keep as an int
var xTime = 0;
var interruptCnt = 0;			// counter to ensure clock gets 50% of available interrupts
var dotDataAddress = 0;			// for protection control - 0 is off (address of .data directive)
var firstDataDefn = 0;			// for default if above not set (set on first .block, .word or .byte)
var dontDisplay = 0;			// one to show nothing during run
var binarySize = 100;			// size to set binary

// need to remember some things while fast running so on error we might show them (not currently)
var lastMessage = '';			// useful (very occasionally) to know the last message shown to the user
var oldPCMarker = -1;
var stepTxt = '';
var lineNo;						// made global so after a pause we know what the previous PC/4 was
var step1Code = 0;				// to special case HALT, RFE etc. out of the fast path
				// 0=error, 1=HALT, 2=Breakpoint, 3=interrupt, 4=input, 5=NOP (MOV r0,r0)

var halted="Program HALTED. STOP, LOAD or EDIT";

// Tables for the new I/O space. Move everything to memory mapped I/O and get rid of INP and OUT
// I wish I had C struct arrays and I cannot cope with faking it - so multiple arrays it is
// The initial order was irrelevant - just matching corresponding entries. However the other two
// arrays must match this one and the indexes for dotLabelValues are used as numbers in the code.
// So you can add new at the end but you will neeed to fix the LDR,LDRB,STR,STRB execution code
// if you change anything else

// some definitions - note the code DOES assume things are in this order - it is almost
// impossible to do otherwise (mainly the write code!)
var IOBase = 0xffffff00;		// use wherever possible (intent - achieved I think)
var fakeBig = 0xfffff300;		// A directly addressable 32x24 big pixel map
// So layout (going down) is Vectors, random I/O, big pixels, char map, pixel map
var charMax = 512;
var charBase =  0xfffff0e0;		// need 32x16 = 0x200 bytes, leave gap to trap errors

// hi-res can be selected by program but the same base is used
var pixelBase =  0xffff3000;	// need 128x96x4 bytes so 0xc000 for hi-res
var vaddressBase = pixelBase;	// used as the lowest I/O address that is legal

var pixelAreaSize = 3072;		// words and pixels (64x48x4 bytes so 0x3000 (12288))
// The alternative 128x96 pixel area sets 0xffff4000 and 12288

// now vaddress[] is split into two to avoid having a big hole in the middle
// So v1address[] starts at vaddressBase and is pixelAreaSize (dynamic) long
// and v2address[] starts at charBase and goes to IOBase (fixed size)
// The "fake" 32x24 pixel area IS in v2address[] so we can read it

var IOVectorBase = 0xffffff80;
// assume 4 types of exceptions/interrupts for now, Syscall, Button, Input Char, Timer
var IOVectors = [-1,-1,-1,-1,-1];		// -1 means not setup yet
// note Syscall is not masked but if you get one without setting it up system will halt
// Interrupt control is now all in words!
var interruptMask = 0; // low bit (0) is master on/off
var keyboardMask = 0
var pinMask = 0;
var clockIntFreq = 0;	// now number of ms
var pixelMask = 0; // Pixel interrupt (2 set/1 poll enable/0 int enable)
var interruptRequest = 0; // one bit for each type of request PIN=0, Char=1, Timer=2, Pixel=3
		// note that syscall is more like a BL except flags, int state and PC are saved on stack

var canSave = false;
var haveSaved = false;
var saveSeq = 0;
var autoRun = 0;		// flag for URL options to load/submit/run
var serviceMode = true;	// false to get direct I/O

/* export interface RunResult {
    run_id: string, //Unused by ARMlite, so always set an empty string
    outcome: number, //A standard integer code - see below - unique to 
    cmpinfo: string, //Relevant only to `SubmitProgram ` command, otherwise empty. Means 'assemblyinfo' in ARMlite context
    formattedsource: string, //(new addition) allows compiler/assembler to return source reformatted
    stdout: string, //Last _system_ message generated by run
    stderr: string //Applies only when program is run, or attempted to run, and an error occurs
    progout: //textual output generated by the program. Scrolling but limited by fixed system definition
} */

/* EXAMPLE const obj = {
    prop1: "I'm",
    prop2: "an",
    prop3: "object"
}
object.property or object["property"] */

var RunResult = {};

// Reduce the number of Events logged (new logging system Nov 2022)
var eventCount = 0;
var eventList = '';
var eventStep = 0;
var logTime = Date.now();
var plh_id = 0;			// to record the Matomo ID and URL for cross-referencing
var plh_url = "";

// List of the register address names
var dotLabelNames = ['.WriteSignedNum', '.WriteUnsignedNum', '.WriteHex', '.WriteChar', '.WriteString',
	'.InputNum', '.LastKey', '.LastKeyAndReset', '.Random', '.InstructionCount',/*10*/ '.Time', '.PinISR',
	'.SysISR', '.KeyboardISR', '.ClockISR', '.InterruptRegister', '.PinMask', '.KeyboardMask',
	'.ClockInterruptFrequency', '.CodeLimit',/*20*/ '.PixelScreen', '.CharScreen', '.OpenFile', '.FileLength',
	'.ReadFileChar', '.ReadString', '.InputFP', '.WriteFP', '.PixelAreaSize', '.WriteFileChar',
	/* 30 */ '.WriteFile', '.ClearScreen', '.ReadSecret', '.PixelISR', '.LastPixelClicked', '.LastPixelAndReset',
	'.PixelMask', '.Resolution'];
	// Note .PixelAreaSize (read only) and .ClearScreen (write only) are the same value

// The addresses associated with the above register names - all are now word addresses
// base to base+32 for old INP/OUT, base+34 to base+124 new I/O, base+128 onwards is ISR vectors
// CARE - the indexes into this table are used in the code, but the content values are NOT
// SO - if you want to change an I/O function to address mapping this should be the only place to change
var dotLabelValues = [IOBase+16, IOBase+20, IOBase+24, IOBase+28, IOBase+32,
	IOBase+8, IOBase+16, IOBase+20, IOBase+32, IOBase+64, /*10*/ IOBase+68, IOVectorBase+4,
	IOVectorBase, IOVectorBase+8, IOVectorBase+12, IOBase+72, IOBase+76, IOBase+52, IOBase+56,
	IOBase+80, /*20*/ pixelBase, charBase, IOBase+84, IOBase+88, IOBase+92, IOBase+96, IOBase+100,
	IOBase+104, IOBase+108, IOBase+112, /*30*/ IOBase+116, IOBase+108, IOBase+36, IOVectorBase+16,
	IOBase+40, IOBase+44, IOBase+48, IOBase+60];
// DO NOT REORDER THE ABOVE TABLE - there are things like dotLabelValues[20] in the code!!!
// ALSO do not use IOBase+0 - it is useful to trap running off the end of the pixels!!

// SPARE IOBase values (BE CAREFUL) 4 12 120 124

// A code to see what we can do with the address. 1=write word, 2=write byte, 4=read word, 8=read byte
// 16=base address (so can be index base - not likely used by LDR/STR check, just assembler)
var dotLabelMode = [1, 1, 1, 3, 1, 4, 12, 12, 4, 4,/*10*/ 4, 5, 16+5, 5, 5, 5, 5, 5, 5, 4,
				/*20*/ 31, 31, 4, 4, 12, 1, 4, 1, 4, 3,/*30*/ 1, 1, 1, 5, 4, 4, 5, 5];

// return index or -1 if not found or value of .Pixelnnn (which is +ve > 0x80000000)
function matchDotLabel(label)
{
	var i;		// do this first so .PixelScreen and similar get seen
	for (i = 0; i < dotLabelNames.length; ++i) {
		if (label == dotLabelNames[i]) {
			//alert("matchDotLabel returning (found) "+i);
			return i;
		}
	}
	// try special case .Pixelnn - where nn is 0 to 767
	if (label.length > 6 && label.startsWith(".Pixel")) {
		var a = parseIntGen(label.substring(6));
		if (isNaN(a) || a<0 || a>767) return -1;
		return fakeBig+a*4;
	}
	//alert("matchDotLabel returning not found -1 for "+label);
	return -1;
}

// return index or -1 if not found - used for checking if I/O operations valid in LDR and STR
// It may be overkill but it will allow us to give better error messages like "unknown address"
// or "invalid operation" rather than just dropping out the bottom of the list of known functions
// not used for pixelBase, charBase or .PinISR (word only) because these are ranges
function matchIOAddress(value)
{
	var i;
	for (i = 0; i < dotLabelValues.length; ++i) {
		if (value == dotLabelValues[i]) return i;
	}
	return -1;
}

/* Draft documentation/plan
Current Outputs:
3 -  .WriteFP
4 -  .WriteSignedNum
5 -  .WriteUnsignedNum
6 -  .WriteHex
7 -  .WriteChar
8 -  .WriteString

Older Inputs:
1 -   .ReadString (note this has to be an STR!!!!)
2 -   .InputNum
3 -   .InputFP
4 -   .LastKey
5 -   .LastKeyAndReset
8 -   .Random
12 -  .OpenFile
13 -  .FileLength
14 -  .ReadFileChar

Newer Inputs
.InstructionCount  - reset only by Reset or (assemble) Submit
.Time  -  I actually suggest resolution of Seconds, and only since 1st Jan 2000  00:00:00 
 STR R0, .PinISR
STR R0, .InterruptRegister
.CodeLimit
.pixelScreen
.charScreen
*/
// .WriteFileChar (STR or STRB) adds a character to the output buffer and .WriteFile (STR) writes the
// buffer to a file (with an .asciz string as the suggested file name). Set register -1 to clear the buffer.
// var fileWriteBuffer = [];			// for file writes
// .WriteFileChar = IOBase+112, /*30*/ .WriteFile = IOBase+116];  		dotLabelValues[29 or 30]

// Cheat for now (maybe always) by passing the globals we have already setup to be done again
// Excluded display features: memOpt, profile, dynamicProgramWidth, binarySize
// ConfigureSystem(slowSpeed, delayTime, maxUsableMem, debug, comment_align);
// comment_align (int 6-300) included because can pass back formatted assembly listing

function ConfigureSystem(a, b, c, d, e)
{
	slowSpeed = a;
	delayTime = b;
	maxUsableMem = c;
	debug = d;
	comment_align = e;
	return ClearSystem();
}

// count how many characters so far in the Edit line
// I did try to use the HTML line but it has <div ....
function do_count(line)
{				// lineEdit only might have tabs!
	var tt = 0;
	var cnt = 0;
	while (tt < line.length) {
		if (line[tt] == '\t') cnt = (cnt+4) & 0xffffc;
		else ++cnt;
		++tt;
	}
	return cnt;
}

// format a program - from the input to the display, also produce the version to edit with correct tabbing
// note cannot use addrToLine[] (or anything that a good assemble() requires) because might be error
function textToHtml()
{
	programHTML = "";
	programEdit = "";
	programSave = "";	
	var lines = 1;			// in address mode count only addresses (I think these are the same now!)
	var lines2 = 1;			// count marked lines only - start from 1
	var count = 0;
	var lineHTML = "";
	var lineEdit = "";
	var comment = false;
	var asciiStart = "";			// start quotes value
	var asciiChar = "";				// last char for backslash detection
	//var justLabel = false;					// to incr instr or not
	var removedNum = false;					// control remove edit numbering
	var pLength = programText.length;
	if (programText[pLength-1] != "\n") {		// make sure last line ends correctly
		programText += "\n";
		++pLength;
	}
	for (var n = 0; n < pLength; ++n) {
		var letter = programText[n];
		
		// Leave this substitution because in the cases we need it
		// we can walk back in the input string rather than have lots of extra cases
		// (we decided not to proceed with many of the original cases so this was a good choice
		// apart from the bug of not allowing spaces or tabs inside comments
		if (letter == '\t') {
			if (comment) {
				var tmp3 = 4 - (do_count(lineEdit)&3);	// range 1-4
				while (tmp3>0) {
					lineHTML += "&nbsp;";
					--tmp3;
				}
				lineEdit += '\t';
				continue;
			}
			letter = ' ';	// if not in a comment treat a tab as a space
		}

		if (letter < ' ' && letter != '\n') continue;	// ignore all control characters other than newline and tab

		// We decided to use | to delimit the added number because a) it has no other current use and
		// b) Richard thought it would look good (it does). This avoids problems with space being ambiguous
		
		// need same number removal here as assemble()
		if (removedNum == false) { // numbering adds 4 (exceptionally 5+) chars but user might edit so be flexible
			removedNum = true;				// whatever happens disable test till next newline seen
			var i;
			for (i = n; i < pLength; ++i) {
				if (programText[i] == '|') { 		// found the numbering delimiter
					n = i;
					break;			// need to do continue; on the outer loop!
				}
				// only numbers and space allowed before the number delimiter
				if (programText[i] != ' ' && programText[i] != '\t' && (programText[i] < '0' || programText[i] > '9')) {
					i = -1;			// to ensure we dont do the continue; (note n can be 0)
					break;			// newline goes this way as well
				}
			}
			if (n == i) continue;
		}

		if (asciiStart) {			// have found ascii string in quotes and not a label
			lineEdit += letter;
			if (letter == ' ') lineHTML += "&nbsp;";
			else if (letter == '&') lineHTML += "&amp;";
			else if (letter == '<') lineHTML += "&lt;";
			else if (letter == '>') lineHTML += "&gt;";
			else lineHTML += letter;
			if (asciiChar == '\\') {
				asciiChar = '';
				continue;
			}
			asciiChar = letter;								// so have for next loop (or next quotes)
			if (asciiStart == letter) {
				asciiStart = '';		// found matching quote
				continue;
			}
			if (letter != '\n') continue;
			// have to allow for one quote and hitting end of line
			asciiStart = '';
			// drop through to handle nl as normal
		}

		if (!comment && (letter == '"' || letter == "'")) {
			asciiStart = letter;
			lineEdit += letter;
			lineHTML += letter;
			continue;
		}

		// possible bug in v1.0 (first release ARMlite) here if space followed by tab
		// however many cases (like start of line) are caught by other logic later
		// also v1.0 bug that spaces or tabs in comments need to be preserved
		if (letter == ' ' && (n+1) < pLength && (programText[n+1]==' '||programText[n+1]=='\t')) {
			if (comment) {		// need to support multiple spaces in comments (tab done above)
				lineHTML += "&nbsp;";		// but force HTML to do it
				lineEdit += ' ';
			}
			continue; // ignore multiple spaces unless in comment
		}

		if (count == 0) { 					// start of a line
			if (letter == '\n') {
				removedNum = false;			// possibly not really blank if we removed a number
				continue;		// ignore blank lines
			}
			if (letter == ' ') continue;		// ignore leading spaces
			if (letter == '/' || letter == ';') {	// accept / or ; as start of comment
				count = 11;		// stop comments being padded
				comment = true;	// or numbered (that option no longer exists)
				// need 4 spaces here but was getting 6 unless the comment looked like a label (got 0)!!
				// multiple else are just too convoluted so duplicate end of loop
				lineHTML += '<span class="comment">';
				// we decided not to comment align here!
				lineHTML += letter;
				lineEdit += letter;
				continue;
			}
			var label = ''+letter;		// hope this forces label to be a string
			var i = 1;
			while (n+i < pLength && programText[n+i] > ' ') {
				label += programText[n+i];
				if (programText[n+i] == ':') break;
				++i;
			}
			if (programText[n+i] != ':') {
				lineHTML += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
				lineEdit += "      ";
				count = 5;
			}
		}
		// remember that HTML will ignore multiple spaces anyway
		while (count < 5 && letter == ' ' && programText[n+1] > ' ') {
			lineEdit += " ";
			lineHTML += "&nbsp;";
			++count;
		}
		if (letter == '\n') {
			// label all the real lines in all modes now!
			// TEMP TO GET SOMETHING - but this seems to be OK and is simple!
			programHTML += '<div id="lin'+lines2+'" class="selectable"><span class="line">';
			++lines2;
			if (lines <10) {
				programHTML += '&nbsp;';
				programEdit += ' ';
			}
			if (lines <100) {
				programHTML += '&nbsp;';
				programEdit += ' ';
			}
			programHTML += lines + '|</span>' + lineHTML;
			if (comment) programHTML += '</span>';
			programEdit += lines + '|';
			++lines;
			programHTML += '</div>';
			lineHTML = "";
			//if (n+1 < pLength) programHTML += "<br />";		// dont need br at end
			programEdit += lineEdit + "\n";
			programSave += lineEdit + "\r\n";
			lineEdit = "";
			count = 0;
			comment = false;
			//justLabel = false;
			removedNum = false;
		} else {
			++count;
			if (!comment && (letter == '/' || letter == ';')) {			// comment after label or code
				comment = true;
				//if (comment_align) {		// going to change the alignment
					// we need to remember that there might have been a space or tab as previous character
					// so let's remove it first
				var last_char = programText[n-1];	// not at start of a line so n must be >0
				if (last_char == ' ' || last_char == '\t') {	// care - might have been a mixture
					if (lineHTML[lineHTML.length-1] == ';')	// prev was probably &nbsp; (not sure possible)
						lineHTML = lineHTML.slice(0, -6);
					else lineHTML = lineHTML.slice(0, -1);
					lineEdit = lineEdit.slice(0, -1);
				}
				var tmp3 = do_count(lineEdit);	// current number of real characters
				if (tmp3 >= comment_align) {	// aligned or to right - insert one space
					lineEdit += ' ';
					lineHTML += "&nbsp;";
				} else {					// pad Edit with space and HTML with &nbsp;
					while (tmp3 < comment_align) {
						lineHTML += "&nbsp;";
						lineEdit += " ";
						++tmp3;
					}
				}
				//}
				lineHTML += '<span class="comment">';
			}
			if (letter == '&') lineHTML += "&amp;";
			else if (letter == '<') lineHTML += "&lt;";
			else if (letter == '>') lineHTML += "&gt;";
			else lineHTML += letter;
			lineEdit += letter;
		}
	}
	if (!serviceMode) resetProgramArea(programHTML);
}

// First API for ARMlite Service
function SubmitProgram(codeString)	// SubmitProgram(code : string) : RunResult
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

function assemble()
{
	if (waitingForInput) return;
	reset2();
	for (var i = 0; i < maxUsableMem; i+=4) {			// clear the memory
		setAddress(i, 0);
	}
	var indexToLine = [];		// needed to set tool tips and errors in assemble pass 2 (needed in functions called)

	// Give line number or address depending on which display mode we are in
	// This version used by assemble gets given a real line number
	function convert(n, inst)		// inst is instText[n] or ''
	{					// the problem with .WORD is that it is the default for instruction does not match
		if (inst == '.WORD') inst = '';
		if (inst !== '') inst = ' '+inst;
		return indexToLine[n]+inst;
	}

	var pLength = programText.length;
	if (programText[pLength-1] != "\n") {		// make sure last line ends correctly
		programText += "\n";
		++pLength;
	}
	var lineCount = 0;
	byteCount = 0;				// used to reset SP, might have other uses later
	dotDataAddress = -1;		// for protection control - 0 is off (address of .data directive)
								// start with -1 so can tell whether set or not - can be set to 0 - fixup at end
	firstDataDefn = -1;			// for default if above not set (set on first .block, .word or .byte)
	var wordCount = 0;
	var tempWordText = "";
	var lastChar = "";	// for .ascii
	var labelText = [];					// make 3 entries per line: label/0, instruction, address/label/0
	var instText = [];
	var offsetText = [];				// for RISC/AQA the offsetText can be quite complex
	var comment = false;
	var comma = false;
	var label = false;
	var justSeenLabel = false;	// to fix the bug that blank lines after labels upset the code highlighter
	var ascii = false;
	var blankLabels = 1;				// recify lines vs. instructions count - see if 1 will get line numbers starting at 1
	addrToLine = [];					// clear mapping lists
	lineToByteAddress = [];
	var removedNum = false;					// control remove edit numbering
	for (var n = 0; n < pLength; ++n) {
		var letterText = programText[n];
		if (letterText == '\r') continue;	// ignore CR (come in with LOAD)
		
		// We decided to use | to delimit the added number because a) it has no other current use and
		// b) Richard thought it would look good (it does). This avoids problems with space being ambiguous
		// Because of a) we can always remove - from LOAD for example in case user has added/copied the numbers

		if (removedNum == false) { // numbering adds 4 (exceptionally 5+) chars but user might edit so be flexible
			removedNum = true;				// whatever happens disable test till next newline seen
			var i;
			for (i = n; i < pLength; ++i) {
				if (programText[i] == '|') { 		// found the numbering delimiter
					n = i;
					break;			// need to do continue; on the outer loop!
				}
				// only numbers and space allowed before the number delimiter
				if (programText[i] != ' ' && programText[i] != '\t' && (programText[i] < '0' || programText[i] > '9')) {
					i = -1;			// to ensure we dont do the continue; (note n can be 0)
					break;			// newline goes this way as well
				}
			}
			if (n == i) continue;
		}		

		if (comment && letterText != '\n') continue;
		if (letterText == '\t') letterText = ' ';

		if (ascii) { 		// need to allow for text strings in the offset field
			if (letterText == '\n') {		// no closing quotes
				message("Closing quotes missing at line "+(lineCount+blankLabels));
				showError(lineCount+blankLabels);
				return;
			}
			if (lastChar == '\\') {						// need to take this character unconditionally
				tempWordText += letterText;
				lastChar = '';
				continue;
			}			
			tempWordText += letterText;
			lastChar = letterText;
			if (letterText != tempWordText[0]) continue;	// not found closing quotes yet
			
			offsetText[lineCount] = tempWordText;
			tempWordText = "";
			++wordCount;						// cannot see a need here to keep a following space
			ascii = false;						// but the formatter will have to be more clever
			continue;							// and also allow for & < > inside the text string
		}

		if (letterText == '/' || letterText == ';') {
			comment = true;
			continue;
		}
		if (letterText == ',') {
			comma = true;
		} else {
			if (comma) {
				if (letterText == ' ') continue;	// remove spaces (or tabs) after a comma
				comma = false;
			}
		}
		// change 21/7/2017 - labels must have a colon and on a line on their own allowed
		if (letterText == ':') { // found a label
			if (wordCount != 0) {
				if (labelText[lineCount]) message("cannot have two labels at one address at line "+(lineCount+blankLabels));
				else message("label must be first item at line "+(lineCount+blankLabels));
				showError(lineCount+blankLabels);
				return;
			}
			// check if the label is a register
			var rs = RegExp("^(r([0-9]|1[0-5])|pc|lr|sp)$");
			var tg = rs.test(tempWordText.toLowerCase());
			//alert("Testing "+tempWordText+" Result "+tg);
			if (tg) {
	/*		if ((tempWordText[0] == 'R' || tempWordText[0] == 'r') &&
				((tempWordText.length == 3 && tempWordText[1] == '1' &&
				tempWordText[2] >= '0' && tempWordText[2] <= '2') ||
				(tempWordText.length == 2 && tempWordText[1] >= '0' &&
				tempWordText[1] <= '9'))) { */
				message("cannot use a register as a label  at line "+(lineCount+blankLabels));
				showError(lineCount+blankLabels);
				return;
			}
			// trap the instructions which do not have parameters
			rs = RegExp("^(ret|rfe|halt|hlt|svc)$");
			tg = rs.test(tempWordText.toLowerCase());
			if (tg) {
				message("cannot use "+tempWordText+" as a label at line "+(lineCount+blankLabels));
				showError(lineCount+blankLabels);
				return;
			}
			// stricter rules for labels, must start with a letter and have normal characters
			// _ counts as a letter - . is reserved for assembler labels and directives
			// was orig + but now letter first needs to be * for 0 or more
			var ms = RegExp("^[a-zA-Z_][0-9a-zA-Z_]*$");
			var tf = ms.test(tempWordText);	// don't understand why running twice failed!
			//alert("Testing "+tempWordText+" Result "+tf);
			if (!tf) {	// not all good characters
				message("bad character in label at line "+(lineCount+blankLabels));
				showError(lineCount+blankLabels);
				return;
			}

			var a;
			for (a = 0; a < lineCount; ++a) {
				if (tempWordText == labelText[a]) break;
			}
			if (a < lineCount) {		// duplicate label
				message("duplicate label at lines "+indexToLine[a]+" and "+(lineCount+blankLabels)+' &nbsp;'+tempWordText);
				showError(lineCount+blankLabels);
				return;
			}
			if (matchDotLabel(tempWordText) != -1) {		// -1 means not found
				message("invalid label at line "+(lineCount+blankLabels)+' &nbsp;'+tempWordText);
				showError(lineCount+blankLabels);
				return;
			}
			labelText[lineCount] = tempWordText;
			label = true;
			justSeenLabel = true;
			tempWordText = "";
			++wordCount;
			continue;
		}
		if (label) {
			if (letterText == ' ') continue; // ignore any spaces after a label
			if (letterText == '\n') {
				if (justSeenLabel || comment) {		// add "comment" to try to fix numbering problem
					++blankLabels;
					justSeenLabel = false;
				}
				comment = false;
				removedNum = false;
				continue;		// pretend label starts next line (but still remove number if any)
			}
			// problem here that .data after label needs not to cancel label markers
			if (letterText == '.' && n < (pLength-5)) { // might be .data
				if (programText.substring(n,n+5).toUpperCase() == ".DATA") {
					dotDataAddress = byteCount;
					comment = true;						// process rest of line as comment
					n += 4;								// move forward so next character is after A
					justSeenLabel = false;				// I think we need this?
					continue;
				}
			}
			label = false;		// end of text after label
			justSeenLabel = false;
		} // drop through if start of another word

		if (letterText == ' ' || letterText == '\n') {	// end of word
			if (tempWordText.toUpperCase() == ".DATA") {
				// need to treat line as label on own or ignore the line
				if (dotDataAddress != -1) {		// not allowed two!
					message("cannot have more than one .data");
					showError(lineCount+blankLabels);
					return;
				}
				dotDataAddress = byteCount;
				// treat this and the rest as if seen a comment
				tempWordText = "";
				if (letterText == '\n') {			// process the newline
					++blankLabels;
					removedNum = false;
				} else {							// treat rest of this line as a comment
					comment = true;
				}
				continue;
			}
			if (tempWordText != "") {				// ignore leading/trailing spaces
				if (wordCount == 0) {
					labelText[lineCount] = 0;				// no label
					++wordCount;
				}
				if (wordCount == 1) {
					if (instruction(tempWordText.toUpperCase()) < 200) { // valid instruction
						instText[lineCount] = tempWordText.toUpperCase();
					} else {
						instText[lineCount] = ".WORD";	// fake a .WORD
						++wordCount;
					}
				}
				if (wordCount == 2) offsetText[lineCount] = tempWordText;
				else if (wordCount > 2)offsetText[lineCount] += tempWordText;
				tempWordText = "";
				++wordCount;
			}
		}
						// need to detect start of .ascii/z string
		if (wordCount == 2 && tempWordText == "" && (letterText == '"' || letterText == "'")) {
			tempWordText = letterText;
			ascii = true;
			continue;
		}
		
		if (letterText == '\n') {
			if (wordCount > 0) {				// ignore blank lines (or just comments)
				if (wordCount == 1) instText[lineCount] = 0;	// no inst - I don't think this can happen now
				if (wordCount < 3) offsetText[lineCount] = 0;	// no offset
				wordCount = 0;
				// need to allow for byte, block or ascii instructions that can be variable length
				var len = getLen(instText[lineCount], offsetText[lineCount], byteCount);
					// note getLen sets firstDataDefn since it has tested the instruction type anyway
				// len is now -1 for a normal aligned instruction, > 0 for a byte or ascii/z directive
				// -2 for a badly formatted .ascii or .asciz string, -3 for bad .block parameter
				if (len == -2) {
					// I could not find a way to test this - so probably cannot happen
					message("bad ascii string at line "+(lineCount+blankLabels));
					showError(lineCount+blankLabels);
					return;
				}
				if (len < -1) {
					message("bad .block or .align count at line "+(lineCount+blankLabels));
					showError(lineCount+blankLabels);
					return;
				}
				if (len < 0 && (byteCount&3) != 0) {
					// this instruction must be aligned
					byteCount = (byteCount & 0xffffffc) + 4;
				}
				if ((byteCount & 3) == 0) {			// only fill in addrToLine for aligned lines
					addrToLine[byteCount/4] = lineCount + blankLabels;
				}
				indexToLine[lineCount] = lineCount + blankLabels;	// needed to set tool tips in pass 2
				lineToByteAddress[lineCount] = byteCount;
				++lineCount;
				if (len >= 0) byteCount += len;		// .align can return len = 0
				else byteCount += 4;
			} else if (comment) ++blankLabels;		// completely empty lines get removed before display
			comment = false;
			removedNum = false;
		}
		if (letterText > ' ') {
			tempWordText += letterText;
		}
	}
	// catch the case that we just saw a label with no instruction as the last item
	if (wordCount) {
		if (wordCount != 1) {if (debug) alert("Simulator bug - wordCount = "+wordCount);}
		else {
			instText[lineCount] = ".WORD";	// try this - fake something
			offsetText[lineCount] = 0;
			indexToLine[lineCount] = lineCount + blankLabels - 1;	// backup because have hit the newline at end already
			lineToByteAddress[lineCount] = byteCount;
			++lineCount;
		}
	}

	// we have not saved anything (in the memory area) here - just worked out how long it is
	if (byteCount >= maxUsableMem) {		// max 3200 memory locations (to start of video area)
		message('program too long - found '+Math.floor((byteCount+3)/4)+' words. Max is '+maxUsableMem);
		showError(lineCount+blankLabels);
		return;
	}
	// we also can setup the code/data boundary
	if (dotDataAddress == -1) {
		dotDataAddress = (firstDataDefn == -1)? byteCount: firstDataDefn;
	}
	//alert("dotDataAddress = "+dotDataAddress);

	var r = 0;
	var error = false;
	while (r < lineCount) {
		var inst = instruction(instText[r]);
		if (inst == 200) {
			error = "unknown instruction at line "+convert(r,instText[r]);
			break;
		}
		// for RISC we need to work out the format of the offsetText
		var decode = instKey[inst];
		// 1 = no parameters 2 = Rd,Rx 4 = Rd,#immed(8) 8 = Rd,Rs,#immed(8) 16 = Rd,address
		// 32 = Rd,device 128 = address(label) 256 = Rd,Rs,Rb 512 = Rd,[Rn+offset]
		// 1024 = number(32) 2048 = Rd,Rs,#immed(5)  * rest unused in AQA implementation
		// 64 now used just for push/pop - format is {register list}

		// get base op code
		var I1 = inst1[inst];

		if (!offsetText[r]) {
			//alert("decoding line "+r+" inst "+instText[r]+" no parameter");
			if ((decode&1)==0) {
				error = "parameters needed at line "+convert(r,instText[r]);
				break;
			}
			// no parameters so use I1 directly
			// note that .byte, .block, .ascii and .asciz are currently not allowed without a parameter
			// so we don't need to allow for byte storing here
		} else if (offsetText[r][0] == '"' || offsetText[r][0] == "'") {
			// here for .ascii and .asciz - only one string allowed
			if ((decode&4096)==0) {
				error = "syntax error at line "+convert(r,instText[r]);
				break;
			}
			var i = 1;
			var cnt = 0;
			while (i < offsetText[r].length && offsetText[r][i] != offsetText[r][0]) {
				var curChar = offsetText[r].charCodeAt(i);
				if (offsetText[r][i] == '\\') {
					++i // an escape sequence
					if (offsetText[r][i] == "n") curChar = 10;
					else if (offsetText[r][i] == "r") curChar = 13;
					else if (offsetText[r][i] == "t") curChar = 8;
					else curChar = offsetText[r].charCodeAt(i);
				}
				// note that setAddress sets words				
				setAddressByte(lineToByteAddress[r]+cnt, curChar);
				++i;
				++cnt;
			}
			if (offsetText[r][i] != offsetText[r][0]) { // failed to terminate - pass1 should have trapped this
				error = "bad string at line "+convert(r,instText[r]);
				break;
			}
			//alert("end of string, length = "+offsetText[r].length+" i = "+i+" next char "+offsetText[r][i+1]);
			if (offsetText[r].length > (i+1)) {		// extraneous stuff after end quote
				error = "syntax error at line "+convert(r,instText[r]);
				break;
			}
			if (inst == 29) setAddressByte(lineToByteAddress[r]+cnt, 0);	// .asciz
			else --cnt;
			setData(indexToLine[r],'0x'+padHex(lineToByteAddress[r],5)+'-0x'+padHex(lineToByteAddress[r]+cnt,5));
			++r;
			continue;			// byte aligns must use continue

		} else if (offsetText[r][0] == '{') {	// PUSH/POP register list only
			if (inst != 22 && inst != 23) {		// PUSH is 22, POP is 23
				error = "syntax error at line "+convert(r,instText[r]);
				break;
			}
			// need to change the RISC code because register can now be 2 or 3 characters
			for (var i = 1; i < offsetText[r].length; ++i) {
				if (offsetText[r][i] == '}') {
						if (offsetText[r].length > (i+1)) {		// extraneous stuff after end bracket
						error = "syntax error at line "+convert(r,instText[r]);
						// need double break!
					}
					break;
				}
				if (offsetText[r][i] == ' ') continue;
				if (offsetText[r][i] == ',') continue;
				var rlen = 2;			// allow for Rnn
				if (offsetText[r][i+2] >= '0' && offsetText[r][i+2] <= '9') ++rlen;
				var reg1 = regDecode(offsetText[r].substring(i,i+rlen));
				// returns 0-15 for R0 to R15, also 15=PC 13=SP 14=LR, 20 if NaN else 21
				if (i > (offsetText[r].length-rlen) || reg1 > 15 ||
				    (offsetText[r][i+rlen] !='-' &&
					offsetText[r][i+rlen] !=',' &&
					offsetText[r][i+rlen] !=' ' &&
					offsetText[r][i+rlen] !='}')) {
					error = "syntax error at line "+convert(r,instText[r]);
					break;
				}
				if (offsetText[r][i+rlen] !='-') { // simple case
					i += rlen-1;
					I1 |= (1<<reg1);
				} else {				// reg1-reg2 case
					i += rlen+1;
					rlen = 2;			// allow for Rnn
					if (offsetText[r][i+2] >= '0' && offsetText[r][i+2] <= '9') ++rlen;
					var reg2 = regDecode(offsetText[r].substring(i,i+rlen));
					if (i > (offsetText[r].length-rlen) || reg2 > 15 ||
					    (offsetText[r][i+rlen] !=',' &&
						offsetText[r][i+rlen] !=' ' &&
						offsetText[r][i+rlen] !='}') ||
					    reg1 > reg2) {
						error = "syntax error at line "+convert(r,instText[r]);
						break;
					}
					i += rlen-1;
					while (reg1 <= reg2) {
						I1 |= (1<<reg1);
						++reg1;
					}
				}
			}
			if (error) break;
			// end of copy from RISC POP/PUSH
		} else {
			var pos1 = offsetText[r].indexOf(',',0);
			var a = 0;
			if (pos1 == -1) {				// no commas - so one parameter only
				var reg = regDecode(offsetText[r]);
				// returns 0-15 for R0 to R15 (or SP=13, LR=14, PC=15), 20 if NaN else 21
				//alert("decoding line "+r+" inst "+instText[r]+" one parameter reg "+reg);
				if (reg == 21) {		// it is numeric
					if ((decode&1024)==0 ||
					    isNaN(a=parseIntGen(offsetText[r])) ||
					    a > 4294967295 || a < -4294967296) {
						error = "syntax error at line "+convert(r,instText[r]);
						break;
					}
					// .byte only allows one constant value (at the moment)
					if (inst == 27) {
						if (a > 255 || a < 0) {
							error = "illegal byte value at line "+convert(r,'');
							break;
						}
						setAddressByte(lineToByteAddress[r], a);	// .byte
						setData(indexToLine[r],'0x'+padHex(lineToByteAddress[r],5));
						++r;
						continue;			// byte aligns must use continue
					}
					// .block reserves n zero bytes
					if (inst == 19) {
						if (a <= 0 || (a+lineToByteAddress[r]) > maxUsableMem) { // pass 1 should have trapped this!!
							error = "illegal .block count "+convert(r,'');
							break;
						}
						for (var i = 0; i < a; ++i) {
							setAddressByte(lineToByteAddress[r]+i, 0);		// ?? is this really needed
						}
						setData(indexToLine[r],'0x'+padHex(lineToByteAddress[r],5)+'-0x'+padHex(lineToByteAddress[r]+a-1,5));
						++r;
						continue;			// byte aligns must use continue
					}
					if (inst == 35) {
						++r;
						continue;	// nothing to do here for .align (memory was init to 0)
					}
					I1 += a;
				} else if (reg == 20) {	// it is a label or .+/-n
					//alert("label at line "+r+" inst "+instText[r]+" one parameter reg "+reg+" param "+offsetText[r]);
					var nxt = offsetText[r].substring(0,1);
					if ((decode&128)==0 || nxt == '#') {
						error = "syntax error at line "+convert(r,instText[r]);
						break;
					}
					// only branch instructions and .WORD have 1 parameter labels (or .+/-n)
					if (nxt == '.') {									// check if the .+/-n case
						// might have a .label
						var dotLabelIndex = matchDotLabel(offsetText[r]);
						if (dotLabelIndex != -1 && I1 == 0) {		// -1 means not found, so have a .[I/Olabel] or .Pixelnn
											// it is silly to branch to a .label so don't allow it
							if (dotLabelIndex & 0x80000000) I1 = dotLabelIndex;
							else I1 = dotLabelValues[dotLabelIndex];
							// no usage check possible here
						} else if ((dotLabelIndex = getColourVal(offsetText[r])) != -1 && I1 == 0) {
							I1 = dotLabelIndex;
						} else {		// must be a number (or error if not)
							if (offsetText[r].length == 1) {
								a = 0;
							} else {
								nxt = offsetText[r].substring(1,2);				// + or -
								var nxt1 = offsetText[r].substring(2,3);		// a digit
								a = parseIntGen(offsetText[r].substring(2));	// decode a number
								// parseIntGen is now better at accepting or rejecting but do we allow .+x66 for example
								if  ((nxt!='+' && nxt!='-') || nxt1<'0' || nxt1>'9' || isNaN(a)) {
									error = "bad address offset at line "+convert(r,'');
									break;
								}
								if (nxt=='-') a = -a;
							}
							var b = (lineToByteAddress[r]>>2) + a;			// real dest address (in words)
							if (b < 0 || b >= (maxUsableMem/4)) {
								error = "bad address offset at line "+convert(r,'');
								break;
							}
							// alert("a="+a+"  b="+b+"  parsing "+offsetText[r].substring(1)+"   PC at "+(lineToByteAddress[r]>>2));
							if (I1 == 0) { 					// (byteOpt)?4*a:a;		// .WORD case
								I1 = b * 4;					// need byte address of absolute destination
							} else {						// branch case
								var a1 = a-2;
								if (a1 < -0x7fffff || a1 > 0x7fffff) {		// check the offset is in range
									error = "bad address offset at line "+convert(r,'');
									break;
								}
								I1 += a1 & 0x00ffffff;	// need offset only (in words)
							}
						}
					} else {							// should be a label
						for (a = 0; a < lineCount; ++a) {
							if (offsetText[r] == labelText[a]) break;
						}
						if (a >= lineCount) {
							error = "unknown address label at line "+convert(r,'');
							break;
						}
						// only branch instructions and .WORD have 1 parameter labels
						if (I1 == 0) { 			// (byteOpt)?4*a:a;		// .WORD case
							I1 = lineToByteAddress[a];
						} else {						// branch case
							if ((lineToByteAddress[a]&3) != 0) {
								error = "unaligned address at line "+convert(r,instText[r]);
								break;
							}
							var b = ((lineToByteAddress[a]>>2) - (lineToByteAddress[r]>>2) - 2);		// value of PC for offset
							if (b < -0x7fffff || b > 0x7fffff) {		// check the offset is in range
								error = "address out of range at line "+convert(r,'');
								break;
							}
							I1 += b & 0x00ffffff;
						}
					}
				//} else if (reg>15) {
				//	error = "syntax error at line "+convert(r,instText[r]);
				//	break;
				} else {				// R0 to R15
					//if ((decode&64)==0){	// no current instructions of this form
						error = "syntax error at line "+convert(r,instText[r]);
						break;
					//}
					//I1 += 1<<reg;
				}
			} else {
				var pos2 = offsetText[r].indexOf(',',pos1+1);
				var reg1 = regDecode(offsetText[r].substring(0,pos1));

				// need to detect the special case of Rd,[Rn,#n] - i.e. a second comma is there!!
				if (offsetText[r].substring(pos1+1,pos1+2) == '[') pos2 = -1;

				if (pos2 == -1) {			// 1 comma - so two parameters
					// returns 0-15 for R0 to R15 (or SP=13, LR=14, PC=15), 20 if NaN else 21
					var reg2 = regDecode(offsetText[r].substring(pos1+1));
					// alert("line "+r+" inst "+instText[r]+" decode "+decode+" reg1 "+reg1+" reg2 "+reg2+" param "+offsetText[r]);
					if (reg1 > 15) {  // all AQA and ARMlite 2 parameter instructions have Rd first
						error = "syntax error at line "+convert(r,instText[r]);
						break;
					}
					if (reg2 < 16) {	// is MOV/MVN/CMP Rx, Ry
						if ((decode&2)==0){
							error = "syntax error at line "+convert(r,instText[r]);
							break;
						}
						if (inst == 2) reg1 *= 16;	// CMP and MOV/MVN have r1 in different places
						I1 += (reg1 << 12) + reg2;
					} else if (reg2 == 21) {		// it is numeric LDR/STR Rd,n
						// now no INP/OUT and LDR/STR/LDRB/STRB can access I/O memory 
						a=parseIntGen(offsetText[r].substring(pos1+1));
						//if ((decode&32)!=0 && !isNaN(a) && a < 16 && a >= 0) { 	// valid INP/OUT
						//	I1 += (reg1 << 20) + a; } else
						if (!isNaN(a) && a >= 0x80000000) {			// I don't know whether a will be +ve or -ve
							a -= 0x100000000;		// so make it -ve if it was of the form 0xfffffxxx
						}
						if ((decode&16)!=0 && !isNaN(a) && a < maxUsableMem) {	// no point in checking I/O it will fail the range check first
									// valid LDR/STR/LDRB/STRB
									// if you don't use a name I'm not going to check I/O type
							
							// Since the pixel and char arrays may move we don't support accessing them
							// as numeric addresses. It is questionable whether the I/O should be
							// allowed but I coded it that way and they are fixed so leave it.
							// Change - since we now want to do more with "just AQA" allow all valid numeric addresses

							if ((a&3) != 0 && (inst == 14 || inst == 15)) 	{		// LDR or STR with bad address
								error = "Unaligned access at line "+convert(r,instText[r]);
								break;
							}

							a -= lineToByteAddress[r] + 8;			// relative address of label
							if (a > 4095 || a < -4095) {
								error = "range error at line "+convert(r,instText[r]);
								break;
							}
							if (a >= 0) I1 += 0x800000 + a;
							else I1 += (-a);
							I1 += reg1<<12;
						} else {
							error = "syntax error at line "+convert(r,instText[r]);
							break;
						}
					} else if (reg2 == 20) {	// a label or a #imm or [R+label]
						var nxt = offsetText[r].substring(pos1+1,pos1+2);
						if (nxt == '#') {		// Rx,#immediate MOV/MVN/CMP
							// need an instruction type check here
							if ((decode&4)==0){
								error = "syntax error at line "+convert(r,instText[r]);
								break;
							}
							nxt = offsetText[r].substring(pos1+2,pos1+3);
							if  ((nxt>='0' && nxt<='9')|| nxt=='+' || nxt=='-') {	// decode a number
								a=checkImm(offsetText[r].substring(pos1+2));	// returns 12 bits or -1 bad number or -2 out of range
								// alert("line "+r+" inst "+instText[r]+" decode "+decode+" reg1 "+reg1+" a "+a+" param "+offsetText[r]);
								if (a == -2 && inst == 3) {			// check if in range for extended MOV Rx,#imm
									a = parseIntGen(offsetText[r].substring(pos1+2));
									if (isNaN(a)) {		// this should not happen with -2 but leave just in case
										error = "syntax error at line "+convert(r,instText[r]);
										break;
									}
									// make 0xffffxxxx addresses negative 
									if (a > 0x80000000) a -= 0x100000000;
									// now three cases to support - first the 16 bit v8 extension cases
									if (a < 0 && a >= -0xffff) {
										a = -a;
										I1 = (reg1 << 12) + 0xE3200000 + (a & 0xfff) + ((a << 4) & 0xf0000);
									} else if (a >= 0 && a <= 0xffff) {
										//	reg splits up the immediate - how strange
										I1 = (reg1 << 12) + 0xE3000000 + (a & 0xfff) + ((a << 4) & 0xf0000);
									} else if (a < -0x2000000 || a > 0x1ffffff || (reg1 >= 13 && a < 0)) {
										// don't allow -ve numbers in PC, LR or SP
										error = "immediate out of range at line "+convert(r,instText[r]);
										break;
									} else {  // 26 bit (non-standard) ARMlite extension - 2s comp
										I1 = (reg1 << 28) + 0x0C000000 + (a & 0x3ffffff);
									}
								} else {							
									if (a<0) {
										error = "syntax error at line "+convert(r,instText[r]);
										break;
									}
									if (inst == 2) reg1 *= 16;	// CMP and MOV/MVN have r1 in different places
									I1 += (reg1 << 12) + a + 0x2000000;
								}
							} else {					// allow a label here
								var b;
								var label = offsetText[r].substring(pos1+2);
								if (label.substring(0,1) == '.') {		// #.label/colour
									var dotLabelIndex = matchDotLabel(label);
									if (dotLabelIndex != -1) {		// -1 means not found, so have a .[I/Olabel] or .Pixelnn
										if (dotLabelIndex & 0x80000000) b = dotLabelIndex;
										else b = dotLabelValues[dotLabelIndex];
										// no usage check possible here because just an immediate
										// note b could be in I/O space or not here!
									} else if ((b = getColourVal(label)) == -1) {
										error = "unknown dot label at line "+convert(r,'');
										break;
									}
								} else {
									for (a = 0; a < lineCount; ++a) {
										if (label == labelText[a]) break;
									}
									if (a >= lineCount || (decode&4)==0) {
										error = "unknown address label at line "+convert(r,'');
										break;
									}
									b = lineToByteAddress[a];
								}
								a=checkImm(''+b); // -1 bad number or -2 out of range
								if (a ==-2 && inst == 3) { // can try the extended MOV instructions
									// make 0xffffxxxx addresses negative 
									if (b > 0x80000000) b -= 0x100000000;
									// now two ca ses to support - first the 16 bit v8 extension
									if (b < 0 && b >= -0xffff) {
										b = -b;
										I1 = (reg1 << 12) + 0xE3200000 + (b & 0xfff) + ((b << 4) & 0xf0000);
									} else if (b >= 0 && b <= 0xffff) {
										//	reg splits up the immediate - how strange
										I1 = (reg1 << 12) + 0xE3000000 + (b & 0xfff) + ((b << 4) & 0xf0000);
									} else if (b < -0x2000000 || b > 0x1ffffff || (reg1 >= 13 && b < 0)) {
										// don't allow -ve numbers in PC, LR or SP
										error = "too many bits in address at line "+convert(r,instText[r]);
										break;
									} else {  // 26 bit (non-standard) ARMlite extension - 2s comp
										I1 = (reg1 << 28) + 0x0C000000 + (b & 0x3ffffff);
									}
								} else {
									if (a < 0) {
										error = "too many bits in address at line "+convert(r,instText[r]);
										break;
									}
									if (inst == 2) reg1 *= 16;	// CMP and MOV/MVN have r1 in different places
									// now hopefully fixed to reject bad addresses
									I1 += (reg1 << 12) + a + 0x2000000;
								}
							}
						} else if (nxt == '[') {		// Rd, [Rx+optional label/+/-number]
							var pos3 = offsetText[r].indexOf(']',pos1+1);
							var pos4 = offsetText[r].indexOf('+',pos1+1);
							var pos6 = offsetText[r].indexOf('-',pos1+1);		// try to support '-' as well
												// we cannot have both a + and a -
							if (pos3 == -1 || (decode & 512) == 0 || (pos4 != -1 && pos6 != -1) ||
									offsetText[r].length > (pos3+1)) {			// text after closing ]
								error = "syntax error at line "+convert(r,instText[r]);
								break;
							}
							var sign = 1;
							if (pos6 != -1) {			// we have a minus (remember we may have just a comma)
								pos4 = pos6;
								sign = -1;
							}

							// The ARM format is actually [Rn,#nnn] where nnn can have +/-
							// Try to allow both formats for now
							var pos5 = offsetText[r].indexOf(',',pos1+1);
							if (pos5 != -1 && (pos4 == -1 || pos5 < pos4)) pos4 = pos5;

							if (pos4 != -1 && pos4 > pos3) pos4 = -1; // + after ] (strange)
							if (pos4 ==-1) {
								reg2 = regDecode(offsetText[r].substring(pos1+2,pos3));
								a = 0;			// no label
							} else {
								reg2 = regDecode(offsetText[r].substring(pos1+2,pos4));
								nxt = offsetText[r].substring(pos4+1,pos4+2);
								if (nxt == '#') {
									++pos4;
									nxt = offsetText[r].substring(pos4+1,pos4+2);
									if (nxt == '+' || nxt == '-') {
										++pos4;
										nxt = offsetText[r].substring(pos4+1,pos4+2);
									}
								// I think the next line will fix the [Rn,-Rm] UAL case
								} else if (nxt == '-') ++pos4;	// The question is what else it breaks!

								// change to allow for register here as well as number or label
								var reg3 = regDecode(offsetText[r].substring(pos4+1,pos3));
								//alert("reg3 is "+reg3+" Decoding "+offsetText[r].substring(pos4+1,pos3)+" sign "+sign);
								if (reg3 == 21) {				// decode a number								
									a = parseIntGen(offsetText[r].substring(pos4+1,pos3));
									// this now needs to be bytes
									if (isNaN(a) || a < 0 || a > 4095) {
										error = "syntax error at line "+convert(r,instText[r]);
										break;
									}
								} else if (reg3 == 20) {		// decode the label
									var label = offsetText[r].substring(pos4+1,pos3);
									// need to support indexing off .PixelScreen and .CharScreen if 
									// in range so might as well support all .labels
									//alert("Looking for "+label+" nxt is "+nxt+" reg2 is "+reg2);
									var dotLabelIndex = matchDotLabel(label);
									if (dotLabelIndex != -1) {	// -1 means not found, so have a .[I/Olabel] or .Pixelnn
										if (dotLabelIndex & 0x80000000) a = dotLabelIndex;
										else a = dotLabelValues[dotLabelIndex];
									} else {
										for (a = 0; a < lineCount; ++a) {
											if (label == labelText[a]) break;
										}
										if (a >= lineCount) {
											error = "unknown address label at line "+convert(r,'');
											break;
										}
										a = lineToByteAddress[a];
									}
									if (sign == -1) {
										error = "cannot subtract a label at line "+convert(r,'');
										break;
									}
									if (a > 0xfffff000) {	// then can be a -ve offset
										sign = -1;
										a = 0x100000000 - a;	// a now +ve
									}
									if (isNaN(a) || a < 0 || a > 4095) {	// now is possible
										error = "label out of range at line "+convert(r,instText[r]);
										// now we can have more memory the form [Rn+label] can be
										// larger than the 12 bits allowed in the instruction format
										break;
									}
								} else {						// decode the register [Rn+Rm] form
									if (reg3 > 15 || reg3 < 0) {		// should not be possible
										error = "unknown 2nd indirect register at line "+convert(r,'');
										break;
									}
									a = reg3;
									I1 += 0x2000000; // change the immediate offset code to the register offset code
								}
							}
							if (reg2 > 15) {
								error = "unknown indirect register at line "+convert(r,'');
								break;
							}
							if (a > 4095 || a < 0) {	// offset can only be +ve
								error = "problem with offset calculation at line "+convert(r,'');
								break;
							}
							I1 = (I1 & 0xfff00000) + (reg2 << 16) + (reg1 << 12) + a;
							if (sign > 0) I1 += 0x800000;
						} else {		// can only be a label (so LDR/LDRB/STR/STRB)
							var nxt = offsetText[r].substring(pos1+1,pos1+2);
							if ((decode&16)==0 || nxt == '#'){	// a better check here
								error = "syntax error at line "+convert(r,instText[r]);
								break;
							}
							// beware that more memory means that offsets can be out of range!
							var label = offsetText[r].substring(pos1+1);
							var dotLabelIndex = matchDotLabel(label);
							if (dotLabelIndex != -1) {		// -1 means not found, so have a .[I/Olabel] or .Pixelnn
								if (dotLabelIndex & 0x80000000) {
									a = dotLabelIndex;					// check word only on .Pixelnn
									if (inst != 14 && inst != 15) {		// only allow LDR and STR
										error = "bad I/O address for operation at line "+convert(r,'');
										break;
									}
								} else {
									a = dotLabelValues[dotLabelIndex];
									var code = dotLabelMode[dotLabelIndex];
									// code has bits - 1=write word, 2=write byte, 4=read word, 8=read byte (allowed)
									// 16=base address (that we don't implement yet)
									if (inst == 14) code &= 4;			// LDR
									else if (inst == 15) code &= 1;		// STR
									else if (inst == 25) code &= 8;		// LDRB
									else if (inst == 26) code &= 2;		// STRB
									else code = 0;
									if (code == 0) {
										error = "bad I/O address for operation at line "+convert(r,'');
										break;
									}
								}
								if (a > 0x80000000) a -= 0x100000000;	// make "a" small negative number
								// assume our table contains only valid addresses (so no range check)
								// obviously the alignment check should not error!
							} else {	// normal label
								for (a = 0; a < lineCount; ++a) {
									if (label == labelText[a]) break;
								}
								if (a >= lineCount) {
									error = "unknown address label at line "+convert(r,'');
									break;
								}
								a = lineToByteAddress[a];
								if (isNaN(a) || a < 0 || a >= maxUsableMem) {	// should not be possible
									error = "label out of range at line "+convert(r,instText[r]);
									break;
								}
							}
							if ((I1 & 0x400000) == 0 && (a & 3) != 0) {		// bit 22 is 1 for byte, 0 for word
								error = "label not word aligned at line "+convert(r,instText[r]);
								break;
							}
							//alert("a is "+a+" PC is "+lineToByteAddress[r]);
							a -= lineToByteAddress[r] + 8;		// relative address of label
							// with more memory we need an extra error check here
							if (a < -4095 || a > 4095) {	// out of range
								error = "label out of range at line "+convert(r,instText[r]);
								break;
							}				
							if (a >= 0) I1 += 0x800000 + a;
							else I1 += (-a);
							I1 += reg1<<12;							
						}
					} else {		// reg2 should not have any other values
						error = "unknown problem at line "+convert(r,'');
						break;
					}
				} else {		// two commas so three parameters
					var reg2 = regDecode(offsetText[r].substring(pos1+1,pos2));
					var reg3 = regDecode(offsetText[r].substring(pos2+1));
					//alert("line "+r+" inst "+instText[r]+" decode "+decode+" reg1 "+reg1+" reg2 "+reg2+" reg3 "+reg3+" param "+offsetText[r]);
					if (reg2 > 15) {
						error = "2nd parameter not a valid register at line "+convert(r,'');
						break;
					}
					if (reg3 < 16) {
						if ((decode&256)==0) {
							error = "syntax error at line "+convert(r,instText[r]);
							break;
						}
						// here with three valid registers
						I1 += reg1<<12;
						if ((I1&0x0fe00000) == 0x01A00000) { // LSR, LSR etc. with 3 regs (using MOV)
							// change above from ff to fe to allow for possible support of LSRS etc.
							I1 += (reg3<<8) + 0x10 + reg2;
						} else {				// ADD SUB etc with 3 regs
							I1 += (reg2<<16) + reg3;
						}
					} else if (reg3 == 	20 && offsetText[r].substring(pos2+1,pos2+2) == '#') {		
						// two registers and a constant
						if ((decode&(8+2048))==0) {		// test first to get better errors
							error = "syntax error at line "+convert(r,instText[r]);
							break;
						}
						nxt = offsetText[r].substring(pos2+2,pos2+3);
						if  (nxt>='0' && nxt<='9') {	// decode a number
							a=checkImm(offsetText[r].substring(pos2+2));	// returns 12 bits or -1 bad number or -2 out of range
							//alert("line "+r+" inst "+instText[r]+" decode "+decode+" reg1 "+reg1+" reg2 "+reg2+" a "+a+" param "+offsetText[r]);
						} else {					// allow a label here
							for (a = 0; a < lineCount; ++a) {
								if (offsetText[r].substring(pos2+2) == labelText[a]) break;
							}
							if (a >= lineCount) {
								error = "unknown address label at line "+convert(r,'');
								break;
							}
							if ((a=checkImm(''+lineToByteAddress[a])) == -1) {	// -1 bad number or -2 out of range
								error = "too many bits in address at line "+convert(r,instText[r]);
								break;
							}
						}
						if (a == -1) {
							error = "syntax error at line "+convert(r,instText[r]);
							break;
						}
						if ((decode&8)==0) {		// doing a shift instruction
							if ((I1&0x60)==0 || (I1&0x60)==0x60) { // LSL or ROR 1-31 only
								if (a == 0 || a > 31) {			// RRX is not supported (? no imm5 anyway)
									error = "immediate must be 1-31 at line "+convert(r,instText[r]);
									break;
								}
								// 0 means different instructions for LSL (MOV) and for ROR (RRX)
							} else {								// LSR or ASR 1-32 allowed
								if (a == 0 || a > 32) {
									error = "immediate must be 1-32 at line "+convert(r,instText[r]);
									break;
								}
								if (a == 32) a = 0;					// 32 codes as 0!!
								// I genuinely cannot think of why you would need 32 but that is the spec
								// on the other hand you don't need 0 either
								// the only thing I can see 32 doing is setting the C bit differently
							}
							// Note that the requirement for shifts is a <= 32 and
							// checkImm will return a unchanged for a <= 255 (0 shift)
							// if you are daft enough to shift with #label it must be <=32
						}
						if (a<0) {
							error = "immediate more than 8 bits at line "+convert(r,instText[r]);
							break;
						}

						I1 += reg1<<12;
						if ((I1&0x0fe00000) == 0x01A00000) { // LSR or LSL 2 regs, imm5 (using MOV)
							// change above from ff to fe to allow for possible support of LSRS etc.
							I1 += (a<<7) + reg2;		// NOTE & 0xfff00000 failed to match!!
						} else {				// ADD SUB etc with 2 regs, imm8
							I1 += (reg2<<16) + a + 0x2000000;    // also set immediate bit
						}
					} else {		// no other valid combinations of 3 parameters
						error = "syntax error at line "+convert(r,instText[r]);
						break;
					}
				}
			}
		}
		// here only if we have built a 32 bit word aligned value, everything else must use continue;
		// we should be aligned here because pass1 is supposed to sort that out
		if ((lineToByteAddress[r]&3) != 0) {if (debug) alert("issue: line "+r+" badly aligned "+lineToByteAddress[r]);}
		setAddress(lineToByteAddress[r], I1);		// may have to detect .WORD here - but could be code!!
		// was a problem here that we only kept "blankLabels" in pass 1 - so use another array
		// I tried backconverting using the address but that did not work for odd addresses
		setCode(indexToLine[r],'0x'+padHex(lineToByteAddress[r],5));		
		++r;
	}
	if (error) {
		var max = Math.floor((byteCount+3)/4);
		for (var i = 0; i <= max; ++i) {					// clear the memory
			setAddress(i*4, 0);
		}
		byteCount = 0;
		message(error);
		showError(indexToLine[r]);		// showError needs actual line number
	} else {
		updateR(13,maxUsableMem);
		message("Program assembled. Run or Step to execute");
	}
}

// The old "lineAddress" is always true now - just hope I delete the right bits

// Give line number or address depending on which display mode we are in
// This version used by step1 gets given PC/4
function convert2(n)
{
	if (n*4 >= byteCount) {				// beyond assembled code
		return "unknown (PC=0x"+padHex((n*4),5)+")";				// all returns used as text strings
	}
	return addrToLine[n];
}

// decode potential Register specifier returns 0-15 for R0 to R15, 20 if NaN else 21
// Also SP=13, LR=14, PC=15
function regDecode(text)
{
	//alert("regDecode passed "+text);
	if (text == 'PC'||text == 'pc') return 15;
	if (text == 'SP'||text == 'sp') return 13;
	if (text == 'LR'||text == 'lr') return 14;
	//if (text == 'flags'||text == 'FLAGS') return 16;
	if (text.length == 2 && (text[0] == 'R' || text[0]=='r')) {
		var x = text[1]-'0';
		if (x >= 0 && x < 10) return x;
	}
	if (text.length == 3 && (text[0] == 'R' || text[0]=='r') && text[1] == '1') {
		var x = text[2]-'0';
		if (x >= 0 && x < 6) return x+10;
	}
	if (isNaN(text)) return 20;
	return 21;
}

// check and encode the #immediate to see if it fits the ARM shift plus 8 bits format
// returns 12 bits if OK, -1 if bad number or -2 if out of range
function checkImm(s)
{
	var a = parseIntGen(s);
	if (isNaN(a)) {/*alert('s='+s+' a='+a);*/ return -1;}
	if (a < 0) {
		a += 0x100000000;		// convert to 32 bit unsigned
		if (a < 0) return -2;	// might be too negative
	}
	if ((a&0xffffff00)==0) return a&255;
	var sh = 0;
	while (++sh<16) {
		var t = (a>>30)&3;
		a <<= 2;
		a |= t;
		if ((a&0xffffff00)==0) return (a&255)+(sh<<8);
	}
	//alert('Immediate not representable s='+s);
	return -2;
}

// rewrite this for generic number input, since we don't know the field size allow -n as a return
// accept x, 0x, b, 0b and + or - to start. Return NaN on anything at all not acceptable
// just x or b not allowed any more
// entend parseInt to a) allow 0xb and b) check no non-numeric characters
function parseIntGen(s)
{
	var sign = 1;								// default plus
	var radix = 10;
	if (s.length == 0) return NaN;				// unlikely to happen
	if (s[0] == '-') {
		sign = -1;
		s = s.substring(1);
	} else if (s[0] == '+') s = s.substring(1);
	if (s.length == 0) return NaN;
	
	if (s[0] < '0' || s[0] > '9') return NaN;	// do not allow x or b without 0 any longer
	
	if (s.length > 2 && s[0] == '0') {			// need at least 0bn or 0xn for the binary/hex cases
		s = s.substring(1);						// remove the 0 - not worth rewriting all this
	}
	if (s[0] == 'b' || s[0] == 'B') {
		radix = 2;
		s = s.substring(1);
	} else if (s[0] == 'x' || s[0] == 'X') {
		radix = 16;
		s = s.substring(1);
	}
	if (s.length == 0) return NaN;				// just 0x or 0b invalid
	var i = 0;
	while (i < s.length) {						// reject any invalid characters
		if (s[i] < '0') return NaN;
		if (radix == 2) { if (s[i] > '1') return NaN; }
		else if (radix == 10) { if (s[i] > '9') return NaN; }
		else {
			if ((s[i] > '9' && s[i] < 'A') || (s[i] > 'F' && s[i] < 'a') || s[i] > 'f') return NaN;
		}
		++i;
	}
	return sign*parseInt(s, radix);
}
	
// convert the encoded immediate back to a real value (entry 0xsnn where n = number of double rotates)
function decodeImm(s)
{
	var sh = (s>>8)&15;
	s &= 255;
	while (sh>0) {
		var t = s&3;
		s = (s>>2)&0x3fffffff;
		s |= t<<30;
		--sh;
	}
	return s;
}

// update PC
function updatePC(y)
{
	while (y < 0) y += 4294967296;		// this sort of mimics what hardware might do
	while (y >= maxUsableMem) y -= maxUsableMem;	// but probably we always check before getting here
	pCounter = y;
	if (dontDisplay == 0 && !serviceMode) updateRDisplay(15,y);
}

// update a register - now all registers are valid
function updateR(r,y)
{
	if (r == 15) {
		updatePC(y);
		//updatePCmarker(pCounter);		// do not change until do the fetch
		return;
	}
	if (r >= 0 && r < 15) {
		while (y < 0) y += 4294967296;
		while (y > 4294967295) y -= 4294967296;
		register[r] = y;
		//if (r > 12 && byteOpt == 0) y /= 4;	// byteOpt : 0 = address 32 bit words, 1 = address bytes
		if (dontDisplay == 0 && !serviceMode) updateRDisplay(r,y);
	}
}

// Dec 2019 - we decided to remove DAT as a fake instruction so it becomes .WORD

// AQA instructions
var instructions = ["ADD", "SUB", "CMP", "MOV", "AND", "ORR", "EOR", "LSR", "LSL", "B",
"BEQ", "BNE", "BLT", "BGT", "LDR", "STR", "HALT", "MVN",
// extras implemented here (RFE and .BLOCK replace INP and OUT)
"RFE", ".BLOCK", ".WORD", "BL", "PUSH", "POP", "RET", "LDRB", "STRB", ".BYTE", ".ASCII", ".ASCIZ",
"ADDS", "SUBS", "BCS", "BVS", "BMI", ".ALIGN", "SVC", "ASR", "ROR", "RRX",
"LSRS", "LSLS", "ASRS", "RORS", "RRXS",			// now support all the "set flags" versions of shifts
// duplicates
"BIS", "XOR", "OR", "JMS", "HLT", "ASL", "ASLS"];

// to assist indexing, put 10 per line
// note the last 6 instructions are fake - any new instruction goes before the BIS
// 00 "ADD", "SUB", "CMP", "MOV", "AND", "ORR",  "EOR", "LSR",  "LSL",   "B",
// 10 "BEQ", "BNE", "BLT", "BGT", "LDR", "STR",  "HALT","MVN",  "RFE",   ".BLOCK",
// 20 ".WORD","BL", "PUSH","POP", "RET", "LDRB", "STRB",".BYTE",".ASCII",".ASCIZ",
// 30 "ADDS","SUBS","BCS", "BVS", "BMI", ".ALIGN","SVC", "ASR", "ROR",	 "RRX",
// 40 "LSRS", "LSLS","ASRS","RORS","RRXS","BIS", "XOR",  "OR",  "JMS", 	 "HLT",
// 50 "ASL", "ASLS"
// Care if change the numbers - some are hard coded in assemble()

// ADD(S), SUB(S), AND, ORR, EOR, LSR(S), LSR(S) are the only three register operations
// now ASR(S) and ROR(S) are also 3 register instructions - same rules as LSR and LSL
// RRX(S) is a two register instruction - treat like MOV Rd,Rs maybe
// LDR(B), STR(B) are the only direct memory instructions (plus branches)

// A key to assist decoding instruction operands - bit significant
// 1 = no parameters 2 = Rd,Rx 4 = Rd,#immed(8) 8 = Rd,Rs,#immed(8) 16 = Rd,address 32 = spare
// 64 = Rx (push/pop) 128 = address(label) 256 = Rd,Rs,Rb 512 = Rd,[Rn+offset] 1024 = number(32)
// 2048 = Rd,Rs,#immed(5) 4096 = ASCII or ASCIZ string
// previous values from RISC simulator 4096 = Rs/Rd can be SP/LR/PC/flags (MOV only - unused)
// 8192 = Rd can be SP (2 param inst - unused) 16384 = Rn can be SP (unused), 32 = Rd,device (now unused)
var instKey = [8+256,8+256,6,6,8+256,8+256,8+256,256+2048,256+2048,128,
	/*10*/ 128,128,128,128,16+512,16+512,1,6,1,1024,
	/*20*/ 1+128+1024,128,64,64,1,16+512,16+512,1024,4096,4096,
	/*30*/ 8+256,8+256,128,128,128,1024,1025,256+2048,256+2048,2,
	/*40*/ 256+2048,256+2048,256+2048,256+2048,2];

// Note 8 changed meaning from RISC code!!!!!! and 64 now just push/pop

// Instruction tables for the basic op codes
var inst1 = [0xE0800000,0xE0400000,0xE1500000,0xE1A00000,0xE0000000,0xE1800000,0xE0100000,
	0xE1A00020,0xE1A00000,0xEA000000,/*10*/0x0A000000,0x1A000000,0xBA000000,0xCA000000,0xE51F0000,
	0xE50F0000,0xE1000070,0xE1E00000,0xF8BD0A00,0,/*20*/0,0xEB000000,0xE92D0000,
	0xE8BD0000,0xE1A0F00E,0xE55F0000,0xE54F0000,0,0,0,/*30*/0xE0900000,0xE0500000,0x2A000000,
	0x6A000000,0x4A000000,0,0xEF000000,0xE1A00040,0xE1A00060,0xE1A00060,
	/*40*/0xE1B00020,0xE1B00000,0xE1B00040,0xE1B00060,0xE1B00060];

// Note 0xF8BD0A00 is the ARM v8 RFE instruction to unstack just one word

/* ADD/CMP etc - base is 2(CMP etc)/3(ADD etc) registers, +0x02000000 for immed8
   LSR/LSL etc - base is immed5, +0x10 for 3 registers
   LDR/STR base for +ve offset, +0x02000000 for negative offset, for indexed replace the F reg
*/

// note map BIS(45)=>ORR(5), XOR(46)=>EOR(6), OR(47)=>ORR(5), JMS(48)=>BL(21), HLT(49)=>HALT(16),
// ASL(50)=>LSL(08) and ASLS(51)=>LSLS(41)
// match the text to a known instruction, return 0-19,21-26,30-44 for instructions, 20,27-29 for directives
// and 200 for no match (use 200 so if increase basic instructions do not change it)
// Note 0-17 are AQA instructions in case we add a "strict AQA" mode
function instruction(text)
{
	for (var k = 0; k < 52; ++k) {
		if (text == instructions[k]) break;
	}
	if (k < 45) return k;		// first because most common case
	if (k == 45) return 5;
	if (k == 46) return 6;
	if (k == 47) return 5;
	if (k == 48) return 21;
	if (k == 49) return 16;
	if (k == 50) return 8;
	if (k == 51) return 41;
	return 200;
}

// routine to work out the length and alignment of an instruction, returns -1 if normal instruction or .WORD
// so .byte or .ascii directives that can be variable length, returns length if byte aligned
// now extended for .block as well - returns -3 for bad .block/.align parameter - also needs byteCount from caller
// returns -2 if could not find a string with .ascii or .asciz
// called as getLen(instText[lineCount], offsetText[lineCount]) - put here because depends on instruction() codes
function getLen(instStr, offset, bCnt)				// INITIAL VERSION - ONE ITEM PER LINE ONLY
{
	var inst = instruction(instStr);
	if (inst == 19) {		// .block reserves n zero bytes
		if (firstDataDefn == -1) firstDataDefn = bCnt;			// .block
		// offset (i.e. text) should be a number (and nothing else)
		var a = parseIntGen(offset);
		if (isNaN(a) || a <= 0 || (a+bCnt) > maxUsableMem) return -3;
		return a;
	}
	if (inst == 35) {		// .align is a bit like .block
		// offset (i.e. text) should be a number (and nothing else)
		var a = parseIntGen(offset);		
		if (isNaN(a) || a <= 0 || a > 0xfffffff) return -3;		// .align 0 does not really make any sense
		// 7ffffff is no longer a design limit but trap silly values > 256M here!
		// odd numbers really dont make any sense either but why bother trapping them
		var b = bCnt%a;
		if (b == 0) a = bCnt;
		else a += bCnt - bCnt%a;
		//alert("Old address "+bCnt+" new address "+a);
		if (a > maxUsableMem) return -3;					// error if too big
		return (a-bCnt);									// need to return bytes to add
	}
	if (inst < 27 || inst > 29) {
		if (inst == 20 && firstDataDefn == -1) firstDataDefn = bCnt;	// .word
		return -1;	// not .byte or .ascii/z
	}
	if (inst == 27) {
		if (firstDataDefn == -1) firstDataDefn = bCnt;			// .byte
		return 1;				// .byte // INITIAL VERSION - ONE ITEM PER LINE ONLY
	}
	// now we have an ascii string
	if (offset[0] != '"' && offset[0] != "'") {
		if (debug) alert("Bad call to getLen, offset = "+offset); // later remove these alerts - caller will detect and abort
		return -2;
	}
	var i = 1;
	var cnt = 0;
	while (i < offset.length && offset[i] != offset[0]) {
		if (offset[i] == '\\') ++i // an escape sequence
		++i;
		++cnt;
	}
	if (offset[i] != offset[0]) {
		if (debug) alert("Bad call to getLen, offset = "+offset); // later remove these alerts - caller will detect and abort
		return -2;
	}
	if (inst == 28) return cnt;			// .ascii
	if (inst == 29) return cnt+1;		// .asciz
	if (debug) alert("Bad call to getLen, inst = "+inst+" i = "+i);
	return -1;
}

//pixelMask = 0; // low byte for Pixel interrupt (2 set/1 poll enable/0 int enable)
//interruptRequest = 0; // one bit for each type of request PIN=0, Char=1, Timer=2, Pixel=3
function pixelInt(inp)
{
	if (inp.id[0] != 'p') {
		if (debug) alert("Bad pixel click decode");
		return;
	}

	// if we are single stepping then we need to be able to accept this at all times
	if ((pixelMask & 1) != 0 && IOVectors[4] >= 0) {	// ignore unless enabled and ISR setup
		interruptRequest |= 4;				// but remember even if master interrupt not enabled (yet)
	} else if ((pixelMask & 2) != 0) {	// non-interrupt I/O
		pixelMask |= 4;
	} else {
		// this should never happen but was used when debugging by forcing always setup
		lastPixelClick = parseInt(inp.id.substring(1));	// TEMP to debug this bit
		if (debug) alert("Pixel click when not enabled "+lastPixelClick);		
		return;		// don't want this
	}
	// now we have to work out which pixel was clicked
	lastPixelClick = parseInt(inp.id.substring(1));
}

// routine to just save one byte
function setAddressByte(x, y)
{
	//alert("setAddressByte called x = "+x+" y = "+y);
	var yy = 255<<((x&3)*8);			// mask for byte we are changing
	var xx = (y<<((x&3)*8)) & yy;		// new value in correct position
	var zz = address[Math.floor(x/4)] & (0xffffffff^yy);
	setAddress(x&0xffffffc, xx|zz);
}

// change the contents of the address x in the memory to y
function setAddress(x, y)
{
	if (isNaN(y)) {if (debug) alert("attempt to store NaN at "+x); return;}
	if (x<0 || x>=maxUsableMem) {if (debug) alert("store at bad address "+x); return;}
	if ((x & 3) != 0) {if (debug) alert("store at unaligned address "+x+" caller should trap this"); return;}
	x = x/4;
	
	while (y < 0) y += 4294967296;
	while (y > 4294967295) y -= 4294967296;	
	address[x] = y;

	if (dontDisplay == 1 || serviceMode) return;

	var base = overlay * 64;
	if (x < base) return;
	x -= base;
	if (x >= 128) return;			// saving an area outside the displayed range

	updateDisplayedMemory(x, y);
}

function Run(maxSteps)		// Service interface
{
	RunResult.run_id = "";		// Unused by ARMlite, so always set an empty string
	RunResult.stdout = "";
	if (waitingForInput) {		// bad sequence
		RunResult.stderr = "Waiting for input";
		RunResult.outcome = 35;		// Invalid
		return RunResult;
	}
	// use the single step code so we can count
	dontDisplay = 1;	// so selects fast path options
	step1Code = 0;				// set non-zero for RFE, HALT and some other cases
		// 0=error, 1=HALT, 2=Breakpoint, 3=interrupt, 4=input, 5=NOP (MOV r0,r0)
	
	stopping = false;	// Not sure whether we really need these
	noKeyEffects = true;

	RunResult.formattedsource = programHTML;	// source reformatted	
	RunResult.cmpinfo = "";
	
	while (--maxSteps >= 0) {
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
				RunResult.outcome = 35;		// Interrupt not supported yet
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
		// NOTE WE ARE NOT CHECKING FOR INTERRUPTS YET
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
function InputText(val)
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

// check stack valid before handling any interrupt
function badStack() 		// return true if error
{
	var addr = (register[13] & 0xfffffffc) - 4;
	if (addr >= maxUsableMem || addr < 0) {
		if (dontDisplay == 1) {
			if (myMaybe) clearTimeout(myMaybe);
			myMaybe = false;
			dontDisplay = 0;
			rewriteMemoryAndRegs(false);
		}
		message("Bad SP value on interrupt");	// do same as step1() error
		// buttons are not dynamic any more
		running = false;
		setStatePaused();
		noKeyEffects = false;
		return true;
	}
	return false;
}

function doClockInt()			// handle detected clock interrupt
{
	xTime = 0;		// if for some reason the interrupt has gone away just clear xTime and return (PC unchanged so no interrupt)
	if (clockIntFreq==0 || IOVectors[3]<0 || (IOVectors[3]&3)!=0 || IOVectors[3]>=maxUsableMem) return;
	interruptMask &= 0xfffffffe;		// will be same in all handlers
	var addr = (register[13] & 0xfffffffc) - 4;
	setAddress(addr, (flags<<28)|1);
	addr -= 4;
	setAddress(addr, pCounter);
	updateR(13, addr);
	updatePC(IOVectors[3]);
	message("Accepted interrupt from the timer");	
}

function doInterrupt()			// normally will change PC (but may leave alone if interrupt not valid)
{								// caller should call badStack() first to trap errors
	if ((++interruptCnt)&1 != 0 || xTime == 0 || xTime >= Date.now()) {
		// force the clock interrupt to be taken alternate times (here if no clock interrupt or alternate times)
		
		var addr = (register[13] & 0xfffffffc) - 4;		// prev badStack() code was here and did this

		// The click the button interrupt
		if ((interruptRequest&1)!=0 && (pinMask&1)!=0 && IOVectors[1]>=0 &&
			(IOVectors[1]&3)==0 && IOVectors[1]<maxUsableMem) {
			// need to do something like a function call but PUSH PC and flags
			interruptMask &= 0xfffffffe;		// will be same in all handlers
			interruptRequest &= 0xfffffffe;		// will be different in all handlers
			setAddress(addr, (flags<<28)|1);
			addr -= 4;
			setAddress(addr, pCounter);
			updateR(13, addr);
			updatePC(IOVectors[1]);
			message("Accepted interrupt from the push button");
			return;
		}
			
		// The Keyboard interrupt
		if ((interruptRequest&2)!=0 && (keyboardMask&1)!=0 && IOVectors[2]>=0 &&
			(IOVectors[2]&3)==0 && IOVectors[2]<maxUsableMem) {
			// need to do something like a function call but PUSH PC and flags
			interruptMask &= 0xfffffffe;		// will be same in all handlers
			interruptRequest &= 0xfffffffd;		// will be different in all handlers
			setAddress(addr, (flags<<28)|1);
			addr -= 4;
			setAddress(addr, pCounter);
			updateR(13, addr);
			updatePC(IOVectors[2]);
			message("Accepted interrupt from the keyboard");
			return;
		}

		// The pixel area click interrupt
		if ((interruptRequest&4)!=0 && (pixelMask&1)!=0 && IOVectors[4]>=0 &&
			(IOVectors[4]&3)==0 && IOVectors[4]<maxUsableMem) {
			// need to do something like a function call but PUSH PC and flags
			interruptMask &= 0xfffffffe;		// will be same in all handlers
			interruptRequest &= 0xfffffffb;		// will be different in all handlers
			setAddress(addr, (flags<<28)|1);
			addr -= 4;
			setAddress(addr, pCounter);
			updateR(13, addr);
			updatePC(IOVectors[4]);
			message("Accepted interrupt from the pixel click");
			return;
		}		
	}
	// The clock interrupt can be serviced now
	if (xTime && (xTime < Date.now())) doClockInt();	
}

function Reset()	// interface Reset call
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

function ClearSystem()
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

function GetRegister(n)
{
	if (n >=0 && n <15) return register[n];
	if (n == 15) return pCounter;
	throw new Error('Illegal register number');
}

// flags	// N Z C V
function GetN()
{
	if (flags&8) return true;
	return false;
}
function GetZ()
{
	if (flags&4) return true;
	return false;
}
function GetC()
{
	if (flags&2) return true;
	return false;
}
function GetV()
{
	if (flags&1) return true;
	return false;
}

function GetMemory(loc)
{
	// note might extend to lowLim later
	// lowLim = vaddressBase-0x100000000;
	if (loc < 0 || loc >= maxUsableMem) throw new Error('bad address');
	return address[loc];
}

// v1address[] starts at vaddressBase and is pixelAreaSize (dynamic) long
// and v2address[] starts at charBase and goes to IOBase (fixed size)
// var v1address = [];						// mid-res or hi-res pixel/video memory
// var v2address = [];						// char map and low-res pixel memory

// GetPixel(n) gives you what LDR Rd,n would give you (and this is a copy of that code!!)
function GetPixel(addr)
{
	// assume given a 32 bit unsigned address
	if (addr < (pixelBase+4*pixelAreaSize) && addr >= pixelBase) // is pixel memory in I/O area
		return v1address[(addr-pixelBase)/4];

	if (addr < IOBase && addr >= charBase) 					// is char or low-res pixel area
		return v2address[(addr-charBase)/4];

	// note the gap between v1address and v2address just drops through to error
	// but the gap between the char and low-res pixel areas just reads white
	throw new Error('Bad pixel or char address');
}

// I have added function GetpixelAreaSize() : int because otherwise you cannot work out whether
// the program changed the resolution or what range is valid for GetPixel(n).
// pixelAreaSize defaults to 3072 but the program can change to 12288 (words and pixels)
function GetPixelAreaSize()
{
	return pixelAreaSize;
}

function GetPixelScreen()
{
	return pixelBase;
}
function GetCharScreen()
{
	return charBase;
}

function GetConsoleOutput()
{
	return output1;
}

function reset3()					// internal reset - non-display parts
{
	updateR(0,0);
	updateR(1,0);
	updateR(2,0);
	updateR(3,0);
	updateR(4,0);
	updateR(5,0);
	updateR(6,0);
	updateR(7,0);
	updateR(8,0);
	updateR(9,0);
	updateR(10,0);
	updateR(11,0);
	updateR(12,0);
	updateR(13,maxUsableMem);
	updateR(14,0);
	updatePC(0);	// (not editable now anyway) if open for editing this replaces the input
	updateFlags(0);
	output1 = "";
	clearIR();
	lastKey = 0;					// clear the read key
	if (waitingForInput) {
		waitingForInput = false;
	}
	clearInputArea();
	// reset the interrupt state
	interruptRequest = 0;		// one bit for each type of request PIN=0, Char=1, Timer=2
	interruptMask = 0; // low bit (0) is master on/off
	keyboardMask = 0	// other devices now have status words
	pinMask = 0;
	clockIntFreq = 0;
	pixelMask = 0; // low byte for Pixel interrupt (2 set/1 poll enable/0 int enable)
	// note Syscall is not masked but if you get one without setting it up system will halt
	IOVectors[0] = -1;			// -1 means not setup yet
	IOVectors[1] = -1;
	IOVectors[2] = -1;
	IOVectors[3] = -1;
	IOVectors[4] = -1;
	pixelAreaSize = 3072;	// reset default size
	clearPixelArea();		// need to do after the pixelMask = 0
	lastPixelClick = -1;
	instructionCount = 0;
	setStateReady();
}

function clearPixelArea()
{
	setPixelAreaSize(pixelAreaSize);	// does a clear
	// The vaddress[] starts at vaddressBase and goes to IOBase - now split!
	// clear the main pixel area 
	var cnt = pixelAreaSize;			// pixels==words
	for (var i = 0; i < cnt; ++i) {
		// serious bug - 0 is BLACK so now we do compare first we don't write black!
		// so we need to set white!
		v1address[i] = 0xffffff;
	}
	// clear the charmap
	cnt = charMax/4;
	var i = 0;
	while (i < cnt) { v2address[i] = 0; ++i; }
	clearCharMap();				// clear the character mapped area
	
	// now clear the gap and the fake (low-res, direct addressed) pixel area
	cnt = (IOBase - charBase)/4;	// v2address is continuous, but gap writes trap
	while (i < cnt) { v2address[i] = 0xffffff; ++i; }
}

// Note 0xF8BD0A00 is an ARM v8 RFE instruction to unstack two words as return from exception
// 0xE3200000 is a variant of 16 bit MOV that I'm using for two's compl. They replace IN and OUT.

// New (I hope) faster decode algorithm - use 2nd nibble to select a start point in the table
// If it is there we do a max of about 6 compares, for a miss we go to end (but we are stopping anyway)

/* this is the theory:
	0 or 2 "ADD","SUB","AND","EOR","ADDS","SUBS", 0-5
	1 "MOV1","LSR","LSL","ASR","RRX","ROR","HALT", 6-12
	1 or 3 "ORR","MOV3","MVN","CMP", 6-16 or 13-16
	3 "MOV2","MOV4",  17-18
	5 "LDR1","STR1","LDRB1","STRB1", 19-22
	7 "LDR2","STR2","LDRB2","STRB2", 23-26
	8 "POP","RFE", 27-28
	9 "PUSH", 29
	A the branches "BEQ","BNE","BLT","BGT","BCS","BVS","BMI","B", 30-37 (maybe "B" first)
	B "BL", 38
	F "SVC", 39
	C,D,E (and F), "MOV5" 40
*/

// before new shifts var startFm = [0,6,0,10,37,16,37,20,24,26,27,35,37,37,37,36];
// Note - to avoid doubling the number of shifts we ignore the S for the shift instructions
var startFm = [0,6,0,13,40,19,40,23,27,29,30,38,40,40,40,39];

var newIValue = [0xE0800000,0xE0400000,0xE0000000,0xE0100000,0xE0900000,0xE0500000,
	/*06*/0xE1A00000,0xE1A00020,0xE1A00000,0xE1A00040,0xE1A00060,0xE1A00060,0xE1000070,
	/*13*/0xE1800000,0xE1A00000,0xE1E00000,0xE1500000,0xE3200000,0xE3000000,
	/*19*/0xE5100000,0xE5000000,0xE5500000,0xE5400000,
	/*23*/0xE7100000,0xE7000000,0xE7500000,0xE7400000,0xE8BD0000,0xF8BD0A00,0xE92D0000,
	/*30*/0xEA000000,0x0A000000,0x1A000000,0xBA000000,0xCA000000,0x2A000000,0x6A000000,0x4A000000,
	/*38*/0xEB000000,0xEF000000,0x0C000000,0];

var newIMask = [0xFDF00000,0xFDF00000,0xFDF00000,0xFDF00000,0xFDF00000,0xFDF00000,
	/*06*/0xFFFF0FF0,0xFFEF0060,0xFFEF0060,0xFFEF0060,0xFFEF0FF0,0xFFEF0060,0xFFFFFFFF,
	/*13*/0xFDF00000,0xFDFF0000,0xFDFF0000,0xFDF0F000,0xFFF00000,0xFFF00000,
	/*19*/0xFF700000,0xFF700000,0xFF700000,0xFF700000,
	/*23*/0xFF700000,0xFF700000,0xFF700000,0xFF700000,0xFFFF0000,0xFFFFFFFF,0xFFFF0000,
	/*30*/0xFF000000,0xFF000000,0xFF000000,0xFF000000,0xFF000000,0xFF000000,0xFF000000,0xFF000000,
	/*38*/0xFF000000,0xFF000000,0x0C000000,0];

var newINmame = [	"ADD",	"SUB",	"AND",	"EOR",	"ADDS",	"SUBS",
	/*06*/	"MOV",	"LSR",	"LSL",	"ASR",	"RRX",	"ROR",	"HALT",
	/*13*/	"ORR",	"MOV",	"MVN",	"CMP",	"MOV",	"MOV",
	/*19*/	"LDR",	"STR",	"LDRB",	"STRB",
	/*23*/ 	"LDR",	"STR",	"LDRB",	"STRB",	"POP",	"RFE",	"PUSH",
	/*30*/	"B",	"BEQ",	"BNE",	"BLT",	"BGT",	"BCS",	"BVS",	"BMI",
	/*38*/	"BL",	"SVC",	"MOV",	"ILLEGAL"];

// Parameter type 0 = HALT/RFE/SVC, 1 = [Rn+Rm], 2 = LDR/STR (with F or other register), 3 = MOV/MVN,
// 4 = CMP, 5 = LSR/LSL, 6 = B/BEQ etc., 7 = ADD/SUB/EOR etc. 8 = 16bit MOV, 9 = PUSH/POP, 10 = 26bit MOV
//										11					21					31					 41
var newIDecode = [7,7,7,7,7,7,3,5,5,5,5,5,0,7,3,3,4,8,8,2,2,2,2,1,1,1,1,9,0,9,6,6,6,6,6,6,6,6,6,0,10,0];

// MOV Rd,Rm needs to be matched first to remove it from LSL Rd,Rn,#x which is identical if x=0
// Note we ONLY support push and pop - not any other form of STM or LDM at the moment

// called from runContinue to do one instruction (no indication), returns !=0 if an error which should cause run to stop
// now also for fastpath uses non-zero return for special cases and a global step1Code tells caller which (caller sets to zero
// before call) values are 0 = error, 1 = HALT, 3 = interrupt enable, 4 = input, 5 = NOP (MOV r0,r0 = 0xE1000070)
// now do 200 instructions per call in the fast path - we are getting 20M inst/sec - so 200 is 10us
// and that is enough granularity for the clock interrupt test
function step1(lim)			// note step did not work but is not listed as a reserved word
{
	// moving these vars to outside the jj loop speeded emulator up by 4.4%
	var newFormat;
	var iy;
	var iz;
	var dreg;
	var nreg;
	var opr2;
	var addr;
	var error = "";		// if this gets changed we are returning in that pass anyway
	var lowLim;
	var result;
	var tmp;
	var flgs;
	var tres;
	var t1;
	var t2;
	
	if (waitingForInput) return 0;		// is this needed??
	setStateForceRunning();				// might have slow set already
	// now do both loops here - call with outer number so start up can use different
for(var ii=0;ii<lim;++ii) {					// loop for the fast code
for (var jj=0;jj<201;++jj) {
	lineNo = Math.floor(pCounter/4);	// tried shift but that was slower
	inst = address[lineNo];
	if (dontDisplay == 0) {
		//setIR(spacedHex(inst, true, true));		// need to change
		setIR(padHex(inst,8));
		updatePC(pCounter+4);			// increment the PC
	} else {
		// less work in fast path - this gave a 5% speed increase over updatePC(pCounter+4);
		pCounter+=4;
		if(pCounter >= maxUsableMem) pCounter = 0;	// not sure whether an error would be better
	}
	
	++instructionCount;				// add 1 to the instruction count

	newFormat = startFm[(inst>>24)&15];
	for (; newFormat < 38; ++newFormat) {
		if ((inst&newIMask[newFormat])==(newIValue[newFormat]&0xffffffff)) break;
		// without the extra &0xffffffff the above comparison failed
	}

	iy = newINmame[newFormat];	// holds instruction name
	iz = "";				// to hold parameter format
	dreg = 100;				// set impossible values as default
	nreg = 100;				// some parameter types set some of these
	opr2 = NaN;				// opr2 is the "value", the rest are the addresses of the values
	addr = 10000;
	switch(newIDecode[newFormat]) {	// switch on parameter type
	case 0:							// HALT, RFE or ILLEGAL
		break;
	default:
		if (debug) alert("step1() fault on decode "+newIDecode[newFormat]+" "+newFormat);
		error = "emulator fault";
	case 1:							// LDR(B)/STR(B) with [Rn+/-Rm]
		dreg = (inst>>12)&15;
		nreg = (inst>>16)&15;		// need to allow for regs to be PC
		addr = (nreg < 15) ? register[nreg] : pCounter + 4;
		var ti = inst&15;
		ti = (ti < 15) ? register[ti] : pCounter + 4;
		if ((inst&0x800000)!=0) {	// +ve offset
			addr += ti;
			iz = "&nbsp;Rd,[Rn+Rm]";
		} else {
			addr -= ti;
			iz = "&nbsp;Rd,[Rn-Rm]";
		}
		// duplicate of next case
		if (addr > 0x80000000) addr -= 0x100000000;	// sometimes addr comes out -ve and sometimes +ve as 0xffffxxxx
											// make the second case -ve even though most following code switches back
		lowLim = vaddressBase-0x100000000;	// now just one memory map
		//alert("addr is "+addr+" lowLim is "+lowLim);				
		if (addr < lowLim || addr >= maxUsableMem) error = "bad address";
		break;
	case 2:							// LDR(B)/STR(B) (with F or other register)
		addr = inst&4095;			// relative to register
		dreg = (inst>>12)&15;
		nreg = (inst>>16)&15;
		if (nreg == 15) {			// AQA case - PC relative
			iz = " Rd,addr";
			if ((inst&0x800000)!=0) {	// +ve offset
				addr += (pCounter&0xffffffc) + 4; // some idiot (me) manually set an odd PC
			} else {								// to see what would happen
				addr = (pCounter&0xffffffc) + 4 - addr;
			}
		} else {
			iz = "&nbsp;Rd,[Rn+nn]";
			if ((inst&0x800000)!=0) {	// +ve offset
				addr += register[nreg];
			} else {
				addr = (register[nreg]) - addr;
			}
		}
		if (addr > 0x80000000) addr -= 0x100000000;	// sometimes addr comes out -ve and sometimes +ve as 0xffffxxxx
											// make the second case -ve even though most following code switches back
		lowLim = vaddressBase-0x100000000;
		//alert("addr is "+addr+" lowLim is "+lowLim);				
		if (addr < lowLim || addr >= maxUsableMem) error = "bad address";
		break;
	case 3:							// MOV/MVN
		dreg = (inst>>12)&15;
		if ((inst&0x2000000)!=0) { 	// immediate
			iz = " Rd,#im";
			opr2 = decodeImm(inst & 0xfff);
		} else {
			iz = " Rd,Rm";
			if ((inst&15)<15) opr2 = register[inst&15];
			else opr2 = pCounter + 4;
		}
		break;
	case 4:							// CMP
		nreg = (inst>>16)&15;
		if ((inst&0x2000000)!=0) {	// immediate
			iz = " Rn,#im";
			opr2 = decodeImm(inst & 0xfff);
		} else {
			iz = " Rn,Rm";
			if ((inst&15)<15) opr2 = register[inst&15];
			else opr2 = pCounter + 4;
		}
		break;
	case 5:							// LSR/LSL - NOW all the shifts
		dreg = (inst>>12)&15;
		nreg = inst&15;
		if ((inst&0x100000)!=0) iy += "S";	// LSRS etc.
		if ((inst&16)!=0) {			// not immediate
			iz = " Rd,Rn,Rm";
			if (((inst>>8)&15)<15) opr2 = register[(inst>>8)&15];
			else opr2 = pCounter + 4;
			opr2 &= 255;			// ARM spec says take low 8 bits of register only
			if (opr2 > 32) opr2 = 32;	// p7526 of ARMv8 spec says do this!
		} else {
			iz = " Rd,Rn,#im";
			opr2 = (inst>>7)&31;
			if (opr2==0) {
				if ((inst&0x60)!=0)opr2=32;	// ARM spec again, note ignored for RRX
				if ((inst&0x60)==0x60)iz=" Rd,Rn";	// RRX is just Rd,Rn
			}
		}
		break;
	case 6:							// B/BEQ etc.
		iz = " addr";
		addr = inst&0xffffff;
		if ((addr&0x800000)!=0) addr -= 0x1000000;	// convert 24 bit 2's comp to +/-
		addr = (addr*4) + pCounter + 4;
		if (addr < 0 || addr > maxUsableMem) error = "bad address";
		break;
	case 7:							// ADD/SUB/EOR etc.
		dreg = (inst>>12)&15;
		nreg = (inst>>16)&15;
		if ((inst&0x2000000)!=0) { 	// immediate
			iz = " Rd,Rn,#im";
			opr2 = decodeImm(inst & 0xfff);
		} else {
			iz = " Rd,Rn,Rm";
			if ((inst&15)<15) opr2 = register[inst&15];
			else opr2 = pCounter + 4;
		}
		break;
	case 8:							// extended MOV Rx,#imm
		dreg = (inst>>12)&15;
		iz = " Rd,#im";
		opr2 = (inst & 0xfff) + ((inst >> 4) & 0xf000);
		break;
	case 9:							// push and pop
		dreg = (inst>>16)&15;		// let's be generic in case we ever do LDM and STM
		opr2 = inst & 0xffff;		// register mask
		iz = " {regs}";
		break;
	case 10:						// 26 bit 2's comp MOV (extended)
		dreg = (inst>>28)&15;
		iz = " Rd,#im";
		opr2 = inst & 0x3ffffff;
		if ((inst & 0x2000000) != 0) {
			opr2 |= 0xfc000000;
		}
		if (dreg >= 13) {		// avoid doing strange things that will be hard for users to debug
			// for example (as I did) setting -1 as data which is MOV PC,#-1 so you end up with PC=3
			if ((opr2&3) != 0 || opr2 < 0 || opr2 > maxUsableMem) error = "bad address";
			// In theory you could do some of these things with LR but that would be an encoding starting with
			// E and we do not want to allow that - in any case a RET would then fail!
			// so we allow only valid memory addresses to be set in SP, LR or PC - except SP needs maxUsableMem
		}
		break;
	}
	if (dontDisplay == 0) addIR(iy+iz);		// poss too long - gets more below
	if (error != "") {
		message("Error: "+error+" at line "+ convert2(lineNo));
		return 1;
	}
	//alert("Inst "+iy+" Param "+iz+" dreg "+dreg+" nreg "+nreg+" opr2 "+opr2+" addr "+addr);

	// we get here with the parameters checked and the instruction displayed
	switch(newFormat) {
	case 38:		// BL
		updateR(14, pCounter);
		// drop through to normal B to update PC
	case 30:			// B
		updatePC(addr);
		break;
	case 31:			// BEQ
		if ((flags&4)!=0) {
			updatePC(addr);
		} else iz += ' Branch not taken';
		break;
	case 32:			// BNE
		if ((flags&4)==0) {
			updatePC(addr);
		} else iz += ' Branch not taken';
		break;
	case 33:			// BLT	N set and V clear OR N clear and V set
		if ((flags&9)==8 || (flags&9)==1) {
			updatePC(addr);
		} else iz += ' Branch not taken';
		break;
	case 34:			// BGT	Z clear AND (N and V both set or both clear)
		if ((flags&4)==0 && ((flags&9)==9 || (flags&9)==0)) {
			updatePC(addr);
		} else iz += ' Branch not taken';
		break;
	case 7:			// LSR/LSRS - nreg = source, dreg = dest, opr2 = shift
		result = (nreg<15)?register[nreg]:pCounter+4;
		if (opr2 > 0) {
			// >> does ASR not LSR so we need to do the first shift by hand
			tmp = (result>>1)&0x7fffffff;
			if (opr2 > 1) tmp=tmp>>(opr2-1);
		} else tmp = result // note can be 0 from the regs case
		updateR(dreg, tmp);
		if ((inst&0x100000)!=0) {		// need to update the flags
			flgs = flags & 1;			// V is never changed
			if (opr2 == 0) flgs += flags & 2;	// C unchanged if no shift
			else if ((result & (1<<(opr2-1))) != 0) flgs += 2;
			// care - it seems logical operations sign extend into 64 bits
			if ((tmp&0xffffffff)==0) flgs |= 4;	// Z bit
			if ((tmp&0x80000000)!=0) flgs |= 8;	// N bit
			updateFlags(flgs);
		}
		break;
	// it turns out that 32 case shifts do nothing!!
	case 8:			// LSL/LSLS - nreg = source, dreg = dest, opr2 = shift
		result = (nreg<15)?register[nreg]:pCounter+4;
		if (opr2>31) tmp = 0;
		else tmp = result<<opr2;
		updateR(dreg, tmp);
		if ((inst&0x100000)!=0) {		// need to update the flags
			flgs = flags & 1;			// V is never changed
			if (opr2 == 0) flgs += flags & 2;	// C unchanged if no shift
			else if (((result<<(opr2-1))&0x80000000) != 0) flgs += 2;
			// care - it seems logical operations sign extend into 64 bits
			if ((tmp&0xffffffff)==0) flgs |= 4;	// Z bit
			if ((tmp&0x80000000)!=0) flgs |= 8;	// N bit
			updateFlags(flgs);
		}
		break;
	case 9:			// ASR/ASRS - nreg = source, dreg = dest, opr2 = shift
		result = (nreg<15)?register[nreg]:pCounter+4;
		// >> does ASR but nothing for 32!
		if (opr2>31) tmp = result>>31;
		else tmp = result>>opr2;
		updateR(dreg, tmp);
		if ((inst&0x100000)!=0) {		// need to update the flags
			flgs = flags & 1;			// V is never changed
			if (opr2 == 0) flgs += flags & 2;	// C unchanged if no shift
			else if ((result & (1<<(opr2-1))) != 0) flgs += 2;
			// care - it seems logical operations sign extend into 64 bits
			if ((tmp&0xffffffff)==0) flgs |= 4;	// Z bit
			if ((tmp&0x80000000)!=0) flgs |= 8;	// N bit
			updateFlags(flgs);
		}
		break;
	case 10:		// RRX - nreg = source, dreg = dest, opr2 to be ignored
		result = (nreg<15)?register[nreg]:pCounter+4;
		// >> does ASR so need an AND
		tmp = (result>>1)&0x7fffffff;
		if ((flags&2)!=0) tmp+=0x80000000;
		updateR(dreg, tmp);
		if ((inst&0x100000)!=0) {		// need to update the flags
			flgs = flags & 1;			// V is never changed
			if ((result&1) != 0) flgs += 2;
			// care - it seems logical operations sign extend into 64 bits
			if ((tmp&0xffffffff)==0) flgs |= 4;	// Z bit
			if ((tmp&0x80000000)!=0) flgs |= 8;	// N bit
			updateFlags(flgs);
		}
		break;
	case 11:		// ROR - nreg = source, dreg = dest, opr2 = shift
		result = (nreg<15)?register[nreg]:pCounter+4;
		tmp = opr2;						// need to know if zero for C flag preservation
		while (opr2 > 31) opr2 -= 32;	// can get any 8 bit value from the three regs case
		while (opr2 > 0) {
			// there must be a better way but this will do
			if ((result&1)==0) result = (result>>1)&0x7fffffff;
			else result = ((result>>1)&0x7fffffff)+0x80000000;
			--opr2;
		}
		updateR(dreg, result);
		if ((inst&0x100000)!=0) {		// need to update the flags
			flgs = flags & 1;			// V is never changed
			if (tmp == 0) flgs += flags & 2;	// C unchanged if no shift
			else if ((result&0x80000000) != 0) flgs += 2;
			// care - it seems logical operations sign extend into 64 bits
			if ((result&0xffffffff)==0) flgs |= 4;	// Z bit
			if ((result&0x80000000)!=0) flgs |= 8;	// N bit
			updateFlags(flgs);
		}
		break;
	case 12:			// HALT (note no longer detected by caller)
		step1Code = 1; // 0 = error, 1 = HALT, 2 = Breakpoint, 4 = input, 5 = NOP (MOV r0,r0)
		message(halted);
		return 1;
		break;
	case 28:		// RFE
		addr = register[13] & 0xfffffffc;
		if (addr >= (maxUsableMem-4) || addr < 0) {
			message("Bad SP value at line = "+convert2(lineNo));
			return 1;
		}
		tmp = address[addr/4];
		if (tmp >= maxUsableMem || tmp < 0) {
			message("Bad return address at line = "+convert2(lineNo));
			return 1;
		}
		updatePC(tmp);
		addr+=4;
		tmp = address[addr/4];
		updateFlags((tmp>>28)&15);
		if ((tmp & 1) != 0) interruptMask |= 1; 	// low bit (0) is master on/off		
		updateR(13, addr+4);
		checkClickColour();			// ungrey on interrupt exit (not rare as I first thought)
		checkClockEnabled();		// check if clock interrupts needed and do the setup
		if (dontDisplay) {			// if in fast path take another interrupt if one pending
			// I did consider doing this earlier but all the exceptional cases would be a problem
			// and you could not use doInterrupt() either
			if ((interruptMask&1)!=0 && (interruptRequest!=0 || (xTime && (xTime < Date.now())))) {
				// stack must be good - we just unstacked to it - so check is not needed
				doInterrupt();					// usually changes PC
			}	// continue fast loop
		}
		break;
	case 6:			// two decodes of MOV (now five)
	case 14:		// MOV
	case 18:		// MOV - ARMv8 instruction to get 16bit immediate range
	case 40:		// MOV - ARMlite extension - 26 bit 2's comp
		updateR(dreg, opr2);
		if (dontDisplay && inst == 	0xE1A00000) { // NOP in fastpath is call scheduler
			step1Code = 5;						// in slowpath we reschedule anyway
			return 1;
		}
		break;
	case 17:		// MOV (v8 ARM is sign and magnitude - convert to 2's comp)
		updateR(dreg, 0x100000000-opr2);
		break;
	case 15:		// MVN
		updateR(dreg, opr2 ^ 0xffffffff);
		break;
	case 16:		// CMP
		flgs = 0;
		result = ((nreg<15)?register[nreg]:pCounter+4) - opr2;
		// had lots of problems with this because it seems logical operations sometimes
		// (or always) sign extend into 64 bits
		if ((result&0xffffffff)==0) flgs |= 4;	// Z bit
		tres = (result>>31)&1;
		if (tres!=0) flgs |= 8;					// N (sign) bit
		t1 = (((nreg<15)?register[nreg]:pCounter+4)>>31)&1;
		t2 = ((-opr2)>>31)&1;				// treat as add for C decode
		// C is set if NO borrow (which is same as C from an add of negated 2nd operand)
		if (t1==0) {
			if (t2==1 && tres==0) flgs |= 2;	// C bit
		} else {
			if (t2==1 || tres==0) flgs |= 2;	// C bit
		}
		// V is set if the operands have the different signs and the result
		// is a different sign from the first operand (note t2 is reversed)
		if ((t1 + t2) != 1 && t1 != tres) flgs |= 1;
		updateFlags(flgs);

		// TEMP SHOW WHAT HAPPENED
		/*var res2 = result&0xffffffff;			// 32 bits I hope
		var lst = ['0000','0001','0010','0011','0100','0101','0110','0111','1000','1001','1010','1011','1100','1101','1110','1111'];
		alert('flags '+lst[flgs]+' result '+result+' 1st operand '+register[nreg]+' 2nd operand '+opr2+' res2 '+res2); */
		break;
	case 23:					// LDR with [Rn+Rm]
	case 19:		// LDR		dreg and addr already calculated
		// switched to memory mapped I/O so need to do all the I/O on LDR,LDRB,STR,STRB
		if ((addr & 3) != 0) {
			message("Unaligned access on LDR at line "+convert2(lineNo));
			return 1;
		}
		if (addr < 0) {				// it is an I/O address
			addr += 0x100000000;	// convert to the 32 bit unsigned address
			if (addr < (pixelBase+4*pixelAreaSize) && addr >= pixelBase) { // is pixel memory in I/O area
				updateR(dreg, v1address[(addr-pixelBase)/4]);
			} else if (addr < IOBase && addr >= charBase) { 				// is char or low-res pixel area
				updateR(dreg, v2address[(addr-charBase)/4]);
				// note the gap between v1address and v2address just drops through to the error at the end
				// but the gap between the char and low-res pixel areas just reads white
			} else if (addr == dotLabelValues[5]) {			// .InputNum
				inputNum(dreg, 0);			// original read an input number (-1/-2 string, +1 FP)
				step1Code = 4; // 0 = error, 1 = HALT, 2 = Breakpoint, 4 = input, 5 = NOP (MOV r0,r0)
				stepTxt = "Done instruction "+iy+iz+" at line "+convert2(lineNo);
				return 1;
			} else if (addr == dotLabelValues[26]) {	// .InputFP
				inputNum(dreg, 1);			// original read an input number (-1 string, +1 FP)
				step1Code = 4; // 0 = error, 1 = HALT, 2 = Breakpoint, 4 = input, 5 = NOP (MOV r0,r0)
				stepTxt = "Done instruction "+iy+iz+" at line "+convert2(lineNo);
				return 1;
			} else if (addr == dotLabelValues[6]) {		// .LastKey
				updateR(dreg, lastKey);
			} else if (addr == dotLabelValues[7]) {		// .LastKeyAndReset
				updateR(dreg, lastKey);
				lastKey = 0;
			} else if (addr == dotLabelValues[34]) {	// .LastPixelClicked
				updateR(dreg, lastPixelClick);
			} else if (addr == dotLabelValues[35]) {	// .LastPixelAndReset
				updateR(dreg, lastPixelClick);
				pixelMask &= 0xfffffffb;			// mask off the poll bit				
				lastPixelClick = -1;
			} else if (addr == dotLabelValues[8]) {		// .Random
				updateR(dreg, (Math.random()*0x100000000)&0xffffffff);
			} else if (addr == dotLabelValues[9]) {		// .InstructionCount
				updateR(dreg, instructionCount);
			} else if (addr == dotLabelValues[10]) {	// .Time
				updateR(dreg, Math.floor(Date.now()/1000)-946684800);	// seconds since 1/1/2000
			} else if (addr == dotLabelValues[11]) {	// 	.PinISR			
				updateR(dreg, IOVectors[1]);
			} else if (addr == dotLabelValues[12]) {	// 	.SysISR			
				updateR(dreg, IOVectors[0]);
			} else if (addr == dotLabelValues[13]) {	//	.KeyboardISR
				updateR(dreg, IOVectors[2]);
			} else if (addr == dotLabelValues[14]) {	//	.ClockISR
				updateR(dreg, IOVectors[3]);
			} else if (addr == dotLabelValues[33]) {	//	.PixelISR
				updateR(dreg, IOVectors[4]);
			} else if (addr == dotLabelValues[15]) {	//	.InterruptRegister
				updateR(dreg, interruptMask);
			} else if (addr == dotLabelValues[16]) {	//	.PinMask
				updateR(dreg, pinMask);
			} else if (addr == dotLabelValues[17]) {	//	.KeyboardMask
				updateR(dreg, keyboardMask);
			} else if (addr == dotLabelValues[18]) {	//	.ClockInterruptFrequency
				updateR(dreg, clockIntFreq);
			} else if (addr == dotLabelValues[36]) {	//  .PixelMask
				updateR(dreg, pixelMask);
			} else if (addr == dotLabelValues[37]) {	//	.Resolution
				if (pixelAreaSize == 12288) updateR(dreg, 2);	// hi-res
				else if (pixelAreaSize == 3072) updateR(dreg, 1); // mid-res
				else updateR(dreg, 0);			// should not be possible
			} else if (addr == dotLabelValues[19]) {	//	.CodeLimit
				updateR(dreg, byteCount);
			} else if (addr == dotLabelValues[22]) {	//	.OpenFile
				fileRead = "";
				fileResult = dreg;
				fileOpen();
				waitingForInput = true;			// NOTE - this is required to pause instruction execution
												// step1Code = 4 is (I think) only to exit the fast path
				inst = 0;	// to distinguish step1() case from FETCH/EXECUTE case (obsolete here I think)
				step1Code = 4; // 0 = error, 1 = HALT, 2 = Breakpoint, 4 = input, 5 = NOP (MOV r0,r0)
				stepTxt = "Done instruction "+iy+iz+" at line "+convert2(lineNo);
				return 1;
			} else if (addr == dotLabelValues[23]) {	//	.FileLength
				updateR(dreg, fileRead ? fileRead.length : 0);
			} else if (addr == dotLabelValues[24]) {	//	.ReadFileChar
				if (fileRead) { // we have already read a file, get the next character
					updateR(dreg, fileRead.charCodeAt(0));
					fileRead = fileRead.substr(1);
				} else updateR(dreg, 0xffffffff);		// failed to open file
			} else if (addr == dotLabelValues[28]) {	//	.PixelAreaSize
				updateR(dreg, pixelAreaSize);
			} else {
				message("Bad I/O address for LDR at line "+convert2(lineNo));
				return 1;
			}
		} else {
			updateR(dreg,address[addr/4]);	// trapped &3 earlier
		}
		break;
	case 24:					// STR with [Rn+Rm]
	case 20:		// STR		dreg and addr already calculated
		// now memory mapped I/O so need to do all the I/O on LDR,LDRB,STR,STRB
		if ((addr & 3) != 0) {
			message("Unaligned access on STR at line "+convert2(lineNo));
			return 1;
		}
		if (addr >= 0 && addr < dotDataAddress) {
			message("Attempt to STR into the code area at line "+convert2(lineNo));
			return 1;
		}
		result = (dreg<15)?register[dreg]:pCounter+4;
		if (addr < 0) {				// it is an I/O address
			addr += 0x100000000;	// convert to the 32 bit unsigned address
			if (addr < IOBase && addr >= vaddressBase) { // is video memory in I/O area
				var offst = (addr-vaddressBase)/4;
				if (offst < pixelAreaSize) {		// pixel map area (hi-res or mid-res)
					if (v1address[offst] == result) break;	// *IGNORE if nothing changed*
					v1address[offst] = result;
					videoWrite(offst, result);		// words
				} else {
					offst = (addr-charBase)/4;					
					if (addr > charBase && offst < (charMax/4)) {	// char map
						if (v2address[offst] == result) break;	// *IGNORE if nothing changed*
						v2address[offst] = result;
						// byte addressable character overlay
						addr -= charBase;
						showChar(addr,result&255);		// need to set 4 bytes
						showChar(addr+1,(result>>8)&255);
						showChar(addr+2,(result>>16)&255);
						showChar(addr+3,(result>>24)&255);
					} else if (addr >= fakeBig) {	// direct addressable (faked) big pixel area
						v2address[offst] = result;
						offst = (addr - fakeBig)/2;		// note /2 so have *2 offset
						var same = true;
						if (pixelAreaSize == 3072) {	// 4 medium pixels to set
							// we have [row][col=5 bits][*2=1 bit]
							offst += offst & 0xffffc0;	// skip alternate rows
							same = videoProc(same,offst,result);
							same = videoProc(same,++offst,result);
							offst += 63;
							same = videoProc(same,offst,result);
							same = videoProc(same,++offst,result);
						} else {						// 16 small pixels to set
							offst *= 2;					// small pixel offset
							// we have [row][col=5 bits][*4=2 bits]
							offst += (offst & 0xfffff80)*3;	// skip 3 rows
							same = videoProc(same,offst,result);
							same = videoProc(same,++offst,result);
							same = videoProc(same,++offst,result);
							same = videoProc(same,++offst,result);
							offst += 125;
							same = videoProc(same,offst,result);
							same = videoProc(same,++offst,result);
							same = videoProc(same,++offst,result);
							same = videoProc(same,++offst,result);
							offst += 125;
							same = videoProc(same,offst,result);
							same = videoProc(same,++offst,result);
							same = videoProc(same,++offst,result);
							same = videoProc(same,++offst,result);
							offst += 125;
							same = videoProc(same,offst,result);
							same = videoProc(same,++offst,result);
							same = videoProc(same,++offst,result);
							same = videoProc(same,++offst,result);
						}
						if (same) break;	// *IGNORE if nothing changed*
					} else {
						message("Bad I/O address for STR at line "+convert2(lineNo));
						return 1;
					}
				}
				jj = 1000;			// slow down the fast loop if lots of output
			} else if (addr == dotLabelValues[0]) {		// .WriteSignedNum
				outputNum(result,4);	// 2nd parameter is previous I/O address for that function
				jj = 1000;			// slow down the fast loop if lots of output
			} else if (addr == dotLabelValues[1]) {		// .WriteUnsignedNum
				outputNum(result,5);
				jj = 1000;			// slow down the fast loop if lots of output
			} else if (addr == dotLabelValues[2]) {		// .WriteHex
				outputNum(result,6);
				jj = 1000;			// slow down the fast loop if lots of output
			} else if (addr == dotLabelValues[3]) {		// .WriteChar
				outputNum(result,7);
				jj = 1000;			// slow down the fast loop if lots of output
			} else if (addr == dotLabelValues[4]) {		// .WriteString
				outputNum(result,8);
				jj = 1000;			// slow down the fast loop if lots of output
			} else if (addr == dotLabelValues[25] || addr == dotLabelValues[32]) {	// .ReadString or .ReadSecret
				// We are only allowing to the real memory now
				// we checked the program area already (I/O might be low or high)
				if (dreg > 14 || result < 0 || result > (maxUsableMem-128)) {
					message("Bad address for .ReadString at line "+convert2(lineNo));
					return 1;
				}
				if (result < dotDataAddress) {
					message("Attempt to .ReadString into the code area at line "+convert2(lineNo));
					return 1;
				}				
				if (addr == dotLabelValues[25])inputNum(dreg,-1); // original read an input number (-1 string, +1 FP)
				else inputNum(dreg,-2);							// -2 for read secret (e.g. password)
				// YES - it is an STR to do an INPUT because we send the register contents to the device
				step1Code = 4; // 0 = error, 1 = HALT, 2 = Breakpoint, 4 = input, 5 = NOP (MOV r0,r0)
				stepTxt = "Done instruction "+iy+iz+" at line "+convert2(lineNo);
				return 1;
			} else if (addr == dotLabelValues[27]) {	// .WriteFP
				outputNum(result,3);
				jj = 1000;			// slow down the fast loop if lots of output
			} else if (addr == dotLabelValues[15]) {	//	.InterruptRegister
				interruptMask = result;
				checkClickColour();
				checkClockEnabled();		// check if clock interrupts needed and do the setup
				if (dontDisplay) {			// if in fast path flag interrupt pending
					if ((interruptMask&1)!=0 && (interruptRequest!=0 || (xTime && (xTime < Date.now())))) {
						step1Code = 3; // 0=error, 1=HALT, 2=Breakpoint, 3=interrupt, 4=input, 5=NOP (MOV r0,r0)
						return 1;
					}	// Note I dont believe you can get instant interrupts from the other cases below!
				}		// the word code below is slightly different so this needs testing carefully
			} else if (addr == dotLabelValues[16]) {	//	.PinMask
				pinMask = result;
				checkClickColour();
			} else if (addr == dotLabelValues[17]) {	//	.KeyboardMask
				keyboardMask = result;
			} else if (addr == dotLabelValues[18]) {	//	.ClockInterruptFrequency
				clockIntFreq = result;
				checkClockEnabled();		// check if clock interrupts needed and do the setup

			/* This is the original word code when interruptMask also held 3 device masks which are now separate
			} else if (addr == dotLabelValues[15]) {	// .InterruptRegister
				interruptMask = result;
				checkClickColour();
				checkClockEnabled();		// check if clock interrupts needed and do the setup
				if (dontDisplay) {			// if in fast path test if interrupt pending
					if ((interruptMask&1)!=0 && (interruptRequest!=0 || (xTime && (xTime < Date.now())))) {
						// too complex to call badStack() here (it resets buttons) so do the test ourselves
						var addr = (register[13] & 0xfffffffc) - 4;
						if (addr >= maxUsableMem || addr < 0) {
							// we are stopping anyway so easiest thing is to finish this instruction
							// and do a reschedule - then error will be detected in the outer loop
							stepTxt = "Done instruction "+iy+iz+" at line "+convert2(lineNo);
							return 0;
						}
						doInterrupt();	// in rare cases might not change PC
						// continue fast loop
					}
				} */
			} else if (addr == dotLabelValues[36]) {	//	.PixelMask
				var clear = false;
				if (((pixelMask & 3) == 0 && (result & 3) != 0) ||
					((pixelMask & 3) != 0 && (result & 3) == 0)) clear = true;
				pixelMask = result;
				if (clear) clearPixelArea();
			} else if (addr == dotLabelValues[37]) {	//	.Resolution
				if (result == 2) pixelAreaSize = 12288;		// words and pixels
				else pixelAreaSize = 3072;
				v1address = [];							// reset the array
				clearPixelArea();
			} else if (addr == dotLabelValues[28]) {	// (was .PixelAreaSize) .ClearScreen
				clearPixelArea();
			} else if (addr == dotLabelValues[29]) {	// .WriteFileChar (4 chars)
				fileWriteBuffer += String.fromCharCode(result&255);
				fileWriteBuffer += String.fromCharCode((result>>8)&255);
				fileWriteBuffer += String.fromCharCode((result>>16)&255);
				fileWriteBuffer += String.fromCharCode((result>>24)&255);
			} else if (addr == dotLabelValues[30]) {	// .WriteFile
			// .WriteFileChar (STR or STRB) adds a character to the output buffer and .WriteFile (STR) writes the
			// buffer to a file (with an .asciz string as the suggested file name). Set register -1 to clear the buffer.
				if (result == -1) {
					fileWriteBuffer = [];
				}else if (result < 0 || result >= maxUsableMem) {
					message("Bad I/O data or address for STR at line "+convert2(lineNo));
					return 1;
				} else {
					// need to see if the given file name looks reasonable (and extract it)
					var fName = "";
					for (var i = 0; i < 64; ++i) {	// repeat till get a null, 64 chars or run out of memory
						var z = (address[Math.floor(result/4)]>>((result&3)*8))&255;
						if (z < 32 || z > 127) break;		// valid alphanumerics only, terminate if not (incl 0)
						fName += String.fromCharCode(z);
						++result;
						if (result >= maxUsableMem) break;
					}
					//waitingForInput = true;
					saveFile(fileWriteBuffer,fName)
					fileWriteBuffer = [];
					
					// TRY THIS TO JUST GET A WAIT
					stepTxt = "Done instruction "+iy+iz+" at line "+convert2(lineNo);
					return 0;

					// TEST WHAT THIS DOES
					step1Code = 4; // 0 = error, 1 = HALT, 2 = Breakpoint, 4 = input, 5 = NOP (MOV r0,r0)
					stepTxt = "Writing File";
					message("Saving File");
					return 1;
				}
			} else {				// all the rest must give a valid routine address
				if (result < 0 || result >= maxUsableMem || (result & 3) != 0) {
					message("Bad I/O data or address for STR at line "+convert2(lineNo));
					return 1;
				}
				if (addr == dotLabelValues[11]) {	// .PinISR
					IOVectors[1] = result;
					checkClickColour();				// now may show on ISR set
				} else if (addr == dotLabelValues[12]) {	// .SysISR
					IOVectors[0] = result;
				} else if (addr == dotLabelValues[13]) {	// .KeyboardISR
					IOVectors[2] = result;
				} else if (addr == dotLabelValues[14]) {	// .ClockISR
					IOVectors[3] = result;
					checkClockEnabled();
				} else if (addr == dotLabelValues[33]) {	// .PixelISR
					IOVectors[4] = result;
				} else {
					message("Bad I/O address for STR at line "+convert2(lineNo));
					return 1;
				}
			}
		} else {
			setAddress(addr, result);
		}
		break;
	case 0:			// ADD		for all these we have dreg, nreg and opr2
		updateR(dreg, ((nreg<15)?register[nreg]:pCounter+4)+opr2);		// for AQA do not set flags
		break;
	case 1:			// SUB
		updateR(dreg, ((nreg<15)?register[nreg]:pCounter+4)-opr2);
		break;
	case 4:			// ADDS - ADD with flag setting
		// copy the CMP code (with plus of course)
		flgs = 0;
		result = ((nreg<15)?register[nreg]:pCounter+4)+opr2;
		// had lots of problems with this because it seems logical operations sometimes
		// (or always) sign extend into 64 bits
		if ((result&0xffffffff)==0) flgs |= 4;	// Z bit
		tres = (result>>31)&1;
		if (tres!=0) flgs |= 8;					// N (sign) bit
		t1 = (((nreg<15)?register[nreg]:pCounter+4)>>31)&1;
		t2 = (opr2>>31)&1;
		// C needs to be set if carry
		if (t1==0) {
			if (t2==1 && tres==0) flgs |= 2;	// C bit
		} else {
			if (t2==1 || tres==0) flgs |= 2;	// C bit
		}
		// V is set if the operands have the same sign and the result is different
		if ((t1 + t2) != 1 && t1 != tres) flgs |= 1;
		updateFlags(flgs);
		updateR(dreg, result);
		break;
	case 5:			// SUBS
		// copy the CMP code
		flgs = 0;
		result = ((nreg<15)?register[nreg]:pCounter+4)-opr2;
		// had lots of problems with this because it seems logical operations sometimes
		// (or always) sign extend into 64 bits
		if ((result&0xffffffff)==0) flgs |= 4;	// Z bit
		tres = (result>>31)&1;
		if (tres!=0) flgs |= 8;					// N (sign) bit
		t1 = (((nreg<15)?register[nreg]:pCounter+4)>>31)&1;
		t2 = ((-opr2)>>31)&1;				// treat as add for C decode
		// C is set if NO borrow (which is same as C from an add of negated 2nd operand)
		if (t1==0) {
			if (t2==1 && tres==0) flgs |= 2;	// C bit
		} else {
			if (t2==1 || tres==0) flgs |= 2;	// C bit
		}
		// V is set if the operands have the different signs and the result
		// is a different sign from the first operand (note t2 is reversed)
		if ((t1 + t2) != 1 && t1 != tres) flgs |= 1;
		updateFlags(flgs);
		updateR(dreg, result);
		break;
	case 35:		// BCS
			if ((flags&2)!=0) updatePC(addr);
			else iz += ' Branch not taken';
			break;
	case 36:		// BVS
			if ((flags&1)!=0) updatePC(addr);
			else iz += ' Branch not taken';
			break;
	case 37:		// BMI
			if ((flags&8)!=0) updatePC(addr);
			else iz += ' Branch not taken';
			break;
	case 2:			// AND
		updateR(dreg, ((nreg<15)?register[nreg]:pCounter+4)&opr2);
		break;
	case 13:		// ORR
		updateR(dreg, ((nreg<15)?register[nreg]:pCounter+4)|opr2);
		break;
	case 3:			// EOR
		updateR(dreg, ((nreg<15)?register[nreg]:pCounter+4)^opr2);
		break;
	case 29:		// PUSH
		addr = register[dreg] & 0xfffffffc;
		nreg = 15;	// NOTE the ARM spec says the lowest register goes to the lowest address
		while (nreg >= 0) {	// so we have to push highest register first
			if (opr2&(1<<nreg)) {
				if (addr > maxUsableMem || addr < 4) {
					message("Bad SP value at line = "+convert2(lineNo));
					return 1;
				}
				addr -= 4;
				if (addr < byteCount) {
					message("Stack overflow error at line = "+convert2(lineNo));
					return 1;
				}
				if (nreg == 15) setAddress(addr, pCounter + 4);
				else setAddress(addr, register[nreg]);
			}
			--nreg;
		}
		updateR(dreg, addr);
		break;
	case 27:		// POP
		addr = register[dreg] & 0xfffffffc;
		nreg = 0;	// NOTE the ARM spec says the lowest register goes to the lowest address
		while (nreg < 16) {	// so we have to work upwards from 0
			if (opr2&(1<<nreg)) {
				if (addr >= maxUsableMem || addr < 0) {
					message("Bad SP value at line = "+convert2(lineNo));
					return 1;
				}
				updateR(nreg, address[addr/4]);
				addr += 4;
			}
			++nreg;
		}
		updateR(dreg, addr);
		break;
	case 25:					// LDRB with [Rn+Rm]
	case 21:		// LDRB		dreg and addr already calculated
		// now memory mapped I/O so need to do all the I/O on LDR,LDRB,STR,STRB
		if (addr < 0) {				// it is an I/O address
			addr += 0x100000000;	// convert to the 32 bit unsigned address
			if (addr < (pixelBase+4*pixelAreaSize) && addr >= pixelBase) { // is pixel memory in I/O area
				updateR(dreg,(v1address[Math.floor((addr-pixelBase)/4)]>>((addr&3)*8))&255);
			} else if (addr < IOBase && addr >= charBase) { 				// is char or low-res pixel area
				updateR(dreg,(v2address[Math.floor((addr-charBase)/4)]>>((addr&3)*8))&255);
				// note the gap between v1address and v2address just drops through to the error at the end
				// but the gap between the char and low-res pixel areas just reads white
		
			// the only read byte I/O are .LastKey(6), .LastKeyAndReset(7), .ReadFileChar(24)
			} else if (addr == dotLabelValues[6]) {		// .LastKey
				updateR(dreg, lastKey&255);
			} else if (addr == dotLabelValues[7]) {		// .LastKeyAndReset
				updateR(dreg, lastKey&255);
				lastKey = 0;
			} else if (addr == dotLabelValues[19]) {	//	.ReadFileChar
				if (fileRead) { // we have already read a file, get the next character
					updateR(dreg, (fileRead.charCodeAt(0))&255);
					fileRead = fileRead.substr(1);
				} else updateR(dreg, 0xff);				// failed to open file
			} else {
				message("Bad I/O address for LDRB at line "+convert2(lineNo));
				return 1;
			}
		} else {
			updateR(dreg,(address[Math.floor(addr/4)]>>((addr&3)*8))&255);
		}
		break;
	case 26:					// STRB with [Rn+Rm]
	case 22:		// STRB		dreg and addr already calculated
		if (addr >= 0 && addr < dotDataAddress) {
			message("Attempt to STRB into the code area at line "+convert2(lineNo));
			return 1;
		}
		result = ((dreg<15)?register[dreg]:pCounter+4)&255;
		// now memory mapped I/O so need to do all the I/O on LDR,LDRB,STR,STRB
		if (addr < 0) {				// it is an I/O address
			addr += 0x100000000;	// convert to the 32 bit unsigned address
			// We do not allow STRB to the pixel area, it would be very complex and
			// really only happen when the user makes a mistake by using STRB instead
			// of STR - rather than risk a bug report we will just say NO
			if (addr < (charBase+charMax) && addr >= charBase) {			
				var yy = 255<<((addr&3)*8);				// mask for byte we are changing
				var xx = (result<<((addr&3)*8)) & yy;	// new value in correct position
				var offset = Math.floor((addr-charBase)/4);
				v2address[offset] &= 0xffffffff^yy;
				v2address[offset] |= xx;
				showChar(addr-charBase,result);
				jj = 1000;			// slow down the fast loop if lots of output
			} else if (addr == dotLabelValues[3]) {			// .WriteChar
				outputNum(result,7);
				jj = 1000;			// slow down the fast loop if lots of output
			} else if (addr == dotLabelValues[29]) {	// .WriteFileChar (1 chars)
				fileWriteBuffer += String.fromCharCode(result&255);
			} else {
				message("Bad I/O address for STRB at line "+convert2(lineNo));
				return 1;
			}
		} else {
			setAddressByte(addr, result);
		}
		break;
	case 39:		// SVC call
		addr = (register[13] & 0xfffffffc) - 4;
		if (addr >= maxUsableMem || addr < 0) {
			message("Bad SP address for SVC at line "+convert2(lineNo));
			return 1;
		}
		if (IOVectors[0]>=0 && (IOVectors[0]&3)==0 && IOVectors[0]<maxUsableMem) {
			// need to do something like a function call but PUSH PC and flags
			setAddress(addr, (flags<<28)|(interruptMask&255));
			addr -= 4;
			setAddress(addr, pCounter);
			updateR(13, addr);
			updatePC(IOVectors[0]);
			interruptMask &= 0xfffffffe;		// not sure - lets disable interrupts for now
		} else {
			message("Bad vector address for SVC at line "+convert2(lineNo));
			return 1;
		}
		break;
	default:		// 41 = ILLEGAL and anything that goes wrong
		message("Bad instruction at line "+convert2(lineNo));
		return 1;
		break;
	}
	if (breakpointAddr == pCounter) { // next instruction is breakpoint - stop before
		step1Code = 2; // 0 = error, 1 = HALT, 2 = Breakpoint, 4 = input, 5 = NOP (MOV r0,r0)
		return 1;
	}
	if (serviceMode) return 0;
	if (dontDisplay) continue;		// fast path - 100 instructions at a time
	// here for the slow path
	stepTxt = "Done instruction "+iy+iz+" at line "+convert2(lineNo);
	return 0;
	break;
}
	// here if done 200 instructions - see if need clock interrupt
	if (xTime && (xTime < Date.now()) && (interruptMask&1)!=0) {		// fast path clock interrupt
		// too complex to call badStack() here (it resets buttons) so do the test ourselves
		var addr = (register[13] & 0xfffffffc) - 4;
		if (addr >= maxUsableMem || addr < 0) {
			// we are stopping anyway so easiest thing is to finish this instruction
			// and do a reschedule - then error will be detected in the outer loop
			stepTxt = "Done instruction "+iy+iz+" at line "+convert2(lineNo);
			return 0;
		}
		doClockInt();			// in rare cases might not change PC but will zero xTime anyway
		// continue in outer fast loop
	}
}
	stepTxt = "Done instruction "+iy+iz+" at line "+convert2(lineNo);
	return 0;
}

// save space in the big pixel expansion code
function videoProc(same,offst,result)
{
	if (v1address[offst] == result) return same;
	v1address[offst] = result;
	videoWrite(offst, result);
	return false;
}

// for change to memory mapped I/O we need to make this into a function rather than just on INP
function inputNum(dreg, code)			// original read an input number (0 num, -1 string, +1 FP)
{
	//message("INPUT required");
	registerToInput = dreg;
	waitingForInput = true;
	inst = code;	// originally used to distinguish step1() case from FETCH/EXECUTE case
					// now used for type of input required
	noKeyEffects = false;
	if (!serviceMode) enableInput(code == -2);
}

// new - mode is 3 for floating point 
// mode is 4 for signed, 5 for unsigned, 6 for hex, 7 for character and 8 for string
function outputNum(y,mode)
{
	if (mode == 8) {			// string
		var by;
		while (1) {			// repeat till get a null or run out of memory
			if (y < 0 || y >= maxUsableMem) break;	// ignore/end if out of range
			by = (address[Math.floor(y/4)]>>((y&3)*8))&255;
			if (by == 0) break;					// end on null
			// we used to call output one char from here but it was overloading the scroll logic
			//outputNum(z,7);			// write a byte
			// so now we copy the string and do one call to show the output
			var z = String.fromCharCode(by);
			if (by == 10) {
				output1 += '\n';
			} else if (by > 31) {
				output1 += z;
			}
			++y;
		}
		if (!serviceMode) justConsole();		// does not matter if called when nothing output due to error
		return;
	}
	// Now output1 is the last line of the scrolling output

	// if the user is outputting a number force a space after any previous output
	if (mode != 7 && output1.length > 0 && output1[output1.length-1] != ' ' && output1[output1.length-1] != '\n') {
		output1 += ' ';
	}
	
	switch (mode) {
	case 3:			// temp for float
		if ((y & 0xffffff00) == 0) {
			output1 += '0';
		} else {
			var mantissa = (y & 0xffffff00)/0x80000000;
			var exponent = y & 255;
			while (exponent > 127) {
				mantissa /= 2;
				++exponent;
				exponent&=255;
			}
			while (exponent > 0) {
				mantissa *= 2;
				--exponent;
			}
			y = mantissa.toPrecision(6);
			output1 += y;
		}
		break;
	case 4:
		if (y > 2147483647) {  // negative
			y = 4294967296 - y
			output1 += '-'+y+' ';
		} else {
			output1 += y+' ';
		}
		break;
	case 5:
		output1 += y+' ';
		break;
	case 6:
		output1 += '0x';
		// this is not the neatest but is the easiest extension from the 16 bit routine
		if ((y&0xf0000000)!=0) {output1 += hex[(y>>28)&15];}
		if ((y&0xff000000)!=0) {output1 += hex[(y>>24)&15];}
		if ((y&0xfff00000)!=0) {output1 += hex[(y>>20)&15];}
		if ((y&0xffff0000)!=0) {output1 += hex[(y>>16)&15];}
		if ((y&0xfffff000)!=0) {output1 += hex[(y>>12)&15];}
		if ((y&0xffffff00)!=0) {output1 += hex[(y>>8)&15];}
		if ((y&0xfffffff0)!=0) {output1 += hex[(y>>4)&15];}
		output1 += hex[y%16] + ' ';
		break;
	case 7:
		var z = String.fromCharCode(y);
		if (y == 10) {
			output1 += '\n';
		} else if (y > 31) {
			output1 += z;
		}
		break;
	}

	// after the project restart these messages go to the console area not the charmap
	if (!serviceMode) justConsole();
	//setValue("console",output1);
}


// This code has gone round the houses several times and now this routine will only be used
// by reset1 (changed name). However it was used until recently for all the char map writes
// In the newest design the char map overlays the pixelmap and the char writes go to the console area
// So we can move to having STR and STRB call showChar directly
function clearCharMap()
{
	var x;
	for (var i = 0; i < charMax; ++i) {			// 32x16
		//x = (vaddress[Math.floor(i/4)]>>(8*(i&3)))&255;
		showChar(i,0);
	}
}

function inputSubmit()
{
	var val = getInput();
	if (val === false) return;
	if (!waitingForInput) {
		if (debug) alert("Bad input - not waiting for input");
		return;
	}	
	if (val=="") {
		message("Bad input - must not be empty");
		return;
	}
	processInput(val);	// returns true if OK, else false and (maybe) message
	// but we only care in service mode!
}

function processInput(val)
{
	var value;
	if (inst == 0) {	// old case
		value = parseIntGen(val);
		if (isNaN(value) || value > 4294967295 || value < -2147483648) {
			message("Bad number format or value");
			return false;
		}
	} else if (inst < 0) {		// read string
		value = val
		var len = value.length;
		if (registerToInput > 14 || len == 0) return false;
		var addr = register[registerToInput];
		//alert("xxx addr="+addr+"  len="+len+" string="+value);
		if (len > 127) len = 127;				// max string 127 chars
		if (addr >= 0 && addr < (maxUsableMem-len)) {			// usable address
			// just ignore bad addresses here - we did check before
			var i;
			for (i = 0; i<len; ++i) {
				// note that setAddress sets words
				setAddressByte(addr, value.charCodeAt(i));
				++addr;
			}
			setAddressByte(addr, 0);		// always write a null byte - poss the 128th
		}
		if (!serviceMode) {
			if (inst == -2) inputRestore("");	// secret mode
			else inputRestore(val);
		}
		noKeyEffects = true;			// back to ignore chars mode
		//message("String read into memory at "+addr); - assume same issue as below
		waitingForInput = false;
		return true;
	} else {			// floating point
		var mantissa = parseFloat(val);		// same issues as parseInt - stops but valid at first bad character
		if (isNaN(mantissa)) {
			message("Bad input - must be a floating point number");
			return false;
		}
		var exponent = 0;
		while (mantissa >= 1 || mantissa < -1) {
			mantissa /= 2;
			++exponent;
			if (exponent > 127) break;		// there seem to be loop cases (78e456 looped somewhere)
		}
		while (mantissa < 0.5 && mantissa >= -0.5 && mantissa != 0) {
			mantissa *= 2;
			--exponent;
			if (exponent < -128) break;		// there seem to be loop cases
		}
		//alert("after loop 2 mantissa="+mantissa+"  exponent="+exponent);
		if (exponent > 127) {
			message("Bad input - number out of range");
			return false;
		}
		if (mantissa == 0 || exponent < -128) value = 0;
		else {
			tmp = mantissa * 0x80000000;		// I don't think shift would work here
			value = (tmp & 0xffffff00) + (exponent & 255);
		}
	}
	if (!serviceMode) inputRestore(val);
	noKeyEffects = true;			// back to ignore chars mode
	//the message below never got seen - even in single step
	//message("INPUT value loaded into register "+registerToInput);
	// INP no longer sets flags
	updateR(registerToInput, value);
	waitingForInput = false;
	return true;
}

var coloursNam = ['.background', '.aliceblue', '.antiquewhite', '.aqua', '.aquamarine', '.azure', '.beige', '.bisque', '.black', '.blanchedalmond', '.blue', '.blueviolet', '.brown', '.burlywood', '.cadetblue', '.chartreuse', '.chocolate', '.coral', '.cornflowerblue', '.cornsilk', '.crimson', '.cyan', '.darkblue', '.darkcyan', '.darkgoldenrod', '.darkgray', '.darkgreen', '.darkgrey', '.darkkhaki', '.darkmagenta', '.darkolivegreen', '.darkorange', '.darkorchid', '.darkred', '.darksalmon', '.darkseagreen', '.darkslateblue', '.darkslategray', '.darkslategrey', '.darkturquoise', '.darkviolet', '.deeppink', '.deepskyblue', '.dimgray', '.dimgrey', '.dodgerblue', '.firebrick', '.floralwhite', '.forestgreen', '.fuchsia', '.gainsboro', '.ghostwhite', '.gold', '.goldenrod', '.gray', '.green', '.greenyellow', '.grey', '.honeydew', '.hotpink', '.indianred', '.indigo', '.ivory', '.khaki', '.lavender', '.lavenderblush', '.lawngreen', '.lemonchiffon', '.lightblue', '.lightcoral', '.lightcyan', '.lightgoldenrodyellow', '.lightgray', '.lightgreen', '.lightgrey', '.lightpink', '.lightsalmon', '.lightseagreen', '.lightskyblue', '.lightslategray', '.lightslategrey', '.lightsteelblue', '.lightyellow', '.lime', '.limegreen', '.linen', '.magenta', '.maroon', '.mediumaquamarine', '.mediumblue', '.mediumorchid', '.mediumpurple', '.mediumseagreen', '.mediumslateblue', '.mediumspringgreen', '.mediumturquoise', '.mediumvioletred', '.midnightblue', '.mintcream', '.mistyrose', '.moccasin', '.navajowhite', '.navy', '.oldlace', '.olive', '.olivedrab', '.orange', '.orangered', '.orchid', '.palegoldenrod', '.palegreen', '.paleturquoise', '.palevioletred', '.papayawhip', '.peachpuff', '.peru', '.pink', '.plum', '.powderblue', '.purple', '.red', '.rosybrown', '.royalblue', '.saddlebrown', '.salmon', '.sandybrown', '.seagreen', '.seashell', '.sienna', '.silver', '.skyblue', '.slateblue', '.slategray', '.slategrey', '.snow', '.springgreen', '.steelblue', '.tan', '.teal', '.thistle', '.tomato', '.turquoise', '.violet', '.wheat', '.white', '.whitesmoke', '.yellow', '.yellowgreen', ''];

// colours indexed background=white
// 00 background, aliceblue, antiquewhite, aqua, aquamarine, azure, beige, bisque, black, blanchedalmond,
// 10 blue, blueviolet, brown, burlywood, cadetblue, chartreuse, chocolate, coral, cornflowerblue, cornsilk,
// 20 crimson, cyan, darkblue, darkcyan, darkgoldenrod, darkgray, darkgreen, darkgrey, darkkhaki, darkmagenta,
// 30 darkolivegreen, darkorange, darkorchid, darkred, darksalmon, darkseagreen, darkslateblue, darkslategray, darkslategrey, darkturquoise,
// 40 darkviolet, deeppink, deepskyblue, dimgray, dimgrey, dodgerblue, firebrick, floralwhite, forestgreen, fuchsia,
// 50 gainsboro, ghostwhite, gold, goldenrod, gray, green, greenyellow, grey, honeydew, hotpink,
// 60 indianred, indigo, ivory, khaki, lavender, lavenderblush, lawngreen, lemonchiffon, lightblue, lightcoral,
// 70 lightcyan, lightgoldenrodyellow, lightgray, lightgreen, lightgrey, lightpink, lightsalmon, lightseagreen, lightskyblue, lightslategray,
// 80 lightslategrey, lightsteelblue, lightyellow, lime, limegreen, linen, magenta, maroon, mediumaquamarine, mediumblue,
// 90 mediumorchid, mediumpurple, mediumseagreen, mediumslateblue, mediumspringgreen, mediumturquoise, mediumvioletred, midnightblue, mintcream, mistyrose,
// 100 moccasin, navajowhite, navy, oldlace, olive, olivedrab, orange, orangered, orchid, palegoldenrod,
// 110 palegreen, paleturquoise, palevioletred, papayawhip, peachpuff, peru, pink, plum, powderblue, purple,
// 120 red, rosybrown, royalblue, saddlebrown, salmon, sandybrown, seagreen, seashell, sienna, silver,
// 130 skyblue, slateblue, slategray, slategrey, snow, springgreen, steelblue, tan, teal, thistle,
// 140 tomato, turquoise, violet, wheat, white, whitesmoke, yellow, yellowgreen

var coloursVal = ['0xffffff','0xf0f8ff','0xfaebd7','0x00ffff','0x7fffd4','0xf0ffff','0xf5f5dc','0xffe4c4','0x000000','0xffebcd','0x0000ff','0x8a2be2','0xa52a2a','0xdeb887','0x5f9ea0','0x7fff00','0xd2691e','0xff7f50','0x6495ed','0xfff8dc','0xdc143c','0x00ffff','0x00008b','0x008b8b','0xb8860b','0xa9a9a9','0x006400','0xa9a9a9','0xbdb76b','0x8b008b','0x556b2f','0xff8c00','0x9932cc','0x8b0000','0xe9967a','0x8fbc8f','0x483d8b','0x2f4f4f','0x2f4f4f','0x00ced1','0x9400d3','0xff1493','0x00bfff','0x696969','0x696969','0x1e90ff','0xb22222','0xfffaf0','0x228b22','0xff00ff','0xdcdcdc','0xf8f8ff','0xffd700','0xdaa520','0x808080','0x008000','0xadff2f','0x808080','0xf0fff0','0xff69b4','0xcd5c5c','0x4b0082','0xfffff0','0xf0e68c','0xe6e6fa','0xfff0f5','0x7cfc00','0xfffacd','0xadd8e6','0xf08080','0xe0ffff','0xfafad2','0xd3d3d3','0x90ee90','0xd3d3d3','0xffb6c1','0xffa07a','0x20b2aa','0x87cefa','0x778899','0x778899','0xb0c4de','0xffffe0','0x00ff00','0x32cd32','0xfaf0e6','0xff00ff','0x800000','0x66cdaa','0x0000cd','0xba55d3','0x9370db','0x3cb371','0x7b68ee','0x00fa9a','0x48d1cc','0xc71585','0x191970','0xf5fffa','0xffe4e1','0xffe4b5','0xffdead','0x000080','0xfdf5e6','0x808000','0x6b8e23','0xffa500','0xff4500','0xda70d6','0xeee8aa','0x98fb98','0xafeeee','0xdb7093','0xffefd5','0xffdab9','0xcd853f','0xffc0cb','0xdda0dd','0xb0e0e6','0x800080','0xff0000','0xbc8f8f','0x4169e1','0x8b4513','0xfa8072','0xf4a460','0x2e8b57','0xfff5ee','0xa0522d','0xc0c0c0','0x87ceeb','0x6a5acd','0x708090','0x708090','0xfffafa','0x00ff7f','0x4682b4','0xd2b48c','0x008080','0xd8bfd8','0xff6347','0x40e0d0','0xee82ee','0xf5deb3','0xffffff','0xf5f5f5','0xffff00','0x9acd32'];

// get value from colour name so not reliant on browser to recognise name
// return index so we can store that value in the simulated video memory
// return -1 if not found, note 0 means "no colour" set
function getColourVal(x)
{
	for (i=0;i<200;++i) {
		if (coloursNam[i] == '') return -1;
		if (x == coloursNam[i]) return coloursVal[i];
	}
	return -1;
}
