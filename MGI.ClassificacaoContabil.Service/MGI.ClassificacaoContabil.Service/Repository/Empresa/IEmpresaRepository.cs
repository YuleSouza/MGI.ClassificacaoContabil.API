using Service.DTO.Empresa;

namespace Service.Repository.Empresa
{
    public interface IEmpresaRepository
    {
        Task<IEnumerable<EmpresaDTO>> ConsultarEmpresa();
    }
}
