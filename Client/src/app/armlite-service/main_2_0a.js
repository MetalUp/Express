// Extended AQA Instruction set simulator, ARMlite
// Copyright 2019-23 Peter Higginson (plh256 at hotmail.com)

// Jan 2023 Partition the code into a Service module for Richard's Angular service
// for now keep the outer layer so I can test it, whether this becomes ARMlite is TBD
// First pass, keep the global definitions in the service module and just the setups here

// For historical comments see last V1 mainxxx.js file
// BEWARE you cannot do == NaN (at least not to get the result you expect)

function main()						// main is called when the page is loaded
{
	serviceMode = false;			// to get direct I/O
	// look at any parameters
	var url = new URL(location);
	var foo = url.searchParams.get('slow_delay');
	if (foo) {
		var tmp = parseInt(foo);
		if (tmp > 1 && tmp < 1000) {	// fast uses speed 1
			slowSpeed = tmp;			// default delay for SLOW mode
		}
	}
	foo = url.searchParams.get('fast_delay');	// UNDOCUMENTED FEATURE - default=4
	if (foo) {		// for some future browser we might need to find a better default
		var tmp = parseInt(foo);
		if (tmp > 0 && tmp < 251) {
			delayTime = tmp;			// default parallel delay in FAST mode
		}
	}
	foo = url.searchParams.get('data');
	if (foo) {
		foo = foo.toLowerCase();
		if (foo.startsWith("sig")) memOpt = 0;		// 0 = 2's comp
		if (foo.startsWith("dec")) memOpt = 0;		// 0 = 2's comp
		if (foo.startsWith("uns")) memOpt = 1;		// 1 = unsigned
		if (foo.startsWith("hex")) memOpt = 2;		// 2 = hex
		if (foo.startsWith("bin")) {
			memOpt = 3;								// 3 = binary
			addClass("xxbody","binary");
		}
	}
	foo = url.searchParams.get('mem_k');
	if (foo) {
		var tmp = parseInt(foo);
		if (tmp >= 1 && tmp <= 1024) {
			maxUsableMem = tmp*1024;			// first byte we cannot use
		}
	}
	foo = url.searchParams.get('debug');
	if (foo) {
		var tmp = parseInt(foo);
		debug = tmp;						// debug options
	}
	foo = url.searchParams.get('profile');
	if (foo) {
		addClass("xxbody","profile-"+foo);
	} else {
		// profile=player is broken by the dynamic sizing of the windows
		// any profile is likely to be incompatible with this
		dynamicProgramWidth = true;			// default if no profile
		var newWidth = maxWidth - 826;
		foo = url.searchParams.get('progw');	// option to set program area width
		if (foo) {
			var tmp = parseInt(foo);
			if (tmp >= 300 && tmp <= 3000) {
				newWidth = tmp;				// anything above 3000 just seems silly
				dynamicProgramWidth = false;
			}
		}
		setProgramWidth(newWidth);
	}

	foo = url.searchParams.get('alcom');	// option to align comments
	if (foo) {	// decided to just go with spaces option
		var tmp = parseInt(foo);
		if (tmp > 5 && tmp < 301) {
			comment_align = tmp;
		}
	}

	if (navigator.platform.indexOf('Win32') > -1) binarySize = 100;
	else binarySize = 92;

	foo = url.searchParams.get('binsiz');	// option make binary smaller
	if (foo) {
		var tmp = parseInt(foo);
		if (tmp > 79 && tmp < 101) {
			binarySize = tmp;
		}
	}

	// ?load  - just loads the code into the Program window, leaving it in Edit mode so that the user can then hit Submit and Play
	// ?submit - loads the code and assembles it. When user goes to the App they can either then hit Play button, or correct errors and submit again
	// ?run - loads code, submits and if no errors runs it
	foo = url.searchParams.get('load');		// option to supply some code
	if (!foo) {
		foo = url.searchParams.get('submit');	// option to supply some code and assemble it
		if (!foo) {
			foo = url.searchParams.get('run');	// option to supply some code and run it
			if (foo) autoRun = 3;			// marker to assemble and run
		} else autoRun = 2;					// marker to assemble but not run
	} else autoRun = 1;						// marker to leave in edit mode
	if (foo) {
		programText = decodeURIComponent(foo);
		textToHtml();
	}

	window.document.title = "ARMlite Assembly Language Simulator by Peter Higginson";
	trapKey(9, "tabKey", null);				// trap TAB in program input mode
	trapKey(27, "escKey", null);			// trap ESC to exit from input mode

	// this will do the rest of the screen setup (basic is in html now)
	setupDivs();

	// although these are setup to hex 0x0 by setupDivs() the url might have asked
	// for signed or binary, so do it again
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
	updateR(13,maxUsableMem);		// treat LR and SP as R14 and R13 for AQA
	updateR(14,0);
	updateFlags(0);
	updatePC(0);
	clearIR();
	output1 = "System Messages";	// first Submit etc. resets this
	message("LOAD, EDIT a program or modify memory");
	removeClass("program","edit");
	
	// Cheat for now (maybe always) by passing the globals we have already setup to be done again
	// Excluded display features: memOpt, profile, dynamicProgramWidth, binarySize
	ConfigureSystem(slowSpeed, delayTime, maxUsableMem, debug, comment_align);
	// comment_align (int 6-300) included because can pass back formatted assembly listing

	setStateReady();
	
	// while we clear all the memory
	for (var i=0; i<maxUsableMem; i+=4) {
		setAddress(i,0);
	}

	if (autoRun != 0) {			// marker for load/submit/run URL options
		if (autoRun == 1) {		// leave in edit mode
			openProgram2();		// because Mamoto not yet initialised
		} else {
			assemble();
			if (address[0] != 0 && autoRun == 3) {	//assemble worked and run wanted
				// force a screen update before doing run
				delay(100, "run");
			}
		}
	}
}

