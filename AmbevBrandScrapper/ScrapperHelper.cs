using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace AmbevBrandScrapper
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> arr)
        {
            return arr.SelectMany(x => x);
        }
    }

    public static class ScrapperHelper
    {
        public static IBrowsingContext CriarBrowsingContext()
        {
            var config = Configuration.Default.WithDefaultLoader().WithXPath().WithJs();
            return BrowsingContext.New(config);
        }

        /// <summary>
        /// Extraí os links das cervejas da seguinte página
        /// https://www.ambev.com.br/marcas/cervejas/brahma
        /// </summary>
        public static IEnumerable<string> ExtrairLinksCervejas(IDocument document)
        {
            var elements = document.QuerySelectorAll(".product__more .product__link");
            return elements
                .OfType<IHtmlAnchorElement>()
                .Select(element => element.Href);
        }

        /// <summary>
        /// Extraí informações de determinada cerveja da seguinte página
        /// https://www.ambev.com.br/marcas/cervejas/brahma/brahma-chopp-pilsen/
        /// </summary>
        public static CervejaOutput ExtrairCerveja(IDocument document, MarcaInput marca)
        {
            var titulo = document.ExtrairTexto(".title.product-info__title");
            var tipo = document.ExtrairTexto(".product-info__category .product-info__tag");
            var descricao = document.ExtrairTexto(".product-info__description--has-socials");

            var harmonizacao = document.QuerySelectorAll(".product-info__item")
                .FirstOrDefault(x => x.TextContent.Contains("Harmonização"))
                ?.TextContent
                ?.Replace("Harmonização", string.Empty)
                ?.Trim();

            var ingredientes = document.QuerySelectorAll(".product-info__text-feature")
                .FirstOrDefault(x => x.TextContent.Contains("Ingredientes:"))
                ?.TextContent
                ?.Replace("Ingredientes:", string.Empty)
                ?.Trim();

            var teorAlcoolico = document.ExtrairTexto("p.product-info__item-icon--alcohol-content")
                ?.Replace("Teor alcoólico", string.Empty)
                ?.Trim();

            var temperaturaIdeal = document.ExtrairTexto("p.product-info__item-icon--ideal-temperature")
                ?.Replace("Temperatura ideal", string.Empty)
                ?.Trim();

            var ibu = document.ExtrairTexto("p.product-info__item-icon--ibu")
                ?.Replace("IBU", string.Empty)
                ?.Trim();

            var img = document.QuerySelector(".product-info__pack-image.product-info__pack-image--active")
                ?.GetAttribute("data-image-src");
            var origemImg = document.ExtrairImagem(".product-info__category .product-info__country-holder img");

            return new CervejaOutput
            {
                Id = Guid.NewGuid(),
                MarcaId = marca.Id,
                Nome = titulo,
                Descricao = descricao,
                DescricaoMarca = marca.Nome,
                Link = document.Location.ToString(),
                Harmonizacao = harmonizacao,
                Ingredientes = ingredientes,
                TeorAlcoolico = teorAlcoolico,
                TemperaturaIdeal = temperaturaIdeal,
                Ibu = ibu,
                Tipo = tipo,
                Img = img,
                OrigemImg = origemImg
            };
        }

        private static string ExtrairTexto(this IDocument document, string selector) =>
            document.QuerySelector(selector)?.TextContent?.Trim()?.Replace("  ", " ");

        private static string ExtrairImagem(this IDocument document, string selector) =>
            document.QuerySelector(selector)?.GetAttribute("src");
    }
}
