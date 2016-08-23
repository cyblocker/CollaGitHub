using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using CollaBotFramework.Models;

namespace CollaBotFramework
{
    class Tools
    {
        public static string DB_CollaGithub = @"Data Source=\\\\msralpa\Users\v-rumeng\public\Github\Colla_Github.db;Version=3;";
        public static HashSet<string> loadHashSet(string file)
        {
            HashSet<string> result = new HashSet<string>();
            try
            {

                Console.WriteLine("Enter loadHashSet...");
                //string file = @"\\msralpa\Users\v-rumeng\public\AliasList.txt";
                string[] lines = System.IO.File.ReadAllLines(file);
                //int count = 0;
                foreach (string ss in lines)
                {

                    result.Add(ss.ToLower().Trim());
                }
                Console.WriteLine("Finished loading all stopTerm,totally {0}", result.Count);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in loadHashSet {0}", e.Message);
                return result;
            }


        }
        public static List<string> loadAllAlias(string file)
        {
            List<string> msraAlias = new List<string>();
            Console.WriteLine("Enter loadAllalias...");
            //string file = @"\\msralpa\Users\v-rumeng\public\AliasList.txt";
            string[] lines = System.IO.File.ReadAllLines(file);
            //int count = 0;
            foreach (string ss in lines)
            {
                //Card_Info ci = GetAlias.cardInfoByAlias(ss.Trim());
                //if (ci == null)
                //    Console.WriteLine("Null alias information for {s}",ss.Trim());
                if (ss.Trim().Equals(""))
                    continue;
                msraAlias.Add(ss.Trim());
            }
            Console.WriteLine("Finished loading all alias,totally {0}", msraAlias.Count);
            return msraAlias;
        }
        public static Dictionary<string, double> parseRankedKeywordsXml(string xmlAlchemy)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlAlchemy);
            XmlNodeList list = doc.GetElementsByTagName("keyword");
            foreach (XmlNode node in list)
            {
                XmlNodeList childs = node.ChildNodes;
                string keyword = "";
                double relavance = 0.0;
                foreach (XmlNode nnode in childs)
                {
                    if (nnode.Name == "relevance")
                        relavance = Convert.ToDouble(nnode.InnerText);
                    else if (nnode.Name == "text")
                        keyword = nnode.InnerText;
                }
                if (!keyword.Equals(""))
                    result.Add(keyword, relavance);
            }
            return result;
        }
        public static Dictionary<int, int> getNextLevelBracket(string input)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            Stack<int> left = new Stack<int>();
            Queue<int> right = new Queue<int>();
            char[] chaA = input.ToCharArray();
            for (int index = 0; index < chaA.Length; index++)
            {
                char c = chaA[index];
                if (c.Equals('('))
                    left.Push(index);
                else if (c.Equals(')'))
                {
                    if (left.Count == 0)
                        return null;
                    else
                    {
                        int leftIn = left.Pop();
                        if (leftIn >= index)
                            return null;
                        else
                            result.Add(leftIn, index);
                    }

                }

            }
            while (left.Count > 0)
            {
                int leftIn = left.Pop();
                int rightIn = right.Dequeue();
                result.Add(leftIn, rightIn);
            }
            return result;
        }
        public static Dictionary<int, int> getBracket(string input)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            Stack<int> left = new Stack<int>();
            Queue<int> right = new Queue<int>();
            char[] chaA = input.ToCharArray();
            for (int index = 0; index < chaA.Length; index++)
            {
                char c = chaA[index];
                if (c.Equals('('))
                    left.Push(index);
                else if (c.Equals(')'))
                {
                    if (left.Count == 0)
                        return null;
                    else
                    {
                        int leftIn = left.Pop();
                        if (leftIn >= index)
                            return null;
                        else
                            result.Add(leftIn, index);
                    }

                }

            }
            while (left.Count > 0)
            {
                int leftIn = left.Pop();
                int rightIn = right.Dequeue();
                result.Add(leftIn, rightIn);
            }
            return result;
        }

        public static Person GetPersonByProjectName(string repoName)
        {
            string[] u_r = repoName.Split('/');
            if (u_r.Length < 2)
                return null;
            else
            {
                string userLogin = u_r[0];
                string homepageUrl = Tools.GetHomepageByUserLogin(userLogin);
                //string emil = DB_Operation.GetEmailByUserLogin(userLogin);
                string emil = "";
                Person p = new Person(userLogin, emil, homepageUrl);
                return p;
            } 
        }

        public static string GetHomepageByProjectName(string r)
        {
            return string.Format("https://github.com/{0}", r);
        }

        internal static string HashToOutput(HashSet<string> hashSet)
        {
            string result = "";
            foreach (string s in hashSet)
                result += s + ",";
            if (result.EndsWith(","))
                result = result.Substring(0, result.Length - 1);
            return result;
        }

        //public static List<int> getSingleMath(string input, Regex reg)
        //{
        //    Console.WriteLine("Get single match...");
        //    int startH = input.Length;
        //    int endH = -1;
        //    Match mat = reg.Match(input);
        //    while (mat.Success)
        //    {
        //        Console.WriteLine("Match index {0},match content:{1}", mat.Index, mat.Value);

        //        if (mat.Index < startH)
        //            startH = mat.Index;
        //        if (mat.Index > endH)
        //            endH = mat.Index + mat.Value.Length - 1;
        //        mat = mat.NextMatch();
        //    }
        //    List<int> result = new List<int>();
        //    result.Add(startH);
        //    result.Add(endH);

        //    Console.WriteLine("start {0}, end {1}", startH, endH);
        //    return result;
        //}
        //public static void getMatch(string input, Regex regex, Dictionary<int, int> result)
        //{

        //    Console.WriteLine("getmatch for {0}", input);

        //    List<int> inddd = getSingleMath(input, regex);
        //    if (inddd[0] == input.Length || inddd[1] == -1)
        //    {
        //        Console.WriteLine("return");
        //        return;

        //    }

        //    else
        //    {
        //        result.Add(inddd[0], inddd[1]);

        //        getMatch(input.Substring(inddd[0] + 1, inddd[1] - inddd[0] - 1), regex, result);
        //    }

        //}
        public static Dictionary<string, string> parseRankedEntityXml(string entityAnswer)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(entityAnswer);
            XmlNodeList list = doc.GetElementsByTagName("entity");
            foreach (XmlNode node in list)
            {
                XmlNodeList childs = node.ChildNodes;
                string entity = "";
                string type = "";
                foreach (XmlNode nnode in childs)
                {
                    string info = nnode.InnerText;
                    if (nnode.Name == "type")
                        type = info;
                    else if (nnode.Name == "text")
                        entity = info;
                }
                if (!entity.Equals(""))
                {
                    if (result.ContainsKey(entity))
                        continue;
                    result.Add(entity, type);
                }

            }
            return result;

        }
        public static void writeFile(Dictionary<string, Dictionary<string, double>> dicion, string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName);
            foreach (string alias in dicion.Keys)
            {
                writer.Write(alias + "\t");
                Dictionary<string, double> keypairs = dicion[alias];
                int count = keypairs.Count;
                int index = 0;
                foreach (string word in keypairs.Keys)
                {
                    index++;
                    string score = keypairs[word].ToString();
                    if (index < count)
                        writer.Write(word + ":" + score + ",");
                    else
                    {
                        writer.Write(word + ":" + score + "\n");
                        //writer.WriteLine();
                    }

                }


            }
            writer.Close();
        }
        public static void writeFileList(Dictionary<string, List<string>> dicion, string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName);
            foreach (string alias in dicion.Keys)
            {
                writer.Write(alias + "\t");
                List<string> keypairs = dicion[alias];
                int count = keypairs.Count;
                int index = 0;
                foreach (string word in keypairs)
                {
                    index++;

                    if (index < count)
                        writer.Write(word + ",");
                    else
                    {
                        writer.Write(word + "\n");
                        //writer.WriteLine();
                    }

                }


            }
            writer.Close();
        }
        public static Dictionary<string, Dictionary<string, double>> loadFile(string file)
        {

            Dictionary<string, Dictionary<string, double>> result = new Dictionary<string, Dictionary<string, double>>();
            string[] lines = System.IO.File.ReadAllLines(file);
            foreach (string line in lines)
            {
                string[] name_pair = line.Split('\t');
                string alias = name_pair[0];
                Dictionary<string, double> pairs_word = new Dictionary<string, double>();
                string[] pairs_readLine = name_pair[1].Split(',');
                foreach (string pair_ele in pairs_readLine)
                {
                    string[] info = pair_ele.Split(':');
                    string keywordName = info[0];
                    double score = Convert.ToDouble(info[1]);
                    //Console.WriteLine("Keyword:{0},score:{1}", keywordName, score);
                    pairs_word.Add(keywordName, score);
                }
                result.Add(alias, pairs_word);
            }

            return result;
        }
        public static Dictionary<string, List<string>> loadFileList(string file)
        {
            Console.WriteLine("LoadfileList from file {0}", file);
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            string[] lines = System.IO.File.ReadAllLines(file);
            foreach (string line in lines)
            {
                string[] name_pair = line.Split('\t');
                if (name_pair.Length < 2)
                    continue;
                string alias = name_pair[0];
                List<string> words = new List<string>();
                string[] pairs_readLine = name_pair[1].Split(',');
                foreach (string pair_ele in pairs_readLine)
                {
                    words.Add(pair_ele);
                }
                result.Add(alias, words);
            }
            Console.WriteLine("Finished loading filelist from file, totally {0}", result.Count);
            return result;
        }
        public static double SimScore(string s, string t)
        {

            int editDist = Tools.miniEditDistance(s.ToLower(), t.ToLower());
            int logerlength = s.Length > t.Length ? s.Length : t.Length;
            double sim = (double)(logerlength - editDist) / (double)logerlength;
            return sim;
        }
        public static int miniEditDistance(string a, string b)
        {


            if (String.IsNullOrEmpty(a) || String.IsNullOrEmpty(b)) return 0;

            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                        (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                        );
                }
            return distances[lengthA, lengthB];
        }
        public static string[] ListToArrayString(List<string> inputList)
        {

            int length = inputList.Count;
            string[] outputList = new string[length];
            int curIndx = 0;
            foreach (string s in inputList)
                outputList[curIndx++] = s;
            return outputList;

        }
        public static string listToOutputString(List<string> input)
        {
            string output = "";
            foreach (string s in input)
                output += s + ",";
            if (output.EndsWith(","))
                //if(output.Length > 1)
                output = output.Substring(0, output.Length - 1);
            return output;
        }
        public static HashSet<string> mergeSetFromDic(Dictionary<string, HashSet<string>> input)
        {
            HashSet<string> result = new HashSet<string>();
            foreach (string s in input.Keys)
            {
                result.UnionWith(input[s]);
            }
            return result;
        }
        public static Dictionary<string, double> normalizeDicStringDoublle(Dictionary<string, double> input)
        {
            double sum = 0.0;
            foreach (string s in input.Keys)
                sum += input[s];
            if (sum == 0)
                sum = Double.MaxValue;
            Dictionary<string, double> result = new Dictionary<string, double>();
            foreach (string ss in input.Keys)
            {
                double scoreNorm = input[ss] / sum;
                result.Add(ss, scoreNorm);
            }
            return result;

        }
        public static void addDicStringDouble(Dictionary<string, double> result, string key, double score)
        {
            string newKey = key.ToLower();
            if (result.ContainsKey(newKey))
            {
                double curScore = result[newKey];
                curScore += score;
                result[newKey] = score;
            }
            else
                result.Add(newKey, score);
        }
        public static void addDicStringDoubleAvgResult(Dictionary<string, double> result, string key, double score, Dictionary<string, int> countSource)
        {
            string newKey = key.ToLower();
            if (result.ContainsKey(newKey))
            {
                double curScore = result[newKey];
                curScore += score;
                result[newKey] = score;
                int curCount = countSource[newKey];
                curCount++;
                countSource[newKey] = curCount;
            }
            else
            {
                result.Add(newKey, score);
                countSource.Add(newKey, 1);
            }

        }
        public static string listToOutputString(HashSet<string> input)
        {
            string output = "";
            foreach (string s in input)
                output += s + ",";
            if (output.EndsWith(","))
                //if(output.Length > 1)
                output = output.Substring(0, output.Length - 1);
            return output;
        }
        public static void addListToList(HashSet<string> target, List<string> source, int length)
        {
            int count = 0;
            foreach (string s in source)
            {
                target.Add(s);
                count++;
                if (count == length)
                    break;
            }
        }
        public static void mergeDicStringInt(HashSet<string> target, Dictionary<string, int> source)
        {
            foreach (string s in source.Keys)
            {
                if (s.Trim().Equals(""))
                    continue;
                target.Add(s);
            }
        }
        //public static Dictionary<string, List<Tuple<string, double>>> readDB_UserKeywords()
        //{
        //    Console.WriteLine("enter readDB_UserKeywords");
        //    //using()
        //    Dictionary<string, List<Tuple<string, double>>> result = new Dictionary<string, List<Tuple<string, double>>>();
        //    using (SQLiteConnection conn = new SQLiteConnection(DB_CollaGithub))
        //    {
        //        conn.Open();
        //        using (SQLiteTransaction tr = conn.BeginTransaction())
        //        {
        //            SQLiteCommand selectSQL = new SQLiteCommand("SELECT UserLogin,KeywordsList FROM User_Keywords", conn);
        //            using (SQLiteDataReader reader = selectSQL.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    string userLoginName = reader.GetValue(0).ToString();
        //                    string listKw = reader.GetValue(1).ToString();
        //                    List<Tuple<string, double>> listT = Tools.StringToTupleList(listKw);

        //                    result.Add(userLoginName, listT);
        //                }
        //                reader.Close();
        //            }
        //            tr.Commit();
        //        }

        //    }
        //    Console.WriteLine("read user keywords totally {0}", result.Count);
        //    return result;
        //}
        public static Dictionary<string, List<Tuple<string, double>>> readKeywords(string file)
        {

            Dictionary<string, List<Tuple<string, double>>> result = new Dictionary<string, List<Tuple<string, double>>>();
            string line = "";
            StreamReader reader = new StreamReader(file);
            while ((line = reader.ReadLine()) != null)
            {
                string[] user_list = line.Split('\t');
                if (user_list.Length < 2)
                    continue;
                string user = user_list[0];
                string list = user_list[1];
                List<Tuple<string, double>> ps = Tools.StringToTupleList(list);
                result.Add(user, ps);
            }
            Console.WriteLine("finished reading totally :{0}", result.Count);
            return result;
        }
        public static string TupleListToString(List<Tuple<string, double>> list)
        {
            string result = "";
            foreach (Tuple<string, double> re in list)
            {
                result += re.Item1 + ":" + re.Item2 + ";";
            }
            if (result.EndsWith(";"))
                result = result.Substring(0, result.Length - 1);
            return result;
        }
        public static List<Tuple<string, double>> StringToTupleList(string input)
        {
            List<Tuple<string, double>> result = new List<Tuple<string, double>>();
            string[] li = input.Split(';');
            foreach (string l in li)
            {
                string[] t = l.Split(':');
                if (t.Length < 2)
                    continue;
                else
                {
                    //Console.WriteLine(t[1]);
                    Tuple<string, double> ttt = new Tuple<string, double>(t[0], Double.Parse(t[1]));
                    result.Add(ttt);
                }
            }
            //Console.WriteLine("lsit num {0}", result.Count);
            return result;
        }
       
        public static HashSet<string> deDuplicateBy(string input, char a)
        {
            HashSet<string> result = new HashSet<string>();
            string[] list = input.Split(a);
            foreach (string s in list)
                result.Add(s);
            return result;
        }
        

        public static string GetHomepageByUserLogin(string userLogin)
        {
            string url = string.Format("https://github.com/{0}", userLogin);
            return url;
        }
    }
}