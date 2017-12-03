namespace BotApplication2017.Dialogs
{
    using System;
    using System.Threading.Tasks;

    using BotApplication2017.OpenWeatherMap;

    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class WeatherDialog : IDialog<WeatherParam>
    {
        WeatherParam WP;

        public async Task StartAsync(IDialogContext context)
        {
            if (WP == null)
            {
                WP = new WeatherParam();
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }
    }
}