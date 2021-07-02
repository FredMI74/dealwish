import 'package:dealwish/helpers/api_helper.dart';
import 'package:dealwish/main.dart';
import 'package:dealwish/models/retorno_proc_model.dart';
import 'package:dealwish/models/secitem_model.dart';
import 'package:dealwish/ui/usuario_page.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/material.dart';
import 'package:dealwish/models/usuario_model.dart';
import 'package:flutter/services.dart';
import 'package:flutter_masked_text/flutter_masked_text.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:firebase_core/firebase_core.dart';

class LoginPage extends StatefulWidget {
  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  @override
  void initState() {
    super.initState();
    showLogin = false;
    _buscarDadosUsuario();
  }

  ApiHelper api_helper = ApiHelper();

  final _storage = new FlutterSecureStorage();
  List<SecItem> _items = [];

  final _emailController = TextEditingController();
  final _senhaController = TextEditingController();
  final _cpfController = MaskedTextController(mask: '000.000.000-00');

  bool isLoading = false;
  bool showLogin = false;

  final _formKey = GlobalKey<FormState>();
  final _scaffoldKey = GlobalKey<ScaffoldState>();

  bool reiniciandoSenha = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: _scaffoldKey,
      appBar: AppBar(
          centerTitle: true,
          title: Text("Bem-vindo", style: TextStyle(color: Colors.white))),
      body: Stack(
        children: <Widget>[
          Image.asset(
            "images/fundo.png",
            fit: BoxFit.fitHeight,
            height: 1000.0,
          ),
          showLogin ? _formulario() : Container(),
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
      ),
    );
  }

  Widget _formulario() {
    return SingleChildScrollView(
        padding: EdgeInsets.all(40.0),
        child: Form(
          key: _formKey,
          child: Column(
            children: <Widget>[
              Container(
                decoration: BoxDecoration(
                    color: Color.fromARGB(255, 255, 204, 153),
                    borderRadius: BorderRadius.only(
                        bottomLeft: const Radius.circular(15.0),
                        bottomRight: const Radius.circular(15.0),
                        topLeft: const Radius.circular(15.0),
                        topRight: const Radius.circular(15.0))),
                padding: EdgeInsets.all(10.0),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceAround,
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: <Widget>[
                    Image.asset('images/dw_icon_rounded.png', scale: 4.0),
                    Text(
                      "Dealwish",
                      style: TextStyle(
                          fontSize: 35.0,
                          color: Colors.white,
                          fontWeight: FontWeight.bold),
                    )
                  ],
                ),
              ),
              Padding(
                padding: EdgeInsets.only(top: 20.0),
                child: Column(
                  children: <Widget>[
                    TextFormField(
                      controller: _emailController,
                      decoration: InputDecoration(labelText: "e-mail"),
                      keyboardType: TextInputType.emailAddress,
                      validator: (value) {
                        if (value.isEmpty) {
                          return "informe o e-mail";
                        }
                      },
                    ),
                    TextFormField(
                      controller: _senhaController,
                      decoration: InputDecoration(labelText: "senha"),
                      keyboardType: TextInputType.text,
                      obscureText: true,
                      validator: (value) {
                        if (value.isEmpty && reiniciandoSenha == false) {
                          return "informe a senha";
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
                      SizedBox(
                          width: 100,
                          child: ElevatedButton(
                            child: Text("Entrar",
                                style: TextStyle(color: Colors.white)),
                            style: ElevatedButton.styleFrom(
                              primary: Color.fromARGB(255, 255, 127, 0),
                              onPrimary: Colors.white, // foreground
                            ),
                            onPressed: () {
                              reiniciandoSenha = false;
                              if (_formKey.currentState.validate()) {
                                usuario.email = _emailController.value.text;
                                usuario.senha1 = _senhaController.value.text;
                                _loginUsusario(usuario);
                              }
                            },
                          )
                      ),
                      SizedBox(
                        width: 100,
                        child: ElevatedButton(
                          child: Text("Cadastrar",
                              style: TextStyle(color: Colors.black)),
                          style: ElevatedButton.styleFrom(
                              primary: Color.fromARGB(255, 230, 230, 230),
                              onPrimary: Colors.white
                          ),
                          onPressed: () {
                            Navigator.push(
                                context,
                                MaterialPageRoute(
                                    builder: (context) =>
                                        UsuarioPage(novousuario: true)));
                          },
                        )
                      ),
                      TextButton(
                        child: Text("esqueci minha senha",
                            style: TextStyle(
                                color: Colors.black26,
                                decoration: TextDecoration.underline)),
                        onPressed: () {
                          reiniciandoSenha = true;
                          if (_formKey.currentState.validate()) {
                            _confirmarReinicioSenha();
                          }
                        },
                      ),
                    ],
                  ))
            ],
          ),
        ));
  }

  void _loginUsusario(Usuario usuario) async {
    setState(() {
      SystemChannels.textInput.invokeMethod('TextInput.hide');
      isLoading = true;
    });

    try {
      await Firebase.initializeApp();

      String _token_app = await FirebaseMessaging.instance.getToken();

      usuario = await api_helper
          .loginUsuario(usuario.email, usuario.senha1, _token_app);

      if (usuario.token != null && usuario.token.isNotEmpty) {
        isLoading = false;
        await _salvardadosUsuario();
        Navigator.of(context).pushReplacementNamed('/desejos');
      } else {
        setState(() {
          isLoading = false;
          showLogin = true;
        });
        _onFail(usuario.mensagem);
      }
    } catch (e) {
      setState(() {
        isLoading = false;
        showLogin = true;
      });
      _onFail("Falha ao entrar.");
    }
  }

  void _confirmarReinicioSenha() {
    String cpf;
    _cpfController.text = '';
    // flutter defined function
    showDialog(
      context: context,
      builder: (BuildContext context) {
        // return object of type Dialog
        return AlertDialog(
          title: Text("Reiniciar a senha?"),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: <Widget>[
              Text("Uma nova senha será enviada para " +
                  _emailController.value.text),
              TextFormField(
                  controller: _cpfController,
                  decoration: InputDecoration(labelText: 'CPF'),
                  keyboardType: TextInputType.number)
            ],
          ),
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
                if (_cpfController.value.text != '') {
                  _reiniciarSenha();
                } else {
                  _onFail('CPF não informado');
                }
                Navigator.of(context).pop();
              },
            ),
          ],
        );
      },
    );
  }

  void _reiniciarSenha() async {
    setState(() {
      SystemChannels.textInput.invokeMethod('TextInput.hide');
      isLoading = true;
    });

    try {
      RetornoInclusao retornoInclusao = await api_helper.reinicarSenha(
          _emailController.value.text, _cpfController.value.text);

      if (retornoInclusao.erro == false) {
        setState(() {
          isLoading = false;
          _mensagem('Nova senha enviada.');
        });
      } else {
        _onFail(retornoInclusao.mensagem);
      }
    } catch (e) {
      isLoading = false;
      _onFail("Falha ao enviar nova senha.");
    }
  }

  Future<Null> _buscarDadosUsuario() async {
    final all = await _storage.readAll();
    setState(() {
      _items = all.keys
          .map((key) => new SecItem(key, all[key]))
          .toList(growable: false);
    });
    if (_items.isNotEmpty) {
      for (var item in _items) {
        if (item.key == 'senha') {
          usuario.senha1 = item.value;
        }
        if (item.key == 'email') {
          usuario.email = item.value;
        }
      }
      _loginUsusario(usuario);
    } else {
      setState(() {
        showLogin = true;
      });
    }
  }

  void _salvardadosUsuario() async {
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

  void _mensagem(String texto) {
    _scaffoldKey.currentState.showSnackBar(SnackBar(
      content: Text(texto),
      backgroundColor: Colors.green,
      duration: Duration(seconds: 2),
    ));
  }
}
