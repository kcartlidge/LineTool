using System.Security.Cryptography;
using System.Text;

namespace LineTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("LINE TOOLS");
            Console.WriteLine();
            Console.WriteLine("count   <filename>");
            Console.WriteLine("find    <filename> <out-filename> 'string'");
            Console.WriteLine("extract <filename> <out-filename> <line-1> <line-2>");
            Console.WriteLine();

            try
            {
                if (args.Length == 0) throw new Exception("No arguments provided.");
                var cmd = args[0].Trim().ToUpperInvariant();
                if (cmd != "COUNT" && cmd != "FIND" && cmd != "EXTRACT") throw new Exception($"Unknown command `{cmd}`.");
                if (cmd == "COUNT" && args.Length != 2) throw new Exception("Incorrect number of arguments.");
                if (cmd == "FIND" && args.Length != 4) throw new Exception("Incorrect number of arguments.");
                if (cmd == "EXTRACT" && args.Length != 5) throw new Exception("Incorrect number of arguments.");
                var filename = args[1].Trim();
                if (File.Exists(filename) == false) throw new Exception("The file cannot be found.");

                if (cmd == "COUNT")
                {
                    // ReadLines, unlikes ReadAllLines, doesn't load them all at once at the start.
                    Console.WriteLine("Counting.");
                    var lines = File.ReadLines(filename);
                    Console.WriteLine("Lines: " + lines.Count().ToString("###,###,###,##0"));
                }
                else
                {
                    var outFilename = args[2].Trim();
                    var folder = Path.GetDirectoryName(Path.GetFullPath((outFilename)));
                    if (Directory.Exists(folder) == false) throw new Exception("The destination folder cannot be found.");

                    if (cmd == "EXTRACT")
                    {
                        Console.WriteLine("Extracting.");
                        if (int.TryParse(args[3].Trim(), out int line1) == false) throw new Exception("Line 1 is not an integer.");
                        if (int.TryParse(args[4].Trim(), out int line2) == false) throw new Exception("Line 2 is not an integer.");
                        if (line1 < 1) throw new Exception("Line 1 must be greater than zero.");
                        if (line2 < line1) throw new Exception("Line 2 cannot be before line 1.");

                        // ReadLines, unlikes ReadAllLines, doesn't load them all at once at the start.
                        var lines = File.ReadLines(filename);
                        var idx = 0;
                        var wid = line2.ToString().Length;
                        using (var writer = new StreamWriter(outFilename, false, Encoding.UTF8))
                        {
                            foreach (var line in lines)
                            {
                                if (++idx >= line1) writer.WriteLine(line);
                                var idxStr = idx.ToString().PadRight(wid, ' ');
                                if (idx >= line2) break;
                            }
                            writer.Close();
                        }
                    }
                    else if (cmd == "FIND")
                    {
                        Console.WriteLine("Searching.");
                        var term = args[3].Trim();

                        // ReadLines, unlikes ReadAllLines, doesn't load them all at once at the start.
                        var lines = File.ReadLines(filename);
                        var idx = 0;
                        var matches = 0;
                        using (var writer = new StreamWriter(outFilename, false, Encoding.UTF8))
                        {
                            foreach (var line in lines)
                            {
                                ++idx;
                                if (line.Contains(term, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    matches++;
                                    writer.WriteLine($"{idx}: {line}");
                                }
                            }
                            writer.Close();
                        }
                        Console.WriteLine($"Found {matches}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Done.");
            Console.WriteLine();
        }
    }
}
