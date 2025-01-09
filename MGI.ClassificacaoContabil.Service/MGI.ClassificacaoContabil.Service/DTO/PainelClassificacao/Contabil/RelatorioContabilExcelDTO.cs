using CsvHelper.Configuration.Attributes;

namespace Service.DTO.PainelClassificacao
{
    public class RelatorioContabilExcelDTO
    {
        [Name("Cod Externo")]
        public string CodExterno { get; set; }
        public string Data { get; set; }

        [Name("Valor Investimento")]
        public decimal ValorInvestimento { get; set; }

        [Name("Qtd. Producao Total")]
        public string QtdProdutcaoTotal { get; set; }

        [Name("Saldo Inicial Andamento")]
        public string SaldoInicialAndamento { get; set; }

        [Name("Tx. Imobilizado")]
        public string TxImobilizado { get; set; }

        [Name("Tx. Transf. Despesas")]
        public string TxTransfDespesa { get; set; }

        [Name("Tx. Producao")]
        public string TxProducao { get; set; }

        [Name("Tx. Depreciacao")]
        public string TxDepreciacao { get; set; }
    }
}
