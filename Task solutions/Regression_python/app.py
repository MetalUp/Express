from best_fit import *
from framework import *
from tests import *

# This is the 'procedural' style code that uses the functions to create an 'app'
# Assumes all x,y values are in range 0,<20 
def plot(l) :
    grid = [ [0]*20 for i in range(20)]
    s = "20"
    for p in l :
        grid[int(p[0])][int(p[1])] = True
    for y in range(19,-1,-1) :
        s += "|"
        for x in range(0,20) :
            s += "■ " if grid[x][y] else "  "
        s += "\n"
    s += "0---------------------------------------20"
    return s;

print(plot(l1))
#run_tests()
