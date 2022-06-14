using System.Text.RegularExpressions;

namespace WordleSolver
{
    /// <summary>
    /// Defines strategies for choosing the first word.
    /// </summary>
    internal enum FirstGuessStrategy
    {
        /// <summary>
        /// Picks a word containing as many vowels as possible.
        /// </summary>
        /// <remarks>
        /// Slightly better performance than <see cref="PrioritiseCommonLetters"/>.
        /// </remarks>
        PrioritiseVowels,

        /// <summary>
        /// Picks a word containing as many common letters as possible.
        /// </summary>
        PrioritiseCommonLetters,

        /// <summary>
        /// Computes frequency of all letters in the corpus, then computes a score for each word.
        /// Strategy is to use the highest scoring word.
        /// </summary>
        SimpleFrequencyScore,
    }

    internal class Solver
    {

        /// <summary>
        /// The most common letters by frequency (absolute.)
        /// </summary>
        /// <remarks>
        /// See https://www3.nd.edu/~busiforc/handouts/cryptography/letterfrequencies.html
        /// </remarks>
        private static readonly IEnumerable<char> sCommonLetters = new List<char>()
        {
            'e', 't', 'a', 'i', 'o', 'n', 's', 'h', 'r'
        };

        /// <summary>
        /// Vowels.
        /// </summary>
        private static readonly IEnumerable<char> sVowels = new List<char>()
        {
            'a', 'e', 'i', 'o', 'u'
        };

        private string myLastGuess = string.Empty;
        private IEnumerable<KeyValuePair<string, int>> myDictionary;

        /// <summary>
        /// Oxford Dictionary (9th edition, 1995) frequency table.
        /// </summary>
        /// <remarks>
        /// These are the relative frequencies, where Q is the least frequent.
        /// </remarks>
        private static readonly IDictionary<char, double> frequencies = new Dictionary<char, double>()
        {
            { 'e', 56.88 },
            { 'a', 43.31 },
            { 'r', 38.64 },
            { 'i', 38.45 },
            { 'o', 36.51 },
            { 't', 35.43 },
            { 'n', 33.92 },
            { 's', 29.23 },
            { 'l', 27.98 },
            { 'c', 21.13 },
            { 'u', 18.51 },
            { 'd', 17.25 },
            { 'p', 16.04 },
            { 'm', 15.36 },
            { 'h', 15.31 },
            { 'g', 12.59 },
            { 'b', 10.56 },
            { 'f', 9.24 },
            { 'y', 9.06 },
            { 'w', 6.57 },
            { 'k', 5.61 },
            { 'v', 5.13 },
            { 'x', 1.48 },
            { 'z', 1.39 },
            { 'j', 1.01 },
            { 'q', 1.0 },
        };

        /// <summary>
        /// character frequencies by position
        /// </summary>
        private readonly IDictionary<char, int[]> positionalFrequencies =
            new Dictionary<char, int[]>();

        public Solver()
        {
            // compute positional frequencies
            for (var i = 0; i < Constants.WordLength; i++)
            {
                foreach (var word in Constants.Words)
                {
                    var character = word[i];
                    if (!positionalFrequencies.ContainsKey(character))
                    {
                        positionalFrequencies[character] = new int[] { 0, 0, 0, 0, 0 };
                    }
                    positionalFrequencies[character][i]++;
                }
            }

            // sort using the positional frequencies
            var temp = new Dictionary<string, int>();
            foreach (var word in Constants.Words)
            {
                var sum = 0;
                for (var i = 0; i < Constants.WordLength; i++)
                {
                    sum += (positionalFrequencies[word[i]][i] / word.Count(character => character == word[i])) + (int)frequencies[word[i]];
                }
                temp[word] = sum;
            }

            // then order and assign
            myDictionary = temp.OrderByDescending(kvp => kvp.Value);
            firsts = new string[] { "jaunt", "sower", "flimp" };
        }

        public Solver(string[] starts)
            : this()
        {
            firsts = starts;
        }

        string[] firsts;

        /// <summary>
        /// Computes first word using a simple frequency scoring algorithm.
        /// </summary>
        /// <returns>A valid word.</returns>
        private string SimpleFrequencyScore()
        {
            // words are now pre-scored and pre-sorted.
            return myDictionary.First().Key;
        }

