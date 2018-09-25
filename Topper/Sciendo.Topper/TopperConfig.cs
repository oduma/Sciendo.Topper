using Sciendo.Topper.Contracts;
using Sciendo.Topper.Notifier;
using Sciendo.Topper.Store;

namespace Sciendo.Topper
{
    public class TopperConfig
    {
        [ConfigProperty("cosmosDb")]
        public CosmosDb Cosmosdb { get; set; }

        [ConfigProperty("topperLastFM")]
        public TopperLastFmConfig TopperLastFmConfig { get; set; }

        [ConfigProperty("topperRules")]
        public TopperRulesConfig TopperRulesConfig { get; set; }

        [ConfigProperty("emailOptions")]
        public EmailOptions EmailOptions { get; set; }

        [ConfigProperty("User")]
        public string User
        {
            set { EmailOptions.UserName = value;
                EmailOptions.DefaultSenderEmail = value;
            }
            get { return EmailOptions.UserName; }
        }

        [ConfigProperty("Key")]
        public string Key
        {
            set { EmailOptions.Key = value; }
            get { return EmailOptions.Key; }
        }

        [ConfigProperty("To")]
        public string DestinationEmail { get; set; }

    }
}
