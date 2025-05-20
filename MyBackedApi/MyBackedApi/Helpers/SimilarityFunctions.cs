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

    }
}
