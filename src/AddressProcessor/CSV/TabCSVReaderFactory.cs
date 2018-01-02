namespace AddressProcessing.CSV
{
    public class TabCSVReaderFactory : ICSVFileReaderFactory
    {
        public ICSVFileReader GetInstance(string fileName)
        {
            return new TabCSVFileReader(fileName);
        }
    }
}
