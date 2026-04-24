import 'package:flutter/material.dart';
import 'package:flutter_highlight/flutter_highlight.dart';
import 'package:flutter_highlight/themes/monokai-sublime.dart';

class CodeEditor extends StatefulWidget {
  final String code;
  final Function(String) onChanged;
  final List<String> suggestions;
  final Function(String) onSuggestionSelected;
  
  const CodeEditor({
    super.key,
    required this.code,
    required this.onChanged,
    this.suggestions = const [],
    required this.onSuggestionSelected,
  });
  
  @override
  State<CodeEditor> createState() => _CodeEditorState();
}

class _CodeEditorState extends State<CodeEditor> {
  late TextEditingController _controller;
  bool _showSuggestions = false;
  
  final List<String> _pythonKeywords = [
    'def', 'class', 'import', 'from', 'as', 'if', 'elif', 'else',
    'for', 'while', 'break', 'continue', 'return', 'yield', 'try',
    'except', 'finally', 'raise', 'with', 'lambda', 'and', 'or',
    'not', 'is', 'in', 'True', 'False', 'None', 'print', 'len',
    'range', 'str', 'int', 'float', 'list', 'dict', 'set', 'tuple',
    'open', 'file', 'input', 'abs', 'sum', 'min', 'max', 'sorted',
  ];
  
  @override
  void initState() {
    super.initState();
    _controller = TextEditingController(text: widget.code);
    _controller.addListener(_onTextChange);
  }
  
  void _onTextChange() {
    widget.onChanged(_controller.text);
    _checkForSuggestions();
  }
  
  void _checkForSuggestions() {
    final text = _controller.text;
    final cursorPos = _controller.selection.baseOffset;
    if (cursorPos < 0) return;
    
    final currentWord = _getCurrentWord(text, cursorPos);
    
    if (currentWord.length >= 2) {
      final matches = _pythonKeywords
          .where((kw) => kw.startsWith(currentWord.toLowerCase()))
          .toList();
      
      setState(() {
        widget.suggestions.clear();
        widget.suggestions.addAll(matches.take(5));
        _showSuggestions = matches.isNotEmpty;
      });
    } else {
      setState(() => _showSuggestions = false);
    }
  }
  
  String _getCurrentWord(String text, int position) {
    int start = position;
    while (start > 0 && RegExp(r'[a-zA-Z0-9_]').hasMatch(text[start - 1])) {
      start--;
    }
    return text.substring(start, position);
  }
  
  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Container(
          decoration: BoxDecoration(
            color: Colors.grey[900],
            borderRadius: BorderRadius.circular(8),
            border: Border.all(color: Colors.grey[700]!),
          ),
          child: HighlightView(
            _controller.text,
            language: 'python',
            theme: monokaiSublimeTheme,
            textStyle: const TextStyle(fontFamily: 'monospace', fontSize: 14),
            onChanged: (code) {
              _controller.text = code;
              widget.onChanged(code);
            },
          ),
        ),
        if (_showSuggestions && widget.suggestions.isNotEmpty)
          Container(
            margin: const EdgeInsets.only(top: 4),
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: Colors.grey[800],
              borderRadius: BorderRadius.circular(8),
            ),
            child: Wrap(
              spacing: 8,
              children: widget.suggestions.map((suggestion) {
                return ActionChip(
                  label: Text(suggestion),
                  onPressed: () => widget.onSuggestionSelected(suggestion),
                  backgroundColor: Colors.blue[900],
                );
              }).toList(),
            ),
          ),
      ],
    );
  }
  
  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }
}
