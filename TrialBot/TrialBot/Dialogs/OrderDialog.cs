using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;

namespace TrialBot.Dialogs
{
    public class OrderDialog : IDialog<bool>
    {
        private TrialBot.Model.Order order;
        public async Task StartAsync(IDialogContext context)
        {
            //           throw new NotImplementedException();
            var orderForm = new FormDialog<TrialBot.Model.Order>(this.order , TrialBot.Model.Order.BuildForm , FormOptions.PromptInStart);
            context.Call(orderForm,this.orderTaken1);
        }

        private async Task orderTaken1(IDialogContext context, IAwaitable<Model.Order> result)
        {
            var order = result;
            context.Done(true);
        }
    }
}