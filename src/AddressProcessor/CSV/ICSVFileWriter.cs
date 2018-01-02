using System;

namespace AddressProcessing.CSV
{
	public interface ICSVFileWriter : IDisposable
	{
		void Write(params string[] columns);
	}
}