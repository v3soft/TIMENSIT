using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models
{
    public class Constant
    {
        public const string ERRORCODE = "101";                                              //lỗi token
        public const string ERRORDATA = "106";                                              //lỗi Data
        public const string ERRORCODETIME = "102";                                          //lỗi về time
        public const string ERRORCODE_SQL = "103";                                          //lỗi sql
        public const string ERRORCODE_FORM = "104";                                         //lỗi về dữ liệu khi post thiếu dl
        public const string ERRORCODE_ROLE = "105";                                         //lỗi về quyền truy cập chức năng
        public const string ERRORCODE_EXCEPTION = "0000";                                   //EXCEPTION

        public static string RootUpload { get { return "dulieu"; } }
        public static int MaxSize { get { return 30000000; } }//maximum file size 30MB
        public const string TEMPLATE_IMPORT_FOLDER = "dulieu/Template";
        public const string ATTACHFILE_YKIEN_FOLDER = "dulieu/dinhkem/YKienXuLy";

        private static Random random = new Random(); // gen mật khẩu mặc định  (đang theo chuẩn của Viettel)
        public static string RandomString(int length)
        {
            string chars1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var Str1 = new string(Enumerable.Repeat(chars1, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            string chars2 = "0123456789";
            var Str2 = new string(Enumerable.Repeat(chars2, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            string chars3 = "!@#$%";
            var Str3 = new string(Enumerable.Repeat(chars3, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            string chars4 = "abcdefghijklmnopqrstvwxyz";
            var Str4 = new string(Enumerable.Repeat(chars4, 5)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return Str1 + Str2 + Str3 + Str4;
        }
        public static IConfigurationRoot getConfig()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
            var root = builder.Build();
            return root;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">ex: JeeWorkConfig:HRConnectionString</param>
        /// <returns></returns>
        public static string getConfig(string code)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
            var root = builder.Build();
            var value = root[code];
            return value;
        }
    }

    public enum StateCode
    {
        NoPermit,
        CannotGetData
    }
}
