using MyBackedApi.Models;
using System.Text.RegularExpressions;

namespace MyBackedApi.Services
{
    public class QuestionSimilarityService
    {
        private List<string> allQuestions;
        private List<Dictionary<string, double>> tfidfVectors;
        private Dictionary<string, double> idfValues;
        private List<(Guid Id, string Text)> questionData;

        public void LoadQuestions(List<Question> questions)
        {
            questionData = questions
                .Select(q => (q.Id, q.Title + " " + StripHtml(q.Content)))
                .ToList();

            allQuestions = questionData.Select(q => q.Text).ToList();
            ComputeTFIDF();
        }


        private void ComputeTFIDF()
        {
            var wordDocFreq = new Dictionary<string, int>();
            var docWordCounts = new List<Dictionary<string, int>>();

            foreach (var question in allQuestions)
            {
                var wordCount = new Dictionary<string, int>();
                var words = Tokenize(question);

                foreach (var word in words)
                {
                    if (!wordCount.ContainsKey(word))
                        wordCount[word] = 0;
                    wordCount[word]++;
                }

                docWordCounts.Add(wordCount);

                foreach (var word in wordCount.Keys)
                {
                    if (!wordDocFreq.ContainsKey(word))
                        wordDocFreq[word] = 0;
                    wordDocFreq[word]++;
                }
            }

            int docCount = allQuestions.Count;
            idfValues = wordDocFreq.ToDictionary(
                kv => kv.Key,
                kv => Math.Log((double)docCount / kv.Value)
            );

            tfidfVectors = new List<Dictionary<string, double>>();

            foreach (var wordCount in docWordCounts)
            {
                int totalWords = wordCount.Values.Sum();
                var tfidf = new Dictionary<string, double>();

                foreach (var word in wordCount.Keys)
                {
                    double tf = (double)wordCount[word] / totalWords;
                    double idf = idfValues[word];
                    tfidf[word] = tf * idf;
                }

                tfidfVectors.Add(tfidf);
            }
        }

        public List<(Guid Id, double Score)> GetSimilarQuestions(string input)
        {
            var inputVector = ComputeVector(Tokenize(input));
            var results = new List<(Guid Id, double Score)>();

            for (int i = 0; i < tfidfVectors.Count; i++)
            {
                double score = CosineSimilarity(inputVector, tfidfVectors[i]);
                results.Add((questionData[i].Id, score));
            }

            return results.OrderByDescending(r => r.Score).ToList();
        }


        private Dictionary<string, double> ComputeVector(List<string> words)
        {
            var wordCount = words.GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());
            int totalWords = words.Count;
            var vector = new Dictionary<string, double>();

            foreach (var word in wordCount.Keys)
            {
                double tf = (double)wordCount[word] / totalWords;
                double idf = idfValues.ContainsKey(word) ? idfValues[word] : 0;
                vector[word] = tf * idf;
            }

            return vector;
        }

        private double CosineSimilarity(Dictionary<string, double> v1, Dictionary<string, double> v2)
        {
            var allKeys = v1.Keys.Union(v2.Keys);
            double dot = 0, mag1 = 0, mag2 = 0;

            foreach (var key in allKeys)
            {
                double a = v1.ContainsKey(key) ? v1[key] : 0;
                double b = v2.ContainsKey(key) ? v2[key] : 0;

                dot += a * b;
                mag1 += a * a;
                mag2 += b * b;
            }

            if (mag1 == 0 || mag2 == 0)
                return 0;

            return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
        }

        private List<string> Tokenize(string text)
        {
            var separators = new[] { ' ', ',', '.', '!', '?', '(', ')', ':', ';', '[', ']', '{', '}', '\n', '\r', '\t' };
            var stopWords = new HashSet<string> {
            "how", "and", "is", "done", "the", "a", "an", "in", "on", "at", "to",
            "for", "with", "by", "of", "this", "that", "it", "from", "i", "you"
        };

            return text.ToLower()
                       .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                       .Where(w => !stopWords.Contains(w))
                       .ToList();
        }

        private string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            // Normalizează tag-urile semantice
            html = Regex.Replace(html, @"<\s*b\s*>", " ");         // Bold -> spațiu
            html = Regex.Replace(html, @"<\s*/\s*b\s*>", " ");
            html = Regex.Replace(html, @"<\s*strong\s*>", " ");
            html = Regex.Replace(html, @"<\s*/\s*strong\s*>", " ");

            html = Regex.Replace(html, @"<\s*code\s*>", " code: "); // cod -> etichetă text
            html = Regex.Replace(html, @"<\s*/\s*code\s*>", " ");

            html = Regex.Replace(html, @"<\s*br\s*/?\s*>", " ");     // newline simplificat
            html = Regex.Replace(html, @"<\s*p\s*>", " ");
            html = Regex.Replace(html, @"<\s*/\s*p\s*>", " ");

            // Șterge tot restul tagurilor HTML
            html = Regex.Replace(html, "<.*?>", string.Empty);

            // Normalizează spațiile duble
            return Regex.Replace(html, @"\s+", " ").Trim();
        }


    }


}
