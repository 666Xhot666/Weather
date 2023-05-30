using Microsoft.Extensions.Configuration;

namespace Weather
{
    internal class AppConfiguration
    {
        public static IConfigurationRoot Configuration { get; }
        static AppConfiguration()
        {
            Configuration = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json",optional: false,reloadOnChange:true)
            .Build();
        }
    }
}
