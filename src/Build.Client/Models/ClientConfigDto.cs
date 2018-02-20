using System;
using System.Collections.Generic;

namespace Build.Client.Models
{
    public class ClientConfigDto
    {
        public PackagingClientDto Packaging { get; set; } = new PackagingClientDto();
        public AppIconClientDto AppIcon { get; set; } = new AppIconClientDto();
        public SplashClientDto Splash { get; set; } = new SplashClientDto();
    }
}
