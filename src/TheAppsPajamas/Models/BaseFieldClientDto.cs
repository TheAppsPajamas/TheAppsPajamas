using System;
namespace TheAppsPajamas.Models
{
    public abstract class BaseFieldClientDto<TValueDto>
    {
        public string FieldId { get; set; }
        public TValueDto Value { get; set; }
        public bool Disabled { get; set; }
    }
}
