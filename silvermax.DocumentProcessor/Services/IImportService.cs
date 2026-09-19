namespace silvermax.DocumentProcessor.Services;

public interface IImportService
{
    List<Contact> ImportAndProcessContacts(string filePath);
}
