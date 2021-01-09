using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Stop
    {
        public string name { get; set; }
        public int[] lines { get; set; }
    }
}
