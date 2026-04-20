import React, { useState } from 'react';

function App() {
  const [a, setA] = useState('');
  const [b, setB] = useState('');
  const [c, setC] = useState('');
  const [result, setResult] = useState('');

  const solve = () => {
    const numA = parseFloat(a);
    const numB = parseFloat(b);
    const numC = parseFloat(c);

    if (isNaN(numA) || isNaN(numB) || isNaN(numC)) {
      setResult('Enter valid numbers');
      return;
    }

    if (numA === 0) {
      setResult('a cannot be zero');
      return;
    }

    const d = numB * numB - 4 * numA * numC;

    if (d < 0) {
      setResult('No real solutions');
    } else if (d === 0) {
      const x = -numB / (2 * numA);
      setResult(`x = ${x.toFixed(4)}`);
    } else {
      const x1 = (-numB + Math.sqrt(d)) / (2 * numA);
      const x2 = (-numB - Math.sqrt(d)) / (2 * numA);
      setResult(`x₁ = ${x1.toFixed(4)}\nx₂ = ${x2.toFixed(4)}`);
    }
  };

  return (
    <div style={{ padding: 20, fontFamily: 'Arial' }}>
      <h1>🧮 Math Tools</h1>
      <h3>Quadratic Solver: ax² + bx + c = 0</h3>
      
      <input 
        type="number" 
        placeholder="a" 
        value={a} 
        onChange={e => setA(e.target.value)}
        style={{ display: 'block', margin: 10, padding: 10, width: 200 }}
      />
      <input 
        type="number" 
        placeholder="b" 
        value={b} 
        onChange={e => setB(e.target.value)}
        style={{ display: 'block', margin: 10, padding: 10, width: 200 }}
      />
      <input 
        type="number" 
        placeholder="c" 
        value={c} 
        onChange={e => setC(e.target.value)}
        style={{ display: 'block', margin: 10, padding: 10, width: 200 }}
      />
      
      <button onClick={solve} style={{ margin: 10, padding: 10, width: 200, background: 'blue', color: 'white' }}>
        Solve
      </button>
      
      {result && (
        <pre style={{ margin: 20, fontSize: 16 }}>{result}</pre>
      )}
    </div>
  );
}

export default App;