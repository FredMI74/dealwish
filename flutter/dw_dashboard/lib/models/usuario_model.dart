class RetornoLogin
{
  int id = 0;
  String token = '';
  String nome  = '';
  int id_cidade_ap = 0;
  String cidade = '';
  String uf = '';
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
      nome = dados['conteudo']['nome'];
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
  String cpf;
  String aplicativo;
  int id_cidade_ap;
  String cidade;
  String uf;
  String token;
  String mensagem;

}