using Common.ExternalConsole.Client;

Console.WriteLine("Common.ExternalConsole");
Console.WriteLine(Environment.Version);

var isRunning = true;
var isWaiting = true;
var pipeName = string.Empty;

for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--connect":
            if (i != args.Length - 1)
            {
                ++i;
                pipeName = args[i];
            }
            else
            {
                Console.WriteLine("No pipe name for connection!");
                return;
            }

            break;
    }
}

var client = new AClient();

Console.WriteLine("Init client and network connection ...");
client.Init(pipeName);
client.Start();

//  Send Thread
new Thread(() =>
{
    while (isRunning)
    {
        Console.Write(">>> ");
        isWaiting = true;
        var input = Console.ReadLine();
        isWaiting = false;
        if (input == null) continue;
        switch (input)
        {
            case "exit":
                isRunning = false;
                Environment.Exit(0);
                break;
            default:
                client.WriteLine(input);
                break;
        }
    }
}).Start();

// Receive Thread
new Thread(() =>
{
    while (isRunning)
    {
        var output = client.ReadLine();
        if (output is null) continue;
        Console.WriteLine($"{(isWaiting ? "\r\n" : "")}### {output}");
        isWaiting = false;
    }
}).Start();

while (isRunning)
{
}