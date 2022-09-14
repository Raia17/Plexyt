using System.Text.RegularExpressions;

namespace BstHelpers;

public static class FunctionsHelper {

    #region Message
    public static void LogMessage(string message, bool error = false) {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") {
            if (error) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            if (error) Console.ForegroundColor = ConsoleColor.White;
        } else {
            var logFolder = Path.Combine(Environment.CurrentDirectory, "Logs");
            if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);

            var logFile = Directory.GetFiles(logFolder)
                .Where(f => Path.GetExtension(f) == ".log" && Regex.IsMatch(Path.GetFileNameWithoutExtension(f), @"\d+"))
                .OrderByDescending(Path.GetFileName)
                .FirstOrDefault();
            if (logFile != null) {
                if (new FileInfo(logFile).Length > 52428800) {// 50MB
                    var newFile = Path.Combine(logFolder, (long.Parse(Path.GetFileNameWithoutExtension(logFile)) + 1).ToString()) + ".log";
                    using var stream = File.Create(newFile);
                    stream.Close();
                    logFile = newFile;
                }
            } else {
                logFile = Path.Combine(logFolder, "1.log");
            }
            File.AppendAllText(logFile, $"{(error ? "----------------ERROR-------------" : "")}{DateTime.Now:dd-MM-yyyy HH:mm}\n{message}{(error ? "\n----------------ERROR-------------" : "")}\n\n\n");
        }
    }
    public static void LogMessage(object messageObject, bool error = false) {
        LogMessage(messageObject?.ToString() ?? string.Empty, error);
    }
    #endregion

    public static TimeSpan GetTimeSpanFromString(string time) { //HH:mm
        var hour = time.Substring(0, 2);
        var minute = time.Substring(3, 2);
        return new TimeSpan(int.Parse(hour), int.Parse(minute), 0);
    }
}