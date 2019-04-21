using System;
using System.Collections.Generic;

namespace TheAppsPajamas.Models
{
    public interface IBaseHolderClientDto{
        bool Disabled { get; set; }
    }

    public class BaseHolderClientDto<TFieldDto, TValueDto> : IBaseHolderClientDto
        where TFieldDto : BaseFieldClientDto<TValueDto>, new()
    {
        public bool Disabled { get; set; }
        public IList<TFieldDto> Fields { get; set; } = new List<TFieldDto>();
    }
}
