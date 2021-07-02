using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace dw_webservice.Repositories
{
    public class DesejosRepository 
    {
        readonly IConfiguration _configuration;
        public DesejosRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public async Task<object> ConsultarDesejoAsync(long id_usuario_login = 0, long id = 0, string descricao = "", long id_usuario = 0, int id_tipo_produto = 0, int id_situacao = 0, string oferta = "", 
                                       int id_empresa_oferta = 0, string uf = "", int id_cidade = 0, string paginacao = "N", int max_id = 0, int pagina = 1, string exportar = "N")
        {
            IEnumerable<dynamic> result = null;
            object result_max_count_id = null;
            string nome_arquivo = "";
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {

                if ((id == 0 && string.IsNullOrWhiteSpace(descricao) && id_usuario == 0 && id_tipo_produto == 0 && id_situacao == 0 && string.IsNullOrWhiteSpace(uf) && id_cidade == 0 && string.IsNullOrWhiteSpace(oferta))
                    || (id_empresa_oferta != Constantes.ID_DEALWISH && id_empresa_oferta != 0 && string.IsNullOrWhiteSpace(oferta)))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_CONSULTA }));
                }

                if (!string.IsNullOrWhiteSpace(oferta))
                {
                    if (oferta != "sem_oferta" && oferta != "com_oferta" && oferta != "like" && oferta != "unlike" && oferta != "perdendo")
                    {
                        return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Parâmetro OFERTA deve ser sem_oferta, com_oferta, perdendo, like ou unlike." }));
                    }
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query_max_count_id = "select max(d.id) max_id, count(d.id) count_id ";

                    var query_campos = "select d.id, d.descricao, c.nome cidade, c.uf, ";

                    dyParam.Add("id_empresa_oferta", id_empresa_oferta, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_dealwish", Constantes.ID_DEALWISH, DbType.Int32, ParameterDirection.Input);

                    if (id_empresa_oferta == Constantes.ID_DEALWISH)
                    {
                        query_campos += "d.id_usuario, u.nome nome_usuario, u.email email_usuario, ";
                    }

                    if (exportar == Constantes.NAO)
                    {
                        query_campos += "tp.icone icone_tp_produto, ";
                    }
                    query_campos = query_campos + " d.id_tipo_produto, tp.descricao desc_tp_produto, d.id_situacao, s.descricao desc_situacao," +
                                                  "(select count(1) from dealwish.ofertas o where d.id = o.id_desejo and o.validade >= current_date and o.id_situacao = 1) qtd_ofertas ";

                    var query = "from dealwish.desejos d, dealwish.usuarios u, dealwish.tp_produtos tp, dealwish.situacoes s, dealwish.cidades c " +
                                "where d.id_usuario = u.id and d.id_tipo_produto = tp.id and d.id_situacao = s.id and c.id = u.id_cidade_ap and (";

                    var query_pagina = " and (d.id <= :max_id or :max_id = 0) order by d.id offset :inicio rows fetch next 20 rows only ";
                    dyParam.Add("max_id", max_id, DbType.Int64, ParameterDirection.Input);
                    dyParam.Add("inicio", (pagina - 1) * 20, DbType.Int32, ParameterDirection.Input);

                    var tem_param = false;

                    if (id != 0)
                    {
                        query += " d.id = :id";
                        dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(descricao))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(d.descricao) like upper(:descricao)";
                        dyParam.Add("descricao", "%" + descricao + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (id_usuario != 0)
                    {
                        query = query + (tem_param ? " and " : "") + " d.id_usuario = :id_usuario";
                        dyParam.Add("id_usuario", id_usuario, DbType.Int64, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (id_tipo_produto != 0)
                    {
                        query = query + (tem_param ? " and " : "") + " d.id_tipo_produto = :id_tipo_produto";
                        dyParam.Add("id_tipo_produto", id_tipo_produto, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (id_situacao != 0)
                    {
                        query = query + (tem_param ? " and " : "") + " d.id_situacao = :id_situacao";
                        dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }


                    if (!string.IsNullOrWhiteSpace(uf))
                    {
                        query = query + (tem_param ? " and " : "") + " c.uf = :uf";
                        dyParam.Add("uf", uf, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (id_cidade != 0)
                    {
                        query = query + (tem_param ? " and " : "") + " c.id = :id_cidade";
                        dyParam.Add("id_cidade", id_cidade, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(oferta))
                    {
                        var base_qry_oferta = "(select count(1) from dealwish.ofertas oft where oft.id_desejo = d.id ";

                        if (id_empresa_oferta != Constantes.ID_DEALWISH)
                        {
                            base_qry_oferta += " and oft.id_empresa = :id_empresa_oferta and oft.id_empresa != :id_dealwish ";
                        }

                        switch (oferta.ToLower())
                        {
                            case "sem_oferta":
                                query = query + (tem_param ? " and " : "") + base_qry_oferta + ") = 0";
                                break;
                            case "com_oferta":
                                query = query + (tem_param ? " and " : "") + base_qry_oferta + ") > 0";
                                break;
                            case "like":
                                query = query + (tem_param ? " and " : "") + base_qry_oferta + " and oft.like_unlike = 'L') > 0";
                                break;
                            case "unlike":
                                query = query + (tem_param ? " and " : "") + base_qry_oferta + " and oft.like_unlike = 'U') > 0";
                                break;
                            case "perdendo":
                                query = query + (tem_param ? " and " : "") + base_qry_oferta + " and oft.valor > (select min(valor) from " +
                                                                                                                  "dealwish.ofertas moft " +
                                                                                                                  "where moft.id_desejo = oft.id_desejo " +
                                                                                                                  "and moft.id_empresa <> :id_empresa_oferta " +
                                                                                                                  "and moft.id_empresa != :id_dealwish)) > 0";
                                break;
                        }

                    }

                    query += ")";

                    if (paginacao == "S")
                    {
                        if (pagina == 1)
                        {
                            result_max_count_id = await SqlMapper.QuerySingleAsync<Paginacao>(conn, query_max_count_id + query, param: dyParam, commandType: CommandType.Text);
                        }
                        query += query_pagina;
                    }

                    result = await SqlMapper.QueryAsync(conn, query_campos + query, param: dyParam, commandType: CommandType.Text);

                }

                if (exportar == Constantes.SIM)
                {
                    return utils.DapperQueryToCsvFile(result, id_usuario_login, "desejos");
                }
            }
            catch (Exception ex)
            {
                return utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message });
            }

            if (exportar == Constantes.NAO)
            {
                return utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }, result_max_count_id);
            }
            else
            {
                return utils.FormataRetorno(new { Nome_arquivo = nome_arquivo }, new { Erro = false, Mensagem = "" });
            }
        }

        public async Task<object> IncluirDesejoAsync( string descricao = "", long id_usuario = 0, int id_tipo_produto = 0, int id_situacao = 0)
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                if (string.IsNullOrWhiteSpace(descricao) || id_usuario == 0 || id_tipo_produto == 0 || id_situacao == 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Descrição, Cód. Usuário, Cód. Tipo Produto ou Cód. Situação." }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "insert into dealwish.desejos (descricao, id_usuario, id_tipo_produto, id_situacao) values " +
                                "(:descricao, :id_usuario, :id_tipo_produto, :id_situacao) returning id";

                    dyParam.Add("id", null, DbType.Int64, ParameterDirection.Output);
                    dyParam.Add("descricao", descricao, DbType.String, ParameterDirection.Input);
                    dyParam.Add("id_usuario", id_usuario, DbType.Int64, ParameterDirection.Input);
                    dyParam.Add("id_tipo_produto", id_tipo_produto, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);

                    await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);

                    string id_desejo = dyParam.Get<long>("id").ToString();
                    result = new { ID = dyParam.Get<long>("id") };
                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> ExcluirDesejoAsync(long id = 0)
        {
            object result = null;
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
                    var query = "delete from dealwish.desejos where id = :id ";

                    dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);

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

        public async Task<object> AtualizarDesejoAsync(long id = 0, string descricao = "", long id_usuario = 0, int id_tipo_produto = 0, int id_situacao = 0)
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                if (id == 0 || string.IsNullOrWhiteSpace(descricao) || id_usuario == 0 || id_tipo_produto == 0 || id_situacao == 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código, DEscrição, Cód. Usuário, Cód. Tipo Produto e Cód. Situação." }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "update dealwish.desejos set descricao = :descricao, id_usuario = :id_usuario, " +
                                "id_tipo_produto = :id_tipo_produto, id_situacao = : id_situacao where id  = :id ";

                    dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                    dyParam.Add("descricao", descricao, DbType.String, ParameterDirection.Input);
                    dyParam.Add("id_usuario", id_usuario, DbType.Int64, ParameterDirection.Input);
                    dyParam.Add("id_tipo_produto", id_tipo_produto, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);

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

        public async Task<object> AtualizarSituacaoDesejoAsync(long id = 0, int id_situacao = 0)
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                if (id == 0 || id_situacao == 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código e Cód. Situação." }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "update dealwish.desejos set id_situacao = :id_situacao where id = :id ";

                    dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                    dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);

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