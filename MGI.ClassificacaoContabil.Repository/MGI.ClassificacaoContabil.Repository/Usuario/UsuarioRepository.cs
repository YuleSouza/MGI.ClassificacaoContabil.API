using Dapper;
using Infra.Data;
using Service.DTO;
using Service.Repository.Usuario;

namespace Repository.Usuario
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DbSession _session;

        public UsuarioRepository(DbSession session)
        {
            _session = session;
        }
        public async Task<IEnumerable<UsuarioDTO>> ConsultarUsuarioPorLogin(string login)
        {
            var resultado = await _session.Connection.QueryAsync<UsuarioDTO>($@"
                                           SELECT trim(g.GERADMUSU) as Login
                                                , u.UsuMai as Email
                                                , trim(g.GERADMPFL) as Grupo 
                                             FROM GERADM g, corpora.USUARI u 
                                            WHERE g.GERADMUSU = u.USULOG 
                                              AND upper(trim(g.GERADMUSU)) = :login
                                        ", new
            {
                login = login                
            });

            return resultado;
        }
    }
}
