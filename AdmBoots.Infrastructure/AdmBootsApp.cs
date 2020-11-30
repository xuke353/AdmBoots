using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AdmBoots.Infrastructure {

    public class AdmBootsApp {

        /// <summary>
        /// 读取配置文件
        /// </summary>
        public static IConfiguration Configuration { get; private set; }

        /// <summary>
        /// 获得配置文件的对象值
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetJson(string jsonPath, string key) {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile(jsonPath).Build(); //json文件地址
            string s = config.GetSection(key).Value; //json某个对象
            return s;
        }

        public static void SetConfiguration(IConfiguration configuration) {
            Configuration = configuration;
        }
    }
}
