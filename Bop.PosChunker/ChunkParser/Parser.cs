using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bop.PosChunker.ChunkParser
{
    /// <summary>
    /// Transforms a string of POS chunked text into a tree structure
    /// </summary>
    public class Parser
    {
        // Regex for a top-level chunk, ignoring nested chunks.
        private readonly Regex elementRegex = new Regex(@"(\[(\w+?)\s+((?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!)))\])|((\w+)/(\w+))");

        /// <summary>
        /// Returns a tree structure from the supplied POS chunked text
        /// </summary>
        public Branch Parse(string chunkedText)
        {
            if (chunkedText == null) throw new ArgumentNullException(nameof(chunkedText));

            var rootBranches = GetBranches(chunkedText);
            if (rootBranches == null) return null;

            return new Branch
            {
                Children = rootBranches
            };
        }

        /// <summary>
        /// Returns a list of Branches, each possibly with child Branches, by recursing through the supplied text
        /// </summary>
        private List<Branch> GetBranches(string text)
        {
            List<Branch> branches = null;

            var elements = elementRegex.Matches(text)
                .OfType<Match>()
                .ToList();

            if (elements.Count > 0)
            {
                branches = new List<Branch>();

                foreach (var match in elements)
                {
                    Branch branch = null;

                    if (match.Value.StartsWith("[")) // Chunk
                    {
                        string chunkName = match.Groups[2].Value;
                        string innerText = match.Groups[3].Value;

                        branch = new ChunkBranch { ChunkName = chunkName };

                        // Recursively add child chunks
                        var children = GetBranches(innerText);
                        if (children != null)
                        {
                            branch.Children = children;
                        }
                    }
                    else // Tag
                    {
                        string word = match.Groups[5].Value;
                        string tag = match.Groups[6].Value;

                        branch = new TagBranch { Word = word, Tag = tag };
                    }

                    branches.Add(branch);
                }
            }

            return branches;
        }
    }
}
