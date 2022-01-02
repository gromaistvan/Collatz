import random

txt: str = "💚" * random.randrange(3, 15)
print(f"{txt}|{type(txt)}|{id(txt)}|{len(txt)}|{txt[:3].upper()}")

print(type(1.2))
print(type(2 + 8j))
print(type(None))
print(type(True))
print(type(10 ** 100))
print(type((1, 'a'.upper())))
print([1, 2, 3])
print(tuple([1, 2, 3]))
print(ascii("💚"))

print(12, bin(12), hex(67))

#data = input("data: ")
#print(len(data))
