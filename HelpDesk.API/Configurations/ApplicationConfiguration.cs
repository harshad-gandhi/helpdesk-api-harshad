namespace HelpDesk.API.Configurations
{
    public class ApplicationConfiguration
    {

        #region BuilderConfiguration

        // Configures services and dependencies before the application builds.
        public void ExecuteBuilderConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.ConfigureLocalization();

            builder.Services.ConfigureDependencies();

            builder.Services.ConfigureCors();

            builder.Services.ConfigureSwagger();

            builder.Services.ConfigureAuthentication(builder.Configuration);

            builder.ConfigureLogger();

            builder.Services.AddHttpClient();

            builder.Services.AddSignalR();

        }

        #endregion

        #region AppConfiguration

        // Configures middleware components after the application is built.
        public void ExecuteAppConfiguration(WebApplication app)
        {
            app.UseLocalization();

            app.UseSwaggerDocumentation();

            app.ConfigureMiddlewares();
        }

        #endregion

    }
}