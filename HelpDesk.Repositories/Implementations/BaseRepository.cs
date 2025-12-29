using System.Data;
using System.Data.Common;
using Dapper;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations
{
    // Base repository for providing common Dapper database operations
    public class BaseRepository(IDbConnectionFactory connectionFactory)
    {

        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        private async Task<TResult> WithConnectionAsync<TResult>(Func<IDbConnection, Task<TResult>> operation)
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            return await operation(connection);
        }

        public Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null, CommandType? commandType = null)
            => WithConnectionAsync(conn => conn.QueryFirstOrDefaultAsync<T>(sql, parameters, commandType: commandType ?? CommandType.Text));

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType? commandType = null)
            => WithConnectionAsync(conn => conn.QueryAsync<T>(sql, parameters, commandType: commandType ?? CommandType.Text));

        public Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null, CommandType? commandType = null)
            => WithConnectionAsync(conn => conn.ExecuteScalarAsync<T>(sql, parameters, commandType: commandType ?? CommandType.Text));

        public Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType? commandType = null)
            => WithConnectionAsync(conn => conn.ExecuteAsync(sql, parameters, commandType: commandType ?? CommandType.Text));

        public Task<T> QueryMultipleAsync<T>(string sql, object? parameters, Func<SqlMapper.GridReader, Task<T>> processFunc, CommandType? commandType = null)
        {
            return WithConnectionAsync(async conn =>
            {
                using SqlMapper.GridReader? grid = await conn.QueryMultipleAsync(sql, parameters, commandType: commandType ?? CommandType.Text);
                return await processFunc(grid);
            });
        }
    }

}