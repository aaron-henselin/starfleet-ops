using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Starfleet.Ops.Utility
{
    public class JsonDataReader
    {
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