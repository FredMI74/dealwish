import 'dart:async';
import 'dart:convert';import 'package:dw_dashboard/main.dart';
import 'package:dw_dashboard/models/indicadores.dart';
import 'package:dw_dashboard/models/usuario_model.dart';
import 'package:http/http.dart' as http;

const URLDW = 'http://192.168.0.38:50001';
const TOKEN_ANONIMO =
    '0bbb03e7c8856d5fbd3e0e04c1af1d687b19c92b908f058f81e7d91538d2d616';
const TEMPO_VIDA_TOKEN = 25;
const ATIVO = "1";
const APLICATIVO = "A";
const SIM = "S";
const NAO = "N";

class ApiHelper {
  // Singleton
  static final ApiHelper _instance = ApiHelper.internal();

  factory ApiHelper() => _instance;

  ApiHelper.internal();

  DateTime _ultima_chamada;

  Future<Usuario> loginUsuario(String email, String senha) async {

    _ultima_chamada = DateTime.now();

    var _login = new Map();
    _login['email'] = email;
    _login['senha'] = senha;
    _login['token'] = TOKEN_ANONIMO;

    final response =
        await http.post(URLDW + '/api/login_usuario', body: _login);

    if (response.statusCode == 200) {
      RetornoLogin _dados = RetornoLogin(json.decode(response.body));

      usuario.token = _dados.token;
      usuario.nome = _dados.nome;
      usuario.id = _dados.id;
      usuario.email = email;
      usuario.senha1 = senha;
      usuario.id = _dados.id;
      usuario.id_cidade_ap = _dados.id_cidade_ap;
      usuario.cidade = _dados.cidade;
      usuario.uf = _dados.uf;
      usuario.cpf = _dados.cpf;
      usuario.mensagem = _dados.mensagem;

      return usuario;
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<List> consultarIndicadores() async {

    await renovartoken();

    var _indicadores = new Map();
    _indicadores['id_usuario'] = usuario.id.toString();
    _indicadores['token'] = usuario.token;

    final response =
    await http.post(URLDW + '/api/consultar_indicadores', body: _indicadores);

    if (response.statusCode == 200) {
      RetornoIndicadores _dados = RetornoIndicadores(json.decode(response.body));

      if (_dados.erro) {
        throw Exception('Erro ao buscar os dados.');
      } else {
        return _dados.indicadoresList;
      }
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future renovartoken() async {

    if  (usuario.token.isNotEmpty)
    {
      Duration difference = DateTime.now().difference(_ultima_chamada);
      if (difference.inMinutes >= TEMPO_VIDA_TOKEN)
      {
        await loginUsuario(usuario.email, usuario.senha1);
      }
    }
    _ultima_chamada = DateTime.now();
  }

}
