using Newtonsoft.Json;

namespace Sciendo.Topper.Notifier
{
    internal class Mail
    {
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}