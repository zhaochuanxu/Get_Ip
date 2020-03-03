using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
namespace 代理ip
{
    public class Ip
    {
        public string Address{get;set;}
         public string  Port{get;set;}


        //设置构造函数
        public Ip(string Address,string Port)
        {
           this.Address  = Address ;
           this.Port = Port ;
        }
    }

    public class Web_Content{
        public bool Connect{get;set;}
        public string Content{get;set;}
        public Web_Content(bool Connetct,string Content)
        {
            this.Connect = Connect;
            this.Content = Content;
        }


        public Web_Content(){

        }
    }

    class Program
    {

        //传递过来一个List对象


        //返回的是一个 类    
        public static Web_Content Test_Ip(Ip ip,string url)
        {

                //Console.WriteLine(ip.Port);



                //尝试代理Ip  接受请求
                WebProxy proxyObject = new WebProxy(ip.Address, Convert.ToInt32(ip.Port));
               // WebProxy proxyObject = new WebProxy("47.240.100.124");
                //设置请求
                HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(url); // 61.183.192.5

                Req.UserAgent = @"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; 
                    QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET 
                        CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";

                Req.Proxy = proxyObject; //设置代理
                Req.Method = "GET";
                Req.Timeout = 5000;

                Web_Content result = new Web_Content();
                try
                {

                    HttpWebResponse Resp = (HttpWebResponse)Req.GetResponse();
                    Encoding code = Encoding.GetEncoding("UTF-8");
                    using (StreamReader sr = new StreamReader(Resp.GetResponseStream(), code))
                    {
                        string str = null;

                        str = sr.ReadToEnd();//获取得到的网址html返回数据，这里就可以使用某些解析html的dll直接使用了,比如htmlpaser 
                        if (str != null)
                        {
                            result.Connect = true;
                            result.Content =str;
                            return result;
                        }


                    }

                }
                catch (Exception e)
                {
                   result.Content="";
                    result.Connect =false;
                }


           

            
                   result.Content="";
                    result.Connect =false;
                    return result;


        }


        public static string Get_Html(string url)
        {
            try
            {
                //
                #region
                

                Encoding ed = Encoding.UTF8;

                string Html = string.Empty;//初始化新的webRequst
                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);

                Request.KeepAlive = true;
                Request.ProtocolVersion = HttpVersion.Version11;
                Request.Method = "GET";
                Request.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.56 Safari/536.5";
                Request.Referer = url;
                //try
                HttpWebResponse htmlResponse = (HttpWebResponse)Request.GetResponse();
                //从Internet资源返回数据流
                Stream htmlStream = htmlResponse.GetResponseStream();
                //读取数据流
                StreamReader weatherStreamReader = new StreamReader(htmlStream, ed);
                //读取数据
                Html = weatherStreamReader.ReadToEnd();
                weatherStreamReader.Close();
                htmlStream.Close();
                htmlResponse.Close();
                //针对不同的网站查看html源文件

                //Console.WriteLine(Html);

                #endregion
                return Html;  //返回捕捉到的网页字符串
            }


            catch (Exception e)
            {
                //输出当前异常的信息
                Console.WriteLine(e.Message);

                //Console.WriteLine(e.ToString());

                return "";
            }
        }





