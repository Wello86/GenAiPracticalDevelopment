using Azure;
using Azure.AI.OpenAI;
using Lab1.Abstractions;
using Lab1.Services;
using NSubstitute;
using NUnit.Framework;
using OpenAI.Chat;

namespace lab1.tests;

[TestFixture]
public class ChatServiceTests
{
    [Test]
    public void TestAlwaysPasses()
    {
        Assert.Pass();
    }
}