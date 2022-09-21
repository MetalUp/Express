def run_tests() :
    test_neighbour_cells(30, [ 9, 10, 11, 29, 31, 49, 50, 51 ])
    all_tests_passed()

def test_neighbour_cells(cell, expected) :
    fn = neighbour_cells.__name__
    n = neighbour_cells(cell)
    msg = fail +f"{fn}({cell})."
    assert_true(fn, cell, len(n) == 8, msg + f" Expected: 8 elements Actual: {len(n)}")
    for val in expected :
        assert_true(fn, cell, val in n, msg + f" List does not contain: {val}")