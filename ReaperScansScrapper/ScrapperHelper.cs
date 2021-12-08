using AngleSharp;
using AngleSharp.Dom;

namespace ReaperScansScrapper
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
            var element = document.QuerySelector("h1#chapter-heading");
            return element.TextContent;
        }

        public static string ExtrairConteudoSanitizado(IDocument document)
        {
            var element = document.QuerySelector("div.reading-content");
            return element.InnerHtml;
        }
    }
}
