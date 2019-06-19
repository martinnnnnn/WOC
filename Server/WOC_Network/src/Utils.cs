using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Network
{
    public static class Utils
    {
        public static string DataPath;

        public static T FromJson<T>(string jobj) where T : class
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            T obj;
            try
            {
                obj = JsonConvert.DeserializeObject<T>(jobj, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                obj = null;
            }
            return obj;
        }

        public static string ToJson<T>(T obj, bool indent = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}
