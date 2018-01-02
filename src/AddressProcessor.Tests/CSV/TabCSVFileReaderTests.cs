using System.IO;
using AddressProcessing.CSV;
using NUnit.Framework;

namespace Csv.Tests
{
    [TestFixture]
    public class TabCSVFileReaderTests
    {
        // NOTE: If file is missing in the target folder, check the output in Solution Explorer properties
        private const string ValidCSVFile = @".\test_data\contacts_reader.csv";
        private const string InvalidCSVFile = @".\test_data\contacts_missing_file.csv";

        [Test]
        public void Default_Constructor_Should_BeAvailable()
        {
            new TabCSVFileReader();
        }

        [Test]
        public void IsOpen_Should_BeFalse_When_Closed()
        {
            var reader = new TabCSVFileReader();
            reader.Open(ValidCSVFile);
            reader.Close();
            Assert.False(reader.IsOpen);
        }

        [Test]
        public void IsOpen_Should_BeFalse_When_Disposed()
        {
            var reader = new TabCSVFileReader();
            using (reader)
            {
                reader.Open(ValidCSVFile);
                Assert.True(reader.IsOpen);
            }

            Assert.False(reader.IsOpen);
        }


        [Test]
        public void IsOpen_Should_BeFalse_When_NoOpeningAFile()
        {
            using (var reader = new TabCSVFileReader())
            {
                Assert.False(reader.IsOpen);
            }
        }

        // Note: Don't think we should cover all the scenarios with file system issues
        [Test]
        public void Open_Should_ThrowException_When_FileDoesntExist_In_ReadMode()
        {
            using (var reader = new TabCSVFileReader())
            {
                Assert.Catch<FileNotFoundException>(() => reader.Open(InvalidCSVFile));
            }
        }

        [Test]
        public void Read_Should_ReturnEmpyColumn_When_LineIsEmpty()
        {
            const string emptyLineFile = @".\test_data\emptyLine.csv";
            File.WriteAllLines(emptyLineFile, new[] {""});
            string column1, column2;

            bool result;
            using (var reader = new TabCSVFileReader())
            {
                reader.Open(emptyLineFile);
                result = reader.Read(out column1, out column2);
            }

            Assert.IsEmpty(column1);
            Assert.IsNull(column2);
            Assert.IsTrue(result);
            File.Delete(emptyLineFile);
        }

        [Test]
        public void Read_Should_ReturnFalse_When_FileIsEmpty()
        {
            const string emptyFile = @".\test_data\emptyFile.csv";
            File.WriteAllLines(emptyFile, new string[] { });
            bool result;

            using (var reader = new TabCSVFileReader())
            {
                reader.Open(emptyFile);
                result = reader.Read(out var _, out var _);
            }

            Assert.IsFalse(result);
            File.Delete(emptyFile);
        }

        [Test]
        public void Read_Should_ReturnValues_When_FileIsValid()
        {
            string column1, column2;
            bool result;
            using (var reader = new TabCSVFileReader())
            {
                reader.Open(ValidCSVFile);
                result = reader.Read(out column1, out column2);
            }

            Assert.AreEqual("Shelby Macias", column1);
            Assert.AreEqual("3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England", column2);
            Assert.IsTrue(result);
        }

        [Test]
        public void Read_Should_ThrowException_When_FileNotOpen()
        {
            using (var reader = new TabCSVFileReader())
            {
                Assert.Throws<IOException>(() => reader.Read(out var _, out var _), "File is not open.");
            }
        }
    }
}