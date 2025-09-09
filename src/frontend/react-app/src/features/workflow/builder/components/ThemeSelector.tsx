import React from 'react';
import { Box, ToggleButton, ToggleButtonGroup, Tooltip } from '@mui/material';
import ColorLensIcon from '@mui/icons-material/ColorLens';
import Brightness4Icon from '@mui/icons-material/Brightness4';
import Brightness7Icon from '@mui/icons-material/Brightness7';
import ContrastIcon from '@mui/icons-material/Contrast';
import { useExpressionSettings } from '../context/ExpressionSettingsContext';

export const ThemeSelector: React.FC = () => {
  const { theme, setTheme } = useExpressionSettings();

  return (
    <Box
      sx={{
        display: 'flex',
        alignItems: 'center',
        bgcolor: 'background.paper',
        border: '1px solid',
        borderColor: 'divider',
        borderRadius: 1,
        px: 1,
        py: 0.25,
        boxShadow: 1
      }}
    >
      <ToggleButtonGroup
        exclusive
        size="small"
        value={theme}
        onChange={(_, val) => val && setTheme(val)}
        aria-label="Monaco theme selection"
      >
        <ToggleButton value="system" aria-label="System">
          <Tooltip title="System (auto)"><ColorLensIcon fontSize="small" /></Tooltip>
        </ToggleButton>
        <ToggleButton value="light" aria-label="Light">
          <Tooltip title="Light"><Brightness7Icon fontSize="small" /></Tooltip>
        </ToggleButton>
        <ToggleButton value="dark" aria-label="Dark">
          <Tooltip title="Dark"><Brightness4Icon fontSize="small" /></Tooltip>
        </ToggleButton>
        <ToggleButton value="hc" aria-label="High Contrast">
          <Tooltip title="High Contrast"><ContrastIcon fontSize="small" /></Tooltip>
        </ToggleButton>
      </ToggleButtonGroup>
    </Box>
  );
};