// Might as well leave the key handling here for now

// ESCAPE key handler - for all input cases except program requested input
function escKey(e)
{
	if (!waitingForInput) return;					// ignore if not doing input
	if (myTimeout || myMaybe) return;	// ignore if running a program
	e.preventDefault();
	waitingForInput = false;
	setStateReady();
	if (savOpenAddress) {						// has a memory address open
		var x = parseInt(savOpenAddress.id[1]);
		if (savOpenAddress.id.length >= 3) x = x*10 + parseInt(savOpenAddress.id[2]);
		if (savOpenAddress.id.length >= 4) x = x*10 + parseInt(savOpenAddress.id[3]);
		if (savOpenAddress.id.length == 5) x = x*10 + parseInt(savOpenAddress.id[4]);
		x += overlay * 64;
		setAddress(x*4, address[x]);	// removes input form as well
		savOpenAddress = false;
	} else if (modifyingProgram) {					// has the program area open
		modifyingProgram = false;
		textToHtml();
	} /* else {								// must be the PC which is open
		updatePC(pCounter);
		//updatePCmarker(pCounter);
	} - **no PC editing now** */
	message("ESC pressed to abort input");
}

// TAB key handler - for all input cases except program requested input
function tabKey(e)
{
	e.preventDefault();
	// Warning - do not put an alert before the preventDefault
	if (!waitingForInput) return;						// ignore if not doing input
	if (myTimeout || myMaybe) return;	// ignore if running a program
	if (savOpenAddress) {						// has a memory address open
		// let's hope x is the offset in this displayed page pair
		var x = parseInt(savOpenAddress.id[1]);
		if (savOpenAddress.id.length >= 3) x = x*10 + parseInt(savOpenAddress.id[2]);
		if (savOpenAddress.id.length >= 4) x = x*10 + parseInt(savOpenAddress.id[3]);
		if (savOpenAddress.id.length == 5) x = x*10 + parseInt(savOpenAddress.id[4]);
		var inp = document.getElementById("aForm");
		var ok = checkInput(inp.value); // so do not overwrite error message
		loseAddress(inp);				// ignore errors because "alert" wrecks things
		if (x < 127) {					// open next location
			++x;
			if (ok) message("Modifying memory contents");
			savOpenAddress = openNextMem(x);
			waitingForInput = true;
			setStateEdit();
			evPush('An');
		} else {
			waitingForInput = false;	// I think it was a bug that this was missing!
			setStateReady();
		}
	} else if (modifyingProgram) {			// has the program area open
		insertTabInProgramArea();
	} /* else {							// must be the PC
		//var inp = document.getElementById("PCForm");
		PCSubmit();					// might as well accept this
	} */
}

function openProgram()
{
	if (waitingForInput) return;			// ignore if doing input (ours or the program)
	if (myTimeout || myMaybe) return;		// ignore if something running
	evPush('Ed');
	openProgram2();
}

function openProgram2()
{	
	message("Modifying Program Area");
	
	// see if can find scroll position before go to edit
	var source = document.getElementById("source");
	var top = Math.floor(source.scrollTop);		// this is a float not an integer so fix it now
	//alert("scrollTop is "+source.scrollTop);

	setStateEdit();							// normally assemble() will call reset2() to clear this
	openProgramArea(programEdit);
	waitingForInput = true;
	modifyingProgram = true;
	// might need a delay to draw element first - no seems to work
	if (top) document.getElementById("pForm").scrollTop = top;
}

function programSubmit()
{
	if (!waitingForInput) {
		if (debug) alert("Bad - Submit when not waiting for input");
		return false;
	}
	programSubmit2(true);
}

// key is true for real Submit, false if part of Save
function programSubmit2(key)
{
	waitingForInput = false;
	modifyingProgram = false;
	removeClass("program","edit");		// put the edit button back
	programText = getProgramArea();
	// test for the DEMO Easter Egg
	if (key && programText.length < 10) {		// very short inputs only
		var foo = programText.toLowerCase();	// and not part of save
		if (foo.startsWith("demo")) {
			evPush('Dm');
			demoSetup();
			textToHtml();
			assemble();
			// force a screen update before doing run
			delay(100, "runOrig");
			return;
		}
	}
	if (key) evPush('Sb');
	else evPush('SvSb');
	textToHtml();
	assemble();
}

