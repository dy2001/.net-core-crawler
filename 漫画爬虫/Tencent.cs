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

namespace 漫画爬虫
{
    public class Tencent
    {
        WebClient web = new WebClient();
        public async Task Start(string url,string name)
        {
            if (!Directory.Exists("腾讯漫画"))
            {
                Directory.CreateDirectory("腾讯漫画");
            }//不存在腾讯漫画文件夹则创建
            var comicInfos = await GetComicInfo(url);//获取漫画每集的标题和链接
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            var page = await browser.NewPageAsync();
            string JiName = "";
            for (int i = 0; i < comicInfos.Count; i++)
            {
                JiName = comicInfos[i].Name;
                for (int ji = 0; ji < JiName.Length; ji++)
                {
                    if (JiName[ji] == '\\' ||
                        JiName[ji] == '/' ||
                        JiName[ji] == '"' ||
                        JiName[ji] == '*' ||
                        JiName[ji] == '?' ||
                        JiName[ji] == '*' ||
                        JiName[ji] == '<' ||
                        JiName[ji] == '>' ||
                        JiName[ji] == '|' ||
                        JiName[ji] == '-' ||
                        JiName[ji] == ':'
                        )
                    {
                        JiName = JiName.Remove(ji, 1);
                        JiName = JiName.Insert(ji, "0");
                    }
                    comicInfos[i].Name = JiName;
                }//如果名字里面含有 \ / " * ? * < > | : 违法字符则改为0，否则windows不允许创建。

                if (!Directory.Exists("腾讯漫画/" + name + "/" + i + "-" + comicInfos[i].Name))
                {
                    Directory.CreateDirectory("腾讯漫画/" + name + "/" + i + "-" + comicInfos[i].Name);
                }//不存在文件夹则创建（说明没有下载过这一集）
                else if (Directory.GetFileSystemEntries("腾讯漫画/" + name + "/" + i + "-" + comicInfos[i].Name).Length < 1)
                {
                    Directory.Delete("腾讯漫画/" + name + "/" + i + "-" + comicInfos[i].Name, true);
                    Directory.CreateDirectory("腾讯漫画/" + name + "/" + i + "-" + comicInfos[i].Name.Trim());
                }//存在文件夹但是文件时空的则删除在此创建（说明上次该下这集并创建了文件夹但是关闭了程序）
                else
                {
                    continue;
                }//文件夹存在并且不为空则跳过（以上结尾断点下载功能）

                await page.GoToAsync(comicInfos[i].Url);
                var html = await page.GetContentAsync();
                HtmlParser parser = new HtmlParser();       //创建解析器
                var doc = parser.ParseDocument(html);       //格式化
                var count = doc.QuerySelectorAll("div#mainView>ul#comicContain > li").Count();//获取li的数量也就是有几张图片
                Console.WriteLine($"---------正在加载{name}第{i+1}集{comicInfos[i].Name}图片------");
                for (int j = 0; j < count * 25; j++)
                {
                    await page.Keyboard.DownAsync("ArrowDown");//模拟按下键盘↓方向键一张图按25次，所以是count*25
                    System.Threading.Thread.Sleep(50);
                }
                html = await page.GetContentAsync();//重新获取加载完成的html
                HtmlParser parser1 = new HtmlParser();
                var doc1 = parser.ParseDocument(html);
                var imginfo= doc1.QuerySelectorAll("div#mainView>ul#comicContain > li > img");

                for (int a = 0; a < imginfo.Count(); a++)
                {
                    await DownLoad(imginfo[a].GetAttribute("src"), "腾讯漫画/" + name + "/" + i + "-" + comicInfos[i].Name+"/图"+a+".jpg");
                    Console.WriteLine($"正在下载{name}第{i+1}集第{a+1}张(请勿关闭程序)");
                }
            }
            await browser.CloseAsync();
            Console.WriteLine("------------ 已 经 全 部 下 载 完 成 ！ --------");//结束
        }

        //获取漫画每集的标题和链接
        private async Task<List<ComicInfo>> GetComicInfo(string url)
        {
            var html = await web.DownloadStringTaskAsync(url);
            HtmlParser parser = new HtmlParser();       //创建解析器
            var doc = parser.ParseDocument(html);       //格式化
            var numberinfo = doc.QuerySelectorAll("ol.chapter-page-all.works-chapter-list > li > p > span >a");//拿到每一集的信息
            List<ComicInfo> comicInfos = new List<ComicInfo>();
            for (int i = 0; i < numberinfo.Count(); i++)
            {
                comicInfos.Add(new ComicInfo
                {
                    Name = numberinfo[i].Text(),
                    Url = numberinfo[i].GetAttribute("href")
                });
            }
            return comicInfos;
        }
        private async Task DownLoad(string url,string filepath)
        {
            await web.DownloadFileTaskAsync(url, filepath);
        }
    }
    class ComicInfo
    {
        string name;
        public string Name
        {
            get
            {
                return name.Trim();
            }
            set
            {
                name = value;
            }
        }
        string url;
        public string Url 
        { 
            get 
            { 
                return url; 
            } 
            set
            {
                url = "https://ac.qq.com" + value;
            }
        }
    }
}
