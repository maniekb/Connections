using Application.Models.Connections.Responses;
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
    public class CreateLine
    {
        public class Command : LineDto, IRequest<BasicResponseDto>
        {
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
                var line = await _client.Cypher.Match($"(stop: Stop) WHERE {request.lineNumber} in stop.lines")
                    .Return(stop => stop.As<Stop>())
                    .ResultsAsync;

                if (line.Any())
                    return new ListResponseDto<int>(false, $"Line with number: {request.lineNumber} arleady exists.");

                // Create stops
                foreach (var stopName in request.stops)
                {
                    var entities = await _client.Cypher.Match($"(stop: Stop) WHERE stop.name = '{stopName}'")
                        .Return(stop => stop.As<Stop>())
                        .ResultsAsync;
                        
                    var entity = entities.FirstOrDefault();

                    if(entity == null)
                    {
                        var stopQuery = $"(stop: Stop {{ name: '{stopName}', lines: [{request.lineNumber}]}})";
                        await _client.Cypher.Create(stopQuery)
                            .ExecuteWithoutResultsAsync();
                    }
                    else
                    {
                        var stopLines = entity.lines.ToList();
                        stopLines.Add(request.lineNumber);
                        var array = $"[{string.Join(",", stopLines.Distinct().ToArray())}]";
                        var query = $"stop.lines = {array}";
                        await _client.Cypher.Match($"(stop: Stop {{ name: '{stopName}' }})").Set(query)
                            .ExecuteWithoutResultsAsync();
                    }
                }

                // Create relations
                var stopsList = request.stops.ToList();
                for(int i = 0; i < stopsList.Count - 1; i++)
                {
                    var start = stopsList.ElementAt(i);
                    var dest = stopsList.ElementAt(i + 1);

                    var relationQuery = $"(a)-[r:LEADS_TO]->(b)";
                    await _client.Cypher.Match($"(a:Stop),(b:Stop) WHERE a.name = '{start}' AND b.name = '{dest}'")
                        .Merge(relationQuery)
                        .ExecuteWithoutResultsAsync();

                    await _client.Cypher.Match($"(a:Stop),(b:Stop) WHERE a.name = '{dest}' AND b.name = '{start}'")
                        .Merge(relationQuery)
                        .ExecuteWithoutResultsAsync();

                }

                // Create line
                var arrayOfStops = String.Join(",", request.stops.Select(x => string.Format("\"{0}\"", x)));
                var lineQuery = $"(line: Line {{ lineNumber: {request.lineNumber}, stops: [{arrayOfStops}]}})";
                await _client.Cypher.Create(lineQuery)
                    .ExecuteWithoutResultsAsync();

                return new BasicResponseDto(true);
            }
        }
    }
}
