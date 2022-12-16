using Common.ExternalConsole.Client;

Console.WriteLine("Common.ExternalConsole");
Console.WriteLine(Environment.Version);

var isRunning = true;
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
        var input = Console.ReadLine();
        if (input == null) continue;
        switch (input)
        {
            case "exit":
                isRunning = false;
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
        if (output is not null) Console.WriteLine(output);
    }
}).Start();

while (isRunning)
{
}