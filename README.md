#POS chunker

A C# library for chunking POS-tagged text such as that output by the Stanford POS tagger: http://sergey-tihon.github.io/Stanford.NLP.NET/StanfordPOSTagger.html

##Example usage:

```

string taggedText = "The/DT professor/NN walked/VBD the/DT dog/NN in/IN the/DT park/NN ./. The/DT dog/NN chased/VBD a/DT big/JJ stick/NN ./.";

var rules = new List<Rule>
{
    new Rule { ChunkName = "NP", Pattern = @"{*/(DT|JJ|NNPS|NNP|NNS|NN|PRP)}+" }, // noun phrase
    new Rule { ChunkName = "PP", Pattern = @"{*/IN} {NP}" },                      // prepositional phrase
    new Rule { ChunkName = "VP", Pattern = @"{*/VB.*?} {NP} {PP}*" },             // verb phrase
    new Rule { ChunkName = "DC", Pattern = @"{NP} {VP}" }                         // declarative clause
};

string chunkedText = new Chunker().Chunk(taggedText, rules);

```

The chunked output will be:

[DC [NP The/DT professor/NN] [VP walked/VBD [NP the/DT dog/NN] [PP in/IN [NP the/DT park/NN]]]] ./. [DC [NP The/DT dog/NN] [VP chased/VBD [NP a/DT big/JJ stick/NN]]] ./.

Each Rule defines a Regex pattern. Additionally, some shortcut patterns are allowed which are transformed into Regexes before application:

To capture an existing chunk: {CHUNKNAME}

To optionally capture an existing chunk: {CHUNKNAME}*

To capture a tagged word regardless of the tag: {word/*}

To capture a tagged word regardless of the word: {*/TAG}

To capture multiple consecutive tagged words: {*/(TAG1|TAG2|TAG3)}+

For example, "{NP} {VP}" above translates to "a noun phrase (NP) chunk followed by a verb phrase (VP) chunk" and this outputs a Declarative Clause chunk (DC).

Any brackets in the original text are expected to have been escaped by the previous POS tagging step. 


##Chunk parser

Also included is a parser which takes the chunked output and produces a tree structure for easier analysis of the sentence(s). An example from the unit tests:

```

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

```


##Inspiration

This library was inspired by Mark Birbeck's POS chunker for Node.js:

https://github.com/markbirbeck/pos-chunker

There are some excellent in-depth examples of the power of POS chunking on his above page.
