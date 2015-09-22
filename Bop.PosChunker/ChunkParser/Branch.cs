using System.Collections.Generic;

namespace Bop.PosChunker.ChunkParser
{
    /// <summary>
    /// Represents a branch in a parse tree generated from a string of chunked text.
    /// </summary>
    public class Branch
    {
        /// <summary>
        /// The children of this Branch, which may represent either chunks or tagged words
        /// </summary>
        public List<Branch> Children { get; set; }
    }
}
