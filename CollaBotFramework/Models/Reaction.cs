using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollaBotFramework.Models
{
    public class Person
    {
        public string userLogin { get; set; }
        public string email { get; set; }
        public string homepageurl { get; set; }
        public Person()
        {
            userLogin = "";
            email = "";
            homepageurl = "";
        }
        public Person(string login, string em, string homepage)
        {
            userLogin = login;
            email = em;
            homepageurl = homepage;
        }
        public Person(string login)
        {
            userLogin = login;
            email = "";
            homepageurl = string.Format("https://github.com/{0}", userLogin);
        }
        public override int GetHashCode()
        {
            int ha = 17;
            ha = ha * 23 + userLogin.GetHashCode();
            return ha;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Person p = (Person)obj;
            return p.userLogin == userLogin;
        }
    }
    public class Project
    {
        public string projectName { get; set; }
        public string projectDescription { get; set; }
        
        public string projectUrl { get; set; }
        public Person creator { get; set; }

        public Project()
        {
              
        }
        public Project(string projectName, string projectDescription, Person p)
        {
            this.projectName = projectName;
            this.projectDescription = projectDescription;
            this.creator = p;
        }

        public override int GetHashCode()
        {
            int has = 17;
            has = has * 23 + projectName.GetHashCode();
            return has;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Project p = (Project)obj;
            return p.projectName == projectName;
        }
    }
    public class Reaction
    {
        public static bool isLookingForPerson(string Content)
        {
            if (Content.ToLower().Contains("collatesting"))
                return true;
            //Console.WriteLine("enter islooking for");
            bool flag = false;
            if (Content.ToLower().Contains("find someone")||Content.ToLower().Contains("who know"))
                flag = true;
            return flag;
        }

        public static bool isLookingForProject(string Content)
        {
            if (Content.ToLower().Contains("find project")|| Content.ToLower().Contains("project"))
                return true;
            else
            {
                return false;
            }
        }
        /// <summary>
        /// return null if no np is detected
        /// 
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        public static Dictionary<Person,double> LookingForPerson(string Content, out HashSet<string> np)
        {
            //HashSet<string> np = NPExtractor.getNP(Content);
            np = new HashSet<string>();
            //if(Content.ToLower().StartsWith("who know"))
            //{
            //    Content = Content.Substring(9).TrimStart();
            //    char[] delimiterChars = { ',', '.', ':', '\t', '?' };
            //    var array = Content.ToLower().Split(delimiterChars);
            //    for(int i = 0; i < array.Length; i++)
            //    {
            //        if (array[i] != "and" && array[i] != "or" && array[i].Length > 0)
            //            np.Add(array[i]);
            //    }
            //}
            //else
            //    np = NPExtractor.getNP(Content);
            
            if (np.Count > 0)
            {
                Dictionary<Person, double> person_list = QueryInterface.queryUser(np);
                return person_list;
            }
            return null;
        }

        public static Dictionary<Project,double> LookingForProject(string Content,out HashSet<string> np)
        {

            np = NPExtractor.getNP(Content);
            string[] prefixes = { "find project about", "find project on", "find projects on", "find projects about" };
            if (prefixes.Any(prefix => Content.ToLower().StartsWith(prefix)))
            {
                char[] delimiterChars = { ' ',',', '.', ':', '\t', '?' };
                var array = Content.ToLower().Split(delimiterChars);
                if (array.Length > 3)
                {
                    for (int i = 3; i < array.Length; i++)
                        if (array[i] != "and" && array[i] != "or" && array[i].Length > 0)
                            np.Add(array[i]);
                }

            }
            if (np.Count > 0)
            {
                Dictionary<Project, double> project_list = QueryInterface.queryProject(np);
                return project_list;
            }
            return null;
        }

        internal static bool isLookingForTeam(string input)
        {
            if (input.Contains("find team")|| input.ToLower().Contains("team"))
                return true;
            return false;

        }

        internal static HashSet<Tuple<Person, string>> LookingForTeam(string input)
        {
            HashSet<string> np = NPExtractor.getNP(input);
            if (np.Count > 0)
            {
                return QueryInterface.queryTeam(np);
            }
            return null;
        }
    }
}