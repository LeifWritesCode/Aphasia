using System.Text.RegularExpressions;

namespace WordleSolver
{
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

        private string myLastGuess = string.Empty;
        private IEnumerable<string> myDictionary = Constants.Words;

        /// <summary>
        /// Finds all words containing common letters, and returns a word containing five of them.
        /// </summary>
        /// <returns>A valid word containing five common letters.</returns>
        private static string FirstGuess()
        {
            var commons = sCommonLetters;
            return Constants.Words
                .Where(answer => commons.Sum(character => answer.Contains(character) ? 1 : 0) == Constants.WordLength)
                .First();
        }

        /// <summary>
        /// Get the size of the candidate dictionary.
        /// </summary>
        /// <returns>The size of the candidate dictionary.</returns>
        public int RemainingCandidates()
        {
            return myDictionary.Count();
        }

        /// <summary>
        /// Given the last guess, the response, and the remaining pool of words, infers a high-probability next guess.
        /// </summary>
        /// <param name="response">The response from wordle, in GYW format.</param>
        /// <returns>The next guess.</returns>
        /// <exception cref="ArgumentNullException">If response is null.</exception>
        /// <exception cref="InvalidOperationException">If no guess possible.</exception>
        public string Guess(string response)
        {
            // if last guess is empty, we haven't guessed yet
            if (string.IsNullOrEmpty(myLastGuess))
            {
                myLastGuess = FirstGuess();
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
            var yellows = "";
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
                        yellows += '.';
                        break;

                    case 'G':
                        greens += letter;
                        yellows += '.';
                        break;

                    case 'Y':
                        greens += '.';
                        yellows += letter;
                        yellows_arr = yellows_arr.Append(letter).ToArray();
                        break;
                }
            }

            // to generate the new pool, we can start by including all words that match our green responses
            greens = $"^{greens}$";
            myDictionary = myDictionary
                .Where(ans => Regex.IsMatch(ans, greens));

            // lastly, if there were yellows, we include those which aren't a match but include a constituent character
            if (yellows_arr.Length > 0)
            {
                yellows = $"^{yellows}$";
                myDictionary = myDictionary
                    .Where(ans => !Regex.IsMatch(ans, yellows) && yellows_arr.All(chr => ans.Contains(chr)));
            }

            // we now need to reconcile the whites expression with the other expressions
            whites = string.Join(string.Empty, whites.Where(character => !(greens.Contains(character) || yellows_arr.Contains(character))));
            if (whites.Length > 0)
            {
                // remember to correct the whites expression - needs to be of form [abc]+
                // we then eliminate all words that match the whites expression
                whites = $"^[^{whites}]+$";
                myDictionary = myDictionary
                    .Where(ans => Regex.IsMatch(ans, whites));
            }

            // uncomment this line to see what my regular expressions were
            // Console.WriteLine("REGEX: MUST MATCH={0} MUST NOT MATCH={1} SHOULD NOT MATCH, BUT BE INCLUDED={2}", greens, whites, yellows);

            if (!myDictionary.Any())
            {
                throw new InvalidOperationException("ran out of candidates to pick from");
            }

            myLastGuess = myDictionary.First();
            return myLastGuess;
        }
    }
}
