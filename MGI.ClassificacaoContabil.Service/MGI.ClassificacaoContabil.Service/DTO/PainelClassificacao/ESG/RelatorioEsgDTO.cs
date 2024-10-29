namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class RelatorioEsgDTO
    {
        public int IdEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int IdProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public string NomeFase { get; set; }
        public string DiretoriaSolicitante { get; set; }
        public string GerenciaSolicitante { get; set; }
        public string DiretoriaExecutora { get; set; }
        public string GerenciaExecutora { get; set; }
        public string Gestor { get; set; }
        public string MesAnoProjeto { get; set; }
        public decimal ValorOrcado { get; set; }
        public decimal ValorRealizado { get; set; }
        public decimal ValorTendencia { get; set; }
        public decimal ValorReplan { get; set; }
        public decimal ValorCiclo { get; set; }
        public int IdClassificacaoContabil { get; set; }
        public int IdGrupoPrograma { get; set; }
        public int IdPrograma { get; set; }
        public int IdClassificacaoEsg { get; set; }
        public string NomeClassificacaoEsg { get; set; }
        public DateTime DtLancamentoProjeto { get; set; }
        public decimal ValoBaseOrcamento { get; set; }
        public decimal ValorFormatoAcompanhamento { get; set; }
    }
}
