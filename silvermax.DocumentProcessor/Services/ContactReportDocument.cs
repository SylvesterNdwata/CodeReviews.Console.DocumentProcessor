using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using silvermax.DocumentProcessor.Dtos;

namespace silvermax.DocumentProcessor.Services;

public class ContactReportDocument(List<ContactResponseDto> contacts) : IDocument
{
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.DefaultTextStyle(x => x.FontSize(11));

            page.Header()
                .Text("Contact Report")
                .SemiBold().FontSize(20);

            page.Content()
                .Padding(1, Unit.Centimetre)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Name").Bold();
                        header.Cell().Text("Email").Bold();
                        header.Cell().Text("Phone").Bold();
                    });

                    foreach (var c in contacts)
                    {
                        table.Cell().Text(c.Name);
                        table.Cell().Text(c.Email);
                        table.Cell().Text(c.PhoneNumber);
                    }
                });

            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
        });
    }
}
