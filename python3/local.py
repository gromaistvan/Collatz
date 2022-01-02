def fizz_buzz(n: int) -> str:    
    txt  = "Fizz" if n % 3 == 0 else "" 
    txt += "Buzz" if n % 5 == 0 else ""
    return txt if txt else str(n)
    

print(fizz_buzz(300))
print(fizz_buzz(100))
print(fizz_buzz(7))
print(fizz_buzz(3))
print(fizz_buzz(2))
