using System.IO;
using AddressProcessing.CSV;
using AddressProcessing.Tests.NUnit;
using NUnit.Framework;

namespace Csv.Tests
{
    [TestFixture]
    public class TabCSVFileWriterTests
    {
        private const string NewWriteFile = @".\tabwriter_file.csv";

        [Test]
        public void Default_Constructor_Should_BeAvailable()
        {
            new TabCSVFileWriter();
        }

        [Test]
        public void IsOpen_Should_BeTrue_When_OpeningValidFile()
        {
            using (var writer = new TabCSVFileWriter())
            {
                writer.Open(NewWriteFile);
                Assert.True(writer.IsOpen);
            }
        }

        [Test]
        public void IsOpen_Should_BeFalse_When_NoOpeningAFile()
        {
            using (var writer = new TabCSVFileWriter())
            {
                Assert.False(writer.IsOpen);
            }
        }

        [Test]
        public void IsOpen_Should_BeFalse_When_Closed()
        {
            var writer = new TabCSVFileWriter();
            writer.Open(NewWriteFile);
            writer.Close();
            Assert.False(writer.IsOpen);
        }

        [Test]
        public void IsOpen_Should_BeFalse_When_Disposed()
        {
            var writer = new TabCSVFileWriter();
            using (writer)
            {
                writer.Open(NewWriteFile);
                Assert.True(writer.IsOpen);
            }

            Assert.False(writer.IsOpen);
        }

        [StringTestCase(new string[] { }, "\r\n")]
        [StringTestCase(new[] { "hello", "world" }, "hello\tworld\r\n")]
        [StringTestCase(null, "\r\n")]
        public string Write_Should_WriteInput_When_InputIsValid(string[] input)
        {
            using (var writer = new TabCSVFileWriter())
            {
                writer.Open(NewWriteFile);
                writer.Write(input);
            }

            return File.ReadAllText(NewWriteFile);
        }

        [Test]
        public void Write_Should_ThrowException_When_FileNotOpen()
        {
            using (var writer = new TabCSVFileWriter())
            {
                Assert.Throws<IOException>(() => writer.Write(null), "File is not open.");
            }
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(NewWriteFile);
        }
    }
}