function demoSetup()
{
	programText = 'MOV R11,#.black// Constant\nMOV R12,#.white// Constant\nMOV R1,#screen2 \nADD R3,R1,#12288// End\nclearPixel:\nSTR R12,[R1]// set everything white\nADD R1,R1,#4\nCMP R1,R3\nBLT clearPixel\n// Initialise 2nd screen with random pattern\nMOV R2,#screen2// 1st pixel\nADD R3,R2,#12288// End\nrandLoop:LDR R0,.Random\nAND R0,R0,#3// start with 25% set only\nCMP R0,#0\nBNE skip// only need to set blacks\nSTR R11,[R2]// set black\nskip:\nADD R2,R2,#4\nCMP R2,R3\nBLT randLoop\ncopyScreen2to1:\nMOV R1,#.PixelScreen\nMOV R2,#screen2\nADD R3,R1,#12288\ncopyLoop:\nLDR R0,[R2]\nSTR R0,[R1]\nADD R1,R1,#4\nADD R2,R2,#4\nCMP R1,R3\nBLT copyLoop\n// Next generation\nMOV R3,#0// R3 is cell offset,0 to 12288 (incr by 4)\nnextGenLoop:\nBL countBlock// count neighbours in R6\n// Now decide fate of cell\nMOV R2,#screen2\nADD R2,R2,R3\nCMP R6,#4\nBLT .+3 \nSTR R12,[R2]// Cell dies (or remains empty) if 4 or more neighbours\nB continue\nCMP R6,#3\nBLT .+3 \nSTR R11,[R2]// Cell born (or remains) if 3 or 4 neighbours\nB continue\nCMP R6,#2\nBEQ continue// Cell remains in present state if 2 neighbours\nSTR R12,[R2]// Cell dies (or remains empty) if < 2 neighbours\ncontinue:\nADD R3,R3,#4\nCMP R3,#12288\nBLT nextGenLoop\nB copyScreen2to1\n\n// R3 is pixel index,R6 return count\n// R11,R12 do not change,R5 used by countIfLive()\n// we use R1,R4 and R10 (as temp for LR!!!!)\ncountBlock:\nMOV R10,LR\nMOV R6,#0// Reset live count\nMOV R1,#.PixelScreen\nADD R1,R1,R3\nAND R4,R3,#255// index in row\nCMP R3,#256\nBLT topRow// remove all the special cases\nCMP R3,#12032\nBEQ leftBot// because BGE not in AQA set\nBGT botRow// and #12028 is > 8 bits\nCMP R4,#0\nBEQ leftCol\nCMP R4,#252\nBEQ rightCol\n// now can do original count neighbours\nSUB R1,R1,#256// North \nBL countIfLive\nADD R1,R1,#4// Northeast\nBL countIfLive\nADD R1,R1,#256// East\nBL countIfLive\nADD R1,R1,#256// Southeast\nBL countIfLive\nSUB R1,R1,#4// South\nBL countIfLive\nSUB R1,R1,#4// Southwest\nBL countIfLive\nSUB R1,R1,#256// West\nBL countIfLive\nSUB R1,R1,#256// Northwest\nBL countIfLive\nMOV PC,R10// RET\nrightCol:// but not top or bottom\nSUB R1,R1,#256// North \nBL countIfLive\nSUB R1,R1,#252// Northeast\nBL countIfLive\nADD R1,R1,#256// East\nBL countIfLive\nADD R1,R1,#256// Southeast\nBL countIfLive\nADD R1,R1,#252// South\nBL countIfLive\nSUB R1,R1,#4// Southwest\nBL countIfLive\nSUB R1,R1,#256// West\nBL countIfLive\nSUB R1,R1,#256// Northwest\nBL countIfLive\nMOV PC,R10// RET\nleftCol:// but not top or bottom\nSUB R1,R1,#256// North \nBL countIfLive\nADD R1,R1,#4// Northeast\nBL countIfLive\nADD R1,R1,#256// East\nBL countIfLive\nADD R1,R1,#256// Southeast\nBL countIfLive\nSUB R1,R1,#4// South\nBL countIfLive\nADD R1,R1,#252// Southwest\nBL countIfLive\nSUB R1,R1,#256// West\nBL countIfLive\nSUB R1,R1,#256// Northwest\nBL countIfLive\nMOV PC,R10// RET\ntopRow:\nCMP R4,#0// note R3=R4\nBEQ leftTop\nCMP R4,#252\nBEQ rightTop\n// now top but not sides\nADD R1,R1,#12032// North \nBL countIfLive\nADD R1,R1,#4// Northeast\nBL countIfLive\nSUB R1,R1,#12032// East\nBL countIfLive\nADD R1,R1,#256// Southeast\nBL countIfLive\nSUB R1,R1,#4// South\nBL countIfLive\nSUB R1,R1,#4// Southwest\nBL countIfLive\nSUB R1,R1,#256// West\nBL countIfLive\nADD R1,R1,#12032// Northwest\nBL countIfLive\nMOV PC,R10// RET\nbotRow:// removed leftBot already\nCMP R4,#252\nBEQ rightBot\nSUB R1,R1,#256// North \nBL countIfLive\nADD R1,R1,#4// Northeast\nBL countIfLive\nADD R1,R1,#256// East\nBL countIfLive\nSUB R1,R1,#12032// Southeast\nBL countIfLive\nSUB R1,R1,#4// South\nBL countIfLive\nSUB R1,R1,#4// Southwest\nBL countIfLive\nADD R1,R1,#12032// West\nBL countIfLive\nSUB R1,R1,#256// Northwest\nBL countIfLive\nMOV PC,R10// RET\n// There must be a way to improve this but I\'m not short of space! \nleftTop:\nADD R1,R1,#12032// North \nBL countIfLive\nADD R1,R1,#4// Northeast\nBL countIfLive\nSUB R1,R1,#12032// East\nBL countIfLive\nADD R1,R1,#256// Southeast\nBL countIfLive\nSUB R1,R1,#4// South\nBL countIfLive\nADD R1,R1,#252// Southwest\nBL countIfLive\nSUB R1,R1,#256// West\nBL countIfLive\nADD R1,R1,#12032// Northwest\nBL countIfLive\nMOV PC,R10// RET\nrightTop:\nADD R1,R1,#12032// North \nBL countIfLive\nSUB R1,R1,#252// Northeast\nBL countIfLive\nMOV R1,#.PixelScreen// East (SUB is > 8 bits)\nBL countIfLive\nADD R1,R1,#256// Southeast\nBL countIfLive\nADD R1,R1,#252// South\nBL countIfLive\nSUB R1,R1,#4// Southwest\nBL countIfLive\nSUB R1,R1,#256// West\nBL countIfLive\nADD R1,R1,#12032// Northwest\nBL countIfLive\nMOV PC,R10// RET\nleftBot:\nSUB R1,R1,#256// North \nBL countIfLive\nADD R1,R1,#4// Northeast\nBL countIfLive\nADD R1,R1,#256// East\nBL countIfLive\nSUB R1,R1,#12032// Southeast\nBL countIfLive\nSUB R1,R1,#4// South (=#.PixelScreen)\nBL countIfLive\nADD R1,R1,#252// Southwest\nBL countIfLive\nADD R1,R1,#12032// West\nBL countIfLive\nSUB R1,R1,#256// Northwest\nBL countIfLive\nMOV PC,R10// RET\nrightBot:\nSUB R1,R1,#256// North \nBL countIfLive\nSUB R1,R1,#252// Northeast\nBL countIfLive\nADD R1,R1,#256// East\nBL countIfLive\nSUB R1,R1,#12032// Southeast (=#.PixelScreen)\nBL countIfLive\nADD R1,R1,#252// South\nBL countIfLive \nSUB R1,R1,#4// Southwest\nBL countIfLive\nADD R1,R1,#12032// West\nBL countIfLive\nSUB R1,R1,#256// Northwest\nBL countIfLive\nMOV PC,R10// RET\n// Subroutines\ncountIfLive:LDR R5,[R1]// Sub\nCMP R5,R12\nBEQ .+2\nADD R6,R6,#1\nRET\nHALT\n.ALIGN 1024\nscreen2:.DATA\n';
}

