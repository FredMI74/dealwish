class RetornoEmpresa {
  List<EmpresaModel> empresaList = List.empty(growable: true);
  bool erro;
  String mensagem = '';

  RetornoEmpresa(Map dados) {
    erro = dados['resultado']['erro'];

    if (!erro) {
      for (Map value in dados['conteudo']) {
        EmpresaModel _empresa = EmpresaModel(value);
        empresaList.add(_empresa);
      }
    }

    mensagem = dados['resultado']['mensagem'];
  }
}

class EmpresaModel {
  int id;
  String fantasia;
  String razao_social;
  String cnpj;
  String insc_est;
  String url;
  String email_sac;
  String fone_sac;

  EmpresaModel(Map _map) {
    id = _map['id'];
    fantasia = _map['fantasia'];
    razao_social = _map['razao_social'];
    cnpj = _map['cnpj'].substring(0,2) + '.' + _map['cnpj'].substring(2,5) + '.' + _map['cnpj'].substring(5,8) + '/' + _map['cnpj'].substring(8,12) + '-' + _map['cnpj'].substring(12,14);
    insc_est = _map['insc_est'];
    url = _map['url'];
    email_sac = _map['email_sac'];
    fone_sac = _map['fone_sac'];
  }
}
