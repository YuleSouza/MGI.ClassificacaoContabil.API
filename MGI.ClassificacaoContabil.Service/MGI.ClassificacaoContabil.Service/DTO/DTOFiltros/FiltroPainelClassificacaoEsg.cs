﻿namespace Service.DTO.Filtros
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
        public int? IdGestor { get; set; }
    }
}
