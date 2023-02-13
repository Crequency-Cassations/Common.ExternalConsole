using Common.ExternalConsole;

Console.WriteLine("Hello, Common.ExternalConsole!");

var manager = await new ExternalConsolesManager()
        .LaunchServer(7777)
    ;

var name = "1";

var console = manager.Register(name);

var keepWorking = true;
var messages2Send = new Queue<string>();

async void Reader(StreamReader reader)
{
    while (keepWorking)
    {
        var message = await reader.ReadLineAsync();
        switch (message)
        {
            case @"|^console_exit|":
                keepWorking = false;
                Console.WriteLine("logout");
                break;
            default:
                Console.WriteLine(message);
                break;
        }
    }
}

async void Writer(StreamWriter writer)
{
    while (keepWorking)
    {
        if (messages2Send.Count > 0)
        {
            await writer.WriteLineAsync(messages2Send.Dequeue());
            await writer.FlushAsync();
        }
    }
}

console.HandleMessages(Reader, Writer);

while (true)
    messages2Send.Enqueue(Console.ReadLine() ?? "null");