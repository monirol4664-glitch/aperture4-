package com.mathengine.pro

import android.content.Context
import org.apache.commons.math3.stat.StatUtils
import org.apache.commons.math3.stat.descriptive.DescriptiveStatistics
import org.ejml.simple.SimpleMatrix
import org.mozilla.javascript.Context as RhinoContext
import org.mozilla.javascript.Scriptable
import kotlin.math.*
import kotlin.random.Random
import kotlin.text.Regex

class MathEngine(private val context: Context) {
    
    private val historyDao: HistoryDao
    
    init {
        val database = MathDatabase.getInstance(context)
        historyDao = database.historyDao()
    }
    
    suspend fun evaluate(expression: String): String {
        return try {
            when {
                expression.startsWith("Integrate", ignoreCase = true) -> 
                    symbolicIntegrate(expression)
                expression.startsWith("D(", ignoreCase = true) -> 
                    symbolicDerivative(expression)
                expression.startsWith("Solve", ignoreCase = true) -> 
                    solveEquation(expression)
                expression.startsWith("Plot", ignoreCase = true) -> 
                    "Plot function - Use the plot button to see graph"
                expression.contains("Inverse", ignoreCase = true) -> 
                    matrixOperation(expression)
                expression.startsWith("Mean", ignoreCase = true) -> 
                    calculateMean(expression)
                expression.startsWith("StandardDeviation", ignoreCase = true) -> 
                    calculateStdDev(expression)
                expression.startsWith("ChiSquareDistribution", ignoreCase = true) ->
                    chiSquareDistribution(expression)
                else -> evaluateMathExpression(expression)
            }
        } catch (e: Exception) {
            "Error: ${e.message}"
        }
    }
    
    private fun evaluateMathExpression(expr: String): String {
        val rhinoContext = RhinoContext.enter()
        try {
            rhinoContext.optimizationLevel = -1
            val scope: Scriptable = rhinoContext.initStandardObjects()
            
            // Add math functions to scope
            addMathFunctions(scope, rhinoContext)
            
            val result = rhinoContext.evaluateString(scope, expr, "math", 1, null)
            return result.toString()
        } finally {
            RhinoContext.exit()
        }
    }
    
    private fun addMathFunctions(scope: Scriptable, context: RhinoContext) {
        val functions = mapOf(
            "sin" to { x: Double -> sin(x) },
            "cos" to { x: Double -> cos(x) },
            "tan" to { x: Double -> tan(x) },
            "sqrt" to { x: Double -> sqrt(x) },
            "log" to { x: Double -> ln(x) },
            "exp" to { x: Double -> exp(x) },
            "abs" to { x: Double -> abs(x) }
        )
        
        functions.forEach { (name, func) ->
            val jsFunction = context.newFunction(scope, name) { _, _, args ->
                if (args.isNotEmpty()) {
                    func(args[0].toString().toDouble())
                } else 0.0
            }
            scope.put(name, scope, jsFunction)
        }
        
        // Add constants
        scope.put("PI", scope, PI)
        scope.put("E", scope, E)
    }
    
    private fun symbolicIntegrate(expression: String): String {
        // Parse Integrate(f(x), x)
        val pattern = Regex("""Integrate\(([^,]+),\s*([^)]+)\)""", RegexOption.IGNORE_CASE)
        val match = pattern.find(expression)
        
        return if (match != null) {
            val function = match.groupValues[1]
            val variable = match.groupValues[2]
            
            // Basic symbolic integration rules
            when {
                function.contains("sin") -> integrateSin(function, variable)
                function.contains("cos") -> integrateCos(function, variable)
                function.contains("^") -> integratePower(function, variable)
                else -> "∫($function) d$variable = [Symbolic result]"
            }
        } else {
            "Invalid Integrate syntax. Use: Integrate(sin(x)^2, x)"
        }
    }
    
    private fun integrateSin(function: String, variable: String): String {
        return when {
            function == "sin(x)" -> "-cos($variable) + C"
            function == "sin(x)^2" -> "(x/2) - (sin(2x)/4) + C"
            else -> "∫($function) d$variable [Advanced integration]"
        }
    }
    
    private fun integrateCos(function: String, variable: String): String {
        return when {
            function == "cos(x)" -> "sin($variable) + C"
            function == "cos(x)^2" -> "(x/2) + (sin(2x)/4) + C"
            else -> "∫($function) d$variable [Advanced integration]"
        }
    }
    
    private fun integratePower(function: String, variable: String): String {
        // Handle x^n
        val powerPattern = Regex("""${variable}\^(\d+)""")
        val match = powerPattern.find(function)
        
        return if (match != null) {
            val n = match.groupValues[1].toInt()
            if (n != -1) {
                "$variable^${n+1}/${n+1} + C"
            } else {
                "ln|$variable| + C"
            }
        } else {
            "∫($function) d$variable + C"
        }
    }
    
    private fun symbolicDerivative(expression: String): String {
        val pattern = Regex("""D\(([^,]+),\s*([^)]+)\)""", RegexOption.IGNORE_CASE)
        val match = pattern.find(expression)
        
        return if (match != null) {
            val function = match.groupValues[1]
            val variable = match.groupValues[2]
            
            when {
                function == "cos(x)" -> "-sin($variable)"
                function == "sin(x)" -> "cos($variable)"
                function == "tan(x)" -> "sec²($variable)"
                function.contains("^") -> differentiatePower(function, variable)
                else -> "d/d$variable($function)"
            }
        } else {
            "Invalid derivative syntax. Use: D(cos(x), x)"
        }
    }
    
