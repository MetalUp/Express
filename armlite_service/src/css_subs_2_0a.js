// The idea is that everything that depends on layout has been taken out to here

// The known exception is the program layout in HTML (sort out later)

// ID's program, R0-R15, flags, ir, ur (inst decode), irq (grey/normal interrupt)
// console (temp just put one line not scroll)
// page - needs input here later - one page only at the moment!!!!
// memory contents are a0 to a127, LHS addresses are meml0 to meml31

var editWin = 296;

// setup the repetitive parts of the html
function setupDivs()
{
	resetLoadButton();
	consoleReset();
	var m="";
	// memory
	var holdI = 0;
	for (var j=0; j<32; ++j) {
		m+='<div class="row"><div class="address" id="meml'+j+'">0x'+padHex(j,4)+'</div>';
		for (var i=0; i<4; ++i) {		// if change to/from binary dimensions change
			m+='<div class="word" id="a'+holdI+'" onclick="openAddress(this)" >0x0</div>';
			++holdI;
		}
		m+='</div>';
	}
	setValue("memory",m);
	
	setPixelAreaSize(pixelAreaSize);	// pixels

	// charmap
	m="";
	for (var j=0; j<512; ++j) {		// 32x16
		m+='<div id="c'+j+'"></div>';
	}
	setValue("chars",m);
}

// Now can only change size by Query String
function setPixelAreaSize(val)
{
	var m="";
	if (val == 12288) {			// 128x96
		pixelAreaSize = 12288;
		removeClass("pixels","pixels1");	// should be ignored if not present
		addClass("pixels","pixels2");		
	} else {		// 768 is faked now using 3072
		pixelAreaSize = 3072;
		removeClass("pixels","pixels2");	// should be ignored if not present
		addClass("pixels","pixels1");
	}
	if (serviceMode) return;

	if ((pixelMask & 5) != 0) { // Pixel interrupt (2 set/1 poll enable/0 int enable)
		document.getElementById("chars").style.display = "none";
		for (var j=0; j<pixelAreaSize; ++j) {
			m+='<div id="p'+j+'" onclick="pixelInt(this)"></div>';
		}
	} else {
		document.getElementById("chars").style.display = "block";		
		for (var j=0; j<pixelAreaSize; ++j) {
			m+='<div id="p'+j+'"></div>';
		}
	}
	setValue("pixels",m);
}

// borrowed from the web
function padHex(d, padding) {
    var hex = Number(d).toString(16);
    padding = typeof (padding) === "undefined" || padding === null ? padding = 2 : padding;
    while (hex.length < padding) {
        hex = "0" + hex;
    }
    return hex;
}

var plhtmp;
// display a system message (with previous user output)
function message(y)
{
	if (dontDisplay == 0 && !serviceMode) {
		var elePosn = document.getElementById("console"); 
		if (y) elePosn.innerHTML = output1+'\n'+y;
		else elePosn.innerHTML = output1;
		//elePosn.scrollTop = elePosn.scrollHeight;
        if (!scrollTimeout) {
			scrollTimeout = setTimeout(scroll_to_max, 10); // Allow for the element to be updated
			plhtmp = 0;
		}
	}
	lastMessage = y;
}

// display just the user output on the system console
function justConsole()
{
	// the scroll can be massive
	//if (output1.length > 100000) return;		// try this - worked but message() output it anyway
	var elePosn = document.getElementById("console"); 
	elePosn.innerHTML = output1;
	//elePosn.scrollTop = elePosn.scrollHeight;
	if (!scrollTimeout) {
		scrollTimeout = setTimeout(scroll_to_max, 10); // Allow for the element to be updated
		plhtmp = 0;
	}
}


// attempt to move the scroll after a delay to allow rendering
// ===========================================================
// I think the issue is that scrollTop is not updated until the screen is drawn so that
// you cannot update when you write text (it still has the old value and limit). Provided
// the offset is large enough (200 = 15 lines I think) that is enough but this code checks
// and so normally gets attempts 2. If no scroll is needed/allowed you exit the first time
function scroll_to_max() {
	++plhtmp;
	var elePosn = document.getElementById("console"); 
	var t = elePosn.scrollTop;
	elePosn.scrollTop = t+4000;
	if(elePosn.scrollTop != t) {
		scrollTimeout = setTimeout(scroll_to_max, 10); // Keep scrolling till we hit max value
	} else {
		scrollTimeout = false;
		// I have seen 20 on the file copy test with 200 - try 2000 pixels above
		if (debug && (plhtmp > 100)) alert("Scroll attempts are "+plhtmp);
	}
}

