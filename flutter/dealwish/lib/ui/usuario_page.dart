import 'package:dealwish/helpers/api_helper.dart';
import 'package:dealwish/main.dart';
import 'package:dealwish/models/cidade_model.dart';
import 'package:dealwish/models/retorno_proc_model.dart';
import 'package:flutter/material.dart';
import 'package:dealwish/models/usuario_model.dart';
import 'package:flutter/services.dart';
import 'package:flutter_masked_text/flutter_masked_text.dart';
import 'package:geolocator/geolocator.dart';
import 'package:geocoding/geocoding.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:intl/intl.dart';

class UsuarioPage extends StatefulWidget {
  bool novousuario;

  UsuarioPage({this.novousuario});

  @override
  _UsuarioPageState createState() => _UsuarioPageState();
}

class _UsuarioPageState extends State<UsuarioPage> {
  ApiHelper api_helper = ApiHelper();

  final _storage = new FlutterSecureStorage();

  final _confsenhaController = TextEditingController();
  final _nomeController = TextEditingController();
  final _datanascController = MaskedTextController(mask: '00/00/0000');
  final _cpfController = MaskedTextController(mask: '000.000.000-00');
  final _emailController = TextEditingController();
  final _senhaController = TextEditingController();

  final _formKey = GlobalKey<FormState>();
  final _scaffoldKey = GlobalKey<ScaffoldState>();

  bool isLoading = false;
  bool cidadePorLocalizacao = false;
  bool localizandoCidade = true;
  bool bucandoListaCidades = false;
  bool localizarnovamente = false;

  static Cidade _cidadeIni = Cidade({'id': 0, 'nome': 'Cidade', 'uf': ''});

  List<Cidade> _cidades = [];

  List _ufs = [
    'Estado',
    'AC',
    'AL',
    'AM',
    'AP',
    'BA',
    'CE',
    'DF',
    'ES',
    'GO',
    'MA',
    'MG',
    'MS',
    'MT',
    'PA',
    'PB',
    'PE',
    'PI',
    'PR',
    'RJ',
    'RN',
    'RO',
    'RR',
    'RS',
    'SC',
    'SE',
    'SP',
    'TO'
  ];

  List<DropdownMenuItem<String>> _dropDownMenuItemsCidade;
  List<DropdownMenuItem<String>> _dropDownMenuItemsUF;
  String _cidadeSel;
  String _ufSel;
  String _nomeCidadeSel;

  @override
  void initState() {
    bucandoListaCidades = !widget.novousuario;
    localizarnovamente = !widget.novousuario;

    super.initState();

    if (widget.novousuario) {
      _getLocation(testonly: !widget.novousuario);
    }

    _dropDownMenuItemsUF = getDropDownMenuItemsUF();
    _ufSel = _dropDownMenuItemsUF[0].value;

    _dropDownMenuItemsCidade = getDropDownMenuItemsCidade();
    _cidadeSel = _dropDownMenuItemsCidade[0].value;

    if (!widget.novousuario) {
      _nomeController.text = usuario.nome;
      _datanascController.text =
          DateFormat('dd/MM/yyyy').format(usuario.data_nasc);
      _cpfController.text = usuario.cpf;
      _emailController.text = usuario.email;
      _cidadeSel = usuario.id_cidade_ap.toString();
      _ufSel = usuario.uf;
      _nomeCidadeSel = usuario.cidade;

      setState(() {
        localizandoCidade = false;
        cidadePorLocalizacao = true;
        bucandoListaCidades = false;
      });
    }
  }

  List<DropdownMenuItem<String>> getDropDownMenuItemsCidade() {
    List<DropdownMenuItem<String>> items = new List.empty(growable: true);
    _cidades.insert(0, _cidadeIni);
    for (Cidade cidade in _cidades) {
      items.add(new DropdownMenuItem(
          value: cidade.id.toString(), child: new Text(cidade.nome)));
    }
    return items;
  }

