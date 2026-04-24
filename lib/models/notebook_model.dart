import 'package:flutter/material.dart';

class CodeCell {
  final String id;
  String code;
  String output;
  bool isRunning;
  DateTime createdAt;
  
  CodeCell({
    required this.id,
    this.code = '',
    this.output = '',
    this.isRunning = false,
    DateTime? createdAt,
  }) : createdAt = createdAt ?? DateTime.now();
}

class NotebookModel extends ChangeNotifier {
  List<CodeCell> _cells = [];
  int _nextId = 1;
  List<Map<String, String>> _variables = [];
  List<int> _searchResults = [];
  
  List<CodeCell> get cells => _cells;
  List<Map<String, String>> get variables => _variables;
  
  NotebookModel() {
    _addInitialCell();
  }
  
  void _addInitialCell() {
    _cells.add(CodeCell(id: 'cell_${_nextId++}', code: '# Welcome to Jupyter IDE!\n# Write Python code and press Run\n\nprint("Hello, World!")'));
    notifyListeners();
  }
  
  void addCell({String? afterId}) {
    final cell = CodeCell(id: 'cell_${_nextId++}');
    
    if (afterId != null) {
      final index = _cells.indexWhere((c) => c.id == afterId);
      _cells.insert(index + 1, cell);
    } else {
      _cells.add(cell);
    }
    notifyListeners();
  }
  
  void addCellAbove(String belowId) {
    final index = _cells.indexWhere((c) => c.id == belowId);
    final cell = CodeCell(id: 'cell_${_nextId++}');
    _cells.insert(index, cell);
    notifyListeners();
  }
  
  void deleteCell(String id) {
    if (_cells.length > 1) {
      _cells.removeWhere((c) => c.id == id);
      notifyListeners();
    }
  }
  
  void copyCell(String id) {
    final index = _cells.indexWhere((c) => c.id == id);
    final original = _cells[index];
    final newCell = CodeCell(
      id: 'cell_${_nextId++}',
      code: original.code,
    );
    _cells.insert(index + 1, newCell);
    notifyListeners();
  }
  
  void updateCode(String id, String newCode) {
    final cell = _cells.firstWhere((c) => c.id == id);
    cell.code = newCode;
    notifyListeners();
  }
  
  void updateOutput(String id, String output) {
    final cell = _cells.firstWhere((c) => c.id == id);
    cell.output = output;
    cell.isRunning = false;
    notifyListeners();
  }
  
  void setRunning(String id, bool running) {
    final cell = _cells.firstWhere((c) => c.id == id);
    cell.isRunning = running;
    notifyListeners();
  }
  
  void clearAllOutputs() {
    for (var cell in _cells) {
      cell.output = '';
    }
    notifyListeners();
  }
  
  void reorderCells(int oldIndex, int newIndex) {
    if (newIndex > oldIndex) newIndex--;
    final cell = _cells.removeAt(oldIndex);
    _cells.insert(newIndex, cell);
    notifyListeners();
  }
  
  void recordVariable(String code, String output) {
    // Simple variable extraction simulation
    if (code.contains('=') && !code.contains('==')) {
      final parts = code.split('=');
      if (parts.length == 2) {
        final varName = parts[0].trim();
        final varValue = parts[1].trim();
        _variables.add({
          'name': varName,
          'value': varValue,
          'type': _inferType(varValue),
        });
        notifyListeners();
      }
    }
  }
  
  String _inferType(String value) {
    if (int.tryParse(value) != null) return 'int';
    if (double.tryParse(value) != null) return 'float';
    if (value == 'True' || value == 'False') return 'bool';
    if (value.startsWith('[') && value.endsWith(']')) return 'list';
    if (value.startsWith('{') && value.endsWith('}')) return 'dict';
    return 'str';
  }
  
  void searchCells(String query) {
    if (query.isEmpty) {
      _searchResults.clear();
    } else {
      _searchResults = [];
      for (int i = 0; i < _cells.length; i++) {
        if (_cells[i].code.toLowerCase().contains(query.toLowerCase())) {
          _searchResults.add(i);
        }
      }
    }
    notifyListeners();
  }
  
  void clearSearch() {
    _searchResults.clear();
    notifyListeners();
  }
  
  List<CodeCell> exportNotebook() {
    return List.from(_cells);
  }
  
  void importNotebook(List<CodeCell> cells) {
    _cells = cells;
    _nextId = _cells.length + 1;
    notifyListeners();
  }
  
  List<Map<String, String>> getVariables() {
    return _variables;
  }
}
