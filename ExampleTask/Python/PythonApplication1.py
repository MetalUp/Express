import random, time, os

size = 400 # Total number of cells in grid
w = 20 # Number of cells horizontally
grid = [] #  a simple list (of Boolean cells), so  the grid 'wraps around' horizontally

# The *relative* positions of each of the eight neighbours to any given cell
relativeNeighbourPositions = [ -w - 1, -w, -w + 1, -1, +1, w - 1, w, w + 1 ]

# 'Wraps' the grid vertically so that top row and bottom row are neighbours
def keepWithinGrid(i) : return  i - size if i >= size else  i + size if i < 0 else i

def liveNeighbourCount(grid, cellNo) : return len(list(filter(lambda relPos: grid[keepWithinGrid(cellNo + relPos)], relativeNeighbourPositions)))

def willLive(currentlyAlive, liveNeighbours) : return (liveNeighbours > 1 and liveNeighbours < 4) if currentlyAlive else liveNeighbours == 3

def nextCellValue(grid, c) : return willLive(grid[c], liveNeighbourCount(grid, c));

def nextGeneration(grid) : return list(map(lambda n : nextCellValue(grid, n), range(0, size)))

# And this is the 'procedural' style code that uses the functions to create an 'app'
grid = list(map(lambda n: random.randint(0,3) > 2, range(0, size)))

while True:
    for i in range(size) :
        print("* " if grid[i] else "  ", end="\n" if i % w == 0 else "")
    grid = nextGeneration(grid);
    time.sleep(0.5)
    os.system('cls') # 'clear' on Linux; 'cls' on Windows