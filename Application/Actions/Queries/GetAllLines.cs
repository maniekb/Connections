using Common.Models.Responses;
using Domain.Models;
using Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using Neo4jClient;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Actions.Queries
{
    public class GetAllLines 
    {
        public class Query : IRequest<ListResponseDto<int>>
        {
        }

        public class Handler : IRequestHandler<Query, ListResponseDto<int>>
        {
            private readonly Neo4JSettings _neo4jSettings;
            private readonly IGraphClient _client;
            public Handler(IOptions<Neo4JSettings> neo4jSettingsOptions)
            {
                _neo4jSettings = neo4jSettingsOptions.Value;
                _client = new GraphClient(new Uri(_neo4jSettings.Server), _neo4jSettings.UserName, _neo4jSettings.Password);             
            }

            public async Task<ListResponseDto<int>> Handle(Query request, CancellationToken cancellationToken)
            {
                await _client.ConnectAsync();
                var result = await _client.Cypher.Match("(stop: Stop)")
                    .Return(stop => stop.As<Stop>())
                    .ResultsAsync;

                if(!result.Any())
                    return new ListResponseDto<int>(true);

                var lines = result.SelectMany(x => x.lines).Distinct().ToList();

                return new ListResponseDto<int>(lines);
            }
        }
    }
}
