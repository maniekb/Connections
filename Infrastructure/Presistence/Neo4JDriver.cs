using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using System;

namespace Infrastructure.Presistence
{
    public class Neo4JDriver : IDisposable
    {
        private readonly Neo4JSettings _neo4jSettings;
        public IDriver Driver;
        public Neo4JDriver(IOptions<Neo4JSettings> neo4jSettingsOptions)
        {
            _neo4jSettings = neo4jSettingsOptions.Value;
            Driver = GraphDatabase.Driver(_neo4jSettings.Server, AuthTokens.Basic(_neo4jSettings.UserName, _neo4jSettings.Password));
        }

        public void Dispose()
        {
            Driver?.Dispose();
        }
    }
}
