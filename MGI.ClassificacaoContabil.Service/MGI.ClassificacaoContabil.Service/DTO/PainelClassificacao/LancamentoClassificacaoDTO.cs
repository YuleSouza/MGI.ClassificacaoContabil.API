namespace Service.DTO.PainelClassificacao
{
    public class LancamentoClassificacaoDTO
    {
        public DateTime DtLancamentoProjeto { get; set; }
        public DateTime DtLancamentoSap { get; set; }
        public string GrupoDePrograma { get; set; }
        public int IdClassifContabil { get; set; }
        public int IdClassificacaoEsg { get; set; }
        public int IdEmpresa { get; set; }
        public string IdGestor { get; set; }
        public int IdGrupoPrograma { get; set; }
        public int IdPrograma { get; set; }
        public int IdProjeto { get; set; }
        public int IdTipoClassificacao { get; set; }
        public string NomeClassifContabil { get; set; }
        public string NomeClassificacaoEsg { get; set; }
        public string NomeEmpresa { get; set; }
        public string NomeFase { get; set; }
        public string NomeGestor { get; set; }
        public string NomeProjeto { get; set; }
        public string Nomenclatura { get; set; }
        public string Pep { get; set; }
        public string Programa { get; set; }
        public int SeqFase { get; set; }
        public string TipoLancamento { get; set; }
        public decimal ValorCiclo { get; set; }
        public decimal ValorOrcado { get; set; }
        public decimal ValorPrevisto { get; set; }
        public decimal ValorRealizado { get; set; }
        public decimal ValorRealizadoSap { get; set; }
        public decimal ValorReplan { get; set; }
        public decimal ValorTendencia { get; set; }
    }
}
