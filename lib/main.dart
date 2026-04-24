import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'screens/editor_screen.dart';
import 'models/notebook_model.dart';

void main() {
  runApp(const PythonIDEApp());
}

class PythonIDEApp extends StatelessWidget {
  const PythonIDEApp({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (_) => NotebookModel(),
      child: MaterialApp(
        title: 'Python IDE',
        theme: ThemeData.dark().copyWith(
          primaryColor: Colors.deepPurple,
          scaffoldBackgroundColor: Colors.grey[900],
          cardColor: Colors.grey[850],
        ),
        home: const EditorScreen(),
        debugShowCheckedModeBanner: false,
      ),
    );
  }
}
