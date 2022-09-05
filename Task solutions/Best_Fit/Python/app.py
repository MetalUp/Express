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

example_data = [(0,2.673040396),(1,3.550437254),(2,5.063963424),(3,6.445493589),(4,5.959864362),(5,7.826499306),(6,5.628729698),(7,10.65942485),(8,8.010532477),(9,10.67121885),(10,9.000669447),(11,13.50830072),(12,11.43464406),(13,13.58275437),(14,13.10821319),(15,10.28617997),(16,17.86384888),(17,15.20409345),(18,17.00764549),(19,12.11596949)]

print(plot(example_data))
#run_tests()
