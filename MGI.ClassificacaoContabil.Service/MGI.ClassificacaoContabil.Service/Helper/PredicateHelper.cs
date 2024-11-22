using Service.DTO.PainelClassificacao;
using Service.Enum;

namespace Service.Helper
{
    public class PredicateHelper
    {

        #region Contabil
        #region [ Base Orcamento ] 
        private Func<LancamentoClassificacaoEsgDTO, bool> predicateBaseOrcamentoRealizado = _ => true;
        private Func<LancamentoClassificacaoEsgDTO, bool>  predicateBaseOrcamentoPrevisto = _ => true;
        private Func<LancamentoClassificacaoEsgDTO, bool> predicateBaseOrcamentoReplan = _ => true;
        private Func<LancamentoClassificacaoEsgDTO, bool> predicateBaseOrcamentoOrcado = _ => true;
 
        private Func<LancamentoFaseContabilDTO, bool> predicateFasePrevisto = _ => true;
        private Func<LancamentoFaseContabilDTO, bool> predicateFasePrevisto_Realizado = _ => true;
        private Func<LancamentoFaseContabilDTO, bool> predicateFaseReplan = _ => true;
        private Func<LancamentoFaseContabilDTO, bool> predicateFaseOrcado = _ => true;
        #endregion

        #region [ Formato Acompanhamento ] 
        private Func<LancamentoClassificacaoEsgDTO, bool>  predicateFormatoAcomp_realizado = _ => true;
        private Func<LancamentoClassificacaoEsgDTO, bool> predicateFormatoAcomp_tendencia = _ => true;
        private Func<LancamentoClassificacaoEsgDTO, bool> predicateFormatoAcomp_ciclo = _ => true;
        
        private Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcompFase_realizado = _ => true;
        private Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcompFase_tendencia = _ => true;
        private Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcompFase_ciclo = _ => true;

        private string _formatoAcompanhamento = string.Empty;
        private DateTime _dataFim;
        private DateTime _dataInicio;
        private DateTime mesAnterior;
        private DateTime finalAno;
        private DateTime anoPosterior_inicio;
        private DateTime anoPosterior_fim;
        private DateTime mesAtual;
        #endregion
        #endregion

        #region [ Contabil ]
        Func<ClassificacaoContabilItemDTO, bool> c_predicateBaseOrcamentoRealizado = _ => true;
        Func<ClassificacaoContabilItemDTO, bool> c_predicateBaseOrcamentoPrevisto = _ => true;
        Func<ClassificacaoContabilItemDTO, bool> c_predicateBaseOrcamentoReplan = _ => true;
        Func<ClassificacaoContabilItemDTO, bool> c_predicateBaseOrcamentoOrcado = _ => true;

        Func<LancamentoFaseContabilDTO, bool> c_predicateFasePrevisto = _ => true;
        Func<LancamentoFaseContabilDTO, bool> c_predicateFasePrevisto_Realizado = _ => true;
        Func<LancamentoFaseContabilDTO, bool> c_predicateFaseReplan = _ => true;
        Func<LancamentoFaseContabilDTO, bool> c_predicateFaseOrcado = _ => true;


        Func<ClassificacaoContabilItemDTO, bool> c_predicateFormatoAcomp_realizado = _ => true;
        Func<ClassificacaoContabilItemDTO, bool> c_predicateFormatoAcomp_tendencia = _ => true;
        Func<ClassificacaoContabilItemDTO, bool> c_predicateFormatoAcomp_ciclo = _ => true;

        Func<LancamentoFaseContabilDTO, bool> c_predicateFormatoAcompFase_realizado = _ => true;
        Func<LancamentoFaseContabilDTO, bool> c_predicateFormatoAcompFase_tendencia = _ => true;
        Func<LancamentoFaseContabilDTO, bool> c_predicateFormatoAcompFase_ciclo = _ => true;
        #endregion
        public PredicateHelper(string formatoAcompanhamento, DateTime dataInicio, DateTime dataFim, string tipoPainel)
        {
            _formatoAcompanhamento = formatoAcompanhamento;
            _dataFim = dataFim;
            _dataInicio = dataInicio;
            SetDatas();
            if (tipoPainel == "E")
            {
                ConfigurarPredicatesEsg();
            }
            else
            {
                ConfigurarPredicatesContabil();
            }
        }

