using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;


namespace dw_webservice.Repositories
{
    public class ConfigRepository 
    {
        IConfiguration configuration;
        public ConfigRepository(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return configuration;
        }

        public object Consultar_config(int id = 0, string codigo = "", string valor = "")
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {

                    if (id == 0 && string.IsNullOrWhiteSpace(codigo) && string.IsNullOrWhiteSpace(valor))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_CONSULTA}));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "select id, codigo, valor from dealwish.configuracoes where ";
                        var tem_param = false;

                        if (id != 0)
                        {
                            query = query + " id = :id";
                            dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(codigo))
                        {
                            query = query + (tem_param ? " and " : "") + " upper(codigo) like upper(:codigo)";
                            dyParam.Add("codigo", "%" + codigo + "%", DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(valor))
                        {
                            query = query + (tem_param ? " and " : "") + " valor = upper(:valor)";
                            dyParam.Add("valor", valor, DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        query = query + " order by codigo";

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


        public static string Consultar_valor_config(string codigo)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "select valor from dealwish.configuracoes where codigo = :codigo";
                        dyParam.Add("codigo", codigo, DbType.String, ParameterDirection.Input);

                        return SqlMapper.QuerySingle(conn, query, param: dyParam, commandType: CommandType.Text).valor;
                    }
                    else
                    {
                        return "ERRO";
                    }

                }
                catch (Exception)
                {
                    return "ERRO";
                }
            }
        }

        public static string Atualizar_valor_config(string codigo, string valor)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "update dealwish.configuracoes set valor =:valor where codigo = :codigo ";

                        dyParam.Add("codigo", codigo, DbType.String, ParameterDirection.Input);
                        dyParam.Add("valor", valor, DbType.String, ParameterDirection.Input);

                        return SqlMapper.Execute(conn, query, dyParam, null, null, CommandType.Text).ToString();
                    }
                    else
                    {
                        return "ERRO";
                    }

                }
                catch (Exception)
                {
                    return "ERRO";
                }
            }
        }

        public object Atualizar_config(int id = 0, string codigo = "", string valor = "", int id_usuario_login = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (id == 0 || string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(valor))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código e Valor" }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "update dealwish.configuracoes set codigo = :codigo, valor =:valor, id_usuario_log = :id_usuario_login where id  = :id ";

                        dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("codigo", codigo, DbType.String, ParameterDirection.Input);
                        dyParam.Add("valor", valor, DbType.String, ParameterDirection.Input);
                        dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                        var linhasafetadas = SqlMapper.Execute(conn, query, dyParam, null, null, CommandType.Text);

                        result = new { linhasafetadas };
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