// set a square in the pixel mapped output area
function videoWrite(indx, y)		// index is offset in 32x24 or 64x48 array, y is HTML code
{
	if (indx < 0 || indx >= pixelAreaSize) return;
	y &= 0xffffff;				// needed because 7 digits is ignored (i.e. did not work)
	document.getElementById("p"+indx).style.background = "#"+padHex(y,6);
}

/* Now we can change size by program we need to reset rather than clear
function clearPixelArea()
{
	for (var indx = 0; indx < pixelAreaSize ; ++indx)
		document.getElementById("p"+indx).style.background = "white";
} */

//var monospace = '"Courier New Bold",courier,monospace';

/* // The blinks need rewriting anyway (and text() is removed!!
function blinkoff()
{
	if (!waitingForInput) return;					// ignore if not doing input
										// should already have been deleted
	text("inpxx", "", 261, 541, 12, "red");
	delay(500, "blinkon");
}
function blinkon()
{
	if (!waitingForInput) return;					// ignore if not doing input
	text("inpxx", " &nbsp; INPUT NEEDED", 261, 541, 12, "red");
	delay(1000, "blinkoff");
}

// same flashing style for input file
//var fileText2 = '<input type="file" id="read-file" onchange="read_file(this)" autocomplete="off" style="display:none;" ><button onclick="document.getElementById(&#39;read-file&#39;).click()" style="color:black; font-size:24px">CLICK TO SELECT FILE</button>';
//var fileText = '<input type="file" id="read-file" onchange="read_file(this)" autocomplete="off" style="display:none;" ><button onclick="document.getElementById(&#39;read-file&#39;).click()" style="color:red; font-size:24px">CLICK TO SELECT FILE</button>';

function bloff()
{
	if (!waitingForInput) return;					// ignore if not doing input
										// should already have been deleted
	text("fileRequest", fileText2, 15, 12, 20, "yellow");
	delay(500, "blon");
}

function blon()
{
	if (!waitingForInput) return;					// ignore if not doing input
	text("fileRequest", fileText, 15, 12, 20, "red");
	delay(1000, "bloff");
	//var elePosn = document.getElementById("read-file");
	//elePosn.click();
}
*/

function openNextMem(x)
{
	var elePosn = document.getElementById("a"+x);
	elePosn.innerHTML = addressInputText(elePosn.innerHTML);
	document.getElementById("aForm").focus();
	document.getElementById("aForm").setAttribute("onblur","loseAddress(this)");
	return elePosn;
}

function insertTabInProgramArea()
{
	var textarea = document.getElementById('pForm');
	var s = textarea.selectionStart;
	textarea.value = textarea.value.substring(0,textarea.selectionStart) + "\t" + textarea.value.substring(textarea.selectionEnd);
	textarea.selectionEnd = s+1; 					// should reset the cursor
}

/* not used now
function getPCFormValue()
{
	var elePosn = document.getElementById("PCForm");
	elePosn.removeAttribute("onblur");
	return elePosn.value;
} */

function openProgramArea(text)
{
	addClass("program","edit");
	var elePosn = document.getElementById("source");
	elePosn.style.overflow = "initial";					// reset scroll mode because textarea has own
	elePosn.style.whiteSpace = "normal";				// reset wrapping (or buttons get moved)
	elePosn.innerHTML = '<form action="javascript:programSubmit()"><textarea id="pForm" rows="36" cols="36"  spellcheck="false" >'+
			text.replace(/&/g, "&amp;");+'</textarea></form>';
	elePosn = document.getElementById("pForm");
	elePosn.style.width = editWin+"px";	
	elePosn.focus();
}

function getProgramArea()
{
	var elePosn = document.getElementById("pForm");
	return elePosn.value;
}

// Impure interface - called with formatted html!!!
function resetProgramArea(newText)
{
	setValue("source",newText);
	var src = document.getElementById("source");
	src.style.overflow = "auto";
	src.style.whiteSpace = "nowrap";
	src.scrollTop = 0;
	// since we did a reset from original there are no markers set
	lastCodeHighlight = -1;
	breakpointAddr = -1;
	errorLineNum = -1;
}

