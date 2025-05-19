using System.Globalization;
using System.Text;
using MyBackedApi.Models;

namespace MyBackedApi.Helpers
{
    public static class TextPreprocessor
    {
        public static string Normalize(string text)
        {
            text = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            return new string(chars.ToArray());
        }

        public static string Preprocess(Question question)
        {
            var textParts = new List<string>
            {
                question.Title ?? "",
                question.Content ?? "",
                question.ImageText ?? "",
                question.DocumentText ?? ""
            };

            if (question.Answers != null)
            {
                foreach (var answer in question.Answers)
                {
                    textParts.Add(answer.Content ?? "");
                }
            }

            var combined = string.Join(" ", textParts);
            return Normalize(combined);
        }
    }
}
