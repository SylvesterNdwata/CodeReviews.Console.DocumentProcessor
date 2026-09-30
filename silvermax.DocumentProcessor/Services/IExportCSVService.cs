namespace silvermax.DocumentProcessor.Services;

public interface IExportCSVService
{
    Task <string?> ExportContactCSVReport();
}
