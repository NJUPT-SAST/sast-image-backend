﻿using System.Data;
using Npgsql;

namespace SastImg.Infrastructure.Persistence.QueryDatabase
{
    internal sealed class DbConnectionFactory(string connectionString)
        : IDbConnectionFactory,
            IDisposable
    {
        private readonly string _connectionString = connectionString;
        private IDbConnection? _connection = null;

        public void Dispose()
        {
            if (_connection is not null && _connection.State == ConnectionState.Open)
            {
                _connection.Dispose();
            }
        }

        public IDbConnection GetConnection()
        {
            if (_connection is null || _connection.State != ConnectionState.Open)
            {
                _connection = new NpgsqlConnection(_connectionString);
            }
            return _connection;
        }
    }
}
