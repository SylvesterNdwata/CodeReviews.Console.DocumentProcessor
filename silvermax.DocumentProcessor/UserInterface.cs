using Microsoft.EntityFrameworkCore;
using silvermax.DocumentProcessor.DbAcess;
using silvermax.DocumentProcessor.Dtos;
using silvermax.DocumentProcessor.Services;
using Spectre.Console;


namespace silvermax.DocumentProcessor;

public class UserInterface(ISeedDatabaseService seedService, IExportPDFService exportService, ContactDbContext db)
{

    public async Task Start()
    {
        Console.WriteLine("Starting application...");

        await seedService.SeedDatabase();

        await PrintContacts();

        await exportService.ExportContactPDFReport();

    }

    private async Task PrintContacts()
    {
        var contacts = await db.Contacts
            .Select(c => new ContactResponseDto { Id = c.Id, Name = c.Name, Email = c.Email, PhoneNumber = c.PhoneNumber })
            .ToListAsync();

        var table = new Table { Border = TableBorder.Rounded };
        table.AddColumn("[yellow]Name[/]");
        table.AddColumn("[yellow]Email[/]");
        table.AddColumn("[yellow]Phone Number[/]");

        foreach (var c in contacts)
        {
            table.AddRow(
                $"[purple]{c.Name}[/]",
                $"[purple]{c.Email}[/]",
                $"[purple]{c.PhoneNumber}[/]"
                );
        }

        AnsiConsole.Write(table);
    }
}
