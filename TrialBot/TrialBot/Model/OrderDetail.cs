using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace TrialBot.Model
{

//    public class c {
//        public void main() {
//            List<int> elements = new List<int>() { 10, 20, 31, 40 };
//            // ... Find index of first odd element.
//            int oddIndex = elements.FindIndex(x => x % 2 != 0);
//            //elements.Find()
//            elements.FindIndex((x) => { return x % 2 != 0; });
//        }
         

//}


[Serializable]
    public class OrderDetails
    {
        public enum UseSaveInfoResponse
        {
            Yes,
            No
        }
       
        public MenuOption MenuItems { get; set; }
        [Prompt("What kind of {&} would you like? {||}")]
        public DeliveryOptions DeliveryMode { get; set; }
        public UseSaveInfoResponse? UseSavedSenderInfo { get; set; }
        //[Prompt("Enter Your {<format>}?")]
        public string UserName { get; set; }
        public string Phone { get; set; }
         
        public string address { get; set; }
        
        public bool AskToUseSavedSenderInfo;
        public OrderDetails()
        {
            AskToUseSavedSenderInfo = true;
                       
    }
        public static IForm<OrderDetails> BuildForm()
        {
 
        var menuItems = MenuDB.GetAllMenuOptions();

            var builder = new FormBuilder<OrderDetails>();
            try
            {
                builder
            .Message("Welcome to demo Restaurant bot!")
            .Field(new FieldReflector<OrderDetails>(nameof(MenuItems)).SetType(null)
            .SetDefine((state, field) =>
            {
                foreach (var item in menuItems)
                {
                    field
        .AddDescription(item, new DescribeAttribute() { Title = item.ItemName, Description = item.ItemName, SubTitle = item.Description, Image = item.ItemImage })
        .AddTerms(item, item.ItemName);
                }
                return Task.FromResult(true);
            })
            .SetPrompt(new PromptAttribute(" What would you like to order? \n {||} \n")
            {
                ChoiceStyle = ChoiceStyleOptions.Carousel   
            })
            ).Confirm (async (state) =>{
                var cost = state.MenuItems.ItemName;
                return new PromptAttribute($"You have choosen product name {cost} is that ok?");
            })
            .Field(new FieldReflector<OrderDetails>(nameof(UseSavedSenderInfo)).SetNext((value,state) =>{
                    var selection = (UseSaveInfoResponse)value;
                if (selection == UseSaveInfoResponse.Yes)
                {
                    return new NextStep(new[] { nameof(UserName) });
                }
                else {
                    return new NextStep();
                }
                }))
            .AddRemainingFields()
            .OnCompletion(async (context, order) =>
            {
                await context.PostAsync("Thanks for your order!");
            });

                return builder.Build();
            }
            catch (Exception ex)
            {
                throw ex;
            }


          
        }

    }
    public enum DeliveryOptions
    {
        TakeAway = 1,
        Delivery
    }
    [Serializable]
    public class MenuOption
    {
        public Int32 ItemIndex { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string ItemImage { get; set; }
    }
    public class MenuDB

    {

        public static List<MenuOption> GetAllMenuOptions()

        {

            return new List<MenuOption>()
            {
            new MenuOption(){ItemName = "CrispyChicken" , Description ="4 pcs of crispy chicken.", ItemImage="http://divascancook.com/wp-content/uploads/2015/01/IMG_0231.jpg" },
            new MenuOption(){ ItemName = "ChickenWings" , Description= "6 pcs of crispy chicken wings." , ItemImage="https://www.munatycooking.com/wp-content/uploads/2016/05/crispy-spicy-chicken-wings-5.jpg"},
            new MenuOption(){ ItemName = "ChickenDrumStick" , Description="4 pcs of chicken drumsticks." , ItemImage="https://www.budgetbytes.com/wp-content/uploads/2016/03/Crispy-Baked-Honey-Sriracha-Chicken-Drumsticks-above-straight.jpg" },
            new MenuOption(){ ItemName = "ChickenPopcorn", Description = "1 medium chicken popcorn.", ItemImage = "http://mybodymykitchen.com/wp-content/uploads/2016/02/jalapeno-popcorn-chicken-1024x1024.jpg" },
            };
        }
    }

}


 