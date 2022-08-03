from functools import reduce

def display(x):
  if isinstance(x, str):
    return str(x)
  else:
    try:
         return str(list(iter(x)))
    except TypeError:
         return str(x)

fail = "Test failed calling "

def arg_string(arguments) :
    args = display(arguments)
    return args[1:len(args)-1]

def test_function(function_name, expected, actual, args):
    if display(actual) != display(expected):
        print(fail + f"{function_name}({arg_string(args)}) Expected: {display(expected)}  Actual: {display(actual)}");
        raise TestFailure

def assert_true(function_name, args, actual, message) :
    if actual is not True :
        print(fail + f"{function_name}({arg_string(args)}) {message}")
        raise TestFailure

all_passed = "All tests passed"

def all_tests_passed():
    print(all_passed)

class TestFailure(Exception) : pass
