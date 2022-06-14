# A solution finder for Wordle

This is a small C# application that can help you find the solution to a given days Wordle challenge. It will guess a word, and you simply respond with a five letter word made up of G, for green, Y, for yellow, and W, for white. It will then continue to guess until it gets the right answer. It usually finds the right solution after two guesses, but it ranges from two to five guesses.

Run me how you would any other C# application. Alternatively, use `program.cs` with `dotnet-script`.

### Benchmark

There is a bunchmark mode that can be accessed by executing `WordleSolver` with the `--bench` switch. Currently, this solver achieves an average of four guesses across all 2,314 challenges. However, the algorithm is naive in some areas and it does not manage to guess all days within the six guess limit. That's my next challenge!

There are current three first word selection strategies, performing as below.

| Strategy | Successes within six tries (out of 2314) | Method |
| -------- | ---------------------------------------- | ------ |
| Simple Frequency Score | 2253 | Assigns a frequency score to letters in the corpus, then scores the corpus according to those. Divides the character score by number of occurrences. Uses the highest scoring word. |
| Prioritise Common Characters | 2226 | Picks a word containing as many commonly used characters (in all English) as possible. |
| Prioritise Vowels | 2207 | Picks a word containing as many vowels as possible. |
