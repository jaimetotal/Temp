using System;

namespace AddressProcessing.CSV
{
	public interface ICSVFileReader : IDisposable
	{
		bool Read(out string column1, out string column2);
	}
}