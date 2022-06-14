# A solution finder for Wordle

This is a small C# application that can help you find the solution to a given days Wordle challenge. It will guess a word, and you simply respond with a five letter word made up of G, for green, Y, for yellow, and W, for white. It will then continue to guess until it gets the right answer. It usually finds the right solution after two guesses, but it ranges from two to five guesses.

Run me how you would any other C# application. Alternatively, use `program.cs` with `dotnet-script`.

### Benchmark

There is a bunchmark mode that can be accessed by executing `WordleSolver` with the `--bench` switch. Currently, this solver achieves an average of four guesses across all 2,314 challenges. However, the algorithm is naive in some areas and it does not manage to guess all days within the six guess limit. That's my next challenge!
