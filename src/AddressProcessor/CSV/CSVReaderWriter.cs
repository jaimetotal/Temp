using System;
using System.IO;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    public class CSVReaderWriter : IDisposable
    {
        private const string CSVSeparator = "\t";
        private const string ExceptionModeMessage = "This instance is not in {0} Mode.";
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;

        // We shouldn't keep the flags as we don't support both modes at the same time. That way we can avoid a condition in the Open method
        // NOTE: When adding a new mode, it's necessary to implement support in the Open method
        public enum Mode { Read = 1, Write = 2 };

        public CSVReaderWriter() { }

        public CSVReaderWriter(string fileName, Mode mode)
        {
            Open(fileName, mode);
        }

        public void Open(string fileName, Mode mode)
        {
            if (mode == Mode.Read)
            {
                _readerStream = File.OpenText(fileName);
            }
            else if (mode == Mode.Write)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                _writerStream = fileInfo.CreateText();
            }
        }

        /// <summary>
        /// Appends the line provided 
        /// </summary>
        /// <param name="columns">Columns to be persisted</param>
        public void Write(params string[] columns)
        {
            // Null columns can still be accepted (for retrocompability) as the end-result will be a newline
            string line = null;
            if (columns != null)
            {
                // NOTE: Here we are going for the most balanced solution, using string.Join
                // Check this if you face an performance issue and decide for StringBuilder instead
                // https://stackoverflow.com/questions/585860/string-join-vs-stringbuilder-which-is-faster
                line = string.Join(CSVSeparator, columns);
            }

            WriteLine(line);
        }

        /// <summary>
        /// Reads a line from CSV. This method is obsolete, check <see cref="Read(out string, out string)"/> instead.
        /// </summary>
        /// <param name="column1">Not applicable</param>
        /// <param name="column2">Not applicable</param>
        /// <returns>Returns true if a line was present with two columns. False otherwise</returns>
        [Obsolete]
        public bool Read(string column1, string column2)
        {
            //This method is not useful except to skip a line
            return Read(out column1, out column2);
        }

        /// <summary>
        /// Reads a line from the CSV if it has at least one column present and parse it to max of two columns
        /// </summary>
        /// <param name="column1">First column to be parsed</param>
        /// <param name="column2">Second column to be parsed</param>
        /// <returns>One line with a column found</returns>
        public bool Read(out string column1, out string column2)
        {
            column1 = null;
            column2 = null;

            string line = ReadLine();
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

        private void WriteLine(string line)
        {
            if (_writerStream == null)
            {
                // Depending on the mood and the rest of the project, I would create an exception specific to report this Mode issue
                // An alternative to this would be to ignore if it's not initialized
                throw new InvalidOperationException(string.Format(ExceptionModeMessage, Mode.Write.ToString()));
            }

            _writerStream.WriteLine(line);
        }

        private string ReadLine()
        {
            if (_readerStream == null)
            {
                // Depending on the mood and the rest of the project, I would create an exception specific to report this Mode issue
                // An alternative to this would be to return null if it's not initialized
                throw new InvalidOperationException(string.Format(ExceptionModeMessage, Mode.Read.ToString()));
            }

            return _readerStream.ReadLine();
        }

        public void Close()
        {
            _writerStream?.Close();
            _readerStream?.Close();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
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
        #endregion
    }
}
