import { useState } from 'react';
import {
  Button,
  IconButton,
  Tooltip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  CircularProgress,
  type ButtonProps,
} from '@mui/material';
import {
  Logout as LogoutIcon,
  ExitToApp as ExitToAppIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext.js';
import { ROUTES } from '@/routes/route.constants.js';
import toast from 'react-hot-toast';

type LogoutVariant = 'button' | 'icon' | 'text';

interface LogoutButtonProps extends Omit<ButtonProps, 'onClick' | 'variant' | 'onError'> {
  /**
   * Display variant of the logout button
   */
  variant?: LogoutVariant;
  
  /**
   * Show confirmation dialog before logging out
   */
  showConfirmation?: boolean;
  
  /**
   * Redirect path after logout (defaults to login page)
   */
  redirectTo?: string;
  
  /**
   * Custom logout icon
   */
  icon?: React.ReactNode;
  
  /**
   * Button text (for button and text variants)
   */
  children?: React.ReactNode;
  
  /**
   * Tooltip text (for icon variant)
   */
  tooltip?: string;
  
  /**
   * Callback fired after successful logout
   */
  onLogout?: () => void;
  
  /**
   * Callback fired if logout fails
   */
  onError?: (error: Error) => void;
}

export function LogoutButton({
  variant = 'button',
  showConfirmation = false,
  redirectTo = ROUTES.LOGIN,
  icon,
  children = 'Logout',
  tooltip = 'Logout',
  onLogout,
  onError,
  disabled = false,
  ...buttonProps
}: LogoutButtonProps) {
  const [isLoading, setIsLoading] = useState(false);
  const [showDialog, setShowDialog] = useState(false);
  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogoutClick = () => {
    if (showConfirmation) {
      setShowDialog(true);
    } else {
      handleLogout();
    }
  };

  const handleLogout = async () => {
    try {
      setIsLoading(true);
      setShowDialog(false);

      await logout();
      
      toast.success('Logged out successfully');
      
      // Call custom callback
      if (onLogout) {
        onLogout();
      }
      
      // Redirect to specified route
      navigate(redirectTo);
    } catch (error) {
      const logoutError = error instanceof Error ? error : new Error('Logout failed');
      console.error('Logout failed:', logoutError);
      
      toast.error('Failed to logout. Please try again.');
      
      // Call error callback
      if (onError) {
        onError(logoutError);
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleDialogClose = () => {
    setShowDialog(false);
  };

  const renderButton = () => {
    const isDisabled = disabled || isLoading;
    const buttonIcon = icon || <LogoutIcon />;
    const loadingIcon = <CircularProgress size={20} />;

    switch (variant) {
      case 'icon':
        return (
          <Tooltip title={tooltip}>
            <span>
              <IconButton
                onClick={handleLogoutClick}
                disabled={isDisabled}
                color="inherit"
                {...buttonProps}
              >
                {isLoading ? loadingIcon : buttonIcon}
              </IconButton>
            </span>
          </Tooltip>
        );

      case 'text':
        return (
          <Button
            onClick={handleLogoutClick}
            disabled={isDisabled}
            startIcon={isLoading ? loadingIcon : buttonIcon}
            variant="text"
            {...buttonProps}
          >
            {children}
          </Button>
        );

      case 'button':
      default:
        return (
          <Button
            onClick={handleLogoutClick}
            disabled={isDisabled}
            startIcon={isLoading ? loadingIcon : buttonIcon}
            variant="contained"
            {...buttonProps}
          >
            {children}
          </Button>
        );
    }
  };

  return (
    <>
      {renderButton()}

      {/* Confirmation Dialog */}
      <Dialog
        open={showDialog}
        onClose={handleDialogClose}
        aria-labelledby="logout-dialog-title"
        aria-describedby="logout-dialog-description"
      >
        <DialogTitle id="logout-dialog-title">
          Confirm Logout
        </DialogTitle>
        <DialogContent>
          <DialogContentText id="logout-dialog-description">
            Are you sure you want to logout? You will need to sign in again to access your account.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleDialogClose} disabled={isLoading}>
            Cancel
          </Button>
          <Button 
            onClick={handleLogout} 
            disabled={isLoading}
            startIcon={isLoading ? <CircularProgress size={20} /> : <ExitToAppIcon />}
            variant="contained"
            color="primary"
          >
            {isLoading ? 'Logging out...' : 'Logout'}
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
}

export default LogoutButton;
