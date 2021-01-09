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

namespace Application.Actions.Commands
{
    public class RemoveLine
    {
        public class Command : IRequest<BasicResponseDto>
        {
            public int LineNo { get; set; }

            public Command(int lineNo)
            {
                LineNo = lineNo;
            }
        }

        public class Handler : IRequestHandler<Command, BasicResponseDto>
        {
            private readonly Neo4JSettings _neo4jSettings;
            private readonly IGraphClient _client;
            public Handler(IOptions<Neo4JSettings> neo4jSettingsOptions)
            {
                _neo4jSettings = neo4jSettingsOptions.Value;
                _client = new GraphClient(new Uri(_neo4jSettings.Server), _neo4jSettings.UserName, _neo4jSettings.Password);
            }

            public async Task<BasicResponseDto> Handle(Command request, CancellationToken cancellationToken)
            {
                await _client.ConnectAsync();

                await _client.ConnectAsync();
                var stops = await _client.Cypher.Match($"(stop: Stop) WHERE {request.LineNo} in stop.lines")
                    .Return(stop => stop.As<Stop>())
                    .ResultsAsync;

                if (!stops.Any())
                    return new ListResponseDto<int>(false, $"Line with number: {request.LineNo} does not exist.");

                // Delete old line

                foreach (var stopToDelete in stops)
                {
                    if (stopToDelete.lines.Count() == 1)
                    {
                        await _client.Cypher.Match($"(stop: Stop {{ name: '{stopToDelete.name}' }})").DetachDelete("stop")
                            .ExecuteWithoutResultsAsync();
                    }
                    else
                    {
                        var stopLines = stopToDelete.lines.ToList();
                        stopLines.RemoveAll(x => x == request.LineNo);
                        var array = $"[{string.Join(",", stopLines.Distinct().ToArray())}]";
                        var query = $"stop.lines = {array}";
                        await _client.Cypher.Match($"(stop: Stop {{ name: '{stopToDelete.name}' }})").Set(query)
                            .ExecuteWithoutResultsAsync();
                    }
                }

                return new BasicResponseDto(true);
            }
        }
    }
}
