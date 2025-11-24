namespace KubeFood.Core.ApiServices.MailSender;

public sealed class MailSenderModel
{
    public required string[] To { get; init; }
    public required string Subject { get; init; }
    public string[]? Bcc { get; init; }
    public string[]? Cc { get; init; }
    public string? HtmlContent { get; init; }
    public string? TextContent { get; init; }
    public IEnumerable<MailSenderAttachmentModel>? Attachments { get; init; }

    public MailSenderModel(
        string[] to,
        string subject,
        string[]? bcc = null,
        string[]? cc = null,
        string? htmlContent = null,
        string? textContent = null,
        IEnumerable<MailSenderAttachmentModel>? attachments = null)
    {
        To = to;
        Subject = subject;
        Bcc = bcc ?? [];
        Cc = cc ?? [];
        HtmlContent = htmlContent;
        TextContent = textContent;
        Attachments = attachments ?? [];
    }

    public MailSenderModel(
        string[] to,
        string subject,
        string? htmlContent = null,
        string? textContent = null,
        IEnumerable<MailSenderAttachmentModel>? attachments = null)
        : this(to, subject, null, null, htmlContent, textContent, attachments)
    { }

    public MailSenderModel(
        string to,
        string subject,
        string? htmlContent = null,
        string? textContent = null,
        IEnumerable<MailSenderAttachmentModel>? attachments = null)
        : this([to], subject, null, null, htmlContent, textContent, attachments)
    { }
}

public sealed class MailSenderAttachmentModel
{
    public string Content { get; init; }
    public string Filename { get; init; }
    public string Path { get; init; }
    public string? ContentType { get; init; }

    public MailSenderAttachmentModel(
        string content,
        string filename,
        string path,
        string? contentType = null)
    {
        Content = content;
        Filename = filename;
        Path = path;
        ContentType = contentType;
    }
}