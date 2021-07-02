using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace dw_webservice.Repositories
{
    public class ConfigRepository 
    {
        readonly IConfiguration _configuration;
        public ConfigRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public async Task<object> ConsultarConfigAsync(int id = 0, string codigo = "", string valor = "")
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {

                if (id == 0 && string.IsNullOrWhiteSpace(codigo) && string.IsNullOrWhiteSpace(valor))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_CONSULTA }));
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
                        query += " id = :id";
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

                    query += " order by codigo";

                    result = await SqlMapper.QueryAsync(conn, query, param: dyParam, commandType: CommandType.Text);

                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }


        public async Task<string> ConsultarValorConfigAsync(string codigo)
        {
            using NpgsqlConnection conn = new(Constantes.string_conexao);

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
                    var retorno = await SqlMapper.QuerySingleAsync(conn, query, param: dyParam, commandType: CommandType.Text);
                    return retorno.valor;
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

        public async Task<string> AtualizarValorConfigAsync(string codigo, string valor)
        {
            using NpgsqlConnection conn = new(Constantes.string_conexao);

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
                    var retorno = await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);
                    return retorno.ToString();
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

        public async Task<object> AtualizarConfigAsync(int id = 0, string codigo = "", string valor = "", long id_usuario_login = 0)
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                if (id == 0 || string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(valor))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código e Valor" }));
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

                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);

                    result = new { linhasafetadas };
                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }
    }
}