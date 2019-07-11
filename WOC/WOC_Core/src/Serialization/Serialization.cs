using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Core
{
    public static class Serialization
    {
        public static T FromJson<T>(string jobj) where T : class
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            T obj;
            try
            {
                obj = JsonConvert.DeserializeObject<T>(jobj, settings);
            }
            catch (Exception e)
            {
                LOG.Print(e.Message);
                obj = null;
            }
            return obj;
        }

        public static string ToJson<T>(T obj, bool indent = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //NullValueHandling = NullValueHandling.Include
            };
            var hello = JsonConvert.SerializeObject(obj, settings: settings, formatting: indent ? Formatting.Indented : Formatting.None);
            return hello;
        }
    }
}
