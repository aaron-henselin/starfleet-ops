using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Starfleet.Ops.Infrastructure
{
    public interface IWritableOptions<out T> : IOptionsSnapshot<T> where T : class, new()
    {
        void Update(Action<T> applyChanges);
    }

    public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly IHostingEnvironment _environment;
        private readonly IOptionsMonitor<T> _options;
        private readonly string _section;
        private readonly string _file;

        public WritableOptions(
            IHostingEnvironment environment,
            IOptionsMonitor<T> options,
            string section,
            string file)
        {
            _environment = environment;
            _options = options;
            _section = section;
            _file = file;
        }

        public T Value  {

            get
            {
                var fileProvider = _environment.ContentRootFileProvider;
                var fileInfo = fileProvider.GetFileInfo(_file);
                var physicalPath = fileInfo.PhysicalPath;

                if (!File.Exists(physicalPath))
                    File.WriteAllText(physicalPath, "{}");

                var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));
                return jObject.TryGetValue(_section, out JToken section) ?
                    JsonConvert.DeserializeObject<T>(section.ToString()) : (Value ?? new T());

            }
        }

        public T Get(string name)
        {
            return Value;
        }

        public void Update(Action<T> applyChanges)
        {
            var fileProvider = _environment.ContentRootFileProvider;
            var fileInfo = fileProvider.GetFileInfo(_file);
            var physicalPath = fileInfo.PhysicalPath;

            if (!File.Exists(physicalPath))
                File.WriteAllText(physicalPath,"{}");

            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));
            var sectionObject = jObject.TryGetValue(_section, out JToken section) ?
                JsonConvert.DeserializeObject<T>(section.ToString()) : (Value ?? new T());

            applyChanges(sectionObject);

            jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
            File.WriteAllText(physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
            
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section,
            string file = "datasettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var environment = provider.GetService<IHostingEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new WritableOptions<T>(environment, options, section.Key, file);
            });
        }
    }
}
