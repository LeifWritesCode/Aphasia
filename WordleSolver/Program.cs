namespace WordleSolver
{
    public class Program
    {
        private static int BenchmarkWithCustomStart(string[] starts)
        {
            // found by brute force
            var totalDays = 2314; //Math.Ceiling(Math.Abs((Constants.Epoch - DateTime.UtcNow).TotalDays));
            var total = 0;
            var frequencyOfGuesses = new Dictionary<int, int>();

            for (var i = 0; i < totalDays; i++)
            {
                var day = Constants.Epoch.AddDays(i);

                var guesses = Benchmark.Play(day, starts);
                if (guesses == -1)
                {
                    throw new Exception("failed to guess the world.");
                }
                frequencyOfGuesses[guesses] = frequencyOfGuesses.TryGetValue(guesses, out int freq) ? freq + 1 : 1;

                total += guesses;
                //Console.WriteLine("Finished day {0} in {1} guesses.", i + 1, guesses);
            }
            //var average = total / totalDays;
            //Console.WriteLine("Finished all days. Average number of guesses was {0}", average);
            //Console.WriteLine("Frequency table (descending.)");
            //Console.WriteLine(string.Join(Environment.NewLine, frequencyOfGuesses.OrderByDescending(kvp => kvp.Value)));
            //Console.WriteLine("Legal Results: {0}", frequencyOfGuesses.Sum(kvp => kvp.Key <= 6 ? kvp.Value : 0));

            return frequencyOfGuesses.Sum(kvp => kvp.Key <= 6 ? kvp.Value : 0);
        }

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

                    var guesses = Benchmark.Play(day, new string[] { "cigar", "thumb", "flown", "pesky" });
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
            else if (args.Any(word => word.Equals("--analyse")))
            {
                Console.WriteLine("Corpsus analysis mode.");
                var mustContain = new char[] { 'e', 't', 'a', 'i', 'o', 'n', 's', 'h', 'r', 'u' };
                for (var a = 0; a < Constants.Words.Count(); a++)
                    for (var b = 0; b < Constants.Words.Count(); b++)
                        for (var c = 0; c < Constants.Words.Count(); c++)
                            for (var d = 0; d < Constants.Words.Count(); d++)
                            {
                                var e = Constants.Words.ElementAt(a);
                                var f = Constants.Words.ElementAt(b);
                                var g = Constants.Words.ElementAt(c);
                                var h = Constants.Words.ElementAt(d);
                                var i = e + f + g + h;
                                if (i.Distinct().Count() == i.Length && mustContain.All(letter => i.Contains(letter)))
                                {
                                    Console.WriteLine("Perfect Pair! {0}, {1}, {2}, and {3}", e, f, g, h);
                                    // Console.WriteLine("New Pair! {0}, {1}, and {2}. Running benchmark.", d, e, f);
                                    //if (BenchmarkWithCustomStart(d, e, f) == 2314)
                                    //{
                                    //    //Console.WriteLine("Found a perfect pair!");
                                    //}
                                }
                            }
                Console.WriteLine("Finished.");
            }
            else
            {
                Usage();
                var dictionary = Constants.Words;
                var response = string.Empty;
                var solver = new Solver(new string[] { "cigar", "thumb", "flown", "pesky" });
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
