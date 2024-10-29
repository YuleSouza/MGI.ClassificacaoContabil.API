﻿namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil
{
    public class RelatorioContabilDTO
    {
        public string CodExterno { get; set; }
        public string Data {  get; set; }
        public decimal ValorInvestimento { get; set; }
        public string QtdProdutcaoTotal { get; set; }
        public string SaldoInicialAndamento { get; set; }
        public string TxImobilizado { get; set; }
        public string TxTransfDespesa { get; set; }
        public string TxProducao { get; set; }
        public string TxDepreciacao { get; set; }
        public decimal ValorProjeto { get; set; }
        public int IdEmpresa { get; set; }
        public int IdGrupoPrograma { get; set; }
        public int IdPrograma   { get; set; }
        public int IdProjeto { get; set; }
        public string IdGestor { get; set; }
        public DateTime DtLancamentoProjeto { get; set; }
        public string TipoValorProjeto { get; set; }
    }
}
