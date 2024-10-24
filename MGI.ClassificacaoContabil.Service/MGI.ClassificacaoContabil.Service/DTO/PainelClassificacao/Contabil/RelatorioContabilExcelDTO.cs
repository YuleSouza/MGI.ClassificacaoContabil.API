using CsvHelper.Configuration.Attributes;

namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil
{
    public class RelatorioContabilExcelDTO
    {
        [Name("Cód Externo")]
        public string CodExterno { get; set; }
        public string Data { get; set; }

        [Name("Valor Investimento")]
        public decimal ValorInvestimento { get; set; }

        [Name("Qtd. Produção Total")]
        public string QtdProdutcaoTotal { get; set; }

        [Name("Saldo Inicial Andamento")]
        public string SaldoInicialAndamento { get; set; }

        [Name("Tx. Imobilizado")]
        public string TxImobilizado { get; set; }

        [Name("Tx. Transf. Despesas")]
        public string TxTransfDespesa { get; set; }

        [Name("Tx. Produção")]
        public string TxProducao { get; set; }

        [Name("Tx. Depreciação")]
        public string TxDepreciacao { get; set; }
    }
}
