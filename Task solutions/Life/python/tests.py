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
    all_tests_passed(fn)


def run_tests_relative_neighbour_positions() :
    fn = "relative_neighbour_positions"
    def test(result, message) :
        assert_true(fn, result, message)
    n = relative_neighbour_positions;
    test(len(n) == 8, f"Returned list should contain 8 elements, currently has {len(n)}")
    for val in [-w - 1, -w, -w + 1, -1, +1, w - 1, w, w + 1 ] :
        test(val in n, f"Returned list is missing the value: {val}")
    all_tests_passed(fn)

def run_tests_keep_within_grid() :
    fn = keep_within_grid.__name__
    def test(p1, expected):
        test_function(fn, expected, keep_within_grid(p1), [p1])
    test(399, 399)
    test(400, 0)
    test(401, 1)
    test(419, 19)
    test(0, 0)
    test(-1, 399)
    test(-20, 380)
    all_tests_passed(fn)

def run_tests_live_neighbour_count() :
    fn = live_neighbour_count.__name__
    def test(p1, expected) :
        test_function(fn, expected, live_neighbour_count(testGrid, p1), [p1])
    test(0, 4)
    test(19, 4)
    test(30, 2)
    test(44, 3)
    test(59, 4)
    test(60, 3)
    test(399, 4)
    all_tests_passed(fn)

testGrid = [False, True, False, True, False, False, True, True, True, False, False, True, True, False, True, True, False, False, True, True, True, True, True, True, False, False, True, False, True, True, True, False, True, True, True, True, False, True, False, True, False, False, True, True, False, False, False, False, True, False, False, False, True, False, False, False, True, False, False, False, True, False, False, True, False, False, True, False, False, True, True, False, False, False, True, True, False, False, True, True, False, True, True, True, True, False, False, False, False, False, False, False, False, True, True, False, True, False, False, True, True, False, False, False, False, True, False, False, False, False, False, False, False, False, False, False, False, False, False, False, True, False, True, False, False, False, True, False, False, True, True, False, True, False, True, True, False, False, False, False, False, True, False, False, True, False, True, False, True, False, True, False, True, False, False, True, False, True, False, True, False, False, True, False, False, True, True, False, True, False, False, True, False, True, False, True, True, False, False, False, False, True, False, False, True, False, False, False, False, True, False, False, False, True, False, True, False, True, False, False, False, False, False, True, True, False, True, True, False, False, False, False, False, True, False, False, True, True, True, False, False, False, True, False, True, False, False, True, True, False, True, False, True, False, True, False, False, True, False, True, False, True, False, True, False, False, False, True, False, False, False, True, False, True, False, True, False, False, True, False, False, False, False, False, False, False, False, False, False, True, True, True, False, False, True, False, True, False, False, True, False, True, False, True, False, False, False, False, False, True, False, True, False, False, False, False, True, True, False, False, True, True, False, True, True, False, True, True, False, True, False, False, True, False, True, False, False, True, False, True, True, False, False, False, False, False, True, True, False, False, False, True, True, True, False, False, False, True, True, False, True, True, True, False, False, False, True, False, True, False, False, False, False, False, False, False, False, False, False, True, False, False, False, False, True, True, False, True, False, False, True, True, False, False, True, True, True, False, False, False, False, False, False, False, True, False, False, False, False, False, False, False, False, False, True, False, False, False, True, False]

def run_tests_next_cell_value() :
    fn = next_cell_value.__name__
    def test(c, expected):
        test_function(fn, expected, next_cell_value(testGrid, c), ["testGrid", c]);
    test(0, False); #Currently: False, 4
    test(19, False); #Currently: True,4
    test(30, True); #Currently: True,2
    test(44, True); #Currently: False,3
    test(59, False); #Currently: False,4
    test(60, True); #Currently: True,3
    test(399, False); #Currently: False,4
    all_tests_passed(fn)

def run_tests_next_generation():
    fn = next_generation.__name__
    nextGen = [False, True, False, True, True, True, True, False, True, False, True, True, True, False, False, False, True, True, False, False, False, False, False, False, True, False, True, False, False, False, True, False, False, False, False, False, False, True, False, False, False, False, False, False, True, False, False, False, True, False, False, False, True, False, False, False, True, True, False, False, True, False, False, False, False, False, False, False, False, True, False, False, False, False, True, False, True, True, True, False, False, True, True, True, True, True, False, False, False, False, False, False, False, True, True, False, False, False, True, False, False, False, False, False, True, True, False, False, False, False, False, False, False, False, False, False, False, False, False, True, True, False, False, False, False, False, True, True, False, True, True, False, False, True, True, True, True, False, False, False, True, True, True, True, False, False, True, False, True, False, True, False, True, False, False, False, False, False, False, False, True, True, True, True, True, False, True, False, True, False, True, True, False, True, False, True, False, True, True, False, False, False, True, False, True, False, False, False, True, False, False, False, False, True, False, True, False, False, True, False, False, False, True, False, True, False, True, True, False, True, False, False, True, True, False, True, False, False, False, False, False, False, True, False, True, True, False, False, True, False, False, True, True, False, True, True, False, False, False, True, False, False, True, True, False, False, False, True, False, False, False, False, False, True, False, True, True, True, True, True, False, False, False, False, False, False, False, False, True, True, False, True, False, False, True, False, True, False, True, True, False, True, False, True, True, False, False, False, False, True, False, True, True, True, False, False, True, True, False, False, False, True, False, True, True, True, True, True, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, True, False, False, False, False, False, False, False, True, True, True, False, False, False, True, False, False, False, True, False, False, False, False, False, False, True, False, True, False, False, True, True, True, True, True, True, True, True, True, False, True, True, True, True, True, False, False, False, False, False, False, True, True, False, False, False, False, False, False, False, True, True, False, False, False, True, False, True, False, True, False, False, False, True, True, True, False]
    test_function(fn, nextGen, next_generation(testGrid), "testGrid")
    all_tests_passed(fn)
