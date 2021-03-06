class RetornoProcessamento
{
  int linhasafetadas = 0;
  bool erro;
  String mensagem = '';

  RetornoProcessamento(Map dados)
  {
    erro = dados['resultado']['erro'];

    if(!erro)
    {
      linhasafetadas = dados['conteudo']['linhasafetadas'];
    }

    mensagem = dados['resultado']['mensagem'];
  }

}

class RetornoInclusao
{
  int id = 0;
  bool erro;
  String mensagem = '';

  RetornoInclusao(Map dados)
  {
    erro = dados['resultado']['erro'];

    if(!erro)
    {
      id = dados['conteudo']['id'];
    }

    mensagem = dados['resultado']['mensagem'];
  }

}