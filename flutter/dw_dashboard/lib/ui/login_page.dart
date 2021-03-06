import 'package:dw_dashboard/helpers/api_helper.dart';
import 'package:dw_dashboard/main.dart';
import 'package:flutter/material.dart';
import 'package:dw_dashboard/models/usuario_model.dart';
import 'package:flutter/services.dart';

class LoginPage extends StatefulWidget {
  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {

  @override
  void initState() {
    super.initState();
  }

  ApiHelper api_helper = ApiHelper();


  final _emailController = TextEditingController();
  final _senhaController = TextEditingController();

  bool isLoading = false;

  final _formKey = GlobalKey<FormState>();
  final _scaffoldKey = GlobalKey<ScaffoldState>();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      resizeToAvoidBottomPadding: false,
      key: _scaffoldKey,
      appBar: AppBar(
          centerTitle: true,
          title: Text("Dealwish - Indicadores", style: TextStyle(color: Colors.white))),
      body: Stack(
        children: <Widget>[
          Image.asset(
            "images/fundo.png",
            fit: BoxFit.fitHeight,
            height: 1000.0,
          ),
          _formulario(),
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
              Padding(
                padding: EdgeInsets.only(top: 20.0),
                child: Column(
                  children: <Widget>[Container(
                      width: 300.0,
                      child:TextFormField(
                        controller: _emailController,
                        decoration: InputDecoration(labelText: "e-mail"),
                        keyboardType: TextInputType.emailAddress,
                        validator: (value) {
                          if (value.isEmpty) {
                            return "informe o e-mail";
                          }
                        },
                      )
                  )
                    ,
                    Container(
                       width: 300.0,
                        child: TextFormField(
                      controller: _senhaController,
                      decoration: InputDecoration(labelText: "senha"),
                      keyboardType: TextInputType.text,
                      obscureText: true,
                      validator: (value) {
                        if (value.isEmpty) {
                          return "informe a senha";
                        }
                      },
                    )),
                  ],
                ),
              ),
              Padding(
                  padding: EdgeInsets.only(top: 20.0),
                  child: Column(
                    children: <Widget>[
                      RaisedButton(
                        child: Text("Entrar",
                            style: TextStyle(color: Colors.white)),
                        color: Color.fromARGB(255, 255, 127, 0),
                        onPressed: () {
                          if (_formKey.currentState.validate()) {
                            usuario.email = _emailController.value.text;
                            usuario.senha1 = _senhaController.value.text;
                            _loginUsusario(usuario);
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

       usuario = await api_helper.loginUsuario(usuario.email, usuario.senha1);

      if (usuario.token != null && usuario.token.isNotEmpty) {
        isLoading = false;
        Navigator.of(context).pushReplacementNamed('/indicadores');
      } else {
        setState(() {
          isLoading = false;
        });
        _onFail(usuario.mensagem);
      }
    } catch (e) {
      setState(() {
        isLoading = false;
      });
      _onFail("Falha ao entrar" + e.toString());
    }
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

}
