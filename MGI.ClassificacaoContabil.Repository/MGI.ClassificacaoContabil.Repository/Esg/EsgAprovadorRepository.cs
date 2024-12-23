using Dapper;
using Infra.Data;
using Service.DTO.Esg;
using Service.Repository.Esg;

namespace Repository.Esg
{
    public class EsgAprovadorRepository : IEsgAprovadorRepository
    {
        private readonly DbSession _session;
        public EsgAprovadorRepository(DbSession session)
        {
            _session = session;
        }

        public async Task<bool> AlterarUsuarioAprovador(string email, int id)
        {
            int qtdLinhas = await _session.Connection.ExecuteAsync(@"update esg_aprovadores set email = :email where id = :id", new { email, id});
            return qtdLinhas == 1;
        }

        public async Task<EsgAprovadorDTO> ConsultarAprovadorPorUsuario(string usuario)
        {
            return await _session.Connection.QueryFirstOrDefaultAsync(@"select id as IdEsgAprovador
                                                                            , id_usuario as usuario
                                                                            , email as Email
                                                                            , status 
                                                                            from esg_aprovadores 
                                                                            where id_usuario = :usuario", new
                                                                                        {
                                                                                            usuario 
                                                                                        });
        }

        public async Task<bool> InserirUsuarioAprovador(string usuario, string email)
        {
            int qtdLinhas = await _session.Connection.ExecuteAsync(@"insert into esg_aprovadores (id_usuario, email) values (:usuario, :email)", new
            {
                usuario,
                email
            });
            return qtdLinhas == 1;
        }

        public async Task<bool> RemoverUsuarioAprovador(int id)
        {
            var qtdLinha = await _session.Connection.ExecuteAsync("delete from esg_aprovadores where id = :id", new { id });
            return qtdLinha == 1;
        }
    }
}