function programCancel()
{				// reset original values
	if (!waitingForInput) {
		if (debug) alert("Bad - Cancel when not waiting for input");
		return false;
	}
	evPush('Rv');
	waitingForInput = false;
	modifyingProgram = false;
	if (programText == "") message("LOAD or EDIT a program");
	else message("RUN/STEP your program or LOAD/EDIT a program");
	removeClass("program","edit");
	resetProgramArea(programHTML);
	setStateReady();			// if exit edit without doing submit
}

function saveProgram()
{
	evPush('Sv');
	// Problem that you might be editing and forgot to do submit first
	if (modifyingProgram == true) programSubmit2(false);

	saveFile(programSave, "myprog.txt");
	message("Saving File");
}

function iclick()			// The interrupt button has been clicked
{
	if ((pinMask & 1) != 0 && IOVectors[1] >= 0) {	// ignore unless enabled and ISR setup
		interruptRequest |= 1;				// but remember even if interrupts not enabled (yet)
		intButtonGrey();
	} else if ((pinMask & 2) != 0) {	// non-interrupt I/O
		pinMask |= 4;
		intButtonGrey();
	} else if (debug) alert("clicked but not enabled");
}

function checkClickColour()
{
	if ((pinMask & 6) == 2) {		// manual mode on
		intButtonShow();			// show includes Setup
	} else if ((pinMask & 1) == 1 &&		// interrupt mode on for button
		IOVectors[1] >= 0 && (IOVectors[1]&3)==0 && IOVectors[1]<maxUsableMem) { // with valid ISR
		if ((interruptMask & 1) != 0) {		// interrupts are on
			intButtonShow();
		// DO NOT set grey if interrupts turn off - so if service another interrupt does not flicker
		} else intButtonSetup();			// not sure we need this case (but belt and braces)
	} else {			// see if is in use, could be a vector or polling but clicked already
		if (IOVectors[1] >= 0 || (pinMask & 2) != 0) {
			intButtonSetup();			// so setup but not clickable
			intButtonGrey();
		} else intButtonReset();		// not enabled in any way
	}
}

function testKeyInterrupt()		// we have had a key press
{
	if ((keyboardMask & 1) == 1 &&			// interrupt mode on for keyboard
		IOVectors[2] >= 0 && (IOVectors[2]&3)==0 && IOVectors[2]<maxUsableMem) { // with valid ISR
		interruptRequest |= 2;				// remember even if interrupts not enabled (yet)
	}
}

