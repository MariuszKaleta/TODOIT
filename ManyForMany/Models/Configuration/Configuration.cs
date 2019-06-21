using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.Models.Configuration
{

    public class Configuration
    {
        public Connectionstrings ConnectionStrings { get; set; }
        public Authentication Authentication { get; set; }
        public Logging Logging { get; set; }
        public string ClientId { get; set; }
        public string AllowedHosts { get; set; }
    }

    public class Connectionstrings
    {
        public string DefaultConnection { get; set; }
        public string LocalConnection { get; set; }
    }

    public class Authentication
    {
        public Google Google { get; set; }
        public Facebook Facebook { get; set; }
    }

    public class Google
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class Facebook
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }

    public class Logging
    {
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
    }

}
