using System;
namespace TheAppsPajamas.Client.Models
{
    public abstract class BaseFieldClientDto
    {
        public int FieldId { get; set; }
        public string Value { get; set; }
        public bool Disabled { get; set; }
    }
}
