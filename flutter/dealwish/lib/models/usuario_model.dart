import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class RetornoLogin
{
  int id = 0;
  String token = '';
  String nome  = '';
  String token_jwt = '';
  int id_cidade_ap = 0;
  String cidade = '';
  String uf = '';
  DateTime data_nasc;
  String cpf = '';
  bool erro;
  String mensagem = '';

  RetornoLogin(Map dados)
  {
    erro = dados['resultado']['erro'];

    if(!erro)
    {
      id = dados['conteudo']['id'];
      token = dados['conteudo']['token'];
      token_jwt = dados['conteudo']['tokenJwt'];
      nome = dados['conteudo']['nome'];
      data_nasc = DateTime.parse(dados['conteudo']['data_nasc']);
      cpf = dados['conteudo']['cpf'];
      id_cidade_ap = dados['conteudo']['id_cidade_ap'];
      cidade = dados['conteudo']['cidade'];
      uf = dados['conteudo']['uf'];
    }

    mensagem = dados['resultado']['mensagem'];
  }

}

class Usuario
{
  int id;
  String nome;
  String email;
  String senha1;
  String senha2;
  DateTime data_nasc;
  String cpf;
  String aplicativo;
  int id_cidade_ap;
  String cidade;
  String uf;
  String token;
  String token_jwt;
  String token_app;
  String mensagem;

  void limpar()
  {
    final _storage = new FlutterSecureStorage();
    _storage.deleteAll();
    id = 0;
    nome = '';
    email = '';
    senha1 = '';
    senha2 = '';
    data_nasc = DateTime.now();
    cpf = '';
    aplicativo = '';
    id_cidade_ap = 0;
    cidade = '';
    uf = '';
    token = '';
    token_jwt = '';
    mensagem = '';
  }
}