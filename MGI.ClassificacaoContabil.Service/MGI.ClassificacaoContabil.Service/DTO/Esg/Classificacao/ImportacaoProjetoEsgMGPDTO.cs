using Service.Enum;

namespace Service.DTO.Esg
{
    public class ImportacaoProjetoEsgMGPDTO
    {
        public int IdEmpresa { get; set; }
        public int IdProjeto { get; set; }
        public int IdJustifClassifEsg { get; set; }
        public int IdClassif { get; set; }        
        public DateTime DataClassif { get; set; }
        public int IdSubClassif { get; set; }        
        public string Justificativa { get; set; } = string.Empty;
        public string StatusAprovacao { get; set; } = EStatusAprovacao.Pendente;
        public string Usuario { get; set; }
        public decimal PercentualKpi { get; set; }
        public int SeqMeta { get; set; }
    }
}
