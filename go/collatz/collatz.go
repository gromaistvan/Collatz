package collatz

import (
	b "math/big"
)

var (
	zero  = b.NewInt(0)
	one   = b.NewInt(1)
	two   = b.NewInt(2)
	three = b.NewInt(3)
	four  = b.NewInt(4)
)

// Sequence is a Collatz sequence (https://en.wikipedia.org/wiki/Collatz_conjecture)
func Sequence(n *b.Int) chan *b.Int {
	out := make(chan *b.Int)
	if n.Sign() != 1 {
		close(out)
		return out
	}
	go func() {
		q := new(b.Int).Set(n)
		m := new(b.Int)
		for {
			res := new(b.Int)
			q.DivMod(q, two, m)
			for m.Cmp(zero) == 0 {
				res.Set(q)
				q.DivMod(q, two, m)
			}
			out <- res
			if res.Cmp(one) == 0 {
				break
			}
			q.Mul(res, three)
			q.Add(q, one)
		}
		close(out)
	}()
	return out
}
