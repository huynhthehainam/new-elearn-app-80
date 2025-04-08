
using System.Text.Json.Serialization;

namespace eLearnApps.Models.GptZeroModels
{
    public class GPTZeroModel
    {
        [JsonPropertyName("documents")]
        public List<Document> Documents { get; set; }
    }

    public class Document
    {
        [JsonPropertyName("average_generated_prob")]
        public float AverageGeneratedProb { get; set; }
        [JsonPropertyName("completely_generated_prob")]
        public float CompletelyGeneratedProb { get; set; }
        [JsonPropertyName("overall_burstiness")]
        public float OverallBurstiness { get; set; }
        [JsonPropertyName("paragraphs")]
        public Paragraph[] Paragraphs { get; set; }
        [JsonPropertyName("sentences")]
        public Sentence[] Sentences { get; set; }
    }

    public class Paragraph
    {
        [JsonPropertyName("completely_generated_prob")]
        public float CompletelyGeneratedProb { get; set; }
        [JsonPropertyName("num_sentences")]
        public int NumSentences { get; set; }
        [JsonPropertyName("start_sentence_index")]
        public int StartSentenceIndex { get; set; }
    }

    public class Sentence
    {
        [JsonPropertyName("generated_prob")]
        public float GeneratedProb { get; set; }
        [JsonPropertyName("perplexity")]
        public float Perplexity { get; set; }
        [JsonPropertyName("sentence")]
        public string SentenceContent { get; set; }
        [JsonPropertyName("highlight_sentence_for_ai")]
        public bool HighlightSentenceForAi { get; set; }
        private Guid guid;
        public void SetGuid(Guid value) => guid = value;
        public Guid GetGuid() => guid;

        private string noSpecialSentenceContent;
        public void SetNoSpecialSentenceContent(string value) => noSpecialSentenceContent = value;
        public string GetNoSpecialSentenceContent() => noSpecialSentenceContent;
    }
}
