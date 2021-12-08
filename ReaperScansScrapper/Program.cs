using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using static ReaperScansScrapper.ScrapperHelper;

var links = await File.ReadAllLinesAsync("input.txt");
if (!links.Any())
{
    Console.WriteLine("Nenhum link para scrapping foi encontrado.");
    return;
}

Console.WriteLine($"Iniciando scrapping de {links.Length} links");
var stopwatcher = Stopwatch.StartNew();

await ExecuteAsync(links);

stopwatcher.Stop();
Console.WriteLine($"Scrapping finalizado em {stopwatcher.ElapsedMilliseconds}ms");

static async Task ExecuteAsync(IEnumerable<string> links)
{
    var context = CriarBrowsingContext();
    var scrappingTasks = links.Select(async link => await ExecutarScrapping(context, link));
    await Task.WhenAll(scrappingTasks);
}

static async Task ExecutarScrapping(IBrowsingContext browsingContext, string link)
{
    var document = await browsingContext.OpenAsync(link);

    var titulo = ExtrairTitulo(document);
    var conteudo = ExtrairConteudoSanitizado(document);

    await SalvarArquivoAsync(titulo, conteudo);
}

static async Task SalvarArquivoAsync(string titulo, string conteudo)
{
    const string resultFolderPath = @"C:\Users\roger.zanelato\RiderProjects\PolterGlastScrapper\ReaperScansScrapper\results";

    var invalids = Path.GetInvalidFileNameChars();
    var tituloSanitizado = string
        .Join("", titulo.Split(invalids, StringSplitOptions.RemoveEmptyEntries))
        .Replace("  ", " ")
        .TrimEnd('.');

    await File.WriteAllTextAsync($"{resultFolderPath}\\{tituloSanitizado}.html", conteudo);
}
