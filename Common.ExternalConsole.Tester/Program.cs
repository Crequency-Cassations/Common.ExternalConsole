using System.Diagnostics;
using Common.ExternalConsole;

Console.WriteLine("Hello, Common.ExternalConsole!");

var manager = new Manager();
var console = manager.Register("Console");
console.Start();

ProcessStartInfo psi = new()
{
    FileName = Path.GetFullPath("./Common.ExternalConsole.Console.exe"),
    Arguments = "--connect CommonExternalConsoleConsole",
    CreateNoWindow = false,
    UseShellExecute = true,
};
Process.Start(psi);

new Thread(() =>
{
    try
    {
        while (true)
        {
            var remote = console.ReadLine();
            if (remote is not null)
                Console.WriteLine($"^ {remote}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"@ {ex.Message}");
    }
}).Start();

while (true)
{
    Console.Write("$ ");
    var input = Console.ReadLine();
    if (input is null) continue;
    if (input == "exit") break;
    console.WriteLine(input);
}

manager.RemoveConsole("Console");