using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
//using HtmlAgilityPack;
namespace CollaBotFramework
{
    public class responseData
    {
        public string analyzerId { get; set; }
        public string[] result { get; set; }
    }

    public class responseDataPOSTag
    {
        public string analyzerId { get; set; }
        public string[][] result { get; set; }
    }
    public class Data
    {
        public string language { get; set; }
        public string[] analyzerIds { get; set; }
        public string text { get; set; }
        public Data(string input, string lan, string[] ana)
        {
            this.text = input;
            this.language = lan;
            this.analyzerIds = ana;
        }
    }


    public class EntityRecognition
    {
        public Entity[] entities { get; set; }
    }

    public class Entity
    {
        public MatchE[] matches { get; set; }
        public string name { get; set; }
        public string wikipediaId { get; set; }
        public float score { get; set; }
    }

    public class MatchE
    {
        public string text { get; set; }
        public Entry[] entries { get; set; }
    }

    public class Entry
    {
        public int offset { get; set; }
    }

    class DesEntity
    {
        string name { get; set; }
        string wikiID { get; set; }
        string description { get; set; }
        public DesEntity(string inputName, string inputWikiID, string inputDescription)
        {
            this.name = inputName;
            this.wikiID = inputWikiID;
            this.description = inputDescription;
        }
        public override string ToString()
        {
            return string.Format("Name:{0}\nWikiId:{1}\nDescription:{2}", name, wikiID, description);
        }
    }

    class LanguageAnalyze
    {
        /// <summary>
        /// analyzers:
        /// Constituency_Tree: 22A6B758-420F-4745-8A3C-46835A67C0D2
        /// POS_Tags: 4FA79AF1-F22C-408D-98BB-B7D7AEEF7F04
        /// Tokens: 08EA174B-BFDB-4E64-987E-602F85DA7F72
        /// Primary Key: 2f0d5d33cbda4086a12d607ef8695ae6 Regenerate
        /// Secondary Key: 8d5f9557491e4b40a38aedbe5661489d Regenerate
        /// </summary>
        /// <param name="input"></param>

        public static string[] analyzeDependancyTree(string input)
        {
            input = RemovePunctuation(input);
            //input += " and database";
            string url = "https://api.projectoxford.ai/linguistics/v1.0/analyze";
            string[] aids = new string[1];
            aids[0] = "22a6b758-420f-4745-8a3c-46835a67c0d2";
            Data d = new Data(input, "en", aids);
            string jsonData = JsonConvert.SerializeObject(d, Formatting.Indented);
            //Console.WriteLine(jsonData);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Ocp-Apim-Subscription-Key", "2f0d5d33cbda4086a12d607ef8695ae6");
            request.Host = "api.projectoxford.ai";
            byte[] data = Encoding.ASCII.GetBytes(jsonData);
            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.Default);
            string content = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
            response.Close();
            //return content;
            List<responseData> rd = JsonConvert.DeserializeObject<List<responseData>>(content);
            Console.WriteLine(rd[0].result[0]);
            return rd[0].result;
        }
        /// <summary>
        /// Text Search: 
        /// Key 1: 2a0a69b2517f42b98a827a95952e8fa3 
        /// Key 2: a2768bc7bbaf48ab8473944070c7413e 
        /// </summary>
        /// <param name="responseString"></param>
        /// <returns></returns>

