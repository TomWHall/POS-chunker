namespace Bop.PosChunker.ChunkParser
{
    /// <summary>
    /// Represents a chunk in a parse tree generated from a string of chunked text.
    /// </summary>
    public class ChunkBranch : Branch
    {
        /// <summary>
        /// The name of the chunk
        /// </summary>
        public string ChunkName { get; set; }
    }
}
