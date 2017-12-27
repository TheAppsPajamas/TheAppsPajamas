using System;
namespace Build.Client.Models
{
    public abstract class BaseFieldClientDto
    {
        public int FieldId { get; set; }
        public string Value { get; set; }
    }
}
