using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using XmlToJsonConverter.Controllers;
using XmlToJsonConverter.Services;

namespace XmlToJsonTests
{
    public class FileConvertControllerTests
    {
        private readonly IUtilService utilServiceMock;
        private readonly ILogger<FileConvertController> loggerMock;
        private readonly FileConvertController sut;

        public FileConvertControllerTests()
        {
            utilServiceMock = new Mock<IUtilService>().Object;
            loggerMock = new Mock<ILogger<FileConvertController>>().Object;
            sut = new FileConvertController(utilServiceMock, loggerMock);
        }

        [Fact]
        public async Task UploadFile_ShouldReturnOkResult_WhenFileIsNotNull()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1);
            var result = await sut.UploadFile(mockFile.Object);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}