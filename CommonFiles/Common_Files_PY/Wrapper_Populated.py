from math import *
from functools import reduce 

# StudentCode - this comment is needed error line number offset
#<StudentCode>

#<HiddenCode>

from typing import Any

def display(x: Any) -> str:
    if isinstance(x, str):
        return f"''{x}'"
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
    return str(args[1:len(args)-1])

def fail_message(function_name : str, expected : Any, actual : Any, args: Any) -> str:
    return f"xxxFailed calling {function_name}({arg_string(args)})  Expected: {display(expected)}  Actual: {display(actual)}xxx"

def qa_fail_message(wrong_answers : str) -> str:
    return f"xxxWrong or missing answer(s) to question number(s): {wrong_answers}.xxx"

#def as_list(answers: str) -> list[str]:
#    arr : list[str] = [''] * 20
#    for a in filter(lambda x: len(x) > 0, answers.split('\n')):
#        parts = a.split(')')
#        n = int(parts[0].strip())
#        arr[n] = parts[1].strip().upper()
#    return arr

def wrong_answers(student : str, answers : str) -> str :
    student_list = as_list(student)
    answer_list = as_list(answers)
    result = ''
    for n in range(len(answer_list)) :
        if answer_list[n] != None and student_list[n] != answer_list[n] :
            result += f"{n} "
    return result

from typing import TypeVar

T = TypeVar('T', int, float)

def set_item(list: list[T], i: int, value: T) : return list[0:i]+[value]+list[i+1:]

def insert_item(list: list[T], i: int, value: T) : return list[0:i]+[value]+list[i:]

def remove_item(list: list[T], i: int) : return list[0:i]+list[i+1:]



#<Tests>

print (display(("Hello",4)))