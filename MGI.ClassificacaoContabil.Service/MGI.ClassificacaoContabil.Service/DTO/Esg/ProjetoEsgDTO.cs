namespace Service.DTO.Esg
{
    public class ProjetoEsgDTO
    {
        public int IdProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public string NomeEmpresa { get; set; }
        public string IdGestor {  get; set; }
        public decimal TotalOrcado { get; set; }
        public decimal TotalReplan {  get; set; }
        public decimal TotalTendencia { get; set; }
        public decimal ValorProjeto { get; set; }


    }
}
