using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace CollaBotFramework.Models
{
    [Serializable]
    public class FormUserInfo
    {
        [Prompt("What is your GitHub user ID?")]
        [Describe("GitHub ID")]
        public string GitHubUID;
        [Optional]
        [Prompt("What is your Email address?")]
        [Pattern("(<mailto:)?[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}(|([\\S])*)?$")]
        public string Email;
        public static IForm<FormUserInfo> BuildForm()
        {
            return new FormBuilder<FormUserInfo>()
                    .Message("Welcome to the colla Register!")
                    .Build();
        }
        public override string ToString()
        {
            return GitHubUID + " (" + Email + ")";
        }
    }
}