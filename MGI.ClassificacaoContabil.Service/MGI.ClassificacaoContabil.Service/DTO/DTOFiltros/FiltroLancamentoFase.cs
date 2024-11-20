﻿namespace Service.DTO.Filtros
{
    public class FiltroLancamentoFase
    {
        public int? IdGrupoPrograma { get; set; }
        public int? IdPrograma { get; set; }
        public int? IdProjeto { get; set; }
        public string? IdGestor { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
