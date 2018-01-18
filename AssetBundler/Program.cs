using System;
using System.IO;

namespace AssetBundler
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                FauxMain(args);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("[error] {0} ({1})", e.Message, e.GetType());
                return 1;
            }
        }
        public static void FauxMain(string[] args)
        {
            if (args.Length == 0)
            {
                var asmName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                Console.WriteLine("usage: {0} <input folder> <output file>", asmName);
                return;
            }
            else if (args.Length < 2)
            {
                using (var fs = new FileStream(args[0], FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(fs))
                {
                    int N = reader.ReadInt32();
                    var names = new string[N];
                    var offsets = new long[N];
                    for (int i = 0; i < N; ++i)
                        names[i] = reader.ReadString();
                    for (int i = 0; i < N; ++i)
                        offsets[i] = reader.ReadInt64();
                    for (int i = 0; i < N; ++i)
                        Console.WriteLine("{0:x016}: {1}", offsets[i], names[i]);
                }
                return;
            }
            else if (args.Length > 2)
            {
                Console.WriteLine("[warning] You provided {0} extra argument(s) that will be ignored", args.Length - 2);
            }
            string inFolder = args[0];
            string outFile = args[1];
            if (! Directory.Exists(inFolder))
                throw new Exception("Directory does not exist");
            using (var fs = new FileStream(outFile, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fs))
            {
                var allFiles = Directory.GetFiles(inFolder);
                int N = allFiles.Length;
                int i;
                writer.Write(N);
                foreach (var file in allFiles)
                {
                    writer.Write(Path.GetFileName(file));
                }
                var origin = fs.Position;
                for (i = 0; i < N; ++i)
                {
                    writer.Write(0L);
                }
                var locs = new long[N];
                locs[0] = fs.Position;
                i = 0;
                foreach (var file in allFiles)
                {
                    var fbyte = File.ReadAllBytes(file);
                    if (i + 1 != N)
                    {
                        locs[i + 1] = locs[i] + fbyte.Length + 4;
                    }
                    writer.Write(fbyte.Length);
                    writer.Write(fbyte);
                    ++i;
                }
                fs.Position = origin;
                for (i = 0; i < N; ++i)
                {
                    writer.Write(locs[i]);
                }
            }
        }
    }
}
