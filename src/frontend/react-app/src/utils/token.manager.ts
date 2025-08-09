const TOKEN_KEY = import.meta.env.VITE_TOKEN_STORAGE_KEY || 'auth_token';
const REFRESH_TOKEN_KEY = import.meta.env.VITE_REFRESH_TOKEN_STORAGE_KEY || 'refresh_token';

class TokenManager {
  private useMemoryStorage = false;
  private memoryStore: { [key: string]: string } = {};

  constructor() {
    // Check if localStorage is available
    try {
      localStorage.setItem('test', 'test');
      localStorage.removeItem('test');
    } catch {
      this.useMemoryStorage = true;
      console.warn('localStorage not available, using memory storage for tokens');
    }
  }

  getToken(): string | null {
    const token = this.useMemoryStorage 
      ? this.memoryStore[TOKEN_KEY] || null
      : localStorage.getItem(TOKEN_KEY);
    
    // 🔧 ADD: Debug token information
    if (token) {
      console.log('🔍 TokenManager: Current token info:', {
        hasToken: !!token,
        tokenLength: token.length,
        isExpired: this.isTokenExpired(token),
        tokenPreview: `${token.substring(0, 50)}...`,
        claims: this.getTokenClaims(token)
      });
    } else {
      console.log('🔍 TokenManager: No token found');
    }
    
    return token;
  }

  getRefreshToken(): string | null {
    const refreshToken = this.useMemoryStorage
      ? this.memoryStore[REFRESH_TOKEN_KEY] || null
      : localStorage.getItem(REFRESH_TOKEN_KEY);
    
    console.log('🔍 TokenManager: Refresh token info:', {
      hasRefreshToken: !!refreshToken,
      refreshTokenLength: refreshToken?.length
    });
    
    return refreshToken;
  }

  setTokens(token: string, refreshToken: string): void {
    console.log('🔍 TokenManager: Setting new tokens:', {
      tokenLength: token.length,
      refreshTokenLength: refreshToken.length,
      newTokenClaims: this.getTokenClaims(token)
    });
    
    if (this.useMemoryStorage) {
      this.memoryStore[TOKEN_KEY] = token;
      this.memoryStore[REFRESH_TOKEN_KEY] = refreshToken;
    } else {
      localStorage.setItem(TOKEN_KEY, token);
      localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
    }
  }

  clearTokens(): void {
    console.log('🔍 TokenManager: Clearing tokens');
    
    if (this.useMemoryStorage) {
      delete this.memoryStore[TOKEN_KEY];
      delete this.memoryStore[REFRESH_TOKEN_KEY];
    } else {
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(REFRESH_TOKEN_KEY);
    }
  }

  isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Date.now() / 1000;
      const isExpired = payload.exp < currentTime;
      
      console.log('🔍 TokenManager: Token expiry check:', {
        expiryTime: new Date(payload.exp * 1000),
        currentTime: new Date(currentTime * 1000),
        isExpired
      });
      
      return isExpired;
    } catch (error) {
      console.error('🔍 TokenManager: Error checking token expiry:', error);
      return true;
    }
  }

  // 🔧 ADD: New method to get all token claims
  getTokenClaims(token: string): Record<string, any> | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload;
    } catch (error) {
      console.error('🔍 TokenManager: Error extracting token claims:', error);
      return null;
    }
  }

  getUserIdFromToken(token: string): string | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const userId = payload.sub || payload.userId || payload.nameid || null;
      
      console.log('🔍 TokenManager: Extracting user ID from token:', {
        userId,
        availableClaims: Object.keys(payload)
      });
      
      return userId;
    } catch (error) {
      console.error('🔍 TokenManager: Error extracting user ID:', error);
      return null;
    }
  }

  getTenantIdFromToken(token: string): string | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const tenantId = payload.tenantId || payload.tenant_id || null;
      
      console.log('🔍 TokenManager: Extracting tenant ID from token:', {
        tenantId,
        availableClaims: Object.keys(payload)
      });
      
      return tenantId;
    } catch (error) {
      console.error('🔍 TokenManager: Error extracting tenant ID:', error);
      return null;
    }
  }
}

export const tokenManager = new TokenManager();