var lastState = 0;
// Master control of the simulator states. Since the running code needs rewiting, I'll make this
// internally consistent and keep a state variable so we are not randomly modifing the DOM state
// States - 0 start, 1 ready, 2 edit, >4 running, 4 just running, 5 slow, 6 paused
// make these setStateXxx so can search in main.js and find all uses
function setStateReady()
{
	changeState(1);
}

function setStateEdit()
{
	changeState(2);
}

function setStateRunning()
{
	changeState(4);
}

// used if might already be in slow mode which we don't want to change
function setStateForceRunning()
{
	if (lastState == 5) return;
	changeState(4);
}

function setStateSlow()
{
	changeState(5);
}

function setStatePaused()
{
	changeState(6);
}

var plhComCnt = 0;
function changeState(val)
{
	function xVal(va)
	{
		if (va == 1) return "ready";
		if (va == 2) return "edit";
		if (va == 4) return "running";
		if (va == 5) return "running+slow";
		if (va == 6) return "running+paused";
		return va;
	}
	if (lastState == val) {
		++plhComCnt;
		if (debug == 2) setValue("credits", "New state "+xVal(val)+" == Old state "+xVal(lastState)+" == Dup "+plhComCnt);
		return;
	}
	plhComCnt = 0;
	if (debug == 2) setValue("credits", "New state "+xVal(val)+" == Old state "+xVal(lastState));
	if (lastState != 0) {		// an old state to remove
		if (lastState == 6) {
			removeClass("xxbody","paused");
			lastState = 4;
		}
		if (lastState == 5) {
			removeClass("xxbody","slow");
			lastState = 4;
		}
		if (lastState == val) return;	// check again
		if (lastState == 4 && val >=4) {
			// can do setup without removing running
			lastState = val;
			if (val == 4) return;		// just running
			if (val == 5) addClass("xxbody","slow");
			if (val == 6) addClass("xxbody","paused");
			return;
		}
		if (lastState == 4) removeClass("xxbody","running");
		if (lastState == 2) removeClass("xxbody","edit");
		if (lastState == 1) removeClass("xxbody","ready");
	}
	// now need to set the new state
	lastState = val;
	if (val >= 4) {
		addClass("xxbody","running");
		if (val == 5) addClass("xxbody","slow");
		if (val == 6) addClass("xxbody","paused");
		return;
	}
	if (lastState == 2) addClass("xxbody","edit");
	if (lastState == 1) addClass("xxbody","ready");
}


// called on reset to remove any state classes from IRQ
// also called if both Pin ISR removed and polling disabled
function intButtonReset()
{
	removeClass("irq","enabled");
	removeClass("irq","active");
}

// called when Pin ISR setup or polling enabled
function intButtonSetup()
{
	addClass("irq","enabled");
}

// not clickable
function intButtonGrey()
{
	removeClass("irq","active");
}

// clickable (so set enabled in case we got here without calling setup)
function intButtonShow()
{
	addClass("irq","enabled");
	addClass("irq","active");
}

/*function getOverlayValue()
{
	return document.getElementById("ov1").value;
}*/

function rewriteSide()			// rewrite the memory left side heading and the overlay value
{
	var base = overlay*16;
	for (var i=0; i<32; ++i) {
		setValue("meml"+i,'0x'+padHex(base+i,4));
	}
	setValue("page", padHex(overlay,3));
}

// OVERLAY CHANGE LOGIC - LATER SPLIT INTO FUNCTIONAL AND HTML/CSS
// open the overlay location when clicked
function changePage()
{
	if (myMaybe || myTimeout) return false;		// if running might interfere with user I/O
	message("Modifying memory overlay");		// it did not work either!
	setValue("page",overlayForm);
	var elePosn = document.getElementById("oForm");
	elePosn.focus();
	waitingForOverlay = true;
}

var overlayForm = '<form action="javascript:pageSubmit()"><input id="oForm" type="text"></form>';

function pageSubmit()
{
	var elePosn = document.getElementById("oForm");
	if (!elePosn || !waitingForOverlay) {
		if (debug) alert("Bad pageSubmit call");
		return false;
	}
	waitingForOverlay = false;
	var val = parseInt(elePosn.value,16);
	if (isNaN(val) || (val * 256) >= maxUsableMem) {
		setValue("page", padHex(overlay,3));		// reset the overlay field
		message("Bad page value");
		return;
	}
	if ((val * 256) == (maxUsableMem-256)) --val;		// you are allowed to ask for the last page
	evPush('Pg'+padHex(val,3));
	var tmp = lastMemHighlight;
	removeMemHighlight();			// need to do this before we change the value of overlay!
	overlay = val;	
	var savDD = dontDisplay;		// currently you cannot change page in fast running so this isn't needed
	dontDisplay = 0;				// mouse click so execution paused
	rewriteSide();					// rewrite the side bar with the new addresses
	rewriteMemoryAndRegs(true);		// do not update registers (does update flags - should not matter)
	if (tmp >= 0) {
		setMemHighlight(tmp);
	}
	dontDisplay = savDD;
	message("Page value set");			
}

