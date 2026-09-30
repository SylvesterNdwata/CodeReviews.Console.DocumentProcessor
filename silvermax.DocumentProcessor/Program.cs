using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuestPDF.Infrastructure;
using silvermax.DocumentProcessor;
using silvermax.DocumentProcessor.DbAcess;
using silvermax.DocumentProcessor.Services;

public class Program
{
    private static async Task Main(string[] args)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<ContactDbContext>(opts =>
                    opts.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
                services.AddKeyedTransient<IImportService, ExcelImportService>("Excel");
                services.AddKeyedTransient<IImportService, CsvImportService>("CSV");
                services.AddTransient<ISeedDatabaseService, SeedDatabaseService>();
                services.AddTransient<IExportPDFService, ExportPDFService>();
                services.AddTransient<IExportCSVService, ExportCSVService>();
                services.AddSingleton<IBlobStorageService, BlobStorageService>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ContactDbContext>();
        var seedService = scope.ServiceProvider.GetRequiredService<ISeedDatabaseService>();
        var exportService = scope.ServiceProvider.GetRequiredService<IExportPDFService>();
        var exportCsvService = scope.ServiceProvider.GetRequiredService<IExportCSVService>();
        var blobStorageService = scope.ServiceProvider.GetRequiredService<IBlobStorageService>();

        UserInterface ui = new(seedService, exportService, exportCsvService, blobStorageService, db);
        await ui.Start();
    }
}
