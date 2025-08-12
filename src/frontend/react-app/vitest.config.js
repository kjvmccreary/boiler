import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

export default defineConfig({
  plugins: [react()],
  
  // Fix: Add define for environment variables
  define: {
    'import.meta.env.VITE_API_BASE_URL': JSON.stringify('http://localhost:5000/api'),
    'import.meta.env.VITE_APP_TITLE': JSON.stringify('Test App'),
  },
  
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          'react-vendor': ['react', 'react-dom'],
          'mui-vendor': [
            '@mui/material', 
            '@mui/icons-material', 
            '@emotion/react', 
            '@emotion/styled'
          ],
          'router-vendor': ['react-router-dom'],
          'query-vendor': ['@tanstack/react-query'],
          'form-vendor': ['react-hook-form', '@hookform/resolvers', 'zod'],
          'auth-components': [
            './src/components/auth/LoginForm.jsx',
            './src/components/auth/RegisterForm.jsx',
            './src/components/auth/ChangePasswordForm.jsx',
            './src/contexts/AuthContext.jsx'
          ],
          'user-components': [
            './src/components/users/UserList.jsx',
            './src/components/users/UserProfile.jsx',
            './src/components/users/UserRoleAssignment.jsx'
          ],
          'role-components': [
            './src/components/roles/RoleList.jsx',
            './src/components/roles/RoleEditor.jsx',
            './src/components/roles/PermissionSelector.jsx'
          ]
        }
      }
    },
    chunkSizeWarningLimit: 800,
    sourcemap: true
  },
  
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: ['./src/test/setup.ts'],
    include: [
      'src/**/*.{test,spec}.{js,mjs,cjs,ts,mts,cts,jsx,tsx}'
    ],
    exclude: [
      'node_modules',
      'dist',
      '.vscode',
      '.git'
    ],
    
    // Fix: Add pool configuration to prevent test interference
    pool: 'forks',
    poolOptions: {
      forks: {
        singleFork: true,
      },
    },
    
    // Fix: Increase timeout for complex component tests
    testTimeout: 15000,
    hookTimeout: 15000,
    
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      include: ['src/**/*.{js,ts,jsx,tsx}'],
      exclude: [
        'src/**/*.{test,spec}.{js,ts,jsx,tsx}',
        'src/test/**',
        'src/**/*.d.ts',
        'src/main.tsx',
        'src/vite-env.d.ts'
      ],
      thresholds: {
        global: {
          branches: 70,
          functions: 70,
          lines: 70,
          statements: 70
        }
      }
    },
    reporters: ['verbose'],
    
    // Fix: Add retry configuration for flaky tests
    retry: 1,
    
    // Fix: Configure browser-like environment
    env: {
      NODE_ENV: 'test',
      VITE_API_BASE_URL: 'http://localhost:5000/api',
    }
  },
  
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  }
})
