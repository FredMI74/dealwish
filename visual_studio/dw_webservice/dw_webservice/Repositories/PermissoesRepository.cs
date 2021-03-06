using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;

namespace dw_webservice.Repositories
{
    public class PermissoesRepository 
    {
        IConfiguration configuration;
        public PermissoesRepository(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return configuration;
        }

        public object Consultar_permissao(int id = 0, string descricao = "", string codigo = "")
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {

                    if (id == 0 && string.IsNullOrWhiteSpace(descricao) && string.IsNullOrWhiteSpace(codigo))
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
                        var query = "select id, descricao, codigo from dealwish.permissoes where ";
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

                        if (!string.IsNullOrWhiteSpace(codigo))
                        {
                            query = query + (tem_param ? " and " : "") + " codigo = upper(:codigo)";
                            dyParam.Add("codigo", codigo, DbType.String, ParameterDirection.Input);
                            tem_param = true;
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
    }
}