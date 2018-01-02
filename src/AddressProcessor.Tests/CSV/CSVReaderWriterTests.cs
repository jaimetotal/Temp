using System;
using System.IO;
using AddressProcessing.CSV;
using AddressProcessing.Tests.NUnit;
using NUnit.Framework;

namespace Csv.Tests
{
    [TestFixture]
    // NOTE: This class covers 96% of the CSVReaderWriter class, with the missing tests being only due to Read method marked as obsolete.
    // We could increase the unit tests number if we test file system scenarios where we have issues but they will be considerated out-of-scope
    // as they feel more of integration tests than anything else
    public class CSVReaderWriterTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(NewWriteFile);
        }

        // NOTE: If file is missing in the target folder, check the output in Solution Explorer properties
        private const string ValidCSVFile = @".\test_data\contacts.csv";
        private const string InvalidCSVFile = @".\test_data\contacts_missing_file.csv";
        private const string NewWriteFile = @".\write_file.csv";


        [StringTestCase(new string[] { }, "\r\n")]
        [StringTestCase(new[] {"hello", "world"}, "hello\tworld\r\n")]
        [StringTestCase(null, "\r\n")]
        public string Write_Should_WriteInput_When_InputIsValid(string[] input)
        {
            using (var writer = new CSVReaderWriter(NewWriteFile, CSVReaderWriter.Mode.Write))
            {
                writer.Write(input);
            }

            return File.ReadAllText(NewWriteFile);
        }

        [Test]
        public void Args_Constructor_Should_BeValid_When_ReadMode()
        {
            using (new CSVReaderWriter(ValidCSVFile, CSVReaderWriter.Mode.Read))
            {
                // It's necessary to dispose it to clear the file handlers.
            }
        }

        [Test]
        public void Args_Constructor_Should_BeValid_When_WriteMode()
        {
            using (new CSVReaderWriter(NewWriteFile, CSVReaderWriter.Mode.Write))
            {
                // It's necessary to dispose it to clear the file handlers.
            }
        }

        // Note: Don't think we should cover all the scenarios with file system issues
        [Test]
        public void Args_Constructor_Should_ThrowException_When_FileDoesntExist_In_ReadMode()
        {
            Assert.Catch<FileNotFoundException>(() => new CSVReaderWriter(InvalidCSVFile, CSVReaderWriter.Mode.Read));
        }

        [Test]
        public void Default_Constructor_Should_BeAvailable()
        {
            new CSVReaderWriter();
        }

        [Test]
        public void Open_Should_BeValid_When_ReadMode()
        {
            using (var reader = new CSVReaderWriter())
            {
                reader.Open(ValidCSVFile, CSVReaderWriter.Mode.Read);
            }
        }

        [Test]
        public void Open_Should_BeValid_When_WriteMode()
        {
            using (var reader = new CSVReaderWriter())
            {
                reader.Open(NewWriteFile, CSVReaderWriter.Mode.Write);
            }
        }

        // Note: Don't think we should cover all the scenarios with file system issues
        [Test]
        public void Open_Should_ThrowException_When_FileDoesntExist_In_ReadMode()
        {
            using (var reader = new CSVReaderWriter())
            {
                Assert.Catch<FileNotFoundException>(() => reader.Open(InvalidCSVFile, CSVReaderWriter.Mode.Read));
            }
        }

        [Test]
        public void Read_Should_ReturnEmpyColumn_When_LineIsEmpty()
        {
            const string emptyLineFile = @".\test_data\emptyLine.csv";
            File.WriteAllLines(emptyLineFile, new[] {""});
            string column1, column2;
            bool result;

            using (var reader = new CSVReaderWriter(emptyLineFile, CSVReaderWriter.Mode.Read))
            {
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

            using (var reader = new CSVReaderWriter(emptyFile, CSVReaderWriter.Mode.Read))
            {
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
            using (var reader = new CSVReaderWriter(ValidCSVFile, CSVReaderWriter.Mode.Read))
            {
                result = reader.Read(out column1, out column2);
            }

            Assert.AreEqual("Shelby Macias", column1);
            Assert.AreEqual("3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England", column2);
            Assert.IsTrue(result);
        }

        [Test]
        public void Read_Should_ThrowException_When_InstanceIsInWriteMode()
        {
            using (var reader = new CSVReaderWriter(NewWriteFile, CSVReaderWriter.Mode.Write))
            {
                Assert.Catch<InvalidOperationException>(() => reader.Read(out string _, out string _),
                    "This instance is not in Read Mode.");
            }
        }

        [Test]
        public void Write_Should_ThrowException_When_InstanceIsInReadMode()
        {
            using (var writer = new CSVReaderWriter(ValidCSVFile, CSVReaderWriter.Mode.Read))
            {
                Assert.Catch<InvalidOperationException>(() => writer.Write(null), "This instance is not in Read Mode.");
            }
        }
    }
}