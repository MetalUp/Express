import { filterCSharpCmpinfo, filterCSharpStderr, findCSharpFunctions, validateCSharpExpression, wrapCSharpExpression } from "./csharp-helpers";

describe('CSharp Helpers', () => {
 

  // placeholders really


  beforeEach(async () => {
   
  });

  it('should wrap code', () => {
    const v = wrapCSharpExpression('test');
    expect(v).not.toBe('test');
  });

  it('should validate', () => {
    const v = validateCSharpExpression('test');
    expect(v).toBe('');
  });

  it('should filter cmpinfo', () => {
    const v = filterCSharpCmpinfo('something CSXXXX: a compile error');
    expect(v).toBe('CSXXXX: a compile error');
  });

  it('should not filter cmpinfo if no match', () => {
    const v = filterCSharpCmpinfo('test');
    expect(v).toBe('test');
  });
  
  it('should filter stderr', () => {
    const v = filterCSharpStderr('SomethingException something');
    expect(v).toBe('SomethingException');
  });

  it('should not filter stderr if no match', () => {
    const v = filterCSharpStderr('test');
    expect(v).toBe('test');
  });

  it('should find no functions', () => {
    const v = findCSharpFunctions('test');
    expect(v).toEqual([]);
  });
  
  it('should find functions', () => {
    const v = findCSharpFunctions('Func() Action()');
    expect(v).toEqual(["Func", "Action"]);
  });
  
});
