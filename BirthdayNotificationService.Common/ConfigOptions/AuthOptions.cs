namespace BirthdayNotificationService.Common.ConfigOptions
{
    public class AuthOptions
    {
        public string Token { get; set; }
        public string BirthdaySheduleTelegramBotToken { get; set; }

        public bool UseTelegramProxy { get; set; }
        public string Socks5Hostname { get; set; }
        public int Socks5Port { get; set; }
        public string Socks5Username { get; set; }
        public string Socks5Password { get; set; }
    }
}