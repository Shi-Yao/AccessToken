namespace AcessToken.DbSetting
{
    public class DbSettings : IDbSettings
    {
        public string ConfigTemplate { get; set; }

        public DbSettings(IConfiguration configuration)
        {
            ConfigTemplate = configuration.GetConnectionString("ShoppingMartConnection");
        }
    }
}