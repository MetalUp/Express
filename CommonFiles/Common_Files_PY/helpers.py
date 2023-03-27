from functools import reduce
from multiprocessing.connection import answer_challenge

def display(x: object) -> str:
  if isinstance(x, str):
    return str(x)
  else:
    try:
         return str(list(iter(x)))
    except TypeError:
         return str(x)

def arg_string(arguments: str) :
    args = display(arguments)
    return args[1:len(args)-1]

def fail_message(function_name, expected, actual, args):
    return f"xxxFailed calling {function_name}({arg_string(args)})  Expected: {display(expected)}  Actual: {display(actual)}xxx"

def extract_int(inp: str) : 
    s = ""
    for c in inp :
        if c.isdigit() :
            s += c
    return int(s)

def as_array(answers)  :
    d = [""]*20
    for a in  answers.split('\n') :
        if len(a) > 0 :
            split = a.split(')')
            n = extract_int(split[0].strip())
            d[n] = split[1].strip().upper()
    return d

#Returns list of the question numbers to which the students answered were incorrect e.g. "1 3 4 9 "
def wrong_answers(student: str, answers: str) -> str:
    answers_arr = as_array(answers)
    student_arr = as_array(student)
    result = ""
    for n in range(len(answers_arr)) :
        if (answers_arr[n] != "") & (student_arr[n] != answers_arr[n]) :
            result += (f"{n} ")
    return result