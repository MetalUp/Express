import { filterPythonCmpinfo, filterPythonStderr, findPythonFunctions, validatePythonExpression, wrapPythonExpression, wrapPythonFunctions } from "./python-helpers";

describe('Python Helpers', () => {
 

  // placeholders really


  beforeEach(async () => {
   
  });

  it('should wrap expressions', () => {
    const v = wrapPythonExpression('test');
    expect(v).not.toBe('test');
  });

  it('should not wrap functions', () => {
    const v = wrapPythonFunctions('test');
    expect(v).toBe('test');
  });

  it('should validate', () => {
    const v = validatePythonExpression('test');
    expect(v).toBe('');
  });

  it('should filter cmpinfo', () => {
    const v = filterPythonCmpinfo('something Error: an error');
    expect(v).toBe(' Error: an error');
  });

  it('should not filter cmpinfo if no match', () => {
    const v = filterPythonCmpinfo('test');
    expect(v).toBe('test');
  });
  
  it('should filter stderr', () => {
    const v = filterPythonStderr('something Error: an error');
    expect(v).toBe(' Error: an error');
  });

  it('should not filter stderr if no match', () => {
    const v = filterPythonStderr('test');
    expect(v).toBe('test');
  });

  it('should find no functions', () => {
    const v = findPythonFunctions('test');
    expect(v).toEqual([]);
  });
  
  it('should find functions', () => {
    const v = findPythonFunctions('Func() Action()');
    expect(v).toEqual(["Func", "Action"]);
  });
});
