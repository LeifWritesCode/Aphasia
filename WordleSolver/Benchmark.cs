namespace WordleSolver
{
    /// <summary>
    /// Automatic solver for arbitrary days.
    /// </summary>
    internal static class Benchmark
    {
        public static int Play(DateTime gameDay)
        {
            var diff = Constants.Epoch - gameDay;
            var index = (int)Math.Ceiling(Math.Abs(diff.TotalMilliseconds) / Constants.ADayInMs) + 1;
            var theAnswer = Constants.Words.ElementAt(index);
            Console.WriteLine("The answer for this day is: {0}", theAnswer);

            var guesses = 0;
            var response = string.Empty;
            var solver = new Solver();
            while (true)
            {
                guesses++;

                string? theGuess;
                try
                {
                    theGuess = solver.Guess(response);
                }
                catch (InvalidOperationException)
                {
                    // failed
                    return -1;
                }

                Console.WriteLine("Solver guessed: {0}", theGuess);
                // if we got it right, return the total guess count.
                if (theGuess.Equals(theAnswer))
                {
                    Console.WriteLine("That was the right answer!");
                    return guesses;
                }

                // otherwise, construct the response value
                response = string.Empty;
                for (var i = 0; i < theGuess.Length; i++)
                {
                    var a = theGuess[i];
                    var b = theAnswer[i];

                    if (a.Equals(b))
                    {
                        response += "G";
                    }
                    else if (!a.Equals(b) && theAnswer.Contains(a))
                    {
                        response += "Y";
                    }
                    else
                    {
                        response += "W";
                    }
                }
                Console.WriteLine("The response is: {0}", response);
            }
        }
    }
}
