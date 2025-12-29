using System.Data;

namespace HelpDesk.Repositories.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}