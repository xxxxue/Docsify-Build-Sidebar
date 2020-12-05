using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DocsifyBuildSidebar
{

    public class JsonConfigHelper
    {
        private JObject jObject = null;

        public string this[string key]
        {
            get
            {
                string str = "";
                if (jObject != null)
                {
                    str = GetValue(key);
                }
                return str;
            }
        }
        public JsonConfigHelper(string path)
        {
            jObject = new JObject();
            using (System.IO.StreamReader file = System.IO.File.OpenText(path))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jObject = JObject.Load(reader);
                }
            };
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <example>
        /// 读取一个集合 GetValue<List<object>>("setting") 
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(string key) where T : class
        {
            T result = null;
            var value = jObject.SelectToken(key)?.ToString();
            if (!string.IsNullOrWhiteSpace(value))
            {
               result= JsonConvert.DeserializeObject<T>(value);
            }
            return result;
        }

        /// <summary>
        /// 获取一个字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            string value = jObject.SelectToken(key)?.ToString();
            if (!string.IsNullOrWhiteSpace(value))
            {
                value = Regex.Replace(value, @"\s", "");
            }
            return value;
        }
    }


}
