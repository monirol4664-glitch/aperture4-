import React, { useState } from 'react';
import _ from 'lodash';

const TableGenerator = () => {
  const [csvData, setCsvData] = useState('x,y\n1,2\n2,4\n3,6\n4,8\n5,10');
  const [points, setPoints] = useState(10);
  const [tableData, setTableData] = useState(null);
  const [statistics, setStatistics] = useState(null);

  const generateData = (type) => {
    const data = [];
    const step = 10 / points;
    
    for (let i = 0; i <= points; i++) {
      const x = i * step;
      let y;
      
      switch(type) {
        case 'linear':
          y = 2 * x + 1;
          break;
        case 'quadratic':
          y = x * x - 5 * x + 6;
          break;
        case 'exponential':
          y = Math.exp(x / 5);
          break;
        case 'sinusoidal':
          y = Math.sin(x);
          break;
        case 'cubic':
          y = x * x * x - 3 * x;
          break;
        default:
          y = x;
      }
      
      data.push([x.toFixed(2), y.toFixed(4)]);
    }
    
    let csv = 'x,f(x)\n';
    data.forEach(row => {
      csv += row.join(',') + '\n';
    });
    
    setCsvData(csv);
    processData(csv);
  };

  const processData = (csv) => {
    try {
      const lines = csv.trim().split('\n');
      const headers = lines[0].split(',').map(h => h.trim());
      const data = [];
      
      for (let i = 1; i < lines.length; i++) {
        const values = lines[i].split(',').map(v => {
          const trimmed = v.trim();
          return isNaN(trimmed) ? trimmed : parseFloat(trimmed);
        });
        if (values.length === headers.length) {
          data.push(values);
        }
      }
      
      setTableData({ headers, data });
      
      // Calculate statistics
      const stats = {};
      headers.forEach((header, colIndex) => {
        const values = data.map(row => row[colIndex]).filter(v => typeof v === 'number' && !isNaN(v));
        if (values.length > 0) {
          const sum = _.sum(values);
          const mean = sum / values.length;
          const sorted = _.sortBy(values);
          const median = sorted[Math.floor(sorted.length / 2)];
          const variance = _.mean(values.map(v => Math.pow(v - mean, 2)));
          const std = Math.sqrt(variance);
          
          stats[header] = {
            mean: mean,
            median: median,
            std: std,
            min: _.min(values),
            max: _.max(values),
            sum: sum,
            count: values.length
          };
        }
      });
      
      setStatistics(stats);
    } catch (error) {
      console.error('Error processing data:', error);
    }
  };

  const handleFileUpload = (event) => {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => {
        setCsvData(e.target.result);
        processData(e.target.result);
      };
      reader.readAsText(file);
    }
  };

  const handleGenerate = () => {
    processData(csvData);
  };

  return (
    <div className="table-container">
      <div className="card">
        <div className="card-header">
          <h2><span className="icon">📊</span> Data Table Generator</h2>
        </div>
        <div className="card-body">
          <div className="button-group">
            <button className="btn-secondary" onClick={() => generateData('linear')}>
              📈 Linear
            </button>
            <button className="btn-secondary" onClick={() => generateData('quadratic')}>
              📉 Quadratic
            </button>
            <button className="btn-secondary" onClick={() => generateData('exponential')}>
              📊 Exponential
            </button>
            <button className="btn-secondary" onClick={() => generateData('sinusoidal')}>
              🎵 Sinusoidal
            </button>
            <button className="btn-secondary" onClick={() => generateData('cubic')}>
              📐 Cubic
            </button>
          </div>
          
          <div className="input-group">
            <label>Number of points: {points}</label>
            <input 
              type="range" 
              value={points} 
              onChange={(e) => setPoints(parseInt(e.target.value))}
              min="2"
              max="50"
              step="1"
            />
          </div>
          
          <div className="input-group">
            <label>CSV Data:</label>
            <textarea 
              value={csvData} 
              onChange={(e) => setCsvData(e.target.value)}
              rows={6}
              placeholder="x,y&#10;1,2&#10;2,4"
            />
          </div>
          
          <div className="input-group">
            <label>Or upload CSV file:</label>
            <input 
              type="file" 
              accept=".csv"
              onChange={handleFileUpload}
            />
          </div>
          
          <button className="btn-primary" onClick={handleGenerate}>
            <span className="icon">🔄</span> Generate Table
          </button>
        </div>
      </div>
      
      {tableData && (
        <div className="card">
          <div className="card-header">
            <h2><span className="icon">📋</span> Data Table</h2>
          </div>
          <div className="card-body">
            <div className="table-wrapper">
              <table className="data-table">
                <thead>
                  <tr>
                    {tableData.headers.map((header, idx) => (
                      <th key={idx}>{header}</th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {tableData.data.map((row, idx) => (
                    <tr key={idx}>
                      {row.map((cell, cellIdx) => (
                        <td key={cellIdx}>
                          {typeof cell === 'number' ? cell.toFixed(4) : cell}
                        </td>
                      ))}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}
      
      {statistics && Object.keys(statistics).length > 0 && (
        <div className="card">
          <div className="card-header">
            <h2><span className="icon">📈</span> Statistics</h2>
          </div>
          <div className="card-body">
            <div className="stats-grid">
              {Object.entries(statistics).map(([key, stats]) => (
                <div className="stat-card" key={key}>
                  <div className="stat-label">{key}</div>
                  <div className="stat-value">Mean: {stats.mean.toFixed(4)}</div>
                  <div>Median: {stats.median.toFixed(4)}</div>
                  <div>Std Dev: {stats.std.toFixed(4)}</div>
                  <div>Min: {stats.min.toFixed(4)}</div>
                  <div>Max: {stats.max.toFixed(4)}</div>
                  <div>Sum: {stats.sum.toFixed(4)}</div>
                  <div>Count: {stats.count}</div>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default TableGenerator;