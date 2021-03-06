using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;

namespace dw_webservice.Repositories
{
    public class TpProdutosRepository 
    {
        IConfiguration configuration;
        public TpProdutosRepository(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return configuration;
        }

        public object Consultar_tp_produto(int id = 0, string descricao = "", int id_grp_prod = 0, int id_situacao = 0, string paginacao = "N", int max_id = 0, int pagina = 1)
        {
            object result = null;
            object result_max_count_id = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {

                    if (id == 0 && string.IsNullOrWhiteSpace(descricao) && id_grp_prod == 0 && id_situacao == 0)
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
                        var query_max_count_id = "select max(tp.id) max_id, count(tp.id) count_id ";

                        var query_campos = "select tp.id, tp.descricao, tp.icone, tp.id_grp_prod, tp.preenchimento, grp.descricao desc_grp_produto, tp.id_situacao, s.descricao desc_situacao, tp.ordem ";

                        var query = " from dealwish.tp_produtos tp, dealwish.grp_produtos grp, dealwish.situacoes s " +
                                    " where tp.id_grp_prod = grp.id and tp.id_situacao = s.id and (";

                        var query_pagina = " and (tp.id <= :max_id or :max_id = 0) order by grp.ordem, tp.ordem, tp.descricao, tp.id offset :inicio rows fetch next 20 rows only ";

                        var query_order = " order by grp.ordem, tp.ordem, tp.descricao";

                        dyParam.Add("max_id", max_id, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("inicio", (pagina - 1) * 20, DbType.Int32, ParameterDirection.Input);

                        var tem_param = false;

                        if (id != 0)
                        {
                            query = query + " tp.id = :id";
                            dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(descricao))
                        {
                            query = query + (tem_param ? " and " : "") + " upper(tp.descricao) like upper(:descricao)";
                            dyParam.Add("descricao", "%" + descricao + "%", DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (id_grp_prod != 0)
                        {
                            query = query + (tem_param ? " and " : "") + " tp.id_grp_prod = :id_grp_prod";
                            dyParam.Add("id_grp_prod", id_grp_prod, DbType.Int32, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (id_situacao != 0)
                        {
                            query = query + (tem_param ? " and " : "") + " tp.id_situacao = :id_situacao";
                            dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                            tem_param = true;
                        }

                        query = query + " )";

                        if (paginacao == "S")
                        {
                            if (pagina == 1)
                            {
                                result_max_count_id = SqlMapper.QuerySingle<Paginacao>(conn, query_max_count_id + query, param: dyParam, commandType: CommandType.Text);
                            }
                            query = query + query_pagina;
                        }
                        else
                        {
                            query = query + query_order;
                        }

                        result = SqlMapper.Query(conn, query_campos + query, param: dyParam, commandType: CommandType.Text);

                    }
                }
                catch (Exception ex)
                {
                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }, result_max_count_id));
            }
        }

        public object Incluir_tp_produto(string descricao = "", int id_grp_prod = 0, string preenchimento = "", string icone = "", int ordem = 0, int id_usuario_login = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {


                try
                {
                    if (string.IsNullOrWhiteSpace(descricao) || id_grp_prod == 0 || string.IsNullOrWhiteSpace(preenchimento) || string.IsNullOrWhiteSpace(icone) || ordem == 0)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Descrição, Cód. Grupo Produto, Preenchimento, Ícone e Ordem" }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        //Novo Tipo de Produto sempre inicia como 2 - Inativo
                        var query = "insert into dealwish.tp_produtos (descricao, id_grp_prod,id_situacao, preenchimento, icone, ordem, id_usuario_log) " +
                                    "values (:descricao, :id_grp_prod, 2, :preenchimento, :icone, :ordem, :id_usuario_login) returning id";

                        dyParam.Add("id", null, DbType.Int32, ParameterDirection.Output);
                        dyParam.Add("descricao", descricao, DbType.String, ParameterDirection.Input);
                        dyParam.Add("id_grp_prod", id_grp_prod, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("preenchimento", preenchimento, DbType.String, ParameterDirection.Input);
                        dyParam.Add("icone", icone, DbType.String, ParameterDirection.Input);
                        dyParam.Add("ordem", ordem, DbType.Int16, ParameterDirection.Input);
                        dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                        SqlMapper.Execute(conn, query, dyParam, null, null, CommandType.Text);

                        Utils.Atualizar_valor_config("ultima_atualizacao_produtos", DateTime.Now.ToString());

                        result = new { ID = dyParam.Get<int>("id") };
                    }
                }
                catch (Exception ex)
                {
                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

        public object Excluir_tp_produto(int id = 0, int id_usuario_login = 0)
        {
            object result = null;
            IDbTransaction transaction = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (id == 0)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código." }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        transaction = conn.BeginTransaction();

                        var query = "update dealwish.tp_produtos set id_usuario_log = :id_usuario_login where id = :id ";
                        dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int32, ParameterDirection.Input);
                        var linhasafetadas = SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        query = "delete from dealwish.tp_produtos where id = :id ";
                        linhasafetadas = linhasafetadas + SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        Utils.Atualizar_valor_config("ultima_atualizacao_produtos", DateTime.Now.ToString());

                        transaction.Commit();

                        result = new { linhasafetadas };
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

        public object Atualizar_tp_produto(int id = 0, string descricao = "", int id_grp_prod = 0, int id_situacao = 0, string preenchimento = "", string icone = "", int ordem = 0, int id_usuario_login = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (id == 0 || string.IsNullOrWhiteSpace(descricao) || string.IsNullOrWhiteSpace(icone) || string.IsNullOrWhiteSpace(preenchimento) ||
                        id_grp_prod == 0 || id_situacao == 0 || ordem == 0)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código, Descrição, Ícone, Cód. Grupo Produto, Preenchimento, Cód. Situação e Ordem" }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "update dealwish.tp_produtos set descricao = :descricao, id_grp_prod = :id_grp_prod, preenchimento = :preenchimento, " +
                                    "id_situacao = :id_situacao, icone = :icone, ordem = :ordem, id_usuario_log = :id_usuario_login where id  = :id ";

                        dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("descricao", descricao, DbType.String, ParameterDirection.Input);
                        dyParam.Add("id_grp_prod", id_grp_prod, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("preenchimento", preenchimento, DbType.String, ParameterDirection.Input);
                        dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("icone", icone, DbType.String, ParameterDirection.Input);
                        dyParam.Add("ordem", ordem, DbType.Int16, ParameterDirection.Input);
                        dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                        var linhasafetadas = SqlMapper.Execute(conn, query, dyParam, null, null, CommandType.Text);

                        Utils.Atualizar_valor_config("ultima_atualizacao_produtos", DateTime.Now.ToString());

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