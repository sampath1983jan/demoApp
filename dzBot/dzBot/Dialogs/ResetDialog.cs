using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
namespace dzBot.Dialogs
{   
    [Serializable]
    public class ResetDialog : IDialog<bool>
    {
        private const string PhoneRegexPattern = @"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";
        private const string EmailRegPattern = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$";
        public async Task StartAsync(IDialogContext context)
        {

            var promptPhoneDialog = new PromptStringRegex(
                "Please enter your phone number:",
                PhoneRegexPattern,
                "The value entered is not phone number. Please try again using the following format (xyz) xyz-wxyz:",
                "You have tried to enter your phone number many times. Please try again later.",
                attempts: 2);

            context.Call(promptPhoneDialog, this.ResumeAfterPhoneEntered);
            
           
        }

        private async Task ResumeAfterPhoneEntered(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var phone = await result;

                if (phone != null)
                {
                    await context.PostAsync($"The phone you provided is: {phone}");

                    var promptBirthDialog = new PromptDate(
                        "Please enter your date of birth (MM/dd/yyyy):",
                        "The value you entered is not a valid date. Please try again:",
                        "You have tried to enter your date of birth many times. Please try again later.",
                        attempts: 2);

                    context.Call(promptBirthDialog, this.AfterDateOfBirthEntered);
                }
                else
                {
                    context.Done(false);
                }
            }
            catch (TooManyAttemptsException)
            {
                context.Done(false);
            }
        }
        private async Task AfterDateOfBirthEntered(IDialogContext context, IAwaitable<DateTime> result)
        {
            try
            {
                var dateOfBirth = await result;

                if (dateOfBirth != DateTime.MinValue)
                {
                    await context.PostAsync($"The date of birth you provided is: {dateOfBirth.ToShortDateString()}");
                    // Add your custom reset password logic here.
                    var promptEmailDialog = new PromptStringRegex(
               "Please enter your EmailID (sample@gmail.com):",
               EmailRegPattern,
               "The value entered is not EmailID. Please try again using the following format (sampale@gmail.com):",
               "You have tried to enter your EmailID many times. Please try again later.",
               attempts: 2);
                    context.Call(promptEmailDialog, this.AfterEmailEntered);
                }
                else
                {
                    context.Done(false);
                }
            }
            catch (TooManyAttemptsException)
            {
                context.Done(false);
            }
        }

        private async Task AfterEmailEntered(IDialogContext context, IAwaitable<string> result) {
            try
            {
                var email = await result;
                await context.PostAsync($"The email you provided is: {email}");

                var newPassword = Guid.NewGuid().ToString().Replace("-", string.Empty);
                await context.PostAsync($"Thanks! Your new password is _{newPassword}_");
                context.Done(true);
            }
            catch (TooManyAttemptsException) {
                context.Done(false);
            }
               

        }
    }

    [Serializable]
    public class PromptStringRegex : Prompt<string, string>
    {
        private readonly Regex regex;

        public PromptStringRegex(string prompt, string regexPattern, string retry = null, string tooManyAttempts = null, int attempts = 3)
            : base(new PromptOptions<string>(prompt, retry, tooManyAttempts, attempts: attempts))
        {
            this.regex = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }
        protected override bool TryParse(IMessageActivity message, out string result)
        {
            var quitCondition = message.Text.Equals("Cancel", StringComparison.InvariantCultureIgnoreCase);
            var validEmail = this.regex.Match(message.Text).Success;

            result = validEmail ? message.Text : null;

            return validEmail || quitCondition;
        }
    }

    [Serializable]
    public class PromptDate : Prompt<DateTime, string>
    {
        public PromptDate(string prompt, string retry = null, string tooManyAttempts = null, int attempts = 3)
          : base(new PromptOptions<string>(prompt, retry, tooManyAttempts, attempts: attempts))
        {
        }

        protected override bool TryParse(IMessageActivity message, out DateTime result)
        {
            var quitCondition = message.Text.Equals("Cancel", StringComparison.InvariantCultureIgnoreCase);

            return DateTime.TryParseExact(message.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result) || quitCondition;
        }
    }

     
}