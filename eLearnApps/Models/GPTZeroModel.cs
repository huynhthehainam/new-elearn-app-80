using Newtonsoft.Json;

namespace eLearnApps.Models.GptZeroModels
{
    public class GPTZeroModel
    {
        [JsonProperty("documents")]
        public List<Document> Documents { get; set; }
    }

    public class Document
    {
        [JsonProperty("average_generated_prob")]
        public float AverageGeneratedProb { get; set; }
        [JsonProperty("completely_generated_prob")]
        public float CompletelyGeneratedProb { get; set; }
        [JsonProperty("overall_burstiness")]
        public float OverallBurstiness { get; set; }
        [JsonProperty("paragraphs")]
        public Paragraph[] Paragraphs { get; set; }
        [JsonProperty("sentences")]
        public Sentence[] Sentences { get; set; }
    }

    public class Paragraph
    {
        [JsonProperty("completely_generated_prob")]
        public float CompletelyGeneratedProb { get; set; }
        [JsonProperty("num_sentences")]
        public int NumSentences { get; set; }
        [JsonProperty("start_sentence_index")]
        public int StartSentenceIndex { get; set; }
    }

    public class Sentence
    {
        [JsonProperty("generated_prob")]
        public float GeneratedProb { get; set; }
        [JsonProperty("perplexity")]
        public float Perplexity { get; set; }
        [JsonProperty("sentence")]
        public string SentenceContent { get; set; }
        [JsonProperty("highlight_sentence_for_ai")]
        public bool HighlightSentenceForAi { get; set; }
        private Guid guid;
        public void SetGuid(Guid value) => guid = value;
        public Guid GetGuid() => guid;

        private string noSpecialSentenceContent;
        public void SetNoSpecialSentenceContent(string value) => noSpecialSentenceContent = value;
        public string GetNoSpecialSentenceContent() => noSpecialSentenceContent;
    }
}
