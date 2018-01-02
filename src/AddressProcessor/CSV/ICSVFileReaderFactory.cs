namespace AddressProcessing.CSV
{
    public interface ICSVFileReaderFactory
    {
        ICSVFileReader GetInstance(string fileName);
    }
}