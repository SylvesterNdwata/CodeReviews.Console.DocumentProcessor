using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace silvermax.DocumentProcessor.Services;

public class CsvImportService : IImportService
{
    public List<Contact> ImportAndProcessContacts(string filePath)
    {
        var contacts = new List<Contact>();

        try
        {
            using var reader = new StreamReader(filePath);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };

            using var csv = new CsvReader(reader, config);

            foreach (var row in csv.GetRecords<ContactCsvRow>())
            {
                contacts.Add(new Contact
                {
                    Id = Guid.NewGuid(),
                    Name = row.Name.Trim(),
                    Email = row.Email.Trim(),
                    PhoneNumber = row.PhoneNumber.Trim()
                });
            }
        }
        catch(FileNotFoundException ex)
        {
            throw new FileNotFoundException($"The file '{filePath}' could not be found", filePath, ex);
        }

        return contacts;
    }

    private class ContactCsvRow
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
    }

}