  List<DropdownMenuItem<String>> getDropDownMenuItemsUF() {
    List<DropdownMenuItem<String>> items = new List.empty(growable: true);
    for (String uf in _ufs) {
      items.add(new DropdownMenuItem(value: uf, child: new Text(uf)));
    }
    return items;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        key: _scaffoldKey,
        appBar: AppBar(
            centerTitle: true,
            iconTheme: IconThemeData(
              color: Colors.white, //change your color here
            ),
            title: Text(widget.novousuario ? 'Novo Usuário' : 'Meus dados',
                style: TextStyle(color: Colors.white))),
        body: Stack(
          children: <Widget>[
            SingleChildScrollView(
                padding: EdgeInsets.all(20.0),
                child: Form(
                  key: _formKey,
                  child: Column(
                    children: <Widget>[
                      Padding(
                        padding: EdgeInsets.only(bottom: 10.0),
                        child: Column(
                          children: <Widget>[
                            TextFormField(
                              controller: _nomeController,
                              decoration: InputDecoration(labelText: 'nome'),
                              keyboardType: TextInputType.text,
                              validator: (value) {
                                if (value.isEmpty) {
                                  return 'informe o nome';
                                }
                              },
                            ),
                            TextFormField(
                              controller: _datanascController,
                              decoration: InputDecoration(
                                  labelText: 'data de nascimento'),
                              keyboardType: TextInputType.number,
                              validator: (value) {
                                if (value.isEmpty) {
                                  return 'informe a data de nascimento';
                                }
                              },
                            ),
                            TextFormField(
                              controller: _cpfController,
                              decoration: InputDecoration(labelText: 'CPF'),
                              keyboardType: TextInputType.number,
                              validator: (value) {
                                if (value.isEmpty) {
                                  return 'informe o CPF';
                                }
                              },
                            ),
                            TextFormField(
                              controller: _emailController,
                              decoration: InputDecoration(labelText: 'e-mail'),
                              keyboardType: TextInputType.emailAddress,
                              validator: (value) {
                                if (value.isEmpty) {
                                  return 'informe o e-mail';
                                }
                              },
                            ),
                            widget.novousuario
                                ? TextFormField(
                                    controller: _senhaController,
                                    decoration:
                                        InputDecoration(labelText: 'senha'),
                                    keyboardType: TextInputType.text,
                                    obscureText: true,
                                    validator: (value) {
                                      if (value.isEmpty) {
                                        return 'informe a senha';
                                      }
                                    },
                                  )
                                : Container(),
                            widget.novousuario
                                ? TextFormField(
                                    controller: _confsenhaController,
                                    decoration: InputDecoration(
                                        labelText: 'confirmação da senha'),
                                    keyboardType: TextInputType.text,
                                    obscureText: true,
                                    validator: (value) {
                                      if (value.isEmpty) {
                                        return 'confirme a senha';
                                      } else {
                                        if (value !=
                                            _senhaController.value.text) {
                                          return 'as senhas informadas não são iguais';
                                        }
                                      }
                                    },
                                  )
                                : Container(),
                            layoutSelecionarCidade(),
                          ],
                        ),
                      ),
                      Padding(
                          padding: EdgeInsets.only(top: 10.0),
                          child: Column(
                            children: <Widget>[

                              SizedBox(
                                  width: 100,
                                  child: ElevatedButton(
                                    child: Text(
                                        widget.novousuario ? 'Criar' : 'Atualizar',
                                        style: TextStyle(color: Colors.white)),
                                    style: ElevatedButton.styleFrom(
                                      primary: Color.fromARGB(255, 255, 127, 0),
                                      onPrimary: Colors.white,
                                    ),
                                    onPressed: () {
                                      if (_formKey.currentState.validate()) {
                                        if (widget.novousuario) {
                                          usuario.limpar();

                                          usuario.email =
                                              _emailController.value.text;
                                          usuario.nome = _nomeController.value.text;
                                          usuario.senha1 =
                                              _senhaController.value.text;
                                          usuario.senha2 =
                                              _confsenhaController.value.text;
                                          usuario.data_nasc = DateTime.parse(
                                              _datanascController.value.text
                                                  .substring(6, 10) +
                                                  '-' +
                                                  _datanascController.value.text
                                                      .substring(3, 5) +
                                                  '-' +
                                                  _datanascController.value.text
                                                      .substring(0, 2));
                                          usuario.cpf = _cpfController.value.text;
                                          usuario.id_cidade_ap =
                                              int.parse(_cidadeSel);
                                          usuario.uf = _ufSel;
                                          usuario.cidade = retornaNomeCidade(
                                              usuario.id_cidade_ap);
                                          _incluirUsuario(usuario);
                                        } else {
                                          Usuario _usuario_atu = new Usuario();
                                          _usuario_atu.id = usuario.id;
                                          _usuario_atu.token = usuario.token;
                                          _usuario_atu.token_jwt =
                                              usuario.token_jwt;
                                          _usuario_atu.email =
                                              _emailController.value.text;
                                          _usuario_atu.nome =
                                              _nomeController.value.text;
                                          _usuario_atu.senha1 =
                                              _senhaController.value.text;
                                          _usuario_atu.senha2 =
                                              _confsenhaController.value.text;
                                          _usuario_atu.data_nasc = DateTime.parse(
                                              _datanascController.value.text
                                                  .substring(6, 10) +
                                                  '-' +
                                                  _datanascController.value.text
                                                      .substring(3, 5) +
                                                  '-' +
                                                  _datanascController.value.text
                                                      .substring(0, 2));
                                          _usuario_atu.cpf =
                                              _cpfController.value.text;
                                          _usuario_atu.id_cidade_ap =
                                              int.parse(_cidadeSel);
                                          _usuario_atu.uf = _ufSel;
                                          _usuario_atu.cidade = retornaNomeCidade(
                                              _usuario_atu.id_cidade_ap);
                                          _atualizarUsuario(_usuario_atu);
                                        }
                                      }
                                    },
                                  )
                              ),
                              widget.novousuario
                                  ? Column(children: <Widget>[
                                      Padding(
                                        padding: EdgeInsets.only(top: 20.0),
                                        child: Text(
                                            'Ao se cadastrar você confirma que leu e concorda com o nosso Termo de Serviço.',
                                            style: TextStyle(
                                                fontStyle: FontStyle.italic)),
                                      ),
                                      TextButton(
                                        child: Text("Termo de Serviço",
                                            style: TextStyle(
                                                color: Colors.indigo,
                                                fontSize: 16.0,
                                                decoration:
                                                    TextDecoration.underline)),
                                        onPressed: () {
                                          Navigator.pushNamed(
                                              context, '/termoservico');
                                        },
                                      ),
                                    ])
                                  : Column(children: <Widget>[
                                      Container(height: 10),
                                      TextButton(
                                        child: Text("Excluir",
                                            style: TextStyle(
                                                color: Colors.indigo,
                                                fontSize: 14.0,
                                                decoration:
                                                    TextDecoration.underline)),
                                        onPressed: () {
                                          _confirmarExclusao();
                                        },
                                      )
                                    ]),
                            ],
                          ))
                    ],
                  ),
                )),
            isLoading
                ? Container(
                    color: Colors.black.withOpacity(0.5),
                    child: Center(
                      child: CircularProgressIndicator(
                        valueColor: new AlwaysStoppedAnimation<Color>(
                            Color.fromARGB(255, 255, 127, 0)),
                      ),
                    ),
                  )
                : Container()
          ],
        ));
  }

  Widget layoutSelecionarCidade() {
    return localizandoCidade
        ? Container(
            height: 48.0,
            decoration: BoxDecoration(
                border: Border(
                    bottom: BorderSide(width: 1.5, color: Colors.black38))),
            child: Row(
              children: <Widget>[
                Text(
                    bucandoListaCidades
                        ? 'Buscando lista de cidades...'
                        : 'Localizando cidade...',
                    style: TextStyle(
                        fontStyle: FontStyle.italic,
                        fontSize: 16.0,
                        color: Colors.black38)),
              ],
            ))
        : cidadePorLocalizacao
            ? Container(
                height: 48.0,
                decoration: BoxDecoration(
                    border: Border(
                        bottom: BorderSide(width: 1.5, color: Colors.black38))),
                child: Row(
                  children: <Widget>[
                    Expanded(
                      flex: 9,
                      child: Text(_nomeCidadeSel + '/' + _ufSel,
                          style: TextStyle(fontSize: 16.0)),
                    ),
                    Expanded(
                      flex: 1,
                      child: IconButton(
                        icon: Icon(localizarnovamente
                            ? Icons.my_location
                            : Icons.edit),
                        onPressed: () {
                          setState(() {
                            if (localizarnovamente) {
                              _getLocation();
                              localizarnovamente = false;
                              bucandoListaCidades = false;
                              localizandoCidade = true;
                            } else {
                              bucandoListaCidades = true;
                              localizandoCidade = true;
                              cidadePorLocalizacao = false;
                              selecionarCidade();
                            }
                          });
                        },
                      ),
                    )
                  ],
                ))
            : Container(
                height: 48.0,
                child: Row(
                  children: <Widget>[
                    Expanded(
                        flex: 3,
                        child: Padding(
                            padding: EdgeInsets.only(right: 10.0),
                            child: DropdownButton(
                              isExpanded: true,
                              value: _ufSel,
                              items: _dropDownMenuItemsUF,
                              onChanged: changedDropDownItemUF,
                            ))),
                    Expanded(
                        flex: 7,
                        child: DropdownButton(
                          isExpanded: true,
                          value: _cidadeSel,
                          items: _dropDownMenuItemsCidade,
                          onChanged: changedDropDownItemCidade,
                        ))
                  ],
                ));
  }

  void _onFail(String mensagem) {
    setState(() {
      isLoading = false;
      _scaffoldKey.currentState.showSnackBar(SnackBar(
        content: Text(mensagem),
        backgroundColor: Colors.redAccent,
        duration: Duration(seconds: 2),
      ));
    });
  }

  void changedDropDownItemCidade(String cidadeSelecionada) {
    setState(() {
      _cidadeSel = cidadeSelecionada;
    });
  }

  void changedDropDownItemUF(String ufSelecionado) async {
    setState(() {
      _ufSel = ufSelecionado;
    });
    _cidades.clear();

    setState(() {
      _dropDownMenuItemsCidade.clear();
      _dropDownMenuItemsCidade.add(
          new DropdownMenuItem(value: '0', child: new Text('Carregando...')));
    });

    if (_ufSel != 'Estado') {
      _cidades = await api_helper.consultarCidades(uf: _ufSel);
    }

    setState(() {
      _dropDownMenuItemsCidade = getDropDownMenuItemsCidade();
      _cidadeSel = _dropDownMenuItemsCidade[0].value;
    });
    isLoading = false;
  }

  Future buscarUfCidade(int id_cidade) async {
    List<Cidade> _cidade = [];
    try {
      _cidade = await api_helper.consultarCidades(id: id_cidade);
      return _cidade[0].uf;
    } catch (e) {
      return 'Estado';
    }
  }

  Future<int> buscarIdCidade(String nome_cidade) async {
    try {
      _cidades = await api_helper.consultarCidades(nome: nome_cidade);
      _dropDownMenuItemsCidade = getDropDownMenuItemsCidade();
      return _cidades[1].id;
    } catch (e) {
      return 0;
    }
  }

  Future selecionarCidade() async {
    await changedDropDownItemUF(_ufSel);
    localizandoCidade = false;
  }

  void _incluirUsuario(Usuario usuario) async {
    setState(() {
      SystemChannels.textInput.invokeMethod('TextInput.hide');
      isLoading = true;
    });

    try {
      RetornoInclusao retornoInclusao =
          await api_helper.incluirUsuario(usuario);

      if (retornoInclusao.id > 0) {
        await _salvardadosUsuario(usuario);
        isLoading = false;
        Navigator.of(context).pushReplacementNamed('/login');
      } else {
        isLoading = false;
        _onFail(retornoInclusao.mensagem);
      }
    } catch (e) {
      isLoading = false;
      _onFail('Erro ao incluir o usuário.');
    }
  }

  void _salvardadosUsuario(Usuario usuario) async {
    await _storage.delete(key: 'email');
    await _storage.delete(key: 'senha');
    await _storage.write(key: 'email', value: usuario.email);
    await _storage.write(key: 'senha', value: usuario.senha1);
  }

  void _atualizarUsuario(Usuario _usuario_atu) async {
    setState(() {
      SystemChannels.textInput.invokeMethod('TextInput.hide');
      isLoading = true;
    });

    try {
      RetornoProcessamento retornoProcessamento =
          await api_helper.atualizarUsuario(_usuario_atu);

      if (retornoProcessamento.linhasafetadas > 0) {
        setState(() {
          usuario.email = _usuario_atu.email;
          usuario.nome = _usuario_atu.nome;
          usuario.senha1 = _usuario_atu.senha1;
          usuario.senha2 = _usuario_atu.senha2;
          usuario.data_nasc = _usuario_atu.data_nasc;
          usuario.cpf = _usuario_atu.cpf;
          usuario.id_cidade_ap = _usuario_atu.id_cidade_ap;
          usuario.uf = _usuario_atu.uf;
          usuario.cidade = _usuario_atu.cidade;
          isLoading = false;
        });
        Navigator.pop(context);
      } else {
        isLoading = false;
        _onFail(retornoProcessamento.mensagem);
      }
    } catch (e) {
      isLoading = false;
      _onFail('Erro ao atualizar o usuário.');
    }
  }

  Future<Position> _determinePosition() async {
    bool serviceEnabled;
    LocationPermission permission;

    serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) {
      return Future.error('Location services are disabled.');
    }

    permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.deniedForever) {
      return Future.error(
          'Location permissions are permantly denied, we cannot request permissions.');
    }

    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
      if (permission != LocationPermission.whileInUse &&
          permission != LocationPermission.always) {
        return Future.error(
            'Location permissions are denied (actual value: $permission).');
      }
    }

    return await Geolocator.getCurrentPosition();
  }

  Future<bool> _getLocation({bool testonly = false}) async {
    Position currentLocation;
    try {
      try {
        currentLocation = await _determinePosition();
      } catch (e) {
        currentLocation = null;
      }

      if (testonly) {
        setState(() {
          localizarnovamente = currentLocation != null;
        });
      }

      if (currentLocation != null) {
        List<Placemark> addresses = await placemarkFromCoordinates(
            currentLocation.latitude, currentLocation.longitude);

        if (addresses.isNotEmpty) {
          var first = addresses.first;
          String uf = siglaUF(first.administrativeArea);

          if (uf != 'NA') {
            _ufSel = uf;

            int id_cidade = await buscarIdCidade(first.subAdministrativeArea);
            if (id_cidade != 0) {
              _cidadeSel = id_cidade.toString();
              _nomeCidadeSel = first.subAdministrativeArea;

              setState(() {
                cidadePorLocalizacao = true;
              });
            } else {
              setState(() {
                _ufSel = 'Estado';
                cidadePorLocalizacao = false;
              });
            }
          }
        }
      }
    } finally {
      setState(() {
        localizandoCidade = false;
      });
    }
  }

  String siglaUF(String nome_uf) {
    switch (nome_uf.toLowerCase()) {
      case 'acre':
        {
          return 'AC';
        }
        break;

      case 'alagoas':
        {
          return 'AL';
        }
        break;
      case 'amazonas':
        {
          return 'AM';
        }
        break;
      case 'amapá':
        {
          return 'AP';
        }
        break;
      case 'bahia':
        {
          return 'BA';
        }
        break;
      case 'ceará':
        {
          return 'CE';
        }
        break;
      case 'distrito federal':
        {
          return 'DF';
        }
        break;
      case 'espírito santo':
        {
          return 'ES';
        }
        break;
      case 'goiás':
        {
          return 'GO';
        }
        break;
      case 'maranhão':
        {
          return 'MA';
        }
        break;
      case 'minas gerais':
        {
          return 'MG';
        }
        break;
      case 'mato grosso do sul':
        {
          return 'MS';
        }
        break;
      case 'mata grosso':
        {
          return 'MT';
        }
        break;
      case 'pará':
        {
          return 'PA';
        }
        break;
      case 'paraíba':
        {
          return 'PB';
        }
        break;
      case 'pernambuco':
        {
          return 'PE';
        }
        break;
      case 'piauí':
        {
          return 'PI';
        }
        break;
      case 'paraná':
        {
          return 'PR';
        }
        break;
      case 'rio de janeiro':
        {
          return 'RJ';
        }
        break;
      case 'rio grande do norte':
        {
          return 'RN';
        }
        break;
      case 'rondonia':
        {
          return 'RO';
        }
        break;
      case 'roraima':
        {
          return 'RR';
        }
        break;

      case 'rio grande do sul':
        {
          return 'RS';
        }
        break;

      case 'santa catarina':
        {
          return 'SC';
        }
        break;
      case 'sergipe':
        {
          return 'SE';
        }
        break;
      case 'são paulo':
        {
          return 'SP';
        }
        break;
      case 'tocantins':
        {
          return 'TO';
        }
        break;

      default:
        {
          return 'NA';
        }
        break;
    }
  }

  String retornaNomeCidade(int id_cidade) {
    String _nome_cidade = null;
    for (Cidade cidade in _cidades) {
      if (cidade.id == id_cidade) {
        _nome_cidade = cidade.nome;
      }
    }
    if (_nome_cidade == null) {
      _nome_cidade = usuario.cidade;
    }
    return _nome_cidade;
  }

  void _confirmarExclusao() {
    // flutter defined function
    showDialog(
      context: context,
      builder: (BuildContext context) {
        // return object of type Dialog
        return AlertDialog(
          title: Text("Excluir usuário?"),
          content: Text(
              "Seu usuário, desejos e ofertas serão permanentemente excluídos. Para voltar a utilizar o aplicativo você deverá fazer um novo cadastro."),
          actions: <Widget>[
            // usually buttons at the bottom of the dialog
            TextButton(
              child: new Text("Cancelar"),
              onPressed: () {
                Navigator.of(context).pop();
              },
            ),
            TextButton(
              child: new Text("OK"),
              onPressed: () {
                _excluirUsuario();
                Navigator.of(context).pop();
              },
            ),
          ],
        );
      },
    );
  }

  void _excluirUsuario() async {
    setState(() {
      SystemChannels.textInput.invokeMethod('TextInput.hide');
      isLoading = true;
    });

    try {
      RetornoProcessamento retornoInclusao =
          await api_helper.excluirUsuario(usuario.id);

      if (retornoInclusao.erro == false) {
        setState(() {
          _mensagem('Usuário excluído.');
          usuario.limpar();
          _storage.deleteAll();
        });
        await Future.delayed(const Duration(seconds: 2));
        setState(() {
          isLoading = false;
        });
        Navigator.pushNamedAndRemoveUntil(
            context, '/login', ModalRoute.withName('/'));
      } else {
        _onFail(retornoInclusao.mensagem);
      }
    } catch (e) {
      isLoading = false;
      _onFail("Falha ao excluir o usuário.");
    }
  }

  _mensagem(String texto) async {
    _scaffoldKey.currentState.showSnackBar(SnackBar(
      content: Text(texto),
      backgroundColor: Colors.green,
      duration: Duration(seconds: 2),
    ));
  }
}
