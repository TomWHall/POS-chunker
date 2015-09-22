using System;
using Bop.PosChunker.ChunkParser;
using NUnit.Framework;

namespace Bop.PosChunker.Tests
{
    /// <summary>
    /// Tests for the Parser class
    /// </summary>
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Parse_GivenNullChunkedText_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Parser().Parse(null));
        }

        [Test]
        public void Parse_GivenEmptyStringChunkedText_ReturnsNull()
        {
            string chunkedText = string.Empty;
            var branch = new Parser().Parse(chunkedText);
            Assert.IsNull(branch);
        }

        [Test]
        public void Parse_GivenWhiteSpaceChunkedText_ReturnsNull()
        {
            string chunkedText = "   ";
            var branch = new Parser().Parse(chunkedText);
            Assert.IsNull(branch);
        }

        [Test]
        public void Parse_GivenNestedChunks_ReturnsNestedChunkBranch()
        {
            string chunkedText = "[DC [NP The/DT professor/NN] [VP walked/VBD [NP the/DT dog/NN]]]";
            var rootBranch = new Parser().Parse(chunkedText);

            Assert.IsNotNull(rootBranch);
            Assert.IsNotNull(rootBranch.Children);
            Assert.AreEqual(1, rootBranch.Children.Count);

            var dcChunkBranch = rootBranch.Children[0] as ChunkBranch;
            Assert.IsNotNull(dcChunkBranch);
            Assert.AreEqual("DC", dcChunkBranch.ChunkName);
            Assert.IsNotNull(dcChunkBranch.Children);
            Assert.AreEqual(2, dcChunkBranch.Children.Count);

            var professorNpChunkBranch = dcChunkBranch.Children[0] as ChunkBranch;
            Assert.IsNotNull(professorNpChunkBranch);
            Assert.AreEqual("NP", professorNpChunkBranch.ChunkName);
            Assert.IsNotNull(professorNpChunkBranch.Children);
            Assert.AreEqual(2, professorNpChunkBranch.Children.Count);

            var professorTheTagBranch = professorNpChunkBranch.Children[0] as TagBranch;
            Assert.IsNotNull(professorTheTagBranch);
            Assert.AreEqual("The", professorTheTagBranch.Word);
            Assert.AreEqual("DT", professorTheTagBranch.Tag);
            Assert.IsNull(professorTheTagBranch.Children);

            var professorTagBranch = professorNpChunkBranch.Children[1] as TagBranch;
            Assert.IsNotNull(professorTagBranch);
            Assert.AreEqual("professor", professorTagBranch.Word);
            Assert.AreEqual("NN", professorTagBranch.Tag);
            Assert.IsNull(professorTagBranch.Children);

            var vpChunkBranch = dcChunkBranch.Children[1] as ChunkBranch;
            Assert.IsNotNull(vpChunkBranch);
            Assert.AreEqual("VP", vpChunkBranch.ChunkName);
            Assert.IsNotNull(vpChunkBranch.Children);
            Assert.AreEqual(2, vpChunkBranch.Children.Count);

            var walkedTagBranch = vpChunkBranch.Children[0] as TagBranch;
            Assert.IsNotNull(walkedTagBranch);
            Assert.AreEqual("walked", walkedTagBranch.Word);
            Assert.AreEqual("VBD", walkedTagBranch.Tag);
            Assert.IsNull(walkedTagBranch.Children);

            var dogNpChunkBranch = vpChunkBranch.Children[1] as ChunkBranch;
            Assert.IsNotNull(dogNpChunkBranch);
            Assert.AreEqual("NP", dogNpChunkBranch.ChunkName);
            Assert.IsNotNull(dogNpChunkBranch.Children);
            Assert.AreEqual(2, dogNpChunkBranch.Children.Count);

            var dogTheTagBranch = dogNpChunkBranch.Children[0] as TagBranch;
            Assert.IsNotNull(dogTheTagBranch);
            Assert.AreEqual("the", dogTheTagBranch.Word);
            Assert.AreEqual("DT", dogTheTagBranch.Tag);
            Assert.IsNull(dogTheTagBranch.Children);

            var dogTagBranch = dogNpChunkBranch.Children[1] as TagBranch;
            Assert.IsNotNull(dogTagBranch);
            Assert.AreEqual("dog", dogTagBranch.Word);
            Assert.AreEqual("NN", dogTagBranch.Tag);
            Assert.IsNull(dogTagBranch.Children);
        }

        [Test]
        public void Parse_GivenMultipleTopLevelChunks_ReturnsMultipleTopLevelChildren()
        {
            string chunkedText = "[DC [NP The/DT professor/NN] [VP walked/VBD [NP the/DT dog/NN]]] ./. [DC [NP The/DT dog/NN] [VP chased/VBD [NP a/DT stick/NN]]] ./.";
            var rootBranch = new Parser().Parse(chunkedText);

            Assert.IsNotNull(rootBranch);
            Assert.IsNotNull(rootBranch.Children);
            Assert.AreEqual(2, rootBranch.Children.Count);

            var professorDcChunkBranch = rootBranch.Children[0] as ChunkBranch;
            Assert.IsNotNull(professorDcChunkBranch);
            Assert.AreEqual("DC", professorDcChunkBranch.ChunkName);

            var dogDcChunkBranch = rootBranch.Children[1] as ChunkBranch;
            Assert.IsNotNull(dogDcChunkBranch);
            Assert.AreEqual("DC", dogDcChunkBranch.ChunkName);
        }
    }
}
