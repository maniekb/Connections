using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models.Responses
{
    public class BasicResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public BasicResponseDto()
        {
        }

        public BasicResponseDto(bool success)
        {
            Success = success;
        }

        public BasicResponseDto(bool success, string message) : this(success)
        {
            Message = message;
        }
    }
}
