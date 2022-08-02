import random, time, os
from life import *
from framework import *
from tests import *

# This is the 'procedural' style code that uses the functions to create an 'app'
grid = list(map(lambda n: random.randint(0,3) > 2, range(0, size)))

def run_app(grid):
    while True:
        for i in range(size) :
            print("* " if grid[i] else "  ", end="\n" if i % w == 0 else "")
        grid = nextGeneration(grid);
        time.sleep(0.5)
        os.system('cls') # 'clear' on Linux; 'cls' on Windows

#run_app(grid)
run_tests_neighbour_cells()
run_tests_keep_within_bounds()
run_tests_adjusted_neighbour_cells()
run_tests_live_neighbours()
run_tests_will_live()
run_tests_next_cell_value()
run_tests_next_generation()
