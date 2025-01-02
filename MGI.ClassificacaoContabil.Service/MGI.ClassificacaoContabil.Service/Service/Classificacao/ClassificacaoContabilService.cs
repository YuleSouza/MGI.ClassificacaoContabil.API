using DTO.Payload;
using Service.Helper;
using Service.DTO.Classificacao;
using Service.DTO.Filtros;
using Service.Interface.Classificacao;
using Service.Repository.Classificacao;
using Service.Base;

namespace MGI.ClassificacaoContabil.Service.Service.Classificacao
{
    public class ClassificacaoContabilService : ServiceBase, IClassificacaoContabilService
    {
        private IClassificacaoRepository _repository;       

        public ClassificacaoContabilService(IClassificacaoRepository classificacaoRepository, ITransactionHelper transactionHelper) : base(transactionHelper)
        {
            _repository = classificacaoRepository;
        }

        public async Task<PayloadDTO> InserirClassificacaoContabil(ClassificacaoContabilDTO classificacao)
        {
            return await ExecutarTransacao(
                async () => await _repository.InserirClassificacaoContabil(classificacao),
                "Classificação Contábil inserida com successo"
            );
        }
        public async Task<PayloadDTO> AlterarClassificacaoContabil(ClassificacaoContabilDTO classificacao)
        {
            var projetos = await _repository.ConsultarProjetoClassificacaoContabil(new FiltroClassificacaoContabil { IdClassificacaoContabil = classificacao.IdClassificacaoContabil });
            var projetoExcluidos = projetos.Where(a => !classificacao.Projetos.Any(b => b.IdClassificacaoContabilProjeto == a.IdClassificacaoContabilProjeto));
            return await ExecutarTransacao(
                async () => {
                    await _repository.DeletarProjetosClassificacaoContabil(projetoExcluidos.ToList());
                    await _repository.SalvarClassificacaoContabil(classificacao);
                    return true;
                },
                "Classificação Contábil inserida com successo"
            );
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
            return await ExecutarTransacao(
                async () => await _repository.InserirProjetoClassificacaoContabil(projeto)
            , "Projeto Classificação Contábil inserido com successo");
        }
        public async Task<PayloadDTO> AlterarProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto)
        {
            return await ExecutarTransacao(
                async () => await _repository.AlterarProjetoClassificacaoContabil(projeto)
            , "jeto Classificação Contábil alterado com successo");
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

        public async Task<IEnumerable<ClassificacaoContabilMgpDTO>> ConsultarClassificacaoContabilMGP()
        {
            return await _repository.ConsultarClassificacaoContabilMGP();
        }
    }
}
