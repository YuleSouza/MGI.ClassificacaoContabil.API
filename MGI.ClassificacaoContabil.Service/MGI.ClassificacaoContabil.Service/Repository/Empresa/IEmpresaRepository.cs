using Service.DTO.Combos;

namespace Service.Repository.Empresa
{
    public interface IEmpresaRepository
    {
        Task<IEnumerable<PayloadComboDTO>> ConsultarEmpresa();
    }
}
