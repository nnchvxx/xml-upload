using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using XmlToJsonConverter.Services;

namespace XmlToJsonTests;

public class UtilServiceTest
{
    private readonly IUtilService sut;
    public UtilServiceTest()
    {
        var key = "OutputFolder";
        var value = "SavedJsonFiles";
        var _mockIConfiguration = new Mock<IConfiguration>();
        var mockIConfigurationSection = new Mock<IConfigurationSection>();
        mockIConfigurationSection.Setup(x => x.Key).Returns(key);
        mockIConfigurationSection.Setup(x => x.Value).Returns(value);
        _mockIConfiguration.Setup(x => x.GetSection(key)).Returns(mockIConfigurationSection.Object);

        sut = new UtilService(_mockIConfiguration.Object);
    }

    [Fact]
    public void ConvertXmlToJson_ValidXml_ReturnsJson()
    {
        var json = "{\"root\":{\"child\":\"test\"}}";
        string xml = "<root><child>test</child></root>";
        string jsonResult = sut.ConvertXmlToJson(xml);
        Assert.NotNull(json);
        Assert.True(json == jsonResult);
    }

    [Fact]
    public void ConvertXmlToJson_InvalidXml_ThrowsException()
    {
        string xml = "invalid xml";
        Assert.Throws<XmlException>(() => sut.ConvertXmlToJson(xml));
    }

    [Fact]
    public async Task SaveJsonToFile_ValidJson_SavesToFile()
    {
        string json = "{\"child\": \"test\"}";
        string filename = "test.json";
        await sut.SaveJsonToFile(json, filename);
        Assert.True(File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "SavedJsonFiles", filename)));
        File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "SavedJsonFiles", filename));
    }

    [Fact]
    public async Task ReadFile_ShouldSkipXmlDeclaration()
    {
        var mockFile = new Mock<IFormFile>();
        var content = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<note>\n<to>Test</to>\n</note>";
        var fileName = "test.xml";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;
        mockFile.Setup(_ => _.FileName).Returns(fileName);
        mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);

        var result = await sut.ReadFile(mockFile.Object);
        Assert.Equal("<note>\n<to>Test</to>\n</note>", result);
    }

    [Fact]
    public async Task ReadFile_ShouldReturnAllContent_WhenNoXmlDeclaration()
    {
        var mockFile = new Mock<IFormFile>();
        var content = "<note><to>Test</to></note>";
        var fileName = "test.xml";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;
        mockFile.Setup(x => x.FileName).Returns(fileName);
        mockFile.Setup(x => x.OpenReadStream()).Returns(ms);

        var result = await sut.ReadFile(mockFile.Object);
        Assert.Equal("<note><to>Test</to></note>", result);
    }

    [Fact]
    public async Task ReadFile_ShouldThrowException_WhenFileNull()
    {
        IFormFile nullFile = null;
        await Assert.ThrowsAsync<NullReferenceException>(() => sut.ReadFile(nullFile));
    }
}
