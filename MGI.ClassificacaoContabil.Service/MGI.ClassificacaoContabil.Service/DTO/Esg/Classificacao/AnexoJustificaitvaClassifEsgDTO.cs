using System.Text.Json.Serialization;

namespace Service.DTO.Esg
{
    public class AnexoJustificaitvaClassifEsgDTO
    {
        [JsonPropertyName("idAnexo")]
        public int IdAnexo {  get; set; }

        [JsonPropertyName("idJustifClassifEsg")]
        public int IdJustifClassifEsg { get; set; }

        [JsonPropertyName("nomeAnexo")]
        public string NomeAnexo { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("idProjeto")]
        public int IdProjeto { get; set; }
    }
}
