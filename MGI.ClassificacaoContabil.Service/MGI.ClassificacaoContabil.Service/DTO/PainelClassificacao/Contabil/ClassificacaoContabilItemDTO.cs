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
        public string NomeProjeto { get; set; }
        public decimal ValorProjeto { get; set; }
        public decimal TotalOrcado { get; set; }
        public decimal TotalOrcadoGrupo { get; set; }
        public decimal TotalOrcadoPrograma { get; set; }
        public string MesOrcamento { get; set; }
        public int IdTipoClassificacao { get; set; }
        public decimal ValorRealizadoSap {  get; set; }
        public string DescricaoLancSap { get; set; }
    }
}
