using System;
namespace TheAppsPajamas.Models
{
    public abstract class BaseFieldClientDto
    {
        public string FieldId { get; set; }
        public string Value { get; set; }
        public bool Disabled { get; set; }
    }
}
