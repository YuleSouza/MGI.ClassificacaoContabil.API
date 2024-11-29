using Service.Enum;
using System.ComponentModel.DataAnnotations;

namespace MGI.ClassificacaoContabil.API.Model
{
    public class PainelClassificacaoContabilModel
    {
        public int IdEmpresa { get; set; }

        /// <summary>
        /// 0 - Acumulado
        /// 1 - Anual
        /// </summary>
        public int TipoAcumuladoOuAnual { get; set; }
        public int? IdClassificacaoContabil { get; set; }
        public int? IdGrupoPrograma { get; set; }
        public int? IdPrograma { get; set; }
        public string? IdGestor { get; set; }
        public int? IdProjeto { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }        
        /// <summary>
        /// C - Ciclo
        /// T - Tendencia
        /// </summary>
        public char FormatAcompanhamento { get; set; }
        /// <summary>
        /// 0 - Orcado
        /// 1 - Replan
        /// </summary>
        [BaseOrcamentoAttribute]
        public string BaseOrcamento { get; set; }
        public int ClassificacaoContabil { get; set; }
        /// <summary>
        /// 0 - ORcado
        /// R - Replan
        /// P - Previsto
        /// 2 - Ciclo
        /// J - Tendencia
        /// </summary>
        public char? ValorInvestimento { get; set; }
    }
}
