using System;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    // NOTE: This code could be refactored to use a proper open source solution but for now, it will just have refactoring
    public class CSVReaderWriter : ICSVFileReader, ICSVFileWriter
	{
	    private readonly ICSVFileReaderFactory csvFileReaderFactory;
	    private readonly ICSVFileWriterFactory csvFileWriterFactory;
	    private const string ExceptionModeMessage = "This instance is not in {0} Mode.";
	    private bool disposedValue; // To detect redundant calls
	    private ICSVFileReader csvFileReader;
	    private ICSVFileWriter csvFileWriter;

        // We shouldn't keep the flags as we don't support both modes at the same time. That way we can avoid an additional condition (and less error usage from the user) in the Open method
        // NOTE: When adding a new mode, it's necessary to implement support in the Open method
        public enum Mode { Read = 1, Write = 2 };

        /// <summary>
        /// Initializes CSVReaderWriter with custom CSVFileReader and CSVFileWriter
        /// </summary>
        /// <param name="csvFileReaderFactory">ICSVFileReaderFactory instance. Mandatory.</param>
        /// <param name="csvFileWriterFactory">ICSVFileWriterFactory instance. Mandatory.</param>
	    public CSVReaderWriter(ICSVFileReaderFactory csvFileReaderFactory, ICSVFileWriterFactory csvFileWriterFactory)
	    {
            // In this case, we are making it mandatory to have both a factory for reader and writer
            // Depending on the requirements, it could be optional
	        if (csvFileReaderFactory == null)
	        {
	            throw new ArgumentNullException(nameof(csvFileReaderFactory));
            }

            if (csvFileWriterFactory == null)
	        {
                throw new ArgumentNullException(nameof(csvFileWriterFactory));
	        }

            this.csvFileReaderFactory = csvFileReaderFactory;
	        this.csvFileWriterFactory = csvFileWriterFactory;
	    }

        /// <summary>
        /// Initializes CSVReaderWriter with CSV (tab separated) file reader and writer capabilities
        /// </summary>
        public CSVReaderWriter() : this(new TabCSVReaderFactory(), new TabCSVFileWriterFactory())
        {

        }

        /// <summary>
        /// Initializes CSVReaderWriter with CSV (tab separated) file reader and writer capabilities
        /// </summary>
        /// <param name="fileName">Filename of file to be parsed</param>
        /// <param name="mode">Opens the file in Read or Write mode</param>
        public CSVReaderWriter(string fileName, Mode mode) : this()
        {
            Open(fileName, mode);
        }

        /// <summary>
        /// Opens the file in either <see cref="Mode.Read"/> which in turn can use <see cref="Read(out string, out string)"/> 
        /// or <see cref="Mode.Write"/> mode, in order to allow <see cref="Write(string[])"/>
        /// </summary>
        /// <param name="fileName">File path to read or create, depending on <paramref name="mode"/></param>
        /// <param name="mode">Read or Write mode allowed</param>
        public void Open(string fileName, Mode mode)
        {
            Close(); // To release any file handler if exists
            if (mode == Mode.Read)
            {
                csvFileReader = csvFileReaderFactory.GetInstance(fileName);
            }
            else if (mode == Mode.Write)
            {
                csvFileWriter = csvFileWriterFactory.GetInstance(fileName);
            }
        }

        /// <summary>
        /// Appends the line provided 
        /// </summary>
        /// <param name="columns">Columns to be persisted</param>
        /// <exception cref="InvalidOperationException">In case the instance's mode is <see cref="Mode.Read"/>.</exception>
        public void Write(params string[] columns)
        {
            if (csvFileWriter == null)
            {
                // NOTE: Depending on the need and the consistency in rest of the project, we can create an exception specific to report this Mode issue
                // An alternative to this would be to ignore if it's not initialized
                throw new InvalidOperationException(string.Format(ExceptionModeMessage, Mode.Write.ToString()));
            }

            csvFileWriter.Write(columns);
        }

        /// <summary>
        /// Reads a line from CSV. This method is obsolete, check <see cref="Read(out string, out string)"/> instead.
        /// </summary>
        /// <param name="column1">Not applicable</param>
        /// <param name="column2">Not applicable</param>
        /// <returns>Returns true if a line was present with two columns. False otherwise</returns>
        /// <exception cref="InvalidOperationException">In case the instance's mode is <see cref="Mode.Write"/>.</exception>
        [Obsolete]
        public bool Read(string column1, string column2)
        {
            // NOTE: This method is not useful except to skip a line
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
            if (csvFileReader == null)
            {
                // NOTE: Depending on the need and the consistency in rest of the project, we can create an exception specific to report this Mode issue
                // An alternative to this would be to return null if it's not initialized
                throw new InvalidOperationException(string.Format(ExceptionModeMessage, Mode.Read.ToString()));
            }

            return csvFileReader.Read(out column1, out column2);
        }

        /// <summary>
        /// Closes any file handler if it was open
        /// </summary>
	    public void Close()
        {
            csvFileWriter?.Dispose();
            csvFileReader?.Dispose();
        }

        #region IDisposable Support
        

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
