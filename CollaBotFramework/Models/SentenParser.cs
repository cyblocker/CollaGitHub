//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using edu.stanford.nlp.pipeline;
//using java.util;
//using System.IO;
//using java.io;
//using edu.stanford.nlp.ling;
//using edu.stanford.nlp.util;
//using edu.stanford.nlp.trees;
//using System.Xml;
//using Console = System.Console;
//using System.Text.RegularExpressions;
//using edu.stanford.nlp.parser.lexparser;
//using edu.stanford.nlp.trees.tregex;
//using System.Net;
//using Newtonsoft.Json.Converters;
//using Newtonsoft.Json;
//namespace CollaBotFramework.SentenParser
//{

//    public class responseData
//    {
//        public string analyzerId { get; set; }
//        public string[] result { get; set; }
//    }
//    public class Data
//    {
//        public string language { get; set; }
//        public string[] analyzerIds { get; set; }
//        public string text { get; set; }
//        public Data(string input, string lan, string[] ana)
//        {
//            this.text = input;
//            this.language = lan;
//            this.analyzerIds = ana;
//        }
//    }
//    public class SentenParser
//    {
//        /* This class perform parse function for sentences
//           1. Parse the query in natrual language format
//         * 2. The patterns of chatterbot are based on keywords matching
//         * 3. know. keywords (NP)
//         */

//        string jarRoot = @"\\msralpa\users\v-rumeng\public\stanford-corenlp-3.6.0-models";
//        java.util.Properties props;
//        StanfordCoreNLP pipeline;
//        string curDir;
//        HashSet<string> stopTerm;
//        public SentenParser()
//        {
//            initial();
//        }
//        public void test()
//        {
//            initial();

//            //var text = "Does any people know about machine learning and deep learning, I want to know more about alphaGo.";
//            while (true)
//            {
//                System.Console.WriteLine("Enter your query..");
//                string input = System.Console.ReadLine();
//                if (input.Trim().Equals("stop"))
//                    break;
//                else
//                {
//                    HashSet<string> np = getNPListHash(input);
//                    foreach (string s in np)
//                    {
//                        System.Console.WriteLine(s);
//                    }
//                }
//            }
//        }

//        public void initial()
//        {
//            props = new java.util.Properties();
//            props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, parse, dcoref");
//            props.setProperty("ner.useSUTime", "0");
//            curDir = Environment.CurrentDirectory;
//            Directory.SetCurrentDirectory(jarRoot);
//            pipeline = new StanfordCoreNLP(props);

//            Directory.SetCurrentDirectory(curDir);
//            stopTerm = Tools.loadHashSet(@"\\msralpa\users\v-rumeng\public\stopTerm.txt");
//        }

//        public List<string> parserContent(string xml)
//        {

//            List<string> result = new List<string>();
//            XmlDocument doc = new XmlDocument();
//            doc.LoadXml(xml);

//            XmlNodeList sentences = doc.GetElementsByTagName("sentence");
//            //Console.WriteLine("sentence num {0}", sentences.Count);
//            foreach (XmlNode sen in sentences)
//            {
//                //Console.WriteLine("sen :{0}", sen.InnerXml);
//                XmlNode parser = sen.SelectSingleNode("./parse");

//                if (parser != null)
//                    result.Add(parser.InnerText);

//            }
//            return result;
//        }

//        public static HashSet<string> parseXML_NamedEntity(string xml)
//        {
//            HashSet<string> result = new HashSet<string>();
//            return result;
//        }

//        public static void addAll(List<Tree> input, List<Tree> tobeAdd)
//        {
//            foreach (Tree t in tobeAdd)
//                input.Add(t);
//        }
//        private static List<Tree> extract(Tree t)
//        {
//            List<Tree> wanted = new List<Tree>();
//            if (t.label().value().Equals("NP"))
//            {
//                wanted.Add(t);
//                foreach (Tree child in t.children())
//                {
//                    List<Tree> temp = new List<Tree>();
//                    temp = extract(child);
//                    if (temp.Count() > 0)
//                    {
//                        int o = -1;
//                        o = wanted.IndexOf(t);
//                        if (o != -1)

//                            wanted.RemoveAt(o);
//                    }
//                    addAll(wanted, temp);
//                }
//            }

//            else
//                foreach (Tree child in t.children())
//                    addAll(wanted, extract(child));
//            return wanted;
//        }
//        public static string GetTerm(Tree np)
//        {
//            // IN PRP DT 
//            if (np.isLeaf())
//            {



//                return np.toString();
//            }

//            else
//            {
//                string cur = "";

//                foreach (Tree c in np.children())
//                {
//                    string typeS = np.label().value();
//                    if (typeS.Trim().Equals("IN") || typeS.Equals("DT") || typeS.Equals("PRP") || typeS.Equals("PRP$"))
//                        cur += "";
//                    else if (typeS.Trim().Equals("CC"))
//                        cur += ",";
//                    else
//                        cur += GetTerm(c) + " ";
//                }
//                //if (cur.EndsWith(","))
//                //    cur = cur.Substring(0, cur.Length - 1);

