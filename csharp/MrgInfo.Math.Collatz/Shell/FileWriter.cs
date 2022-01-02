using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using JetBrains.Annotations;

namespace MrgInfo.Math.Collatz.Shell
{
    sealed class FileWriter: IWriter, IDisposable
    {
        const int _batchSize = 100_000;

        [NotNull]
        readonly string _path;

        [NotNull]
        readonly string _fileName;

        int _fileNumber;

        StreamWriter _output;

        int _line;

        bool _disposed;

        bool _active = true;

        public FileWriter(string path, string fileName)
        {
            if (! Directory.Exists(_path = Path.GetFullPath(path) ?? ".")) throw new ArgumentException("Not found!", nameof(path));
            _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));

            SetOutput();
        }

        void CheckDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(_fileName);
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _output?.Close();
            _disposed = true;
        }

        [SuppressMessage("ReSharper", "ArgumentsStyleOther")]
        void SetOutput()
        {
            StreamWriter output = _output;
            try
            {
                var stream = new FileStream(
                    Path.Combine(_path, $"{_fileName}.{_fileNumber++:000000}.txt"),
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.Read,
                    bufferSize: 10 * 1024 * 1024,
                    useAsync: true);
                _output = new StreamWriter(stream);
            }
            catch (IOException)
            {
                _active = false;
            }
            finally
            {
                try
                {
                    output?.Close();
                }
                catch (IOException)
                { }
            }
        }

        public void Print(SequenceRecord record)
        {
            CheckDisposed();
            try
            {
                _output?.WriteLine(record?.ToString());
            }
            catch (IOException)
            {
                _active = false;
            }
        }

        public void Print(CollatzSequence sequence)
        {
            CheckDisposed();
            if (++_line % _batchSize > 0) return;
            SetOutput();
        }

        public bool Active =>
            _active
            && _output != null
            && _output.BaseStream.CanWrite;

        public void Dispose() => Dispose(true);
    }
}
