namespace Service.DTO.Filtros
{
    public class ClassificacaoContabilFiltro
    {
        public int? IdClassificacaoContabil { get; set; }
        public string? Nome { get; set; }
        public int? IdEmpresa { get; set; }
        public int? IdProjeto { get; set; }
        public string? DataInicial { get; set; }
        public string? DataFinal { get; set; }
    }
}
