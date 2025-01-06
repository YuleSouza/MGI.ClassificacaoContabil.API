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
        public decimal RealizadoAnoAnterior { get; set; }
        public decimal RealizadoMesAnterior { get; set; }
        public decimal OrcadoPartirAnoAtual { get; set; }
        public decimal PrevistoPartirAnoAtual { get; set; }
        public decimal ReplanPartirAnoAtual { get; set; }
        public decimal TedenciaMesAtualAteAnoVigente { get; set; }
        public decimal CicloPartirAnoSeguinte { get; set; }
        public decimal TendenciaPartirMesAtual { get; set; }
    }
}
