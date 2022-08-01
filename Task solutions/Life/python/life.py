

size = 400 # Total number of cells in grid
w = 20 # Number of cells horizontally
grid = [] #  a simple list (of Boolean cells), so  the grid 'wraps around' horizontally

# The *relative* positions of each of the eight neighbours to any given cell
relative_neighbour_positions = [ -w - 1, -w, -w + 1, -1, +1, w - 1, w, w + 1 ]

# 'Wraps' the grid vertically so that top row and bottom row are neighbours
def keep_within_grid(i) : return  i - size if i >= size else  i + size if i < 0 else i

def live_neighbour_count(grid, cellNo) : return len(list(filter(lambda relPos: grid[keep_within_grid(cellNo + relPos)], relative_neighbour_positions)))

#def will_live(currentlyAlive, liveNeighbours) : return (liveNeighbours > 1 and liveNeighbours < 4) if currentlyAlive else liveNeighbours == 3

def will_live(alive, neighbours) : 
    return (alive and neighbours > 1 and neighbours < 4) \
            or (not alive and neighbours == 3) 

def next_cell_value(grid, c) : return will_live(grid[c], live_neighbour_count(grid, c));

def next_generation(grid) : return list(map(lambda n : next_cell_value(grid, n), range(0, size)))


