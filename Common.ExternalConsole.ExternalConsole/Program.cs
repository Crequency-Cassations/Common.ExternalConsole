using System.Net.Sockets;
using System.Text;
using Common.ExternalConsole.ExternalConsole;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Common.ExternalConsole v2");
Console.WriteLine($"Runtime Version: {Environment.Version}");

const string serverAddress = "127.0.0.1";
const string consoleTitle = "Common.ExternalConsole.Console | Name: ";

var config = ArgumentHelper.Process(args);
if (config is null) return;

var name = config.Name;
var port = config.Port;
var keepWorking = true;
var messages2Send = new Queue<string>();
var messages2SendLock = new object();

Console.WriteLine("Init client and network connection ...");

var client = new TcpClient();
await client.ConnectAsync(serverAddress, port);

Console.WriteLine($"Connected to {serverAddress}:{port} .");

void Stop()
{
    keepWorking = false;
    client.Close();
    Environment.Exit(0);
}

//  Receive messages. 
async void ReceiveMessages()
{
    await using var ns = client.GetStream();
    using var sr = new StreamReader(ns);
    while (keepWorking)
    {
        if (client.Available <= 0) continue;
        var message = await sr.ReadLineAsync();

        PromptHelper.Debug($"Receive: {message}");

        switch (message)
        {
            case null:
                continue;
            case @"|^stop_console|":
                Stop();
                break;
        }

        // if (isWaitingInput) MoveCursorToLineStart();
        PromptHelper.Remote(message);
    }

    sr.Close();
    sr.Dispose();
}

//  Send messages.
async void SendMessages()
{
    await using var ns = client.GetStream();
    await using var sw = new StreamWriter(ns);
    while (keepWorking)
    {
        lock (messages2SendLock)
        {
            while (messages2Send.Count != 0)
            {
                var message = messages2Send.Dequeue();
                sw.WriteLine(message);

                PromptHelper.Debug($"Send: {message}");
            }
        }

        if (!keepWorking) Stop();

        await sw.FlushAsync();
    }
}

async void HandleUserInterface()
{
    while (keepWorking)
    {
        // if (Console.GetCursorPosition().Left != 0) Console.WriteLine();

        if (!PromptHelper.DebugEnabled)
            PromptHelper.User();
        var input = Console.ReadLine();

        switch (input)
        {
            case null: continue;
            case "exit":
                messages2Send?.Enqueue(@"|^console_exit|");

                await Task.Run(() =>
                {
                    Task.Delay(1000);
                    Stop();
                });
                break;
            case @"|^disable_debug|":
                PromptHelper.DebugEnabled = false;
                Console.Beep();
                break;
            case @"|^enable_debug|":
                PromptHelper.DebugEnabled = true;
                PromptHelper.User();
                Console.Beep();
                break;
            default:
                messages2Send?.Enqueue(input);
                break;
        }
    }
}

var sendingTitle = $"{consoleTitle}{name}";
messages2Send.Enqueue(sendingTitle);

new Thread(ReceiveMessages).Start();
new Thread(SendMessages).Start();
new Thread(HandleUserInterface).Start();

while (keepWorking)
{
    keepWorking = true;
}