//"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" --disable-web-security --disable-gpu --user-data-dir=~/chromeTemp
import 'package:dw_dashboard/models/usuario_model.dart';
import 'package:dw_dashboard/ui/login_page.dart';
import 'package:flutter/material.dart';
import 'package:dw_dashboard/ui/indicadores.dart';

void main() => runApp(MyApp());

final Usuario usuario = Usuario();

class MyApp extends StatelessWidget {

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: "Dealwish's Dashboard",
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        primaryColor: Color.fromARGB(255, 255, 127, 0),
        scaffoldBackgroundColor: Colors.white
      ),

      home: LoginPage(),
      routes: <String, WidgetBuilder>{
        // Set routes for using the Navigator.
        '/indicadores': (BuildContext context) => IndicadoresPage(),
        '/login': (BuildContext context) => LoginPage()
      },
    );
  }
}
