using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using HelpDesk.Common.Constants;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Repositories.Interfaces;
using Microsoft.Extensions.Localization;
using HelpDesk.Common.Resources;

namespace HelpDesk.Repositories.Implementations
{
    public class DbConnectionFactory(IConfiguration configuration, IStringLocalizer<Messages> localizer) : IDbConnectionFactory
    {

        private readonly IConfiguration _configuration = configuration;

        private readonly IStringLocalizer<Messages> _localizer = localizer;

        #region CreateConnection

        public IDbConnection CreateConnection()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(SystemConstant.CONNECTION_STRING_NAME)
                    ?? throw new InternalServerErrorException(_localizer["CONNECTION_STRING_NOT_FOUND"]);

                SqlConnection connection = new(connectionString);
                connection.Open();
                return connection;
            }
            catch (SqlException ex)
            {
                throw new ServiceUnavailableException(_localizer["DATABASE_CONNECTION_FAILED"], new Dictionary<string, object>
                {
                     { "reason", ex.Message }
                });
            }
        }

        #endregion
    }
}
