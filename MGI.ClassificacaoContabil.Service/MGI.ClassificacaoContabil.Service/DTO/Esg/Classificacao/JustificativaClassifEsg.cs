using Microsoft.AspNetCore.Http;

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
        public string StatusAprovacao { get; set; }
        public decimal PercentualKpi { get; set; }
        public String UsuarioCripto {  get; set; }
        public List<AnexoJustificaitvaClassifEsgDTO>? Anexos { get; set; }
        public List<IFormFile> ArquivosAnexos { get; set; }
    }
}
