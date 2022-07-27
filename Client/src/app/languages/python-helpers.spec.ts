import { wrapPythonExpression, wrapPythonFunctions } from "./python-helpers";

describe('Python Helpers', () => {

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
});
