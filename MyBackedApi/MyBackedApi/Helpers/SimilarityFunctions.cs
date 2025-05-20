using System;
using System.Collections.Generic;
using System.Linq;

namespace MyBackedApi.Helpers
{
    public static class SimilarityFunctions
    {
        public static double CosineSimilarity(float[] a, float[] b)
        {
            double dot = 0, magA = 0, magB = 0;

            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }

            return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
        }

        public static double JaccardSimilarity(string textA, string textB)
        {
            var setA = new HashSet<string>(Normalize(textA).Split(' '));
            var setB = new HashSet<string>(Normalize(textB).Split(' '));

            var intersection = new HashSet<string>(setA);
            intersection.IntersectWith(setB);

            var union = new HashSet<string>(setA);
            union.UnionWith(setB);

            return union.Count == 0 ? 0 : (double)intersection.Count / union.Count;
        }

        public static double CombinedSimilarity(float[] vectorA, float[] vectorB, string textA, string textB)
        {
            double cosine = CosineSimilarity(vectorA, vectorB);
            double jaccard = JaccardSimilarity(textA, textB);

            return 0.7 * cosine + 0.3 * jaccard;
        }

        private static string Normalize(string text)
        {
            return new string(text.ToLowerInvariant()
                .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
