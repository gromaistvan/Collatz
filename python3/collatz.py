from typing import Dict, Iterable
from colorama import Fore, init
from matplotlib import pyplot

init(autoreset=True)

_collatz: Dict[int, Iterable[int]] = {}


def collatz(n: int) -> Iterable[int]:
    "Collatz conjecture (https://en.wikipedia.org/wiki/Collatz_conjecture)."
    while True:
        while n % 2 == 0:
            n //= 2
        if n in _collatz:
            yield from _collatz[n]
            return
        yield n
        if n > 1:
            n += n + n + 1
        else:
            return


def pprint(lst = []):
    for n in lst:
        print(bin(n), Fore.RED + ' ➡ ', n)


def save(lst = []):
    with open(f"{lst[0]}.txt", 'w', encoding='utf-8') as out:
        for n in lst:
            out.write(f"{n:050b} ➡ {n}\n")


def plot(lst = []):       
    pyplot.bar(list(range(0, len(lst))), lst, color=['red'])
    pyplot.title(f"Collatz for {lst[0]}")
    pyplot.show()
    pyplot.savefig(f"{lst[n]}.png", bbox_inches='tight')    


for n in range(1, 1000, 2):
    _collatz[n] = list(collatz(n))
    
n = int(input("n? "))
pprint(_collatz[n])    
save(_collatz[n])
plot(_collatz[n])        
