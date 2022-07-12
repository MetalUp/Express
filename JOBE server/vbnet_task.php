<?php defined('BASEPATH') OR exit('No direct script access allowed');

/* ==============================================================
 *
 * VB.Net
 *
 * ==============================================================
 *
 * @license    http://www.gnu.org/copyleft/gpl.html GNU GPL v3 or later
 */

require_once('application/libraries/LanguageTask.php');

class VBNet_Task extends Task {

    public function __construct($filename, $input, $params) {
        parent::__construct($filename, $input, $params);
        $this->default_params['memorylimit'] = 600; 

        $this->default_params['compileargs'] = array();
    }

    public static function getVersionCommand() {
        return array('mono --version', '/version ([0-9.]+)/');
    }

    public function compile() {
        $src = basename($this->sourceFileName);
        $this->executableFileName = $execFileName = "$src.exe";
        $compileargs = $this->getParam('compileargs');
        $cmd = "vbnc " . implode(' ', $compileargs) . "$src -out:$src.exe 2>&1";
        list($output, $this->cmpinfo) = $this->run_in_sandbox($cmd);
        if(preg_match('/.*error.*/', $output, $matches)) {
                $this->cmpinfo = $matches[0];

        }
    }

    // A default name for VB programs
    public function defaultFileName($sourcecode) {
        return 'prog.vb';
    }
     // The executable is the output from the compilation
    public function getExecutablePath() {
        return "/usr/bin/mono";
    }


    public function getTargetFile() {
        return $this->sourceFileName . ".exe";
    }
};
