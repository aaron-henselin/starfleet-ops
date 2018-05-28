using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Starfleet.Ops.Utility
{
    public class JsonFile
    {
        public string Path { get; set; }
        public string Contents { get; set; }
    }

    public class JsonDataReader
    {
        public IReadOnlyCollection<JsonFile> FindJsonFiles(string folder)
        {
            var ret = new List<JsonFile>();
            var allFiles = Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                var contentsRaw = File.ReadAllText(file);
                ret.Add(new JsonFile
                {
                    Path = file,
                    Contents = contentsRaw,



                });
            }

            return ret;
        }

        public IReadOnlyCollection<T> ReadAllSpecifications<T>(string folder)
        {
            var ret = new List<T>();
            var allFiles = Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories);
            foreach (var file in  allFiles)
            {
                var spec = ReadSingleSpecification<T>(file);
                ret.Add(spec);
            }

            return ret;
        }

        public T ReadSingleSpecification<T>(string file)
        {
            var contentsRaw = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<T>(contentsRaw);
        }
    }
}