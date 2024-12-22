using Service.DTO.Esg;

namespace Service.Repository.Esg
{
    public interface IEsgAnexoRepository
    {
        Task<bool> ApagarAnexo(int id);
        Task<IEnumerable<AnexoJustificaitvaClassifEsgDTO>> ConsultarAnexos(int idJustifClassif);
        Task<AnexoJustificaitvaClassifEsgDTO> ConsultarAnexoiPorId(int idAnexo);
        Task<int> InserirAnexoJustificativaEsg(List<AnexoJustificaitvaClassifEsgDTO> anexos);
        Task<int> InserirAnexoJustificativaEsg(AnexoJustificaitvaClassifEsgDTO anexo);
        Task<IEnumerable<AnexosMGPDTO>> ConsultarAnexosMGP(int idProjeto, int seqMeta);
        Task<IEnumerable<ImportacaoProjetoEsgMGPDTO>> ConsultarProjetosEsgMGP();
    }
}
