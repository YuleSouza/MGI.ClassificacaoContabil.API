namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class LancamentoClassificacaoEsgDTO
    {
        public int IdEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int IdGrupoPrograma { get; set; }
        public string GrupoDePrograma { get; set; }
        public int IdPrograma { get; set; }
        public string Programa { get; set; }
        public int IdProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public decimal ValorOrcado { get; set; }
        public decimal ValorRealizado { get; set; }
        public decimal ValorReplan {  get; set; }
        public decimal ValorTendencia { get; set; }
        public int IdTipoClassificacao { get; set; }
        public decimal ValorRealizadoSap { get; set; }
        public int IdClassificacaoEsg { get; set; }
        public string IdGestor { get; set; }
        public string NomeGestor { get; set; }
        public DateTime DtLancamentoSap { get; set; }
        public string Pep { get; set; }
        public string Nomenclatura { get; set; }
        public string NomeFase { get; set; }
    }
}
