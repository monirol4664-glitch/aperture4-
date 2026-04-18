from kivy.app import App
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.textinput import TextInput
from kivy.uix.button import Button
from kivy.uix.label import Label
import sys
from io import StringIO

class PythonConsole(BoxLayout):
    def __init__(self, **kwargs):
        super().__init__(**kwargs)
        self.orientation = 'vertical'
        self.spacing = 10
        self.padding = 10
        
        self.code_input = TextInput(
            text='print("Hello from Android!")',
            multiline=True,
            size_hint=(1, 0.5),
            font_size='16sp',
            background_color=(0.1, 0.1, 0.1, 1),
            foreground_color=(0.9, 0.9, 0.9, 1),
        )
        
        button_layout = BoxLayout(size_hint=(1, 0.1), spacing=10)
        
        run_btn = Button(text='Run Code', background_color=(0.2, 0.6, 0.2, 1))
        run_btn.bind(on_press=self.execute_code)
        
        clear_btn = Button(text='Clear', background_color=(0.6, 0.2, 0.2, 1))
        clear_btn.bind(on_press=self.clear_output)
        
        button_layout.add_widget(run_btn)
        button_layout.add_widget(clear_btn)
        
        self.output = TextInput(
            text='Ready...',
            multiline=True,
            size_hint=(1, 0.35),
            readonly=True,
            background_color=(0.05, 0.05, 0.05, 1),
            foreground_color=(0.8, 0.8, 0.8, 1),
        )
        
        self.add_widget(self.code_input)
        self.add_widget(button_layout)
        self.add_widget(Label(text='Output:', size_hint=(1, 0.05), halign='left', bold=True))
        self.add_widget(self.output)
    
    def execute_code(self, instance):
        code = self.code_input.text
        old_stdout = sys.stdout
        redirected_output = StringIO()
        sys.stdout = redirected_output
        
        try:
            exec(code, {'__name__': '__main__'})
            output = redirected_output.getvalue()
            self.output.text = f"Output:\n{output}" if output else "Output:\n(No output)"
        except Exception as e:
            self.output.text = f"Error:\n{str(e)}"
        finally:
            sys.stdout = old_stdout
    
    def clear_output(self, instance):
        self.output.text = "Ready..."
        self.code_input.text = ""

class PythonAppApp(App):
    def build(self):
        return PythonConsole()

if __name__ == '__main__':
    PythonAppApp().run()