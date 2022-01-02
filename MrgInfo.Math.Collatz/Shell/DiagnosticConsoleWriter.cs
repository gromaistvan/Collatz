using System;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using static System.Console;
using static System.ConsoleColor;

namespace MrgInfo.Math.Collatz.Shell
{
    sealed class DiagnosticConsoleWriter: IWriter
    {
        public const string Message = "Press Ctrl+Break or Ctrl+C to exit!";

        [NotNull]
        readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        bool _killed;

        public DiagnosticConsoleWriter()
        {
            Title = Message;
            OutputEncoding = Encoding.UTF8 ?? throw new InvalidOperationException();
            CancelKeyPress += delegate { _killed = true;  };
            Clear();
        }

        public void Print(SequenceRecord record)
        {
            ForegroundColor = Cyan;
            foreach (char character in record?.ToString() ?? "")
            {
                if (! Active) break;
                switch (character)
                {
                    case '(':
                        ForegroundColor = Yellow;
                        Write('(');
                        break;
                    case ')':
                        Write(')');
                        ResetColor();
                        break;
                    case '/':
                        ForegroundColor = DarkRed;
                        Write('/');
                        ResetColor();
                        break;
                    case '\\':
                        ForegroundColor = DarkGreen;
                        Write('\\');
                        ResetColor();
                        break;
                    case ':':
                        ResetColor();
                        Write(':');
                        break;
                    default:
                        Write(character);
                        break;
                }
            }
            ResetColor();
            WriteLine();
        }

        public void Print(CollatzSequence sequence)
        {
            Title = $"{Message} [Mem={GC.GetTotalMemory()} | Max={sequence?.Max} | Size={sequence?.Size} | Time={_stopwatch.Elapsed:hh\\:mm\\:ss}]";
            _stopwatch.Restart();
        }

        public bool Active => ! _killed;
    }
}
