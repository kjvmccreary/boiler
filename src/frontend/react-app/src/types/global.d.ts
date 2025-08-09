/// <reference types="vite/client" />
/// <reference types="vitest/globals" />

declare global {
  interface ImportMetaEnv {
    readonly VITE_API_BASE_URL: string
    readonly VITE_APP_VERSION: string
    readonly VITE_ENABLE_ANALYTICS: string
    readonly VITE_LOG_LEVEL: string
  }

  interface ImportMeta {
    readonly env: ImportMetaEnv
  }
}

// React component props helpers
declare module '*.svg' {
  import type { FunctionComponent, SVGProps } from 'react'
  export const ReactComponent: FunctionComponent<SVGProps<SVGSVGElement> & { title?: string }>
  export default ReactComponent
}

declare module '*.png' {
  const value: string
  export default value
}

declare module '*.jpg' {
  const value: string
  export default value
}

declare module '*.jpeg' {
  const value: string
  export default value
}

declare module '*.gif' {
  const value: string
  export default value
}

export {}
