using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CollaBotFramework
{
    class NPExtractor
    {

        public static bool isStop(string w)
        {

            if (w.ToLower().Contains("project") || w.ToLower().Contains("contact") || w.ToLower().Contains("expert") || w.ToLower().Contains("repository"))
                return true;
            if (w.ToLower().Contains("team") || w.ToLower().Contains("skill") || w.ToLower().Contains("cover"))
                return true;
            return false;
        }
        public static void getNN_NNs(string input, string[][] pos, HashSet<string> result)
        {
            //HashSet<string> result = new HashSet<string>();
            //List<int> npIndex = new List<int>();
            //for (int i = 0; i < pos.Length; i++)
            //{
            //    if (pos[i].Equals("NN") || pos[i].Equals("NNS"))
            //        npIndex.Add(i);

            //}
            //string[] words = input.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            //    foreach (int indx in npIndex)
            //        result.Add(words[indx]);

            //return result;
            List<int> npIndex = new List<int>();
            int indx = 0;
            for (int i = 0; i < pos.GetLength(0); i++)
            {
                for (int j = 0; j < pos[0].Length; j++)
                {

                    if (pos[i][j].Equals("NN") || pos[i][j].Equals("NNS"))
                        npIndex.Add(indx);
                    indx++;
                }
            }
            //foreach (int ind in npIndex)
            //    Console.WriteLine(ind);
            input = LanguageAnalyze.RemovePunctuation(input);
            string[] words = input.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            //foreach (String s in words)
            //    Console.WriteLine(s);
            foreach (int inin in npIndex)
            {
                if (inin < words.Length)
                    result.Add(words[inin]);
            }

        }

        public static bool containWord(HashSet<string> words, string word)
        {
            foreach (string w in words)
            {
                //if (Regex.IsMatch(w, string.Format(@"\b{0}\b",word)))
                if (w.Contains("word ") || w.Contains(" word"))
                    return true;
            }
            return false;
        }

        public static HashSet<string> getNP(string input)
        {
            HashSet<string> npAll = new HashSet<string>();
            string[] syntaxt = LanguageAnalyze.analyzeDependancyTree(input);
            foreach (string syn in syntaxt)
            {
                Extract(syn, npAll);

            }
            Tuple<string, string[][]> t = LanguageAnalyze.analyzePOSTag(input);
            HashSet<string> nn = new HashSet<string>();
            getNN_NNs(t.Item1, t.Item2, nn);
            HashSet<string> result = new HashSet<string>();
            foreach (string nnn in nn)
            {
                if (!containWord(npAll, nnn) && !isStop(nnn))
                    result.Add(nnn);
            }
            foreach (string nnp in npAll)
                if (!isStop(nnp))
                    result.Add(nnp);
            //if (result.Contains("data mining"))
            //    result.Remove("data mining");
            if (result.Contains("database"))
                result.Remove("database");
            return result;
        }
        public static void Extract(string SyntaxTree, HashSet<string> result)
        {
            Console.WriteLine("parse:{0}", SyntaxTree);
            //HashSet<string> result = new HashSet<string>();
            string npPattern = "\\(NP ";
            var matches = Regex.Matches(SyntaxTree, npPattern);
            List<int> indexes = new List<int>();
            foreach (Match match in matches)
            {
                indexes.Add(match.Index);
            }
            string nnpPattern = "\\(NNP";
            var matchesNNP = Regex.Matches(SyntaxTree, nnpPattern);
            foreach (Match matchNNP in matchesNNP)
                indexes.Add(matchNNP.Index);
            foreach (int index in indexes)
            {
                int count = 1;
                int endindex = index + 1;
                while (count > 0)
                {
                    if (SyntaxTree[endindex] == '(')
                        count++;
                    else if (SyntaxTree[endindex] == ')')
                        count--;
                    endindex++;
                }
                bool flag = false;
                foreach (int test in indexes)
                {
                    if (test > index && test < endindex)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag == true)
                    continue;
                string sub = SyntaxTree.Substring(index, endindex - index + 1);
                var splited = sub.Split(' ');
                string res = "";
                bool skip = false;
                foreach (string substring in splited)
                {
                    if (skip)
                    {
                        skip = false;
                        continue;
                    }
                    if (substring.StartsWith("("))
                    {
                        if (substring == "(IN" || substring == "(DT" || substring == "(PRP" || substring == "(PRP$")
                            skip = true;
                        continue;
                    }
                    res += (res.Length == 0 ? "" : " ");
                    res += substring.Replace(")", "");
                }
                if (res.Length > 0)
                {
                    if (res.Contains(" and "))
                    {
                        string[] termList = res.Split(new string[] { "and" }, StringSplitOptions.None);
                        foreach (string s in termList)
                        {
                            if (!s.Trim().Equals(""))
                                result.Add(s.Trim());
                        }
                    }
                    else
                        result.Add(res.Trim());
                }

            }
            //return result;
        }
    }
}