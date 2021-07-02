using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace dw_webservice
{
    public class ValidarTokenPermissao
    {
        readonly IConfiguration configuration;
        public ValidarTokenPermissao(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public async Task<LoginUsuario> ValidarAsync(string token = "", string codigo_permissao = "", bool anonimo = false)
        {
            LoginUsuario loginUsuario = new()
            {
                Id = 0,
                Valido = false
             };

            bool token_permissao_ok = false;

            if (anonimo && token == configuration.GetSection("Chaves").GetSection("TokenAnonimo").Value)
            {
                loginUsuario.Valido = true;
                return loginUsuario;
            }

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(codigo_permissao))
                {
                    loginUsuario.Valido = false;
                    return loginUsuario;
                }

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {

                    var query = "select u.id from dealwish.usuarios u, dealwish.tokens t, dealwish.grp_usr g, dealwish.perm_grp pg, dealwish.permissoes p " +
                                "where u.id = t.id_usuario and t.val_token >= current_date and g.id_grp_permissao = pg.id_grp_permissao and p.id = pg.id_permissao and g.id_usuario = u.id " +
                                "and t.token = :token and p.codigo = lower(:codigo_permissao) limit 1";

                    var dyParam = new DynamicParameters();
                    dyParam.Add("token", token, DbType.String, ParameterDirection.Input);
                    dyParam.Add("codigo_permissao", codigo_permissao, DbType.String, ParameterDirection.Input);

                    var v_valido = await SqlMapper.QuerySingleAsync(conn, query, param: dyParam, commandType: CommandType.Text);
                    token_permissao_ok = v_valido.id != null;
                    loginUsuario.Id = token_permissao_ok ? unchecked((int)v_valido.id) : 0;

                }

                if (token_permissao_ok)
                {
                    var query = "update dealwish.tokens set val_token = current_timestamp + interval '30 min' where id_usuario = :id";

                    var dyParam = new DynamicParameters();
                    dyParam.Add("id", loginUsuario.Id, DbType.Int32, ParameterDirection.Input);

                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);

                    loginUsuario.Valido = true;
                    return loginUsuario;

                }
                else
                {
                    loginUsuario.Valido = false;
                    return loginUsuario;
                }

            }
            catch (Exception)
            {
                loginUsuario.Valido = false;
                return loginUsuario;
            }
        }
    }
}
