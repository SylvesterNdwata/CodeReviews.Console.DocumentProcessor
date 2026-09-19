using Microsoft.EntityFrameworkCore;
using silvermax.DocumentProcessor.DbAcess;

namespace silvermax.DocumentProcessor.Services;

public class SeedDatabaseService(IImportService importService, ContactDbContext db) : ISeedDatabaseService
{
    public async Task SeedDatabase()
    {
        if (await db.Contacts.AnyAsync())
        {
            Console.WriteLine("The database already has contacts");
            return;
        }

        var contacts = importService.ImportAndProcessContacts(
            Path.Combine(AppContext.BaseDirectory, "Documents", "Contacts.xlsx"));

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
