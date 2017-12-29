using NUnit.Framework;

namespace AddressProcessing.Tests.NUnit
{
    /// <summary>
    /// This class allows to pass array as argument to TestCaseAttribute from NUnit
    /// </summary>
    public class StringTestCaseAttribute : TestCaseAttribute
    {
        public StringTestCaseAttribute(string[] array, string expectedResult) : base(array as object)
        {
            ExpectedResult = expectedResult;
        }
    }
}
