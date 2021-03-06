using Dapper;
using dw_webservice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace dw_webservice.Repositories
{
    public class OfertasRepository 
    {
        IConfiguration configuration;
        public OfertasRepository(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return configuration;
        }

        public object Consultar_oferta(int id = 0, int id_fatura = 0, int id_desejo = 0, int id_empresa = 0, int id_situacao = 0, string validade = "", double valor = 0, string url = "", string descricao = "", string origem = "",
                                       string data_ini = "", string data_fim = "", string paginacao = "N", int max_id = 0, int pagina = 1)
        {
            object result = null;
            object result_max_count_id = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {

                    if ((id == 0 && id_desejo == 0 && id_empresa == 0 && id_situacao == 0 && string.IsNullOrWhiteSpace(validade) && valor == 0 && string.IsNullOrWhiteSpace(url) && string.IsNullOrWhiteSpace(descricao))
                        || string.IsNullOrWhiteSpace(origem) || (origem == Constantes.FRONTOFFICE && id_empresa == 0)
                        || (!string.IsNullOrWhiteSpace(data_ini) && string.IsNullOrWhiteSpace(data_fim))
                        || (string.IsNullOrWhiteSpace(data_ini) && !string.IsNullOrWhiteSpace(data_fim)))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_CONSULTA }));
                    }

                    DateTime v_validade = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(validade))
                    {
                        try
                        {
                            v_validade = DateTime.Parse(validade);
                        }
                        catch (Exception)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Data de validade inválida." }));
                        }
                    }

                    DateTime v_data_ini = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(data_ini))
                    {
                        try
                        {
                            v_data_ini = DateTime.Parse(data_ini);
                        }
                        catch (Exception)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Data inicial inválida." }));
                        }
                    }

                    DateTime v_data_fim = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(data_fim))
                    {
                        try
                        {
                            v_data_fim = DateTime.Parse(data_fim);
                        }
                        catch (Exception)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Data final inválida." }));
                        }
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query_max_count_id = "select max(o.id) max_id, count(o.id) count_id ";

                        var query_campos = "select o.id, o.id_desejo, " +
                            "case :id_empresa  when o.id_empresa then  o.id_empresa  else case :visualizacao  when 'P' then  0  else o.id_empresa end end id_empresa, " +
                            "case :id_empresa when o.id_empresa then  e.fantasia  else case :visualizacao when 'P' then  '-'  else e.fantasia end end fantasia, " +
                            "case :id_empresa  when o.id_empresa then  q.descricao  else case :visualizacao  when 'P' then  '-'  else q.descricao end end desc_qualificacao, " +
                            "e.logo, o.validade, o.valor, " +
                            "case :id_empresa  when o.id_empresa then  o.url when 0 then  o.url else '-' end url, " +
                            "o.descricao, o.id_situacao, s.descricao desc_situacao, o.lida, o.like_unlike, o.destaque, " +
                            "case :id_empresa  when o.id_empresa then  u.nome  else '-' end usuario_inclusao, o.data ";

                        var query = "from dealwish.ofertas o, dealwish.empresas e, dealwish.situacoes s, dealwish.usuarios u, dealwish.qualificacoes q " +
                                    "where o.id_empresa = e.id and o.id_situacao = s.id and o.id_usuario = u.id and e.id_qualificacao = q.id and ";

                        var query_pagina = " and (o.id <= :max_id or :max_id = 0) order by o.validade, o.id offset :inicio rows fetch next 20 rows only ";
                        dyParam.Add("max_id", max_id, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("inicio", (pagina - 1) * 20, DbType.Int32, ParameterDirection.Input);

                        var tem_param = false;

                        if (origem == Constantes.APLICATIVO) //Somente ofertas dentro da validade e ativas
                        {
                            query = query + " o.validade >= current_date and o.id_situacao = :situacao and ( ";
                            dyParam.Add("situacao", Constantes.ATIVO, DbType.Int32, ParameterDirection.Input);
                        }
                        else
                        {
                            query = query + " ( ";
                        }

                        if (id != 0)
                        {
                            query = query + " o.id = :id";
                            dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (id_fatura != 0)
                        {
                            query = query + (tem_param ? " and " : "") + " o.id_fatura = :id_fatura";
                            dyParam.Add("id_fatura", id_fatura, DbType.Int32, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (id_desejo != 0)
                        {
                            query = query + (tem_param ? " and " : "") + " o.id_desejo = :id_desejo";
                            dyParam.Add("id_desejo", id_desejo, DbType.Int64, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (id_empresa != 0 && id_desejo == 0)
                        {
                            query = query + (tem_param ? " and " : "") + " o.id_empresa = :id_empresa";
                            tem_param = true;
                        }
                        // Parâmetro id_empresa é sempre utilizado, mesmo quando zerado. 
                        dyParam.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);

                        if (!string.IsNullOrWhiteSpace(validade))
                        {
                            query = query + (tem_param ? " and " : "") + " o.validade = :validade";
                            dyParam.Add("validade", v_validade, DbType.Date, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(data_ini))
                        {
                            query = query + (tem_param ? " and " : "") + " o.data between :data_ini and :data_fim";
                            dyParam.Add("data_ini", v_data_ini, DbType.Date, ParameterDirection.Input);
                            dyParam.Add("data_fim", v_data_fim, DbType.Date, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            query = query + (tem_param ? " and " : "") + " o.url = upper(:url)";
                            dyParam.Add("url", url, DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(descricao))
                        {
                            query = query + (tem_param ? " and " : "") + " upper(o.descricao) like upper(:descricao)";
                            dyParam.Add("descricao", '%' + descricao + '%', DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (id_situacao != 0)
                        {
                            query = query + (tem_param ? " and " : "") + " o.id_situacao = :id_situacao";
                            dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (origem == Constantes.FRONTOFFICE && id_desejo != 0) //Verifica se a empresa pode visualizar ofertas de outras empresas
                        {
                            var dyParam_visu = new DynamicParameters();
                            var query_visu = "select visualizacao from dealwish.contratos c, dealwish.planos p where c.id_plano = p.id and c.id_situacao = :situacao and c.id_empresa = :id_empresa";
                            dyParam_visu.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);
                            dyParam_visu.Add("situacao", Constantes.ATIVO, DbType.Int32, ParameterDirection.Input);

                            var result_visu = SqlMapper.QuerySingle(conn, query_visu, param: dyParam_visu, commandType: CommandType.Text);
                            if (result_visu.visualizacao == Constantes.VISUALIZACAO_PRECO || result_visu.visualizacao == Constantes.VISUALIZACAO_EMPRESA_PRECO)
                            {
                                dyParam.Add("visualizacao", result_visu.visualizacao, DbType.String, ParameterDirection.Input);

                                query = query + (tem_param ? " and " : "") + " (o.id_empresa in " +
                                        "(select c.id_empresa from dealwish.contratos c, dealwish.planos p where c.id_plano = p.id and c.id_situacao = :situacao_contrato and p.visualizacao != 'N') " +
                                        "or o.id_empresa = :id_dealwish)";
                                dyParam.Add("situacao_contrato", Constantes.ATIVO, DbType.Int32, ParameterDirection.Input);
                                dyParam.Add("id_dealwish", Constantes.ID_DEALWISH, DbType.Int32, ParameterDirection.Input);
                                tem_param = true;
                            }
                            else
                            {
                                query = query + (tem_param ? " and " : "") + " o.id_empresa = :id_empresa";
                                dyParam.Add("visualizacao", null, DbType.String, ParameterDirection.Input);
                                tem_param = true;
                            }
                        }
                        else
                        {
                            dyParam.Add("visualizacao", null, DbType.String, ParameterDirection.Input);
                        }

                        query = query + " ) ";

                        if (paginacao == "S")
                        {
                            if (pagina == 1)
                            {
                                result_max_count_id = SqlMapper.QuerySingle<Paginacao>(conn, query_max_count_id + query, param: dyParam, commandType: CommandType.Text);
                            }
                            query = query + query_pagina;
                        }

                        if (origem == Constantes.APLICATIVO) //Ordenar por destaque e preço
                        {
                            query = query + " order by o.destaque desc, o.valor asc ";
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

        public async System.Threading.Tasks.Task<NovoRegistro> Incluir_oferta(int id_desejo = 0, int id_empresa = 0, string validade = "", double valor = 0, string url = "", string descricao = "", string destaque = "", int id_usuario = 0)
        {
            long _id = 0;
            string _mensagem = "";
            NovoRegistro _novo_registro = new NovoRegistro();

            validade = Utils.formatarDataZero(validade);

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (id_desejo == 0 || id_empresa == 0 || string.IsNullOrWhiteSpace(validade) || valor == 0 || string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(descricao) || string.IsNullOrWhiteSpace(destaque))
                    {
                        _novo_registro.Conteudo.Id = 0;
                        _novo_registro.Resultado.Erro = true;
                        _novo_registro.Resultado.Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Cód. Desejo, Cód. Empresa, Validade, Valor, URL, Descrição e Destaque.";
                        return _novo_registro;
                    }


                    DateTime v_validade = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(validade))
                    {
                        try
                        {
                            v_validade = DateTime.Parse(validade);

                            if (v_validade <= DateTime.Today)
                            {
                                _novo_registro.Conteudo.Id = 0;
                                _novo_registro.Resultado.Erro = true;
                                _novo_registro.Resultado.Mensagem = "Data de validade deve ser maior que 1 dia.";
                                return _novo_registro;
                            }
                        }
                        catch (Exception)
                        {
                            _novo_registro.Conteudo.Id = 0;
                            _novo_registro.Resultado.Erro = true;
                            _novo_registro.Resultado.Mensagem = "Data de validade inválida.";
                            return _novo_registro;
                        }
                    }

                    bool url_valida = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (!url_valida)
                    {
                        _novo_registro.Conteudo.Id = 0;
                        _novo_registro.Resultado.Erro = true;
                        _novo_registro.Resultado.Mensagem = "URL inválida.";
                        return _novo_registro;
                    }

                    if (destaque != "S" && destaque != "N")
                    {
                        _novo_registro.Conteudo.Id = 0;
                        _novo_registro.Resultado.Erro = true;
                        _novo_registro.Resultado.Mensagem = "Destaque deve ser S ou N.";
                        return _novo_registro;
                    }

                    var dyParam = new DynamicParameters();

                    if (id_empresa == Constantes.ID_DEALWISH)
                    {
                        descricao = Utils.Consultar_valor_config("texto_padrao_oferta_dealwish") + descricao;
                    }

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "insert into dealwish.ofertas (id_desejo, id_empresa, validade, valor, url, descricao, id_usuario, destaque, id_usuario_log) " +
                                    "values (:id_desejo, :id_empresa, :validade, :valor, :url, :descricao, :id_usuario, :destaque, :id_usuario) returning id";

                        dyParam.Add("id", null, DbType.Int64, ParameterDirection.Output);
                        dyParam.Add("id_desejo", id_desejo, DbType.Int64, ParameterDirection.Input);
                        dyParam.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("validade", v_validade, DbType.Date, ParameterDirection.Input);
                        dyParam.Add("valor", valor,DbType.VarNumeric, ParameterDirection.Input);
                        dyParam.Add("url", url, DbType.String, ParameterDirection.Input);
                        dyParam.Add("descricao", descricao, DbType.String, ParameterDirection.Input);
                        dyParam.Add("destaque", destaque, DbType.String, ParameterDirection.Input);
                        dyParam.Add("id_usuario", id_usuario, DbType.Int64, ParameterDirection.Input);

                        SqlMapper.Execute(conn, query, dyParam, null, null, CommandType.Text);
                        _id = dyParam.Get<long>("id");

                        dyParam = new DynamicParameters();
                        query = "select t.token_app from dealwish.usuarios u, dealwish.tokens t, dealwish.desejos d where u.id = t.id_usuario and " +
                                "d.id = :id_desejo and d.id_usuario = u.id";
                        dyParam.Add("id_desejo", id_desejo, DbType.Int64, ParameterDirection.Input);
                        var result_token_app = SqlMapper.QuerySingle(conn, query, param: dyParam, commandType: CommandType.Text);

                        ClienteHttp httpClient = ClienteHttp.Instance;
                        string _url_fcm = configuration.GetSection("Firebase").GetSection("url_fcm").Value;
                        string _server_key = configuration.GetSection("Firebase").GetSection("server_key").Value;
                        var request = new HttpRequestMessage(HttpMethod.Post, _url_fcm);
                        request.Headers.TryAddWithoutValidation("Authorization", "key=" + _server_key);

                        string title = "Você tem uma nova oferta.";
                        string body = "Toque para visualizar.";
                        string to = result_token_app.token_app;
                        var _conteudo = new
                        {
                            to,
                            notification = new { title, body }
                        };
                        string _oferta = JsonSerializer.Serialize(_conteudo).ToString();

                        request.Content = new StringContent(_oferta, Encoding.UTF8, "application/json");

                        HttpResponseMessage response;
                        using (var client = new HttpClient())
                        {
                            try
                            {
                                response = await client.SendAsync(request);
                            }
                            catch
                            {
                                _mensagem = "Não foi possível enviar a notificação.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    _novo_registro.Conteudo.Id = 0;
                    _novo_registro.Resultado.Erro = true;
                    _novo_registro.Resultado.Mensagem = ex.Message;
                    return _novo_registro;
                }

                _novo_registro.Conteudo.Id = _id;
                _novo_registro.Resultado.Erro = false;
                _novo_registro.Resultado.Mensagem = _mensagem;
                return _novo_registro;
            }
        }

        public object Excluir_oferta(int id = 0, int id_usuario_login = 0)
        {
            object result = null;
            IDbTransaction transaction = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (id == 0)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código" }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        transaction = conn.BeginTransaction();

                        var query = "update dealwish.ofertas set id_usuario_log = :id_usuario_login where id = :id ";
                        dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                        dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                        var linhasafetadas = SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        query = "delete from dealwish.ofertas where id = :id ";
                        linhasafetadas = linhasafetadas + SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

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

        public object Atualizar_situacao_oferta(int id = 0, int id_situacao = 0, int id_empresa = 0, int id_usuario_login = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (id == 0 || id_situacao == 0 || id_empresa == 0)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código, Cód. Situação e Cód. Empresa." }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "update dealwish.ofertas set id_situacao = :id_situacao, id_usuario_log = :id_usuario_login where id = :id and id_empresa = :id_empresa ";

                        dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                        dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);
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

        public object Atualizar_oferta(int id = 0, int id_desejo = 0, int id_empresa = 0, string validade = "", double valor = 0, string url = "", string descricao = "", int id_usuario_login = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (id_desejo == 0 || id_empresa == 0 || string.IsNullOrWhiteSpace(validade) || valor == 0 || string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(descricao))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Cód. Desejo, Cód. Empresa, Validade, Valor, URL ou Descrição." }));
                    }

                    DateTime v_validade = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(validade))
                    {
                        try
                        {
                            v_validade = DateTime.Parse(validade);
                        }
                        catch (Exception)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Data de validade inválida." }));
                        }
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "update dealwish.ofertas set id_desejo = :id_desejo, id_empresa  = :id_empresa, " +
                                    "validade =:validade, valor =:valor, url = :url, descricao = :descricao, id_usuario_log = :id_usuario_login where id  = :id ";

                        dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                        dyParam.Add("id_desejo", id_desejo, DbType.Int64, ParameterDirection.Input);
                        dyParam.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("validade", v_validade, DbType.Date, ParameterDirection.Input);
                        dyParam.Add("valor", valor,DbType.VarNumeric, ParameterDirection.Input);
                        dyParam.Add("url", url, DbType.String, ParameterDirection.Input);
                        dyParam.Add("descricao", descricao, DbType.String, ParameterDirection.Input);
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

        private bool Verifica_oferta_cadastrada(int id_desejo = 0, int id_empresa = 0, string descricao = "")
        {
            bool existe_oferta = false;

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
                        var query = "select count(o.id) qtd from dealwish.ofertas o " +
                                    "where o.id_desejo = :id_desejo and o.id_empresa = :id_empresa and o.descricao = :descricao and o.id_situacao = :situacao";
                        dyParam.Add("id_desejo", id_desejo, DbType.Int64, ParameterDirection.Input);
                        dyParam.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("descricao", descricao, DbType.String, ParameterDirection.Input);
                        dyParam.Add("situacao", Constantes.ATIVO, DbType.Int32, ParameterDirection.Input);
                        var num_ofertas = SqlMapper.QuerySingle(conn, query, param: dyParam, commandType: CommandType.Text);
                        existe_oferta = num_ofertas.qtd > 0;
                    }
                }
                catch
                {
                    return false;
                }

                return existe_oferta;
            }
        }

        private async System.Threading.Tasks.Task<string> Incluir_oferta_linhaAsync(Oferta oferta)
        {
            NovoRegistro retorno;
           
            string mensagem = "";
            try
            {
                if (!Verifica_oferta_cadastrada(oferta.Id_desejo, oferta.Id_empresa, oferta.Descricao))
                {
                    retorno = await Incluir_oferta(oferta.Id_desejo, oferta.Id_empresa, oferta.Validade, oferta.Valor, oferta.Url, oferta.Descricao, oferta.Destaque, oferta.Id_usuario);

                    if (retorno.Conteudo.Id != 0)
                    {
                        mensagem = "Incluída";
                        if (retorno.Resultado.Mensagem != "")
                        {
                            mensagem = mensagem + ": " + retorno.Resultado.Mensagem;
                        }
                    }
                    else
                    {
                        mensagem = "Não incluída: " + retorno.Resultado.Mensagem;
                    }
                }

                else
                {
                    mensagem = "Não Incluída: Oferta já existe";

                }
            }
            catch (Exception ex)
            {
                return oferta.Id_desejo.ToString() + ";Erro crítico: " + ex.Message;
            }

            return oferta.Id_desejo.ToString() + ";" + mensagem;
        }

        public async System.Threading.Tasks.Task<object> Incluir_oferta_loteAsync(IFormFile file = null, int id_empresa = 0, int id_usuario_login = 0)
        {
            int linha = 0;

            try
            {
                var reader = new StreamReader(file.OpenReadStream());
                List<Oferta> ofertas = new List<Oferta>();
                while (!reader.EndOfStream)
                {
                    linha++;
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if (values.Length != 6)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Linha " + linha.ToString() + " não contém 6 colunas." }));
                    }

                    if (linha > 1)
                    {
                        Oferta oferta = new Oferta(values)
                        {
                            Id_empresa = id_empresa,
                            Id_usuario = id_usuario_login
                        };
                        ofertas.Add(oferta);
                    }
                }

                List<string> retorno = new List<string>();
                retorno.Add("ID_DESEJO;MENSAGEM");
                foreach (Oferta oferta in ofertas)
                {
                    retorno.Add(await Incluir_oferta_linhaAsync(oferta));
                }
                Utils.Listtofile(retorno, id_usuario_login.ToString() + Constantes.LOTEOFERTAS_CSV);
            }
            catch (Exception ex)
            {
                return (new { linhasafetadas = 0 }, new { Erro = false, Mensagem = ex.Message});
            }

            return (new { linhasafetadas = 1 }, new { Erro = false, Mensagem = ""});
        }

        public object Atualizar_lida_oferta(int id = 0, int id_usuario_login = 0)
        {
            object result = null;

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
                        var query = "update dealwish.ofertas set lida = 'S', id_usuario_log = :id_usuario_login where id = :id ";

                        dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
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

        public object Atualizar_like_unlike_oferta(int id = 0, string like_unlike = "", int id_usuario_login = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (id == 0 || string.IsNullOrWhiteSpace(like_unlike))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código e Like/Unlike." }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "update dealwish.ofertas set like_unlike = :like_unlike, id_usuario_log = :id_usuario_login where id = :id ";

                        dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                        dyParam.Add("like_unlike", like_unlike, DbType.String, ParameterDirection.Input);
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