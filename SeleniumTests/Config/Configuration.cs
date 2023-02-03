using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTests.Config
{
    public class Configuration
    {
        public string Browser { get; set; }
        public bool IsRemote { get; set; }
        public Uri RemoteAddress { get; set; }
        public string PlatformName { get; set; }
        public string BaseUrl { get; set; }
    }
}
