using CsvHelper.Configuration.Attributes;

namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class RelatorioEsgExcelDTO
    {       
        [Name("Empresa")]
        public string NomeEmpresa { get; set; }

        [Name("BGID")]
        public int IdProjeto { get; set; }

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

        [Name("Mês")]
        public string MesProjeto { get; set; }

        [Name("Ano")]
        public string AnoProjeto { get; set; }

        [Name("R$ Orçado")]
        public decimal ValorOrcado { get; set; }

        [Name("Realizado/Tendendia")]
        public decimal ValorRealizado { get; set; }

        [Name("Cenário")]
        public string Cenario {  get; set; }

        [Name("Classificação ESG")]
        public string ClassifEsg {  get; set; }
        
    }
}
