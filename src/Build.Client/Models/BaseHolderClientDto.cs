using System;
using System.Collections.Generic;

namespace Build.Client.Models
{
    public class BaseHolderClientDto<TFieldDto>
        where TFieldDto : BaseFieldClientDto, new()
    {
        public bool Disabled { get; set; }
        public IList<TFieldDto> Fields { get; set; } = new List<TFieldDto>();
    }
}
