using Service.DTO.PainelClassificacao;
using Service.Enum;

namespace Service.Helper
{
    public class PredicateHelper
    {
        #region [ Base Orcamento ] 

        private Func<LancamentoClassificacaoDTO, bool> predicateBaseOrcamentoRealizado = _ => true;
        private Func<LancamentoClassificacaoDTO, bool>  predicateBaseOrcamentoPrevisto = _ => true;
        private Func<LancamentoClassificacaoDTO, bool> predicateBaseOrcamentoReplan = _ => true;
        private Func<LancamentoClassificacaoDTO, bool> predicateBaseOrcamentoOrcado = _ => true;
 
        private Func<LancamentoFaseContabilDTO, bool> predicateFasePrevisto = _ => true;
        private Func<LancamentoFaseContabilDTO, bool> predicateFasePrevisto_Realizado = _ => true;
        private Func<LancamentoFaseContabilDTO, bool> predicateFaseReplan = _ => true;
        private Func<LancamentoFaseContabilDTO, bool> predicateFaseOrcado = _ => true;
        #endregion

        #region [ Formato Acompanhamento ] 
        private Func<LancamentoClassificacaoDTO, bool>  predicateFormatoAcomp_realizado = _ => true;
        private Func<LancamentoClassificacaoDTO, bool> predicateFormatoAcomp_tendencia = _ => true;
        private Func<LancamentoClassificacaoDTO, bool> predicateFormatoAcomp_ciclo = _ => true;
        
        private Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcompFase_realizado = _ => true;
        private Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcompFase_tendencia = _ => true;
        private Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcompFase_ciclo = _ => true;        
        #endregion

        private string _formatoAcompanhamento = string.Empty;
        private DateTime _dataFim;
        private DateTime _dataInicio;
        private DateTime _mesAnterior;
        private DateTime _finalAno;
        private DateTime _anoPosteriorInicio;
        private DateTime _anoPosteriorFim;
        private DateTime _mesAtual;
        private int _anoAtual = DateTime.Now.Year;
        private readonly string _formatoAcompanhamentoCiclo = "C";
        private readonly string _formatoAcompanhamentoTendencia = "T";
        
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
            _mesAnterior = DateTime.Now.AddMonths(-1);
            _finalAno = new DateTime(_anoAtual, 12, 31);
            _anoPosteriorInicio = new DateTime(_anoAtual + 1, 1, 1);
            _anoPosteriorFim = new DateTime(_anoAtual + 1, 12, 31);
            _mesAtual = new DateTime(_anoAtual, DateTime.Now.Month, 1);
        }
        private void ConfigurarPredicatesEsg()
        {
            predicateBaseOrcamentoRealizado = p => p.DtLancamentoProjeto.Year < _anoAtual && p.TipoLancamento == ETipoOrcamento.Realizado;
            predicateBaseOrcamentoPrevisto = p => p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Previsto;
            predicateBaseOrcamentoReplan = p => (p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Replan);
            predicateBaseOrcamentoOrcado = p => (p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Orcado);

            predicateFasePrevisto = p => p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Previsto;
            predicateFasePrevisto_Realizado = p => p.DtLancamentoProjeto.Year < _anoAtual && p.TipoLancamento == ETipoOrcamento.Realizado;
            predicateFaseReplan = p => p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Replan;
            predicateFaseOrcado = p => p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Orcado;

            if (_formatoAcompanhamento == _formatoAcompanhamentoCiclo)
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto <= _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= _anoPosteriorInicio && p.TipoLancamento == ETipoOrcamento.Ciclo;

                predicateFormatoAcompFase_realizado = p => p.DtLancamentoProjeto <= _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto <= _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto >= _anoPosteriorInicio && p.TipoLancamento == ETipoOrcamento.Ciclo;

                if (_dataFim > _mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= _anoPosteriorInicio && p.DtLancamentoProjeto <= _anoPosteriorFim && p.TipoLancamento == ETipoOrcamento.Ciclo;

                    predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataInicio > _mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= _anoPosteriorInicio && p.DtLancamentoProjeto <= _anoPosteriorFim && p.TipoLancamento == ETipoOrcamento.Ciclo;

                    predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto >= _anoPosteriorInicio && p.DtLancamentoProjeto <= _anoPosteriorFim && p.TipoLancamento == ETipoOrcamento.Ciclo;
                }
                else if (_dataFim < _mesAnterior)
                {
                    predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                    predicateFormatoAcompFase_realizado = p => p.DtLancamentoProjeto <= _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                }
            }
            else if (_formatoAcompanhamento == _formatoAcompanhamentoTendencia)
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto < _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto >= _mesAtual && p.TipoLancamento == ETipoOrcamento.Tendencia;

