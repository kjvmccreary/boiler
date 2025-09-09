import React from 'react';

// Simple mock replacement for HybridExpressionEditor.
// Props loosely matched; we only need value/onChange for tests.
const HybridExpressionEditor: React.FC<any> = ({ value, onChange, height = 100, placeholder }) => {
  return (
    <textarea
      aria-label="hybrid-editor"
      style={{ width: '100%', height, fontFamily: 'monospace' }}
      value={value}
      placeholder={placeholder}
      onChange={(e) => onChange(e.target.value)}
    />
  );
};

export default HybridExpressionEditor;
