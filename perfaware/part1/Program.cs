// See https://aka.ms/new-console-template for more information
Console.WriteLine("bits 16");
var fileName = "listing_0037_single_register_mov";
var fileNames = new string[]{fileName, "listing_0038_many_register_mov", "listing_0039_more_movs", "listing_0040_challenge_movs"};
foreach(var aFile in fileNames)
{
    Console.WriteLine(aFile);
    using(var stream = File.Open(aFile, FileMode.Open))
    {
        try
        {
            using(var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, false))
            {
                while(reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var firstByte = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                    var secondByte = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                    Console.Write($"{firstByte} {secondByte} \n");
                    string instruction = firstByte + secondByte; 
                    //Console.WriteLine(instruction);
                    var opcode = instruction.Substring(0, 6);
                    var regFieldEncoding = "ERROR";
                    bool isREGSource = false;
                    bool isByteData = false;
                    bool foundMatch = false;

                    switch(opcode)
                    {
                        case("100010"):
                        {
                            foundMatch = true;
                            Console.Write("mov ");

                            // D part
                            if(instruction[6] == '0')
                            {
                                isREGSource = true;
                            }

                            // W part
                            if(instruction[7] == '0')
                            {
                                isByteData = true;
                            }

                            var mod = instruction.Substring(8, 2);
                            var reg = instruction.Substring(10, 3);
                            switch(reg)
                            {
                                case "000":
                                {
                                    if(isByteData)
                                    {
                                        regFieldEncoding = "al";
                                    }
                                    else
                                    {
                                        regFieldEncoding = "ax";
                                    }
                                } break;
                                case "001":
                                {
                                    if(isByteData)
                                    {
                                        regFieldEncoding = "cl";
                                    }
                                    else
                                    {
                                        regFieldEncoding = "cx";
                                    }
                                } break;
                                case "010":
                                {
                                    if(isByteData)
                                    {
                                        regFieldEncoding = "dl";
                                    }
                                    else
                                    {
                                        regFieldEncoding = "dx";
                                    }
                                } break;
                                case "011":
                                {
                                    if(isByteData)
                                    {
                                        regFieldEncoding = "bl";
                                    }
                                    else
                                    {
                                        regFieldEncoding = "bx";
                                    }
                                } break; 
                                case "100":
                                {
                                    if(isByteData)
                                    {
                                        regFieldEncoding = "ah";
                                    }
                                    else
                                    {
                                        regFieldEncoding = "sp";
                                    }
                                } break;
                                case "101":
                                {
                                    if(isByteData)
                                    {
                                        regFieldEncoding = "ch";
                                    }
                                    else
                                    {
                                        regFieldEncoding = "bp";
                                    }
                                } break;
                                case "110":
                                {
                                    if(isByteData)
                                    {
                                        regFieldEncoding = "dh";
                                    }
                                    else
                                    {
                                        regFieldEncoding = "si";
                                    }
                                } break;
                                case "111":
                                {
                                    if(isByteData)
                                    {
                                        regFieldEncoding = "bh";
                                    }
                                    else
                                    {
                                        regFieldEncoding = "di";
                                    }
                                } break;
                            }

                            var rm = instruction.Substring(13, 3);
                            string source;
                            string destination;
                            //Console.Write($"{opcode} {isREGSource} {isByteData} {mod} {reg} {rm} ");
                            switch(mod)
                            {
                                case "00":
                                {
                                    switch(rm)
                                    {
                                        case "000":
                                        {
                                            if(isREGSource)
                                            {
                                                destination = "[bx + si]";
                                                source = regFieldEncoding;
                                            }
                                            else
                                            {
                                                destination = regFieldEncoding;
                                                source = "[bx + si]";
                                            }

                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "001":
                                        {
                                            var address = GetEffectiveAddressCalculation(rm, mod, reader);
                                            if(isREGSource)
                                            {
                                                destination = address;
                                                source = regFieldEncoding;
                                            }
                                            else
                                            {
                                                destination = regFieldEncoding;
                                                source = address;
                                            }

                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "010":
                                        {
                                            var address = GetEffectiveAddressCalculation(rm, mod, reader);
                                            if(isREGSource)
                                            {
                                                destination = address;
                                                source = regFieldEncoding;
                                            }
                                            else
                                            {
                                                destination = regFieldEncoding;
                                                source = address;
                                            }

                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "100":
                                        {
                                            var address = GetEffectiveAddressCalculation(rm, mod, reader);
                                            if(isREGSource)
                                            {
                                                destination = address;
                                                source = regFieldEncoding;
                                            }
                                            else
                                            {
                                                destination = regFieldEncoding;
                                                source = address;
                                            }

                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "011":
                                        {                                            
                                            if(isREGSource)
                                            {
                                                destination = "[bp + di]";
                                                source = regFieldEncoding;
                                            }
                                            else
                                            {
                                                destination = regFieldEncoding;
                                                source = "[bp + di]";
                                            }

                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "101":
                                        {
                                            var address = GetEffectiveAddressCalculation(rm, mod, reader);
                                            if(isREGSource)
                                            {
                                                destination = address;
                                                source = regFieldEncoding;
                                            }
                                            else
                                            {
                                                destination = regFieldEncoding;
                                                source = address;
                                            }

                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "110":
                                        {
                                            var address = GetEffectiveAddressCalculation(rm, mod, reader);
                                            if(isREGSource)
                                            {
                                                destination = address;
                                                source = regFieldEncoding;
                                            }
                                            else
                                            {
                                                destination = regFieldEncoding;
                                                source = address;
                                            }

                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "111":
                                        {
                                            var address = GetEffectiveAddressCalculation(rm, mod, reader);
                                            if(isREGSource)
                                            {
                                                destination = address;
                                                source = regFieldEncoding;
                                            }
                                            else
                                            {
                                                destination = regFieldEncoding;
                                                source = address;
                                            }

                                            Console.Write($"{destination}, {source}");
                                        } break;
                                    }
                                } break;
                                case "01":
                                {
                                    //Console.Write("This is the spot to watch. ");
                                    var address = GetEffectiveAddressCalculation(rm, mod, reader);
                                    if(isREGSource)
                                    {
                                        destination = address;
                                        source = regFieldEncoding;
                                    }
                                    else
                                    {
                                        destination = regFieldEncoding;
                                        source = address;
                                    }

                                    Console.Write($"{destination}, {source}");
                                } break;
                                case "10":
                                {
                                    //Console.Write("This is the last one: ");
                                    var address = GetEffectiveAddressCalculation(rm, mod, reader);
                                    if(isREGSource)
                                    {
                                        destination = address;
                                        source = regFieldEncoding;
                                    }
                                    else
                                    {
                                        destination = regFieldEncoding;
                                        source = address;
                                    }

                                    Console.Write($"{destination}, {source}");

                                } break;
                                case "11":
                                {
                                    switch(rm)
                                    {
                                        case "000":
                                        {
                                            if(isByteData)
                                            {
                                                if(isREGSource)
                                                {
                                                    Console.Write($"al, {regFieldEncoding}");
                                                }
                                                else
                                                {
                                                    Console.Write($"{regFieldEncoding}, al");
                                                }
                                            }
                                            else
                                            {
                                                if(isREGSource)
                                                {
                                                    Console.Write($"ax, {regFieldEncoding}");
                                                }
                                                else
                                                {
                                                    Console.Write($"{regFieldEncoding}, ax");
                                                }
                                            }
                                        } break;
                                        case "001":
                                        {
                                            if(isByteData)
                                            {
                                                if(isREGSource)
                                                {
                                                    Console.Write($"cl, {regFieldEncoding}");
                                                }
                                                else
                                                {
                                                    Console.Write($"{regFieldEncoding}, cl");
                                                }
                                            }
                                            else
                                            {
                                                if(isREGSource)
                                                {
                                                    Console.Write($"cx, {regFieldEncoding}");
                                                }
                                                else
                                                {
                                                    Console.Write($"{regFieldEncoding}, cx");
                                                }
                                            }
                                        } break;
                                        case "010":
                                        {
                                            if(isByteData)
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "dl";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "dl";
                                                }
                                            }
                                            else
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "dx";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "dx";
                                                }
                                            }
                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "011":
                                        {
                                            if(isByteData)
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "bl";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "bl";
                                                }
                                            }
                                            else
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "bx";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "bx";
                                                }
                                            }
                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "100":
                                        {
                                            if(isByteData)
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "ah";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "ah";
                                                }
                                            }
                                            else
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "sp";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "sp";
                                                }
                                            }
                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "101":
                                        {
                                            if(isByteData)
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "ch";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "ch";
                                                }
                                            }
                                            else
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "bp";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "bp";
                                                }
                                            }
                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "110":
                                        {
                                            if(isByteData)
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "dh";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "dh";
                                                }
                                            }
                                            else
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "si";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "si";
                                                }
                                            }
                                            Console.Write($"{destination}, {source}");
                                        } break;
                                        case "111":
                                        {
                                            if(isByteData)
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "bh";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "bh";
                                                }
                                            }
                                            else
                                            {
                                                if(isREGSource)
                                                {
                                                    destination = "di";
                                                    source = regFieldEncoding;
                                                }
                                                else
                                                {
                                                    destination = regFieldEncoding;
                                                    source = "di";
                                                }
                                            }
                                            Console.Write($"{destination}, {source}");
                                        } break;
                                    }
                                } break;
                            }
                        } break;
                    }

                    if(!foundMatch)
                    {
                        opcode = instruction.Substring(0, 4);
                        var reg = instruction.Substring(5, 3);
                        var value = "ERROR";
                        switch(opcode)
                        {
                            case "1011":
                            {
                                foundMatch = true;
                                Console.Write("mov ");
                                regFieldEncoding = GetREGValue(isByteData, reg);
                                
                                var fullData = instruction.Substring(8);
                                //Console.Write($"What is this: {fullData} ");
                                if(!isByteData)
                                {
                                    var nextByte = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                                    //Console.Write($"Next Byte: {nextByte} ");
                                    fullData = nextByte + fullData;
                                    //Console.Write($"What is this now: {fullData} ");
                                }

                                value = Convert.ToUInt16(fullData, 2).ToString();
                                Console.Write($"{regFieldEncoding}, {value}");
                            } break;
                        }

                        if(!foundMatch)
                        {
                            opcode = instruction.Substring(0, 7);
                            reg = instruction.Substring(5, 3);
                            value = "ERROR";
                            switch(opcode)
                            {
                                case "1100011":
                                {
                                    // W part
                                    if(instruction[7] == '0')
                                    {
                                        isByteData = true;
                                    }

                                    var mod = instruction.Substring(8, 2);
                                    var rm = instruction.Substring(13, 3);
                                    var address = GetEffectiveAddressCalculation(rm, mod, reader);
                                    //Console.Write(address + " testing ");
                                    Console.Write($"mov {address}, ");
                                    var low = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                                    var high = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                                    var data = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                                    Console.Write($"Stuff {high} {low} {data}");
                                    if(!isByteData)
                                    {
                                        var data2 = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                                    }
                                } break;
                            }
                        }
                    }

                    Console.Write("\n");
                }
            }
    }catch(EndOfStreamException ex)
        {
            Console.WriteLine("Finished reading. " + ex);
        }
    }

    Console.WriteLine();
}

