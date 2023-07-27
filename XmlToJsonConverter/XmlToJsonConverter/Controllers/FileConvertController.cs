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

        [HttpGet("list")]
        public IActionResult ListFiles()
        {
            try
            {
                var filesNames = utilService.ListFiles();
                return Ok(filesNames);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return BadRequest();
            }
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            try
            {
                var fileBytes = await utilService.GetFile(fileName);
                return File(fileBytes, "application/json", fileName);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return NotFound("File is empty.");

            var newKeysOnly = new List<string>();

            foreach (var file in files)
            {
                var fileId = Guid.NewGuid().ToString();
                fileStatuses[fileId] = "Processing";
                newKeysOnly.Add(fileId);

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

                Task.Run(async () => await ProcessFile(fileId, content, file.FileName));
            }
            return Ok(newKeysOnly);
        }

        [HttpPost("status")]
        public IActionResult GetStatus([FromBody] List<string> fileIds)
        {
            var result = new Dictionary<string, string>();
            foreach (var fileId in fileIds)
            {
                if (fileStatuses.TryGetValue(fileId, out var status))
                {
                    result[fileId] = status;
                }
            }
            if (result.Any())
            {
                var asd = result.Select(x => new
                {
                    fileId = x.Key,
                    status = x.Value
                });
                return Ok(asd);
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

