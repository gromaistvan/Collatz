package main

import (
	"bufio"
	"fmt"
	"math/big"
	"os"
	"research/collatz"
)

// ReadNumber reads a number from input.
func ReadNumber(n *big.Int) {
	reader := bufio.NewReader(os.Stdin)
	for {
		fmt.Print("Enter number: ")
		text, _ := reader.ReadString('\n')
		if text == "" {
			continue
		}
		_, err := fmt.Sscan(text, n)
		if err != nil {
			fmt.Println(err)
		}
	}
}

// CheckArgs reads a number from the first argument.
func CheckArgs(n *big.Int) error {
	_, err := fmt.Sscan(os.Args[1], n)
	return err
}

func main() {
	n := new(big.Int)
	if CheckArgs(n) != nil {
		ReadNumber(n)
	}
	for x := range collatz.Sequence(n) {
		fmt.Printf("(%15d) %50b\n", x, x)
	}
}
