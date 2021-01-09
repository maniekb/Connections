using Application.Models.Connections.Responses;
using Common.Models.Responses;
using Domain.Models;
using Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Actions.Queries
{
    public class CheckConnection
    {
        public class Query : IRequest<ListResponseDto<Stop>>
        {
            public string StartStop { get; set; }
            public string DestStop { get; set; }
        }

        public class Handler : IRequestHandler<Query, ListResponseDto<Stop>>
        {
            private readonly Neo4JSettings _neo4jSettings;
            private readonly IGraphClient _client;
            public Handler(IOptions<Neo4JSettings> neo4jSettingsOptions)
            {
                _neo4jSettings = neo4jSettingsOptions.Value;
                _client = new GraphClient(new Uri(_neo4jSettings.Server), _neo4jSettings.UserName, _neo4jSettings.Password);
            }

            public async Task<ListResponseDto<Stop>> Handle(Query request, CancellationToken cancellationToken)
            {
                await _client.ConnectAsync();
                var query = $"(start: Stop {{ name: '{request.StartStop}' }}),(dest: Stop {{ name: '{request.DestStop}' }})";
                var result = await _client.Cypher.Match(query).Return<List<Stop>>("shortestPath((start) -[*]- (dest))").ResultsAsync;

                if(!result.Any())
                    return new ListResponseDto<Stop>(true, $"Cannot find connection between {request.StartStop} and {request.DestStop}.");

                var stops = result.First().Where(s => s.name != null).ToList();
                
                return new ListResponseDto<Stop>(stops);
            }
        }
    }
}
