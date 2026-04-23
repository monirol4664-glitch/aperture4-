package com.example.mathe

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.example.mathe.ui.theme.MathEngineTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            MathEngineTheme {
                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = MaterialTheme.colorScheme.background
                ) {
                    MathEngineApp()
                }
            }
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun MathEngineApp(viewModel: MathViewModel = viewModel()) {
    var input by remember { mutableStateOf("") }
    val result by viewModel.result.collectAsState()
    val history by viewModel.history.collectAsState()

    Column(modifier = Modifier
        .fillMaxSize()
        .padding(16.dp)) {
        Text("Math Engine (Symja)", style = MaterialTheme.typography.headlineMedium)

        // Input
        OutlinedTextField(
            value = input,
            onValueChange = { input = it },
            label = { Text("Enter math (e.g., Integrate(sin(x),x), Solve(x^2-2==0))") },
            modifier = Modifier.fillMaxWidth().height(80.dp),
            maxLines = 3
        )

        Spacer(Modifier.height(8.dp))

        Button(
            onClick = {
                if (input.isNotBlank()) {
                    viewModel.evaluate(input)
                    input = ""
                }
            },
            modifier = Modifier.fillMaxWidth()
        ) {
            Text("Evaluate")
        }

        Spacer(Modifier.height(16.dp))

        // Result
        Card(modifier = Modifier.fillMaxWidth()) {
            Column(modifier = Modifier.padding(16.dp)) {
                Text("Result:", style = MaterialTheme.typography.titleMedium)
                Text(result, style = MaterialTheme.typography.bodyLarge)
            }
        }

        Spacer(Modifier.height(16.dp))

        // History
        Card(modifier = Modifier.fillMaxWidth().weight(1f)) {
            Column(modifier = Modifier
                .fillMaxSize()
                .verticalScroll(rememberScrollState())
                .padding(16.dp)) {
                Text("History:", style = MaterialTheme.typography.titleMedium)
                history.forEach { entry ->
                    Text(entry, modifier = Modifier.padding(vertical = 2.dp))
                }
            }
        }
    }
}