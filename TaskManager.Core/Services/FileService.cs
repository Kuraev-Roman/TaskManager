using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services
{
    public class FileService
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public void SaveToFile(List<TaskItem> tasks, string filePath)
        {
            string json = JsonSerializer.Serialize(tasks, _jsonOptions);
            File.WriteAllText(filePath, json);
        }

        public List<TaskItem> LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.", filePath);
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<TaskItem>>(json, _jsonOptions)
                   ?? new List<TaskItem>();
        }
    }
}