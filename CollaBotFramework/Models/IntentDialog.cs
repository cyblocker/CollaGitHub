using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace CollaBotFramework.Models
{
    [LuisModel("<d0161fe8-16ac-41b8-8103-d43ab44dbf85>", "<d9aa4c3769bd495eb66204ee51dea8e2>")]
    [Serializable]
    public class IntentDialog: LuisDialog<object>
    {
        public const string Entity = "Skill";
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string displayString = "I'm a bot to help you find people or project with specific technological skills on GitHub easily. [More Channel](http://collabotframework.azurewebsites.net)";
            displayString += "\n\n You can start with: \n";
            displayString += "* \"Who knows *skill1, skill2, ...*\"\n";
            displayString += "* \"Find project about *tech1, tech2, ...*\"\n";
            displayString += "* \"Find a team covers skills of *skill1, skill2, ...*\"";
            await context.PostAsync(displayString);
            context.Wait(MessageReceived);
        }
    }
}