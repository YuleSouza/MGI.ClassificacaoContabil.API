namespace Service.DTO.Esg
{
    public class JustificativaClassifEsg
    {        
        public int IdEmpresa { get; set; }
        public DateTime DataClassif { get; set; }
        public int IdProjeto { get; set; }
        public int IdClassif { get; set; }
        public int IdSubClassif { get; set; }
        public string Justificativa { get; set; }        
        public string UsCriacao { get; set; }
        public char StatusAprovacao { get; set; }
        public decimal PercentualKpi { get; set; }
    }
}
