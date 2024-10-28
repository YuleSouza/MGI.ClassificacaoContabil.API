namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class FaseEsgDTO
    {
        public int IdEmpresa { get; set; }
        public int IdGrupoPrograma { get; set; }
        public DateTime DtLancamentoProjeto { get; set; }
        public string NomeFase { get; set; }
        public int SeqFase { get; set; }
        public string Pep { get; set; }
        public int IdClassificacaoEsg { get; set; }
        public LancamentoESG LancamentosESG { get; set; }
    }
}
