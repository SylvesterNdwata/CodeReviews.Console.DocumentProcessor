using CsvHelper;
using silvermax.DocumentProcessor.DbAcess;
using silvermax.DocumentProcessor.Dtos;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace silvermax.DocumentProcessor.Services;

public class ExportCSVService(ContactDbContext db) : IExportCSVService
{
    public async Task<string?> ExportContactCSVReport()
    {
        var contacts = await db.Contacts
            .Select(c => new ContactResponseDto { Id = c.Id, Name = c.Name, Email = c.Email, PhoneNumber = c.PhoneNumber })
            .ToListAsync();

        if (contacts.Count == 0)
        {
            Console.WriteLine("No contacts to export.");
            return null;
        }

        var path = Path.Combine(AppContext.BaseDirectory, "Documents", "ContactReport.csv");

        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<ContactCsvMap>();
        await csv.WriteRecordsAsync(contacts);

        return path;
    }
}
