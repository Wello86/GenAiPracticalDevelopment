using Azure;
using Azure.AI.OpenAI;
using NSubstitute;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Images;

namespace lab2.tests;

[TestFixture]
public class ImageServiceTests
{
    [Test]
    public void TestAlwaysPasses()
    {
        Assert.Pass();
    }
}