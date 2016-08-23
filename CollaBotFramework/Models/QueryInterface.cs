using CollaBotFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollaBotFramework
{
    public class QueryInterface
    {
        //public SentenParser sp;

        //public QueryInterface()
        //{
        //    sp = new SentenParser();
        //}

        //public HashSet<string> parse_keyword_sentence(string sentence)
        //{
        //    HashSet<string> result = new HashSet<string>();
        //    HashSet<string> initialNP = sp.getNPListHash(sentence);
        //    if (initialNP != null)
        //        return initialNP;
        //    else
        //        return result;
        //}

        //public List<string> queryByInput(string sentence, Dictionary<string, List<Tuple<string, double>>> keywords_user)
        //{
        //    List<string> user = new List<string>();
        //    Dictionary<string, double> rankU = new Dictionary<string, double>();
        //    HashSet<string> nps = parse_keyword_sentence(sentence);
        //    foreach (string np in nps)
        //    {
        //        foreach (string s in keywords_user.Keys)
        //        {
        //            if (Tools.SimScore(s, np) > 0.9)
        //            {
        //                foreach (Tuple<string, double> p in keywords_user[s])
        //                {
        //                    string uu = p.Item1;
        //                    double sco = p.Item2;
        //                    if (rankU.ContainsKey(uu))
        //                    {
        //                        double cu = rankU[uu];
        //                        rankU[uu] = cu + sco;
        //                    }
        //                    else
        //                        rankU.Add(uu, sco);
        //                }

        //            }
        //        }
        //    }
        //    var sortedDict = from entry in rankU orderby entry.Value descending select entry;
        //    int topCount = 0;
        //    int stopNum = 10 < sortedDict.Count() ? 10 : sortedDict.Count();
        //    foreach (var v in sortedDict)
        //    {
        //        //if (ga.nameByAlias(v.Key) == null)
        //        //    continue;
        //        topCount++;
        //        string uInfo = v.Key;
        //        double vote = v.Value;
        //        if (topCount > stopNum)
        //            break;
        //        user.Add(uInfo);
        //    }
        //    return user;
        //}
        //public void run()
        //{
        //    while (true)
        //    {
        //        Console.WriteLine("Please input the query sentence:");
        //        //string query = "machine learning, data mining";
        //        string q = Console.ReadLine();
        //        if (q.Equals("stop"))
        //            break;
        //        HashSet<string> keywords = parse_keyword_sentence(q);
        //        //List<string> also = QueryRefinement.getAlsoSearch_K(q, 3);
        //        Console.WriteLine("keywords {0} ", keywords.Count);

        //        foreach (string s in keywords)
        //        {
        //            Console.WriteLine(s);
        //        }
        //    }
        //}
        internal static HashSet<Tuple<Person, string>> queryTeam(HashSet<string> skills)
        {
            HashSet<Tuple<Person, string>> result = new HashSet<Tuple<Person, string>>();
            Dictionary<string, HashSet<string>> user_skills = new Dictionary<string, HashSet<string>>();
            foreach (string sk in skills)
            {
                HashSet<string> sk_has = new HashSet<string>();
                sk_has.Add(sk);
                Dictionary<Person, double> u_sk = queryUser(sk_has);
                foreach (Person p in u_sk.Keys)
                {
                    addFun(user_skills, p.userLogin, sk);
                }
            }

            return findTeam(skills, user_skills);
        }

        public static HashSet<Tuple<Person, string>> findTeam(HashSet<string> skills, Dictionary<string, HashSet<string>> user_skills)
        {
            HashSet<Tuple<Person, string>> result = new HashSet<Tuple<Person, string>>();

            while (skills.Count > 0)
            {
                string maxUser = getMaxUser(user_skills);
                if (maxUser.Equals("") || !user_skills.ContainsKey(maxUser))
                {
                    break;
                }
                else
                {
                    HashSet<string> cover_skills = user_skills[maxUser];
                    if (cover_skills.Count == 0)
                        break;

                    Person p = new Person(maxUser);
                    string skillsString = Tools.HashToOutput(cover_skills);
                    Tuple<Person, string> user_record = new Tuple<Person, string>(p, skillsString);
                    result.Add(user_record);
                    int num_before = skills.Count;
                    removeSkill(skills, cover_skills);
                    int num_after = skills.Count;
                    if (num_after == num_before)
                        break;
                }
            }

            return result;
        }

        public static void removeSkill(HashSet<string> whole_skills, HashSet<string> cover_skills)
        {
            foreach (string s in cover_skills)
                if (whole_skills.Contains(s))
                    whole_skills.Remove(s);
        }
        public static string getMaxUser(Dictionary<string, HashSet<string>> userDict)
        {
            int max = -1;
            string maxUser = "";
            foreach (string u in userDict.Keys)
            {
                int countU = userDict[u].Count;
                if (countU > max)
                {
                    max = countU;
                    maxUser = u;
                }
            }
            return maxUser;
        }
        public static void addFun(Dictionary<string, HashSet<string>> user_skills, string userLogin, string skill)
        {
            if (user_skills.ContainsKey(userLogin))
            {
                HashSet<string> skillset = user_skills[userLogin];
                skillset.Add(skill);
                user_skills[userLogin] = skillset;
            }
            else
            {
                HashSet<string> skillset = new HashSet<string>();
                skillset.Add(skill);
                user_skills.Add(userLogin, skillset);
            }
        }
        internal static Dictionary<Person, double> queryUser(HashSet<string> np)
        {
            Dictionary<Person, double> result = new Dictionary<Person, double>();
            Dictionary<Person, double> s_result = new Dictionary<Person, double>();
            foreach (string nnp in np)
            {
                Dictionary<string, string> users = DB_Operation.queryUser(nnp);
                foreach (string u in users.Keys)
                {
                    Person p = new Person();
                    p.userLogin = u;
                    //p.email = DB_Operation.GetEmailByUserLogin(u);
                    p.email = "";
                    p.homepageurl = Tools.GetHomepageByUserLogin(u);
                    double score = Convert.ToDouble(users[u]);
                    //string keywordsList = users[u];
                    //List<Tuple<string, double>> kList = Tools.StringToTupleList(keywordsList);
                    //foreach (Tuple<string, double> t in kList)
                    //{
                    //    if (Tools.SimScore(t.Item1, nnp) > 0.8)
                    //    {
                    //        string k = nnp.ToLower();
                    //        if (result.ContainsKey(p))
                    //        {
                    //            double curScore = result[p];
                    //            curScore += t.Item2;
                    //            result[p] = curScore;
                    //        }
                    //        else
                    //            result.Add(p, t.Item2);
                    //    }
                    if (result.ContainsKey(p))
                    {
                        result[p] += score;
                    }
                    else
                        result.Add(p, score);

                }
            }
            int stop = 10 < result.Count ? 10 : result.Count;
            var sortedDic = from entry in result orderby entry.Value descending select entry;
            int topcount = 0;
            foreach (var s in sortedDic)
            {
                //s.Key.email = DB_Operation.GetEmailByUserLogin(s.Key.userLogin);
                s_result.Add(s.Key, s.Value);
                topcount++;
                if (topcount >= stop)
                    break;
            }
            return s_result;
        }


        internal static Dictionary<Project, double> queryProject(HashSet<string> np)
        {
            Dictionary<Project, double> result = new Dictionary<Project, double>();
            Dictionary<Project, double> s_result = new Dictionary<Project, double>();
            foreach (string nnp in np)
            {
                Dictionary<string, string> repos = DB_Operation.queryProject(nnp);
                //Console.WriteLine("projects for keyword:{0}", nnp);
                foreach (string r in repos.Keys)
                {
                    Project pr = new Project();
                    //pr.projectDescription = 
                    string description = repos[r];
                    pr.projectName = r;
                    pr.projectDescription = Tools.HashToOutput(Tools.deDuplicateBy(description, ','));
                    pr.creator = Tools.GetPersonByProjectName(r);
                    pr.projectUrl = Tools.GetHomepageByProjectName(r);
                    HashSet<string> list = Tools.deDuplicateBy(description, ',');
                    //List<Tuple<string, double>> kList = Tools.StringToTupleList(keywordsList);
                    foreach (string descrip in list)
                    {
                        string[] compo = description.Split(' ');
                        foreach (string c in compo)
                        {
                            double ssscore = Tools.SimScore(c, nnp);
                            if (ssscore > 0.8)
                            {
                                string k = nnp.ToLower();
                                if (result.ContainsKey(pr))
                                {
                                    double curScore = result[pr];
                                    curScore += ssscore;
                                    result[pr] = curScore;
                                }
                                else
                                    result.Add(pr, ssscore);
                            }
                        }
                    }
                    //foreach (Tuple<string, double> t in kList)
                    //{
                    //    if (Tools.SimScore(t.Item1, nnp) > 0.8)
                    //    {
                    //        string k = nnp.ToLower();
                    //        if (result.ContainsKey(u))
                    //        {
                    //            double curScore = result[k];
                    //            curScore += t.Item2;
                    //            result[u] = curScore;
                    //        }
                    //        else
                    //            result.Add(u, t.Item2);
                    //    }

                    //}
                }

            }
            int stop = 10 < result.Count ? 10 : result.Count;
            var sortedDic = from entry in result orderby entry.Value descending select entry;
            int topcount = 0;
            foreach (var s in sortedDic)
            {
                s_result.Add(s.Key, s.Value);
                topcount++;
                if (topcount >= stop)
                    break;
            }
            return s_result;
        }
    }
}