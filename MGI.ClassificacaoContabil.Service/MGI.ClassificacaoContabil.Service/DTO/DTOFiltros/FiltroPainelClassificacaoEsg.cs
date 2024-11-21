namespace Service.DTO.Filtros
{
    public class FiltroPainelClassificacaoEsg
    {
        public int IdEmpresa { get; set; }

        /// <summary>
        /// 0 - Acumulado
        /// 1 - Anual
        /// </summary>
        public int TipoAcumuladoOuAnual { get; set; }
        public int IdCenario {  get; set; }
        public int? IdClassificacaoEsg { get; set; }
        public int? IdGrupoPrograma { get; set; }
        public int? IdPrograma { get; set; }
        public string? IdGestor { get; set; }
        public int? IdProjeto { get; set; }
        public int? SeqFase { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime? DataRealizadoInicio { get; set; }
        public DateTime? DataRealizadoFim { get; set; }
        public DateTime? DataTendenciaInicio { get; set; }
        public DateTime? DataTendenciaFim { get; set; }
        public DateTime? DataCicloInicio { get; set; }
        public DateTime? DataCicloFim { get; set; }
        public DateTime? DataOrcadoInicio { get; set; }
        public DateTime? DataOrcadoFim { get; set; }
        public DateTime? DataReplanInicio { get; set; }
        public DateTime? DataReplanFim { get; set; }
        public string FormatAcompanhamento { get; set; }
        public string BaseOrcamento { get; set; }
    }
}
