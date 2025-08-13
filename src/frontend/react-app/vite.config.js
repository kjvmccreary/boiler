import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'node:path'
import { fileURLToPath } from 'node:url'
import fs from 'fs'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

export default defineConfig({
  plugins: [react()],
  
  server: {
    port: 3000,
    https: {
      key: fs.readFileSync(path.resolve(__dirname, 'certs/localhost-key.pem')),
      cert: fs.readFileSync(path.resolve(__dirname, 'certs/localhost.pem'))
    },
    proxy: {
      // ✅ FIXED: Route auth requests to AuthService
      '/api/auth': {
        target: 'https://localhost:7001', // AuthService port
        changeOrigin: true,
        secure: false,
        configure: (proxy, _options) => {
          proxy.on('error', (err, _req, _res) => {
            console.log('🚨 Auth Proxy error:', err);
          });
          proxy.on('proxyReq', (proxyReq, req, _res) => {
            console.log('🔍 Auth Proxying request:', req.method, req.url, '→', proxyReq.path);
          });
          proxy.on('proxyRes', (proxyRes, req, _res) => {
            console.log('✅ Auth Proxy response:', req.method, req.url, '→', proxyRes.statusCode);
          });
        },
      },
      // ✅ Route everything else to UserService  
      '/api': {
        target: 'https://localhost:7002', // UserService port
        changeOrigin: true,
        secure: false,
        configure: (proxy, _options) => {
          proxy.on('error', (err, _req, _res) => {
            console.log('🚨 User Proxy error:', err);
          });
          proxy.on('proxyReq', (proxyReq, req, _res) => {
            console.log('🔍 User Proxying request:', req.method, req.url, '→', proxyReq.path);
          });
          proxy.on('proxyRes', (proxyRes, req, _res) => {
            console.log('✅ User Proxy response:', req.method, req.url, '→', proxyRes.statusCode);
          });
        },
      },
    },
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
            './src/components/roles/RoleDetails.jsx',
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
