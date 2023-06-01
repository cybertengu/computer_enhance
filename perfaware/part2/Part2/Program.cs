// See https://aka.ms/new-console-template for more information
using System.Text;

//Console.WriteLine("Hello, World!");
//Console.Write(args[1]);
var method = args[0];
Console.WriteLine($"Method: {method}");
var seed = Convert.ToInt32(args[1]);
Console.WriteLine($"Random seed: {seed}");
var pairCount = Convert.ToInt32(args[2]);
Console.WriteLine($"Pair count: {pairCount}");
var haversines = new List<double>();
CreateJSON(ref haversines);
//Console.WriteLine("Done with calculating.");
/*
var jsonFile = "results.json";
using (StreamWriter outFile = new StreamWriter(jsonFile, false, Encoding.UTF8, 65536))
{ 
    //  write the 10k lines with .write NOT writeline..
    outFile.Write(result); 
} */

//Console.WriteLine(result);
double sum = 0;
var fileName = "sums.txt";
using (var stream = File.Open(fileName, FileMode.Create))
{
    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
    {
        foreach(var haversine in haversines)
        {
            sum += haversine;
            writer.Write(haversine);
            //Console.Write($"{haversine}\n");
        }
    }
}
var mean = sum / haversines.Count;
Console.WriteLine($"Expected sum: {mean}");

/*
using (var stream = File.Open(fileName, FileMode.Open))
{
    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
    {
        for(int i = 0; i < haversines.Count; ++i)
        {
            Console.WriteLine(reader.ReadDouble());
        }
    }
}
*/
void CreateJSON(ref List<double> haversines)
{
    //string result = "{\"pairs\":[";

    switch(method.ToLower())
    {
        case "cluster":
        {
            //Console.WriteLine("Start of calculation!");
            Random random = new Random(seed);
            var clusters = new List<Part2.Haversine.Point>();
            for(int i = 0; i < 64; ++i)
            {
                Part2.Haversine.Point newPoint = new Part2.Haversine.Point(random.NextDouble() * 360 - 180, random.NextDouble() * 180 - 90, random.NextDouble() * 360 - 180, random.NextDouble() * 180 - 90);
                clusters.Add(newPoint);
            }
            //Console.WriteLine("Done with making numbers.");
            var jsonFile = "results.json";
            using (StreamWriter outFile = new StreamWriter(jsonFile, false, Encoding.UTF8, 65536))
            { 
                outFile.Write("{\"pairs\":[");
                for(int i = 0; i < pairCount;)
                {
                    for(int j = 0; j < clusters.Count; ++j)
                    {
                        //Console.WriteLine(i);
                        double x0 = GetNextDouble(random, clusters[j].X0, clusters[j].X1);
                        double y0 = GetNextDouble(random, clusters[j].Y0, clusters[j].Y1);
                        double x1 = GetNextDouble(random, clusters[j].X0, clusters[j].X1);
                        double y1 = GetNextDouble(random, clusters[j].Y0, clusters[j].Y1);
                        outFile.Write($"{{\"x0\":{x0},\"y0\":{y0},\"x1\":{x1},\"y1\":{y1}}}");
                        if(i < pairCount - 1)
                        {
                            outFile.Write(",");
                        }
                        ++i;
                        if(i >= pairCount)
                        {
                            break;
                        }
                        haversines.Add(Part2.Haversine.ReferenceHaversine(x0, y0, x1, y1));
                    }
                }
                outFile.Write("]}");
            } 
            //Console.WriteLine("This is painful!");
        } break;
        default:
        {
            Console.WriteLine($"Didn't expect this method name: {method}");
        } break;
    }
}

double GetNextDouble(Random random, double x0, double x1)
{
    if(x0 > x1)
    {
        double result = (x0 - x1) * random.NextDouble() + x1;
        return result;
    }
    else
    {
        double result = (x1 - x0) * random.NextDouble() + x0;
        return result;
    }
}