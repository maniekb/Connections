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
    public class GetLine
    {
        public class Query : IRequest<ListResponseDto<LineDto>>
        {
            public int Number { get; set; }

            public Query(int number)
            {
                Number = number;
            }
        }

        public class Handler : IRequestHandler<Query, ListResponseDto<LineDto>>
        {
            private readonly Neo4JSettings _neo4jSettings;
            private readonly IGraphClient _client;
            public Handler(IOptions<Neo4JSettings> neo4jSettingsOptions)
            {
                _neo4jSettings = neo4jSettingsOptions.Value;
                _client = new GraphClient(new Uri(_neo4jSettings.Server), _neo4jSettings.UserName, _neo4jSettings.Password);
            }

            public async Task<ListResponseDto<LineDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                await _client.ConnectAsync();
                var query = $"(line: Line) WHERE line.lineNumber = {request.Number}";
                var lines = await _client.Cypher.Match(query)
                    .Return(line => line.As<Line>())
                    .ResultsAsync;

                if (!lines.Any())
                    return new ListResponseDto<LineDto>(true, "Line not found.");

                var lineDto = new LineDto()
                {
                    lineNumber = lines.First().lineNumber,
                    stops = lines.First().stops
                };

                return new ListResponseDto<LineDto>(lineDto);
            }
        }
    }
}
