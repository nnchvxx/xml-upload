using System.Xml;
using Newtonsoft.Json;

namespace XmlToJsonConverter.Services
{
    public class UtilService : IUtilService
    {
        private readonly IConfiguration configuration;

        public UtilService(IConfiguration configuration)
        {
            this.configuration = configuration;
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
            var outputFolder = configuration.GetValue<string>("OutputFolder");
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), outputFolder);
            Directory.CreateDirectory(directoryPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            string filePath = Path.Combine(directoryPath, $"{fileNameWithoutExtension}.json");
            await File.WriteAllTextAsync(filePath, json);
            //await Task.Delay(5000);
        }
    }
}

