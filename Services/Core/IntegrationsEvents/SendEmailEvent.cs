﻿namespace Core.IntegrationsEvents;

public sealed record SendEmailEvent(
    string Origin,
    string Destiny,
    string Subject,
    string Body,
    string? Attachment);