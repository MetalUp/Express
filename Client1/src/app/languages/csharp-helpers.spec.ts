import { wrapCSharpExpression, wrapCSharpFunctions } from "./csharp-helpers";

describe('CSharp Helpers', () => {

  beforeEach(async () => {

  });

  it('should wrap expressions', () => {
    const v = wrapCSharpExpression('test');
    expect(v).not.toBe('test');
  });

  it('should wrap functions', () => {
    const v = wrapCSharpFunctions('test');
    expect(v).not.toBe('test');
  });
});
