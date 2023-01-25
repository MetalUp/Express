// © 2019 Peter Higginson (plh256 at hotmail.com)
// Set of functions for use with JavaScript sessons
// Minimised version

// note 0,0 is the top left corner, any names must go in quotes e.g. "name"

// Provided functions
// 	text("name", "text", x, y, size, "colour");
// 	text("name", "text", x, y, size, "colour", "attribute", "value", "font"); 	// attribute, value and font are optional
// 	remove("name");
//	randomInt(min, max);						// note returns the result
//	trapKey("character", "upFunction", "downFn");		// note functions may have one parameter - the character trapped
//	getMilliseconds();
// 	timeout = delay(milliseconds, "function", parameters);  	// note returns a value which can be used with clearTimeout
// System functions
//	clearTimeout(timeout);
//	alert("message");
//	prompt("Question : ", "default answer");
//	location.reload();						// reload the page (to restart the game)

var maxHeight, maxWidth; 	//current window dimensions
var lastKey=0;				// last key pressed
var noKeyEffects=false;		// when running

function startIt()
{
	getDimensions();
	main();
}

function getDimensions()
{
	if(typeof(window.innerWidth) == 'number' ) {	//Chrome
		maxWidth = window.innerWidth;
		maxHeight = window.innerHeight;
	} else { 							//IE 6+ in 'standards compliant mode'
		maxWidth = document.documentElement.clientWidth;
		maxHeight = document.documentElement.clientHeight;
	}
}
// for ARMlite scaling made all the CSS box borders visible so was useless!!

function randomInt(min,max)
{
    return Math.floor(Math.random()*(max-min+1)+min);
}

function getMilliseconds()
{
	var d = new Date();
	return d.getTime();
}

function delay(ms, fn)	// has additional parameters as arguments to fn
{
	var tmp = fn+"(";
	for (var i = 2; i < arguments.length; i++) {
		if (i > 2) tmp += ",";
		tmp += arguments[i];
	}
	tmp += ")";
	// console.log("delay "+ms+"ms, call "+tmp);
	return setTimeout(tmp, ms);
}

var chrsToMatch = [];
var dnFnCall = [];
var upFnCall = [];
var chrsDown = [];

function trapKey(chr, fnDn, fnUp)		// note fnDn/fnUp can have one parameter - the character trapped
{
	var indx = chrsToMatch.indexOf(chr);
	if (indx == -1) {				// not seen this character before
		indx = chrsToMatch.push(chr) - 1;
		chrsDown[indx] = 0;
	}
	dnFnCall[indx] = fnDn;
	upFnCall[indx] = fnUp;
}

function keyDown(e)
{
	var x=e.keyCode;
	if (x == 8) {
		var d = e.srcElement || e.target;
		var preventKeyPress;
		switch (d.tagName.toUpperCase()) {
		case 'TEXTAREA':
			preventKeyPress = d.readOnly || d.disabled;
			break;
		case 'INPUT':
			preventKeyPress = d.readOnly || d.disabled ||
				(d.attributes["type"] && ["radio", "checkbox", "submit", "button"].indexOf(d.attributes["type"].value.toLowerCase()) >= 0);
			// error above in previous versions because $.inArray is a jQuery function which we don't have
			break;
		case 'DIV':
			preventKeyPress = d.readOnly || d.disabled || !(d.attributes["contentEditable"] && d.attributes["contentEditable"].value == "true");
			break;
		default:
			preventKeyPress = true;
			break;
		}
		if (preventKeyPress) e.preventDefault();
	}
	var keychar=String.fromCharCode(x);
	if (x == 27 || x == 9) keychar = x;
	var indx = chrsToMatch.indexOf(keychar);
	if (indx == -1) {
		// waste of space on console log
		// console.log("Key " + keychar + " was pressed down "+x);
		if (noKeyEffects) {
			if ((keyboardMask & 4) == 0) { // bit 2 is accept all chars
				if (x > 90) return;			// ignore keys > Z
				if ((keyboardMask & 2) == 0) {// bit 1 is accept arrows
					if (x < 48) return;		// ignore keys < 0
				} else {
					if (x < 37 || (x > 40 && x < 48)) return
				}
			}
			lastKey = x;
			testKeyInterrupt();		// hope not called unless wanted
			e.preventDefault();		// using keys
		}
	} else {
		// when you hold a key down you do not get ups
		// if (chrsDown[indx] == 0) {
			chrsDown[indx] = 1;
			if (dnFnCall[indx]) window[dnFnCall[indx]](e);
		//}
	}
}

function keyUp(e)
{
	var x=e.keyCode;
	var keychar=String.fromCharCode(x);
	if (x == 27 || x == 9) keychar = x;
	var indx = chrsToMatch.indexOf(keychar);
	if (indx != -1 && chrsDown[indx] == 1) {
		chrsDown[indx] = 0;
		if (upFnCall[indx]) window[upFnCall[indx]](e);		
	}
}
