using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;

namespace CollaBotFramework.Models
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private bool IsRegister = false;
        private bool RegisterCheck = false;
        private FormUserInfo user = null;
        private string channel;
        private string uid;
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            //await context.PostAsync("You said: " + message.Text);
            string input = message.Text;
            //Activity reply;
            string displayString = "";
            channel = message.ChannelId;
            uid = message.From.Id;
            if(!RegisterCheck)
            {
                RegisterCheck = true;
                user = DB_Operation.queryUserInfo(channel, uid);
                if (user != null)
                    IsRegister = true;
            }
            if (input.ToLower() == "register")
            {
                var userForm = new FormDialog<FormUserInfo>(new FormUserInfo(), FormUserInfo.BuildForm, FormOptions.PromptInStart);
                context.Call<FormUserInfo>(userForm, UserFormComplete);
            }

            else
            {
                INTENT userIntent;
                HashSet<string> keywords = LUIS.getNP(input, out userIntent);


                //if (Reaction.isLookingForPerson(input))
                if (userIntent == INTENT.FindUser)
                {
                    await context.PostAsync("Let me check...");
                    //HashSet<string> keywords;
                    //Dictionary<Person, double> user_list = Reaction.LookingForPerson(input, out keywords);
                    if (keywords.Count == 0)
                    {
                        displayString += "Sorry, I can not understand your query. Do you meant to find a user with specific skill? You can try to ask me: \"Who knows *skill1, skill2, ...*\".";
                    }

                    else
                    {
                        Dictionary<Person, double> user_list = QueryInterface.queryUser(keywords);
                        if (user_list.Count == 0)
                        {
                            displayString += "Sorry, I could not find any appropriate user for your query \"" + input + "\"";
                        }
                        else
                        {
                            displayString += "I find serveral users who might help you with";
                            foreach (var keyword in keywords)
                            {
                                displayString += " " + keyword;
                            }
                            displayString += ":";
                            foreach (Person p in user_list.Keys)
                            {
                                displayString += "\n";
                                displayString += string.Format("* [{1}]({0})", p.homepageurl, p.userLogin);
                            }
                        }
                    }

                }

                //else if (Reaction.isLookingForProject(input))
                else if (userIntent == INTENT.FindProject)
                {
                    await context.PostAsync("Let me check...");
                    //HashSet<string> keywords;
                    //Dictionary<Project, double> project_list = Reaction.LookingForProject(input,out keywords);
                    if (keywords.Count == 0)
                    {
                        displayString += "Sorry, I can not understand your query. Do you meant to find a project on specific techs? You can try to ask me: \"Find project about *tech1, tech2, ...*\".";
                    }
                    else
                    {
                        Dictionary<Project, double> project_list = QueryInterface.queryProject(keywords);

                        if (project_list.Count == 0)
                        {
                            displayString += "Sorry, I could not find any appropriate project for your query \"." + input + "\"";
                        }
                        else
                        {
                            displayString += "I find serveral projects about";
                            foreach (var keyword in keywords)
                            {
                                displayString += " " + keyword;
                            }
                            displayString += ":";
                            foreach (Project p in project_list.Keys)
                            {
                                displayString += "\n";
                                displayString += string.Format("* [{1}]({0})", p.projectUrl, p.projectName);
                            }
                        }
                    }


                }
                //else if (Reaction.isLookingForTeam(input))
                else if (userIntent == INTENT.FindTeam)
                {
                    await context.PostAsync("Let me check...");
                    //HashSet<Tuple<Person, string>> records = Reaction.LookingForTeam(input);
                    if (keywords.Count == 0)
                    {
                        displayString += "Sorry, I can not understand your query. Do you meant to find a team of users to cover specific skills? You can try to ask me: \"Find a team covers skill of *skill1, skill2, ...*\".";
                    }
                    else
                    {
                        HashSet<Tuple<Person, string>> records = QueryInterface.queryTeam(keywords);
                        if (records.Count == 0)
                        {
                            displayString += "Sorry, I could not find any team according to your requirement \"." + input + "\"";
                        }
                        else
                        {
                            displayString += "I find a team consists of following users according to your requirement:\n\n";
                            displayString += "**People**\t\t**Skills**";
                            foreach (Tuple<Person, string> t in records)
                            {
                                displayString += "\n\n";
                                displayString += string.Format("* [{2}]({0})\t\t{1}", t.Item1.userLogin, t.Item2, t.Item1.homepageurl);
                            }
                        }
                    }

                }
                else
                {
                    displayString = "I'm a bot to help you find people or project with specific technological skills on GitHub easily. [More Channel](http://collabotframework.azurewebsites.net)";
                    displayString += "\n\n You can start with: \n";
                    displayString += "* \"Who knows *skill1, skill2, ...*\"\n";
                    displayString += "* \"Find project about *tech1, tech2, ...*\"\n";
                    displayString += "* \"Find a team covers skills of *skill1, skill2, ...*\"";
                    displayString += "\n\n Say *register* to let me serve you better!";
                    //int length = (activity.Text ?? string.Empty).Length;
                    //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters <br/> Test new line");
                }
                await context.PostAsync(displayString);
                System.Diagnostics.Trace.TraceInformation("Reply: [markdown]{0}", displayString);
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task UserFormComplete(IDialogContext context, IAwaitable<FormUserInfo> result)
        {
            try
            {
                user = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the register!");
                return;
            }

            if (user != null)
            {
                if(user.Email.StartsWith("<"))
                {
                    user.Email = user.Email.Substring(8).Split('|')[0];
                }
                if (!IsRegister)
                {
                    await context.PostAsync("Your information is registered: " + user.ToString());
                    IsRegister = true;
                    DB_Operation.InsertUserInfo(user, channel, uid);
                }
                else
                {
                    await context.PostAsync("Your information has been updated: " + user.ToString());
                    IsRegister = true;
                    DB_Operation.UpdateUserInfo(user, channel, uid);
                }
            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}