package com.example.mathe

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import org.matheclipse.core.eval.ExprEvaluator
import org.matheclipse.core.interfaces.IExpr
import org.matheclipse.parser.client.SyntaxError
import org.matheclipse.parser.client.math.MathException

class MathViewModel : ViewModel() {
    private val evaluator = ExprEvaluator(false, 1000)  // Increased recursion limit [web:30]

    private val _history = MutableStateFlow<List<String>>(emptyList())
    val history: StateFlow<List<String>> = _history

    private val _result = MutableStateFlow("")
    val result: StateFlow<String> = _result

    fun evaluate(expression: String) {
        viewModelScope.launch {
            try {
                val res: IExpr = evaluator.eval(expression)
                val output = res.toString()
                _result.value = output
                _history.value = _history.value + "$expression = $output"
            } catch (e: SyntaxError) {
                _result.value = "Syntax Error: ${e.message}"
            } catch (e: MathException) {
                _result.value = "Math Error: ${e.message}"
            } catch (e: Exception) {
                _result.value = "Error: ${e.message}"
            }
        }
    }
}