namespace MyBackedApi.Helpers
{
    public static class DocumentMatch
    {
        public static bool FuzzyMatch(string text, string searchTerm)
        {
            var textWords = Tokenize(text);
            var searchWords = Tokenize(searchTerm);

            // Verifici dacă TOATE cuvintele din searchTerm există în text
            return searchWords.All(word => textWords.Contains(word));
        }

        private static HashSet<string> Tokenize(string text)
        {
            // Normalizează textul: lowercase, elimină semne de punctuație, sparge în cuvinte
            var cleaned = new string(text
                .ToLowerInvariant()
                .Select(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) ? c : ' ')
                .ToArray());

            return cleaned
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToHashSet();
        }

    }
}
