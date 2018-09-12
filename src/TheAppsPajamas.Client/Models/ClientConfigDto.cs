﻿using System;
using System.Collections.Generic;

namespace TheAppsPajamas.Client.Models
{
    public class ClientConfigDto
    {
        public BuildConfigClientClientDto BuildConfig { get; set; } = new BuildConfigClientClientDto();
        public PackagingClientDto Packaging { get; set; } = new PackagingClientDto();
        public AppIconClientDto AppIcon { get; set; } = new AppIconClientDto();
        public SplashClientDto Splash { get; set; } = new SplashClientDto();
    }
}