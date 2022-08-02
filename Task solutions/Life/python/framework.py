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

def arg_string(arguments) : return display(arguments).strip("[]")

def test_function(function_name, expected, actual, args):
    if display(actual) != display(expected):
        print(fail + f"{function_name}({arg_string(args)}) Expected: {display(expected)}  Actual: {display(actual)}");
        raise TestException

def assert_true(function_name, args, actual, message) :
    if actual is not True :
        print(fail + f"{function_name}({arg_string(args)}) {message}")
        raise TestException


def all_tests_passed(function_name):
    print(f"All tests passed on function: {function_name}")

class TestException(Exception) : pass
