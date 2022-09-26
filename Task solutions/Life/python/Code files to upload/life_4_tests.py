def run_tests() :
    test_live_neighbours(0, 4)
    test_live_neighbours(19, 4)
    test_live_neighbours(30, 2)
    test_live_neighbours(44, 3)
    test_live_neighbours(59, 4)
    test_live_neighbours(60, 3)
    test_live_neighbours(399, 4)

    test_adjusted_neighbour_cells(399, [ 378, 379, 380, 398, 0, 18, 19, 20]);
    test_adjusted_neighbour_cells(380, [ 359, 360, 361, 379, 381, 399, 0,1]);
    test_adjusted_neighbour_cells(0, [ 379, 380, 381, 399, 1, 19, 20, 21]);
    test_adjusted_neighbour_cells(19, [ 398, 399, 0, 18, 20, 38, 39, 40]);

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

def test_adjusted_neighbour_cells(cell, expected) :
    fn = adjusted_neighbour_cells.__name__
    n = adjusted_neighbour_cells(cell)
    msg = fail +f"{fn}({cell})."
    assert_true(fn, cell, len(n) == 8, msg + f" Expected: 8 elements Actual: {len(n)}")
    for val in expected :
        assert_true(fn, cell, val in n, msg + f" List does not contain: {val}")

testGrid = [False, True, False, True, False, False, True, True, True, False, False, True, True, False, True, True, False, False, True, True, True, True, True, True, False, False, True, False, True, True, True, False, True, True, True, True, False, True, False, True, False, False, True, True, False, False, False, False, True, False, False, False, True, False, False, False, True, False, False, False, True, False, False, True, False, False, True, False, False, True, True, False, False, False, True, True, False, False, True, True, False, True, True, True, True, False, False, False, False, False, False, False, False, True, True, False, True, False, False, True, True, False, False, False, False, True, False, False, False, False, False, False, False, False, False, False, False, False, False, False, True, False, True, False, False, False, True, False, False, True, True, False, True, False, True, True, False, False, False, False, False, True, False, False, True, False, True, False, True, False, True, False, True, False, False, True, False, True, False, True, False, False, True, False, False, True, True, False, True, False, False, True, False, True, False, True, True, False, False, False, False, True, False, False, True, False, False, False, False, True, False, False, False, True, False, True, False, True, False, False, False, False, False, True, True, False, True, True, False, False, False, False, False, True, False, False, True, True, True, False, False, False, True, False, True, False, False, True, True, False, True, False, True, False, True, False, False, True, False, True, False, True, False, True, False, False, False, True, False, False, False, True, False, True, False, True, False, False, True, False, False, False, False, False, False, False, False, False, False, True, True, True, False, False, True, False, True, False, False, True, False, True, False, True, False, False, False, False, False, True, False, True, False, False, False, False, True, True, False, False, True, True, False, True, True, False, True, True, False, True, False, False, True, False, True, False, False, True, False, True, True, False, False, False, False, False, True, True, False, False, False, True, True, True, False, False, False, True, True, False, True, True, True, False, False, False, True, False, True, False, False, False, False, False, False, False, False, False, False, True, False, False, False, False, True, True, False, True, False, False, True, True, False, False, True, True, True, False, False, False, False, False, False, False, True, False, False, False, False, False, False, False, False, False, True, False, False, False, True, False]

def test_live_neighbours(p1, expected) :
    fn = live_neighbours.__name__
    test_function(fn, expected, live_neighbours(testGrid, p1), [p1])