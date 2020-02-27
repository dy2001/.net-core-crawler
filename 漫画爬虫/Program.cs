using System;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace 漫画爬虫
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("请输入腾讯漫画的漫画链接地址:");
            Console.WriteLine("例如：https://ac.qq.com/Comic/comicInfo/id/630959");
            string url = Console.ReadLine();
            Console.WriteLine("请输入项目名称:");
            string name = Console.ReadLine();
            Tencent tencent = new Tencent();
            await tencent.Start(url, name);

            Console.ReadKey();
        }
    }
}
