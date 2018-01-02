namespace AddressProcessing.CSV
{
	public interface ICSVFileWriter : ICSVFileHandler
	{
		void Write(params string[] columns);
	}

    public interface ICSVFileHandler
    {
        void Open(string fileName);
        void Close();
        bool IsOpen { get; }
    }
}