        public static string analyzeEntityAPI(string input)
        {
            string url = "https://api.projectoxford.ai/entitylinking/v1.0/link";
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "text/plain";
            request.Headers.Add("Ocp-Apim-Subscription-Key", "ab2340c6103f4f158c8e2b86190fa9c6");
            //request.Host = "api.projectoxford.ai";
            byte[] data = Encoding.ASCII.GetBytes(input);
            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.Default);
            string content = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
            response.Close();
            return content;
        }
        public static string RemovePunctuation(string str)
        {
            str = str.Replace(",", " ")
                              .Replace("，", " ")
                              .Replace(".", " ")
                              .Replace("。", " ")
                              .Replace("!", " ")
                              .Replace("！", " ")
                              .Replace("?", " ")
                              .Replace("？", " ")
                              .Replace(":", " ")
                              .Replace("：", " ")
                              .Replace(";", " ")
                              .Replace("；", " ")
                              .Replace("～", " ")
                              .Replace("-", " ")
                              .Replace("_", " ")
                              .Replace("——", " ")
                              .Replace("—", " ")
                              .Replace("--", " ")
                              .Replace("【", " ")
                              .Replace("】", " ")
                              .Replace("\\", " ")
                              .Replace("(", " ")
                              .Replace(")", " ")
                              .Replace("（", " ")
                              .Replace("）", " ")
                              .Replace("#", " ")
                              .Replace("$", " ");

            return str;
        }

        public static Tuple<string, string[][]> analyzePOSTag(string input)
        {

            input = RemovePunctuation(input);
            Console.WriteLine("input:{0}", input);
            string url = "https://api.projectoxford.ai/linguistics/v1.0/analyze";
            string[] aids = new string[1];
            aids[0] = "4FA79AF1-F22C-408D-98BB-B7D7AEEF7F04";
            Data d = new Data(input, "en", aids);
            string jsonData = JsonConvert.SerializeObject(d, Formatting.Indented);
            //Console.WriteLine(jsonData);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Ocp-Apim-Subscription-Key", "2f0d5d33cbda4086a12d607ef8695ae6");
            request.Host = "api.projectoxford.ai";
            byte[] data = Encoding.ASCII.GetBytes(jsonData);
            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.Default);
            string content = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
            response.Close();
            //return content;
            Console.WriteLine(content);
            List<responseDataPOSTag> rd = JsonConvert.DeserializeObject<List<responseDataPOSTag>>(content);
            //Console.WriteLine(rd[0].result[0]);
            //return rd[0].result[0];

            foreach (string s in rd[0].result[0])
                Console.Write(s + " ");
            Console.WriteLine();
            //Console.WriteLine(rd[0].result[0][0]);
            return new Tuple<string, string[][]>(input, rd[0].result);
        }
        //public static string getTextFromWiki(string url, int num)
        //{
        //    string result = "";
        //    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        //    try
        //    {
        //        using (WebResponse response = request.GetResponse())
        //        {
        //            Stream responseStream = response.GetResponseStream();
        //            StreamReader reader = new StreamReader(responseStream);
        //            string htmlDoc = reader.ReadToEnd();
        //            HtmlDocument doc = new HtmlDocument();
        //            doc.LoadHtml(htmlDoc);
        //            if (doc != null)
        //            {
        //                //Console.WriteLine(doc.DocumentNode.InnerText);
        //                HtmlNode descriptionNode = doc.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']/p");
        //                string descrption = descriptionNode.InnerText;
        //                string[] des_sens = descrption.Split('.');
        //                int stop = des_sens.Length < num ? des_sens.Length : num;
        //                for (int i = 0; i < stop; i++)
        //                    result += des_sens[i] + ".";

        //            }

        //            else
        //            {
        //                Console.WriteLine("doc is null");
        //                result = "null";
        //            }

        //        }
        //    }
        //    catch (WebException wex)
        //    {
        //        Console.WriteLine("Exception in getText from url:{0}, Message:{1}", url, wex.Message);
        //    }
        //    return result;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="response">response from Cognitive Service</param>
        ///// <param name="num">how many sentences extracted from wikipedia</param>
        ///// <returns>A List of Tuples. Each tuple corresponss to an entity, firstElement of Tuple is the name of recognized entity, secondItem is the text</returns>
        //public static List<DesEntity> getDescriptionFromResponse(string response, int num)
        //{
        //    EntityRecognition er = JsonConvert.DeserializeObject<EntityRecognition>(response);
        //    Entity[] eList = er.entities;
        //    List<DesEntity> result = new List<DesEntity>();
        //    foreach (Entity e in eList)
        //    {
        //        string wikiID = e.wikipediaId.Replace(" ", "_");
        //        string name = e.matches[0].text;
        //        string wikiUrl = String.Format("https://en.wikipedia.org/wiki/{0}", wikiID);
        //        string text = getTextFromWiki(wikiUrl, num);
        //        DesEntity de = new DesEntity(name, wikiID, text);
        //        result.Add(de);
        //    }
        //    return result;
        //}

        //public static List<DesEntity> getDescriptionFromInput(string input, int num)
        //{
        //    // get the entities
        //    string response = analyzeEntityAPI(input);
        //    List<DesEntity> des = getDescriptionFromResponse(response, num);
        //    return des;
        //}

        //public static void test()
        //{
        //    List<DesEntity> result_Rutgers = LanguageAnalyze.getDescriptionFromInput("Rutgers University", 2);
        //    foreach (DesEntity de in result_Rutgers)
        //        Console.WriteLine(de);

        //    // Fei-fei Li no result/ CVPR is fine
        //    List<DesEntity> result_CVPR = LanguageAnalyze.getDescriptionFromInput("Fei-fei Li", 2);
        //    foreach (DesEntity de in result_CVPR)
        //        Console.WriteLine(de);
        //}
    }
}
