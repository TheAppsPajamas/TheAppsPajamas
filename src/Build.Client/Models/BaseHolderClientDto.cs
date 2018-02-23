using System;
using System.Collections.Generic;

namespace Build.Client.Models
{
    public interface IBaseHolderClientDto{
        bool Disabled { get; set; }
    }

    public class BaseHolderClientDto<TFieldDto> : IBaseHolderClientDto
        where TFieldDto : BaseFieldClientDto, new()
    {
        public bool Disabled { get; set; }
        public IList<TFieldDto> Fields { get; set; } = new List<TFieldDto>();
    }
}
