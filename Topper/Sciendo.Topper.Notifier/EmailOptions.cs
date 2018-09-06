namespace Sciendo.Topper.Notifier
{
    public class EmailOptions
    {
        public string Domain { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Key { get; set; }
        public bool UseSsl { get; set; }
        public bool RequiresAuthentication { get; set; } = true;
        public string DefaultSenderEmail { get; set; }
        public string DefaultSenderDisplayName { get; set; }
        public bool UseHtml { get; set; }
    }
}
