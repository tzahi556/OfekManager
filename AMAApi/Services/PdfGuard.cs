using iTextSharp.text.pdf;
using System;
using System.IO;


public static class PdfGuard
{
    public static bool IsPdfOk(string path, out string reason)
    {
        reason = "";
        try
        {
            if (string.IsNullOrWhiteSpace(path)) { reason = "Path is empty"; return false; }
            if (!File.Exists(path)) { reason = "File not found"; return false; }

            var fi = new FileInfo(path);
            if (fi.Length < 50) { reason = $"File too small ({fi.Length} bytes)"; return false; }

            using (var reader = new PdfReader(path))
            {
                if (reader.NumberOfPages <= 0) { reason = "PDF has 0 pages"; return false; }
                // אופציונלי: נגיעה בעמוד 1
                reader.GetPageContent(1);
            }

            return true;
        }
        catch (Exception ex)
        {
            reason = ex.GetType().Name + ": " + ex.Message;
            return false;
        }
    }

    public static bool IsImageOk(string path, out string reason)
    {
        reason = "";
        try
        {
            if (string.IsNullOrWhiteSpace(path)) { reason = "Path is empty"; return false; }
            if (!File.Exists(path)) { reason = "File not found"; return false; }

            var fi = new FileInfo(path);
            if (fi.Length < 32) { reason = $"File too small ({fi.Length} bytes)"; return false; }

            // Magic bytes (PNG/JPG) - סינון מהיר
            using (var fs = File.OpenRead(path))
            {
                var header = new byte[8];
                int read = fs.Read(header, 0, header.Length);
                if (read < 8) { reason = "Cannot read header"; return false; }

                bool isPng = header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47;
                bool isJpg = header[0] == 0xFF && header[1] == 0xD8;

                if (!isPng && !isJpg)
                {
                    reason = "Not PNG/JPG by header";
                    return false;
                }
            }

            // בדיקה אמיתית: iTextSharp מצליח לפענח?
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var img = iTextSharp.text.Image.GetInstance(fs); // אם זה פגום - יזרוק חריגה
                if (img.Width <= 0 || img.Height <= 0)
                {
                    reason = "Invalid dimensions";
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            reason = ex.GetType().Name + ": " + ex.Message;
            return false;
        }
    }
}
