using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Bop.PosChunker.Tests
{
    /// <summary>
    /// Tests for the Chunker class
    /// </summary>
    [TestFixture]
    public class ChunkerTests
    {
        [Test]
        public void Chunk_GivenNullTaggedText_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Chunker().Chunk(null, GetChunkRules()));
        }

        [Test]
        public void Chunk_GivenNullRules_ThrowsArgumentNullException()
        {
            string taggedText = "The/DT professor/NN walked/VBD the/DT dog/NN";
            Assert.Throws<ArgumentNullException>(() => new Chunker().Chunk(taggedText, null));
        }

        [Test]
        public void Chunk_GivenEmptyStringTaggedText_ReturnsTaggedText()
        {
            string taggedText = string.Empty;
            string chunkedText = new Chunker().Chunk(taggedText, GetChunkRules());
            Assert.AreEqual(taggedText, chunkedText);
        }

        [Test]
        public void Chunk_GivenWhiteSpaceTaggedText_ReturnsTaggedText()
        {
            string taggedText = "   ";
            string chunkedText = new Chunker().Chunk(taggedText, GetChunkRules());
            Assert.AreEqual(taggedText, chunkedText);
        }

        [Test]
        public void Chunk_GivenNestedChunkRules_ReturnsNestedChunks()
        {
            string taggedText = "The/DT professor/NN walked/VBD the/DT dog/NN in/IN the/DT park/NN ./. The/DT dog/NN chased/VBD a/DT big/JJ stick/NN ./.";
            string chunkedText = new Chunker().Chunk(taggedText, GetChunkRules());
            Assert.AreEqual("[DC [NP The/DT professor/NN] [VP walked/VBD [NP the/DT dog/NN] [PP in/IN [NP the/DT park/NN]]]] ./. [DC [NP The/DT dog/NN] [VP chased/VBD [NP a/DT big/JJ stick/NN]]] ./.", chunkedText);
        }

        [Test]
        public void Chunk_GivenRuleToMatchWordOnly_ReturnsWordChunksForAnyTags()
        {
            string taggedText = "I/PRP thought/VBD a/DT strange/JJ thought/NN";

            var rules = new List<Rule>
            {
                new Rule { ChunkName = "TH", Pattern = @"{thought/*}" }
            };

            string chunkedText = new Chunker().Chunk(taggedText, rules);
            Assert.AreEqual("I/PRP [TH thought/VBD] a/DT strange/JJ [TH thought/NN]", chunkedText);
        }

        [Test]
        public void Chunk_GivenRuleToMatchTagOnly_ReturnsTagChunksForAnyWords()
        {
            string taggedText = "The/DT good/JJ ,/, the/DT bad/JJ and/CC the/DT ugly/JJ";

            var rules = new List<Rule>
            {
                new Rule { ChunkName = "ADJ", Pattern = @"{*/JJ}" }
            };

            string chunkedText = new Chunker().Chunk(taggedText, rules);
            Assert.AreEqual("The/DT [ADJ good/JJ] ,/, the/DT [ADJ bad/JJ] and/CC the/DT [ADJ ugly/JJ]", chunkedText);
        }

        [Test]
        public void Chunk_GivenRuleWithConsecutiveTags_CapturesConsecutiveTags()
        {
            string taggedText = "The/DT old/JJ friendly/JJ science/NN professor/NN";

            var rules = new List<Rule>
            {
                new Rule { ChunkName = "NP", Pattern = @"{*/(DT|JJ|NNPS|NNP|NNS|NN|PRP|CD)}+" }
            };

            string chunkedText = new Chunker().Chunk(taggedText, rules);
            Assert.AreEqual("[NP The/DT old/JJ friendly/JJ science/NN professor/NN]", chunkedText);
        }

        [Test]
        public void Chunk_GivenRuleWithRegexOptions_AppliesRegexOptions()
        {
            string taggedText = "Faster/JJR and/CC faster/JJR he/PRP went/VBD";

            var rules = new List<Rule>
            {
                new Rule { ChunkName = "FS", Pattern = @"{faster/JJR}", RegexOptions = RegexOptions.IgnoreCase }
            };

            string chunkedText = new Chunker().Chunk(taggedText, rules);
            Assert.AreEqual("[FS Faster/JJR] and/CC [FS faster/JJR] he/PRP went/VBD", chunkedText);
        }

        [Test]
        public void Chunk_GivenRuleWithOptionalChunk_CapturesWithAndWithoutOptionalChunk()
        {
            string taggedText = "[A Arnold] [B barrel] [B Bill] [A apple] [A Adam] [A Africa] [B Bob]";

            var rules = new List<Rule>
            {
                new Rule { ChunkName = "ABB", Pattern = @"{A} {B} {B}*" }
            };

            string chunkedText = new Chunker().Chunk(taggedText, rules);
            Assert.AreEqual("[ABB [A Arnold] [B barrel] [B Bill]] [A apple] [A Adam] [ABB [A Africa] [B Bob]]", chunkedText);
        }

        [Test]
        public void Chunk_GivenRuleWithConsecutiveChunks_CapturesConsecutiveChunks()
        {
            string taggedText = "[NP I/NN] [VP ate/VBD [NP cake/NN]]";

            var rules = new List<Rule>
            {
                new Rule { ChunkName = "DC", Pattern = @"{NP} {VP}" }
            };

            string chunkedText = new Chunker().Chunk(taggedText, rules);
            Assert.AreEqual("[DC [NP I/NN] [VP ate/VBD [NP cake/NN]]]", chunkedText);
        }

        [Test]
        public void Chunk_GivenRuleWithConsecutiveChunks_DoesNotCapturesNestedChunks()
        {
            string taggedText = "[NP I/NN] [FOOD [VP ate/VBD [NP cake/NN]]]";

            var rules = new List<Rule>
            {
                new Rule { ChunkName = "DC", Pattern = @"{NP} {VP}" }
            };

            string chunkedText = new Chunker().Chunk(taggedText, rules);
            Assert.AreEqual("[NP I/NN] [FOOD [VP ate/VBD [NP cake/NN]]]", chunkedText);
        }

        private List<Rule> GetChunkRules()
        {
            return new List<Rule>
            {
                new Rule { ChunkName = "NP", Pattern = @"{*/(DT|JJ|NNPS|NNP|NNS|NN|PRP)}+" },
                new Rule { ChunkName = "PP", Pattern = @"{*/IN} {NP}" },
                new Rule { ChunkName = "VP", Pattern = @"{*/VB.*?} {NP} {PP}*" },
                new Rule { ChunkName = "DC", Pattern = @"{NP} {VP}" }
            };
        }
    }
}
