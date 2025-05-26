using System.Text;
using DocumentFormat.OpenXml.Packaging;
using Tesseract;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

public static class FileTextExtractor
{
    public class ExtractedFileText
    {
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public List<PageText> Pages { get; set; } = new(); // Pentru PDF
        public string? FullText { get; set; } // Pentru imagini și .txt
    }

    public class PageText
    {
        public int PageNumber { get; set; }
        public string Text { get; set; } = null!;
    }

    public static async Task<List<ExtractedFileText>> ExtractTextFromFilesAsync(List<(string FileName, Stream FileStream, string ContentType)> files)
    {
        var results = new List<ExtractedFileText>();

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            var extracted = new ExtractedFileText
            {
                FileName = file.FileName,
                ContentType = file.ContentType
            };

            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                var text = await ExtractTextFromImageAsync(file.FileStream);
                extracted.FullText = text;
            }
            else if (extension == ".pdf")
            {
                extracted.Pages = ExtractTextFromPdfWithPages(file.FileStream);
            }

            results.Add(extracted);
        }

        return results;
    }

    private static async Task<string> ExtractTextFromImageAsync(Stream imageStream)
    {
        var tempFile = Path.GetTempFileName();
        await using (var fileStream = File.Create(tempFile))
        {
            await imageStream.CopyToAsync(fileStream);
        }

        var ocrEngine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        using var img = Pix.LoadFromFile(tempFile);
        using var page = ocrEngine.Process(img);
        return page.GetText();
    }

    private static List<PageText> ExtractTextFromPdfWithPages(Stream stream)
    {
        var result = new List<PageText>();
        using var pdf = PdfDocument.Open(stream);

        foreach (var page in pdf.GetPages())
        {
            result.Add(new PageText
            {
                PageNumber = (int)page.Number,
                Text = page.Text
            });
        }

        return result;
    }

    public static string ExtractSnippet(string text, string searchTerm, int contextLength = 50)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(searchTerm))
            return text;

        var index = text.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);
        if (index == -1)
            return text.Length > 200 ? text.Substring(0, 200) + "..." : text; // fallback: primii 200 de char

        var start = Math.Max(0, index - contextLength);
        var length = Math.Min(searchTerm.Length + 2 * contextLength, text.Length - start);
        var snippet = text.Substring(start, length).Trim();

        return "..." + snippet + "...";
    }

}
