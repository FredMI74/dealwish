import 'dart:async';
import 'package:dealwish/helpers/animator.dart';
import 'package:dealwish/main.dart';
import 'package:dealwish/models/desejo_model.dart';
import 'package:dealwish/ui/ofertas_page.dart';
import 'package:dealwish/ui/usuario_page.dart';
import 'package:flutter/material.dart';
import 'novo_desejo_grupo_page.dart';
import 'package:dealwish/helpers/api_helper.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:dealwish/helpers/hint.dart';

final _scaffoldKey = GlobalKey<ScaffoldState>();

class DesejosPage extends StatefulWidget {
  @override
  _DesejosPageState createState() => _DesejosPageState();
}

class _DesejosPageState extends State<DesejosPage> with WidgetsBindingObserver {
  AppLifecycleState _notification;
  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    setState(() {
      _notification = state;
      if (_notification.index == 0) {
        _atualizaDesejos();
      }
    });
  }

  ApiHelper api_helper = ApiHelper();
  final _storage = new FlutterSecureStorage();

  bool isLoading = true;

  @override
  void initState() {
    super.initState();

    WidgetsBinding.instance.addObserver(this);

    FirebaseMessaging.instance.requestPermission();

    FirebaseMessaging.onMessage.listen((RemoteMessage message) {
      RemoteNotification notification = message.notification;
      AndroidNotification android = message.notification?.android;
      if (notification != null && android != null) {
        _mensagem(notification.title);
        _atualizaDesejos(); //Ativos
      }
    });

    @override
    void dispose() {
      WidgetsBinding.instance.removeObserver(this);
      super.dispose();
    }

    _atualizaDesejos(); //Ativos
  }

  List<Desejo> desejos = List.empty(growable: true);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        drawer: Drawer(
            child: ListView(
          children: <Widget>[
            Container(
                color: Color.fromARGB(255, 255, 127, 0),
                height: 120.0,
                child: DrawerHeader(
                  child: Column(
                    children: <Widget>[
                      Text(
                        usuario.nome,
                        style: TextStyle(fontSize: 20.0, color: Colors.white),
                      ),
                      Divider(),
                      Text(
                        usuario.email,
                        style: TextStyle(color: Colors.white),
                      )
                    ],
                  ),
                )),
            ListTile(
              title: new Text('Meus dados'),
              onTap: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                        builder: (context) => UsuarioPage(novousuario: false)));
              },
            ),
            ListTile(
              title: new Text('Trocar senha'),
              onTap: () {
                Navigator.pushNamed(context, '/trocarsenha');
              },
            ),
            ListTile(
              title: new Text('Sair'),
              onTap: () {
                usuario.limpar();
                _storage.deleteAll();
                Navigator.pushNamedAndRemoveUntil(
                    context, '/login', ModalRoute.withName('/'));
              },
            ),
            Divider(),
            ListTile(
              title: new Text('Termo de serviço'),
              onTap: () {
                Navigator.pushNamed(context, '/termoservico');
              },
            ),
          ],
        )),
        key: _scaffoldKey,
        appBar: AppBar(
          iconTheme: IconThemeData(
            color: Colors.white, //change your color here
          ),
          centerTitle: true,
          title: Column(
            children: <Widget>[
              Container(
                padding: EdgeInsets.only(bottom: 6.0),
                child: Text("Dealwish",
                    style: TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                        fontSize: 20.0)),
              ),
              Text("Meus Desejos", style: TextStyle(color: Colors.white))
            ],
          ),
        ),
        body: Stack(
          children: <Widget>[
            Image.asset(
              "images/fundo.png",
              fit: BoxFit.fitHeight,
              height: 1000.0,
            ),
            FutureBuilder(
                future: _hintDesejo(),
                builder: (BuildContext context, AsyncSnapshot<bool> lida) {
                  return _hintincluirDesejo(lida.data);
                }),
            RefreshIndicator(
                onRefresh: _atualizaDesejos,
                child: _desejos(), //Ativos
                color: Color.fromARGB(255, 255, 127, 0))
          ],
        ),
        floatingActionButton: Opacity(
            opacity: 0.6,
            child: FloatingActionButton(
              onPressed: () {
                _showNovoDesejoGrupo();
              },
              child: Icon(Icons.add),
              backgroundColor: Color.fromARGB(255, 255, 127, 0),
            )));
  }

  Widget _desejos() {
    if (isLoading) {
      return Container(
          child: Center(
        child: CircularProgressIndicator(
          valueColor: new AlwaysStoppedAnimation<Color>(
              Color.fromARGB(255, 255, 127, 0)),
        ),
      ));
    } else {
      return ListView.builder(
          padding: EdgeInsets.all(10.0),
          itemCount: desejos.length,
          itemBuilder: (context, index) {
            return WidgetAnimator(_desejoCard(context, index));
          });
    }
  }

  Widget _desejoCard(BuildContext context, int index) {
    return GestureDetector(
      child: Card(
        child: Padding(
          padding: EdgeInsets.all(10.0),
          child: Row(
            children: <Widget>[
              Container(
                width: 80.0,
                height: 80.0,
                decoration: BoxDecoration(
                  shape: BoxShape.rectangle,
                  image: DecorationImage(
                      image: MemoryImage(desejos[index].icone_tp_produto)),
                ),
              ),
              Expanded(
                child: Container(
                  child: Padding(
                    padding: EdgeInsets.only(left: 10.0),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: <Widget>[
                        Text(
                          desejos[index].desc_tp_produto,
                          style: TextStyle(
                              fontSize: 16.0, fontWeight: FontWeight.bold),
                        ),
                        Text(
                          desejos[index].descricao,
                          style: TextStyle(fontSize: 22.0),
                          maxLines: 3,
                          overflow: TextOverflow.ellipsis,
                        ),
                        Text(
                          'ofertas: ' + desejos[index].qtd_ofertas.toString(),
                          style: TextStyle(fontSize: 12.0),
                          maxLines: 3,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ],
                    ),
                  ),
                ),
              )
            ],
          ),
        ),
      ),
      onTap: () {
        _showOferta(i: index);
      },
    );
  }

  void _showOferta({int i}) async {
    setState(() {
      isLoading = true;
    });
    await _atualizaListaDesejos();
    setState(() {
      isLoading = false;
    });
    final recContact = await Navigator.push(context,
        MaterialPageRoute(builder: (context) => Ofertas(desejo: desejos[i])));
  }

  void _showNovoDesejoGrupo() async {
    bool lida = await _hintDesejo();
    if (!lida) {
      _salvarhintDesejo();
    }
    final recContact = await Navigator.push(
        context, MaterialPageRoute(builder: (context) => NovoDesejoGrupo()));
  }

  Future<Null> _atualizaDesejos() {
    int id_situacao = 1; //Ativo
    return api_helper.consultarDesejos(id_situacao).then((list) {
      setState(() {
        isLoading = false;
        desejos = list;
      });
    }).catchError((e) {
      _onFail('Falha ao carregar lista.');
    });
  }

  Future<Null> _atualizaListaDesejos() {
    int id_situacao = 1; //Ativo
    return api_helper.consultarDesejos(id_situacao).then((list) {
      isLoading = false;
      desejos = list;
    }).catchError((e) {
      _onFail('Falha ao carregar lista.');
    });
  }

  void _mensagem(String texto) {
    _scaffoldKey.currentState.showSnackBar(SnackBar(
      content: Text(
        texto,
        style: TextStyle(color: Colors.white),
      ),
      backgroundColor: Color.fromARGB(255, 255, 127, 0),
      duration: Duration(seconds: 2),
    ));
  }

  Widget _hintincluirDesejo(bool lida) {
    if (desejos.length == 0 && !(lida ?? false) && !isLoading) {
      return Container(
          alignment: Alignment.bottomCenter,
          child: hint(
            line1: 'Começe por aqui para incluir',
            line2: 'o seu desejo de compra.',
            width: 240.0,
            padding: EdgeInsets.fromLTRB(0.0, 0.0, 20.0, 15.0),
          ));
    } else {
      return Container();
    }
  }

  Future _salvarhintDesejo() async {
    await _storage.write(key: 'hint_desejo', value: 'S');
  }

  Future<bool> _hintDesejo() async {
    String lida;
    lida = await _storage.read(key: 'hint_desejo');
    return lida == 'S' ?? false;
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
