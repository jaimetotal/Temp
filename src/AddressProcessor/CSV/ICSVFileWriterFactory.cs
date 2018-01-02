namespace AddressProcessing.CSV
{
    public interface ICSVFileWriterFactory
    {
        ICSVFileWriter GetInstance(string fileName);
    }
}