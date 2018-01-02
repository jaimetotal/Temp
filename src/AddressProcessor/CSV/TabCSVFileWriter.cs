using System;
using System.IO;

namespace AddressProcessing.CSV
{
    public sealed class TabCSVFileWriter : ICSVFileWriter, IDisposable
    {
        private const string CSVSeparator = "\t";
        private StreamWriter writerStream;
        private bool disposedValue;

        public void Open(string fileName)
        {
            Close();
            FileInfo fileInfo = new FileInfo(fileName);
            writerStream = fileInfo.CreateText();
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
            writerStream?.Close();
        }

        public bool IsOpen { get; private set; }

        public void Write(params string[] columns)
        {
            if (writerStream == null)
            {
                throw new IOException("File not open.");
            }

            // Null columns can still be accepted (for retrocompability) as the end-result will be a newline
            string line = null;
            if (columns != null)
            {
                // NOTE: Here we are going for the most balanced solution, using string.Join
                // Check this if you face an performance issue and decide for StringBuilder instead
                // https://stackoverflow.com/questions/585860/string-join-vs-stringbuilder-which-is-faster
                line = string.Join(CSVSeparator, columns);
            }

            writerStream.WriteLine(line);
        }


        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}