// function to setup the next time interval if timer interrupts are enabled and setup
function checkClockEnabled()
{
	if (xTime != 0) return;		// already running (and we are called from RFE)
	if (clockIntFreq == 0) return;	// need count at least 1
	if ((interruptMask&1) == 0) return;
	if (IOVectors[3] >= 0 && (IOVectors[3]&3)==0 && IOVectors[3]<maxUsableMem) {
		xTime = Date.now() + clockIntFreq;
	}
}

function clearMem()
{
	evPush('Clr');
	reset2();
	overlay = 0;	
	rewriteSide();	// rewrite the side bar with the new addresses and the page field

	for (var i = 0; i < maxUsableMem; i+=4) {
		setAddress(i, 0);
	}
	byteCount = 0;		// prevent false program highlighting
	programText = "";
	textToHtml();		// better than setting all the resets
	message("System Cleared");	
}

// bring back part of original op2() for the data display mode switch
// not sure whether we need more for binary - try this and see what happens
// memOpt values 0 = 2's comp, 1 = unsigned, 2 = hex, 3 = binary
function op2()				// a data display mode selection has been made
{
	var op = document.getElementById("data").value;
	evPush(op);
	// switch did not work on numbers - try strings here
	if (memOpt == 3) {		// undo binary class
		if (op == 'bin') return;	// no change
		removeClass("xxbody","binary");
		setMemBinary(false);
	}
	switch(op) {
	case 'sig':
		memOpt = 0;
		break;
	case 'uns':
		memOpt = 1;
		break;
	case 'bin':
		memOpt = 3;
		addClass("xxbody","binary");
		setMemBinary(true);
		break;
	default:		// hex
		memOpt = 2;
		break;
	}
	var savDD = dontDisplay;
	dontDisplay = 0;				// mouse click so execution paused
	rewriteMemoryAndRegs(false);
	dontDisplay = savDD;
}

function rewriteMemoryAndRegs(noregs)
{
	updateFlags(flags);				// do this because this routine used to do general screen resets

	var base = overlay*64;			// need the word offset not the byte one

	for (var i=0; i<128; ++i) {		// rewrite the contents (only need to rewrite visible mem)
		setAddress((i+base)*4, address[i+base]);
	}
	
	if (noregs) return;

	// new register setup - the memory mode class isn't done here any more
	//var newClass = modeNames[memOpt];
	//if (!newClass) {if (debug) alert("memOpt error "+memOpt);}
	//newClass = "value "+newClass;

	for (var i=0; i<15; ++i) {
		//setClass("R"+i,newClass);
		updateR(i, register[i]);
	}
	//setClass("R15",newClass);
	updatePC(pCounter);				// switch mode on PC
}

// file read for LOAD button
function read_chg(inp)
{
	evPush('Ld');
	// we did start in edit mode so the user might just click LOAD first
	if (modifyingProgram == true) programCancel();
	reset2();
	var fs = inp['files'];
	if(!fs) { message("BAD FILE SELECTION RETURN"); return; }
	var fr=new FileReader;
	fr.readAsText(fs[0],'utf-8');
	fr.onload = function() {
		programText = fr.result;
		textToHtml();
		assemble();
		resetLoadButton(); // re-write so can load same file again		
	}
}

// file read for INP instruction - uses fake STOP button click
function read_file(inp)
{
	var fs = inp['files'];
	consoleReset();
	if(!fs) {
		message("BAD FILE SELECTION RETURN");
		if (fileResult >= 0) updateR(fileResult, 0xffffffff);		// failed to open file
		fileResult = -1;
		waitingForInput = false;
		return;
	}
	var fr=new FileReader;
	fr.onload = function() {
		fileRead = fr.result;
		if (fileResult >= 0) updateR(fileResult, fileRead.length);	// read the file
		waitingForInput = false;
		fileResult = -1;
	}
	fr.readAsText(fs[0],'utf-8');
}

var savOpenAddress = false;

function openAddress(inp)
{
	if (waitingForInput) return;					// ignore if doing input (ours or the program)
	if (myTimeout || myMaybe) return;	// ignore if something running
	evPush('A'+inp.id);
	message("Modifying memory contents");
	openAddressToEdit(inp);
	waitingForInput = true;
	savOpenAddress = inp;
	setStateEdit();
}

// used by tab and focus changes to filter out the error cases, return true if OK
// reason is to avoid addressSubmit() doing an alert (which changes focus)
// it means that in some cases (like TAB) we do these tests 3 times
function checkInput(iValue)
{
	if (!waitingForInput) return false;	// should not happen
	if (memOpt == 3) iValue = iValue.replace(' ','');
	if (/*isNaN(iValue) ||*/ iValue=="") return false;
	var value = parseIntGen(iValue);
	if (isNaN(value) || value > 4294967295 || value < -2147483648) {
		return false;
	}
	return true;
}

