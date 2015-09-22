using System.Text.RegularExpressions;

namespace Bop.PosChunker
{
    /// <summary>
    /// A Regex-based rule used to create a chunk within a string of POS-tagged text.
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// The Regex pattern to use to define a chunk.
        /// Some shortcut patterns are allowed which are transformed into Regexes before application:
        /// To capture an existing chunk: {CHUNKNAME}
        /// To optionally capture an existing chunk: {CHUNKNAME}*
        /// To capture a tagged word regardless of the tag: {word/*}
        /// To capture a tagged word regardless of the word: {*/TAG}
        /// To capture multiple consecutive tagged words: {*/(TAG1|TAG2|TAG3)}+
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The name to be given to the output chunk
        /// </summary>
        public string ChunkName { get; set; }

        /// <summary>
        /// The RegexOptions to apply
        /// </summary>
        public RegexOptions RegexOptions { get; set; } = RegexOptions.None;
    }
}
