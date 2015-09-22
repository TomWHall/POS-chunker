using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bop.PosChunker
{
    /// <summary>
    /// Performs chunking on POS-tagged text
    /// </summary>
    public class Chunker
    {
        // String format for a recursive Regex pattern to match the chunk with name {0}
        private const string ChunkTemplate = @"(\[{0}\s+(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\]){1}";

        private readonly Regex tagAbbreviationRegex = new Regex(@"{(.+?)/(.+?)}", RegexOptions.Compiled);
        private readonly Regex multiTagAbbreviationRegex = new Regex(@"{(.+?)/(.+?)}\+", RegexOptions.Compiled);

        private readonly Regex chunkAbbreviationRegex = new Regex(@"{(\w+?)}(\*)?", RegexOptions.Compiled);

        private readonly Regex spaceAbbreviationRegex = new Regex(@"\s+", RegexOptions.Compiled);

        /// <summary>
        /// Returns the result of applying the supplied chunking rules to the supplied text
        /// </summary>
        public string Chunk(string taggedText, List<Rule> rules)
        {
            if (taggedText == null) throw new ArgumentNullException(nameof(taggedText));
            if (rules == null) throw new ArgumentNullException(nameof(rules));

            string result = taggedText;

            foreach (var rule in rules)
            {
                result = ApplyRule(result, rule);
            }

            return result;
        }

        private string ApplyRule(string text, Rule rule)
        {
            string pattern = rule.Pattern;
            pattern = spaceAbbreviationRegex.Replace(pattern, @"\s?");
            pattern = multiTagAbbreviationRegex.Replace(pattern, MultiTagAbbreviationMatchEvaluator);
            pattern = tagAbbreviationRegex.Replace(pattern, TagAbbreviationMatchEvaluator);
            pattern = chunkAbbreviationRegex.Replace(pattern, string.Format(ChunkTemplate, "$1", "$2"));

            string result = new Regex(pattern, rule.RegexOptions).Replace(text, $"[{rule.ChunkName} {"$0"}]");
            result = result.Replace(" ]", "] ").Replace("][", "] [");

            return result;
        }

        private string TagAbbreviationMatchEvaluator(Match match)
        {
            return match.Value
                .Replace("{*/", @"{\S+/")
                .Replace("/*}", @"/\S+}")
                .Replace("{", string.Empty)
                .Replace("}", string.Empty);
        }

        private string MultiTagAbbreviationMatchEvaluator(Match match)
        {
            string result = TagAbbreviationMatchEvaluator(match);

            result = $@"({result}\s?)+";

            return result;
        }
    }
}
