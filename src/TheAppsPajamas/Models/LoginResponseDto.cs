﻿using System;
namespace TheAppsPajamas.Models
{
    public class LoginResponseDto
    {
        public string access_token { get; set; }
        public string id_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
    }
}
