using System.Diagnostics;
using PerformanceTool.classes;

namespace PerformanceTool;

class Program
{

    static void Main(string[] args)
    {
        Storage storage = new Storage();
        Processes processes = new Processes();

        Console.WriteLine("*** Welcome to Performance Tool ***");
        Thread.Sleep(500);
        Console.WriteLine("Select your option:");
        Console.WriteLine("[1] Clear Files");
        Console.WriteLine("[2] Disable GameBar");

        var input = int.Parse(Console.ReadLine()!);

        switch (input)
        {
            case 1:
                storage.DeletePrefetchFiles();
                storage.DeleteTempFiles();
                storage.DeleteRecycleBin();
                break;

            case 2:
                processes.DisableXboxGameBar();
                break;

        }
    }
}
