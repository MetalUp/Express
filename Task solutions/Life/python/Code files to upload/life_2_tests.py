def run_tests() :
    test_keep_within_bounds(399, 399)
    test_keep_within_bounds(400, 0)
    test_keep_within_bounds(401, 1)
    test_keep_within_bounds(419, 19)
    test_keep_within_bounds(0, 0)
    test_keep_within_bounds(-1, 399)
    test_keep_within_bounds(-20, 380)

    test_neighbour_cells(30, [ 9, 10, 11, 29, 31, 49, 50, 51 ])
    all_tests_passed()

def test_neighbour_cells(cell, expected) :
    fn = neighbour_cells.__name__
    n = neighbour_cells(cell)
    msg = fail +f"{fn}({cell})."
    assert_true(fn, cell, len(n) == 8, msg + f" Expected: 8 elements Actual: {len(n)}")
    for val in expected :
        assert_true(fn, cell, val in n, msg + f" List does not contain: {val}")

def test_keep_within_bounds(p1, expected):
    fn = keep_within_bounds.__name__
    test_function(fn, expected, keep_within_bounds(p1), [p1])