// if you don't do this then you cannot load a file, edit it and load it again (you get the old version)
function resetLoadButton()
{
	setValue("program-controls", loadText);
}
var loadText = '<input type="file" id="read-input" onchange="read_chg(this)" autocomplete="off"><button id="load" onclick="document.getElementById(&#39;read-input&#39;).click()">Load</button><button  id="save" onclick="saveProgram()">Save</button><button  id="edit" onclick="openProgram()">Edit</button><button  id="submit" onclick="programSubmit()">Submit</button><button  id="revert" onclick="programCancel()">Revert</button>';

// note that assemble() rewrites the whole program area html so there should not be a need to clear these three
function setLabel(r)
{
	var elePosn = document.getElementById("lin"+r);
	if (elePosn) {
		elePosn.classList.add("label");
	} else {
		if (debug) alert("could not find line "+r+" to set label class");
	}
}

// mem might be text of the form 0xa-0xb or 0xnn - it must be TEXT
function setCode(r,mem)
{
	var elePosn = document.getElementById("lin"+r);
	if (elePosn) {
		elePosn.classList.add("selectable");
		elePosn.title = mem;
		elePosn.setAttribute("onclick","javascript:setBreakpoint(this);");
	} else {
		if (debug) alert("could not find line "+r+" to set code options");
	}
}

function setData(r,mem)		// as above but data not code
{
	var elePosn = document.getElementById("lin"+r);
	if (elePosn) {
		elePosn.title = mem;
	} else {
		if (debug) alert("could not find line "+r+" to set data options");
	}
}

function setBreakpoint(inp)
{
	if (waitingForInput) return;		// ignore if doing some other input (ours or the program)
	if (myTimeout || myMaybe) return;	// ignore if something running
	var lin = parseInt(inp.id.substring(3));
	var tit = parseInt(inp.title);		// is of form 0xnnnn
	if (isNaN(lin) || isNaN(tit)) {
		if (debug) alert("could not parse breakpoint id "+inp.id+" title "+tit);
		return;
	}
	//alert("Check line number "+lin+" title "+tit);

	if (tit == breakpointAddr) {		// remove this breakpoint address
		inp.classList.remove(breakpoint);
		breakpointAddr = -1;
		message("Breakpoint removed at line "+lin+" address 0x"+padHex(tit,5));
	} else {
		if (breakpointAddr >= 0) {		// remove the old marker
			removeClassFromLineNumber(addrToLine[breakpointAddr/4], breakpoint);
		}
		breakpointAddr = tit;			// set the new breakpoint
		inp.classList.add(breakpoint);
		message("Breakpoint set at line "+lin+" address 0x"+padHex(tit,5));
	}
}

// update a register or the PC on the screen
function updateRDisplay(r,y)
{
	if (Math.floor(y) != y) {
		if (debug) alert("Attempt to put non-integer value into register "+y);
		y = Math.floor(y);
	}
	// Now with tool tips we need all the options	
	var uns = ''+y;
	var sig = uns;
	if (y > 2147483647) sig = '-'+(4294967296-y);
	var bin = '0b';
	for (var i = 0; i<32; ++i) {
		bin += ((y<<i) & 0x80000000)?"1":"0";
	}
	var hex = '0x'+padHex(y,8);
	var tip = "";
	// 0 = 2's comp, 1 = unsigned, 2 = hex, 3 = binary
	if (memOpt == 0) {
		y = sig;
		if (uns == sig) tip = hex+' '+bin;
		else tip = '('+uns+') '+hex+' '+bin;
	} else if (memOpt == 1) {
		y = uns;
		if (uns == sig) tip = hex+' '+bin;
		else tip = sig+' '+hex+' '+bin;
	} else if (memOpt == 3) {
		y = bin;
		if (uns == sig) tip = sig+' '+hex;
		else tip = sig+' ('+uns+') '+hex;
	} else {
		y = hex;
		if (uns == sig) tip = sig+' '+bin;
		else tip = sig+' ('+uns+') '+bin;
	}

	var elePosn = document.getElementById("R"+r); 
	if (!elePosn) {if (debug) alert("Simulator bug - cannot find R"+r+" to set new contents "+y);}
	else {
		elePosn.innerHTML = y;
		elePosn.title = tip;
	}
}

