import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

export default defineConfig({
  plugins: [react()],

  define: {
    'import.meta.env.VITE_API_BASE_URL': JSON.stringify('http://localhost:5000/api'),
    'import.meta.env.VITE_APP_TITLE': JSON.stringify('Test App')
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

  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  },

  test: {
    name: 'frontend',
    environment: 'jsdom',
    globals: true,
    setupFiles: ['src/test/setup.ts'],

    include: ['src/**/*.{test,spec}.{js,mjs,cjs,ts,mts,cts,jsx,tsx}'],
    exclude: ['node_modules', 'dist', '.vscode', '.git'],

    pool: 'forks',
    poolOptions: { forks: { singleFork: true } },

    testTimeout: 15000,
    hookTimeout: 15000,
    retry: 1,

    css: true,

    server: {
      deps: {
        inline: [
          /@mui\/x-data-grid.*/,
          /@mui\/x-date-pickers.*/,
          /@mui\/material.*/,
          /@mui\/icons-material.*/,
          /@emotion\/react.*/,
          /@emotion\/styled.*/
        ]
      }
    },

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
        // Raised as part of Item 16 (was 70).
        // TODO(WF-COV): Raise branches/functions/lines to 85 after operations tests re-enabled.
        global: {
          branches: 80,
          functions: 80,
          lines: 80,
          statements: 80
        }
      }
    },

    reporters: ['verbose'],

    env: {
      NODE_ENV: 'test',
      VITE_API_BASE_URL: 'http://localhost:5000/api'
    }
  }
})
