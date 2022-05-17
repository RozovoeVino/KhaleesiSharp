using KhaleesiSharp;

var khaleesi = new Khaleesi();

while (true)
{
    var msg = Console.ReadLine();
    if (msg.Length == 0)
        break;

    var kh = khaleesi.Process(msg);
    Console.WriteLine(kh);
}