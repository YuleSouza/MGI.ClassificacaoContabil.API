namespace Service.DTO.Esg
{
    public class ProjetoEsgDTO
    {
        public int IdProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public int IdEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public string IdGestor {  get; set; }
        public decimal ValorProjeto { get; set; }        
        public string IdStatusProjeto { get; set; }
        public string DescricaoStatusProjeto { get; set; }
        public string NomePatrocinador { get; set; }                
        public DateTime DtLancamentoProjeto { get; set; }
        public string TipoValor {  get; set; }
        public decimal ValorOrcamento { get; set; }
        public string StatusAprovacao {  get; set; }
        public int QtdAprovados { get; set; }
        public int QtdReprovados { get; set; }
        public int QtdExcluidos { get; set; }
        public int QtdPendente { get; set; }
    }
}
