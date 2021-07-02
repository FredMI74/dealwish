import 'package:dealwish/helpers/api_helper.dart';
import 'package:dealwish/models/empresa_model.dart';
import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';

final _scaffoldKey = GlobalKey<ScaffoldState>();

class Empresa extends StatefulWidget {
  @override
  _EmpresaPageState createState() => _EmpresaPageState();

  int id_empresa;
  String url_oferta;

  Empresa(this.id_empresa, this.url_oferta);
}

class _EmpresaPageState extends State<Empresa> {
  ApiHelper api_helper = ApiHelper();
  bool isLoading = false;
  List<EmpresaModel> lista_empresas = List.empty(growable: true);

  @override
  void initState() {
    super.initState();

    setState(() {
      isLoading = true;
    });

    api_helper.consultarEmpresas(widget.id_empresa).then((list) {
      setState(() {
        isLoading = false;
        lista_empresas = list;
      });
    }).catchError((e) {
      _onFail('Falha ao carregar info. empresa.');
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: _scaffoldKey,
      appBar: AppBar(
          iconTheme: IconThemeData(
            color: Colors.white, //change your color here
          ),
          centerTitle: true,
          title:
              Text(lista_empresas[0].fantasia, style: TextStyle(color: Colors.white))),
      body: _empresacard(),
    );
  }

  Widget _empresacard() {
    return Padding(
        padding: EdgeInsets.all(5.0),
        child: Card(
            child: Padding(
                padding: EdgeInsets.all(10.0),
                child: Row(
                  children: <Widget>[
                    Expanded(
                      child: Container(
                        child: Padding(
                          padding: EdgeInsets.all(0.0),
                          child: _dados_empresa(),
                        ),
                      ),
                    ),
                  ],
                )
            )
        )
    );
  }

  Widget _dados_empresa() {
    return Padding(
        padding: EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: <Widget>[
            widget.id_empresa != 1 ? _info_empresa() : Container(), //1.Dealwish
            _link_oferta(),
    ],
    ));
  }

  Widget _link_oferta() {
    return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children:[
            Text('Link da oferta', textAlign: TextAlign.left, style: TextStyle(fontWeight: FontWeight.bold)),
            Text(widget.url_oferta)
          ]
    );
  }

  Widget _info_empresa() {
    return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children:[
          Text('Razão Social', textAlign: TextAlign.left, style: TextStyle(fontWeight: FontWeight.bold)),
          Text(lista_empresas[0].razao_social + '\n'),

          Text('CNPJ', textAlign: TextAlign.left, style: TextStyle(fontWeight: FontWeight.bold)),
          Text(lista_empresas[0].cnpj + '\n'),

          Text('Insc. Est.', textAlign: TextAlign.left, style: TextStyle(fontWeight: FontWeight.bold)),
          Text(lista_empresas[0].insc_est + '\n'),

          Text('Site', textAlign: TextAlign.left, style: TextStyle(fontWeight: FontWeight.bold)),
          GestureDetector(
            onTap: () { _launchURL(lista_empresas[0].url); },
            child: Text(lista_empresas[0].url + '\n', style: TextStyle(color: Colors.blue),),
          ),

          Text('e-mail', textAlign: TextAlign.left, style: TextStyle(fontWeight: FontWeight.bold)),
          GestureDetector(
            onTap: () { _launchURL('mailto:'+lista_empresas[0].email_sac); },
            child: Text(lista_empresas[0].email_sac + '\n', style: TextStyle(color: Colors.blue),),
          ),

          Text('SAC', textAlign: TextAlign.left, style: TextStyle(fontWeight: FontWeight.bold)),
          GestureDetector(
            onTap: () { _launchURL('tel:'+lista_empresas[0].fone_sac); },
            child: Text(lista_empresas[0].fone_sac + '\n', style: TextStyle(color: Colors.blue),),
          )
        ]
    );
  }

  _launchURL(String url) async {
    if (await canLaunch(url)) {
      await launch(url);
    } else {
      throw 'Could not launch $url';
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
