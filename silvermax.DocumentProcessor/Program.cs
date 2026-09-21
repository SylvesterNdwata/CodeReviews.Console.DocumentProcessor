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
                services.AddTransient<IImportService, ExcelImportService>();
                services.AddTransient<ISeedDatabaseService, SeedDatabaseService>();
                services.AddTransient<IExportPDFService, ExportPDFService>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ContactDbContext>();
        var importService = scope.ServiceProvider.GetRequiredService<IImportService>();
        var exportService = scope.ServiceProvider.GetRequiredService<IExportPDFService>();

        var seedService = new SeedDatabaseService(importService, db);

        UserInterface ui = new(seedService, exportService, db);
        await ui.Start();
    }
}
