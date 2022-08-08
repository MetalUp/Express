def run_tests() :
    fn = double.__name__
    def test(inp, out) :
        n = double(inp)
        assert_true(fn, "", n == out, f" Expected: {out}  Actual: {n}")
      
    test(3, 9)
    all_tests_passed()