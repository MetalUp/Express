from functools import reduce

def display(x):
  if isinstance(x, str):
    return str(x)
  else:
    try:
         return str(list(iter(x)))
    except TypeError:
         return str(x)

def test_function(function_name, expected, actual, arguments):
    argString = display(arguments).strip("[]")
    if display(actual) != display(expected):
        print(f"Test Failed calling function {function_name}({argString})");
        print(f"Expected: {display(expected)}  Actual: {display(actual)}");
        raise TestException


def assert_true(function_name, actual, message) :
    if actual is not True :
        print(f"Test Failed calling function {function_name}")
        print(message)
        raise TestException


def all_tests_passed(function_name):
    print(f"All tests passed on function: {function_name}")

class TestException(Exception) : pass
