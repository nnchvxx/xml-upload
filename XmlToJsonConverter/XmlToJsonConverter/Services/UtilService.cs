using System.Xml;
using Newtonsoft.Json;
using XmlToJsonConverter.Controllers;

namespace XmlToJsonConverter.Services
{
    public class UtilService : IUtilService
    {
        private readonly string folderPath;
        private readonly string outputFolder;
        private readonly IConfiguration configuration;

        public UtilService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.folderPath = this.configuration.GetValue<string>("FolderPath");
            this.outputFolder = this.configuration.GetValue<string>("OutputFolder");
        }
        public async Task<string> ReadFile(IFormFile file)
        {
            string content = "";
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string firstLine = await reader.ReadLineAsync();
                if (!firstLine.TrimStart().StartsWith("<?xml"))
                {
                    content = firstLine;
                }
                content += await reader.ReadToEndAsync();
            }
            return content;
        }

        public string ConvertXmlToJson(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            string json = JsonConvert.SerializeXmlNode(doc);
            return json;
        }

        public async Task SaveJsonToFile(string json, string filename)
        {
            string directoryPath = Path.Combine(folderPath, outputFolder);
            Directory.CreateDirectory(directoryPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            string filePath = Path.Combine(directoryPath, $"{fileNameWithoutExtension}.json");
            await File.WriteAllTextAsync(filePath, json);
            //await Task.Delay(5000);
        }

        public async Task<byte[]> GetFile(string fileName)
        {
            var filePath = Path.Combine(folderPath, outputFolder, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            var fileContent = await File.ReadAllBytesAsync(filePath);
            return fileContent;
        }

        public IEnumerable<string> ListFiles()
        {
            var result = new List<string>();
            var filesPath = Path.Combine(folderPath, outputFolder);
            var filesNames = Directory.GetFiles(filesPath);
            foreach (var fileName in filesNames)
            {
                result.Add(Path.GetFileName(fileName));
            }

            return result;
        }
    }
}

