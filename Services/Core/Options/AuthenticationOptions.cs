﻿namespace Core.Options;

public sealed class AuthenticationOptions
{
    public required string Key { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
}