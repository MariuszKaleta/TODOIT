namespace TODOIT.Model.Configuration
{

    public class Configuration
    {
        public Connectionstrings ConnectionStrings { get; set; }
        public Authentication Authentication { get; set; }
        public Logging Logging { get; set; }
        public string ClientId { get; set; }
        public string AllowedHosts { get; set; }

        public string Policy { get; set; }
    }
}
