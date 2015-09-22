namespace Bop.PosChunker.ChunkParser
{
    /// <summary>
    /// Represents a tagged word in a parse tree generated from a string of chunked text.
    /// </summary>
    public class TagBranch : Branch
    {
        /// <summary>
        /// The POS tag
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// The word
        /// </summary>
        public string Word { get; set; }
    }
}
