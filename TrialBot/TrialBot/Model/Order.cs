using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using BotAssets;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Builder.Dialogs;

namespace TrialBot.Model
{        
    [Serializable]      
    public class Order
    {
        public enum UseSaveInfoResponse
        {
            Yes = 1,
            Edit
        }
        public UseSaveInfoResponse? UseSavedSenderInfo { get; set; }
        public string OrderID { set; get; }       
        public string RecipientFirstName { get; set; }       
        public bool AskToUseSavedSenderInfo { get; set; }        
        public string RecipientLastName { get; set; }        
        public string RecipientPhone { get; set; }         
        public string RecipientEmail { get; set; }        
        public string RecipientAddress { get; set; }        
        public string ProductName { get; set; }        
        public string SenderFirstName { get; set; }        
        public string SenderLastName { get; set; }        
        public string SenderPhone { get; set; }
        public string SenderEmail { get; set; }       
        
       
        public static IForm<Order> BuildForm()
        {
            return new FormBuilder<Order>()
                    .Message("Welcome to demo Restaurant bot!")
                    .OnCompletion(async (context, order) =>
                    { 
                        await context.PostAsync("Thanks for your order!");
                    })
                    .Build();
        }
    }

}