//                return cur.Trim();
//            }
//        }
//        public HashSet<string> getNPListHash(string text)
//        {
//            HashSet<string> result = new HashSet<string>();
//            var annotation = new Annotation(text);

//            pipeline.annotate(annotation);

//            using (var stream = new ByteArrayOutputStream())
//            {
//                Console.WriteLine("Xml:");

//                pipeline.xmlPrint(annotation, new PrintWriter(stream));


//                List<string> par = parserContent(stream.toString());
//                foreach (string p in par)
//                    Console.WriteLine("Parser:{0}", p);

//                stream.close();
//            }

//            var sentences = annotation.get(new CoreAnnotations.SentencesAnnotation().getClass()) as ArrayList;
//            foreach (CoreMap SEN in sentences)
//            {
//                Tree tr = (Tree)SEN.get(typeof(edu.stanford.nlp.trees.TreeCoreAnnotations.TreeAnnotation));
//                List<Tree> np = extract(tr);
//                //Console.WriteLine("number of np {0}", np.Count);
//                foreach (Tree t in np)
//                {
//                    string term = GetTerm(t);
//                    if (term.Contains(","))
//                    {
//                        string[] termList = term.Split(',');
//                        foreach (string s in termList)
//                        {
//                            if (!s.Trim().Equals("") && !stopTerm.Contains(s.Trim().ToLower()))
//                                result.Add(s.ToLower().Trim());
//                        }
//                    }
//                    else if (!term.Trim().Equals("") && !stopTerm.Contains(term.Trim().ToLower()))
//                        result.Add(term.ToLower());
//                }

//            }

//            return result;
//        }

//        public static string analyzeDependancyTree(string input)
//        {
//            string url = "https://api.projectoxford.ai/linguistics/v1.0/analyze";
//            string[] aids = new string[1];
//            aids[0] = "22a6b758-420f-4745-8a3c-46835a67c0d2";
//            Data d = new Data(input, "en", aids);
//            string jsonData = JsonConvert.SerializeObject(d);
//            //Console.WriteLine(jsonData);
//            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
//            request.Method = "POST";
//            request.ContentType = "application/json";
//            request.Headers.Add("Ocp-Apim-Subscription-Key", "2f0d5d33cbda4086a12d607ef8695ae6");
//            request.Host = "api.projectoxford.ai";
//            byte[] data = Encoding.ASCII.GetBytes(jsonData);
//            request.ContentLength = data.Length;
//            Stream requestStream = request.GetRequestStream();
//            requestStream.Write(data, 0, data.Length);
//            requestStream.Close();
//            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
//            Stream responseStream = response.GetResponseStream();
//            StreamReader reader = new StreamReader(responseStream, Encoding.Default);
//            string content = reader.ReadToEnd();
//            reader.Close();
//            responseStream.Close();
//            response.Close();
//            return content;
//        }
//        public static HashSet<string> getNPListHash_Cognitive(string text)
//        {
//            string parseString = analyzeDependancyTree(text);
//            return Extract(parseString);
//        }

//        public static HashSet<string> Extract(string SyntaxTree)
//        {
//            HashSet<string> result = new HashSet<string>();
//            string npPattern = "\\(NP ";
//            var matches = Regex.Matches(SyntaxTree, npPattern);
//            List<int> indexes = new List<int>();
//            foreach (Match match in matches)
//            {
//                indexes.Add(match.Index);
//            }
//            foreach (int index in indexes)
//            {
//                int count = 1;
//                int endindex = index + 1;
//                while (count > 0)
//                {
//                    if (SyntaxTree[endindex] == '(')
//                        count++;
//                    else if (SyntaxTree[endindex] == ')')
//                        count--;
//                    endindex++;
//                }
//                bool flag = false;
//                foreach (int test in indexes)
//                {
//                    if (test > index && test < endindex)
//                    {
//                        flag = true;
//                        break;
//                    }
//                }
//                if (flag == true)
//                    continue;
//                string sub = SyntaxTree.Substring(index, endindex - index + 1);
//                var splited = sub.Split(' ');
//                string res = "";
//                bool skip = false;
//                foreach (string substring in splited)
//                {
//                    if (skip)
//                    {
//                        skip = false;
//                        continue;
//                    }
//                    if (substring.StartsWith("("))
//                    {
//                        if (substring == "(IN" || substring == "(DT" || substring == "(PRP" || substring == "(PRP$")
//                            skip = true;
//                        continue;
//                    }
//                    res += (res.Length == 0 ? "" : " ");
//                    res += substring.Replace(")", "");
//                }
//                if (res.Length > 0)
//                {
//                    if (res.Contains(" and "))
//                    {
//                        string[] termList = res.Split(new string[] { "and" }, StringSplitOptions.None);
//                        foreach (string s in termList)
//                        {
//                            if (!s.Trim().Equals(""))
//                                result.Add(s);
//                        }
//                    }
//                    else
//                        result.Add(res);
//                }

//            }
//            return result;
//        }
//    }
//}
