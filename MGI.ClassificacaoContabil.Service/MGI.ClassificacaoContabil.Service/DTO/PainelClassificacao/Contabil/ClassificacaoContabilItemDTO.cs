using System;


namespace Service.DTO.PainelClassificacao
{
    public class ClassificacaoContabilItemDTO
    {
        public int IdEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int IdGrupoPrograma { get; set; }
        public string GrupoDePrograma { get; set; }
        public int IdPrograma { get; set; }
        public string Programa { get; set; }
        public int IdProjeto { get; set; }
        public int FseSeq {  get; set; }
        public string NomeProjeto { get; set; }
        public decimal ValorOrcado { get; set; }
        public decimal ValorTendencia { get; set; }
        public decimal ValorRealizado { get; set; }
        public decimal ValorCiclo { get; set; }
        public decimal ValorReplan { get; set; }
        public string TipoLancamento { get; set; }
        public DateTime DtLancamentoProjeto { get; set; }
        public string IdGestor { get; set; }
        public string NomeClassifContabil { get; set; }
        public int IdClassifContabil { get; set; }
        public string Pep {  get; set; }
    }
}
