using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.Connections.Responses
{
    public class LineDto
    {
        public int lineNumber { get; set; }
        public string[] stops { get; set; }
    }
}
