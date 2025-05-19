using System.Text;
using DocumentFormat.OpenXml.Packaging;
using Tesseract;
using UglyToad.PdfPig;

public static class FileTextExtractor
{
    public static async Task<(string imageText, string documentText)> ExtractTextFromFilesAsync(List<(string FileName, Stream FileStream, string ContentType)> files)
    {
        var imageText = new StringBuilder();
        var documentText = new StringBuilder();

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                var text = await ExtractTextFromImageAsync(file.FileStream);
                imageText.AppendLine(text);
            }
            else if (extension == ".pdf")
            {
                var text = ExtractTextFromPdf(file.FileStream);
                documentText.AppendLine(text);
            }
            else if (extension == ".docx")
            {
                var text = ExtractTextFromDocx(file.FileStream);
                documentText.AppendLine(text);
            }
            else if (extension == ".txt")
            {
                using var reader = new StreamReader(file.FileStream);
                var text = await reader.ReadToEndAsync();
                documentText.AppendLine(text);
            }
        }

        return (imageText.ToString(), documentText.ToString());
    }

    private static async Task<string> ExtractTextFromImageAsync(Stream imageStream)
    {
        // Salvează temporar fișierul (Tesseract cere path local)
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

    private static string ExtractTextFromPdf(Stream stream)
    {
        using var pdf = PdfDocument.Open(stream);
        var text = new StringBuilder();

        foreach (var page in pdf.GetPages())
        {
            text.AppendLine(page.Text);
        }

        return text.ToString();
    }

    private static string ExtractTextFromDocx(Stream stream)
    {
        using var memStream = new MemoryStream();
        stream.CopyTo(memStream);
        memStream.Position = 0;

        using var wordDoc = WordprocessingDocument.Open(memStream, false);
        var body = wordDoc.MainDocumentPart.Document.Body;
        return body?.InnerText ?? "";
    }

}
