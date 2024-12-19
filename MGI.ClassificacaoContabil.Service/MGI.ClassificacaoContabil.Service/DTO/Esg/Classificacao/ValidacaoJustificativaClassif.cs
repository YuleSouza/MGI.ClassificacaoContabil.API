namespace MGI.ClassificacaoContabil.Service.DTO.Esg.Classificacao
{
    public class ValidacaoJustificativaClassif
    {
        public int IdEmpresa { get; set; }
        public int IdProjeto { get; set; }
        public DateTime DataClassif { get; set; }
        public decimal Percentual { get; set; }
        public int IdClassif { get; set; }
        public int IdSubClassif { get; set; }        
    }
}
