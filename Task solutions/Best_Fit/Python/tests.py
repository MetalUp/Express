from framework import *
from best_fit import *

empty = {}
l1 = { (0.71, 1.12), (3.56, 5.36), (7.83, 9.04) }

def run_tests() :
    test_term(sum_x, empty, 0)
    test_term(sum_x, l1, 12.100000000000001)

    test_term(sum_y, empty, 0)
    test_term(sum_y, l1, 15.52)

    test_term(sum_x_sq, empty, 0)
    test_term(sum_x_sq, l1, 74.4866)

    test_term(sum_xy, empty, 0)
    test_term(sum_xy, l1, 90.66)

    test_function(calc_a.__name__, 0.7663359541491319, calc_a(sum_x(l1), sum_x_sq(l1), sum_y(l1), sum_xy(l1), len(l1)), l1)
    test_function(calc_b.__name__, 1.0926439783101325, calc_b(sum_x(l1), sum_x_sq(l1), sum_y(l1), sum_xy(l1), len(l1)), l1)

    test_function(best_fit_from_summary_terms.__name__, [0.7663359541491319, 1.0926439783101325], best_fit_from_summary_terms(sum_x(l1), sum_x_sq(l1), sum_y(l1), sum_xy(l1), len(l1)), l1)
    test_function(best_fit_from_points.__name__, [0.7663359541491319, 1.0926439783101325], best_fit_from_points(l1), l1)

    all_tests_passed()

def test_term(f,  data, expected) :
    test_function(f.__name__, expected, f(data), data);

