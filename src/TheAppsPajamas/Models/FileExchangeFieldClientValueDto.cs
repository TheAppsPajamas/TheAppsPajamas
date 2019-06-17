using System;
namespace TheAppsPajamas.Models
{
    public class FileExchangeFieldClientValueDto
    {
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }

        public string BuildAction { get; set; }
    }
}
