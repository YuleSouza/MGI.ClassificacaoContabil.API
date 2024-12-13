namespace Service.DTO.Filtros
{
    public class FiltroProjetoEsg
    {
        public int IdEmpresa { get; set; }
        public int IdProjeto { get; set; }
        public string? IdGestor { get; set; }
        public string? IdGrupoPrograma { get; set; }
        public string? StatusProjeto { get; set; }
        public string? IdDiretoria { get; set; }
        public string? IdGerencia { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime DataInicio { get; set; }        
        public string StatusAprovacao { get; set; }
        public string ClassificacaoInvestimento { get; set; }
        public string BaseOrcamento { get; set; }
        public string FormatoAcompanhamento { get; set; }
    }
}
