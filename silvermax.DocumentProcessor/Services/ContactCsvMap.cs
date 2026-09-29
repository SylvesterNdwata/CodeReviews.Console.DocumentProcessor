using CsvHelper.Configuration;
using silvermax.DocumentProcessor.Dtos;

namespace silvermax.DocumentProcessor.Services;

public sealed class ContactCsvMap : ClassMap<ContactResponseDto>
{
    public ContactCsvMap()
    {
        Map(m => m.Name);
        Map(m => m.Email);
        Map(m => m.PhoneNumber);
    }
}
