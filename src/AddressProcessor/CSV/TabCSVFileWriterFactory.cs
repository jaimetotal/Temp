namespace AddressProcessing.CSV
{
    public class TabCSVFileWriterFactory : ICSVFileWriterFactory
    {
        public ICSVFileWriter GetInstance(string fileName)
        {
            return new TabCSVFileWriter(fileName);
        }
    }
}