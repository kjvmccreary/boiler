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
    if (this.useMemoryStorage) {
      return this.memoryStore[TOKEN_KEY] || null;
    }
    return localStorage.getItem(TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    if (this.useMemoryStorage) {
      return this.memoryStore[REFRESH_TOKEN_KEY] || null;
    }
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  setTokens(token: string, refreshToken: string): void {
    if (this.useMemoryStorage) {
      this.memoryStore[TOKEN_KEY] = token;
      this.memoryStore[REFRESH_TOKEN_KEY] = refreshToken;
    } else {
      localStorage.setItem(TOKEN_KEY, token);
      localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
    }
  }

  clearTokens(): void {
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
      return payload.exp < currentTime;
    } catch {
      return true;
    }
  }

  getUserIdFromToken(token: string): string | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.sub || payload.userId || null;
    } catch {
      return null;
    }
  }

  getTenantIdFromToken(token: string): string | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.tenantId || null;
    } catch {
      return null;
    }
  }
}

export const tokenManager = new TokenManager();
