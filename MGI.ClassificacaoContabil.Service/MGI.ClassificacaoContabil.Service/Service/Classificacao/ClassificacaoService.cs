using DTO.Payload;
using Infra.Interface;
using Service.DTO.Filtros;
using Service.DTO.Classificacao;
using Service.Interface.Classificacao;
using Service.Repository.Classificacao;
using OfficeOpenXml.Interfaces.Drawing.Text;

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
                    
                    var projetos = await _repository.ConsultarProjetoClassificacaoContabil(new FiltroClassificacaoContabil { IdClassificacaoContabil = classificacao.IdClassificacaoContabil });
                 
                    if (projetos.Any())
                    {
                        var projetosExistente = projetos.Except(classificacao.Projetos);

                        if (projetosExistente.Count() > classificacao.Projetos.Count())
                        {
                            var projetosInativos = projetos.Where(a => !classificacao.Projetos.Any(b => b.IdClassificacaoContabilProjeto == a.IdClassificacaoContabilProjeto));
                            projetosInativos.ToList().ForEach(P => P.Status = "I");

                            await _repository.DeletarProjetosClassificacaoContabil(projetosInativos.ToList());
                        }
                        else if (projetosExistente.Count() == classificacao.Projetos.Count())
                        {
                            await _repository.AlterarProjetosClassificacaoContabil(classificacao.Projetos.ToList());
                        }
                        else
                        {
                            var projetosNovos = classificacao.Projetos.Where(a => !projetos.Any(b => b.IdClassificacaoContabilProjeto == a.IdClassificacaoContabilProjeto));

                            await _repository.InserirProjetosClassificacaoContabil(projetosNovos.ToList());
                        }
                    }
                    else
                    {
                        await _repository.InserirProjetosClassificacaoContabil(classificacao.Projetos.ToList());
                    }
                    
                    if (!classificacao.MesAnoFim.HasValue || !classificacao.MesAnoInicio.HasValue)
                    {
                        return new PayloadDTO(string.Empty, false, "Data de início e fim são obrigatórias!");
                    }
                    bool ok = false;
                    if (classificacao!.IdClassificacaoContabil == 0)
                    {
                        ok = await _repository.InserirClassificacaoContabil(classificacao);
                    }
                    else
                    {
                        ok = await _repository.AlterarClassificacaoContabil(classificacao);
                    }
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
            if (resultado?.Count() > 0)
            {
                foreach (var item in resultado)
                {
                    item.Projetos = await _repository.ConsultarProjetoClassificacaoContabil(new FiltroClassificacaoContabil()
                    {
                        IdClassificacaoContabil = item.IdClassificacaoContabil
                    });
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
        public async Task<PayloadDTO> ConsultarProjetoClassificacaoContabil()
        {
            var resultado = await _repository.ConsultarProjetoClassificacaoContabil();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarProjetoClassificacaoContabil(FiltroClassificacaoContabil filtro)
        {
            var resultado = await _repository.ConsultarProjetoClassificacaoContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }

        public async Task<bool> VerificarRegraExcessaoContabil(FiltroClassificacaoContabil filtro)
        {
            var resultado = await _repository.ConsultarProjetoClassificacaoContabil(filtro);
            return resultado.Any();
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

        public async Task<IEnumerable<ClassificacaoContabilMgpDTO>> ConsultarClassificacaoContabilMGP()
        {
            return await _repository.ConsultarClassificacaoContabilMGP();
        }


        #endregion
    }
}
