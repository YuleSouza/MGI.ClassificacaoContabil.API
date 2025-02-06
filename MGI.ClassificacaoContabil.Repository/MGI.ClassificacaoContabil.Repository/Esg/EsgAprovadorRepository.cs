using Dapper;
using Infra.Data;
using Service.DTO.Esg;
using Service.Repository.Esg;
using System.Text;

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

        public async Task<IEnumerable<EsgAprovadorDTO>> ConsultarUsuarioAprovador(string usuario, string email)
        {
            StringBuilder parametros = new StringBuilder();
            if (!string.IsNullOrEmpty(usuario)) 
            { 
                parametros.Append(" and id_usuario = :usuario");
            }
            if (!string.IsNullOrEmpty(email))
            {
                parametros.Append(" and email = :email");
            }
            var result = await _session.Connection.QueryAsync<EsgAprovadorDTO>(@$"select id        as IdEsgAprovador
                                                                            , id_usuario as usuario
                                                                            , email      as Email
                                                                            , status     as Status
                                                                            from esg_aprovadores 
                                                                            where 1 = 1 {parametros}" , new
                                                                            {
                                                                                usuario,
                                                                                email
                                                                            });
            return result;
        }

        public Task<IEnumerable<EsgAprovadorDTO>> ConsultarUsuariosSustentabilidade()
        {
            return _session.Connection.QueryAsync<EsgAprovadorDTO>(@"SELECT g.GERADMUSU as Usuario, trim(u.UsuMai) as Email
                                                                       FROM GERADM g, corpora.USUARI u 
                                                                      WHERE g.GERADMUSU = u.USULOG 
                                                                        AND trim(g.GERADMPFL) = 'Sustentabilidade'");
        }

        public async Task<bool> InserirUsuarioAprovador(string usuario, string email)
        {
            int qtdLinhas = await _session.Connection.ExecuteAsync(@"insert into SERVDESK.esg_aprovadores (id_usuario, email) values (:usuario, :email)", new
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
