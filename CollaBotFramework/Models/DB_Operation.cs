using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace CollaBotFramework.Models
{
    public class DB_Operation
    {
        //public static Dictionary<string, string> queryUser(string keyword)
        //{
        //    Console.WriteLine("enter query user,keyword:{0}...", keyword);
        //    Dictionary<string, string> result = new Dictionary<string, string>();
        //    using (var connection = new SQLiteConnection(@"Data Source=\\\\msralpa\Users\v-rumeng\public\Github\Colla_Github.db;Version=3;"))
        //    {

        //        connection.Open();
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            var selectCommand = connection.CreateCommand();
        //            selectCommand.Transaction = transaction;
        //            //selectCommand.CommandText = String.Format("select * from User_Keywords where KeywordsList like '%{0}%'",keyword);
        //            selectCommand.CommandText = string.Format(@"select * from User_Keywords where KeywordsList like '%{0}%' limit 50", keyword);
        //            using (var reader = selectCommand.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    Console.WriteLine("read line here");
        //                    string name = reader.GetString(0);
        //                    //double score = 1.0;
        //                    string klist = reader.GetString(1);
        //                    result.Add(name, klist);
        //                    //+": " + reader.GetString(1);
        //                }
        //                reader.Close();
        //            }

        //            transaction.Commit();
        //        }
        //    }
        //    return result;
        //}
        //public static Dictionary<string, string> queryProject(string keyword)
        //{
        //    Console.WriteLine("enter query user...");
        //    Dictionary<string, string> result = new Dictionary<string, string>();
        //    using (var connection = new SQLiteConnection(@"Data Source=\\\\msralpa\Users\v-rumeng\public\Github\Colla_Github.db;Version=3;"))
        //    {
        //        connection.Open();
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            var selectCommand = connection.CreateCommand();
        //            selectCommand.Transaction = transaction;
        //            //selectCommand.CommandText = String.Format("select * from User_Keywords where KeywordsList like '%{0}%'",keyword);
        //            selectCommand.CommandText = string.Format(@"select RepoName,Description from RepoText where Description like '%{0}%' limit 50", keyword);
        //            using (var reader = selectCommand.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    string repoName = reader.GetString(0);
        //                    string descrip = reader.GetString(1);
        //                    //double score = 1.0;
        //                    //string klist = reader.GetString(1);
        //                    //HashSet<string> keywords = deDuplicateBy(descrip, ',');
        //                    //foreach(string s in keywords)
        //                    result.Add(repoName, descrip);
        //                    //+": " + reader.GetString(1);
        //                }
        //            }
        //            transaction.Commit();
        //        }
        //    }
        //    return result;
        //}

        //public static string GetEmailByUserLogin(string u)
        //{
        //    string result = "";
        //    Console.WriteLine("enter get email by userlogin for :{0}...", u);
        //    using (var connection = new SQLiteConnection(@"Data Source=\\\\msralpa\Users\v-rumeng\public\Github\Colla_Github.db;Version=3;"))
        //    {

        //        connection.Open();
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            var selectCommand = connection.CreateCommand();
        //            selectCommand.Transaction = transaction;
        //            //selectCommand.CommandText = String.Format("select * from User_Keywords where KeywordsList like '%{0}%'",keyword);
        //            selectCommand.CommandText = string.Format(@"select * from UserEmail where Login = '{0}'", u);
        //            using (var reader = selectCommand.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    Console.WriteLine("read line here");
        //                    string email = reader.GetString(1);
                           
        //                    //+": " + reader.GetString(1);
        //                }
        //                reader.Close();
        //            }

        //            transaction.Commit();
        //        }
        //    }
        //    return result;
        //}


        
        public static Dictionary<string, string> queryProject(string keyword)
        {
            keyword = keyword.ToLower();
            Dictionary<string, string> result = new Dictionary<string, string>();
            using (var db = new UserKeywords())
            {
                try
                {
                    var query = (from p in db.RepoTexts
                                 where p.Description.ToLower().Contains(keyword)
                                 select p).Take(100);
                    foreach (var p in query)
                    {
                        if (!result.ContainsKey(p.RepoName))
                            result.Add(p.RepoName, p.Description);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception:{0}", e.Message);
                    return result;
                }
                //var query = (from u in db.User_Keywords where u.KeywordsList.Contains("java") select u).First();
                
            }
            return result;
        }
        public static Dictionary<string, string> queryUser(string keyword)
        {
            keyword = keyword.ToLower();
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (keyword.Trim().Equals(""))
                return result;
            try
            {
               
                using (var db = new UserKeywords())
                {

                    var query = (from u in db.User_Keyword_Score
                                 where u.Keyword.Equals(keyword, StringComparison.OrdinalIgnoreCase)
                                 orderby u.Score descending
                                 select u
                                 ).Take(10);
                   
                    if (query == null || !query.Any())
                        return result;
                    foreach (var u in query)
                    {
                        if (!result.ContainsKey(u.UserLogin))
                            result.Add(u.UserLogin, u.Score.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("Exception in queryUser for {0}, Message:{1};{2}",keyword,e.Message,e.InnerException.Message);
                //Console.WriteLine("e:{0}", e.Message);
                return result;
            }
            return result;
        }
        public static string GetEmailByUserLogin(string user)
        {
            if (user.Equals(""))
                return "";
            string result = "";
            return result;
            //using (var db = new UserKeywords())
            //{
            //    var email = (from u in db.UserEmails
            //                 where u.Login.ToLower().Contains(user)
            //                 select u).FirstOrDefault();
            //    return (email == null ? result : email.Email);
            //}
        }
        public static void InsertUserInfo(FormUserInfo userinfo, string channel, string id)
        {
            using (var db = new UserKeywords())
            {
                var UserInfo = new UserInfo();
                UserInfo.GitHubLogin = userinfo.GitHubUID;
                UserInfo.Email = userinfo.Email;
                UserInfo.channel = channel;
                UserInfo.userID = id;
                db.UserInfo.Add(UserInfo);
                db.SaveChanges();
            }
        }
        public static void UpdateUserInfo(FormUserInfo userinfo, string channel, string id)
        {
            using (var db = new UserKeywords())
            {
                var UserInfo = new UserInfo();
                UserInfo.GitHubLogin = userinfo.GitHubUID;
                UserInfo.Email = userinfo.Email;
                UserInfo.channel = channel;
                UserInfo.userID = id;
                db.UserInfo.Attach(UserInfo);
                db.Entry(UserInfo).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public static void AddOrUpdateUserInfo(FormUserInfo userinfo, string channel, string id)
        {
            using (var db = new UserKeywords())
            {
                var UserInfo = new UserInfo();
                UserInfo.GitHubLogin = userinfo.GitHubUID;
                UserInfo.Email = userinfo.Email;
                UserInfo.channel = channel;
                UserInfo.userID = id;

                if (db.UserInfo.Any(e => e.userID == id && e.channel == channel))
                {
                    db.UserInfo.Attach(UserInfo);
                    db.Entry(UserInfo).State = EntityState.Modified;
                }
                else
                {
                    db.UserInfo.Add(UserInfo);
                }
                db.SaveChanges();
            }
        }
        public static FormUserInfo queryUserInfo(string channel, string id)
        {
            FormUserInfo result = null;
            try
            {
                using (var db = new UserKeywords())
                {
                    var query = db.UserInfo
                        .Where(b => b.channel == channel && b.userID == id).FirstOrDefault();

                    if (query == null)
                        return result;
                    result = new FormUserInfo();
                    result.Email = query.Email;
                    result.GitHubUID = query.GitHubLogin;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("Exception in queryUserInfo for {0}, Message:{1};{2}", channel + ":" + id, e.Message, e.InnerException.Message);
                //Console.WriteLine("e:{0}", e.Message);
                return result;
            }
            return result;
        }
    }
}