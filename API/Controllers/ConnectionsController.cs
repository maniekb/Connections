using Application.Actions.Commands;
using Application.Actions.Queries;
using Application.Models.Connections.Responses;
using Common.Models.Responses;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/connections")]
    public class ConnectionsController : BaseController
    {
        [HttpGet("")]
        public Task<ListResponseDto<int>> GetAllLines(CancellationToken cancellationToken)
            =>  
            Mediator.Send(new GetAllLines.Query(), cancellationToken);

        [HttpGet("line/{lineNumber}")]
        public Task<ListResponseDto<LineDto>> GetLine([FromRoute]int lineNumber, CancellationToken cancellationToken)
            => Mediator.Send(new GetLine.Query(lineNumber), cancellationToken);

        [HttpGet("connection")]
        public Task<ListResponseDto<Stop>> GetConection([FromQuery]CheckConnection.Query request, CancellationToken cancellationToken)
            => Mediator.Send(request, cancellationToken);

        [HttpPost]
        public Task<BasicResponseDto> CreateLine([FromBody]CreateLine.Command request, CancellationToken cancellationToken)
            => Mediator.Send(request, cancellationToken);

        [HttpPut("{lineNumber}")]
        public Task<BasicResponseDto> UpdateLine([FromBody]UpdateLine.Command request, CancellationToken cancellationToken)
            => Mediator.Send(request, cancellationToken);

        [HttpDelete("{lineNumber}")]
        public Task<BasicResponseDto> RemoveLine([FromRoute]int lineNumber, CancellationToken cancellationToken)
            => Mediator.Send(new RemoveLine.Command(lineNumber), cancellationToken);


    }
}
