namespace BotApplication2017.Dialogs
{
    using System;
    using System.Threading.Tasks;

    using BotApplication2017.OpenWeatherMap;
    using BotApplication2017.Utils;

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
            var activity = await result;

            // get user name
            string userName = !string.IsNullOrEmpty(activity.From?.Name) 
                ? $" {activity.From.Name}" 
                : string.Empty;

            // get previous asked value
            var state = activity.GetStateClient();
            var userData = await state.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            var previousParam = userData.GetProperty<WeatherParam>("weather");

            var repl = await Reply(activity.Text, userName, previousParam);
            await context.PostAsync(repl);
            context.Wait(MessageReceivedAsync);
        }

        async Task<string> Reply(string msg, string userName, WeatherParam previousParam)
        {
            string[] a = msg.ToLower().Split(' ');

            if (a.IsPresent("help"))
            {
                return "Hey" + userName + @"! This is a simple weather bot.<br/>
Examples of commands:<br/>
  temperature today<br/>
  temperature in Minsk<br/>
  humidity tomorrow<br/>
  pressure today<br/>
  weather tomorrow in London";
            }

            if (a.IsPresent("temperature"))
            {
                WP.MeasurementType = Measurement.Temperature;
            }

            if (a.IsPresent("humidity"))
            {
                WP.MeasurementType = Measurement.Humidity;
            }

            if (a.IsPresent("pressure"))
            {
                WP.MeasurementType = Measurement.Pressure;
            }

            if (a.IsPresent("weather"))
            {
                WP.MeasurementType = Measurement.Weather;
            }

            if (a.IsPresent("today"))
            {
                WP.Today();
            }

            if (a.IsPresent("tomorrow"))
            {
                WP.Tomorrow();
            }

            if (!string.IsNullOrEmpty(a.NextTo("in")))
            {
                WP.Location = a.NextTo("in");
            }

            if (a.IsPresent("emotions"))
            {
                WP.MeasurementType = Measurement.Emotions;
            }

            if (!string.IsNullOrEmpty(a.NextTo("of")))
            {
                WP.Person = a.NextToAllTheRest("of");
            }

            return await WP.BuildResult(userName, previousParam);
        }
    }
}