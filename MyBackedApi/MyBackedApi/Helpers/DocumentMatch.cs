using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MyBackedApi.Helpers
{
    public static class DocumentMatch
    {
        public static bool FuzzyMatch(string text, string searchTerm)
        {
            var normalizedText = Normalize(text);
            var normalizedSearch = Normalize(searchTerm);

            return normalizedText.Contains(normalizedSearch);
        }

        private static string Normalize(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            // Elimină diacriticele
            var normalized = input.Normalize(NormalizationForm.FormD);
            var withoutDiacritics = new StringBuilder();
            foreach (var c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                    withoutDiacritics.Append(c);
            }

            // Elimină spațiile, punctuația, trece la lowercase
            var result = Regex.Replace(withoutDiacritics.ToString(), @"[\s\p{P}]", "").ToLowerInvariant();

            return result;
        }
    }
}
