def run_tests() :
    fn = keep_within_bounds.__name__
    def test(p1, expected):
        test_function(fn, expected, keep_within_bounds(p1), [p1])
    test(399, 399)
    test(400, 0)
    test(401, 1)
    test(419, 19)
    test(0, 0)
    test(-1, 399)
    test(-20, 380)
    all_tests_passed()