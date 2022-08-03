def run_tests() :
    fn = adjusted_neighbour_cells.__name__
    def test(cell, expected) :
        n = adjusted_neighbour_cells(cell)
        msg = fail +f"{fn}({cell})."
        assert_true(fn, cell, len(n) == 8, msg + f" Expected: 8 elements Actual: {len(n)}")
        for val in expected :
            assert_true(fn, cell, val in n, msg + f" List does not contain: {val}")
        
    test(399, [ 378, 379, 380, 398, 0, 18, 19, 20]);
    test(380, [ 359, 360, 361, 379, 381, 399, 0,1]);
    test(0, [ 379, 380, 381, 399, 1, 19, 20, 21]);
    test(19, [ 398, 399, 0, 18, 20, 38, 39, 40]);
    all_tests_passed()