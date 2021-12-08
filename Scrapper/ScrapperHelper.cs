using System.Linq;
using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace Scrapper
{
    public static class ScrapperHelper
    {
        public static IBrowsingContext CriarBrowsingContext()
        {
            var config = Configuration.Default.WithDefaultLoader();
            return BrowsingContext.New(config);
        }

        public static string ExtrairTitulo(IDocument document)
        {
            var element = document.QuerySelector("h3.post-title.entry-title a");
            return CorrigirTituloCapitulo(element.TextContent);
        }

        /// <summary>
        /// Recebe um titulo como "Meu titulo 1" e transforma em "Meu titulo 01"
        /// </summary>
        private static string CorrigirTituloCapitulo(string titulo)
        {
            var parts = titulo.Split(" ").ToList();
            var chapter = parts.Last();

            parts.RemoveAt(parts.Count - 1);

            return string.Join(" ", parts.Append(chapter.PadLeft(2, '0')));
        }

        public static string ExtrairConteudoSanitizado(IDocument document)
        {
            var element = document.QuerySelector("div.post-body.entry-content");

            RemoverElementosDesnecessarios(element);
            SanitizarCss(element);

            return element.InnerHtml;
        }

        private static void RemoverElementosDesnecessarios(IElement element)
        {
            // Remover botões de compartilhamento
            element.QuerySelector("div.post-share-buttons").Remove();
            element.QuerySelector("span.reaction-buttons").Remove();

            // Remover parágrafos vazios
            element.QuerySelectorAll("p")
                .Where(el => string.IsNullOrEmpty(el.TextContent.Trim()))
                .ToList()
                .ForEach(el => el.Remove());
        }

        private static void SanitizarCss(IElement element)
        {
            var paragrafos = element.QuerySelectorAll("p").ToList();
            paragrafos.ForEach(FormatarParagrafo);
        }

        private static void FormatarParagrafo(IElement paragrafo)
        {
            LimparEstilizacao(paragrafo);
            paragrafo.SetAttribute("style", "line-height: 200%;text-align: justify;");

            paragrafo.Children.OfType<IHtmlSpanElement>().ToList().ForEach(FormatarSpan);
        }

        private static void FormatarSpan(IHtmlSpanElement span)
        {
            LimparEstilizacao(span);
            span.SetAttribute("style", "line-height: 200%;font-size: 12pt;");

            span.Children.OfType<IHtmlSpanElement>().ToList().ForEach(FormatarChildSpan);
        }

        private static void FormatarChildSpan(IHtmlSpanElement childSpan)
        {
            LimparEstilizacao(childSpan);
            childSpan.SetAttribute("style", "font-family: arial;");
        }

        private static void LimparEstilizacao(IElement element)
        {
            element.RemoveAttribute("class");
            element.RemoveAttribute("style");
        }
    }
}
