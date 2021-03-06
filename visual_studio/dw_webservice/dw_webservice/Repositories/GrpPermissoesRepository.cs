using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;

namespace dw_webservice.Repositories
{
    public class GrpPermissoesRepository 
    {
        IConfiguration configuration;
        public GrpPermissoesRepository(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return configuration;
        }

        public object Consultar_grp_permissao(int id = 0, int id_usuario_login = 0, string descricao = "", string origem = "")
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {

                    if ((id == 0 && string.IsNullOrWhiteSpace(descricao)) || (string.IsNullOrWhiteSpace(origem)))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_CONSULTA }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "select id, descricao from dealwish.grp_permissoes where ( ";
                        var tem_param = false;

                        if (id != 0)
                        {
                            query = query + " id = :id";
                            dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(descricao))
                        {
                            query = query + (tem_param ? " and " : "") + " upper(descricao) like upper(:descricao)";
                            dyParam.Add("descricao", "%" + descricao + "%", DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        query = query + " ) ";

                        if (origem == Constantes.FRONTOFFICE)
                        {
                            query = query + " and  (codigo in ('fto', 'fta'))";
                        }

                        if (origem == Constantes.BACKOFFICE)
                        {
                            query = query + " and  (codigo in ('bka', 'bkc', 'bkf', 'bki', 'fto', 'fta', " +
                                                  "(select gp.codigo from dealwish.grp_usr gu, dealwish.grp_permissoes gp " +
                                                  " where gu.id_grp_permissao = gp.id and gp.codigo = 'tin' and gu.id_usuario = :id_usuario_login limit 1)))";
                            dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);
                        }

                        result = SqlMapper.Query(conn, query, param: dyParam, commandType: CommandType.Text);

                    }
                }
                catch (Exception ex)
                {
                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

        public object Consultar_permissao_grupo(int id_grp_permissao = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {

                    if (id_grp_permissao == 0)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_CONSULTA }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "select pg.id, pg.id_permissao, p.descricao, p.codigo from dealwish.perm_grp pg, dealwish.permissoes p where p.id = pg.id_permissao and pg.id_grp_permissao = :id_grp_permissao";

                        dyParam.Add("id_grp_permissao", id_grp_permissao, DbType.Int32, ParameterDirection.Input);

                        result = SqlMapper.Query(conn, query, param: dyParam, commandType: CommandType.Text);

                    }
                }
                catch (Exception ex)
                {
                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));

            }
        }
    }
}