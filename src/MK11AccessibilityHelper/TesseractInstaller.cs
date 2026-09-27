using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http;

namespace MK11AccessibilityHelper;

public static class TesseractInstaller
{
    private const string TessdataUrl = "https://github.com/UB-Mannheim/tesseract/wiki";
    private const string EngTraineddataUrl = "https://raw.githubusercontent.com/UB-Mannheim/tesseract/master/tessdata/eng.traineddata";
    private const string TesseractExeUrl = "https://github.com/UB-Mannheim/tesseract/releases/download/v5.3.0/tesseract-ocr-w64-setup-v5.3.0.exe";

    public static async Task<bool> EnsureTesseractInstalled()
    {
        // Tesseract'ın kurulu olup olmadığını kontrol et
        if (IsTesseractInstalled()) return true;

        // Kurulu değilse, tessdata klasörünü kontrol et
        var tessdataPath = GetLocalTessdataPath();
        if (!Directory.Exists(tessdataPath))
        {
            Directory.CreateDirectory(tessdataPath);
        }

        // eng.traineddata dosyasını indir
        var engTraineddataPath = Path.Combine(tessdataPath, "eng.traineddata");
        if (!File.Exists(engTraineddataPath))
        {
            Console.WriteLine("eng.traineddata indiriliyor...");
            try
            {
                await DownloadFile(EngTraineddataUrl, engTraineddataPath);
                Console.WriteLine($"eng.traineddata indirildi: {engTraineddataPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"İndirme hatası: {ex.Message}");
                return false;
            }
        }

        return true;
    }

    public static bool IsTesseractInstalled()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "tesseract",
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);
            process?.WaitForExit(5000);
            return process?.ExitCode == 0;
        }
        catch { return false; }
    }

    public static string GetLocalTessdataPath()
    {
        return Path.Combine(Environment.CurrentDirectory, "tessdata");
    }

    private static async Task DownloadFile(string url, string filePath)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        using var contentStream = await response.Content.ReadAsStreamAsync();
        using var fileStream = File.Create(filePath);
        await contentStream.CopyToAsync(fileStream);
    }
}
