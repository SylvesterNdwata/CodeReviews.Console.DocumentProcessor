using ClosedXML.Excel;

namespace silvermax.DocumentProcessor.Services;

public class ExcelImportService : IImportService
{
    public List<Contact> ImportAndProcessContacts(string filePath)
    {
        var contacts = new List<Contact>();
        try
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);

            var headerRow = worksheet.Row(1);
            var columnMap = headerRow.CellsUsed()
                .ToDictionary(cell => cell.GetString().Trim(), cell => cell.Address.ColumnNumber);

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                if (row.IsEmpty()) continue;

                var contact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Name = row.Cell(columnMap["Name"]).GetString().Trim(),
                    PhoneNumber = row.Cell(columnMap["PhoneNumber"]).GetString().Trim(),
                    Email = row.Cell(columnMap["Email"]).GetString().Trim(),
                };

                contacts.Add(contact);
            }
        }
        catch (FileNotFoundException ex)
        {
            throw new FileNotFoundException($"The file '{filePath}' could not  be found", filePath, ex);
        }
        catch (FileFormatException ex)
        {
            throw new FileFormatException($"The file '{filePath}' is not a valid Excel file or is corrupted.", ex);
        }

        return contacts;
    }
}
