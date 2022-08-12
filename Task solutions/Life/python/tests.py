from framework import *
from life import *

def run_tests_will_live() :
    fn = will_live.__name__
    def test(alive,  neighbours, expected):
       test_function(fn, expected, will_live(alive, neighbours), [alive, neighbours])
    test(False, 0, False)
    test(False, 1, False)
    test(False, 2, False)
    test(False, 3, True)
    test(False, 4, False)
    test(False, 5, False)
    test(False, 6, False)
    test(False, 7, False)
    test(False, 8, False)
    test(True, 0, False)
    test(True, 1, False)
    test(True, 2, True)
    test(True, 3, True)
    test(True, 4, False)
    test(True, 5, False)
    test(True, 6, False)
    test(True, 7, False)
    test(True, 8, False)
    all_tests_passed()


def run_tests_neighbour_cells() :
    fn = neighbour_cells.__name__
    def test(cell, expected) :
        n = neighbour_cells(cell)
        msg = fail +f"{fn}({cell})."
        assert_true(fn, cell, len(n) == 8, msg + f" Expected: 8 elements Actual: {len(n)}")
        for val in expected :
            assert_true(fn, cell, val in n, msg + f" List does not contain: {val}")
        
    test(30, [ 9, 10, 11, 29, 31, 49, 50, 51 ])
    all_tests_passed()

def run_tests_keep_within_bounds() :
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

def run_tests_adjusted_neighbour_cells() :
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

def run_tests_live_neighbours() :
    fn = live_neighbours.__name__
    def test(p1, expected) :
        test_function(fn, expected, live_neighbours(testGrid, p1), [p1])
    test(0, 4)
    test(19, 4)
    test(30, 2)
    test(44, 3)
    test(59, 4)
    test(60, 3)
    test(399, 4)
    all_tests_passed()

testGrid = [False, True, False, True, False, False, True, True, True, False, False, True, True, False, True, True, False, False, True, True, True, True, True, True, False, False, True, False, True, True, True, False, True, True, True, True, False, True, False, True, False, False, True, True, False, False, False, False, True, False, False, False, True, False, False, False, True, False, False, False, True, False, False, True, False, False, True, False, False, True, True, False, False, False, True, True, False, False, True, True, False, True, True, True, True, False, False, False, False, False, False, False, False, True, True, False, True, False, False, True, True, False, False, False, False, True, False, False, False, False, False, False, False, False, False, False, False, False, False, False, True, False, True, False, False, False, True, False, False, True, True, False, True, False, True, True, False, False, False, False, False, True, False, False, True, False, True, False, True, False, True, False, True, False, False, True, False, True, False, True, False, False, True, False, False, True, True, False, True, False, False, True, False, True, False, True, True, False, False, False, False, True, False, False, True, False, False, False, False, True, False, False, False, True, False, True, False, True, False, False, False, False, False, True, True, False, True, True, False, False, False, False, False, True, False, False, True, True, True, False, False, False, True, False, True, False, False, True, True, False, True, False, True, False, True, False, False, True, False, True, False, True, False, True, False, False, False, True, False, False, False, True, False, True, False, True, False, False, True, False, False, False, False, False, False, False, False, False, False, True, True, True, False, False, True, False, True, False, False, True, False, True, False, True, False, False, False, False, False, True, False, True, False, False, False, False, True, True, False, False, True, True, False, True, True, False, True, True, False, True, False, False, True, False, True, False, False, True, False, True, True, False, False, False, False, False, True, True, False, False, False, True, True, True, False, False, False, True, True, False, True, True, True, False, False, False, True, False, True, False, False, False, False, False, False, False, False, False, False, True, False, False, False, False, True, True, False, True, False, False, True, True, False, False, True, True, True, False, False, False, False, False, False, False, True, False, False, False, False, False, False, False, False, False, True, False, False, False, True, False]

def run_tests_next_cell_value() :
    fn = next_cell_value.__name__
    def test(c, expected):
        test_function(fn, expected, next_cell_value(example_cells, c), ["example_cells", c]);
    test(0, False); #Currently: False, 4
    test(19, False); #Currently: True,4
    test(30, True); #Currently: True,2
    test(44, True); #Currently: False,3
    test(59, False); #Currently: False,4
    test(60, True); #Currently: True,3
    test(399, False); #Currently: False,4
    all_tests_passed()

def run_tests_next_generation():
    fn = next_generation.__name__
    nextGen = [False, True, False, True, True, True, True, False, True, False, True, True, True, False, False, False, True, True, False, False, False, False, False, False, True, False, True, False, False, False, True, False, False, False, False, False, False, True, False, False, False, False, False, False, True, False, False, False, True, False, False, False, True, False, False, False, True, True, False, False, True, False, False, False, False, False, False, False, False, True, False, False, False, False, True, False, True, True, True, False, False, True, True, True, True, True, False, False, False, False, False, False, False, True, True, False, False, False, True, False, False, False, False, False, True, True, False, False, False, False, False, False, False, False, False, False, False, False, False, True, True, False, False, False, False, False, True, True, False, True, True, False, False, True, True, True, True, False, False, False, True, True, True, True, False, False, True, False, True, False, True, False, True, False, False, False, False, False, False, False, True, True, True, True, True, False, True, False, True, False, True, True, False, True, False, True, False, True, True, False, False, False, True, False, True, False, False, False, True, False, False, False, False, True, False, True, False, False, True, False, False, False, True, False, True, False, True, True, False, True, False, False, True, True, False, True, False, False, False, False, False, False, True, False, True, True, False, False, True, False, False, True, True, False, True, True, False, False, False, True, False, False, True, True, False, False, False, True, False, False, False, False, False, True, False, True, True, True, True, True, False, False, False, False, False, False, False, False, True, True, False, True, False, False, True, False, True, False, True, True, False, True, False, True, True, False, False, False, False, True, False, True, True, True, False, False, True, True, False, False, False, True, False, True, True, True, True, True, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, True, False, False, False, False, False, False, False, True, True, True, False, False, False, True, False, False, False, True, False, False, False, False, False, False, True, False, True, False, False, True, True, True, True, True, True, True, True, True, False, True, True, True, True, True, False, False, False, False, False, False, True, True, False, False, False, False, False, False, False, True, True, False, False, False, True, False, True, False, True, False, False, False, True, True, True, False]
    test_function(fn, nextGen, next_generation(example_cells), "example_cells")
    all_tests_passed()