        static void Main(string[] args)
        {
            /*获取网页内容*/
            try
            {
                //传递的字符串
                string url = "http://www.xicidaili.com/nn/1";
                string Html = Get_Html(url);

                /* //获取代理*/
                #region
                string regex = @"<td>(\d+\.\d+\.\d+\.\d+)</td>\s+<td>(\d+)</td>";
                Match mstr = Regex.Match(Html, regex);

                //c#高效队列 
                //ConcurrentQueue<string> proxyIp = new ConcurrentQueue<string>();

                List<Ip> proxyIp = new List<Ip>();
                //设置dictionary 来存放
                while (mstr.Success)
                {

                    // Response.Write(match.Value + "<br/>");
                    // match = match.NextMatch();
                    //} 获取 
                    /* Console.WriteLine(mstr.Groups[1].Value + ":" + mstr.Groups[2].Value);*/


                    /*存放进高效队列*/

                    proxyIp.Add(new Ip(mstr.Groups[1].Value.ToString(), mstr.Groups[2].Value.ToString()));
                    mstr = mstr.NextMatch();
                }

                #endregion
                //输出获得代理ip List中的个数
                Console.WriteLine("从代理网站获取的Ip个数"+proxyIp.Count);
                //处理获取到的Ip 用来测试网页   输入值为List中的Ip，输出值为爬取的网站
                 #region

                string Catched_url = "https://baidu.com";
                //string Catched_url = @"http://ditu.92cha.com/dizhen.php?dizhen_ly=china&dizhen_zjs=1&dizhen_zje=6&dizhen_riqis=&dizhen_riqie=&ckwz=";
               
                int i = 0;
                foreach (Ip ip in proxyIp)
                {
                    Web_Content result = Test_Ip(ip, Catched_url);
                    if (result.Connect == false)
                    {
                        Console.WriteLine(ip.Address + "不可以用");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine(ip.Address + "可以用");


                        //写入文件
                        //StreamWriter.Write

                        //在将文本写入文件前，处理文本行
                        //StreamWriter一个参数默认覆盖
                        //StreamWriter第二个参数为false覆盖现有文件，为true则把文本追加到文件末尾

                        //StreamWriter(Stream stream);推荐使用，灵活便于理解多态
                        //StreamWriter(string  path);写死了就是指向硬盘写文件
                        /*
                                            //C#文件流写文件,默认追加FileMode.Append 
                                            Console.WriteLine("你爬取的网站为"+Catched_url+"\n"+"请输入你保存文件的名字");
                                            string FileName = Console.ReadLine();
                                            FileName = @"../../"+DateTime.Now.Year.ToString()+"_"+DateTime.Now.Month.ToString()+"_"+DateTime.Now.Day.ToString()+"_"+DateTime.Now.Hour+"_"+DateTime.Now.Minute +"_"+ FileName + ".txt";
                                            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(result.Content);
                                            using (FileStream fsWrite = new FileStream(FileName, FileMode.Append))
                                            {
                                                fsWrite.Write(myByte, 0, myByte.Length);
                                            }; 
                                                    Console.WriteLine("写入完成");
                         * 
                         * */
                        i++;
                        continue;
                    }
                }
                #endregion

                Console.WriteLine(i + "可用");
            }


            catch(Exception e)
            {
                //输出当前异常的信息
                Console.WriteLine(e.Message);

                Console.WriteLine(e.ToString());
            }





            Console.ReadKey();

        }











        }







        #region
        /*

                /// <summary>
            /// 远程获取数据
            /// </summary>
            /// <param name="url">url</param>
            /// <param name="code">编码</param>
            /// <param name="ProxyStr">代理IP，格式：10.20.30.40:8888</param>
            /// <returns></returns>
            public static string SendUrl(string url, Encoding code, string ProxyStr)
            {
                string html = string.Empty;
                try
                {
                    HttpWebRequest WebReques = (HttpWebRequest)HttpWebRequest.Create(url);
                    WebReques.Method = "GET";
                    WebReques.Timeout = 20000;
                    if (ProxyStr.Length > 0)
                    {
                        WebProxy proxy = new WebProxy(ProxyStr, true);
                        WebReques.Proxy = proxy;
                    }

                    HttpWebResponse WebRespon = (HttpWebResponse)WebReques.GetResponse();
                    if (WebRespon != null)
                    {
                        StreamReader sr = new StreamReader(WebRespon.GetResponseStream(), code);
                        html = sr.ReadToEnd();
                        sr.Close();
                        sr.Dispose();
                        WebRespon.Close();
                    }
                }
                catch
                {
            html = “err”;
                }
                return html;
            }



                 public static string doPost(string Url, byte[] postData, SinaCookie bCookie, String encodingFormat, String referer, string ProxyStr)
                {
                        try
                        {
                            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url.ToString());
                            if (ProxyStr != ""&&ProxyStr != null)
                            {
                                //设置代理
                                WebProxy proxy = new WebProxy();
                                proxy.Address = new Uri(ProxyStr);
                                myRequest.UseDefaultCredentials = true;
                                myRequest.Proxy = proxy;
                            }
                            //myRequest.ServicePoint.Expect100Continue = false;
                            myRequest.CookieContainer = bCookie.mycookie;
                            myRequest.Method = "POST";
                            myRequest.Timeout = 30000;






        */
        #endregion

    

            
 
}



  
 