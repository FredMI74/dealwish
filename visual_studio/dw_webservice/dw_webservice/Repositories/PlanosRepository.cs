using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace dw_webservice.Repositories
{
    public class PlanosRepository
    {
        readonly IConfiguration _configuration;
        public PlanosRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public async Task<object> ConsultarPlanoAsync(int id = 0, string descricao = "")
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {

                if (id == 0 && string.IsNullOrWhiteSpace(descricao))
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
                    var query = "select id, descricao , qtd_ofertas, valor_mensal, valor_oferta, visualizacao, " +
                        " case visualizacao  " +
                        "   when 'N' then  'Sem visualização'  " +
                        "   when 'P' then  'Visualização de Preço'  " +
                        "   else 'Visualização de Empresa e Preço' end desc_visualizacao " +
                        " from dealwish.planos where ";
                    var tem_param = false;

                    if (id != 0)
                    {
                        query += " id = :id";
                        dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(descricao))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(descricao) like upper(:descricao)";
                        dyParam.Add("descricao", "%" + descricao + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    result = await SqlMapper.QueryAsync(conn, query, param: dyParam, commandType: CommandType.Text);

                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> IncluirPlanoAsync(string descricao = "", int qtd_ofertas = 0, float valor_mensal = 0, float valor_oferta = 0, string visualizacao = "", long id_usuario_login = 0)
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                if (string.IsNullOrWhiteSpace(descricao) || qtd_ofertas == 0 || valor_mensal == 0 || valor_oferta == 0 || string.IsNullOrWhiteSpace(visualizacao))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Descrição , Qtd. Ofertas, Valor Mensal, Valor Oferta e Visualização." }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "insert into dealwish.planos (descricao , qtd_ofertas, valor_mensal, valor_oferta, visualizacao, id_usuario_log) " +
                                "values (:descricao , :qtd_ofertas, :valor_mensal, :valor_oferta, :visualizacao, :id_usuario_login) returning id";

                    dyParam.Add("id", null, DbType.Int32, ParameterDirection.Output);
                    dyParam.Add("descricao", descricao, DbType.String, ParameterDirection.Input);
                    dyParam.Add("qtd_ofertas", qtd_ofertas, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("valor_mensal", valor_mensal, DbType.VarNumeric, ParameterDirection.Input);
                    dyParam.Add("valor_oferta", valor_oferta, DbType.VarNumeric, ParameterDirection.Input);
                    dyParam.Add("visualizacao", visualizacao, DbType.String, ParameterDirection.Input);
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                    await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);
                    result = new { ID = dyParam.Get<int>("id") };
                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> ExcluirPlanoAsync(int id = 0, long id_usuario_login = 0)
        {
            object result = null;
            IDbTransaction transaction = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {

                if (id == 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código." }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    transaction = conn.BeginTransaction();

                    var query = "update dealwish.planos set id_usuario_log = :id_usuario_login where id = :id ";
                    dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

                    query = "delete from dealwish.planos where id = :id ";

                    linhasafetadas += await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

                    transaction.Commit();

                    result = new { linhasafetadas };
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> AtualizarPlanoAsync(int id = 0, string descricao = "", int qtd_ofertas = 0, float valor_mensal = 0, float valor_oferta = 0, string visualizacao = "", long id_usuario_login = 0)
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                if (id == 0 || string.IsNullOrWhiteSpace(descricao) || qtd_ofertas == 0 || valor_mensal == 0 || valor_oferta == 0 || string.IsNullOrWhiteSpace(visualizacao))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código, Descrição, Qtd. Ofertas, Valor Mensal, Valor Oferta, Alerta e Visualização" }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "update dealwish.planos set descricao = :descricao, qtd_ofertas = :qtd_ofertas, valor_mensal = :valor_mensal, " +
                                "valor_oferta = :valor_oferta, visualizacao = :visualizacao, id_usuario_log = :id_usuario_login " +
                                "where id  = :id ";

                    dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("descricao", descricao, DbType.String, ParameterDirection.Input);
                    dyParam.Add("qtd_ofertas", qtd_ofertas, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("valor_mensal", valor_mensal, DbType.VarNumeric, ParameterDirection.Input);
                    dyParam.Add("valor_oferta", valor_oferta, DbType.VarNumeric, ParameterDirection.Input);
                    dyParam.Add("visualizacao", visualizacao, DbType.String, ParameterDirection.Input);
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