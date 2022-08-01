from functools import reduce

def display(x):
  if isinstance(x, str):
    return x
  else:
    try:
         return list(iter(x))
    except TypeError:
         return x

def test_function(function_name, expected, actual, arguments):
    argString = reduce(lambda s,a :  s + display(a) + ", ", arguments, "" ).rstrip(' ,')
    if display(actual)  is not display(expected):
        print(f"Test Failed calling function {function_name}");
        if arguments.Length > 0: print(f" with arguments: {argString}");
        print(f"Expected: {display(expected)}  Actual: {display(actual)}");
        raise TestException


def assert_true(function_name, actual, message) :
    if actual is not True :
        print(f"Test Failed calling function {function_name}")
        print(message)
        raise TestException


def all_tests_passed(function_name):
    print(f"All tests passed on function: {function_name}")

class TestException(Exception) :
    '''this is a subclass'''