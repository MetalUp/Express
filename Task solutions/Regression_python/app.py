from best_fit import *
from framework import *
from tests import *

# This is the 'procedural' style code that uses the functions to create an 'app'
# grid = list(map(lambda n: random.randint(0,3) > 2, range(0, size)))

#def run_app(grid):
#    while True:
#        print(as_grid(grid))
#        grid = next_generation(grid);
#        time.sleep(0.5)
#        os.system('cls') # 'clear' on Linux; 'cls' on Windows

run_tests()
