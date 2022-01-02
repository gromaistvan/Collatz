using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using static System.Console;
using static System.Environment;
using static System.StringComparison;

namespace MrgInfo.Math.Collatz.Shell
{
    class Q<TH>
    {
        const double Pi = 3.14;

        readonly double _x = 2*Pi;

        public double Y = 3;

        void Run()
        {
            double x = _x;
            Console.WriteLine(x);
        }
    }

    static class Program
    {
        [NotNull] static CollatzSequence Sequence { get; } = new CollatzSequence();

        [NotNull]
        static string ParameterValue(this IEnumerable<string> args, [NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return args
                ?.FirstOrDefault(arg => arg != null && arg.StartsWith($"/{name}:", OrdinalIgnoreCase))
                ?.Substring(name.Length + 2) ?? "";
        }

        static bool CheckParameter(this IEnumerable<string> args, [NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return args != null && args.Any(arg => arg != null && arg.Equals($"/{name}", OrdinalIgnoreCase));
        }

        static bool Log(Exception exception)
        {
            WriteLine(exception);
            return false;
        }

        static void CreateNumbers(IWriter writer, BigInteger start)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            for (BigInteger n = start; writer.Active; ++n)
            {
                SequenceRecord record = Sequence.Get(n);
                writer.Print(record);
                writer.Print(Sequence);
            }
        }

        [NotNull]
        static Task CreateNumbers(IWriter writer, BigInteger start, int producers)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            var tasks = new Task[producers + 1];
            for (var i = 0; i < producers; i++)
            {
                BigInteger offset = start + i;
                tasks[i] = Task.Run(
                    delegate
                    {
                        for (BigInteger n = offset; writer.Active; n += producers)
                        {
                            Sequence.Get(n);
                        }
                    });
            }
            tasks[producers] = Task.Run(
                delegate
                {
                    for (BigInteger n = start; writer.Active;)
                    {
                        SequenceRecord record = Sequence.Find(n);
                        if (record == null) continue;
                        writer.Print(record);
                        writer.Print(Sequence);
                        ++n;
                    }
                });
            return Task.WhenAny(tasks);
        }

        static void Run(BigInteger start, string path, bool full = false)
        {
            IWriter consoleWriter = full
                ? new DiagnosticConsoleWriter()
                : (IWriter)new EasyConsoleWriter();
            using (var fileWriter = new FileWriter(path ?? ".", "collatz"))
            {
                var writer = new AggregateWriter(fileWriter, consoleWriter);
                CreateNumbers(writer, start);
            }
        }

        static async Task RunAsync(BigInteger start, string path, bool full = false)
        {
            IWriter consoleWriter = full
                ? new DiagnosticConsoleWriter()
                : (IWriter)new EasyConsoleWriter();
            using (var fileWriter = new FileWriter(path ?? ".", "collatz"))
            {
                var writer = new AggregateWriter(fileWriter, consoleWriter);
                await CreateNumbers(writer, start, 1).ConfigureAwait(false);
            }
        }

        static async Task Main(string[] args)
        {
            BigInteger start = BigInteger.TryParse(args.ParameterValue(nameof(start)), out BigInteger n)
                ? n
                : BigInteger.One;
            bool full = args.CheckParameter(nameof(full));
            bool parallel = args.CheckParameter(nameof(parallel));
            string path = GetEnvironmentVariable("WEBROOT_PATH") ?? CurrentDirectory;
            WriteLine($"Output folder is: {path}");
            try
            {
                if (parallel)
                {
                    await RunAsync(start, path, full).ConfigureAwait(false);
                }
                else
                {
                    Run(start, path, full);
                }
            }
            catch (Exception exception) when (Log(exception))
            { }
        }

#pragma warning restore IDE1006 // Naming Styles
    }
}