function addressSubmit()
{
	if (!waitingForInput) {
		if (debug) alert("Bad input - not waiting for input");
		return;
	}
	var epvalue = getAddressValue();  // so can remove space in binary
	//if (memOpt == 3) epvalue = epvalue.replace(' ','');
	
	if (/*isNaN(epvalue) ||*/ epvalue=="") {
		message("Bad input - must be a number");
		return;
	}

	var value = parseIntGen(epvalue);
	if (isNaN(value) || value > 4294967295 || value < -2147483648) {
		message("Bad number format or value");
		return;
	}
	document.getElementById("aForm").removeAttribute("onblur");	
	// need to decode savOpenAddress.id
	var x = parseInt(savOpenAddress.id[1]);
	if (savOpenAddress.id.length >= 3) x = x*10 + parseInt(savOpenAddress.id[2]);
	if (savOpenAddress.id.length >= 4) x = x*10 + parseInt(savOpenAddress.id[3]);
	if (savOpenAddress.id.length == 5) x = x*10 + parseInt(savOpenAddress.id[4]);
	x += overlay * 64;
	setAddress(x*4, value);		// removes input form as well
	waitingForInput = false;
	setStateReady();
	savOpenAddress = false;
	if (programText == "") message("LOAD or EDIT a program");
	else message("RUN/STEP your program or LOAD/EDIT a program");
}

function loseAddress(inp)
{
	if (!waitingForInput) return;		// happens after addressSubmit as well
	if (checkInput(inp.value)) addressSubmit();
	else {		// bad value
		resetAddressInput();
		var x = parseInt(savOpenAddress.id[1]);
		if (savOpenAddress.id.length >= 3) x = x*10 + parseInt(savOpenAddress.id[2]);
		if (savOpenAddress.id.length >= 4) x = x*10 + parseInt(savOpenAddress.id[3]);
		if (savOpenAddress.id.length == 5) x = x*10 + parseInt(savOpenAddress.id[4]);
		x += overlay * 64;
		setAddress(x*4, address[x]);	// removes input form as well
		waitingForInput = false;
		setStateReady();
		savOpenAddress = false;
		message("BAD input ignored");
	}
}

// step button pressed 
function step3()		// run step2 to get execution indication of one instruction
{
	evStep();
	oneStep = true;
	speed = 11;
	if (dontDisplay == 1) {			// we were running fast
		// (I think) we cannot get here without being in a "myMaybe" delay
		if (myMaybe) clearTimeout(myMaybe);
		myMaybe = false;
		dontDisplay = 0;
		rewriteMemoryAndRegs(false);
	} else {
		if (myTimeout) clearTimeout(myTimeout);
		myTimeout = false;
	}
	run2();
	setValue("counter",""+instructionCount);
}

// run slow button pressed
function runSlow()
{
	if (myTimeout) {	// multiple presses seemed to get through
		// we are running slow already - speed up
		speed = Math.floor((speed+2)/2);	// min value 2
		return;
	}
	if (pCounter == 0 && address[0] == 0) {
		message("No program to run");
		return;
	}
	// only record valid first presses
	evPush('Slw');
	oneStep = false;
	speed = slowSpeed;
	setStateSlow();
	if (dontDisplay == 1) {			// we were running fast
		// (I think) we cannot get here without being in a "myMaybe" delay
		if (myMaybe) clearTimeout(myMaybe);
		myMaybe = false;
		dontDisplay = 0;
		rewriteMemoryAndRegs(false);
		if (!waitingForInput) myTimeout = delay(speed, "runContinue");
	} else run2();
}

// run button pressed
//function run() - change because interface has Run() and too similar
function runOrig()
{
	if (waitingForInput || myMaybe) return;		// prevent multiple presses
	if (myTimeout) {
		clearTimeout(myTimeout);
		myTimeout = false;
	}
	evPush('Rn');
	if (pCounter == 0 && address[0] == 0) {
		message("No program to run");
		return;
	}
	oneStep = false;
	
	// extra instructions for speed measurement - change to own variable
	measureSpeed = instructionCount;
	setValue("counter","0");
	runTimer = getMilliseconds();
	
	speed = 1;
	setStateRunning();		// remove pause or slow
	//message("Running without screen updates");
	message("");
	dontDisplay = 1;	// need to quench display
	removeCodeHighlight();
	removeMemHighlight();
	clearIR();
	myMaybe = delay(delayTime, "maybeRunContinue");			// attempt to get refreshes with minimal pauses
	run2();
}

function run2()				// make step same as run 
{
	if (waitingForInput) return;
	//reset1();
	// buttons are not dynamic any more
	stopping = false;
	noKeyEffects = true;
	IEKeyEnable();
	runContinue();
}

function runContinuey()					// wait for a halt to finish
{
	if (dontDisplay == 1) {				// probably overkill but I have added this everywhere we reset the RUN button
		if (myMaybe) clearTimeout(myMaybe);
		myMaybe = false;
		dontDisplay = 0;
		rewriteMemoryAndRegs(false);
	}
	// buttons are not dynamic any more
	running = false;
	setStatePaused();
	noKeyEffects = false;
	message(halted);		// try adding this
	myTimeout = false;
}