// produce lower case hex string with 0 (left) padding (if true) and without the x
// the important point is that consecutive ffff's are spaced out more
// NOT USED ANY MORE
/*function spacedHex(y, padding, span)
{
	var i = 0;
	var z = y;
	y = hex[y%16];
	// the problem is that consequtive f are badly spaced and turning off kerning does not work
	while (++i < 8) {
		if (!padding && Math.floor(z/16) == 0) break;	// no padding and rest is zero (first char above)
		if (z % 16 == 15) {		// last char was f
			if (i == 1 && span == true) y = 'f</span>';
			z = Math.floor(z/16);
			if (z % 16 != 15 && span == true) {		// this char is not f
				y = '<span class="ffff">'+y;
			}
			y = hex[z%16]+y;
		} else {
			z = Math.floor(z/16);
			if (z % 16 == 15 && span == true) {		// this char is f but last was not
				y = 'f</span>'+y;
			}else {
				y = hex[z%16]+y;
			}
		}
	}
	if (z % 16 == 15 && span == true) {		// last char added char was f (of course first in string)
		y = '<span class="ffff">'+y;
	}
	return y;
}*/

function updateFlags(y)
{
	flags = y&15;
	if (dontDisplay == 1) return;
	var txt = (y&8)!=0?'1':'0'; //thinsp removed
	txt += ((y&4)!=0?'1':'0');
	txt += ((y&2)!=0?'1':'0');
	txt += ((y&1)!=0?'1':'0');
	setValue("flags",txt);
}

function addressInputText(val)
{
	if (val == '0' || val == '0x00000000' || val == '0b00000000000000000000000000000000') val ='';
	var widthX = (memOpt == 3) ? '252' :'75';
	return '<form action="javascript:addressSubmit()"><input id="aForm" type="text" style="padding:0; border:0; font-family: monospace; width:'+widthX+'px;height:14px;font-size:13.4px;" value="'+val+'"></form>';
}

function openAddressToEdit(inp)
{
	inp.innerHTML = addressInputText(inp.innerHTML);
	document.getElementById("aForm").focus();
	document.getElementById("aForm").setAttribute("onblur","loseAddress(this)");
}

function getAddressValue()
{
	var elePosn = document.getElementById("aForm");
	return elePosn.value;
}

function resetAddressInput() // the setAddress finishes tidying up
{
	document.getElementById("aForm").removeAttribute("onblur");	
}

var breakpoint = "breakpoint";
var current = "current";
var error = "error";
// x is the index into the memory locations displayed on screen (words not bytes so 0 to 127!!)
// y is the value to put there, memOpt tells you how to do it (byte mode now removed)
function updateDisplayedMemory(x, y)
{
	// Now with tool tips we need all the options	
	// but should not need padding now
	var uns = ''+y;
	var sig = uns;
	if (y > 2147483647) sig = '-'+(4294967296-y);
	var bin = '0b';
	for (var i = 0; i<32; ++i) {
		bin += ((y<<i) & 0x80000000)?"1":"0";
	}
	var hex = '0x'+padHex(y,8);
	var tip = "";
	// 0 = 2's comp, 1 = unsigned, 2 = hex, 3 = binary
	if (memOpt == 0) {
		y = sig;
		if (uns == sig) tip = hex+' '+bin;
		else tip = '('+uns+') '+hex+' '+bin;
	} else if (memOpt == 1) {
		y = uns;
		if (uns == sig) tip = hex+' '+bin;
		else tip = sig+' '+hex+' '+bin;
	} else if (memOpt == 3) {
		y = bin;
		if (uns == sig) tip = sig+' '+hex;
		else tip = sig+' ('+uns+') '+hex;
	} else {
		y = hex;
		if (uns == sig) tip = sig+' '+bin;
		else tip = sig+' ('+uns+') '+bin;
	}

	var elePosn = document.getElementById("a"+x); 
	if (!elePosn) {if (debug) alert("Simulator bug - cannot find a"+x+" to set new contents "+y);}
	else {
		elePosn.innerHTML = y;
		elePosn.title = tip;
	}
}

