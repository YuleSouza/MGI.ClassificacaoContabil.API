using DTO.Payload;
using Infra.Interface;
using Service.DTO.Filtros;
using Service.DTO.Classificacao;
using Service.Interface.Classificacao;
using Service.Repository.Classificacao;
using System.Collections.Generic;

using System.Linq;

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

                    if (classificacao?.Projetos.Count() > 0)
                    { 
                        var projetos = _repository.ConsultarProjetoClassificacaoContabil(new ClassificacaoContabilFiltro { IdClassificacaoContabil = classificacao.IdClassificacaoContabil });
                 
                        if (projetos.Result.Count() > 0)
                        {
                            var projetosExistente = projetos.Result.Except(classificacao.Projetos);

                            if (projetosExistente.Count() > classificacao.Projetos.Count())
                            {
                                var projetosInativos = projetos.Result.Where(a => !classificacao.Projetos.Any(b => b.IdClassificacaoContabilProjeto == a.IdClassificacaoContabilProjeto));
                                projetosInativos.ToList().ForEach(P => P.Status = "I");

                                await AlterarProjetosClassificacaoContabil(projetosInativos.ToList());
                            }
                            else if (projetosExistente.Count() == classificacao.Projetos.Count())
                            {
                                await AlterarProjetosClassificacaoContabil(classificacao.Projetos.ToList());
                            }
                            else
                            {
                                var projetosNovos = classificacao.Projetos.Where(a => !projetos.Result.Any(b => b.IdClassificacaoContabilProjeto == a.IdClassificacaoContabilProjeto));

                                InserirProjetosClassificacaoContabil(projetosNovos.ToList());
                            }
                        }
                        else
                        {
                            InserirProjetosClassificacaoContabil(classificacao.Projetos.ToList());
                        }
                    }
                    

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
        public async Task<PayloadDTO> ConsultarClassificacaoContabil()
        {
            var resultado = await _repository.ConsultarClassificacaoContabil();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarClassificacaoContabil(ClassificacaoContabilFiltro filtro)
        {
            var resultado = await _repository.ConsultarClassificacaoContabil(filtro);

            if (resultado?.Count() > 0)
            {
                foreach (var item in resultado)
                {
                    filtro.IdClassificacaoContabil = item.IdClassificacaoContabil;
                    item.Projetos = await _repository.ConsultarProjetoClassificacaoContabil(filtro);
                }
            }

            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> InserirProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.InserirProjetoClassificacaoContabil(projeto);
                    unitOfWork.Commit();
                    return new PayloadDTO("Projeto Classificação Contábil inserido com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro ao inserir Projeto Classificação Contábil", false, ex.Message);
                }
            }
        }
        public async Task<PayloadDTO> AlterarProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.AlterarProjetoClassificacaoContabil(projeto);
                    unitOfWork.Commit();
                    return new PayloadDTO("Projeto Classificação Contábil alterada com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro na alteração Projeto Classificação Contábil", false, ex.Message);
                }
            }
        }
        public async Task<PayloadDTO> InserirProjetosClassificacaoContabil(IList<ClassificacaoProjetoDTO> projetos)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.InserirProjetosClassificacaoContabil(projetos);
                    unitOfWork.Commit();
                    return new PayloadDTO("Projetos Classificação Contábil inseridos com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro ao inserir Projetos Classificação Contábil", false, ex.Message);
                }
            }
        }
        public async Task<PayloadDTO> AlterarProjetosClassificacaoContabil(IList<ClassificacaoProjetoDTO> projetos)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.AlterarProjetosClassificacaoContabil(projetos);
                    unitOfWork.Commit();
                    return new PayloadDTO("Projetos Classificação Contábil alterados com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro na alteração Projetos Classificação Contábil", false, ex.Message);
                }
            }
        }
        public async Task<PayloadDTO> ConsultarProjetoClassificacaoContabil()
        {
            var resultado = await _repository.ConsultarProjetoClassificacaoContabil();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarProjetoClassificacaoContabil(ClassificacaoContabilFiltro filtro)
        {
            var resultado = await _repository.ConsultarProjetoClassificacaoContabil(filtro);
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
        public async Task<PayloadDTO> ConsultarClassificacaoEsg()
        {
            var resultado = await _repository.ConsultarClassificacaoEsg();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarClassificacaoEsg(ClassificacaoEsgFiltro filtro)
        {
            var resultado = await _repository.ConsultarClassificacaoEsg(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        #endregion
    }
}
