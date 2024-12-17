namespace SampleGame.Helpers;

public static class FileLogService
{
    public static void Write(string message)
    {
        // TODO : log4net 
        Console.WriteLine(message);
    }
}