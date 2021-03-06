class RetornoTermoServico {
  String texto = '';
  bool erro;
  String mensagem = '';

  RetornoTermoServico(Map dados) {
    erro = dados['resultado']['erro'];

    if (!erro) {
      texto = dados['conteudo'][0]['texto'];
    }

    mensagem = dados['resultado']['mensagem'];
  }
}
