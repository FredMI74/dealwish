import 'package:dealwish/models/usuario_model.dart';
import 'package:dealwish/ui/desejos_page.dart';
import 'package:dealwish/ui/login_page.dart';
import 'package:dealwish/ui/usuario_page.dart';
import 'package:dealwish/ui/termo_servico_page.dart';
import 'package:dealwish/ui/trocar_senha_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  SystemChrome.setPreferredOrientations([DeviceOrientation.portraitUp]).then((_) {
    runApp(new MyApp());
  });
}

final Usuario usuario = Usuario();

class MyApp extends StatelessWidget {

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: "Dealwish",
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        primaryColor: Color.fromARGB(255, 255, 127, 0),

      ),
      home: LoginPage(),
      routes: <String, WidgetBuilder>{
        // Set routes for using the Navigator.
        '/desejos': (BuildContext context) => DesejosPage(),
        '/login': (BuildContext context) => LoginPage(),
        '/usuario': (BuildContext context) => UsuarioPage(),
        '/termoservico': (BuildContext context) => TermoServicoPage(),
        '/trocarsenha': (BuildContext context) => TrocarSenhaPage()
      },
    );
  }
}
