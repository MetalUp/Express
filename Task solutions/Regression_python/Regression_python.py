#def sum_x(l) : return sum(map(lambda x : x[1], l))
def sum_x(l) : return sum(x for x,y in l) 

def sum_y(l) : return sum(y for x,y in l) 

def sum_x_sq(l) : return sum(x*x for x,y in l) 

def sum_xy(l) : return sum(x*y for x,y in l) 

def calc_a( sum_x,  sum_x_sq, sum_y, sum_xy, n) : 
    return (sum_y * sum_x_sq - sum_x * sum_xy) / (n * sum_x_sq - sum_x)

def calc_b(sum_x, sum_x_sq, sum_y, sum_xy, n) : 
    return  (n * sum_xy - sum_x * sum_y) / (n * sum_x_sq - sum_x*sum_x)

def best_fit_from_summary_terms(sum_x, sum_x_sq, sum_y, sum_xy, n) : 
    return calc_a(sum_x, sum_x_sq, sum_y, sum_xy, n), calc_b(sum_x, sum_x_sq, sum_y, sum_xy, n)

#Returns the terms (a,b) for best fit line of form y = a + bx for a list of (x,y) points
def best_fit_from_points(l) : 
    return best_fit_from_summary_terms(sum_x(l), sum_x_sq(l), sum_y(l), sum_xy(l), len(l))