// note breakpoint and current are now orthogonal (in the program area)
function removeCodeHighlight()
{
	if (lastCodeHighlight >= 0) {
		var line = document.getElementById("lin"+lastCodeHighlight);
		line.classList.remove(current);
		lastCodeHighlight = -1;
	}
}

// indicate the line being executed - called with the real line number (i.e. not PC/4)
function showExecute(r)
{
	removeCodeHighlight();				// need to remove previous one (if any)
	addClassToLineNumber(r, current);
	lastCodeHighlight = r;
}

// indicate the line being executed - called with the real line number (i.e. not PC/4)
// this version called if stopping to also scroll to correct place
function showExecuteStop(r)
{
	removeCodeHighlight();				// need to remove previous one (if any)
	addClassToLineNumber(r, current);
	lastCodeHighlight = r;
	var pos = (r-18)*15.2395;
	if (r < 30) pos = 0;
	document.getElementById("source").scrollTop = pos;
}

// Indicate where the error is - called with the real line number (never the address)
function showError(r)
{
	byteCount = 0;			// prevent any future STEP or RUN trying to highlight instructions
	errorLineNum = r;
	if (serviceMode) return;
	addClassToLineNumber(r, error);
	var pos = (r-18)*15.2395;
	if (r < 30) pos = 0;
	document.getElementById("source").scrollTop = pos;
}

// called by reset to remove any existing markers
function removeError()
{
	if (errorLineNum >= 0) {
		removeClassFromLineNumber(errorLineNum, error);
		errorLineNum = -1;
	}
}

function addClassToLineNumber(r, classToAdd)
{
	if (r < 0) return; // should not happen
	var elePosn = document.getElementById("lin"+r);
	if (elePosn) {
		elePosn.classList.add(classToAdd)
	} else {
		if (debug) alert("could not find line "+r+" to add class "+classToAdd);
	}
}

function removeClassFromLineNumber(r, classToRemove)
{
	if (r < 0) return; // should not happen
	var elePosn = document.getElementById("lin"+r);
	if (elePosn) {
		elePosn.classList.remove(classToRemove)
	} else {
		if (debug) alert("could not find line "+r+" to remove class "+classToRemove);
	}
}

function removeMemHighlight()
{
	if (lastMemHighlight >= 0) {
		var x = overlay * 256;
		if (lastMemHighlight >= x && lastMemHighlight < (x+512)) {
			// should be a multiple of 4 but would go horribly wrong if wasn't
			x = Math.floor((lastMemHighlight-x)/4);
			document.getElementById("a"+x).classList.remove(current);
		}
		lastMemHighlight = -1;
	}
}

// r is a memory address
function setMemHighlight(r)
{
	removeMemHighlight();
	if (r < 0) return; // should not happen
	var x = overlay * 256;
	if (r >= x && r < (x+512)) {
		// should be a multiple of 4 but would go horribly wrong if wasn't
		x = Math.floor((r-x)/4);
		// since we have tested for a range it should always exist!
		document.getElementById("a"+x).classList.add(current);
	}
	lastMemHighlight = r; // remembered even if not set so can set if overlay changes
}

function IEKeyEnable()
{
	document.getElementById("xxbody").focus();	//	IE needs this to get single keys
}

function clearInputArea()
{
	setValue("input","&nbsp;");
}

function enableInput(secret)
{
	if (secret) setValue("input",inputFormSecret);
	else setValue("input",inputForm);
	var elePosn = document.getElementById("iForm");
	elePosn.focus();
	//delay(1000, "blinkoff");					// start blink
}

var inputForm = '<form action="javascript:inputSubmit()"><input id="iForm" type="text" placeholder="Input expected"></form>';
var inputFormSecret = '<form action="javascript:inputSubmit()"><input id="iForm" type="password" placeholder="Input expected"></form>';

// This sets a character in the character map
function showChar(i,x)		// note x is a number
{
	if (i < 0 || i >= 512) {
		if (debug) alert("Bad index for showChar() "+i);
		return;
	}
	var cList = "";				// default is to set a null string (for space)
	if (x > 32 && x < 128) {	// going to display this (non-space)
		if (x == '<') cList = '&lt;';
		else if (x == '>') cList = '&gt;';
		else if (x == '&') cList = '&amp;';
		else cList = String.fromCharCode(x);
	}
	setValue("c"+i,cList);
}

