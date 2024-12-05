﻿namespace Service.DTO.Esg
{
    public class ProjetoEsgDTO
    {
        public int IdProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public int IdEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public string IdGestor {  get; set; }
        public decimal ValorFormatoAcompanhamento { get; set; }
        public decimal ValorBaseOrcamento { get; set; }
        public string IdStatusProjeto { get; set; }
        public string DescricaoStatusProjeto { get; set; }
        public string StatusAprovacao { get; set; }
    }
}
