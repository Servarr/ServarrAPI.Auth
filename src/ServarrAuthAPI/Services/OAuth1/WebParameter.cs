﻿namespace ServarrAuthAPI.Services.OAuth1
{
    public class WebParameter
    {
        public WebParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Value { get; set; }
        public string Name { get; set; }
    }
}
