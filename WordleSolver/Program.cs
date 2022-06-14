namespace WordleSolver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            if (args.Any(word => word.Equals("--bench")))
            {
                // found by brute force
                var totalDays = 2314; //Math.Ceiling(Math.Abs((Constants.Epoch - DateTime.UtcNow).TotalDays));
                var total = 0;
                var frequencyOfGuesses = new Dictionary<int, int>();

                for (var i = 0; i < totalDays; i++)
                {
                    var day = Constants.Epoch.AddDays(i);

                    var guesses = Benchmark.Play(day);
                    if (guesses == -1)
                    {
                        throw new Exception("failed to guess the world.");
                    }
                    frequencyOfGuesses[guesses] = frequencyOfGuesses.TryGetValue(guesses, out int freq) ? freq + 1 : 1;

                    total += guesses;
                    Console.WriteLine("Finished day {0} in {1} guesses.", i + 1, guesses);
                }
                var average = total / totalDays;
                Console.WriteLine("Finished all days. Average number of guesses was {0}", average);
                Console.WriteLine("Frequency table (descending.)");
                Console.WriteLine(string.Join(Environment.NewLine, frequencyOfGuesses.OrderByDescending(kvp => kvp.Value)));
                Console.WriteLine("Legal Results: {0}", frequencyOfGuesses.Sum(kvp => kvp.Key <= 6 ? kvp.Value : 0));
            }
            else
            {
                Usage();
                var dictionary = Constants.Words;
                var response = string.Empty;
                var solver = new Solver();
                while (true)
                {
                    string? nextGuess;
                    nextGuess = solver.Guess(response);

                    Console.WriteLine("My guess, from {1} candidates, is: {0}", nextGuess.ToUpper(), solver.RemainingCandidates()); Console.Write("What was the response? ");

                    response = Console.ReadLine()?.ToUpper();
                    while (string.IsNullOrEmpty(response) ||
                           response.Where(character => Constants.ValidResponseCharacters.Contains(character)).Count() != Constants.WordLength)
                    {
                        Console.WriteLine("That doesn't look right. The response should be {0} characters, consisting of G, Y and W.", Constants.WordLength);
                        Console.Write("What was the response? ");
                        response = Console.ReadLine();
                    }

                    if (response == "GGGGG")
                    {
                        Console.WriteLine("Nice, we did it!");
                        Console.WriteLine("That's all from me. See you next time.");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Prints instructions to console.
        /// </summary>
        private static void Usage()
        {
            Console.WriteLine("Hello, I'm a Wordle solver. I'll guess a word, and then you tell me the response.");
            Console.Write("For example, if I say WORDS and the response is ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("W");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("OR");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("DS");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("You would tell me YWWGG");
            Console.WriteLine("Let's begin!");
        }

    }
}
