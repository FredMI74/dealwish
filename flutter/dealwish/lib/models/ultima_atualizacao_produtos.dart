class UltimaAtualizacaoProdutos
{
  String ultima_atualizacao_produtos = "";
  bool erro;
  String mensagem = '';

  UltimaAtualizacaoProdutos(Map dados)
  {
    erro = dados['resultado']['erro'];

    if(!erro)
    {
      ultima_atualizacao_produtos = dados['conteudo']['ultima_atualizacao_produtos'];
    }

    mensagem = dados['resultado']['mensagem'];
  }

}