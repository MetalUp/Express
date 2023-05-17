from functools import reduce 
from math import *
from typing import Any, Tuple, TypeVar

# StudentCode - this comment is needed error line number offset
#<StudentCode>

#<HiddenCode>

#<Tests>

# Helpers
def display(x: Any) -> str:
    if isinstance(x, str):
        return f"'{x}'"
    elif isinstance(x, tuple):
        return str(x)
    elif isinstance(x, list):
        d = map(lambda y: display(y),x)
        return "\n[" + ', '.join(d)+"]"
    else:
        try:
            iter(x)
            return "Result is an Iterator. Convert to a list to display contents."
        except TypeError:
            return str(x)

def arg_string(arguments : Any) -> str:
    args = display(arguments)
    return str(args[2:len(args)-1])

def fail_message(function_name : str, expected : Any, actual : Any, args: Any) -> str:
    return f"xxxTest failed calling {function_name}({arg_string(args)}) Expected: {display(expected)} Actual: {display(actual)}xxx"

def EqualIfRounded(expected: float, actual: float) -> bool:
    places = len(str(expected)) - str(expected).index('.') - 1
    rounded = round(actual, places)
    return expected == rounded

T = TypeVar('T')

def set_item(list: list[T], i: int, value: T) -> list[T]: return list[0:i]+[value]+list[i+1:]

def insert_item(list: list[T], i: int, value: T) -> list[T]: return list[0:i]+[value]+list[i:]

def remove_item(list: list[T], i: int) -> list[T]: return list[0:i]+list[i+1:]


print(display("<Expression>"))