using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using silvermax.DocumentProcessor.DbAcess;
using silvermax.DocumentProcessor.Dtos;

namespace silvermax.DocumentProcessor.Services;

public class ExportPDFService(ContactDbContext db) : IExportPDFService
{
    public async Task ExportContactPDFReport()
    {
        var contacts = await db.Contacts
            .Select(c => new ContactResponseDto { Id = c.Id, Name = c.Name, Email = c.Email, PhoneNumber = c.PhoneNumber })
            .ToListAsync();

        if (contacts.Count == 0)
        {
            Console.WriteLine("No contacts to export.");
            return;
        }

        try
        {
            new ContactReportDocument(contacts).GeneratePdf(
                Path.Combine(AppContext.BaseDirectory, "Documents", "ContactReport.pdf"));
            Console.WriteLine("ContactReport.pdf successfully generated");
        }
        catch (IOException ex)
        {
            throw new IOException("Could not write ContactReport.pdf.");
        }
        
    }
}