function getInput()
{
	var elePosn = document.getElementById("iForm");
	if (!elePosn) {
		if (debug) alert("Bad getInput call");
		return false;
	}
	return elePosn.value;
}

function inputRestore(val)
{
	setValue("input",val);
}

// clear the instruction decode area
function clearIR()
{
	setValue("ir", "&nbsp;");
	setValue("ur", "&nbsp;");
}

// set the top line of the instruction decode area (spaced hex number)
function setIR(y)
{
	setValue("ir", y);
}

// set the 2nd line of the instruction decode area (text decode of inst)
function addIR(y)
{
	setValue("ur", y);
}

// This is the class names that the display modes map into
//var modeNames = ['signed','unsigned','hex','binary'];

// file read functions
function fileOpen()
{
	// create our own button under where the file window often is that the user can click
	// if the file window does not show
	//text("fileRequest", fileText, 15, 12, 20, "red");
	//delay(1000, "bloff");
	//message("File Open required");
	
	// let's ignore the above and try a non blinking version in the message field
	// we cannot put tags inside a textarea so we need to rewrite the whole thing
	setValue("outer_console",'<textarea class="console" id="console" readonly>'+output1+'\n</textarea>'+fileText);
	
	// REMEMBER WE NEED TO RESET THIS LATER
	
	var elePosn = document.getElementById("read-file");
	elePosn.click();
}

var fileText = '<input type="file" id="read-file" onchange="read_file(this)" autocomplete="off"><button onclick="document.getElementById(&#39;read-file&#39;).click()">File Open required</button>';

function consoleReset()		// after get file click or do reset
{
	setValue("outer_console",'<textarea class="console" id="console" readonly>'+output1+'\n</textarea>');
}

// This code was adapted from:
// https://thiscouldbebetter.wordpress.com/2012/12/18/
// loading-editing-and-saving-a-text-file-in-html5-using-javascrip/
function saveFile(text,name)
{
	var textToSaveAsBlob = new Blob([text], {type:"text/plain"});
	var textToSaveAsURL = window.URL.createObjectURL(textToSaveAsBlob);
	var fileNameToSaveAs = "myprog.txt";

	var downloadLink = document.createElement("a");
	downloadLink.download = name;
	downloadLink.innerHTML = "Download File";
	downloadLink.href = textToSaveAsURL;
	downloadLink.onclick = destroyClickedElement;
	downloadLink.style.display = "none";
	document.body.appendChild(downloadLink);
 
	downloadLink.click();
} 
function destroyClickedElement(event)
{
    document.body.removeChild(event.target);
	//waitingForInput = false;
	//output1 = "Saved the File";
}


// this should work to set the contents of any named object
function setValue(id, y)
{
	if (serviceMode) return;
	var elePosn = document.getElementById(id); 
	if (!elePosn) {if (debug) alert("Simulator bug - cannot find "+id+" to set new contents "+y);}
	else elePosn.innerHTML = y;
}

function setClass(id,newClass)
{
	if (serviceMode) return;
	var elePosn = document.getElementById(id); 
	if (!elePosn) if (debug) {alert("Simulator bug - cannot find "+id+" to set new class "+newClass);}
	else elePosn.className = newClass;
}

function addClass(id,newClass)
{
	if (serviceMode) return;
	var elePosn = document.getElementById(id); 
	if (!elePosn) {if (debug) alert("Simulator bug - cannot find "+id+" to add class "+newClass);}
	else elePosn.classList.add(newClass);
}

function removeClass(id,oldClass)
{
	if (serviceMode) return;
	var elePosn = document.getElementById(id); 
	if (!elePosn) {if (debug) alert("Simulator bug - cannot find "+id+" to remove class "+newClass);}
	else elePosn.classList.remove(oldClass);
}

// New function to set or change the program area size
function setProgramWidth(width)
{
	if (serviceMode) return;
	// we need to move the other things out of the way
	editWin = width-4;
	var mid = width+50;
	var memPosn = width+400;
	if (memOpt == 3) memPosn += 170;
	var elePosn = document.getElementById("mxx");
	if (!elePosn) {if (debug) alert("Simulator bug - cannot find mxx to set left "+memPosn);}
	else elePosn.style.left = memPosn+"px";
	var elePosn = document.getElementById("processor"); 
	if (!elePosn) {if (debug) alert("Simulator bug - cannot find processor to set left "+mid);}
	else elePosn.style.left = mid+"px";
	var elePosn = document.getElementById("io"); 
	if (!elePosn) {if (debug) alert("Simulator bug - cannot find io to set left "+mid);}
	else elePosn.style.left = mid+"px";
	// now can make proram area wider
	var elePosn = document.getElementById("program"); 
	if (!elePosn) {if (debug) alert("Simulator bug - cannot find program to set width "+width);}
	else elePosn.style.width = width+"px";
	if (modifyingProgram) {
		elePosn = document.getElementById("pForm");
		elePosn.style.width = editWin+"px";
	}
}

