using System;
using System.Collections.Generic;

namespace Build.Client.Models
{
    public class ClientConfigDto
    {
        public IList<StringFieldClientDto> PackagingFields { get; set; }
        public IList<MediaFieldClientDto> AppIconFields { get; set; }
        public IList<MediaFieldClientDto> SplashFields { get; set; }

    }
}
