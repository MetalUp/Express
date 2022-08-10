

size = 400 # Total number of cells in grid
w = 20 # Number of cells horizontally
grid1 = [False, True, False, True, False, False, True, True, True, False, False, True, True, False, True, True, False, False, True, True, True, True, True, True, False, False, True, False, True, True, True, False, True, True, True, True, False, True, False, True, False, False, True, True, False, False, False, False, True, False, False, False, True, False, False, False, True, False, False, False, True, False, False, True, False, False, True, False, False, True, True, False, False, False, True, True, False, False, True, True, False, True, True, True, True, False, False, False, False, False, False, False, False, True, True, False, True, False, False, True, True, False, False, False, False, True, False, False, False, False, False, False, False, False, False, False, False, False, False, False, True, False, True, False, False, False, True, False, False, True, True, False, True, False, True, True, False, False, False, False, False, True, False, False, True, False, True, False, True, False, True, False, True, False, False, True, False, True, False, True, False, False, True, False, False, True, True, False, True, False, False, True, False, True, False, True, True, False, False, False, False, True, False, False, True, False, False, False, False, True, False, False, False, True, False, True, False, True, False, False, False, False, False, True, True, False, True, True, False, False, False, False, False, True, False, False, True, True, True, False, False, False, True, False, True, False, False, True, True, False, True, False, True, False, True, False, False, True, False, True, False, True, False, True, False, False, False, True, False, False, False, True, False, True, False, True, False, False, True, False, False, False, False, False, False, False, False, False, False, True, True, True, False, False, True, False, True, False, False, True, False, True, False, True, False, False, False, False, False, True, False, True, False, False, False, False, True, True, False, False, True, True, False, True, True, False, True, True, False, True, False, False, True, False, True, False, False, True, False, True, True, False, False, False, False, False, True, True, False, False, False, True, True, True, False, False, False, True, True, False, True, True, True, False, False, False, True, False, True, False, False, False, False, False, False, False, False, False, False, True, False, False, False, False, True, True, False, True, False, False, True, True, False, False, True, True, True, False, False, False, False, False, False, False, True, False, False, False, False, False, False, False, False, False, True, False, False, False, True, False]


def neighbour_cells(c) : return [ c-21, c-20, c-19, c-1, c+1, c+19, c+20, c+21]

def adjusted_neighbour_cells(c): return list(map(lambda x :keep_within_bounds(x),neighbour_cells(c)))

# 'Wraps' the grid vertically so that top row and bottom row are neighbours
def keep_within_bounds(i) : return  i - 400 if i >= 400 else  i + 400 if i < 0 else i

def live_neighbours(grid, c) : return len(list(filter(lambda c: grid[c] is True, adjusted_neighbour_cells(c))))
 
#def will_live(currentlyAlive, liveNeighbours) : return (liveNeighbours > 1 and liveNeighbours < 4) if currentlyAlive else liveNeighbours == 3

def will_live(alive, neighbours) : 
    return (alive and neighbours > 1 and neighbours < 4) \
            or (not alive and neighbours == 3) 

def next_cell_value(grid, c) : return will_live(grid[c], live_neighbours(grid, c))

def next_generation(grid) : return list(map(lambda n : next_cell_value(grid, n), range(0, size)))


