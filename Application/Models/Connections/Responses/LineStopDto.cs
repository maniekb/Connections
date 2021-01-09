using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.Connections.Responses
{
    public class LineStopDto
    {
        public StopDto Stop { get; set; }
        public ICollection<TimeSpan> TimeTable { get; set; }
    }
}
