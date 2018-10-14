using Sciendo.Config;
using Sciendo.Last.Fm;
using Sciendo.Topper.Notifier;
using Sciendo.Topper.Store;

namespace Sciendo.Topper
{
    public class TopperConfig
    {
        [ConfigProperty("cosmosDb")]
        public CosmosDbConfig CosmosDbConfig { get; set; }

        [ConfigProperty("topperLastFM")]
        public LastFmConfig TopperLastFmConfig { get; set; }

        [ConfigProperty("topperRules")]
        public TopperRulesConfig TopperRulesConfig { get; set; }

        [ConfigProperty("emailOptions")]
        public EmailConfig EmailOptions { get; set; }

        [ConfigProperty("User")]
        public string User
        {
            set { EmailOptions.UserName = value;
                EmailOptions.DefaultSenderEmail = value;
            }
            get => EmailOptions.UserName;
        }

        [ConfigProperty("Key")]
        public string Key
        {
            set => EmailOptions.Key = value;
            get => EmailOptions.Key;
        }

        [ConfigProperty("To")]
        public string DestinationEmail { get; set; }

    }
}
