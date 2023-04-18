// See https://aka.ms/new-console-template for more information
Console.WriteLine("bits 16");

//var fileName = "listing_0037_single_register_mov";
var fileNames = new string[]{
    //fileName, "listing_0038_many_register_mov", 
//"listing_0039_more_movs", "listing_0041_add_sub_cmp_jnz",
//"listing_0043_immediate_movs", "listing_0044_register_movs",
//"listing_0046_add_sub_cmp",
//"listing_0048_ip_register", 
//"listing_0049_conditional_jumps",
//"listing_0051_memory_mov",
//"listing_0052_memory_add_loop",
"listing_0054_draw_rectangle"
 }; //"listing_0040_challenge_movs"};

foreach(var aFile in fileNames)
{
    Console.WriteLine(aFile);
    var ipRegistryInfo = new Dictionary<string, string>();
    UInt16 currentIP = 0;
    var registers = new Dictionary<string, string>();
    registers.Add("ax", "0");
    registers.Add("bx", "0");
    registers.Add("cx", "0");
    registers.Add("dx", "0");
    registers.Add("sp", "0");
    registers.Add("bp", "0");
    registers.Add("si", "0");
    registers.Add("di", "0");
    registers.Add("ip", "0");
    registers.Add("flags", "");
    
    var memory = new List<string>();
    for(int i = 0; i < 256 + 64*64*4; ++i)
    {
        memory.Add("0");
    }

    using(var stream = File.Open(aFile, FileMode.Open))
    {
        using(var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, false))
        {
            var binaries = new List<byte>();
            while(reader.BaseStream.Position != reader.BaseStream.Length)
            {
                var result = reader.ReadByte();
                binaries.Add(result);
            }

            while(currentIP < binaries.Count)
            {
                var originalIP = currentIP;
                var firstByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                ++currentIP;
                var secondByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                ++currentIP;
                //Console.Write($" {firstByte} {secondByte} \n");
                string instruction = firstByte + secondByte;
                //Console.WriteLine(instruction);

                if(instruction.Substring(0, 4) == "1011")
                {
                    var reg = instruction.Substring(5, 3);
                    bool isByteData = (instruction[4] == '0') ? true : false;
                    var destination = GetREGFieldEncoding(reg, isByteData);
                    string source = "error";
                    if(isByteData)
                    {
                        source = Convert.ToUInt16(secondByte, 2).ToString();
                    }
                    else
                    {
                        firstByte = Convert.ToString(binaries[currentIP], 2);
                        ++currentIP;
                        var word = firstByte + secondByte;
                        source = Convert.ToUInt16(word, 2).ToString();
                        //Console.Write($" word: {word} ");
                    }

                    //Console.Write($" {firstByte} {secondByte} ");
                    Console.Write($"mov {destination}, {source}");
                    PerformAction("mov", ref registers, destination, source, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 7) == "1100011")
                {
                    // W
                    var isByteData = (instruction[7] == '0') ? true : false;

                    var mod = instruction.Substring(8, 2);
                    var reg = instruction.Substring(10, 3);
                    var rm = instruction.Substring(13, 3);
                    var regFieldEncoding = GetREGFieldEncoding(reg, isByteData);
                    var address = GetRMFieldEncodingValue(rm, isByteData, mod, binaries, ref currentIP);
                    var data = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    string data2 = "";
                    if(!isByteData)
                    {
                        data2 = Convert.ToString(binaries[currentIP], 2);
                        ++currentIP; 
                    }
                    var word = data2 + data;
                    var result = Convert.ToUInt16(word, 2).ToString();
                    //Console.WriteLine($"\n Contents 1: {regFieldEncoding} {address} {result}");
                    if(address.Contains('+') && !isByteData)
                    {
                        address = $"word {address}";
                        Console.Write($"mov {address}, {result}");
                    }
                    else if(address.Contains('+') && isByteData)
                    {
                        address = $"byte {address}";
                        Console.Write($"mov {address}, {result}");
                    }
                    else if(registers[regFieldEncoding] == "0" || (!registers[regFieldEncoding].Contains('a') ||
                    !registers[regFieldEncoding].Contains('b') && !registers[regFieldEncoding].Contains('c') ||
                    !registers[regFieldEncoding].Contains('d') && !registers[regFieldEncoding].Contains('e') ||
                    !registers[regFieldEncoding].Contains('2') && !registers[regFieldEncoding].Contains('1') ||
                    !registers[regFieldEncoding].Contains('3') && !registers[regFieldEncoding].Contains('4') ||
                    !registers[regFieldEncoding].Contains('5') && !registers[regFieldEncoding].Contains('6') ||
                    !registers[regFieldEncoding].Contains('7') && !registers[regFieldEncoding].Contains('8') ||
                    !registers[regFieldEncoding].Contains('9')))
                    {
                        Console.Write($"mov {address}, {result}");
                    }
                    else
                    {
                        result = $"{regFieldEncoding}+{result}";
                        Console.Write($"mov {address}, {result}");
                    }
                    PerformAction("mov", ref registers, address, result, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 6) == "100010")
                {
                    // D
                    var isREGSource = (instruction[6] == '0') ? true : false;

                    // W
                    var isByteData = (instruction[7] == '0') ? true : false;

                    var mod = instruction.Substring(8, 2);
                    var rm = instruction.Substring(13, 3);
                    var reg = instruction.Substring(10, 3);
                    var regFieldEncoding = GetREGFieldEncoding(reg, isByteData);
                    var address = GetRMFieldEncodingValue(rm, isByteData, mod, binaries, ref currentIP);
                    //Console.WriteLine($" Contents 2: {reg} {mod} {rm} {regFieldEncoding} {address}");
                    var source = isREGSource ? regFieldEncoding : address;
                    var destination = isREGSource ? address : regFieldEncoding;

                    //Console.WriteLine($"\n Contents 1: {regFieldEncoding} {address} {result}");
                    if(destination.Contains('+') || isByteData == false)
                    {
                        destination = $"word {destination}";
                    }
                    if(source.Contains('+') && source.ElementAt(source.IndexOf('+') - 1) != '[')
                    {
                        source = $"word {source}";
                    }
                    Console.Write($"mov {destination}, {source}");
                    PerformAction("mov", ref registers, destination, source, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 6) == "000000")
                {
                    // D
                    var isREGSource = (instruction[6] == '0') ? true : false;

                    // W
                    var isByteData = (instruction[7] == '0') ? true : false;

                    var mod = instruction.Substring(8, 2);
                    var rm = instruction.Substring(13, 3);
                    var reg = instruction.Substring(10, 3);
                    var regFieldEncoding = GetREGFieldEncoding(reg, isByteData);
                    var address = GetRMFieldEncodingValue(rm, isByteData, mod, binaries, ref currentIP);
                    //Console.WriteLine($"{reg} {mod} {rm} {regFieldEncoding} {address}");
                    var source = isREGSource ? regFieldEncoding : address;
                    var destination = isREGSource ? address : regFieldEncoding;
                    //Console.WriteLine($"\n Contents: {regFieldEncoding} {address} {source} ");
                    Console.Write($"add {destination}, {source}");
                    PerformAction("add", ref registers, destination, source, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 6) == "100000" && instruction.Substring(10, 3) == "000")
                {
                    // S
                    var isNoSignExtension = (instruction[6] == '0') ? true : false;

                    // W
                    var isByteData = (instruction[7] == '0') ? true : false;

                    var mod = instruction.Substring(8, 2);
                    var rm = instruction.Substring(13, 3);
                    var reg = instruction.Substring(10, 3);
                    //var regFieldEncoding = GetREGFieldEncoding(reg, isByteData);
                    var address = GetRMFieldEncodingValue(rm, isByteData, mod, binaries, ref currentIP);
                    //Console.WriteLine($"{reg} {mod} {rm} {regFieldEncoding} {address}");
                    string source = "error";
                    secondByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                    ++currentIP;
                    //Console.WriteLine($" {!isByteData}  {isNoSignExtension} {instruction.Substring(6, 2)} ");
                    if(!isByteData && isNoSignExtension)
                    {
                        firstByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                        ++currentIP;
                        var word = firstByte + secondByte;
                        //Console.WriteLine($" {secondByte}  {firstByte}");
                        source = Convert.ToUInt16(word, 2).ToString();
                        //Console.Write($" word: {word} ");
                    }
                    else
                    {
                        source = Convert.ToUInt16(secondByte, 2).ToString();
                    }
                    var destination = address;

                    //Console.Write($" addtesting {firstByte} {secondByte} ");
                    Console.Write($"add {destination}, {source}");
                    PerformAction("add", ref registers, destination, source, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 7) == "0000010")
                {
                    var reg = instruction.Substring(5, 3);
                    bool isByteData = (instruction[7] == '0') ? true : false;
                    var destination = isByteData ? "al" : "ax";//GetREGFieldEncoding(reg, isByteData);
                    string source = "error";
                    if(isByteData)
                    {
                        source = Convert.ToUInt16(secondByte, 2).ToString();
                    }
                    else
                    {
                        firstByte = Convert.ToString(binaries[currentIP], 2);
                        ++currentIP;
                        var word = firstByte + secondByte;
                        source = Convert.ToUInt16(word, 2).ToString();
                        //Console.Write($" word: {word} ");
                    }

                    Console.Write($" {firstByte} {secondByte} ");
                    Console.Write($"add {destination}, {source}");
                    PerformAction("add", ref registers, destination, source, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 6) == "001010")
                {
                    // D
                    var isREGSource = (instruction[6] == '0') ? true : false;

                    // W
                    var isByteData = (instruction[7] == '0') ? true : false;

                    var mod = instruction.Substring(8, 2);
                    var rm = instruction.Substring(13, 3);
                    var reg = instruction.Substring(10, 3);
                    var regFieldEncoding = GetREGFieldEncoding(reg, isByteData);
                    var address = GetRMFieldEncodingValue(rm, isByteData, mod, binaries, ref currentIP);
                    //Console.WriteLine($"{reg} {mod} {rm} {regFieldEncoding} {address}");
                    var source = isREGSource ? regFieldEncoding : address;
                    var destination = isREGSource ? address : regFieldEncoding;

                    Console.Write($"sub {destination}, {source}");
                    PerformAction("sub", ref registers, destination, source, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 6) == "100000" && instruction.Substring(10, 3) == "101")
                {
                    // S
                    var isNoSignExtension = (instruction[6] == '0') ? true : false;

                    // W
                    var isByteData = (instruction[7] == '0') ? true : false;

                    var mod = instruction.Substring(8, 2);
                    var rm = instruction.Substring(13, 3);
                    var reg = instruction.Substring(10, 3);
                    //var regFieldEncoding = GetREGFieldEncoding(reg, isByteData);
                    var address = GetRMFieldEncodingValue(rm, isByteData, mod, binaries, ref currentIP);
                    //Console.WriteLine($"{reg} {mod} {rm} {regFieldEncoding} {address}");
                    string source = "error";
                    secondByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    if(!isByteData && isNoSignExtension)
                    {
                        firstByte = Convert.ToString(binaries[currentIP], 2);
                        ++currentIP;
                        var word = firstByte + secondByte;
                        source = Convert.ToUInt16(word, 2).ToString();
                        //Console.Write($" word: {word} ");
                    }
                    else
                    {
                        source = Convert.ToUInt16(secondByte, 2).ToString();
                    }
                    var destination = address;

                    Console.Write($"sub {destination}, {source}");
                    PerformAction("sub", ref registers, destination, source, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 7) == "0010110")
                {
                    var reg = instruction.Substring(5, 3);
                    bool isByteData = (instruction[7] == '0') ? true : false;
                    var destination = isByteData ? "al" : "ax";//GetREGFieldEncoding(reg, isByteData);
                    string source = "error";
                    if(isByteData)
                    {
                        source = Convert.ToUInt16(secondByte, 2).ToString();
                    }
                    else
                    {
                        firstByte = Convert.ToString(binaries[currentIP], 2);
                        ++currentIP;
                        var word = firstByte + secondByte;
                        source = Convert.ToUInt16(word, 2).ToString();
                        //Console.Write($" word: {word} ");
                    }

                    //Console.Write($" {firstByte} {secondByte} ");
                    Console.Write($"sub {destination}, {source}");
                    PerformAction("sub", ref registers, destination, source, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                // CMP
                else if(instruction.Substring(0, 6) == "001110")
                {
                    // D
                    var isREGSource = (instruction[6] == '0') ? true : false;

                    // W
                    var isByteData = (instruction[7] == '0') ? true : false;

                    var mod = instruction.Substring(8, 2);
                    var rm = instruction.Substring(13, 3);
                    var reg = instruction.Substring(10, 3);
                    var regFieldEncoding = GetREGFieldEncoding(reg, isByteData);
                    var address = GetRMFieldEncodingValue(rm, isByteData, mod, binaries, ref currentIP);
                    //Console.WriteLine($"{reg} {mod} {rm} {regFieldEncoding} {address}");
                    var source = isREGSource ? regFieldEncoding : address;
                    var destination = isREGSource ? address : regFieldEncoding;

                    Console.Write($"cmp {destination}, {source}");
                    PerformAction("cmp", ref registers, destination, source, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 6) == "100000" && instruction.Substring(10, 3) == "111")
                {
                    // S
                    var isNoSignExtension = (instruction[6] == '0') ? true : false;

                    // W
                    var isByteData = (instruction[7] == '0') ? true : false;

                    var mod = instruction.Substring(8, 2);
                    var rm = instruction.Substring(13, 3);
                    var reg = instruction.Substring(10, 3);
                    //var regFieldEncoding = GetREGFieldEncoding(reg, isByteData);
                    var address = GetRMFieldEncodingValue(rm, isByteData, mod, binaries, ref currentIP);
                    //Console.WriteLine($"{reg} {mod} {rm} {regFieldEncoding} {address}");
                    string source = "error";
                    secondByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    if(isByteData && !isNoSignExtension)
                    {
                        firstByte = Convert.ToString(binaries[currentIP], 2);
                        ++currentIP;
                        var word = firstByte + secondByte;
                        source = Convert.ToUInt16(word, 2).ToString();
                        //Console.Write($" word: {word} ");
                    }
                    else
                    {
                        source = Convert.ToUInt16(secondByte, 2).ToString();
                    }
                    var destination = address;

                    Console.Write($"cmp {destination}, {source}");
                    PerformAction("cmp", ref registers, destination, source, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 7) == "0011110")
                {
                    var reg = instruction.Substring(5, 3);
                    bool isByteData = (instruction[7] == '0') ? true : false;
                    var destination = isByteData ? "al" : "ax";//GetREGFieldEncoding(reg, isByteData);
                    string source = "error";
                    if(isByteData)
                    {
                        source = Convert.ToUInt16(secondByte, 2).ToString();
                    }
                    else
                    {
                        firstByte = Convert.ToString(binaries[currentIP], 2);
                        ++currentIP;
                        var word = firstByte + secondByte;
                        source = Convert.ToUInt16(word, 2).ToString();
                        //Console.Write($" word: {word} ");
                    }

                    //Console.Write($" {firstByte} {secondByte} ");
                    Console.Write($"cmp {destination}, {source}");
                    PerformAction("cmp", ref registers, destination, source, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                // Jumps
                else if(instruction.Substring(0, 8) == "01110101")
                {
                    //Console.WriteLine(secondByte);
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    if(secondByte[0] == '1')
                    {
                        var valueToMinus = Int16.Parse(destination) - 254;
                        Console.Write($"jne ${valueToMinus} ;");
                        PerformAction("jne", ref registers, valueToMinus.ToString(), string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                    }
                    else
                    {
                        Console.Write($"jne {destination} ;");
                        PerformAction("jne", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                    }
                }
                else if(instruction.Substring(0, 8) == "01110100")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"je {destination}");
                    PerformAction("je", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01111100")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jl {destination}");
                    PerformAction("jl", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01111110")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jle {destination}");
                    PerformAction("jle", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01110010")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jb {destination}");
                    PerformAction("jb", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01110110")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jbe {destination}");
                    PerformAction("jbe", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01111010")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jp {destination}");
                    PerformAction("jp", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01110000")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jo {destination}");
                    PerformAction("jo", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01111000")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"js {destination}");
                    PerformAction("js", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01110101")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jnz {destination}");
                    PerformAction("jnz", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01111101")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jnl {destination}");
                    PerformAction("jnl", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01111111")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jg {destination}");
                }
                else if(instruction.Substring(0, 8) == "01110011")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jnb {destination}");
                    PerformAction("jnb", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01110111")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jnbe {destination}");
                    PerformAction("jnbe", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01111011")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jnp {destination}");
                    PerformAction("jnp", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01110001")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jno {destination}");
                    PerformAction("jno", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "01111001")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jns {destination}");
                    PerformAction("jns", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "11100010")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"loop {destination}");
                    PerformAction("loop", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "11100001")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"loopz {destination}");
                    PerformAction("loopz", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "11100000")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"loopnz {destination}");
                    PerformAction("loopnz", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }
                else if(instruction.Substring(0, 8) == "11100011")
                {
                    var destination = Convert.ToUInt16(secondByte, 2).ToString();
                    Console.Write($"jcxz {destination}");
                    PerformAction("jcxz", ref registers, destination, string.Empty, originalIP.ToString("x"), ref currentIP, ref memory);
                }

                Console.WriteLine();
            }
        }
    }
    
    Console.WriteLine();
    registers["ip"] = currentIP.ToString("x");
    Console.Write("Final registers:\n");
    UInt16 memoryIndex = 0;
    foreach(var key in registers.Keys)
    {
        //Console.WriteLine(key);
        //Console.WriteLine(registers[key]);
        if(registers[key] == "0x0" || registers[key] == "0" || registers[key] == "")
        {
            memory[memoryIndex] = "0000";
            ++memoryIndex;
            continue;
        }
        else if(!key.Contains("flags"))
        {
            Console.Write($"\t\t{key}: 0x{registers[key].PadLeft(4, '0')} ({int.Parse(registers[key], System.Globalization.NumberStyles.HexNumber ).ToString()})\n");
            memory[memoryIndex] = registers[key];
            ++memoryIndex;
        }
        else
        {
            Console.Write($"\t{key}: {registers[key]}\n");
        }
    }

    Console.WriteLine();
    
    using (var stream = File.Open("test.data", FileMode.Create))
    {
        using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, false))
        {
            for(int index = 0; index < memory.Count; ++index)
            {
                if(index % 64 == 0 && index > 0)
                {
                    writer.Write('\n');
                    //Console.WriteLine();
                }
                var binary = Convert.ToString(UInt16.Parse(memory[index], System.Globalization.NumberStyles.HexNumber), 2).PadLeft(8, '0');
                //Console.Write($"{binary} ");
                writer.Write(binary);
            }
        }
    }

    using (var stream = File.Open("testHardcoded.data", FileMode.Create))
    {
        using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, false))
        {
            for(int index = 0; index < memory.Count; ++index)
            {
                if(index % 64 == 0 && index > 0)
                {
                    //writer.Write('\n');
                    //Console.WriteLine();
                }
                if(index != 5)
                {
                    var binary = Convert.ToByte(UInt16.Parse(memory[index], System.Globalization.NumberStyles.HexNumber));
                    //Console.Write($"{binary} ");
                    writer.Write(binary);
                }
            }
        }
    }
}

