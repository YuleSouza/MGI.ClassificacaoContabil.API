namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil
{
    public class FaseContabilDTO
    {
        public int IdEmpresa { get; set; }
        public int IdGrupoPrograma { get; set; }
        public DateTime DtLancamentoProjeto { get; set; }
        public string Nome { get; set; }
        public int FseSeq { get; set; }
        public string Pep { get; set; }
        public int IdClassifContabil { get; set; }
        public LancamentoContabilDTO Lancamentos {  get; set; }
        public LancamentoContabilTotalDTO TotalLancamento { get; set; }
        public IEnumerable<LancamentoSAP> LancamentoSAP { get; set; }
    }
}
