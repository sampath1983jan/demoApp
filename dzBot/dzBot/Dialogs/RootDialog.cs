using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace dzBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            PromptDialog.Choice(context, this.afterChoosen, new[] { "Change Password", "Reset Password" }, "What do you want to do today?",
                "I am sorry but I didn't understand that. I need you to select one of the options below", attempts: 3);
        }

        private async Task afterChoosen(IDialogContext context, IAwaitable<object> result) {
            try
            {
                var Selected = await result;
                switch (Selected.ToString())
                {
                    case "Change Password":
                        break;
                    case "Reset Password":
                        context.Call(new ResetDialog(), this.AfterResetPassword);
                        break;                 
                }
            }
            catch (TooManyAttemptsException) {
                await this.StartAsync(context);
            }
        }
        private async Task AfterResetPassword(IDialogContext context, IAwaitable<bool> result)
        {
            var success = await result;

            if (!success)
            {
                await context.PostAsync("Your identity was not verified and your password cannot be reset");
            }

            await this.StartAsync(context);
        }
    }
}