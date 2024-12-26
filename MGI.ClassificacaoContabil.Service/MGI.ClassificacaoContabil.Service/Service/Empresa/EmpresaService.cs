using Infra.Interface;
using DTO.Payload;
using Service.Interface.Empresa;
using Service.Repository.Empresa;

namespace Service.Empresa
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _EmpresaRepository;        
        public EmpresaService(IEmpresaRepository EmpresaRepository)
        {
            _EmpresaRepository = EmpresaRepository;            
        }
        public async Task<PayloadDTO> ConsultarEmpresa()
        {
            var resultado = await _EmpresaRepository.ConsultarEmpresa();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
    }
}
