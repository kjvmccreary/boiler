import React from 'react';
import { Box, FormControlLabel, Switch, Tooltip, Typography } from '@mui/material';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import { useExpressionSettings } from '../context/ExpressionSettingsContext';

export const SemanticValidationToggle: React.FC = () => {
  const { semanticEnabled, toggleSemantic } = useExpressionSettings();
  return (
    <Box
      sx={{
        display: 'flex',
        alignItems: 'center',
        gap: 0.5,
        px: 1,
        py: 0.5,
        bgcolor: 'background.paper',
        border: '1px solid',
        borderColor: 'divider',
        borderRadius: 1,
        boxShadow: 1
      }}
    >
      <FormControlLabel
        sx={{
          m: 0,
          '& .MuiFormControlLabel-label': { fontSize: '0.75rem', whiteSpace: 'nowrap' }
        }}
        control={
          <Switch
            size="small"
            checked={semanticEnabled}
            onChange={toggleSemantic}
            inputProps={{ 'aria-label': 'Semantic validation toggle' }}
          />
        }
        label={semanticEnabled ? 'Semantic ON' : 'Semantic OFF'}
      />
      <Tooltip
        title={
          <Typography variant="caption">
            Semantic validation calls backend for operator & logic checks.<br />
            Disable to reduce network usage or work offline.
          </Typography>
        }
        arrow
      >
        <InfoOutlinedIcon fontSize="inherit" sx={{ fontSize: 14, color: 'text.secondary' }} />
      </Tooltip>
    </Box>
  );
};
