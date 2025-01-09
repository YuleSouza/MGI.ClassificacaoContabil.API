using CsvHelper.Configuration.Attributes;

namespace Service.DTO.PainelClassificacao
{
    public class RelatorioEsgExcelDTO
    {       
        [Name("Empresa")]
        public string NomeEmpresa { get; set; }

        [Name("BGID")]
        public int IdProjeto { get; set; }

        [Name("Projeto")]
        public string NomeProjeto {  get; set; }

        [Name("Fase")]
        public string NomeFase { get; set; }

        [Name("Dir. Solic")]
        public string DiretoriaSolicitante { get; set; }

        [Name("Ger. Solic")]
        public string GerenciaSolicitante { get; set; }

        [Name("Dir. Exec.")]
        public string DiretoriaExecutora { get; set; }

        [Name("Ger. Exec.")]
        public string GerenciaExecutora { get; set; }

        [Name("Gestor")]
        public string Gestor { get; set; }

        [Name("Mes")]
        public string MesProjeto { get; set; }

        [Name("Ano")]
        public string AnoProjeto { get; set; }

        [Name("R$ Orcado")]
        public decimal ValoBaseOrcamento { get; set; }

        [Name("Realizado/Tendendia")]
        public decimal ValorFormatoAcompanhamento { get; set; }

        [Name("Cenario")]
        public string Cenario {  get; set; }

        [Name("Classificacao ESG")]
        public string ClassifEsg {  get; set; }
    }
}
