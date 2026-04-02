public static class Logger
{
    private static string logPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "log.txt"
    );

    public static void Log(string mensaje)
    {
        try
        {
            File.AppendAllText(logPath,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {mensaje}\n");
        }
        catch { }
    }
}