// Only for mov immediate to register.
string GetREGValue(bool IsByteData, string RegFieldEncoding)
{
    string value = "ERROR";
    switch(RegFieldEncoding)
    {
        case "000":
        {
            if(IsByteData)
            {
                value = "al"; 
            }
            else
            {
                value = "ax";
            }
        } break;
        case "001":
        {
            if(IsByteData)
            {
                value = "cl"; 
            }
            else
            {
                value = "cx";
            }
        } break;
        case "010":
        {
            if(IsByteData)
            {
                value = "dl"; 
            }
            else
            {
                value = "dx";
            }
        } break;
        case "011":
        {
            if(IsByteData)
            {
                value = "bl"; 
            }
            else
            {
                value = "bx";
            }
        } break;
        case "100":
        {
            if(IsByteData)
            {
                value = "ah"; 
            }
            else
            {
                value = "sp";
            }
        } break;
        case "101":
        {
            if(IsByteData)
            {
                value = "ch"; 
            }
            else
            {
                value = "bp";
            }
        } break;
        case "110":
        {
            if(IsByteData)
            {
                value = "dh"; 
            }
            else
            {
                value = "si";
            }
        } break;
        case "111":
        {
            if(IsByteData)
            {
                value = "bh"; 
            }
            else
            {
                value = "di";
            }
        } break;
    }

    return(value);
}

