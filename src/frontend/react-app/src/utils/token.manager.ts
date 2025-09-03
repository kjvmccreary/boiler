const TOKEN_KEY = import.meta.env.VITE_TOKEN_STORAGE_KEY || 'auth_token';
const REFRESH_TOKEN_KEY = import.meta.env.VITE_REFRESH_TOKEN_STORAGE_KEY || 'refresh_token';

class TokenManager {
  private useMemoryStorage = false;
  private memoryStore: { [key: string]: string } = {};

  constructor() {
    try {
      localStorage.setItem('test', 'test');
      localStorage.removeItem('test');
    } catch {
      this.useMemoryStorage = true;
      console.warn('localStorage not available, using memory storage for tokens');
    }
  }

  private isLikelyJwt(token: string | null | undefined): boolean {
    return !!token && token.split('.').length === 3;
  }

  getToken(): string | null {
    const token = this.useMemoryStorage
      ? this.memoryStore[TOKEN_KEY] || null
      : localStorage.getItem(TOKEN_KEY);

    if (token && this.isLikelyJwt(token)) {
      console.log('🔍 TokenManager: Current token info:', {
        hasToken: true,
        tokenLength: token.length,
        isExpired: this.isTokenExpired(token),
        tokenPreview: `${token.substring(0, 50)}...`,
        claims: this.getTokenClaims(token)
      });
    } else if (token) {
      console.log('🔍 TokenManager: Non-JWT token stored (skipping decode)', {
        tokenLength: token.length
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
      isJwt: this.isLikelyJwt(token),
      newTokenClaims: this.isLikelyJwt(token) ? this.getTokenClaims(token) : null
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
    if (!this.isLikelyJwt(token)) return true;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Date.now() / 1000;
      return payload.exp < currentTime;
    } catch {
      return true;
    }
  }

  getTokenClaims(token: string): Record<string, any> | null {
    if (!this.isLikelyJwt(token)) return null;
    try {
      return JSON.parse(atob(token.split('.')[1]));
    } catch {
      return null;
    }
  }

  getUserIdFromToken(token: string): string | null {
    const payload = this.getTokenClaims(token);
    if (!payload) return null;
    return payload.sub || payload.userId || payload.nameid || null;
  }

  getTenantIdFromToken(token: string): string | null {
    const payload = this.getTokenClaims(token);
    if (!payload) return null;
    return payload.tenantId || payload.tenant_id || null;
    }
}

export const tokenManager = new TokenManager();
