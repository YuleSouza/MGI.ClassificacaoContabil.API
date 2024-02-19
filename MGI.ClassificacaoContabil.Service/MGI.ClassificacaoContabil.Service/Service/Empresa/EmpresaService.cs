﻿using Infra.Interface;
using Service.DTO.Payload;
using Service.Interface.Empresa;
using Service.Repository.Empresa;

namespace Service.Empresa
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _EmpresaRepository;

        private IUnitOfWork _unitOfWork;
        public EmpresaService(IEmpresaRepository EmpresaRepository, IUnitOfWork unitOfWork)
        {
            _EmpresaRepository = EmpresaRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<PayloadDTO> ConsultarEmpresa()
        {
            var resultado = await _EmpresaRepository.ConsultarEmpresa();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
    }
}
