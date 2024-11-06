﻿namespace Service.DTO.PainelClassificacao
{
    public class FaseEsgDTO
    {
        public int IdEmpresa { get; set; }
        public int IdGrupoPrograma { get; set; }
        public DateTime DtLancamentoProjeto { get; set; }
        public string Nome { get; set; }
        public int SeqFase { get; set; }
        public string Pep { get; set; }
        public int IdClassificacaoEsg { get; set; }
        public LancamentoESG LancamentoESG { get; set; }
    }
}
