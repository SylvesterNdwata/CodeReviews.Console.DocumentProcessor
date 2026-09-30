using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using silvermax.DocumentProcessor.DbAcess;
using Spectre.Console;

namespace silvermax.DocumentProcessor.Services;

public class SeedDatabaseService(IServiceProvider serviceProvider, IBlobStorageService blobStorageService, ContactDbContext db) : ISeedDatabaseService
{
    public async Task SeedDatabase()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Should the contacts be seeded from the CSV file or Excel file?:")
            .AddChoices("Excel", "CSV"));

        var importService = serviceProvider.GetRequiredKeyedService<IImportService>(choice);
        var fileName = choice == "Excel" ? "Contacts.xlsx" : "Contacts.csv";

        await blobStorageService.DownloadFileAsync(fileName, Path.Combine(AppContext.BaseDirectory, "Documents", fileName));

        var contacts = importService.ImportAndProcessContacts(
            Path.Combine(AppContext.BaseDirectory, "Documents", fileName));

        var existingPhoneNumbers = (await db.Contacts.Select(c => c.PhoneNumber).ToListAsync()).ToHashSet();
        var newContacts = contacts.Where(c => !existingPhoneNumbers.Contains(c.PhoneNumber)).ToList();

        if (newContacts.Count == 0)
        {
            Console.WriteLine("No new contacts to add.");
            return;
        }

        db.Contacts.AddRange(newContacts);
        await db.SaveChangesAsync();
        Console.WriteLine("The contacts were added sucessfully");
    }
}
