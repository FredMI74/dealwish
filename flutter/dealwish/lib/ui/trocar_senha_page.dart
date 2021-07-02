import 'dart:io';

import 'package:dealwish/helpers/api_helper.dart';
import 'package:dealwish/main.dart';
import 'package:dealwish/models/retorno_proc_model.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class TrocarSenhaPage extends StatefulWidget {
  @override
  _TrocarSenhaPageState createState() => _TrocarSenhaPageState();
}

class _TrocarSenhaPageState extends State<TrocarSenhaPage> {
  ApiHelper api_helper = ApiHelper();

  final _storage = new FlutterSecureStorage();

  final _senhaController = TextEditingController();
  final _novasenhaController = TextEditingController();
  final _confnovasenhaController = TextEditingController();

  bool isLoading = false;

  final _formKey = GlobalKey<FormState>();
  final _scaffoldKey = GlobalKey<ScaffoldState>();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        key: _scaffoldKey,
        appBar: AppBar(
            centerTitle: true,
            iconTheme: IconThemeData(
              color: Colors.white, //change your color here
            ),
            title: Text('Trocar Senha', style: TextStyle(color: Colors.white))),
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
                              controller: _senhaController,
                              decoration:
                                  InputDecoration(labelText: 'senha atual'),
                              keyboardType: TextInputType.text,
                              obscureText: true,
                              validator: (value) {
                                if (value.isEmpty) {
                                  return 'informe a senha atual';
                                }
                              },
                            ),
                            TextFormField(
                              controller: _novasenhaController,
                              decoration:
                                  InputDecoration(labelText: 'nova senha'),
                              keyboardType: TextInputType.text,
                              obscureText: true,
                              validator: (value) {
                                if (value.isEmpty) {
                                  return 'informe a nova senha';
                                }
                              },
                            ),
                            TextFormField(
                              controller: _confnovasenhaController,
                              decoration: InputDecoration(
                                  labelText: 'confirmação da nova senha'),
                              keyboardType: TextInputType.text,
                              obscureText: true,
                              validator: (value) {
                                if (value.isEmpty) {
                                  return 'confirme a nova senha';
                                } else {
                                  if (value !=
                                      _novasenhaController.value.text) {
                                    return 'as senhas informadas não são iguais';
                                  }
                                }
                              },
                            ),
                          ],
                        ),
                      ),
                      Padding(
                          padding: EdgeInsets.only(top: 20.0),
                          child: Column(
                            children: <Widget>[
                              ElevatedButton(
                                child: Text('Confirmar',
                                    style: TextStyle(color: Colors.white)),
                                style: ElevatedButton.styleFrom(
                                  primary: Color.fromARGB(255, 255, 127, 0),
                                  onPrimary: Colors.white,
                                ),
                                onPressed: () {
                                  FocusScope.of(context).requestFocus(new FocusNode());
                                  if (_formKey.currentState.validate()) {
                                    _trocarSenha(
                                        usuario.email,
                                        _senhaController.value.text,
                                        _novasenhaController.value.text,
                                        _confnovasenhaController.value.text);
                                  }
                                },
                              )
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

  void _trocarSenha(String email, String senha_atual, String senha_nova,
      String senha_nova_conf) async {
    setState(() {
      SystemChannels.textInput.invokeMethod('TextInput.hide');
      isLoading = true;
    });

    try {
      RetornoProcessamento retornoProcessamento = await api_helper.trocarSenha(
          email, senha_atual, senha_nova, senha_nova_conf);

      usuario.senha1 = senha_nova;
      usuario.senha2 = senha_nova_conf;

      if (retornoProcessamento.linhasafetadas > 0) {
        setState(() {
          isLoading = false;
        });
        _salvardadosUsuario().then((_) {
          _onSucess();
        });
      } else {
        _onFail(retornoProcessamento.mensagem);
      }
    } catch (e) {
      isLoading = false;
      _onFail('Erro ao trocar a senha.');
    }
  }

  Future _salvardadosUsuario() async {
    await _storage.delete(key: 'email');
    await _storage.delete(key: 'senha');
    await _storage.write(key: 'email', value: usuario.email);
    await _storage.write(key: 'senha', value: usuario.senha1);
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

  void _onSucess() {
    setState(() {
      _senhaController.clear();
      _novasenhaController.clear();
      _confnovasenhaController.clear();
      isLoading = false;
      _scaffoldKey.currentState.showSnackBar(SnackBar(
        content: Text('Senha alterada'),
        backgroundColor: Colors.green,
        duration: Duration(seconds: 3),
      ));
    });
  }
}
