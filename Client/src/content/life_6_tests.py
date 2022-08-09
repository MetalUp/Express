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
    all_tests_passed()