import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  Button,
  Alert,
  Chip,
  ToggleButton,
  ToggleButtonGroup,
  Paper,
} from '@mui/material';
import {
  Code as CodeIcon,
  Tune as TuneIcon,
} from '@mui/icons-material';

interface ConditionBuilderProps {
  value: string;
  onChange: (condition: string) => void;
}

interface SimpleCondition {
  variable: string;
  operator: string;
  value: string | boolean | number;
}

export function ConditionBuilder({ value, onChange }: ConditionBuilderProps) {
  const [mode, setMode] = useState<'simple' | 'advanced'>('simple');
  const [simpleCondition, setSimpleCondition] = useState<SimpleCondition>({
    variable: 'approved',
    operator: '==',
    value: true
  });

  // Parse existing JsonLogic to simple condition (if possible)
  useEffect(() => {
    const parsed = parseJsonLogic(value);
    if (parsed) {
      setSimpleCondition(parsed);
      setMode('simple');
    } else {
      setMode('advanced');
    }
  }, [value]);

  const parseJsonLogic = (jsonLogic: string): SimpleCondition | null => {
    try {
      const parsed = JSON.parse(jsonLogic);
      
      // Handle different operators
      for (const op of ['==', '!=', '>', '<', '>=', '<=']) {
        if (parsed[op] && Array.isArray(parsed[op]) && parsed[op].length === 2) {
          const [varObj, val] = parsed[op];
          if (varObj && typeof varObj === 'object' && varObj.var) {
            return {
              variable: varObj.var,
              operator: op,
              value: val
            };
          }
        }
      }
    } catch {
      return null;
    }
    return null;
  };

  const buildJsonLogic = (condition: SimpleCondition): string => {
    return JSON.stringify({
      [condition.operator]: [
        { var: condition.variable },
        condition.value
      ]
    });
  };

  const operators = [
    { value: '==', label: 'equals', symbol: '=' },
    { value: '!=', label: 'does not equal', symbol: '≠' },
    { value: '>', label: 'is greater than', symbol: '>' },
    { value: '<', label: 'is less than', symbol: '<' },
    { value: '>=', label: 'is greater than or equal to', symbol: '≥' },
    { value: '<=', label: 'is less than or equal to', symbol: '≤' },
  ];

  const commonVariables = [
    { value: 'approved', label: 'Approved (true/false)', type: 'boolean' },
    { value: 'status', label: 'Status (text)', type: 'string' },
    { value: 'amount', label: 'Amount (number)', type: 'number' },
    { value: 'priority', label: 'Priority (text)', type: 'string' },
    { value: 'category', label: 'Category (text)', type: 'string' },
    { value: 'result', label: 'Result (text)', type: 'string' },
    { value: 'count', label: 'Count (number)', type: 'number' },
  ];

  const handleSimpleConditionChange = (field: keyof SimpleCondition, newValue: any) => {
    const updated = { ...simpleCondition, [field]: newValue };
    setSimpleCondition(updated);
    onChange(buildJsonLogic(updated));
  };

  const handleModeChange = (newMode: 'simple' | 'advanced') => {
    if (newMode === 'simple') {
      // Try to parse current value
      const parsed = parseJsonLogic(value);
      if (parsed) {
        setSimpleCondition(parsed);
      } else {
        // Set a default simple condition
        const defaultCondition = { variable: 'approved', operator: '==', value: true };
        setSimpleCondition(defaultCondition);
        onChange(buildJsonLogic(defaultCondition));
      }
    }
    setMode(newMode);
  };

  const renderValueInput = () => {
    const selectedVar = commonVariables.find(v => v.value === simpleCondition.variable);
    
    if (selectedVar?.type === 'boolean') {
      return (
        <FormControl size="small" sx={{ minWidth: 100 }}>
          <Select
            value={simpleCondition.value.toString()}
            onChange={(e) => handleSimpleConditionChange('value', e.target.value === 'true')}
          >
            <MenuItem value="true">Yes (true)</MenuItem>
            <MenuItem value="false">No (false)</MenuItem>
          </Select>
        </FormControl>
      );
    }

    if (selectedVar?.type === 'number') {
      return (
        <TextField
          size="small"
          type="number"
          value={simpleCondition.value.toString()}
          onChange={(e) => handleSimpleConditionChange('value', Number(e.target.value) || 0)}
          placeholder="Enter number"
          sx={{ minWidth: 100 }}
        />
      );
    }

    return (
      <TextField
        size="small"
        value={simpleCondition.value.toString()}
        onChange={(e) => handleSimpleConditionChange('value', e.target.value)}
        placeholder="Enter value"
        sx={{ minWidth: 100 }}
      />
    );
  };

  return (
    <Box>
      <Box sx={{ mb: 2 }}>
        <ToggleButtonGroup
          value={mode}
          exclusive
          onChange={(_, newMode) => newMode && handleModeChange(newMode)}
          size="small"
        >
          <ToggleButton value="simple">
            <TuneIcon sx={{ mr: 1 }} />
            Simple
          </ToggleButton>
          <ToggleButton value="advanced">
            <CodeIcon sx={{ mr: 1 }} />
            Advanced
          </ToggleButton>
        </ToggleButtonGroup>
      </Box>

      {mode === 'simple' ? (
        <Box>
          <Typography variant="subtitle2" gutterBottom>
            🎯 Build condition visually
          </Typography>
          
          <Paper variant="outlined" sx={{ p: 2, mb: 2 }}>
            <Box sx={{ display: 'flex', gap: 1, alignItems: 'center', flexWrap: 'wrap' }}>
              <Typography>When</Typography>
              
              <FormControl size="small" sx={{ minWidth: 150 }}>
                <InputLabel>Variable</InputLabel>
                <Select
                  value={simpleCondition.variable}
                  label="Variable"
                  onChange={(e) => handleSimpleConditionChange('variable', e.target.value)}
                >
                  {commonVariables.map(variable => (
                    <MenuItem key={variable.value} value={variable.value}>
                      {variable.label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>

              <FormControl size="small" sx={{ minWidth: 120 }}>
                <InputLabel>Condition</InputLabel>
                <Select
                  value={simpleCondition.operator}
                  label="Condition"
                  onChange={(e) => handleSimpleConditionChange('operator', e.target.value)}
                >
                  {operators.map(op => (
                    <MenuItem key={op.value} value={op.value}>
                      {op.label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>

              {renderValueInput()}
            </Box>
          </Paper>

          <Alert severity="success" sx={{ mb: 2 }}>
            <Typography variant="body2">
              <strong>This means:</strong> When <strong>{simpleCondition.variable}</strong> {operators.find(op => op.value === simpleCondition.operator)?.label} <strong>{simpleCondition.value.toString()}</strong>, take the "True" path. Otherwise, take the "False" path.
            </Typography>
          </Alert>

          <Typography variant="caption" color="text.secondary">
            💡 Connect the gateway to two different paths - one for when this condition is true, one for when it's false.
          </Typography>
        </Box>
      ) : (
        <Box>
          <Typography variant="subtitle2" gutterBottom>
            ⚙️ Advanced JsonLogic expression
          </Typography>
          <TextField
            fullWidth
            multiline
            rows={4}
            value={value}
            onChange={(e) => onChange(e.target.value)}
            placeholder='{"==": [{"var": "approved"}, true]}'
            sx={{ fontFamily: 'monospace', mb: 1 }}
          />
          <Alert severity="info">
            <Typography variant="caption">
              Learn JsonLogic syntax at: <a href="https://jsonlogic.com/" target="_blank" rel="noopener">jsonlogic.com</a>
            </Typography>
          </Alert>
        </Box>
      )}
    </Box>
  );
}
