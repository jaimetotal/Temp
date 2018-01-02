using System.IO;

namespace AddressProcessing.CSV
{
    public class TabCSVFileWriter : ICSVFileWriter
    {
        private const string CSVSeparator = "\t";
        private readonly StreamWriter writerStream;
        private bool disposedValue;

        public TabCSVFileWriter(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            writerStream = fileInfo.CreateText();
        }

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

            writerStream.WriteLine(line);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    writerStream?.Dispose();
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