function runContinuez()				// wait for input in fast execution - needed for one step case
{
	if (waitingForInput) {
		myTimeout = delay(speed, "runContinuez");
		
		if (fileResult >= 20) {			// delay the click
			fileResult -= 20;
			var elePosn = document.getElementById("read-file");
			elePosn.click();
		}
	} else {
		lastKey = 0;
		if (breakpointAddr == pCounter) { //  breakpoint instruction - stop before
			if (myMaybe) alert("BUG - myMaybe");
			if (dontDisplay) {
				dontDisplay = 0;
				rewriteMemoryAndRegs(false);
				message(lastMessage);
				if (lineNo*4 < byteCount) showExecuteStop(addrToLine[lineNo]);
				setMemHighlight(lineNo*4);
				setValue("counter",""+instructionCount);
			}
			message("Breakpoint detect at PC = 0x"+padHex(pCounter,5));
			running = false;
			setStatePaused();		// need to check all 3 modes!!!
			noKeyEffects = false;
			return;
		}

		message(stepTxt);
		if (speed == 1 && oneStep == false) {
			//message("Running without screen updates");
			message("");
			dontDisplay = 1;	// need to quench display
			clearIR();
			// do another 5ms of work
			maybeWaiting = 0;
			myMaybe = delay(delayTime, "maybeRunContinue");		// set delay
			runContinue();
		} else myTimeout = delay(speed, "runContinue");
	}
}

// Requesting a delay (which calls setTimeout) and then doing 5ms of work cheats the Chrome delay algorithm
var myMaybe = 0;
var maybeWaiting = 0;
function maybeRunContinue()
{
	if (myMaybe == 0) {if (debug) alert("myMaybe == 0 should not happen");}
	if (maybeWaiting == 0) {
		//alert("maybeWaiting == 0 - outside test parameters");	// this should not happen with the loop test program
		myMaybe = 0;				// later need to decide how to handle this case
		return;
	}
	// here if need to do another 5ms of work
	maybeWaiting = 0;
	myMaybe = delay(delayTime, "maybeRunContinue");		// set ourselves another
	runContinue();
}

// delays - unused  1  2   3   4   5  6	 7 8 9 10
var DC =    [4000,750,750,300,100,40,20,10,5,2,1];
// adding this to the inner loop slowed things down by more than 10%
// thinking about this, you cannot get a stopping, oneStep, a click or a keyboard interrupt
// without a re-schedule so you probably only have to check on calling runContinue()
// (ACTUALLY wrong, RFE and various interupt enable cases now need trapping)
// maybe also linearly do 10 calls to step1() 
// also change HALT to be post detected not pre-detected
// look for other optimisations in step1

