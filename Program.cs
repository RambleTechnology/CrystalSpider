using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Net;
using VscodeSpider.Models;
using VscodeSpider.PublicClass;
using System.Threading;

namespace VscodeSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // CityCrawler();
            CrystalSpider();
            Console.WriteLine("成功："+i);
        }

        /// <summary>
        /// 抓取城市列表
        /// </summary>
        public static void CityCrawler() {
            
            var cityUrl = "http://hotels.ctrip.com/citylist";//定义爬虫入口URL
            var cityList = new List<City>();//定义泛型列表存放城市名称及对应的酒店URL
            var cityCrawler = new SimpleCrawler();//调用刚才写的爬虫程序
            cityCrawler.OnStart += (s, e) =>
            {
                Console.WriteLine("爬虫开始抓取地址：" + e.Uri.ToString());
            };
            cityCrawler.OnError += (s, e) =>
            {
                Console.WriteLine("爬虫抓取出现错误：" + e.Uri.ToString() + "，异常消息：" + e.Exception.Message);
            };
            cityCrawler.OnCompleted += (s, e) =>
            {
                //使用正则表达式清洗网页源代码中的数据
                var links = Regex.Matches(e.PageSource, @"<a[^>]+href=""*(?<href>/hotel/[^>\s]+)""\s*[^>]*>(?<text>(?!.*img).*?)</a>", RegexOptions.IgnoreCase);
                foreach (Match match in links)
                {
                    var city = new City
                    {
                        CityName = match.Groups["text"].Value,
                        Uri = new Uri("http://hotels.ctrip.com" + match.Groups["href"].Value
                    )
                    };
                    if (!cityList.Contains(city)) cityList.Add(city);//将数据加入到泛型列表
                    Console.WriteLine(city.CityName + "|" + city.Uri);//将城市名称及URL显示到控制台
                }
                Console.WriteLine("===============================================");
                Console.WriteLine("爬虫抓取任务完成！合计 " + links.Count + " 个城市。");
                Console.WriteLine("耗时：" + e.Milliseconds + "毫秒");
                Console.WriteLine("线程：" + e.ThreadId);
                Console.WriteLine("地址：" + e.Uri.ToString());
            };
            cityCrawler.Start(new Uri(cityUrl)).Wait();//没被封锁就别使用代理：60.221.50.118:8090
        }

        //定义全局变量，记录成功次数
        static int i=0;
        /// <summary>
        /// 爬取水晶DJ网
        /// </summary>
        public static void CrystalSpider(){
            var  baseurl="http://m.oscaches.com/mp4/djmusic/myxc/20170824/";
            // var  url="http://m.oscaches.com/mp4/djmusic/myxc/20170824/8.mp4";
            for (int i = 0; i < 100; i++)
            {
                string  url=baseurl+i+".mp4";
                string name=i+".mp3";
                Console.WriteLine("开始爬取：当前url："+url+"，当前文件名称："+name);
                Do(url,name);
               
            }            
        }
        
        /// <summary>
        /// 执行爬取
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        private static void Do(string url,string name){
            try
            {
                 WebRequest request = WebRequest.Create(url);  
                WebResponse response = request.GetResponse();  
                Stream reader = response.GetResponseStream();                 
                FileStream writer = new FileStream(@"E:\cml\mm\"+name, FileMode.OpenOrCreate, FileAccess.Write);  
                byte[] buff = new byte[512];  
                int c = 0; 
                while ((c = reader.Read(buff, 0, buff.Length)) > 0)  
                {  
                    writer.Write(buff, 0, c);  
                }  
                writer.Close();  
                writer.Dispose();  
                reader.Close();  
                reader.Dispose();  
                response.Close(); 
                 Console.WriteLine("爬取完毕：当前url："+url+"，当前文件名称："+name);
                Thread.Sleep(5000);      
                i++;
            }
            catch (System.Exception)
            {
                Console.WriteLine("爬取失败，报异常了：当前url："+url+"，当前文件名称："+name);
            }                        
        }


    }
}
