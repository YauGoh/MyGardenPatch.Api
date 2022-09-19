namespace MyGardenPatch.Common;

public record FileAttachment(GardenId GardenId, ImageId ImageId, string Filename, string ContentType, Stream Stream);

public interface IFileAttachments
{
    void Add(FileAttachment attachment);

    IEnumerable<FileAttachment> GetAll();
}

public class InMemoryFileAttachments : IFileAttachments
{
    private readonly List<FileAttachment> attachments;

    public InMemoryFileAttachments()
    {
        attachments = new List<FileAttachment>();
    }

    public void Add(FileAttachment attachment) => attachments.Add(attachment);

    public IEnumerable<FileAttachment> GetAll() => attachments;
}