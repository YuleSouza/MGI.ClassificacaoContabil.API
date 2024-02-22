using DTO.Payload;
using Service.DTO.Filtros;
using Infra.Interface;
using Service.DTO.Classificacao;
using Service.Interface.Classificacao;
using Service.Repository.Classificacao;

namespace Service.Classificacao
{
    public class ClassificacaoService : IClassificacaoService
    {
        private IClassificacaoRepository _repository;
        private IUnitOfWork _unitOfWork;

        public ClassificacaoService(IClassificacaoRepository classificacaoRepository, IUnitOfWork unitOfWork)
        {
            _repository = classificacaoRepository;
            _unitOfWork = unitOfWork;
        }

        #region Contabil
        public async Task<PayloadDTO> InserirClassificacaoContabil(ClassificacaoContabilDTO classificacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.InserirClassificacaoContabil(classificacao);
                    unitOfWork.Commit();
                    return new PayloadDTO("Classificação Contábil inserida com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro ao inserir Classificação Contábil", false, ex.Message);
                }
            }
        }
        public async Task<PayloadDTO> AlterarClassificacaoContabil(ClassificacaoContabilDTO classificacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.AlterarClassificacaoContabil(classificacao);
                    unitOfWork.Commit();
                    return new PayloadDTO("Classificação Contábil alterada com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro na alteração Classificação Contábil", false, ex.Message);
                }
            }
        }
        public async Task<PayloadDTO> ConsultarClassificacaoContabil(ClassificacaoContabilFiltro filtro)
        {
            var resultado = await _repository.ConsultarClassificacaoContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        #endregion

        #region ESG
        public async Task<PayloadDTO> InserirClassificacaoEsg(ClassificacaoEsgDTO classificacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.InserirClassificacaoEsg(classificacao);
                    unitOfWork.Commit();
                    return new PayloadDTO("Classificação Contábil inserida com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro ao inserir Classificação Contábil", false, ex.Message);
                }
            }
        }
        public async Task<PayloadDTO> AlterarClassificacaoEsg(ClassificacaoEsgDTO classificacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.AlterarClassificacaoEsg(classificacao);
                    unitOfWork.Commit();
                    return new PayloadDTO("Classificação Contábil alterada com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro na alteração Classificação Contábil", false, ex.Message);
                }
            }
        }
        public async Task<PayloadDTO> ConsultarClassificacaoEsg(ClassificacaoEsgFiltro filtro)
        {
            var resultado = await _repository.ConsultarClassificacaoEsg(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        #endregion
    }
}
