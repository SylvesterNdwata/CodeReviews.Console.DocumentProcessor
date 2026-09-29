using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using silvermax.DocumentProcessor.DbAcess;
using Spectre.Console;

namespace silvermax.DocumentProcessor.Services;

public class SeedDatabaseService(IServiceProvider serviceProvider, ContactDbContext db) : ISeedDatabaseService
{
    public async Task SeedDatabase()
    {
        if (await db.Contacts.AnyAsync())
        {
            Console.WriteLine("The database already has contacts");
            return;
        }

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Should the contacts be seeded from the CSV file or Excel file?:")
            .AddChoices("Excel", "CSV"));

        var importService = serviceProvider.GetRequiredKeyedService<IImportService>(choice);
        var fileName = choice == "Excel" ? "Contacts.xlsx" : "Contacts.csv";

        var contacts = importService.ImportAndProcessContacts(
            Path.Combine(AppContext.BaseDirectory, "Documents", fileName));

        if (contacts is null)
        {
            Console.WriteLine("The excel file is empty. There were no contacts to add");
            return;
        }

        db.Contacts.AddRange(contacts);
        await db.SaveChangesAsync();
        Console.WriteLine("The contacts were added sucessfully");
    }
}
