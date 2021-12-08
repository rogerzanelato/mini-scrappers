using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AmbevBrandScrapper;
using AngleSharp;
using AngleSharp.Dom;
using static AmbevBrandScrapper.ScrapperHelper;

using var streamReader = new StreamReader("input.json");
var marcas = await JsonSerializer.DeserializeAsync<IList<MarcaInput>>(streamReader.BaseStream, new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});

if (!marcas.Any())
{
    Console.WriteLine("Nenhum link para scrapping foi encontrado.");
    return;
}

Console.WriteLine($"Iniciando scrapping de {marcas.Count} links");
var stopwatcher = Stopwatch.StartNew();

await ExecuteAsync(marcas);

stopwatcher.Stop();
Console.WriteLine($"Scrapping finalizado em {stopwatcher.ElapsedMilliseconds}ms");

static async Task ExecuteAsync(IList<MarcaInput> marcas)
{
    var context = CriarBrowsingContext();
    var cervejas = await Task.WhenAll(marcas.Select(async marca => await ExecutarScrapping(context, marca)));

    await SalvarOutputAsync(cervejas.Flatten());
}

static async Task<IEnumerable<CervejaOutput>> ExecutarScrapping(IBrowsingContext browsingContext, MarcaInput marca)
{
    var document = await browsingContext.OpenAsync(marca.Link);

    var cervejasLinks = ExtrairLinksCervejas(document);
    var cervejasTasks = cervejasLinks.Select(async cervejaLink => await ExtrairCervejaAsync(browsingContext, marca, cervejaLink));

    return await Task.WhenAll(cervejasTasks);
}

static async Task<CervejaOutput> ExtrairCervejaAsync(IBrowsingContext browsingContext, MarcaInput marca, string cervejaLink)
{
    Console.WriteLine($"Extraindo cerveja {cervejaLink}, Marca: {marca.Nome}");
    var document = await browsingContext.OpenAsync(cervejaLink).WaitUntilAvailable();
    return ExtrairCerveja(document, marca);
}

static async Task SalvarOutputAsync(IEnumerable<CervejaOutput> cervejas)
{
    var json = JsonSerializer.Serialize(cervejas, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    });

    const string resultFolderPath = @"C:\Users\roger.zanelato\RiderProjects\PolterGlastScrapper\AmbevBrandScrapper";

    await File.WriteAllTextAsync($"{resultFolderPath}\\output.json",json);
}