        /// <summary>
        /// Finds the first word according to the given search strategy.
        /// </summary>
        /// <returns>A valid word.</returns>
        private string FirstGuess(FirstGuessStrategy strategy = FirstGuessStrategy.PrioritiseCommonLetters)
        {
            return strategy switch
            {
                FirstGuessStrategy.PrioritiseVowels => Constants.Words
                    .OrderByDescending(answer => sVowels.Count(character => answer.Contains(character)))
                    .First(),
                FirstGuessStrategy.PrioritiseCommonLetters => Constants.Words
                    .Where(answer => sCommonLetters.Count(character => answer.Contains(character)) == Constants.WordLength)
                    .First(),
                FirstGuessStrategy.SimpleFrequencyScore => SimpleFrequencyScore(),
                _ => throw new ArgumentOutOfRangeException(nameof(strategy)),
            };
        }

        /// <summary>
        /// Get the size of the candidate dictionary.
        /// </summary>
        /// <returns>The size of the candidate dictionary.</returns>
        public int RemainingCandidates()
        {
            return myDictionary.Count();
        }

        private int guessesSoFar = 0;

        /// <summary>
        /// Given the last guess, the response, and the remaining pool of words, infers a high-probability next guess.
        /// </summary>
        /// <param name="response">The response from wordle, in GYW format.</param>
        /// <returns>The next guess.</returns>
        /// <exception cref="ArgumentNullException">If response is null.</exception>
        /// <exception cref="InvalidOperationException">If no guess possible.</exception>
        public string Guess(string response)
        {
            if (string.IsNullOrEmpty(myLastGuess))
            {
                myLastGuess = firsts[guessesSoFar++];
                return myLastGuess;
            }

            // needs a response if we're allowing it
            if (string.IsNullOrEmpty(response))
            {
                throw new ArgumentNullException(nameof(response));
            }

            // validate the response and throw if it's bad
            response = response.ToUpper();
            if (response.Where(character => Constants.ValidResponseCharacters.Contains(character)).Count() != Constants.WordLength)
            {
                throw new ArgumentException("response is invalid");
            }

            // a regular expression that all candidates should match
            // represents the green responses
            string greens = "";

            // a regular expression that all candidates should NOT match
            // represents the white responses
            string whites = "";

            // a regular expression that all candidates should NOT match
            // candidates that don't match are included if they contain the characters
            // in yellows_arr
            // represents the yellow responses
            var yellows_arr = Array.Empty<char>();

            // just parse the input sequentially
            // update each regular expression with either a wildcard or the letter
            for (var i = 0; i < myLastGuess.Length; i++)
            {
                var letter = myLastGuess[i];
                var result = response[i];

                switch (result)
                {
                    case 'W':
                        whites += letter;
                        greens += '.';
                        yellows_arr = yellows_arr.Append('#').ToArray();
                        break;

                    case 'G':
                        greens += letter;
                        yellows_arr = yellows_arr.Append('#').ToArray();
                        break;

                    case 'Y':
                        greens += '.';
                        yellows_arr = yellows_arr.Append(letter).ToArray();
                        break;
                }
            }

            // to generate the new pool, we can start by including all words that match our green responses
            greens = $"^{greens}$";
            myDictionary = myDictionary
                .Where(ans => Regex.IsMatch(ans.Key, greens));

            // lastly, if there were yellows, we include those which aren't a match but include a constituent character
            if (yellows_arr.Length > 0)
            {
                for (var i = 0; i < yellows_arr.Length - 1; i++)
                {
                    var c = yellows_arr[i];
                    if (c == '#')
                        continue;
                    myDictionary = myDictionary.Where(ans => ans.Key[i] != c && ans.Key.Contains(c));
                }
            }

            // we now need to reconcile the whites expression with the other expressions
            whites = string.Join(string.Empty, whites.Where(character => !(greens.Contains(character) || yellows_arr.Contains(character))));
            if (whites.Length > 0)
            {
                // remember to correct the whites expression - needs to be of form [abc]+
                // we then eliminate all words that match the whites expression
                whites = $"^[^{whites}]+$";
                myDictionary = myDictionary
                    .Where(ans => Regex.IsMatch(ans.Key, whites));
            }

            // uncomment this line to see what my regular expressions were
            // Console.WriteLine("REGEX: MUST MATCH={0} MUST NOT MATCH={1} SHOULD NOT MATCH, BUT BE INCLUDED={2}", greens, whites, yellows);

            if (!myDictionary.Any())
            {
                throw new InvalidOperationException("ran out of candidates to pick from");
            }

            // return firsts for first three guesses, else whatevers in the dictionary
            myLastGuess = guessesSoFar < firsts.Length ? firsts[guessesSoFar++] : myDictionary.First().Key;
            return myLastGuess;
        }
    }
}