                if (_dataFim > _mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataFim < _mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataInicio > _mesAnterior)
                {
                    predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                }
            }
        }
        private void ConfigurarPredicatesContabil()
        {
            if (_formatoAcompanhamento == "C")
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto <= _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= _anoPosteriorInicio && p.TipoLancamento == ETipoOrcamento.Ciclo;

                predicateFormatoAcompFase_realizado = p => p.DtLancamentoProjeto <= _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto <= _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto >= _anoPosteriorInicio && p.TipoLancamento == ETipoOrcamento.Ciclo;

                if (_dataFim > _mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= _anoPosteriorInicio && p.DtLancamentoProjeto <= _anoPosteriorFim && p.TipoLancamento == ETipoOrcamento.Ciclo;

                    predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataInicio > _mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= _anoPosteriorInicio && p.DtLancamentoProjeto <= _anoPosteriorFim && p.TipoLancamento == ETipoOrcamento.Ciclo;

                    predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto >= _anoPosteriorInicio && p.DtLancamentoProjeto <= _anoPosteriorFim && p.TipoLancamento == ETipoOrcamento.Ciclo;
                }
                else if (_dataFim < _mesAnterior)
                {
                    predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                    predicateFormatoAcompFase_realizado = p => p.DtLancamentoProjeto <= _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                }
            }
            else if (_formatoAcompanhamento == "T")
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto < _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto >= _mesAtual && p.TipoLancamento == ETipoOrcamento.Tendencia;

                if (_dataFim > _mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataFim < _mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < _finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (_dataInicio > _mesAnterior)
                {
                    predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= _mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                }
            }

            predicateBaseOrcamentoRealizado = p => p.DtLancamentoProjeto.Year < _anoAtual && p.TipoLancamento == ETipoOrcamento.Realizado;
            predicateBaseOrcamentoPrevisto = p => p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Previsto;
            predicateBaseOrcamentoReplan = p => (p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Replan);
            predicateBaseOrcamentoOrcado = p => (p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Orcado);

            predicateFasePrevisto = p => p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Previsto;
            predicateFasePrevisto_Realizado = p => (p.DtLancamentoProjeto.Year < _anoAtual && p.TipoLancamento == ETipoOrcamento.Realizado);
            predicateFaseReplan = p => (p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Replan);
            predicateFaseOrcado = p => (p.DtLancamentoProjeto.Year >= _anoAtual && p.TipoLancamento == ETipoOrcamento.Orcado);
        }        
        public Func<LancamentoClassificacaoDTO, bool> PredicateBaseOrcamentoRealizado { get { return predicateBaseOrcamentoRealizado; } }
        public Func<LancamentoClassificacaoDTO, bool> PredicateBaseOrcamentoPrevisto { get { return predicateBaseOrcamentoPrevisto; } }
        public Func<LancamentoClassificacaoDTO, bool> PredicateBaseOrcamentoReplan { get { return predicateBaseOrcamentoReplan; } }
        public Func<LancamentoClassificacaoDTO, bool> PredicateBaseOrcamentoOrcado { get { return predicateBaseOrcamentoOrcado; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFasePrevisto { get { return predicateFasePrevisto; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFasePrevisto_Realizado { get { return predicateFasePrevisto_Realizado; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFaseReplan { get { return predicateFaseReplan; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFaseOrcado { get { return predicateFaseOrcado; } }
        public Func<LancamentoClassificacaoDTO, bool> PredicateFormatoAcomp_realizado { get { return predicateFormatoAcomp_realizado; } }
        public Func<LancamentoClassificacaoDTO, bool> PredicateFormatoAcomp_tendencia { get { return predicateFormatoAcomp_tendencia; } }
        public Func<LancamentoClassificacaoDTO, bool> PredicateFormatoAcomp_ciclo { get { return predicateFormatoAcomp_ciclo; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFormatoAcompFase_realizado { get { return predicateFormatoAcompFase_realizado; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFormatoAcompFase_tendencia { get { return predicateFormatoAcompFase_tendencia; } }
        public Func<LancamentoFaseContabilDTO, bool> PredicateFormatoAcompFase_ciclo { get { return predicateFormatoAcompFase_ciclo; } }
        
    }
}
