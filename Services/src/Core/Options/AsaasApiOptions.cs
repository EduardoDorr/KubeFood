﻿namespace Core.Options;

public sealed class AsaasApiOptions
{
    public required string ApiKey { get; set; }
    public required string WebhookToken { get; set; }
    public required string BaseUrl { get; set; }
    public required string CustomerEndpoint { get; set; }
    public required string PaymentEndpoint { get; set; }
}