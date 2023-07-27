using System;
namespace XmlToJsonConverter.Services
{
    public interface IUtilService
    {
        Task<string> ReadFile(IFormFile file);
        string ConvertXmlToJson(string xml);
        Task SaveJsonToFile(string json, string filename);
        Task<byte[]> GetFile(string fileName);
        IEnumerable<string> ListFiles();
    }
}

