using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models.Responses
{
    public class ListResponseDto<TDto> : BasicResponseDto
    {
        public ICollection<TDto> Items { get; set; }

        protected ListResponseDto() { }

        public ListResponseDto(bool success) : base(success) { }
        public ListResponseDto(bool success, string message) : base(success, message) { }

        public ListResponseDto(ICollection<TDto> items) : base(true) => Items = items;

        public ListResponseDto(TDto item) : base(true)
        {
            if (item != null)
                Items = new List<TDto> { item };
        }
    }
}
