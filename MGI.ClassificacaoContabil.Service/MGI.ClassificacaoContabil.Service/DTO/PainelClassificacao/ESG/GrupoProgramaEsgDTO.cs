using Service.DTO.Empresa;
using System.Text.Json.Serialization;

namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class GrupoProgramaEsgDTO
    {
        [JsonPropertyName("idGrupoPrograma")]
        public int IdGrupoPrograma { get; set; }
        public string? Nome { get; set; }
        public LancamentoESG LancamentoESG { get; set; }
        public LancamentoTotalESG Total { get; set; }
        public IEnumerable<ProgramaEsgDTO>? Programas { get; set; }
    }
}