        private void SetDatas()
        {
            mesAnterior = DateTime.Now.AddMonths(-1);
            finalAno = new DateTime(DateTime.Now.Year, 12, 31);
            anoPosterior_inicio = new DateTime(DateTime.Now.Year + 1, 1, 1);
            anoPosterior_fim = new DateTime(DateTime.Now.Year + 1, 12, 31);
            mesAtual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }
        private void ConfigurarPredicatesEsg()
        {
            predicateBaseOrcamentoRealizado = p => p.DtLancamentoProjeto.Year < DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Realizado;
            predicateBaseOrcamentoPrevisto = p => p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Previsto;
            predicateBaseOrcamentoReplan = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Replan);
            predicateBaseOrcamentoOrcado = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Orcado);

            predicateFasePrevisto = p => p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Previsto;
            predicateFasePrevisto_Realizado = p => (p.DtLancamentoProjeto.Year < DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Realizado);
            predicateFaseReplan = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Replan);
            predicateFaseOrcado = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Orcado);

            if (_formatoAcompanhamento == "C")
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto <= finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.TipoLancamento == ETipoOrcamento.Ciclo;

                predicateFormatoAcompFase_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto <= finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.TipoLancamento == ETipoOrcamento.Ciclo;

                if (_dataFim > mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;

                    predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataInicio > mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;

                    predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;
                }
                else if (_dataFim < mesAnterior)
                {
                    predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                    predicateFormatoAcompFase_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                }
            }
            else if (_formatoAcompanhamento == "T")
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto < mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto >= mesAtual && p.TipoLancamento == ETipoOrcamento.Tendencia;

                if (_dataFim > mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataFim < mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataInicio > mesAnterior)
                {
                    predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                }
            }
        }
        private void ConfigurarPredicatesContabil()
        {
            if (_formatoAcompanhamento == "C")
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto <= finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.TipoLancamento == ETipoOrcamento.Ciclo;

                predicateFormatoAcompFase_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto <= finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.TipoLancamento == ETipoOrcamento.Ciclo;

                if (_dataFim > mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;

                    predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataInicio > mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;

                    predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;
                }
                else if (_dataFim < mesAnterior)
                {
                    predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                    predicateFormatoAcompFase_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                }
            }
            else if (_formatoAcompanhamento == "T")
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto < mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto >= mesAtual && p.TipoLancamento == ETipoOrcamento.Tendencia;

                if (_dataFim > mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataFim < mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataInicio > mesAnterior)
                {
                    predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                }
            }

            c_predicateBaseOrcamentoRealizado = p => p.DtLancamentoProjeto.Year < DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Realizado;
            c_predicateBaseOrcamentoPrevisto = p => p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Previsto;
            c_predicateBaseOrcamentoReplan = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Replan);
            c_predicateBaseOrcamentoOrcado = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Orcado);

            c_predicateFasePrevisto = p => p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Previsto;
            c_predicateFasePrevisto_Realizado = p => (p.DtLancamentoProjeto.Year < DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Realizado);
            c_predicateFaseReplan = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Replan);
            c_predicateFaseOrcado = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Orcado);
        }

        #region  [ Esg ]
        public Func<LancamentoClassificacaoEsgDTO, bool> PredicateBaseOrcamentoRealizado
        {
            get
            {
                return predicateBaseOrcamentoRealizado;
            }
        }
        public Func<LancamentoClassificacaoEsgDTO, bool> PredicateBaseOrcamentoPrevisto
        {
            get
            {
                return predicateBaseOrcamentoPrevisto;
            }
        }
        public Func<LancamentoClassificacaoEsgDTO, bool> PredicateBaseOrcamentoReplan
        {
            get { return predicateBaseOrcamentoReplan; } 
        }
        public Func<LancamentoClassificacaoEsgDTO, bool> PredicateBaseOrcamentoOrcado
        {
            get { return predicateBaseOrcamentoOrcado; } 
        }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFasePrevisto { get { return predicateFasePrevisto; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFasePrevisto_Realizado { get { return predicateFasePrevisto_Realizado; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFaseReplan { get { return predicateFaseReplan; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFaseOrcado { get { return predicateFaseOrcado; } }
        public Func<LancamentoClassificacaoEsgDTO, bool> PredicateFormatoAcomp_realizado { get { return predicateFormatoAcomp_realizado; } }
        public Func<LancamentoClassificacaoEsgDTO, bool> PredicateFormatoAcomp_tendencia { get { return predicateFormatoAcomp_tendencia; } }
        public Func<LancamentoClassificacaoEsgDTO, bool> PredicateFormatoAcomp_ciclo { get { return predicateFormatoAcomp_ciclo; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFormatoAcompFase_realizado { get { return predicateFormatoAcompFase_realizado; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFormatoAcompFase_tendencia { get { return predicateFormatoAcompFase_tendencia; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFormatoAcompFase_ciclo { get { return predicateFormatoAcompFase_ciclo; } }
        #endregion

        #region [ Contabil ]
        public Func<ClassificacaoContabilItemDTO, bool> C_PredicateBaseOrcamentoRealizado { get { return c_predicateBaseOrcamentoRealizado; } }
        public Func<ClassificacaoContabilItemDTO, bool> C_PredicateBaseOrcamentoPrevisto { get { return c_predicateBaseOrcamentoPrevisto; } }
        public Func<ClassificacaoContabilItemDTO, bool> C_PredicateBaseOrcamentoReplan { get { return c_predicateBaseOrcamentoReplan; } }
        public Func<ClassificacaoContabilItemDTO, bool> C_PredicateBaseOrcamentoOrcado { get { return c_predicateBaseOrcamentoOrcado; } }
        public Func<LancamentoFaseContabilDTO, bool> C_PredicateFasePrevisto { get { return c_predicateFasePrevisto; } }
        public Func<LancamentoFaseContabilDTO, bool> C_PredicateFasePrevisto_Realizado { get { return c_predicateFasePrevisto_Realizado; } }
        public Func<LancamentoFaseContabilDTO, bool> C_PredicateFaseReplan { get { return c_predicateFaseReplan; } }
        public Func<LancamentoFaseContabilDTO, bool> C_PredicateFaseOrcado { get { return c_predicateFaseOrcado; } }
        public Func<ClassificacaoContabilItemDTO, bool> C_PredicateFormatoAcomp_realizado { get { return c_predicateFormatoAcomp_realizado; } }
        public Func<ClassificacaoContabilItemDTO, bool> C_PredicateFormatoAcomp_tendencia { get { return c_predicateFormatoAcomp_tendencia; } }
        public Func<ClassificacaoContabilItemDTO, bool> C_PredicateFormatoAcomp_ciclo { get { return c_predicateFormatoAcomp_ciclo; } }
        public Func<LancamentoFaseContabilDTO, bool> C_PredicateFormatoAcompFase_realizado { get { return c_predicateFormatoAcompFase_realizado; } }
        public Func<LancamentoFaseContabilDTO, bool> C_PredicateFormatoAcompFase_tendencia { get { return c_predicateFormatoAcompFase_tendencia; } }
        public Func<LancamentoFaseContabilDTO, bool> C_PredicateFormatoAcompFase_ciclo { get { return c_predicateFormatoAcompFase_ciclo; } }
        #endregion
    }
}
