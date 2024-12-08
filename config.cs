using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using IniParser;
using IniParser.Model;

namespace DataBass
{


    public class Config
    {
        public string Host { get; private set; }
        public string Port { get; private set; }
        public string Database { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public Config(string configFilePath)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(configFilePath);

            Host = data["Database"]["Host"];
            Port = data["Database"]["Port"];
            Database = data["Database"]["Database"];
            Username = data["Database"]["Username"];
            Password = data["Database"]["Password"];
        }
    }

}