function runContinue()
{
	myTimeout = false;
	if (stopping) {						// stop button pressed
		if (dontDisplay == 1) {
			if (myMaybe) clearTimeout(myMaybe);
			myMaybe = false;
			dontDisplay = 0;
			rewriteMemoryAndRegs(false);
			// this was a problem because the PC has advanced so made lineNo global
			if (lineNo*4 < byteCount) showExecuteStop(addrToLine[lineNo]);
			setMemHighlight(lineNo*4);
			setValue("counter",""+instructionCount);
			// special for measurment
			measureSpeed = instructionCount - measureSpeed;
			runTimer = getMilliseconds()-runTimer;
			var tmp1 = Math.floor((measureSpeed/runTimer)/10); 	// 10K inst/sec
			var tmp2 = Math.floor(runTimer/100); 					// tenths of a second
			if (step1Code!=2) message("Program paused. "+measureSpeed+" ins in "+tmp2/10+" secs, "+tmp1/100+"M ins/sec");
		} else if (!oneStep) { 
			if (step1Code!=2) message("Program paused. RUN or STEP to continue, STOP to abort.");
		} else if (step1Code!=2) message(stepTxt);
		// buttons are not dynamic any more
		running = false;
		setStatePaused();		// main case - probably minor ones to find
		noKeyEffects = false;
		return;
	}
	if (oneStep) stopping = true;

	// we can only get a click or KB interrupt after a reschedule so we can check those outside the fast loop
	// EXCEPT that we need to then special case the RFE and all of the ways an interrupt might be enabled so that
	// the interrupt request is then detected by the fast path

	// Test if need an interrupt before going to do any instruction
	if ((interruptMask&1)!=0 && (interruptRequest!=0 || (xTime && (xTime < Date.now())))) {
	
		// should not get a request unless the corresponding ISR and enable are set
		// but we will double check those below as well

		// xTime is designed to keep the overheads down. Whenever there might be a clock
		// interrupt we set xTime to how long we have to go. It is cleared if you write zero
		// to the byte .ClockInterruptFrequency and set when do anything that might enable
		// a clock interrupt. The RFE from the clock interrupt restarts the timer.
		//alert("xTime is "+xTime+" now is "+Date.now());
		
		if (badStack()) return; 			// badStack() returns true if error
		doInterrupt();						// usually changes PC
	}

	// separate the slow case from the fast case!
	step1Code = 0;				// set non-zero for RFE, HALT and some other cases
				// 0=error, 1=HALT, 2=Breakpoint, 3=interrupt, 4=input, 5=NOP (MOV r0,r0)

	if (dontDisplay == 0) {		// note single step can only come here
		var pc = Math.floor(pCounter/4);
		if (pc*4 < byteCount) showExecuteStop(addrToLine[pc]);	// now for run and step
		setMemHighlight(pCounter);

		if (!step1(1) && breakpointAddr != pCounter) {	// instruction went OK
			message(stepTxt);
			setValue("counter",""+instructionCount);			
			myTimeout = delay((speed-10)*4, "runContinue");
			return;
		}
		// error return and step1Code only used in slow path for error, HALT, Breakpoint and input
		setValue("counter",""+instructionCount);
		if (waitingForInput) {			// wait for input or alert to be cancelled (step1Code == 4)
			myTimeout = delay(speed, "runContinuez");
			return;
		}
		// here if error or HALT or Breakpoint
		// buttons are not dynamic any more
		if (breakpointAddr == pCounter) { //  breakpoint instruction - stop before
			message("Breakpoint detect at PC = 0x"+padHex(pCounter,5));
		}
		running = false;
		setStatePaused();
		noKeyEffects = false;
		return;
	}

	// This is the fast path - the call was expensive so all the loops are now in step1()
	//if (step1(DC[speed])) {
	if (step1(751)) {				// no longer dynamic
		// here if exception or special case

		// convert NOP at breakpoint into breakpoint
		if (breakpointAddr == pCounter && step1Code == 5) step1Code = 2;

		if (step1Code < 3) { // error or HALT *** need to check messages for HALT!
					// 0=error, 1=HALT, 2=Breakpoint, 3=interrupt, 4=input, 5=NOP (MOV r0,r0)
			if (myMaybe) clearTimeout(myMaybe);
			myMaybe = false;
			dontDisplay = 0;
			rewriteMemoryAndRegs(false);
			message(lastMessage);
			if (step1Code == 2) message("Stopped on Breakpoint at PC = 0x"+padHex(pCounter,5));
			// this was a problem because the PC has advanced so made lineNo global
			if (lineNo*4 < byteCount) showExecuteStop(addrToLine[lineNo]);
			setMemHighlight(lineNo*4);
			setValue("counter",""+instructionCount);
			// buttons are not dynamic any more
			running = false;
			setStatePaused();
			noKeyEffects = false;
			return;
		}
		// step1Code is 3 = interrupt, 4 = input, 5 = NOP (MOV r0,r0 = 0xE1000070)
		if (waitingForInput) {			// wait for input or alert to be cancelled (step1Code == 4)
			if (myMaybe) clearTimeout(myMaybe);
			myMaybe = false;
			dontDisplay = 0;
			rewriteMemoryAndRegs(false);
			message(lastMessage);
			setValue("counter",""+instructionCount);
			myTimeout = delay(speed, "runContinuez");
			return;
		}
		// here if NOP to cause reschedule wait - just wait the remainder of the current cycle
	}
	maybeWaiting = 1;
	if (myMaybe == 0) {if (debug) alert("myMaybe == 0 - interlock went wrong");}	// this should not happen
	setValue("counter",""+instructionCount);		// only update in fastest mode
	if (speed == 1) return;			// if not very fastest show regs and memory
	dontDisplay = 0;	// This doen't happen any more - there is no regs only mode
	rewriteMemoryAndRegs(false);
	dontDisplay = 1;
	return;
}

// stop button pressed (note that stop is only available when running)
// Also what used to be stop is now Pause and Stop is now reset
function stop()
{
	evPush('Ps');
	stopping = true;					// always stop
}

function stop2()		// strange - only called from reset1
{
	if (myMaybe) clearTimeout(myMaybe);
	myMaybe = false;
	if (myTimeout) clearTimeout(myTimeout);
	myTimeout = false;
	//message("Program stopped. RUN or STEP to continue, RESET to abort.");
	// buttons are not dynamic any more
	running = false;
	noKeyEffects = false;
	fileResult = -1;			// cancel waiting for file read
	consoleReset();
	removeCodeHighlight();
	removeMemHighlight();
	removeError();
}

// reset button pressed (now stop button)
function reset1()								// note reset did not work but is not listed as a reserved word
{
	evPush('Stp');
	reset2();
	message("Stop done, edit &amp; Submit, RUN/STEP or alter memory");
}

function reset2()								// internal reset
{
	dontDisplay = 0;				// so updates get done
	stop2();
	reset3();
	if (savOpenAddress) {			// has a memory address open
		// these two lines look like a bug!!!
		//setAddress(x, address[Math.floor(x/4)]);		// removes input form and resets value
		//savOpenAddress = false;

		var x = parseInt(savOpenAddress.id[1]);
		if (savOpenAddress.id.length >= 3) x = x*10 + parseInt(savOpenAddress.id[2]);
		if (savOpenAddress.id.length >= 4) x = x*10 + parseInt(savOpenAddress.id[3]);
		if (savOpenAddress.id.length == 5) x = x*10 + parseInt(savOpenAddress.id[4]);
		x += overlay * 64;
		setAddress(x*4, address[x]);	// removes input form as well
		waitingForInput = false;
		savOpenAddress = false;
	}
	if (modifyingProgram) {
		modifyingProgram = false;
		textToHtml();
		waitingForInput = false;
		removeClass("program","edit");
	}
	setValue("counter","0");
	rewriteMemoryAndRegs(false);	// need to refresh memory in case it was off
	intButtonReset();
}
