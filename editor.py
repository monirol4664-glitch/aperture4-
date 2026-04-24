"""
Custom Code Editor with Syntax Highlighting and Auto-suggestions
"""

from kivy.uix.textinput import TextInput
from kivy.clock import Clock
from kivy.core.window import Window
from kivy.uix.popup import Popup
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.button import Button
from kivy.graphics import Color, Rectangle
import re


class CodeEditor(TextInput):
    """Enhanced code editor with syntax highlighting and suggestions"""
    
    def __init__(self, autocompleter=None, **kwargs):
        super().__init__(**kwargs)
        self.autocompleter = autocompleter
        self.suggestion_popup = None
        self.current_suggestions = []
        self.tab_width = 4
        self.font_name = 'RobotoMono'
        self.font_size = 14
        self.background_color = (0.15, 0.15, 0.18, 1)
        self.foreground_color = (0.9, 0.9, 0.9, 1)
        self.cursor_color = (0.8, 0.8, 0.2, 1)
        self.selection_color = (0.3, 0.4, 0.6, 1)
        
        # Syntax highlighting patterns
        self.keywords = {
            'and', 'or', 'not', 'if', 'elif', 'else', 'for', 'while', 'break',
            'continue', 'return', 'def', 'class', 'import', 'from', 'as', 'with',
            'try', 'except', 'finally', 'raise', 'assert', 'lambda', 'yield',
            'True', 'False', 'None', 'global', 'nonlocal', 'is', 'in'
        }
        
        self.builtins = {
            'print', 'len', 'range', 'str', 'int', 'float', 'bool', 'list',
            'dict', 'set', 'tuple', 'open', 'file', 'input', 'abs', 'sum',
            'min', 'max', 'sorted', 'enumerate', 'zip', 'map', 'filter'
        }
        
        self.bind(text=self.on_text_change)
        self.bind(cursor=self.on_cursor_move)
    
    def on_text_change(self, instance, value):
        """Handle text changes and show suggestions"""
        # Get current word being typed
        cursor_pos = self.cursor_col
        lines = value.split('\n')
        current_line = lines[self.cursor_row] if self.cursor_row < len(lines) else ""
        
        # Split line into words
        words = re.findall(r'\b\w+\b', current_line[:cursor_pos])
        if words:
            current_word = words[-1]
            if len(current_word) >= 2 and self.autocompleter:
                suggestions = self.autocompleter.get_suggestions(current_word)
                if suggestions and suggestions != self.current_suggestions:
                    self.current_suggestions = suggestions
                    self.show_suggestions(suggestions)
                elif not suggestions:
                    self.hide_suggestions()
    
    def on_cursor_move(self, instance, value):
        """Hide suggestions when cursor moves"""
        self.hide_suggestions()
    
    def show_suggestions(self, suggestions):
        """Show suggestion popup below cursor"""
        if not suggestions:
            return
        
        if self.suggestion_popup:
            self.suggestion_popup.dismiss()
        
        # Create popup content
        layout = BoxLayout(orientation='vertical', size_hint=(None, None), width=200)
        
        for suggestion in suggestions[:5]:
            btn = Button(text=suggestion, size_hint_y=None, height=40)
            btn.bind(on_press=lambda x, s=suggestion: self.insert_suggestion(s))
            layout.add_widget(btn)
        
        # Calculate popup position
        cursor_pos = self.get_cursor_pos()
        
        popup = Popup(
            title='Suggestions',
            content=layout,
            size_hint=(None, None),
            size=(200, min(250, len(suggestions) * 45)),
            pos_hint={'x': 0.1, 'y': 0.5}  # Will adjust in on_open
        )
        
        # Position popup near cursor
        def position_popup(instance):
            instance.pos = (cursor_pos[0] + 50, cursor_pos[1] - 100)
        
        popup.bind(on_open=position_popup)
        popup.open()
        self.suggestion_popup = popup
    
    def hide_suggestions(self):
        """Dismiss suggestion popup"""
        if self.suggestion_popup:
            self.suggestion_popup.dismiss()
            self.suggestion_popup = None
    
    def insert_suggestion(self, suggestion):
        """Insert selected suggestion"""
        # Get current line and word
        lines = self.text.split('\n')
        current_line = lines[self.cursor_row]
        cursor_pos = self.cursor_col
        
        # Find start of current word
        word_start = cursor_pos
        while word_start > 0 and current_line[word_start - 1].isalnum():
            word_start -= 1
        
        # Replace current word with suggestion
        new_line = current_line[:word_start] + suggestion + current_line[cursor_pos:]
        lines[self.cursor_row] = new_line
        self.text = '\n'.join(lines)
        
        # Move cursor to end of inserted word
        self.cursor = (self.cursor_row, word_start + len(suggestion))
        
        self.hide_suggestions()
    
    def keyboard_on_key_down(self, window, keycode, text, modifiers):
        """Handle Tab key for indentation"""
        if keycode[1] == 'tab':
            self.insert_text(' ' * self.tab_width)
            return True
        return super().keyboard_on_key_down(window, keycode, text, modifiers)
    
    def get_cursor_pos(self):
        """Get cursor position in screen coordinates"""
        # This is a simplified version - actual implementation would need
        # to calculate based on font size and cursor position
        return (100, 200)  # Placeholder