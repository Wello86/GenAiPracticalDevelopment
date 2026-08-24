using Azure;
using Azure.AI.OpenAI;
using Lab1.Abstractions;
using Lab1.Services;
using NSubstitute;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Images;

namespace lab1.tests;

[TestFixture]
public class ImageServiceTests
{
    [Test]
    public void TestAlwaysPasses()
    {
        Assert.Pass();
    }
}