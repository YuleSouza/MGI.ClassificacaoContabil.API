﻿namespace Service.DTO.PainelClassificacao
{
    public class LancamentoFaseContabilDTO
    {
        public int IdEmpresa { get; set; }
        public int IdGrupoPrograma { get; set; }
        public int IdPrograma { get; set; }
        public int IdProjeto { get; set; }
        public decimal ValorOrcado { get; set; }
        public decimal ValorTendencia { get; set; }
        public decimal ValorRealizado { get; set; }
        public decimal ValorReplan { get; set; }
        public decimal ValorCiclo { get; set; }
        public DateTime DtLancamentoProjeto { get; set; }
        public string IdGestor { get; set; }
        public string NomeFase { get; set; }
        public int FseSeq { get; set; }
        public string Pep {  get; set; }
        public int SeqFase { get; set; }
        public LancamentoContabilDTO Lancamentos { get; set; }
    }
}
