import 'package:flutter/material.dart';

class CodeEditor extends StatefulWidget {
  final String code;
  final Function(String) onChanged;
  
  const CodeEditor({
    super.key,
    required this.code,
    required this.onChanged,
  });
  
  @override
  State<CodeEditor> createState() => _CodeEditorState();
}

class _CodeEditorState extends State<CodeEditor> {
  late TextEditingController _controller;
  
  final List<String> _pythonKeywords = [
    'def', 'class', 'import', 'from', 'as', 'if', 'elif', 'else',
    'for', 'while', 'break', 'continue', 'return', 'yield', 'try',
    'except', 'finally', 'raise', 'with', 'lambda', 'and', 'or',
    'not', 'is', 'in', 'True', 'False', 'None', 'print', 'len',
    'range', 'str', 'int', 'float', 'list', 'dict', 'set', 'tuple',
  ];
  
  List<String> _suggestions = [];
  bool _showSuggestions = false;
  
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
    if (cursorPos < 0) {
      setState(() => _showSuggestions = false);
      return;
    }
    
    // Find current word
    int start = cursorPos;
    while (start > 0 && _isWordChar(text[start - 1])) {
      start--;
    }
    final currentWord = text.substring(start, cursorPos);
    
    if (currentWord.length >= 2) {
      final matches = _pythonKeywords
          .where((kw) => kw.toLowerCase().startsWith(currentWord.toLowerCase()))
          .toList();
      
      setState(() {
        _suggestions = matches.take(5).toList();
        _showSuggestions = matches.isNotEmpty;
      });
    } else {
      setState(() => _showSuggestions = false);
    }
  }
  
  bool _isWordChar(String char) {
    return RegExp(r'[a-zA-Z0-9_]').hasMatch(char);
  }
  
  void _insertSuggestion(String suggestion) {
    final text = _controller.text;
    final cursorPos = _controller.selection.baseOffset;
    
    // Find start of current word
    int start = cursorPos;
    while (start > 0 && _isWordChar(text[start - 1])) {
      start--;
    }
    
    final newText = text.substring(0, start) + suggestion + text.substring(cursorPos);
    _controller.text = newText;
    _controller.selection = TextSelection.collapsed(offset: start + suggestion.length);
    widget.onChanged(newText);
    setState(() => _showSuggestions = false);
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
          child: TextField(
            controller: _controller,
            maxLines: null,
            style: const TextStyle(
              fontFamily: 'monospace',
              fontSize: 14,
              color: Colors.white,
            ),
            decoration: const InputDecoration(
              border: InputBorder.none,
              contentPadding: EdgeInsets.all(12),
              hintText: '# Write Python code here',
              hintStyle: TextStyle(color: Colors.grey),
            ),
          ),
        ),
        if (_showSuggestions && _suggestions.isNotEmpty)
          Container(
            margin: const EdgeInsets.only(top: 4),
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: Colors.grey[800],
              borderRadius: BorderRadius.circular(8),
              border: Border.all(color: Colors.blueGrey),
            ),
            child: Wrap(
              spacing: 8,
              runSpacing: 4,
              children: _suggestions.map((suggestion) {
                return ActionChip(
                  label: Text(
                    suggestion,
                    style: const TextStyle(fontSize: 12),
                  ),
                  onPressed: () => _insertSuggestion(suggestion),
                  backgroundColor: Colors.deepPurple[800],
                  labelStyle: const TextStyle(color: Colors.white),
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
