// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

int size = args.Length;
if(size == 1)
{
    var fileName = args[0];
    ParseHaversineJson(fileName);
}
else if(size == 2)
{
    var binaryFile = args[1];
}
else
{
    Console.WriteLine("Usage: dotnet run [haversine_input.json]");

    Console.WriteLine("       dotnet run [haversine_input.json] [answers.txt]");
}

void ParseHaversineJson(string fileName)
{
    using(var stream = File.Open(fileName, FileMode.Open))
    {
        int value = 0;
        char character;
        int leadingBracket = 0;
        bool expectDoubleQuote = false;
        while(true)
        {
            value = stream.ReadByte();
            if(value < 0)
            {
                break;
            }
            character = (char)value;
            //Console.Write(character);
            switch(character)
            {
                case '{':
                {
                    ++leadingBracket;
                    expectDoubleQuote = true;
                } break;
                case '"':
                {
                    if(!expectDoubleQuote)
                    {
                        Console.WriteLine("Something went horribly wrong with parsing double quotes");
                    }
                }
            }
        }
    }
}