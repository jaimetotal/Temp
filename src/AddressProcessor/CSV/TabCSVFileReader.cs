using System;
using System.IO;

namespace AddressProcessing.CSV
{
    public sealed class TabCSVFileReader : ICSVFileReader, IDisposable
    {
        private const string CSVSeparator = "\t";
        private StreamReader readerStream;
        private bool disposedValue;

        public void Open(string fileName)
        {
            Close();
            readerStream = File.OpenText(fileName);
            IsOpen = true;
        }

        public void Close()
        {
            readerStream?.Close();
            IsOpen = false;
        }

        public bool IsOpen { get; private set; }

        /// <summary>
        /// Reads a line from the CSV if it has at least one column present and parse it to max of two columns
        /// </summary>
        /// <param name="column1">First column to be parsed</param>
        /// <param name="column2">Second column to be parsed</param>
        /// <returns>One line with a column found</returns>
        public bool Read(out string column1, out string column2)
        {
            if (!IsOpen)
            {
                throw new IOException("File not open.");
            }

            column1 = null;
            column2 = null;

            string line = readerStream.ReadLine();
            if (line == null)
            {
                return false;
            }

            // NOTE: In this case we are going to get all columns.
            // If we have performance issues from loading huge files with too many columns,
            // a regex solution that fetches only the first two columns would be preferable
            string[] columns = line.Split(new[] { CSVSeparator }, StringSplitOptions.None);
            if (columns.Length >= 2)
            {
                column2 = columns[1];
            }

            //Split always returns at least one element in the array, even if the line is empty
            column1 = columns[0];

            return true;
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