using Dapper;
using dw_webservice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace dw_webservice.Repositories
{
    public class FaturasRepository 
    {
        readonly IConfiguration _configuration;
        public FaturasRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return _configuration;
        }

        string query_faturas_abertas_cabecalho =
               "with valores_contrato as (select c.id_empresa, p.qtd_ofertas, p.valor_mensal, p.valor_oferta, c.dia_vct from dealwish.contratos c, dealwish.planos p where c.id_situacao = 1 and c.id_plano = p.id) " +
               "select <id>" +
               "extract(month from o.data) mes, " +
               "extract(year from o.data) ano, " +
               "o.id_empresa,  " +
               "'-' nosso_numero, " +
               "(vc.valor_mensal + (case when count(o.id) - vc.qtd_ofertas > 0 then(count(o.id) - vc.qtd_ofertas) * vc.valor_oferta else 0 end)) valor, " +
               "to_date(lpad(cast(vc.dia_vct as varchar),2,'0')||'/'||extract(month from o.data)||'/'||extract(year from o.data),'DD/MM/YYYY') + interval '1 month' data_vct, " +
               "null data_pg, " +
               "null multa, " +
               "null juros, " +
               "0 valor_pg, " +
               "count(o.id) qtd_ofertas, " +
               "s.id id_situacao," +
               "null num_nfe," +
               "null serie_nfe, " +
               ":id_usuario_login id_usuario_login ";

        readonly string query_faturas_abertas_campos_adic =
               ", e.razao_social, s.descricao desc_situacao, " +
               "null endereco, null numero, null complemento, null bairro, null cep, null cidade, null uf, null email_com, " +
               "null endereco_cob, null numero_cob, null cep_cob, null cidade_cob, null cnpj, null pix ";

        readonly string query_faturas_abertas_corpo =
               "from (select * from dealwish.ofertas where destaque = 'S' union all select * from dealwish.ofertas) o, " +
               "dealwish.empresas e, dealwish.situacoes s, valores_contrato vc " +
               "where s.id = 8 and o.id_empresa = e.id and (:id_empresa = 0 or e.id = :id_empresa) " +
               "and vc.id_empresa = e.id and o.id_fatura is null " +
               "and o.data between (select cast(date_trunc('month', min(of_dt.data)) as date) from dealwish.ofertas of_dt where of_dt.id_fatura is null and of_dt.id_empresa = o.id_empresa) and " +
               "(select cast(date_trunc('month', min(of_dt.data)) + interval '1 month - 1 day' as date) from dealwish.ofertas of_dt where of_dt.id_fatura is null and of_dt.id_empresa = o.id_empresa) " + 
               "group by o.id_empresa, e.razao_social, s.id, s.descricao, vc.valor_mensal, vc.qtd_ofertas, vc.valor_oferta, " +
               "extract(month from o.data), extract(year from o.data),vc.dia_vct";

        public object ConsultarFatura(out bool erro_exportar, long id_usuario_login = 0, int id = 0, int id_empresa = 0, string razao_social = "", int mes = 0, int ano = 0, string nosso_numero = "", int id_situacao = 0, string abertas = "", string exportar = "N")
        {
            IEnumerable<dynamic> result = null;
            erro_exportar = false;
            Utils utils = new();

            UsuariosRepository usuariosRepository = new(_configuration);

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {

                if ((id == 0 && id_empresa == 0 && string.IsNullOrWhiteSpace(razao_social) && (mes == 0 || ano == 0) && string.IsNullOrWhiteSpace(nosso_numero) && id_situacao == 0) && string.IsNullOrWhiteSpace(abertas))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_CONSULTA }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var lista_permissoes = usuariosRepository.ConsultarListaPermUsuarioAsync(id_usuario_login);
                if (!lista_permissoes.Result.Contains("bkf") && !lista_permissoes.Result.Contains("tin") && (exportar == Constantes.NOTA_FISCAL || exportar == Constantes.BOLETO || exportar == Constantes.PIX))
                {
                    return utils.ArquivoErro(id_usuario_login, "sem_permissao", "Sem permissão para gerar cobrança ou nota fiscal.");
                }

                string query_where = "";
                string query_from = "";

                if (conn.State == ConnectionState.Open)
                {
                    string query = "";


                    if (abertas == "S")
                    {
                        query_faturas_abertas_cabecalho = query_faturas_abertas_cabecalho.Replace("<id>", "null id,");
                        query = query_faturas_abertas_cabecalho + query_faturas_abertas_campos_adic + query_faturas_abertas_corpo + " union all ";
                    }

                    query = query + "select f.id, f.mes, f.ano, f.id_empresa, f.nosso_numero, f.valor, f.data_vct, " +
                                    "f.data_pg, f.multa, f.juros, f.valor_pg, f.qtd_ofertas, f.id_situacao, f.num_nfe, f.serie_nfe, :id_usuario_login id_usuario_login, " +
                                    "e.razao_social, s.descricao desc_situacao, " +
                                    "e.endereco, e.numero, e.complemento, e.bairro, e.cep, c.nome cidade, c.uf, e.email_com, " +
                                    "e.endereco_cob, e.numero_cob, e.cep_cob, ccob.nome || '/' || ccob.uf cidade_cob, e.cnpj, f.pix ";

                    query_from = "from dealwish.faturas f, dealwish.empresas e, dealwish.situacoes s, dealwish.cidades ccob, dealwish.cidades c " +
                                    "where f.id_empresa = e.id and f.id_situacao = s.id and e.id_cidade_cob = ccob.id and e.id_cidade = c.id and ( ";

                    query += query_from;

                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                    var tem_param = false;

                    if (id != 0)
                    {
                        query_where += " f.id = :id";
                        dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (id_empresa != 0)
                    {
                        query_where = query_where + (tem_param ? " and " : "") + " f.id_empresa = :id_empresa";
                        dyParam.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(razao_social))
                    {
                        query_where = query_where + (tem_param ? " and " : "") + " upper(e.razao_social) like upper(:razao_social)";
                        dyParam.Add("razao_social", "%" + razao_social + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(nosso_numero))
                    {
                        query_where = query_where + (tem_param ? " and " : "") + " f.nosso_numero = :nosso_numero";
                        dyParam.Add("nosso_numero", nosso_numero, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (mes != 0 && ano != 0)
                    {
                        query_where = query_where + (tem_param ? " and " : "") + " (f.mes = :mes and f.ano = :ano) ";
                        dyParam.Add("mes", mes, DbType.Int16, ParameterDirection.Input);
                        dyParam.Add("ano", ano, DbType.Int16, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (id_situacao != 0)
                    {
                        query_where = query_where + (tem_param ? " and " : "") + " f.id_situacao = :id_situacao";
                        dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (exportar == Constantes.NOTA_FISCAL)
                    {
                        query_where = query_where + (tem_param ? " and " : "") + " f.id_situacao = :id_situacao";
                        dyParam.Add("id_situacao", Constantes.GERAR_REMESSA_NF, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (exportar == Constantes.BOLETO)
                    {
                        query_where = query_where + (tem_param ? " and " : "") + " f.id_situacao = :id_situacao";
                        dyParam.Add("id_situacao", Constantes.GERAR_COBRANCA, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    query = query + query_where + " ) ";

                    if (exportar != Constantes.NAO)
                    {
                        result = SqlMapper.Query(conn, query, param: dyParam, commandType: CommandType.Text);
                    }
                    else
                    {
                        result = SqlMapper.Query<Fatura>(conn, query, param: dyParam, commandType: CommandType.Text);
                    }

                }

                if (!(exportar == Constantes.NAO))
                {

                    if (exportar == Constantes.CSV)
                    {
                        return utils.DapperQueryToCsvFile(result, id_usuario_login, "faturas");
                    }

                    string query_atualiza = "update dealwish.faturas fa set id_situacao = :situacao_para " +
                                            "where fa.id_situacao = :situacao_de and " +
                                            "fa.id in (select f.id " + query_from + query_where + "))";

                    if (exportar == Constantes.NOTA_FISCAL)
                    {
                        // Atualiza situacao GERAR_REMESSA_NF para GERAR_REMESSA_BOLETO
                        dyParam.Add("situacao_de", Constantes.GERAR_REMESSA_NF, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("situacao_para", Constantes.GERAR_COBRANCA, DbType.Int32, ParameterDirection.Input);
                        var linhasafetadas = SqlMapper.Execute(conn, query_atualiza, dyParam, null, null, CommandType.Text);

                        if (linhasafetadas > 0)
                        {
                            var retorno = utils.DapperQueryToRemessaNFAsync(result, _configuration);
                            return retorno.Result;
                        }
                        else
                        {
                            return utils.ArquivoErro(id_usuario_login, "erro_gerar_remessa_nf", "Nenhuma Nota Fiscal a ser gerada.");
                        }
                    }

                    if (exportar == Constantes.BOLETO || exportar == Constantes.PIX)
                    {
                        // Atualiza situacao GERAR_COBRANCA para A_LIQUIDAR
                        dyParam.Add("situacao_de", Constantes.GERAR_COBRANCA, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("situacao_para", Constantes.A_LIQUIDAR, DbType.Int32, ParameterDirection.Input);

                        ConfigRepository configRepository = new(_configuration); 

                        var linhasafetadas = SqlMapper.Execute(conn, query_atualiza, dyParam, null, null, CommandType.Text);

                        if (exportar == Constantes.BOLETO)
                        {
                            if (linhasafetadas > 0)
                            {
                                var retorno = utils.DapperQueryToRemessaBoletoAsync(result, _configuration);
                                return retorno.Result;
                            }
                            else
                            {
                                return utils.ArquivoErro(id_usuario_login, "erro_gerar_remessa_boleto", "Nenhum Boleto a ser gerado.");
                            }
                        }
                        else //PIX
                        {
                            if (linhasafetadas > 0)
                            {
                                string query_pix = "update dealwish.faturas set pix = :pix where id = :id";

                                string pix, pix_base, valor, identificador;

                                var pix_chave_conta = configRepository.ConsultarValorConfigAsync("pix_chave_conta");
                                var pix_nome = configRepository.ConsultarValorConfigAsync("pix_nome");
                                var pix_cidade = configRepository.ConsultarValorConfigAsync("pix_cidade");

                                pix_base = "0002010102122644" +
                                      pix_chave_conta.Result +
                                      "520400005303986" +
                                      "5413<valor>" +
                                      "5802BR" +
                                      "5900" + pix_nome.Result +
                                      "6000" + pix_cidade.Result +
                                      "6207<identificador>" +
                                      "6304";

                                foreach (IDictionary<string, object> fatura in result)
                                {
                                    pix = pix_base;

                                    valor = double.Parse(fatura["valor"].ToString()).ToString("#.00").Replace(",", ".").PadLeft(13, '0');
                                    pix = pix.Replace("<valor>", valor);

                                    identificador = fatura["id"].ToString().PadLeft(7, '0');
                                    pix = pix.Replace("<identificador>", identificador);

                                    pix += Utils.CRC16.ComputeChecksum(Encoding.ASCII.GetBytes(pix));

                                    var dyParam_pix = new DynamicParameters();
                                    dyParam_pix.Add("id", fatura["id"], DbType.Int32, ParameterDirection.Input);
                                    dyParam_pix.Add("pix", pix, DbType.String, ParameterDirection.Input);

                                    SqlMapper.Execute(conn, query_pix, dyParam_pix, null, null, CommandType.Text);

                                }

                                return (utils.FormataRetorno("", new { Erro = false, Mensagem = "Cobranças PIX geradas." })); ;
                            }
                            else
                            {
                                return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Nenhuma cobrança PIX foi gerada." })); ;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                if (exportar == Constantes.NAO || exportar == Constantes.PIX)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }
                else
                {
                    return utils.ArquivoErro(id_usuario_login, "erro", ex.Message);
                }
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> ConsultarFaturasAbertasAsync(long id_usuario_login)
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
                    dyParam.Add("id_empresa", 0, DbType.Int32, ParameterDirection.Input); //Todas as empresas
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);
                    query_faturas_abertas_cabecalho = query_faturas_abertas_cabecalho.Replace("<id>", "");
                    var query = query_faturas_abertas_cabecalho + query_faturas_abertas_campos_adic + query_faturas_abertas_corpo;

                    result = await SqlMapper.QueryAsync<Fatura>(conn, query, param: dyParam, commandType: CommandType.Text);

                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> IncluirFaturaAsync(int id_empresa = 0, int mes = 0, int ano = 0, string nosso_numero = "", double valor = 0, string data_vct = "", int qtd_ofertas = -1, int id_situacao = 0, long id_usuario_login = 0)
        {
            object result = null;
            Utils utils = new();

            data_vct = utils.FormatarDataZero(data_vct);

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                if (id_empresa == 0 || mes == 0 || ano == 0 || string.IsNullOrWhiteSpace(nosso_numero) || valor == 0 || string.IsNullOrWhiteSpace(data_vct) || qtd_ofertas == -1 || id_situacao == 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Cód. Empresa, Mês, ANo, Nosso Número, Valor, Data Vct., Qtd. Ofertas e Cód. Situação." }));
                }

                data_vct = utils.FormatarDataZero(data_vct);

                DateTime v_data_vct = DateTime.Now;
                try
                {
                    v_data_vct = DateTime.Parse(data_vct);
                }
                catch (Exception)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Data de vencimento inválida." }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "insert into dealwish.faturas (id_empresa, mes, ano, nosso_numero, valor , data_vct, qtd_ofertas, id_situacao, id_usuario_log) " +
                                "values (:id_empresa, :mes, :ano, :nosso_numero, :valor , :data_vct, :qtd_ofertas, :id_situacao, :id_usuario_login) returning id";

                    dyParam.Add("id", null, DbType.Int32, ParameterDirection.Output);
                    dyParam.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("mes", mes, DbType.Int16, ParameterDirection.Input);
                    dyParam.Add("ano", ano, DbType.Int16, ParameterDirection.Input);
                    dyParam.Add("nosso_numero", nosso_numero, DbType.String, ParameterDirection.Input);
                    dyParam.Add("valor", valor, DbType.VarNumeric, ParameterDirection.Input);
                    dyParam.Add("data_vct", v_data_vct, DbType.Date, ParameterDirection.Input);
                    dyParam.Add("qtd_ofertas", qtd_ofertas, DbType.Int16, ParameterDirection.Input);
                    dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
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

        public async Task<object> ExcluirFaturaAsync(int id = 0, long id_usuario_login = 0)
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

                    var query = "select id_situacao from dealwish.faturas where id = :id ";
                    dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                    var fatura = await SqlMapper.QuerySingleAsync(conn, query, param: dyParam, commandType: CommandType.Text);

                    if (fatura.id_situacao == Constantes.LIQUIDADA ||
                        fatura.id_situacao == Constantes.A_LIQUIDAR)
                    {
                        return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Uma fatura LIQUIDADA ou A LIQUIDAR não pode ser excluída." }));
                    }

                    query = "update dealwish.ofertas set id_fatura = null, id_usuario_log = :id_usuario_login where id_fatura = :id ";
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);
                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

                    query = "update dealwish.faturas set id_usuario_log = :id_usuario_login where id = :id ";
                    linhasafetadas += await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

                    query = "delete from dealwish.faturas where id = :id ";
                    linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

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

        public async Task<object> EfetivarFaturasAbertasAsync(long id_usuario_login)
        {
        
            object result = null;
            IDbTransaction transaction = null;
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
                    transaction = conn.BeginTransaction();

                    // Somente pode efetivar fatura se o mês corrente é maior que o menor mês das ofertas não faturadas
                    var query = "select case when (date_trunc('month', min(data)) + interval '1 month' - interval '1 day') >= current_date then 0 else 1 end num from dealwish.ofertas where id_fatura is null";
                    var meses = await SqlMapper.QuerySingleAsync(conn, query, param: dyParam, commandType: CommandType.Text);
                    bool nao_pode_gerar = meses.num == 0;

                    if (nao_pode_gerar)
                    {
                        return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Somente é possível efetivar as faturas com mês fechado." }));
                    }

                    query = "insert into dealwish.faturas (mes,ano,id_empresa,nosso_numero,valor,data_vct,data_pg,multa,juros,valor_pg,qtd_ofertas,id_situacao,num_nfe,serie_nfe,id_usuario_log) " +
                             query_faturas_abertas_cabecalho +
                             query_faturas_abertas_corpo;

                    query = query.Replace("<id>", "");
                    dyParam.Add("id_empresa", 0, DbType.Int32, ParameterDirection.Input); //Todas as empresas
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

                    if (linhasafetadas > 0)
                    {
                        query = "update dealwish.ofertas o set id_fatura = (select f.id from dealwish.faturas f where f.id_empresa = o.id_empresa and f.id_situacao = :situacao), id_usuario_log = :id_usuario_login " +
                                "where o.id_fatura is null " +
                                "and o.id_empresa = (select f.id_empresa from dealwish.faturas f where f.id_empresa = o.id_empresa and f.id_situacao = :situacao) " +
                                "and o.data <= (select (date_trunc('month',to_date('01/' || f.mes || '/' || f.ano, 'DD/MM/YYYY')) + interval '1 month' - interval '1 day') as date from dealwish.faturas f where f.id_empresa = o.id_empresa and f.id_situacao = :situacao) ";
                        dyParam.Add("situacao", Constantes.ABERTA, DbType.Int32, ParameterDirection.Input);
                        linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

                        if (linhasafetadas > 0)
                        {
                            query = "update dealwish.faturas set id_situacao = :situacao_para, id_usuario_log = :id_usuario_login " +
                                    "where id_situacao = :situacao_de";
                            dyParam.Add("situacao_de", Constantes.ABERTA, DbType.Int32, ParameterDirection.Input);
                            dyParam.Add("situacao_para", Constantes.GERAR_REMESSA_NF, DbType.Int32, ParameterDirection.Input);
                            linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

                            transaction.Commit();

                            result = new { linhasafetadas };
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Não existem faturas a serem efetivadas." }));
                    }

                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> AtualizarFaturaAsync(int id = 0, int mes = 0, int ano = 0, string nosso_numero = "", double valor = 0, string data_vct = "", string data_pg = "", double multa = 0, double juros = 0, double valor_pg = 0, int qtd_ofertas = -1, int id_situacao = 0, long id_usuario_login = 0)
        {
            object result = null;
            Utils utils = new();

            data_vct = utils.FormatarDataZero(data_vct);
            data_pg = utils.FormatarDataZero(data_pg);

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                if (id == 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código." }));
                }

                if (mes == 0 && ano == 0 && string.IsNullOrWhiteSpace(nosso_numero) && valor == 0 && string.IsNullOrWhiteSpace(data_vct) &&
                    string.IsNullOrWhiteSpace(data_pg) && multa == 0 && juros == 0 && valor_pg == 0 && qtd_ofertas == -1 && id_situacao == 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_CONSULTA }));
                }

                DateTime v_data_vct = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(data_vct))
                {
                    try
                    {
                        v_data_vct = DateTime.Parse(data_vct);
                    }
                    catch (Exception)
                    {
                        return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Data de vencimento inválida." }));
                    }
                }

                DateTime v_data_pg = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(data_pg))
                {
                    try
                    {
                        v_data_pg = DateTime.Parse(data_pg);
                    }
                    catch (Exception)
                    {
                        return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Data de pagamento inválida." }));
                    }
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var tem_param = false;
                    var campos = "";

                    if (mes != 0)
                    {
                        campos += "mes = :mes";
                        dyParam.Add("mes", mes, DbType.Int16, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (ano != 0)
                    {
                        campos = campos + (tem_param ? " , " : "") + "ano = :ano";
                        dyParam.Add("ano", ano, DbType.Int16, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(nosso_numero))
                    {
                        campos = campos + (tem_param ? " , " : "") + "nosso_numero = :nosso_numero";
                        dyParam.Add("nosso_numero", nosso_numero, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (valor != 0)
                    {
                        campos = campos + (tem_param ? " , " : "") + "valor = :valor";
                        dyParam.Add("valor", valor, DbType.VarNumeric, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(data_vct))
                    {
                        campos = campos + (tem_param ? " , " : "") + "data_vct = :data_vct";
                        dyParam.Add("data_vct", v_data_vct, DbType.Date, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(data_pg))
                    {
                        campos = campos + (tem_param ? " , " : "") + "data_pg = :data_pg";
                        dyParam.Add("data_pg", v_data_pg, DbType.Date, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (multa != 0)
                    {
                        campos = campos + (tem_param ? " , " : "") + "multa = :multa";
                        dyParam.Add("multa", multa, DbType.VarNumeric, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (juros != 0)
                    {
                        campos = campos + (tem_param ? " , " : "") + "juros = :juros";
                        dyParam.Add("juros", juros, DbType.VarNumeric, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (valor_pg != 0)
                    {
                        campos = campos + (tem_param ? " , " : "") + "valor_pg = :valor_pg";
                        dyParam.Add("valor_pg", valor_pg, DbType.VarNumeric, ParameterDirection.Input);
                        tem_param = true;
                    }


                    if (qtd_ofertas != -1)
                    {
                        campos = campos + (tem_param ? " , " : "") + "qtd_ofertas = :qtd_ofertas";
                        dyParam.Add("qtd_ofertas", qtd_ofertas, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }


                    if (id_situacao != 0)
                    {
                        campos = campos + (tem_param ? " , " : "") + "id_situacao = :id_situacao";
                        dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    var query = "update dealwish.faturas set " + campos + ", id_usuario_log = :id_usuario_login where id = :id";
                    dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);
                    result = new { linhasafetadas };
                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "Fatura atualizada." }));
        }

        public async Task<object> ProcessarRetornoBoletoAsync(IFormFile file = null, long id_usuario_login = 0)
        {
            int linha = 1;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                var reader = new StreamReader(file.OpenReadStream());

                var conteudo = reader.ReadLine();
                HeaderRetornoBoleto headerretornoboleto = new(conteudo);

                if (!headerretornoboleto.Valido)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Linha " + linha.ToString() + " não contém 400 posições." }));
                }

                List<TransacaoRetornoBoleto> boletos_pagos = new();
                while (!reader.EndOfStream)
                {
                    linha++;
                    conteudo = reader.ReadLine();
                    TransacaoRetornoBoleto transacaoretornoboleto = new(headerretornoboleto.Num_banco, conteudo);
                    if (!transacaoretornoboleto.Valido)
                    {
                        return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Linha " + linha.ToString() + " não contém 400 posições." }));
                    }
                    boletos_pagos.Add(transacaoretornoboleto);
                }

                linha = 0;
                List<string> retorno = new();
                retorno.Add("NOSSO_NUMERO;MENSAGEM");
                foreach (TransacaoRetornoBoleto boleto in boletos_pagos)
                {
                    if (boleto.Id_registro == "1") //Transação
                    {
                        retorno.Add(await BaixarBoletoAsync(boleto, id_usuario_login));
                        linha++;
                    }
                }

                utils.Listtofile(retorno, id_usuario_login.ToString() + Constantes.RETORNO_PROCESSAMENTO_BOLETO_CSV);
            }
            catch (Exception ex)
            {
                return utils.FormataRetorno(new { linhasafetadas = 0 }, new { Erro = false, Mensagem = ex.Message });
            }

            return utils.FormataRetorno(new { linhasafetadas = linha }, new { Erro = false, Mensagem = "" });
        }

        public async Task<object> ProcessarRetornoNFAsync(IFormFile file = null, long id_usuario_login = 0)
        {
            int linha = 1;
            Utils utils = new();

            try
            {
                var reader = new StreamReader(file.OpenReadStream());

                var conteudo = reader.ReadLine();
                HeaderRetornoNF headerretornonf = new(conteudo);

                if (!headerretornonf.Valido)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Linha " + linha.ToString() + " não contém 41 posições." }));
                }

                List<DetalheRetornoNF> notas_fiscais = new();
                while (!reader.EndOfStream)
                {
                    linha++;
                    conteudo = reader.ReadLine();
                    if (conteudo.Substring(0, 1) == "2") //Detalhe NF
                    {
                        DetalheRetornoNF detalheretornonf = new(conteudo);
                        if (!detalheretornonf.Valido)
                        {
                            return (utils.FormataRetorno("", new { Erro = true, Mensagem = "Linha " + linha.ToString() + " não contém 1628 posições." }));
                        }
                        notas_fiscais.Add(detalheretornonf);
                    }
                }

                linha = 0;
                List<string> retorno = new();
                retorno.Add("ID_FATURA;MENSAGEM");
                foreach (DetalheRetornoNF nota_fiscal in notas_fiscais)
                {
                      retorno.Add(await RegistrarNFAsync(nota_fiscal, id_usuario_login));
                      linha++;
                }

                utils.Listtofile(retorno, id_usuario_login.ToString() + Constantes.RETORNO_PROCESSAMENTO_NF_CSV);
            }
            catch (Exception ex)
            {
                return utils.FormataRetorno(new { linhasafetadas = 0 }, new { Erro = false, Mensagem = ex.Message });
            }

            return utils.FormataRetorno(new { linhasafetadas = linha }, new { Erro = false, Mensagem = "" });
        }

        private async Task<string> BaixarBoletoAsync(TransacaoRetornoBoleto boleto, long id_usuario_login = 0)
        {

            string mensagem;
            try
            {
                if (await VerificaFaturaCadastradaAsync(id_titulo_banco: boleto.Id_titulo_banco))
                {
                    mensagem = await BaixarFaturaAsync(boleto, id_usuario_login);
                }
                else
                {
                    mensagem = "Fatura não localizada";
                }
            }
            catch (Exception ex)
            {
                return boleto.Id_titulo_banco + ";Erro crítico: " + ex.Message;
            }

            return boleto.Id_titulo_banco + ";" + mensagem;
        }

        private async Task<string> RegistrarNFAsync(DetalheRetornoNF nf, long id_usuario_login)
        {

            string mensagem;
            try
            {
                if (await VerificaFaturaCadastradaAsync(id: int.Parse(nf.Num_rps)))
                {
                    mensagem = await RegistrarNFFaturaAsync(nf, id_usuario_login);
                }
                else
                {
                    mensagem = "Fatura não localizada";
                }
            }
            catch (Exception ex)
            {
                return nf.Num_rps+ ";Erro crítico: " + ex.Message;
            }

            return nf.Num_rps + ";" + mensagem;
        }

        private static async Task<bool> VerificaFaturaCadastradaAsync(string id_titulo_banco = "", int id = 0)
        {
            bool existe_fatura = false;

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
                    var query = "select count(f.id) qtd from dealwish.faturas f ";

                    if (id != 0)
                    {
                        query += "where f.id = :id";
                        dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                    }
                    else
                    {
                        query += "where f.nosso_numero = :nosso_numero";
                        dyParam.Add("nosso_numero", id_titulo_banco, DbType.String, ParameterDirection.Input);
                    }

                    var num_faturas = await SqlMapper.QuerySingleAsync(conn, query, param: dyParam, commandType: CommandType.Text);
                    existe_fatura = num_faturas.qtd > 0;
                }
            }
            catch
            {
                return false;
            }

            return existe_fatura;
        }

        public async Task<string> BaixarFaturaAsync(TransacaoRetornoBoleto boleto, long id_usuario_login = 0)
        {

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {

                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime v_data_pg;
                if (!string.IsNullOrWhiteSpace(boleto.Data_correncia))
                {
                    try
                    {
                        v_data_pg = DateTime.ParseExact(boleto.Data_correncia, "ddMMyy", provider);
                    }
                    catch (Exception)
                    {
                        return "Data de pagamento inválida.";
                    }
                }
                else
                {
                    return "Data de pagamento não informada.";
                }

                double v_desconto;
                if (!string.IsNullOrWhiteSpace(boleto.Desconto))
                {
                    try
                    {
                        v_desconto = double.Parse(boleto.Desconto) / 100;
                    }
                    catch (Exception)
                    {
                        return "Desconto inválido.";
                    }
                }
                else
                {
                    return "Desconto não informado.";
                }

                double v_multa;
                if (!string.IsNullOrWhiteSpace(boleto.Juros))
                {
                    try
                    {
                        v_multa = double.Parse(boleto.Juros) / 100;
                    }
                    catch (Exception)
                    {
                        return "Juros(Multa) inválido.";
                    }
                }
                else
                {
                    return "Juros(Multa) não informado.";
                }


                double v_juros;
                if (!string.IsNullOrWhiteSpace(boleto.Juros_mora))
                {
                    try
                    {
                        v_juros = double.Parse(boleto.Juros_mora) / 100;
                    }
                    catch (Exception)
                    {
                        return "Juros de Mora inválido.";
                    }
                }
                else
                {
                    return "Juros de Mora não informado.";
                }

                double v_valor_pg;
                if (!string.IsNullOrWhiteSpace(boleto.Valor_pago))
                {
                    try
                    {
                        v_valor_pg = double.Parse(boleto.Valor_pago) / 100;
                    }
                    catch (Exception)
                    {
                        return "Valor pago inválido.";
                    }
                }
                else
                {
                    return "Valor Pago não informado.";
                }


                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {

                    var query = "update dealwish.faturas set data_pg = :data_pg, multa = :multa, juros = :juros, valor_pg = :valor_pg, id_situacao = :situacao_para, id_usuario_log = :id_usuario_login " +
                                "where nosso_numero = :nosso_numero and id_situacao = :situacao_de";
                    dyParam.Add("nosso_numero", boleto.Id_titulo_banco, DbType.String, ParameterDirection.Input);
                    dyParam.Add("data_pg", v_data_pg, DbType.Date, ParameterDirection.Input);
                    dyParam.Add("multa", v_multa, DbType.VarNumeric, ParameterDirection.Input);
                    dyParam.Add("juros", v_juros, DbType.VarNumeric, ParameterDirection.Input);
                    dyParam.Add("valor_pg", v_valor_pg, DbType.VarNumeric, ParameterDirection.Input);
                    dyParam.Add("situacao_de", Constantes.A_LIQUIDAR, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("situacao_para", Constantes.LIQUIDADA, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);


                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);
                    if (linhasafetadas > 0)
                    {
                        return "Fatura baixada com sucesso.";
                    }
                    else
                    {
                        return "Fatura já baixada ou cancelada";
                    }

                }
                else
                {
                    return "Erro crítico";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<string> RegistrarNFFaturaAsync(DetalheRetornoNF nf, long id_usuario_login)
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

                    var query = "update dealwish.faturas set num_nfe = :num_nfe, serie_nfe = :serie_nfe, int id_usuario_log = :int id_usuario_login " +
                                "where id = :id and num_nfe is null and serie_nfe is null";
                    dyParam.Add("num_nfe", int.Parse(nf.Num_nf), DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("serie_nfe", nf.Serie_nf, DbType.String, ParameterDirection.Input);
                    dyParam.Add("id", int.Parse(nf.Num_rps), DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);
                    if (linhasafetadas > 0)
                    {
                        return "NFe registrada com sucesso.";
                    }
                    else
                    {
                        return "NFe já registrada";
                    }

                }
                else
                {
                    return "Erro crítico";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<object> ConsultarIndicadoresAsync()
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
                    var query = "select 1 ordem, 'Usuários' indicador, count(id) valor from dealwish.usuarios where aplicativo = 'S'" +
                                " union" +
                                " select 2 ordem, 'Desejos' indicador, count(id) valor from dealwish.desejos" +
                                " union" +
                                " select 3 orderm, 'Empresas' indicador, count(id) valor from dealwish.contratos where id_situacao = 1" +
                                " union" +
                                " select 4 ordem, 'Ofertas' indicador, count(id) valor from dealwish.ofertas " +
                                " order by ordem";

                    result = await SqlMapper.QueryAsync(conn, query, param: dyParam, commandType: CommandType.Text);

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