using Synapse.Config;
using System.ComponentModel;

namespace HelperPlus.Configs
{
    public class HelperConfig : IConfigSection
    {
        public ServerConfig Server { get; set; }
        public EnvironmentConfig Environment { get; set; }
        public ItemsConfig Items { get; set; }
        public MapConfig Map { get; set; }
        public HelperConfig()
        {
            Server = new ServerConfig();
            Environment = new EnvironmentConfig();
            Items = new ItemsConfig();
            Map = new MapConfig();
        }
    }
}
