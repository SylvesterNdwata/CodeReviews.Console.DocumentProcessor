namespace silvermax.DocumentProcessor.Services;

public interface IExportPDFService
{
    Task<string?> ExportContactPDFReport();
}