// called when change in or out of binary mode
function setMemBinary(flag)		// memOpt has yet not been set when this is called
{
	if (serviceMode) return;
	var memPosn = editWin+404;
	if (flag) memPosn += 170;
	var elePosn = document.getElementById("mxx");
	if (!elePosn) {if (debug) alert("Simulator bug - cannot find mxx to set left "+memPosn);}
	else {
		elePosn.style.left = memPosn+"px";
		if (flag) elePosn.style.fontSize = binarySize+"%";
		else elePosn.style.fontSize = "100%";
	}
	elePosn = document.getElementById("registers");
	if (!elePosn) {if (debug) alert("Simulator bug - cannot find registers to set fontSize");}
	else {
		if (flag) elePosn.style.fontSize = binarySize+"%";
		else elePosn.style.fontSize = "100%";
	}
}

// here if window size changes
function changeDimensions()
{
	getDimensions();
	if (dynamicProgramWidth) {
		var tmp = maxWidth - 826;
		if (tmp < 300) tmp = 300;			// impose limits
		if (tmp > 3000) tmp = 3000;			// anything above 3000 just seems silly
		setProgramWidth(tmp);	// need to set something as window reduces in size
	}
}

// Event compressing and logging functions
function evStep()		// special case, so own call
{
	if (serviceMode) return;		// no tracking in service mode (for now anyway)
	if (eventCount == 0) {
		evFirst();
		evPusha("St1");	// it's really unlikely that the first event will be Step
		return;			// but we might as well allow for it
	}
	++eventStep;
	if (eventStep < 100 && (Date.now() - logTime) < 300000) return;
	// force a report every 100 steps or 5 minutes
	evPusha("St"+eventStep);
	eventStep = 0;
}
	
// Log a cross-reference in Matomo on first call
function evFirst()
{
	_paq.push(["trackEvent","First",plh_url+" "+plh_id]);
	eventCount = 55;		// force the first event to also log in new system
}

// general button event
function evPush(button)
{
	if (serviceMode) return;		// no tracking in service mode (for now anyway)
	if (eventCount == 0) evFirst();
	if (eventStep != 0) {
		evPusha("St"+eventStep);
		eventStep = 0;
	}
	evPusha(button);
}

// record an event (can be string as well as button)
function evPusha(button)
{	
	if (eventList == "") eventList = button;
	else eventList += " "+button;
	// 1500 is a reasonable limit to send in a URL (unlikely but BAD if 2048 total exceeded)
	if (eventList.length < 1500 && (Date.now()-logTime) < 300000 && ++eventCount < 53) return;

	// here if we are going to send a log report to the new system
	var UrlToSend = "https://www.peterhigginson.co.uk:/log/log.php?u="+plh_url+"&i="+plh_id+"&x="+eventList;
	if (window.fetch) {
		// Try this alternative
		const myInit = {method: 'GET', mode: 'no-cors', keepalive: true};
		var myRequest = new Request(UrlToSend);
		fetch(myRequest, myInit);
	} else {
		// THIS WORKED EXCEPT WHEN MOVING AWAY FROM THE PAGE (CORS errors)
		var xhttp = new XMLHttpRequest();
		xhttp.open("GET", UrlToSend+" OLD", true);	// I did try POST but getting url parameters is harder
		xhttp.send();						// true sets ASYNC (and SYNC isn't allowed for onbeforeunload etc.)
	}
	// now reset all the variables to start saving up events again (should the instance continue)
	logTime = Date.now();
	eventList = "";
	eventCount = 2;
}

// here (we hope) when the user closes the window
function flushLog()
{
	if (serviceMode) return;	// probably can't happen but better a wasted line than a hard to find bug

	if (eventCount > 1) {		// if we haven't started, no point in recording a finish
		eventCount = 55;		// force the event to flush the log
		evPush("Unld");			// Problem with delay to make sure message is actually sent
		eventCount = 1;			// avoid sending two consecutive Unloads
	}
}
