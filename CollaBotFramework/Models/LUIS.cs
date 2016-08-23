using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace CollaBotFramework
{
    public enum INTENT { FindUser, FindProject, FindTeam, Chat };
    public class LUISObject
    {
        public string query { get; set; }
        public Intent[] intents { get; set; }
        public KEntity[] entities { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

    public class KEntity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
    }

    public class LUIS
    {
        public static LUISObject analyze(string input)
        {

            Console.WriteLine("input:{0}", input);
            string url = string.Format("https://api.projectoxford.ai/luis/v1/application?id=d0161fe8-16ac-41b8-8103-d43ab44dbf85&subscription-key=d9aa4c3769bd495eb66204ee51dea8e2&q={0}", Uri.EscapeUriString(input));
            //string[] aids = new string[1];
            //aids[0] = "4FA79AF1-F22C-408D-98BB-B7D7AEEF7F04";
            //Data d = new Data(input, "en", aids);
            //string jsonData = JsonConvert.SerializeObject(d, Formatting.Indented);
            //Console.WriteLine(jsonData);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";
            //request.Headers.Add("Ocp-Apim-Subscription-Key", "2f0d5d33cbda4086a12d607ef8695ae6");
            request.Host = "api.projectoxford.ai";
            //byte[] data = Encoding.ASCII.GetBytes(input);
            //request.ContentLength = data.Length;
            //Stream requestStream = request.GetRequestStream();
            //requestStream.Write(data, 0, data.Length);
            //requestStream.Close();
            request.ContentType = "text/xml; encoding='utf-8'";
            request.UseDefaultCredentials = true;
            WebResponse response = request.GetResponse();
            //Stream dataStream = response.GetResponseStream();
            //HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.Default);
            string content = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
            response.Close();
            //return content;
            Console.WriteLine(content);
            return JsonConvert.DeserializeObject<LUISObject>(content);
        }


        public static HashSet<string> getNP(string input, out INTENT userIntent)
        {
            LUISObject LO = analyze(input);
            userIntent = INTENT.Chat;
            Intent[] ins = LO.intents;
            string firstIn = ins[0].intent;
            if (firstIn.Equals("FindUser"))
                userIntent = INTENT.FindUser;
            else if (firstIn.Equals("FindProject"))
                userIntent = INTENT.FindProject;
            else if (firstIn.Equals("FormTeam"))
                userIntent = INTENT.FindTeam;


            KEntity[] ens = LO.entities;
            HashSet<string> result = new HashSet<string>();
            if (ens == null || ens.Length == 0)
                return result;
           
            foreach (KEntity ke in ens)
            {
                result.Add(ke.entity);
            }
            return result;
        }


    }
}