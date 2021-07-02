using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace dw_webservice.Repositories
{
    public class GrpProdutosRepository 
    {
        readonly IConfiguration _configuration;
        public GrpProdutosRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return _configuration;
        }


        public async Task<object> ConsultarTodosGrpProdutoAsync()
        {
            object result = null;
            Utils utils = new();

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
                    var query = "with tipos as ( select id_grp_prod, string_agg(tp.descricao, ', ' order by tp.ordem)  as tipos_produtos " +
                        "from dealwish.tp_produtos tp where tp.id_situacao = :situacao group by id_grp_prod) " +
                        "select gp.id, gp.descricao, tipos_produtos, gp.icone " +
                        "from dealwish.grp_produtos gp, tipos tp " +
                        "where gp.id = tp.id_grp_prod and gp.id_situacao = :situacao " +
                        "order by gp.ordem, gp.descricao";
                    dyParam.Add("situacao", Constantes.ATIVO, DbType.Int32, ParameterDirection.Input);

                    result = await SqlMapper.QueryAsync(conn, query, param: dyParam, commandType: CommandType.Text);

                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }


        public async Task<object> ConsultarGrpProdutoAsync(int id = 0, string descricao = "", int id_situacao = 0)
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {

                if (id == 0 && string.IsNullOrWhiteSpace(descricao) && id_situacao == 0)
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
                    var query = "select gp.id, gp.descricao, gp.id_situacao, s.descricao desc_situacao, gp.icone, gp.ordem " +
                                "from dealwish.grp_produtos gp, dealwish.situacoes s " +
                                "where gp.id_situacao = s.id and ( ";
                    var tem_param = false;

                    if (id != 0)
                    {
                        query += " gp.id = :id";
                        dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(descricao))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(gp.descricao) like upper(:descricao)";
                        dyParam.Add("descricao", "%" + descricao + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (id_situacao != 0)
                    {
                        query = query + (tem_param ? " and " : "") + " gp.id_situacao = :id_situacao";
                        dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    query += " ) order by gp.ordem, gp.descricao";

                    result = await SqlMapper.QueryAsync(conn, query, param: dyParam, commandType: CommandType.Text);

                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> IncluirGrpProdutoAsync(string descricao = "", string icone = "", int ordem = 0, long id_usuario_login = 0)
        {
            object result = null;
            Utils utils = new();
            ConfigRepository configRepository = new(_configuration);

            using NpgsqlConnection conn = new(Constantes.string_conexao);
            try
            {
                if (string.IsNullOrWhiteSpace(descricao) || string.IsNullOrWhiteSpace(icone) || ordem == 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Descrição, Ícone e Ordem." }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    // Novo grupo sempre inicia como Inativo
                    var query = "insert into dealwish.grp_produtos (descricao, id_situacao, icone, ordem, id_usuario_log) " +
                                "values (:descricao, :situacao, :icone, :ordem, :id_usuario_login) returning id";

                    dyParam.Add("id", null, DbType.Int32, ParameterDirection.Output);
                    dyParam.Add("descricao", descricao, DbType.String, ParameterDirection.Input);
                    dyParam.Add("icone", icone, DbType.String, ParameterDirection.Input);
                    dyParam.Add("situacao", Constantes.INATIVO, DbType.String, ParameterDirection.Input);
                    dyParam.Add("ordem", ordem, DbType.Int16, ParameterDirection.Input);
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                    await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);

                    await configRepository.AtualizarValorConfigAsync("ultima_atualizacao_produtos", DateTime.Now.ToString());

                    result = new { ID = dyParam.Get<int>("id") };
                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> ExcluirGrpProdutoAsync(int id = 0, long id_usuario_login = 0)
        {
            object result = null;
            IDbTransaction transaction = null;
            Utils utils = new();
            ConfigRepository configRepository = new(_configuration);

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

                    var query = "update dealwish.grp_produtos set id_usuario_log = :id_usuario_login where id = :id ";
                    dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);
                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

                    query = "delete from dealwish.grp_produtos where id = :id ";

                    linhasafetadas += await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

                    await configRepository.AtualizarValorConfigAsync("ultima_atualizacao_produtos", DateTime.Now.ToString());

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

        public async Task<object> AtualizarGrpProdutoAsync(int id = 0, string descricao = "", int id_situacao = 0, string icone = "", int ordem = 0, long id_usuario_login = 0)
        {
            object result = null;
            Utils utils = new();
            ConfigRepository configRepository = new(_configuration);

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                if (id == 0 || string.IsNullOrWhiteSpace(descricao) || string.IsNullOrWhiteSpace(icone) || id_situacao == 0 || ordem == 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código,  Descrição, Cód. Situação, Ícone e Ordem" }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "update dealwish.grp_produtos set descricao = :descricao, id_situacao = :id_situacao, icone = :icone, " +
                                "ordem = :ordem,  id_usuario_log = :id_usuario_login where id  = :id ";

                    dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("descricao", descricao, DbType.String, ParameterDirection.Input);
                    dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("icone", icone, DbType.String, ParameterDirection.Input);
                    dyParam.Add("ordem", ordem, DbType.Int16, ParameterDirection.Input);
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);

                    await configRepository.AtualizarValorConfigAsync("ultima_atualizacao_produtos", DateTime.Now.ToString());

                    result = new { linhasafetadas };
                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> ConsultarUltimaAtualizacaoProdutosAsync()
        {
            Utils utils = new();
            ConfigRepository configRepository = new(_configuration);

            return utils.FormataRetorno(new { ultima_atualizacao_produtos = await configRepository.ConsultarValorConfigAsync("ultima_atualizacao_produtos")}, new { Erro = false, Mensagem = "" });
        }

    }
}