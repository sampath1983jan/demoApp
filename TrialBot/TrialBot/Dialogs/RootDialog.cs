using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
namespace TrialBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private Model.Order order;
        private Model.OrderDetails HodelOrder;
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            //var activity = await result as Activity;
            //// Calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;
            //// Return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");
            //context.Wait(MessageReceivedAsync);
            PromptDialog.Choice(context, this.afterSelection, new[] { "Order", "Products","Hotels Order" }, "Hello How can I help you?", "I didnt understand. please select from above list", attempts: 3);
         }
        private async Task afterSelection(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;
                switch (selection.ToString())
                {
                    case "Order":
                        this.order = new Model.Order();
                        this.order.AskToUseSavedSenderInfo = true;
                  //      await context.PostAsync("We are currently processing your sandwich. We will message you the status.");
                        var orderForm = new FormDialog<TrialBot.Model.Order>(this.order, TrialBot.Model.Order.BuildForm,  FormOptions.PromptInStart);
                        context.Call(orderForm, this.AfterOrderForm);
                        break;
                    case "Products":
                    case "Hotels Order":
                        this.HodelOrder = new Model.OrderDetails();
                        //this.HodelOrder.AskToUseSavedSenderInfo = true;
                        //      await context.PostAsync("We are currently processing your sandwich. We will message you the status.");
                        var sHodelOrder = new FormDialog<TrialBot.Model.OrderDetails>(this.HodelOrder, TrialBot.Model.OrderDetails.BuildForm, FormOptions.PromptInStart);
                        context.Call(sHodelOrder, this.AfterOrderForm);

                        break;
                }

            }
            catch (Exception ex)
            {
              //  await this.StartOverAsync(context, Resources.RootDialog_TooManyAttempts);
            }
           

        }
        private async Task AfterOrderForm(IDialogContext context, IAwaitable<Model.Order> result)
        {
             var order = await result;
            context.Done(true);
        }
        private async Task AfterOrderForm(IDialogContext context, IAwaitable<Model.OrderDetails> result)
        {
            var order = await result;
            context.Done(true);
        }
    }
}