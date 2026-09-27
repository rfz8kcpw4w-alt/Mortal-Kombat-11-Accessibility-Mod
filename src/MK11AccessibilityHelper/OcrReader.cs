using System.Drawing;
using System.Drawing.Imaging;
using Tesseract;

namespace MK11AccessibilityHelper;

public static class OcrReader
{
    public static string ReadTextFromRegion(Rectangle region, string tessdataPath)
    {
        using var bmp = CaptureRegion(region);
        return ReadTextFromBitmap(bmp, tessdataPath);
    }

    public static Bitmap CaptureRegion(Rectangle region)
    {
        var bitmap = new Bitmap(region.Width, region.Height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(region.Location, Point.Empty, region.Size, CopyPixelOperation.SourceCopy);
        return bitmap;
    }

    public static string ReadTextFromBitmap(Bitmap bitmap, string tessdataPath)
    {
        var enginePath = ResolveTessdataPath(tessdataPath);
        if (!Directory.Exists(enginePath))
        {
            return "Tesseract tessdata klasörü bulunamadı. Lütfen Tesseract OCR kurulumunu yapın.";
        }

        try
        {
            using var engine = new TesseractEngine(enginePath, "eng", EngineMode.Default);
            using var image = PixConverter.ToPix(bitmap);
            using var page = engine.Process(image);
            return page.GetText();
        }
        catch (Exception ex)
        {
            return $"OCR hatası: {ex.Message}";
        }
    }

    public static string ResolveTessdataPath(string configured)
    {
        if (!string.IsNullOrWhiteSpace(configured) && Directory.Exists(configured))
            return configured;

        var candidates = new[]
        {
            Path.Combine(Environment.CurrentDirectory, "tessdata"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Tesseract-OCR", "tessdata"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Tesseract-OCR", "tessdata"),
            Path.Combine("C:\\", "Program Files", "Tesseract-OCR", "tessdata"),
            Path.Combine("C:\\", "Program Files (x86)", "Tesseract-OCR", "tessdata")
        };

        foreach (var candidate in candidates)
            if (Directory.Exists(candidate)) return candidate;

        return configured;
    }
}
