namespace Service.DTO.Esg
{
    public class FiltroJustificativaClassifEsg
    {
        public int IdProjeto { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime DataClassif { get; set; }
        public bool ExibirClassificaoExcluida { get; set; } = false;        
        public int IdClassif { get; set; }
        public int IdSubClassif { get; set; }
        public decimal ValorProjeto { get; set; }
    }
}