bool DoesHexNumberHaveLeadingBit(string hex)
{
   return hex[0] != '0' &&
            hex[0] != '1' && 
            hex[0] != '2' && 
            hex[0] != '3' && 
            hex[0] != '4' && 
            hex[0] != '5' &&
            hex[0] != '6' && 
            hex[0] != '7';
}

void PerformAction(string action, ref Dictionary<string, string> registers, string destination, string source, string originalIPString, ref UInt16 currentIP, ref List<string> memory)
{
    var currentIPString = currentIP.ToString("x");
    switch(action)
    {
        case "mov":
        {
            string originalHex = "";
            var destinationContainsANumber = destination.Contains('[');
            UInt16 destinationValue = 0;
            var destinationContainsALetter = destination.Contains('a') || 
             destination.Contains('b') ||
              destination.Contains('c') ||
               destination.Contains('x') ||
                destination.Contains('i');
            //Console.WriteLine($"\n{destination}");
            bool containsComplexWord = false;
            if(destinationContainsANumber && !destinationContainsALetter)
            {
                //Console.Write($" {destination.Substring(1, destination.Length - 2)} ");
                var tokens = destination.Split('[');
                var lastToken = tokens[tokens.Length - 1];
                destinationValue = UInt16.Parse(lastToken.Substring(0, lastToken.Length - 1));
                originalHex = memory.ElementAt(destinationValue);
            }
            // This should be for when we see a letter and a number with a word in front.
            else if(destination.Contains("word") && destination.Contains('+'))
            {
                //Console.WriteLine("Forgot to handle this zzz");
                containsComplexWord = true;
                var tokens = destination.Split('+');
                var firstToken = tokens[0].Split('[')[1].Trim();
                var secondToken = tokens[1].Split(']')[0].Trim();
                // TODO: This assumed the register value is the first token.
                var isSecondTokenANumber = UInt16.TryParse(secondToken, out UInt16 secondTokenValue);
                int outcome = 0;
                if(isSecondTokenANumber)
                {
                    outcome = UInt16.Parse(registers[firstToken], System.Globalization.NumberStyles.AllowHexSpecifier) + secondTokenValue;
                }
                else
                {
                    outcome = UInt16.Parse(registers[firstToken], System.Globalization.NumberStyles.AllowHexSpecifier) + UInt16.Parse(registers[secondToken], System.Globalization.NumberStyles.AllowHexSpecifier);
                }
                destinationValue = (UInt16)outcome;
            }
            else if(destination.Contains('+'))
            {
                containsComplexWord = true;
                var tokens = destination.Split('+');
                var firstToken = tokens[0].Split('[')[1].Trim();
                var secondToken = tokens[1].Split(']')[0].Trim();
                // TODO: This assumed the register value is the first token.
                var isSecondTokenANumber = UInt16.TryParse(secondToken, out UInt16 secondTokenValue);
                int outcome = 0;
                if(isSecondTokenANumber)
                {
                    outcome = UInt16.Parse(registers[firstToken], System.Globalization.NumberStyles.AllowHexSpecifier) + secondTokenValue;
                }
                else
                {
                    outcome = UInt16.Parse(registers[firstToken], System.Globalization.NumberStyles.AllowHexSpecifier) + UInt16.Parse(registers[secondToken], System.Globalization.NumberStyles.AllowHexSpecifier);
                }
                destinationValue = (UInt16)outcome;
            }
            else if(destination.Contains('['))
            {
                var tokens = destination.Split("word ");
                var content = tokens[1].Substring(1, tokens[1].Length - 2);
                destinationValue = UInt16.Parse(registers[content], System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            else
            {
                originalHex = registers[destination];
            }
            var isNumber = UInt16.TryParse(source, out UInt16 sourceValue);
            //Console.WriteLine($"\n{source} {sourceValue}");
            if(!isNumber && !source.Contains('['))
            {
                sourceValue = UInt16.Parse(registers[source], System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            else if(source.Contains("word"))
            {
                var tokens = source.Split('[');
                var contents = tokens[1].Substring(0, tokens[1].Length - 1).Trim();
                var registerValues = contents.Split('+');
                var addressValue = UInt16.Parse(registers[registerValues[0].Trim()], System.Globalization.NumberStyles.AllowHexSpecifier) + UInt16.Parse(registers[registerValues[1].Trim()], System.Globalization.NumberStyles.AllowHexSpecifier);
                sourceValue = UInt16.Parse(memory.ElementAt(addressValue), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            else if(source.Contains('['))
            {
                var tokens = source.Split('[');
                var addressValue = tokens[1].Substring(0, tokens[1].Length - 1).Trim();
                sourceValue = UInt16.Parse(memory.ElementAt(UInt16.Parse(addressValue)), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            
            var hex = (sourceValue).ToString("x");
            if(!destinationContainsANumber && !containsComplexWord)
            {
                registers[destination] = hex;
                if(!originalHex.Equals(hex))
                //Console.WriteLine($"\nWhat is the hex? {hex} {destination} {sourceValue} {registers[destination]}");
                {
                    Console.Write($" ; {destination}:0x{originalHex}->0x{hex} ");
                }
                else
                {
                    Console.Write($" ; ");
                }
            }
            else
            {
                memory[destinationValue] = hex;
                //Console.WriteLine($"\n{memory[destinationValue]} {sourceValue}");
                Console.Write(" ; ");
            }
            Console.Write($"ip:0x{originalIPString}->0x{currentIPString}");
        } break;
        case "sub":
        {
            var originalFlags = registers["flags"];
            var originalHex = registers[destination];
            var destinationValue = UInt16.Parse(registers[destination], System.Globalization.NumberStyles.AllowHexSpecifier);
            var isNumber = UInt16.TryParse(source, out UInt16 sourceValue);
            if(!isNumber)
            {
                sourceValue = UInt16.Parse(registers[source], System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            var newFlags = "";
            //Console.Write($" sub test {destinationValue} < {sourceValue} ");
            if(destinationValue < sourceValue)
            {
                newFlags += "C";
            }
            UInt16 difference = (UInt16)(destinationValue - sourceValue);
            var hex = (difference).ToString("x");
            //Console.WriteLine($"{difference} {hex}");
            int counter = 0;
            int evenCount = 0;
            var differenceString = Convert.ToString(difference, 2);
            for(int i = differenceString.Length - 1; i > -1; --i)
            {
                if(differenceString[i] == '1')
                {
                    ++evenCount;
                }
                if(counter++ >= 7)
                {
                    break;
                }
            }
            //Console.Write($" {differenceString} {evenCount} ");
            if(evenCount % 2 == 0)
            {
                newFlags += "P";
            }
            
            if(difference == 0)
            {
                newFlags += "Z";
            }
            else if(DoesHexNumberHaveLeadingBit(hex))
            {
                newFlags += "S";
            }
            
            registers["flags"] = newFlags;

            registers[destination] = hex;
            Console.Write($" ; {destination}:0x{originalHex}->0x{hex} ip:0x{originalIPString}->0x{currentIPString} flags:{originalFlags}->{newFlags}");
        } break;
        case "cmp":
        {
            var originalFlags = registers["flags"];
            var originalHex = registers[destination];
            var destinationValue = UInt16.Parse(registers[destination], System.Globalization.NumberStyles.AllowHexSpecifier);
            var isNumber = UInt16.TryParse(source, out UInt16 sourceValue);
            if(!isNumber)
            {
                sourceValue = UInt16.Parse(registers[source], System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            var difference = destinationValue - sourceValue;
            var hex = (difference).ToString("x");
            var newFlags = "";
            if(difference == 0)
            {
                newFlags = "PZ";
            }
            else if(DoesHexNumberHaveLeadingBit(hex))
            {
                newFlags = "S";
            }
            registers["flags"] = newFlags;

            Console.Write($" ; ip:0x{originalIPString}->0x{currentIPString} flags:{originalFlags}->{newFlags}");
        } break;
        case "add":
        {
            var originalFlags = registers["flags"];
            var originalHex = registers[destination];
            var destinationValue = UInt16.Parse(registers[destination], System.Globalization.NumberStyles.AllowHexSpecifier);
            var isNumber = UInt16.TryParse(source, out UInt16 sourceValue);
            //Console.WriteLine($"\n{destination} {source} {sourceValue}");
            if(!isNumber)
            {
                sourceValue = UInt16.Parse(registers[source], System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            var sum = destinationValue + sourceValue;
            var hex = (sum).ToString("x");
            var newFlags = "";
            bool isCarry = false;
            bool isFirstMismatch = false;
            int j = originalHex.Length - 1;
            for(int i = hex.Length - 1; i > -1 && j > -1; --i)
            {
                if(!isFirstMismatch && hex[i] != originalHex[j])
                {
                    isFirstMismatch = true;
                }
                else if(hex[i] != originalHex[j])
                {
                    isCarry = true;
                    break;
                }
                else
                {
                    //Console.WriteLine(" Can we ever reach this? ");
                    isFirstMismatch = false;
                }
                --j;
            }

            int counter = 0;
            int evenCount = 0;
            var sumString = Convert.ToString(sum, 2);
            for(int i = sumString.Length - 1; i > -1; --i)
            {
                if(sumString[i] == '1')
                {
                    ++evenCount;
                }
                if(counter++ >= 7)
                {
                    break;
                }
            }
            //Console.Write($" {sumString} {evenCount} ");
            if(evenCount % 2 == 0)
            {
                newFlags += "P";
            }

            if(isCarry)
            {
                newFlags += "A";
            }
            registers["flags"] = newFlags;
            registers[destination] = hex;
            Console.Write($" ; ");
            if(originalHex != hex)
            {    
                Console.Write($"{destination}:0x{originalHex}->0x{hex} ip:0x{originalIPString}->0x{currentIPString} ");
            }
            Console.Write($"ip:0x{originalIPString}->0x{currentIPString}");
            //Console.WriteLine($"\nTesting add {originalHex} {hex} {originalFlags} {newFlags} ");
            if((originalFlags.Length > 0 || newFlags.Length > 0) && originalFlags != newFlags)
            {
                Console.Write($" flags:{originalFlags}->{newFlags}");
            }
        } break;
        case "jne":
        {
            if(!registers["flags"].Contains("Z"))
            {
                currentIP = (UInt16)(Int16.Parse(originalIPString, System.Globalization.NumberStyles.HexNumber) + Int16.Parse(destination));
                currentIPString = currentIP.ToString("x");
            }

            Console.Write($" ip:0x{originalIPString}->0x{currentIPString}");
        } break;
        default:
        {
            Console.Write($" Could not handle {action} ");
        } break;
    }
}

string GetREGFieldEncoding(string reg, bool isByteData)
{
    string result = "ERROR";
    //Console.WriteLine($" What is this reg: {reg} {isByteData}");
    switch(reg)
    {
        case "000":
        {
            if(isByteData)
            {
                result = "al";
            }
            else
            {
                result = "ax";
            }
        } break;
        case "001":
        {
            if(isByteData)
            {
                result = "cl";
            }
            else
            {
                result = "cx";
            }
        } break;
        case "010":
        {
            if(isByteData)
            {
                result = "dl";
            }
            else
            {
                result = "dx";
            }
        } break;
        case "011":
        {
            if(isByteData)
            {
                result = "bl";
            }
            else
            {
                result = "bx";
            }
        } break;
        case "100":
        {
            if(isByteData)
            {
                result = "ah";
            }
            else
            {
                result = "sp";
            }
        } break;
        case "101":
        {
            if(isByteData)
            {
                result = "ch";
            }
            else
            {
                result = "bp";
            }
        } break;
        case "110":
        {
            if(isByteData)
            {
                result = "dh";
            }
            else
            {
                result = "si";
            }
        } break;
        case "111":
        {
            if(isByteData)
            {
                result = "bh";
            }
            else
            {
                result = "di";
            }
        } break;
    }
    return(result);
}

string GetRMFieldEncodingValue(string rm, bool isByteData, string mod, List<byte> binaries, ref UInt16 currentIP)
{
    string result = "Error";
    //Console.Write($" {rm} {isByteData} {mod} What is this? ");
    switch(rm)
    {
        case "000":
        {
            if(mod == "11")
            {
                if(isByteData)
                {
                    result = "al";
                }
                else
                {
                    result = "ax";
                }
            }
            else
            {
                result = "[bx+si";
                if(mod == "01")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var value = Convert.ToUInt16(firstByte, 2).ToString();
                    result += $" + {value}";
                }
                else if(mod == "10")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                    ++currentIP;
                    var secondByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var word = secondByte + firstByte;
                    var value = Convert.ToUInt16(word, 2).ToString();
                    result += $" + {value}";
                }
                result += "]";
            }
        } break;
        case "001":
        {
            if(mod == "11")
            {
                if(isByteData)
                {
                    result = "cl";
                }
                else
                {
                    result = "cx";
                }
            }
            else
            {
                result = "[bx+di";
                if(mod == "01")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var value = Convert.ToUInt16(firstByte, 2).ToString();
                    result += $" + {value}";
                }
                else if(mod == "10")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                    ++currentIP;
                    var secondByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var word = secondByte + firstByte;
                    var value = Convert.ToUInt16(word, 2).ToString();
                    result += $" + {value}";
                }
                result += "]";
            }
        } break;
        case "010":
        {
            if(mod == "11")
            {
                if(isByteData)
                {
                    result = "dl";
                }
                else
                {
                    result = "dx";
                }
            }
            else
            {
                result = "[bp+si";
                if(mod == "01")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var value = Convert.ToUInt16(firstByte, 2).ToString();
                    result += $" + {value}";
                }
                else if(mod == "10")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                    ++currentIP;
                    var secondByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var word = secondByte + firstByte;
                    var value = Convert.ToUInt16(word, 2).ToString();
                    result += $" + {value}";
                }
                result += "]";
            }
        } break;
        case "011":
        {
            if(mod == "11")
            {
                if(isByteData)
                {
                    result = "bl";
                }
                else
                {
                    result = "bx";
                }
            }
            else
            {
                result = "[bp+di";
                if(mod == "01")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var value = Convert.ToUInt16(firstByte, 2).ToString();
                    result += $" + {value}";
                }
                else if(mod == "10")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                    ++currentIP;
                    var secondByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var word = secondByte + firstByte;
                    var value = Convert.ToUInt16(word, 2).ToString();
                    result += $" + {value}";
                }
                result += "]";
            }
        } break;
        case "100":
        {
            if(mod == "11")
            {
                if(isByteData)
                {
                    result = "ah";
                }
                else
                {
                    result = "sp";
                }
            }
            else
            {
                result = "[si";
                if(mod == "01")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var value = Convert.ToUInt16(firstByte, 2).ToString();
                    result += $"+{value}";
                }
                else if(mod == "10")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                    ++currentIP;
                    var secondByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var word = secondByte + firstByte;
                    var value = Convert.ToUInt16(word, 2).ToString();
                    result += $"+{value}";
                }
                result += "]";
            }
        } break;
        case "101":
        {
            if(mod == "11")
            {
                if(isByteData)
                {
                    result = "ch";
                }
                else
                {
                    result = "bp";
                }
            }
            else
            {
                result = "[di";
                if(mod == "01")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var value = Convert.ToUInt16(firstByte, 2).ToString();
                    result += $"+{value}";
                }
                else if(mod == "10")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                    ++currentIP;
                    var secondByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var word = secondByte + firstByte;
                    var value = Convert.ToUInt16(word, 2).ToString();
                    result += $"+{value}";
                }
                result += "]";
            }
        } break;
        case "110":
        {
            //Console.Write($" Did I go here? ");
            if(mod == "11")
            {
                //Console.Write($" Did I go here? 1 ");
                if(isByteData)
                {
                    result = "dh";
                }
                else
                {
                    result = "si";
                }
            }
            else
            {
                result = "[";
                if(mod == "00")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                    ++currentIP;
                    var secondByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var word = secondByte + firstByte;
                    var value = Convert.ToUInt16(word, 2).ToString();
                    result = $"[+{value}";
                }
                else if(mod == "01")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var value = Convert.ToUInt16(firstByte, 2).ToString();
                    if(value == "0")
                    {
                        value = "";
                    }
                    else
                    {
                        value = $"+{value}";
                    }
                    result += $"bp{value}";
                }
                else if(mod == "10")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                    ++currentIP;
                    var secondByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var word = secondByte + firstByte;
                    var value = Convert.ToUInt16(word, 2).ToString();
                    if(value == "0")
                    {
                        value = "";
                    }
                    else
                    {
                        value = $"+{value}";
                    }
                    result += $"bp{value}";
                }
                result += "]";
                //Console.Write($"Test {result} ");
            }
        } break;
        case "111":
        {
            if(mod == "11")
            {
                if(isByteData)
                {
                    result = "bh";
                }
                else
                {
                    result = "di";
                }
            }
            else
            {
                result = "[bx";
                if(mod == "01")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var value = Convert.ToUInt16(firstByte, 2).ToString();
                    result += $"+{value}";
                }
                else if(mod == "10")
                {
                    var firstByte = Convert.ToString(binaries[currentIP], 2).PadLeft(8, '0');
                    ++currentIP;
                    var secondByte = Convert.ToString(binaries[currentIP], 2);
                    ++currentIP;
                    var word = secondByte + firstByte;
                    var value = Convert.ToUInt16(word, 2).ToString();
                    result += $"+{value}";
                }
                result += "]";
            }
        } break;
    }

    return(result);
}
