import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  Alert,
  Box,
  Typography,
  CircularProgress
} from '@mui/material';
import { apiClient } from '@/services/api.client.js';
import { useTenant } from '@/contexts/TenantContext.js';
import type { TenantDto, CreateAdditionalTenantDto } from '@/types/tenant.js'; // 🔧 ADD: Import types

interface CreateAdditionalTenantProps {
  open: boolean;
  onClose: () => void;
  onSuccess?: (tenant: TenantDto) => void; // 🔧 FIX: Use proper type instead of any
}

export function CreateAdditionalTenant({ open, onClose, onSuccess }: CreateAdditionalTenantProps) {
  const [tenantName, setTenantName] = useState('');
  const [tenantDomain, setTenantDomain] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { switchTenant } = useTenant();

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      // 🔧 FIX: Use proper typing for request and response
      const requestData: CreateAdditionalTenantDto = {
        tenantName,
        tenantDomain: tenantDomain || null
      };

      const response = await apiClient.post<TenantDto>('/api/tenants/create-additional', requestData);

      // 🔧 FIX: response.data is now properly typed as TenantDto
      if (response.data) {
        console.log('✅ Created tenant:', response.data);
        
        // Switch to the new tenant - response.data.id is now properly typed
        await switchTenant(response.data.id.toString());
        
        if (onSuccess) {
          onSuccess(response.data);
        }
        
        // Reset form
        setTenantName('');
        setTenantDomain('');
        onClose();
      }
    } catch (err: any) {
      console.error('❌ Failed to create additional tenant:', err);
      
      // 🔧 IMPROVED: Better error handling
      let errorMessage = 'Failed to create organization. Please try again.';
      
      if (err.response?.data) {
        // Handle .NET ApiResponseDto error structure
        if (typeof err.response.data === 'string') {
          errorMessage = err.response.data;
        } else if (err.response.data.message) {
          errorMessage = err.response.data.message;
        } else if (err.response.data.errors && err.response.data.errors.length > 0) {
          errorMessage = err.response.data.errors.map((e: any) => e.message || e).join(', ');
        }
      } else if (err.message) {
        errorMessage = err.message;
      }
      
      setError(errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    if (!isSubmitting) {
      setTenantName('');
      setTenantDomain('');
      setError(null);
      onClose();
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>
          <Typography variant="h5" component="h2">
            Create New Organization
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
            Create a new organization and get administrator access immediately.
          </Typography>
        </DialogTitle>
        
        <DialogContent>
          <Box sx={{ mt: 2, display: 'flex', flexDirection: 'column', gap: 3 }}>
            {error && (
              <Alert severity="error">{error}</Alert>
            )}
            
            <TextField
              label="Organization Name"
              value={tenantName}
              onChange={(e) => setTenantName(e.target.value)}
              required
              fullWidth
              disabled={isSubmitting}
              helperText="Enter the name for your new organization"
              autoFocus
            />
            
            <TextField
              label="Domain (optional)"
              value={tenantDomain}
              onChange={(e) => setTenantDomain(e.target.value)}
              fullWidth
              disabled={isSubmitting}
              helperText="Optional: Custom domain for your organization"
              placeholder="e.g., mycompany.com"
            />
          </Box>
        </DialogContent>
        
        <DialogActions sx={{ p: 3 }}>
          <Button 
            onClick={handleClose} 
            disabled={isSubmitting}
          >
            Cancel
          </Button>
          <Button 
            type="submit" 
            variant="contained" 
            disabled={isSubmitting || !tenantName.trim()}
            startIcon={isSubmitting ? <CircularProgress size={20} /> : null}
          >
            {isSubmitting ? 'Creating...' : 'Create Organization'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
}
