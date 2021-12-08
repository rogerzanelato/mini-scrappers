using System;

namespace AmbevBrandScrapper
{
    public record MarcaInput(Guid Id, string LogoImg, string Link, string Nome);

    public record CervejaOutput
    {
        public Guid Id { get; init; }
        public Guid MarcaId { get; init; }
        public string Nome { get; init; }
        public string Descricao { get; init; }
        public string DescricaoMarca { get; init; }
        public string Link { get; init; }
        public string Harmonizacao { get; init; }
        public string Ingredientes { get; init; }
        public string TeorAlcoolico { get; init; }
        public string TemperaturaIdeal { get; init; }
        public string Ibu { get; init; }
        public string Tipo { get; init; }
        public string Img { get; init; }
        public string OrigemImg { get; init; }
    }
}