string GetEffectiveAddressCalculation(string rm, string mod, BinaryReader reader)
{
    string result = "ERROR";

    switch(rm)
    {
        case "000":
        {
            result = "bx + si";
            switch(mod)
            {
                case "01":
                {
                    var currentByte = Convert.ToString(reader.ReadByte(), 2);
                    var value = Convert.ToUInt16(currentByte, 2);
                    //Console.Write("What is this? " + value + " ");
                    result = $"{result} + {value}";
                } break;
                case "10":
                {
                    var low = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                    var high =  Convert.ToString(reader.ReadByte(), 2);
                    var binaryString = high + low;
                    var value = Convert.ToUInt16(binaryString, 2);
                    result = $"{result} + {value}";
                    //Console.Write("What is this 2? " + value + " " + nextValue + " ");
                } break;
            }
        } break;
        case "001":
        {
            result = "bx + di";
            switch(mod)
            {
                case "01":
                {
                    result = $"{result} + {Convert.ToUInt16(Convert.ToString(reader.ReadByte(), 2), 2)}";
                } break;
                case "10":
                {
                    var low = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                    var high =  Convert.ToString(reader.ReadByte(), 2);
                    var binaryString = high + low;
                    var value = Convert.ToUInt16(binaryString, 2);
                    result = $"{result} + {value}";
                } break;
            }
        } break;
        case "010":
        {
            result = "bp + si";
            switch(mod)
            {
                case "01":
                {
                    result = $"{result} + {Convert.ToUInt16(Convert.ToString(reader.ReadByte(), 2), 2)}";
                } break;
                case "10":
                {
                    var low = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                    var high =  Convert.ToString(reader.ReadByte(), 2);
                    var binaryString = high + low;
                    var value = Convert.ToUInt16(binaryString, 2);
                    result = $"{result} + {value}";
                } break;
            }
        } break;
        case "011":
        {
            result = "bp + di";
            switch(mod)
            {
                case "01":
                {
                    result = $"{result} + {Convert.ToUInt16(Convert.ToString(reader.ReadByte(), 2), 2)}";
                } break;
                case "10":
                {
                    var low = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                    var high =  Convert.ToString(reader.ReadByte(), 2);
                    var binaryString = high + low;
                    var value = Convert.ToUInt16(binaryString, 2);
                    result = $"{result} + {value}";
                } break;
            }
        } break;
        case "100":
        {
            result = "si";
            switch(mod)
            {
                case "01":
                {
                    result = $"{result} + {Convert.ToUInt16(Convert.ToString(reader.ReadByte(), 2), 2)}";
                } break;
                case "10":
                {
                    var low = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                    var high =  Convert.ToString(reader.ReadByte(), 2);
                    var binaryString = high + low;
                    var value = Convert.ToUInt16(binaryString, 2);
                    result = $"{result} + {value}";
                } break;
            }
        } break;
        case "101":
        {
            result = "di";
            switch(mod)
            {
                case "01":
                {
                    result = $"{result} + {Convert.ToUInt16(Convert.ToString(reader.ReadByte(), 2), 2)}";
                } break;
                case "10":
                {
                    var low = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                    var high =  Convert.ToString(reader.ReadByte(), 2);
                    var binaryString = high + low;
                    var value = Convert.ToUInt16(binaryString, 2);
                    result = $"{result} + {value}";
                } break;
            }
        } break;
        case "110":
        {
            result = $"bp";
            if(mod == "10")
            {
                var low = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                var high =  Convert.ToString(reader.ReadByte(), 2);
                var binaryString = high + low;
                var value = Convert.ToUInt16(binaryString, 2);
                if(value != 0)
                {
                    result += $" + {value}";
                }
            }
            else if(mod == "01")
            {
                var value = Convert.ToUInt16(Convert.ToString(reader.ReadByte(), 2), 2);
                if(value != 0)
                {
                    result += $" + {value}";
                }
            }
            else if(mod == "00")
            {
                Console.Write("Hello");
                return("[DIRECT ADDRESS]");
            }
        } break;
        case "111":
        {
            result = "bx";
            switch(mod)
            {
                case "01":
                {
                    result = $"{result} + {Convert.ToUInt16(Convert.ToString(reader.ReadByte(), 2), 2)}";
                } break;
                case "10":
                {
                    var low = Convert.ToString(reader.ReadByte(), 2).PadLeft(8, '0');
                    var high =  Convert.ToString(reader.ReadByte(), 2);
                    var binaryString = high + low;
                    var value = Convert.ToInt16(binaryString, 2);
                    result += $" + {value}";
                } break;
            }            
        } break;
    }
    
    result = $"[{result}]";

    return(result);
}