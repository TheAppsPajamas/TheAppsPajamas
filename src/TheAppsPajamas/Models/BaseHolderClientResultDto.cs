using System;
using System.Collections.Generic;

namespace TheAppsPajamas.Models
{

    public class BaseHolderClientResultDto<TFieldDto> : BaseHolderClientDto<TFieldDto>
        where TFieldDto : BaseFieldClientDto, new()
    {

        public bool Succeeded { get; set; }
        public int Status { get; set; }

        public IList<MessageDto> Messages { get; set; } = new List<MessageDto>();
    }
}
