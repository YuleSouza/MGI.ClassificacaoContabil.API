namespace Service.DTO.Esg.Email
{
    public class GestorEmailDTO
    {
        public string EmailDestinatario { get; set; }
        public int IdProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public string NomePatrocinador { get; set; }
        public string Usuario { get; set; }
        public int PercentualKPI { get; set; }
        public string NomeClassificacao { get; set; }
        public string NomeSubClassificacao { get; set; }
        public string Aprovacao { get; set; }
        public string NomeGestor { get; set; }
    }
}
