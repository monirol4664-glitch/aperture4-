import 'dart:async';

class ExecutionService {
  static final ExecutionService _instance = ExecutionService._internal();
  factory ExecutionService() => _instance;
  ExecutionService._internal();
  
  // Simulated Python execution with support for common operations
  Future<String> executeCode(String code) async {
    await Future.delayed(const Duration(milliseconds: 100));
    
    try {
      final trimmed = code.trim();
      if (trimmed.isEmpty) return '';
      
      // Mathematical expressions
      if (trimmed.contains('+') && !trimmed.contains('+=')) {
        final parts = trimmed.split('+');
        if (parts.length == 2) {
          final a = double.tryParse(parts[0].trim());
          final b = double.tryParse(parts[1].trim());
          if (a != null && b != null) {
            return (a + b).toString();
          }
        }
      }
      
      if (trimmed.contains('-') && trimmed.indexOf('-') > 0) {
        final parts = trimmed.split('-');
        if (parts.length == 2) {
          final a = double.tryParse(parts[0].trim());
          final b = double.tryParse(parts[1].trim());
          if (a != null && b != null) {
            return (a - b).toString();
          }
        }
      }
      
      if (trimmed.contains('*')) {
        final parts = trimmed.split('*');
        if (parts.length == 2) {
          final a = double.tryParse(parts[0].trim());
          final b = double.tryParse(parts[1].trim());
          if (a != null && b != null) {
            return (a * b).toString();
          }
        }
      }
      
      if (trimmed.contains('/')) {
        final parts = trimmed.split('/');
        if (parts.length == 2) {
          final a = double.tryParse(parts[0].trim());
          final b = double.tryParse(parts[1].trim());
          if (a != null && b != null && b != 0) {
            return (a / b).toString();
          }
        }
      }
      
      // Print statements - simple version
      if (trimmed.startsWith('print(') && trimmed.endsWith(')')) {
        String content = trimmed.substring(6, trimmed.length - 1);
        // Remove quotes if present
        if (content.startsWith('"') && content.endsWith('"')) {
          content = content.substring(1, content.length - 1);
        }
        if (content.startsWith("'") && content.endsWith("'")) {
          content = content.substring(1, content.length - 1);
        }
        return content;
      }
      
      // Variable assignments
      if (trimmed.contains('=') && !trimmed.contains('==')) {
        return "✓ Variable assigned";
      }
      
      // len() function
      if (trimmed.startsWith('len(') && trimmed.endsWith(')')) {
        String content = trimmed.substring(4, trimmed.length - 1);
        // Remove quotes
        if (content.startsWith('"') && content.endsWith('"')) {
          content = content.substring(1, content.length - 1);
        }
        if (content.startsWith("'") && content.endsWith("'")) {
          content = content.substring(1, content.length - 1);
        }
        return content.length.toString();
      }
      
      // String operations
      if (trimmed.contains('.upper()')) {
        String before = trimmed.split('.')[0];
        return before.toUpperCase();
      }
      
      if (trimmed.contains('.lower()')) {
        String before = trimmed.split('.')[0];
        return before.toLowerCase();
      }
      
      // Loop simulation
      if (trimmed.startsWith('for ') || trimmed.startsWith('while ')) {
        return "Loop executed (simulated)";
      }
      
      // Function definition
      if (trimmed.startsWith('def ')) {
        return "Function defined";
      }
      
      // Default response
      if (trimmed.isNotEmpty) {
        return "✓ Code executed successfully";
      }
      
      return '';
      
    } catch (e) {
      return 'Error: ${e.toString()}';
    }
  }
}
