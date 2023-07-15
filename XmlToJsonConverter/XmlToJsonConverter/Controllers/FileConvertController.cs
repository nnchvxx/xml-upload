using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using XmlToJsonConverter.Services;

namespace XmlToJsonConverter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileConvertController : Controller
    {
        private static string successMsg = "File saved successfully.";
        private static string processMsg = "File processed successfully.";
        private static ConcurrentDictionary<string, string> fileStatuses = new ConcurrentDictionary<string, string>();
        private readonly IUtilService utilService;
        private readonly ILogger<FileConvertController> logger;

        public FileConvertController(IUtilService utilService, ILogger<FileConvertController> logger)
        {
            this.utilService = utilService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return NotFound("File is empty.");

            var fileId = Guid.NewGuid().ToString();
            fileStatuses[fileId] = "Processing";

            string content;
            try
            {
                content = await utilService.ReadFile(file);
            }
            catch (Exception e)
            {
                var logMsg = "Error reading file.";
                logger.LogError(e.Message);
                fileStatuses[fileId] = $"Error: {logMsg}";
                return BadRequest(logMsg);
            }

            ProcessFile(fileId, content, file.FileName);

            return Ok(fileId);
        }

        [HttpGet("{fileId}")]
        public IActionResult GetStatus(string fileId)
        {
            if (fileStatuses.TryGetValue(fileId, out var status))
            {
                return Ok(new { status });
            }
            else
            {
                return NotFound();
            }
        }

        private async Task ProcessFile(string fileId, string content, string fileName)
        {
            try
            {
                string json;
                try
                {
                    json = utilService.ConvertXmlToJson(content);
                }
                catch (Exception e)
                {
                    var logMsg = "Error: Invalid XML format.";
                    logger.LogError(e.Message);
                    fileStatuses[fileId] = logMsg;
                    return;
                }

                try
                {
                    await utilService.SaveJsonToFile(json, fileName);
                }
                catch (Exception e)
                {
                    var logMsg = "Error writing json file.";
                    logger.LogError(e.Message);
                    fileStatuses[fileId] = $"Error: {logMsg}";
                    return;
                }

                logger.LogInformation(successMsg);
                fileStatuses[fileId] = "Completed";
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                fileStatuses[fileId] = $"Error: {e.Message}";
            }
        }
    }
}

