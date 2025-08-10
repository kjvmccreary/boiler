import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

export default defineConfig({
  plugins: [react()],
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          // Vendor chunks
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
          
          // App chunks
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
            './src/components/roles/RoleDetails.jsx', // Add this
            './src/components/roles/PermissionSelector.jsx'
          ]
        }
      }
    },
    // Increase chunk size warning limit for .NET 9 enterprise app
    chunkSizeWarningLimit: 800,
    // Enable source maps for production debugging
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
    testTimeout: 10000
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  }
})