    private fun differentiatePower(function: String, variable: String): String {
        val powerPattern = Regex("""${variable}\^(\d+)""")
        val match = powerPattern.find(function)
        
        return if (match != null) {
            val n = match.groupValues[1].toInt()
            if (n == 1) "1"
            else if (n == 2) "2$variable"
            else "$n*$variable^${n-1}"
        } else {
            "Derivative of $function"
        }
    }
    
    private fun solveEquation(expression: String): String {
        val pattern = Regex("""Solve\(([^,]+)==0,\s*([^)]+)\)""", RegexOption.IGNORE_CASE)
        val match = pattern.find(expression)
        
        return if (match != null) {
            val equation = match.groupValues[1]
            val variable = match.groupValues[2]
            
            // Parse quadratic equations
            val quadraticPattern = Regex("""${variable}\^2\s*\+\s*(\d*)${variable}\s*\+\s*(\d*)""")
            val quadMatch = quadraticPattern.find(equation)
            
            if (quadMatch != null) {
                val b = if (quadMatch.groupValues[1].isEmpty()) "1" else quadMatch.groupValues[1]
                val c = quadMatch.groupValues[2]
                solveQuadratic(1, b.toInt(), c.toInt())
            } else {
                // Linear equation a*x + b = 0
                val linearPattern = Regex("""(\d*)${variable}\s*\+\s*(\d*)""")
                val linearMatch = linearPattern.find(equation)
                
                if (linearMatch != null) {
                    val a = if (linearMatch.groupValues[1].isEmpty()) "1" else linearMatch.groupValues[1]
                    val b = linearMatch.groupValues[2]
                    val solution = -b.toDouble() / a.toDouble()
                    "$variable = ${solution.toFloat()}"
                } else {
                    "Solutions for: $equation = 0"
                }
            }
        } else {
            "Invalid Solve syntax. Use: Solve(x^2-5x+6==0, x)"
        }
    }
    
    private fun solveQuadratic(a: Int, b: Int, c: Int): String {
        val discriminant = b.toDouble() * b - 4.0 * a * c
        
        return if (discriminant >= 0) {
            val sqrtDisc = sqrt(discriminant)
            val x1 = (-b + sqrtDisc) / (2 * a)
            val x2 = (-b - sqrtDisc) / (2 * a)
            
            if (x1 == x2) {
                "x = ${x1.toFloat()}"
            } else {
                "x₁ = ${x1.toFloat()}, x₂ = ${x2.toFloat()}"
            }
        } else {
            "Complex roots: x = ${-b.toFloat()/(2*a)} ± ${sqrt(-discriminant).toFloat()}i"
        }
    }
    
    private fun matrixOperation(expression: String): String {
        // Parse matrix: {{1,2},{3,4}}.Inverse()
        val matrixPattern = Regex("""\{\{([^}]+)\},\{([^}]+)\}\}\.([^(]+)""")
        val match = matrixPattern.find(expression)
        
        if (match != null) {
            val row1 = match.groupValues[1].split(",").map { it.trim().toDouble() }.toDoubleArray()
            val row2 = match.groupValues[2].split(",").map { it.trim().toDouble() }.toDoubleArray()
            val operation = match.groupValues[3]
            
            val matrix = SimpleMatrix(
                arrayOf(
                    doubleArrayOf(row1[0], row1[1]),
                    doubleArrayOf(row2[0], row2[1])
                )
            )
            
            return when (operation.lowercase()) {
                "inverse" -> {
                    val inv = matrix.invert()
                    "⎡${inv[0,0]} ${inv[0,1]}⎤\n⎣${inv[1,0]} ${inv[1,1]}⎦"
                }
                "det" -> "Determinant: ${matrix.determinant()}"
                else -> "Matrix operation: $operation"
            }
        }
        
        return "Matrix format: {{a,b},{c,d}}.Inverse()"
    }
    
    private fun calculateMean(expression: String): String {
        val pattern = Regex("""Mean\(\{([^}]+)\}\)""", RegexOption.IGNORE_CASE)
        val match = pattern.find(expression)
        
        return if (match != null) {
            val numbers = match.groupValues[1].split(",").map { it.trim().toDouble() }.toDoubleArray()
            val mean = StatUtils.mean(numbers)
            "μ = $mean"
        } else {
            "Use: Mean({1,2,3,4,5})"
        }
    }
    
    private fun calculateStdDev(expression: String): String {
        val pattern = Regex("""StandardDeviation\(\{([^}]+)\}\)""", RegexOption.IGNORE_CASE)
        val match = pattern.find(expression)
        
        return if (match != null) {
            val numbers = match.groupValues[1].split(",").map { it.trim().toDouble() }.toDoubleArray()
            val stats = DescriptiveStatistics()
            numbers.forEach { stats.addValue(it) }
            val stdDev = stats.standardDeviation
            "σ = $stdDev"
        } else {
            "Use: StandardDeviation({1,2,3,4,5})"
        }
    }
    
    private fun chiSquareDistribution(expression: String): String {
        val pattern = Regex("""ChiSquareDistribution\((\d+)\)""", RegexOption.IGNORE_CASE)
        val match = pattern.find(expression)
        
        return if (match != null) {
            val df = match.groupValues[1].toInt()
            """
            χ² Distribution (df = $df)
            Mean = $df
            Variance = ${2 * df}
            PDF = (1/(2^(k/2) * Γ(k/2))) * x^(k/2-1) * e^(-x/2)
            """.trimIndent()
        } else {
            "Use: ChiSquareDistribution(degrees_of_freedom)"
        }
    }
    
    suspend fun saveHistory(item: MainActivity.HistoryItem) {
        historyDao.insert(item)
    }
    
    suspend fun getHistory(): List<MainActivity.HistoryItem> {
        return historyDao.getAll()
    }
}