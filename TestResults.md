PS C:\Users\mccre\dev\boiler\src\frontend\react-app> npm test -- --run --reporter=verbose

> microservices-frontend@0.0.0 test
> vitest --config vitest.config.js --run --reporter=verbose


 RUN  v1.6.1 C:/Users/mccre/dev/boiler/src/frontend/react-app

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '11',
    VITEST_POOL_ID: '11',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '2',
    VITEST_POOL_ID: '2',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '4',
    VITEST_POOL_ID: '4',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '3',
    VITEST_POOL_ID: '3',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '6',
    VITEST_POOL_ID: '6',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '5',
    VITEST_POOL_ID: '5',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '9',
    VITEST_POOL_ID: '9',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '8',
    VITEST_POOL_ID: '8',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '7',
    VITEST_POOL_ID: '7',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '10',
    VITEST_POOL_ID: '10',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '1',
    VITEST_POOL_ID: '1',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Role-Based Data Filtering > should filter user data based on role permissions
[MSW] Error: intercepted a request without a matching request handler:

  • GET /api/users

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '13',
    VITEST_POOL_ID: '11',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '12',
    VITEST_POOL_ID: '10',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle 403 Forbidden responses gracefully
An update to ProtectedActionComponent inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ProtectedActionComponent inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
[MSW] Error: intercepted a request without a matching request handler:

  • DELETE /api/users/1

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
An update to ProtectedActionComponent inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ProtectedActionComponent inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '14',
    VITEST_POOL_ID: '4',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '15',
    VITEST_POOL_ID: '1',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:15:13)
🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG: {
  'import.meta.env.MODE': 'test',
  'import.meta.env.DEV': true,
  'import.meta.env.PROD': false,
  'import.meta.env.VITE_API_BASE_URL': undefined,
  'window.location': {
    origin: 'http://localhost:3000',
    hostname: 'localhost',
    port: '3000',
    protocol: 'http:'
  },
  'All environment variables': {
    HOMEPATH: '\\Users\\mccre',
    TEST: 'true',
    APPDATA: 'C:\\Users\\mccre\\AppData\\Roaming',
    VITEST: 'true',
    NODE_ENV: 'test',
    ALLUSERSPROFILE: 'C:\\ProgramData',
    VITEST_MODE: 'RUN',
    BASE_URL: '/',
    ChocolateyInstall: 'C:\\ProgramData\\chocolatey',
    EFC_11020_2397410445: '1',
    ChocolateyLastPathUpdate: '133993438273562931',
    COLOR: '1',
    EDITOR: 'C:\\WINDOWS\\notepad.exe',
    CommonProgramFiles: 'C:\\Program Files\\Common Files',
    'CommonProgramFiles(Arm)': 'C:\\Program Files (Arm)\\Common Files',
    NUMBER_OF_PROCESSORS: '12',
    EFC_11020_1592913036: '1',
    'CommonProgramFiles(x86)': 'C:\\Program Files (x86)\\Common Files',
    npm_config_local_prefix: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    CommonProgramW6432: 'C:\\Program Files\\Common Files',
    EFC_11020_1262719628: '1',
    PROCESSOR_IDENTIFIER: 'ARMv8 (64-bit) Family 8 Model 1 Revision 201, Qualcomm Technologies Inc',
    npm_config_userconfig: 'C:\\Users\\mccre\\.npmrc',
    COMPUTERNAME: 'MACDADDYARM',
    USERNAME: 'mccre',
    ComSpec: 'C:\\WINDOWS\\system32\\cmd.exe',
    npm_command: 'test',
    DEV: '1',
    DriverData: 'C:\\Windows\\System32\\Drivers\\DriverData',
    EFC_11020_2283032206: '1',
    EFC_11020_2775293581: '1',
    npm_execpath: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    npm_config_node_gyp: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    npm_config_init_module: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_NODE_GYP: 'C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\node-gyp\\bin\\node-gyp.js',
    EFC_11020_3789132940: '1',
    npm_config_noproxy: '',
    PSMODULEPATH: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    FPS_BROWSER_APP_PROFILE_STRING: 'Internet Explorer',
    PROGRAMDATA: 'C:\\ProgramData',
    FPS_BROWSER_USER_PROFILE_STRING: 'Default',
    npm_config_global_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    HOME: 'C:\\Users\\mccre',
    npm_package_version: '0.0.0',
    HOMEDRIVE: 'C:',
    INIT_CWD: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    Path: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    npm_lifecycle_event: 'test',
    LOCALAPPDATA: 'C:\\Users\\mccre\\AppData\\Local',
    LOGONSERVER: '\\\\MACDADDYARM',
    MODE: 'test',
    NODE: 'C:\\Program Files\\nodejs\\node.exe',
    npm_config_cache: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    npm_config_globalconfig: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    OneDriveConsumer: 'C:\\Users\\mccre\\OneDrive',
    npm_config_legacy_peer_deps: 'true',
    npm_config_npm_version: '10.9.3',
    npm_config_prefix: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    OS: 'Windows_NT',
    npm_config_user_agent: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    npm_lifecycle_script: 'vitest --config vitest.config.js',
    npm_node_execpath: 'C:\\Program Files\\nodejs\\node.exe',
    WINDIR: 'C:\\WINDOWS',
    npm_package_json: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    npm_package_name: 'microservices-frontend',
    OneDrive: 'C:\\Users\\mccre\\OneDrive',
    PATHEXT: '.COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;.CPL',
    CHOCOLATEYINSTALL: 'C:\\ProgramData\\chocolatey',
    PROCESSOR_ARCHITECTURE: 'ARM64',
    PROCESSOR_LEVEL: '1',
    PROCESSOR_REVISION: '0201',
    PROD: '',
    ProgramData: 'C:\\ProgramData',
    ProgramFiles: 'C:\\Program Files',
    'ProgramFiles(Arm)': 'C:\\Program Files (Arm)',
    'ProgramFiles(x86)': 'C:\\Program Files (x86)',
    ProgramW6432: 'C:\\Program Files',
    PROMPT: '$P$G',
    PSModulePath: 'C:\\Users\\mccre\\OneDrive\\Documents\\WindowsPowerShell\\Modules;C:\\Program Files\\WindowsPowerShell\\Modules;C:\\WINDOWS\\system32\\WindowsPowerShell\\v1.0\\Modules',
    PUBLIC: 'C:\\Users\\Public',
    SESSIONNAME: 'Console',
    SystemDrive: 'C:',
    SystemRoot: 'C:\\WINDOWS',
    TEMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    TMP: 'C:\\Users\\mccre\\AppData\\Local\\Temp',
    USERDOMAIN: 'MACDADDYARM',
    USERDOMAIN_ROAMINGPROFILE: 'MACDADDYARM',
    USERPROFILE: 'C:\\Users\\mccre',
    windir: 'C:\\WINDOWS',
    CHOCOLATEYLASTPATHUPDATE: '133993438273562931',
    COMMONPROGRAMFILES: 'C:\\Program Files\\Common Files',
    'COMMONPROGRAMFILES(ARM)': 'C:\\Program Files (Arm)\\Common Files',
    'COMMONPROGRAMFILES(X86)': 'C:\\Program Files (x86)\\Common Files',
    COMMONPROGRAMW6432: 'C:\\Program Files\\Common Files',
    COMSPEC: 'C:\\WINDOWS\\system32\\cmd.exe',
    DRIVERDATA: 'C:\\Windows\\System32\\Drivers\\DriverData',
    NPM_COMMAND: 'test',
    NPM_CONFIG_CACHE: 'C:\\Users\\mccre\\AppData\\Local\\npm-cache',
    NPM_CONFIG_GLOBALCONFIG: 'C:\\Users\\mccre\\AppData\\Roaming\\npm\\etc\\npmrc',
    NPM_CONFIG_GLOBAL_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_INIT_MODULE: 'C:\\Users\\mccre\\.npm-init.js',
    NPM_CONFIG_LEGACY_PEER_DEPS: 'true',
    NPM_CONFIG_LOCAL_PREFIX: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app',
    NPM_CONFIG_NOPROXY: '',
    NPM_CONFIG_NPM_VERSION: '10.9.3',
    NPM_CONFIG_PREFIX: 'C:\\Users\\mccre\\AppData\\Roaming\\npm',
    NPM_CONFIG_USERCONFIG: 'C:\\Users\\mccre\\.npmrc',
    NPM_CONFIG_USER_AGENT: 'npm/10.9.3 node/v22.18.0 win32 arm64 workspaces/false',
    NPM_LIFECYCLE_SCRIPT: 'vitest --config vitest.config.js',
    NPM_EXECPATH: 'C:\\Program Files\\nodejs\\node_modules\\npm\\bin\\npm-cli.js',
    NPM_LIFECYCLE_EVENT: 'test',
    PROGRAMFILES: 'C:\\Program Files',
    NPM_NODE_EXECPATH: 'C:\\Program Files\\nodejs\\node.exe',
    NPM_PACKAGE_JSON: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\package.json',
    NPM_PACKAGE_NAME: 'microservices-frontend',
    NPM_PACKAGE_VERSION: '0.0.0',
    ONEDRIVE: 'C:\\Users\\mccre\\OneDrive',
    ONEDRIVECONSUMER: 'C:\\Users\\mccre\\OneDrive',
    PATH: 'C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\src\\node_modules\\.bin;C:\\Users\\mccre\\dev\\boiler\\node_modules\\.bin;C:\\Users\\mccre\\dev\\node_modules\\.bin;C:\\Users\\mccre\\node_modules\\.bin;C:\\Users\\node_modules\\.bin;C:\\node_modules\\.bin;C:\\Program Files\\nodejs\\node_modules\\npm\\node_modules\\@npmcli\\run-script\\lib\\node-gyp-bin;C:\\WINDOWS\\system32;C:\\WINDOWS;C:\\WINDOWS\\System32\\Wbem;C:\\WINDOWS\\System32\\WindowsPowerShell\\v1.0\\;C:\\WINDOWS\\System32\\OpenSSH\\;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files\\dotnet\\;C:\\Program Files\\nodejs\\;C:\\Program Files\\Git\\cmd;C:\\Program Files\\Docker\\Docker\\resources\\bin;C:\\ProgramData\\chocolatey\\bin;C:\\Users\\mccre\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Users\\mccre\\.dotnet\\tools;C:\\Users\\mccre\\AppData\\Roaming\\npm',
    'PROGRAMFILES(ARM)': 'C:\\Program Files (Arm)',
    'PROGRAMFILES(X86)': 'C:\\Program Files (x86)',
    PROGRAMW6432: 'C:\\Program Files',
    SYSTEMDRIVE: 'C:',
    SYSTEMROOT: 'C:\\WINDOWS',
    VITEST_WORKER_ID: '16',
    VITEST_POOL_ID: '3',
    SSR: ''
  }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:32:13)
🔍 API CLIENT: Creating axios instance with baseURL:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:44:13)
🔍 AXIOS INSTANCE CONFIG: {
  baseURL: '',
  timeout: 10000,
  headers: {
    common: {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': undefined
    },
    delete: {},
    get: {},
    head: {},
    post: {},
    put: {},
    patch: {},
    'Content-Type': 'application/json'
  },
  adapter: [ 'xhr', 'http', 'fetch' ]
}

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Email Confirmation Flow > handles confirmation failure and provides recovery options
Email confirmation failed: Error: The confirmation link has expired
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\auth-integration.test.tsx:80:9
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Logout Functionality > performs logout without confirmation by default
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Logout Functionality > shows confirmation dialog when showConfirmation is true
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Portal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Modal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Logout Flow > performs logout with confirmation dialog
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Portal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Modal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Logout Flow > performs logout with confirmation dialog
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Logout Functionality > cancels logout from confirmation dialog
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Portal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Modal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Logout Flow > performs logout with confirmation dialog
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to MemoryRouter inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Logout Functionality > cancels logout from confirmation dialog
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Logout Functionality > cancels logout from confirmation dialog
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Logout Flow > cancels logout when user chooses cancel
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Portal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Modal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Logout Flow > cancels logout when user chooses cancel
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Logout Flow > cancels logout when user chooses cancel
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Logout Functionality > confirms logout from confirmation dialog
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Portal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Modal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Logout Functionality > confirms logout from confirmation dialog
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Error Recovery > provides clear error messages and recovery paths
Email confirmation failed: Error: Network error occurred
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\auth-integration.test.tsx:149:9
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Logout Functionality > confirms logout from confirmation dialog
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Error Handling > handles logout error gracefully
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
Logout failed: Error: Logout failed
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\LogoutButton.test.tsx:171:21
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Error Handling > shows loading state during logout
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Accessibility Integration > provides appropriate ARIA announcements for state changes
Email confirmation failed: Error: Confirmation failed
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\auth-integration.test.tsx:188:9
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Accessibility > manages dialog accessibility correctly
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to LogoutButton inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Portal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(Modal) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to Transition inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > handles search functionality
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(FormControl) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(FormControl) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > handles search functionality
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(FormControl) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(FormControl) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(FormControl) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > handles search functionality
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > handles search functionality
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(FormControl) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(FormControl) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > Successful Confirmation > navigates to login when Sign In Now button is clicked
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > handles search functionality
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > Error Handling > displays error message when confirmation fails
Email confirmation failed: Error: Confirmation failed
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:101:65
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > handles search functionality
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(FormControl) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(FormControl) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > Error Handling > handles expired token error specifically
Email confirmation failed: Error: The confirmation link has expired
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:114:28
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > Error Handling > handles invalid token error specifically
Email confirmation failed: Error: The confirmation link is invalid
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:125:28
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > Error Handling > handles not found error specifically
Email confirmation failed: Error: Token not found
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:136:29
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > User Actions > handles resend confirmation button click
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/RoleDetails.test.tsx > RoleDetails > handles edit button click for non-system roles
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > User Actions > handles back to login button click
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > API Integration > handles network errors gracefully
Email confirmation failed: Error: Network error
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:193:28
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > Accessibility > provides appropriate error announcements
Email confirmation failed: Error: Test error
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:208:65
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > handles pagination correctly
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > handles pagination correctly
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(SelectInput) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > calls onEditRole when edit button is clicked
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to RoleList inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/RoleDetails.test.tsx > RoleDetails > handles delete role with confirmation
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/RoleDetails.test.tsx > RoleDetails > handles role not found error
Failed to load role details: Error: Role not found
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\roles\RoleDetails.test.tsx:261:58
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7
    at withEnv (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:83:5)

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > calls onDeleteRole when delete button is clicked
An update to ForwardRef(ButtonBase) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to TransitionGroup inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to ForwardRef(TouchRipple) inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should render loading state initially
An update to PermissionSelector inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act
An update to PermissionSelector inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */

This ensures that you're testing the behavior the user would see in the browser. Learn more at https://react.dev/link/wrap-tests-with-act

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should render permissions after loading
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should show search box when showSearch is true
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should call onChange when permission is selected
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should select all permissions in category when category checkbox is clicked
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should be disabled when disabled prop is true
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.

stderr | src/components/__tests__/roles/PermissionTreeView.test.tsx > PermissionTreeView > should render permission hierarchy
Each child in a list should have a unique "key" prop.

Check the render method of `ul`. It was passed a child from PermissionTreeView. See https://react.dev/link/warning-keys for more information.
In HTML, <div> cannot be a descendant of <p>.
This will cause a hydration error.

  ...
    <MuiList-root as="ul" className="MuiList-ro..." ref={null} ownerState={{dense:false, ...}} sx={{width:"100%"}}>
      <Insertion>
      <ul className="MuiList-ro...">
        <ListItem sx={{borderRadius:1, ...}}>
          <MuiListItem-root as="li" ref={function} ownerState={{sx:{...}, ...}} className="MuiListIte..." ...>
            <Insertion>
            <li className="MuiListIte..." ref={function}>
              <ListItemIcon>
              <ListItemText primary={<ForwardRef(Box)>} secondary={<ForwardRef(Box)>}>
                <MuiListItemText-root className="MuiListIte..." ref={null} ownerState={{primary:true, ...}}>
                  <Insertion>
                  <div className="MuiListIte...">
                    <Typography>
                    <Typography variant="body2" color="textSecondary" className="MuiListIte..." ref={null} ...>
                      <MuiTypography-root as="p" ref={null} className="MuiTypogra..." ownerState={{variant:"b...", ...}} ...>
                        <Insertion>
>                       <p
>                         className="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiT..."
>                         style={{}}
>                       >
                          <Box>
                            <Styled(div) as="div" ref={null} className="MuiBox-root" theme={{...}} sx={{}}>
                              <Insertion>
>                             <div className="MuiBox-root css-0">
        ...

<p> cannot contain a nested <div>.
See this log for the ancestor stack trace.

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should handle API error gracefully
Failed to load permissions: Error: API Error
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\roles\PermissionSelector.test.tsx:126:74
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runNextTicks (node:internal/process/task_queues:65:5)
    at listOnTimeout (node:internal/timers:549:9)
    at processTimers (node:internal/timers:523:7)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)

 ❯ src/test/scenarios/api-permission-integration.test.tsx (6) 4795ms
   ❯ API Permission Integration Scenarios (6) 4793ms
     ❯ API Call Authorization (2) 2348ms
       × should allow API calls for users with correct permissions 1147ms
       × should reject API calls for users without permissions 1201ms
     ❯ Role-Based Data Filtering (2) 1094ms
       × should filter user data based on role permissions 1055ms
       × should limit data for lower privilege users
     ❯ Permission-Based Error Handling (2) 1350ms
       ✓ should handle 403 Forbidden responses gracefully
       × should handle successful operations for authorized users 1144ms
 ❯ src/test/scenarios/multi-tenant-isolation.test.tsx (6)
   ❯ Multi-Tenant Permission Isolation Scenarios (6)
     ❯ Tenant Data Isolation (2)
       × should isolate tenant data access
       ✓ should prevent cross-tenant role assignment
     ❯ Tenant-Scoped Permissions (2)
       × should scope permissions to specific tenants
       ✓ should allow system admins to access multiple tenants
     ❯ Tenant Context Switching (2)
       ✓ should handle tenant context switching for multi-tenant users
       × should restrict tenant switching for single-tenant users
 ❯ src/test/scenarios/role-hierarchy-scenarios.test.tsx (6)
   ❯ Role Hierarchy Validation Scenarios (6)
     ❯ Permission Inheritance (2)
       ✓ should validate role hierarchy levels
       × should ensure higher roles have more permissions than lower roles
     ❯ System vs Tenant Role Separation (2)
       × should separate system-level and tenant-level permissions
       ✓ should validate system role permissions
     ✓ Multi-Role User Scenarios (2)
       ✓ should handle users with multiple roles correctly
       ✓ should prioritize highest role level for admin checks
 ❯ src/test/scenarios/permission-component-patterns.test.tsx (4) 1494ms
   ❯ RBAC Permission Component Patterns (4) 1487ms
     ❯ Conditional UI Rendering (1) 1162ms
       × should show/hide UI elements based on user permissions 1162ms
     ❯ Form Field Permissions (1)
       × should enable/disable form fields based on permissions
     ❯ Navigation Menu Permissions (1)
       × should show navigation items based on user permissions
     ❯ Data Table Action Buttons (1)
       × should show/hide action buttons based on permissions
 ❯ src/test/examples/rbac-usage-examples.test.tsx (10) 1109ms
   ❯ RBAC Test Utilities - Usage Examples (10) 1106ms
     ❯ Basic Role Rendering (1)
       × should render differently for different roles
     ✓ Scenario Builder Pattern (1)
       ✓ should support complex scenarios
     ✓ Permission Assertions (1)
       ✓ should assert element visibility based on permissions
     ❯ Batch Role Testing (1)
       × should test all roles against component
     ✓ Permission Matrix Testing (1)
       ✓ should test permission matrix
     ❯ Form Testing (1)
       × should test form field permissions
     ❯ Navigation Testing (1)
       × should test menu visibility
     ✓ Common Scenarios (1)
       ✓ should use predefined scenarios
     ✓ Multi-Role User Testing (1)
       ✓ should test users with multiple roles
     ❯ Error State Testing (1) 1018ms
       × should test permission-based errors 1018ms
 ✓ src/components/common/__tests__/LoadingStates.test.tsx (11) 2435ms
   ✓ LoadingStates (11) 2424ms
     ✓ LoadingSpinner (3) 594ms
       ✓ renders with default props 446ms
       ✓ renders with custom message
       ✓ renders with full height when specified
     ✓ PageLoading (3)
       ✓ renders with default message
       ✓ renders with custom message
       ✓ shows progress percentage when provided
     ✓ TableSkeleton (3) 1158ms
       ✓ renders skeleton elements 708ms
       ✓ renders custom number of rows and columns
       ✓ can hide header
     ✓ UserListSkeleton (2) 442ms
       ✓ renders default number of user items 350ms
       ✓ renders custom number of user items
 ✓ src/components/__tests__/auth/auth-integration.test.tsx (7) 3851ms
   ✓ Authentication Integration Tests (7) 3841ms
     ✓ Email Confirmation Flow (2) 1328ms
       ✓ completes email confirmation and shows success state 665ms
       ✓ handles confirmation failure and provides recovery options 663ms
     ✓ Logout Flow (2) 1781ms
       ✓ performs logout with confirmation dialog 925ms
       ✓ cancels logout when user chooses cancel 855ms
     ✓ Error Recovery (1)
       ✓ provides clear error messages and recovery paths
     ✓ Accessibility Integration (2) 467ms
       ✓ maintains proper focus management through auth flows
       ✓ provides appropriate ARIA announcements for state changes
 ✓ src/components/__tests__/auth/LogoutButton.test.tsx (12) 4234ms
   ✓ LogoutButton (12) 4225ms
     ✓ Button Variants (4) 881ms
       ✓ renders button variant by default 413ms
       ✓ renders icon variant correctly
       ✓ renders text variant correctly
       ✓ renders custom children text
     ✓ Logout Functionality (4) 2316ms
       ✓ performs logout without confirmation by default 353ms
       ✓ shows confirmation dialog when showConfirmation is true 708ms
       ✓ cancels logout from confirmation dialog 673ms
       ✓ confirms logout from confirmation dialog 582ms
     ✓ Error Handling (2) 559ms
       ✓ handles logout error gracefully
       ✓ shows loading state during logout 321ms
     ✓ Accessibility (2) 469ms
       ✓ has proper ARIA attributes for icon variant
       ✓ manages dialog accessibility correctly 378ms
 ✓ src/components/auth/__tests__/ChangePasswordForm.test.tsx (7) 5976ms
   ✓ ChangePasswordForm (7) 5967ms
     ✓ renders the form fields 1483ms
     ✓ shows validation errors for empty fields 488ms
     ✓ shows error when new password is same as current 727ms
     ✓ shows error when passwords do not match 702ms
     ✓ successfully changes password and shows success screen 1305ms
     ✓ toggles password visibility
     ✓ calls logout when clicking "Sign In Again" 1109ms
 ✓ src/components/__tests__/roles/RoleDetails.test.tsx (10) 6079ms
   ✓ RoleDetails (10) 6063ms
     ✓ renders role information correctly 1406ms
     ✓ displays permissions grouped by category 413ms
     ✓ shows assigned users with correct status chips 417ms
     ✓ handles edit button click for non-system roles 1416ms
     ✓ disables edit and delete buttons for system roles 1103ms
     ✓ handles delete role with confirmation 833ms
     ✓ handles role not found error
     ✓ shows loading state initially
     ✓ handles empty permissions list
     ✓ handles empty users list
 ❯ src/components/__tests__/roles/RoleList.test.tsx (10) 6993ms
   ❯ RoleList (10) 6988ms
     ✓ renders roles list correctly 1031ms
     ✓ handles search functionality 1834ms
     × handles pagination correctly 1095ms
     ✓ calls onEditRole when edit button is clicked 538ms
     × calls onDeleteRole when delete button is clicked 500ms
     ✓ disables edit and delete buttons for system roles 464ms
     ✓ shows loading state initially
     × shows error state when loading fails 1022ms
     × displays role information correctly
     × handles page size changes
 ✓ src/components/__tests__/auth/EmailConfirmation.test.tsx (13) 3308ms
   ✓ EmailConfirmation (13) 3304ms
     ✓ Token Validation (2) 1390ms
       ✓ displays invalid token message when no token is provided 1110ms
       ✓ shows loading state initially when token is present
     ✓ Successful Confirmation (2) 680ms
       ✓ displays success message when confirmation succeeds 409ms
       ✓ navigates to login when Sign In Now button is clicked
     ✓ Error Handling (4)
       ✓ displays error message when confirmation fails
       ✓ handles expired token error specifically
       ✓ handles invalid token error specifically
       ✓ handles not found error specifically
     ✓ User Actions (2) 637ms
       ✓ handles resend confirmation button click
       ✓ handles back to login button click 360ms
     ✓ API Integration (2)
       ✓ calls authService.confirmEmail with correct token
       ✓ handles network errors gracefully
     ✓ Accessibility (1)
       ✓ provides appropriate error announcements
 ✓ src/components/__tests__/roles/PermissionSelector.test.tsx (8) 3066ms
   ✓ PermissionSelector (8) 3061ms
     ✓ should render loading state initially
     ✓ should render permissions after loading 420ms
     ✓ should show search box when showSearch is true
     ✓ should call onChange when permission is selected 1159ms
     ✓ should select all permissions in category when category checkbox is clicked 679ms
     ✓ should show selected count
     ✓ should be disabled when disabled prop is true
     ✓ should handle API error gracefully
 ✓ src/components/__tests__/routes/AppRoutes.test.tsx (6)
   ✓ AppRoutes (6)
     ✓ renders role details on /roles/:id route
     ✓ renders role editor on /roles/:id/edit route
     ✓ renders role editor on /roles/new route
     ✓ renders role list on /roles route
     ✓ redirects root to dashboard
     ✓ renders login form on /login route
 ✓ src/components/common/__tests__/ErrorBoundary.test.tsx (6) 890ms
   ✓ ErrorBoundary (6) 886ms
     ✓ renders children when no error occurs
     ✓ renders error UI when error occurs 421ms
     ✓ shows page-level error UI when level is page
     ✓ calls onError callback when error occurs
     ✓ shows error details when details button is clicked
     ✓ renders custom fallback when provided
 ✓ src/components/__tests__/roles/PermissionTreeView.test.tsx (5) 734ms
   ✓ PermissionTreeView (5) 730ms
     ✓ should render permission hierarchy 368ms
     ✓ should highlight selected permissions
     ✓ should show search when enabled
     ✓ should handle permission click when callback provided
     ✓ should expand categories with selected permissions

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯ Failed Tests 24 ⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯

 FAIL  src/test/examples/rbac-usage-examples.test.tsx > RBAC Test Utilities - Usage Examples > Basic Role Rendering > should render differently for different roles
TestingLibraryElementError: Found multiple elements by: [data-testid="user-button"]

Here are the matching elements:

Ignored nodes: comments, script, style
<button
  data-testid="user-button"
>
  All Users
</button>

Ignored nodes: comments, script, style
<button
  data-testid="user-button"
>
  All Users
</button>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div>
      <button
        data-testid="admin-button"
      >
        Admin Only
      </button>
      <button
        data-testid="user-button"
      >
        All Users
      </button>
    </div>
  </div>
  <div>
    <div>
      <button
        data-testid="admin-button"
      >
        Admin Only
      </button>
      <button
        data-testid="user-button"
      >
        All Users
      </button>
    </div>
  </div>
</body>
 ❯ Object.getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ src/test/examples/rbac-usage-examples.test.tsx:31:21
     29|       // Test as regular user
     30|       rbacRender.asUser(<TestComponent />)
     31|       expect(screen.getByTestId('user-button')).toBeInTheDocument()
       |                     ^
     32|       // Admin button would be hidden by CanAccess component
     33|     })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[1/24]⎯

 FAIL  src/test/examples/rbac-usage-examples.test.tsx > RBAC Test Utilities - Usage Examples > Batch Role Testing > should test all roles against component
Error: Objects are not valid as a React child (found: object with keys {role}). If you meant to render a collection of children, use an array instead.
 ❯ throwOnInvalidObjectType node_modules/react-dom/cjs/react-dom-client.development.js:7082:13
 ❯ reconcileChildFibersImpl node_modules/react-dom/cjs/react-dom-client.development.js:8017:11
 ❯ node_modules/react-dom/cjs/react-dom-client.development.js:8057:33
 ❯ reconcileChildren node_modules/react-dom/cjs/react-dom-client.development.js:8621:13
 ❯ beginWork node_modules/react-dom/cjs/react-dom-client.development.js:10793:13
 ❯ runWithFiberInDEV node_modules/react-dom/cjs/react-dom-client.development.js:1522:13
 ❯ performUnitOfWork node_modules/react-dom/cjs/react-dom-client.development.js:15140:22
 ❯ workLoopSync node_modules/react-dom/cjs/react-dom-client.development.js:14956:41
 ❯ renderRootSync node_modules/react-dom/cjs/react-dom-client.development.js:14936:11
 ❯ performWorkOnRoot node_modules/react-dom/cjs/react-dom-client.development.js:14462:44

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[2/24]⎯

 FAIL  src/test/examples/rbac-usage-examples.test.tsx > RBAC Test Utilities - Usage Examples > Form Testing > should test form field permissions
TestingLibraryElementError: Found multiple elements by: [data-testid="name-field"]

Here are the matching elements:

Ignored nodes: comments, script, style
<input
  data-testid="name-field"
/>

Ignored nodes: comments, script, style
<input
  data-testid="name-field"
/>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <form>
      <input
        data-testid="name-field"
      />
      <input
        data-testid="admin-field"
      />
      <button
        data-testid="submit-button"
      >
        Submit
      </button>
    </form>
  </div>
  <div>
    <form>
      <input
        data-testid="name-field"
      />
      <input
        data-testid="admin-field"
      />
      <button
        data-testid="submit-button"
      >
        Submit
      </button>
    </form>
  </div>
</body>
 ❯ Object.getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ Object.testFieldPermissions src/test/utils/rbac-component-helpers.tsx:21:30
     19|         rbacRender.scenario().asRole(role as any).render(formComponent)
     20|
     21|         const field = screen.queryByTestId(fieldTest.fieldTestId)
       |                              ^
     22|
     23|         switch (expectedState) {
 ❯ src/test/examples/rbac-usage-examples.test.tsx:145:34

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[3/24]⎯

 FAIL  src/test/examples/rbac-usage-examples.test.tsx > RBAC Test Utilities - Usage Examples > Navigation Testing > should test menu visibility
Error: expect(element).not.toBeInTheDocument()

expected document not to contain element, found <a
  data-testid="users-link"
>
  Users
</a> instead
 ❯ Object.testMenuVisibility src/test/utils/rbac-component-helpers.tsx:90:35
     88|           expect(menuElement).toBeInTheDocument()
     89|         } else {
     90|           expect(menuElement).not.toBeInTheDocument()
       |                                   ^
     91|         }
     92|       }
 ❯ src/test/examples/rbac-usage-examples.test.tsx:184:40

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[4/24]⎯

 FAIL  src/test/examples/rbac-usage-examples.test.tsx > RBAC Test Utilities - Usage Examples > Error State Testing > should test permission-based errors
TestingLibraryElementError: Found multiple elements with the text: Access Denied: Insufficient permissions

Here are the matching elements:

Ignored nodes: comments, script, style
<div>
  Access Denied: Insufficient permissions
</div>

Ignored nodes: comments, script, style
<div>
  Access Denied: Insufficient permissions
</div>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div>
      <div>
        Access Denied: Insufficient permissions
      </div>
    </div>
  </div>
  <div>
    <div>
      <div>
        Access Denied: Insufficient permissions
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div>
        <div>
          Access Denied: Insufficient permissions
        </div>
      </div>
    </div>
    <div>
      <div>
        <div>
          Access Denied: Insufficient permissions
        </div>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ Object.testPermissionErrors src/test/utils/rbac-component-helpers.tsx:202:15
    200|
    201|       if (scenario.expectedError) {
    202|         await waitFor(() => {
       |               ^
    203|           expect(screen.getByText(scenario.expectedError!)).toBeInTheDocument()
    204|         })
 ❯ src/test/examples/rbac-usage-examples.test.tsx:256:7

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[5/24]⎯

 FAIL  src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should allow API calls for users with correct permissions
TestingLibraryElementError: Unable to find an element by: [data-testid="success-message"]

Ignored nodes: comments, script, style
<body>
  <div>
    <div>
      <button
        data-testid="create-user-btn"
      >
        Create User
      </button>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div>
        <button
          data-testid="create-user-btn"
        >
          Create User
        </button>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/test/scenarios/api-permission-integration.test.tsx:63:13
     61|       await user.click(createButton)
     62|
     63|       await waitFor(() => {
       |             ^
     64|         expect(screen.getByTestId('success-message')).toBeInTheDocument()
     65|       })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[6/24]⎯

 FAIL  src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should reject API calls for users without permissions
TestingLibraryElementError: Unable to find an element by: [data-testid="permission-error"]

Ignored nodes: comments, script, style
<body>
  <div>
    <div>
      <button
        data-testid="create-user-btn"
      >
        Create User
      </button>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div>
        <button
          data-testid="create-user-btn"
        >
          Create User
        </button>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/test/scenarios/api-permission-integration.test.tsx:90:13
     88|       await user.click(createButton)
     89|
     90|       await waitFor(() => {
       |             ^
     91|         expect(screen.getByTestId('permission-error')).toBeInTheDocument()
     92|       })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[7/24]⎯

 FAIL  src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Role-Based Data Filtering > should filter user data based on role permissions
Error: expect(element).not.toBeInTheDocument()

expected document not to contain element, found <div
  data-testid="loading"
>
  Loading...
</div> instead

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div
        data-testid="loading"
      >
        Loading...
      </div>
    </div>
  </body>
</html>
 ❯ src/test/scenarios/api-permission-integration.test.tsx:130:53
    128|
    129|       await waitFor(() => {
    130|         expect(screen.queryByTestId('loading')).not.toBeInTheDocument()
       |                                                     ^
    131|       })
    132|
 ❯ runWithExpensiveErrorDiagnosticsDisabled node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/config.js:47:12
 ❯ checkCallback node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/wait-for.js:124:77
 ❯ Timeout.checkRealTimersCallback node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/wait-for.js:118:16

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[8/24]⎯

 FAIL  src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Role-Based Data Filtering > should limit data for lower privilege users
Error: expect(element).toHaveTextContent()

Expected element to have text content:
  Users: 0
Received:
  Users: 2
 ❯ src/test/scenarios/api-permission-integration.test.tsx:180:25
    178|       // Viewer should see no users due to permission restrictions
    179|       const userCount = screen.getByTestId('user-count')
    180|       expect(userCount).toHaveTextContent('Users: 0')
       |                         ^
    181|     })
    182|   })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[9/24]⎯

 FAIL  src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
TestingLibraryElementError: Unable to find an element by: [data-testid="success-message"]

Ignored nodes: comments, script, style
<body>
  <div>
    <div>
      <button
        data-testid="delete-btn"
      >
        Delete User
      </button>
      <div
        data-testid="error-message"
      >
        An unexpected error occurred
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div>
        <button
          data-testid="delete-btn"
        >
          Delete User
        </button>
        <div
          data-testid="error-message"
        >
          An unexpected error occurred
        </div>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/test/scenarios/api-permission-integration.test.tsx:258:13
    256|       await user.click(deleteButton)
    257|
    258|       await waitFor(() => {
       |             ^
    259|         expect(screen.getByTestId('success-message')).toHaveTextContent(
    260|           'User deleted successfully'

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[10/24]⎯

 FAIL  src/test/scenarios/multi-tenant-isolation.test.tsx > Multi-Tenant Permission Isolation Scenarios > Tenant Data Isolation > should isolate tenant data access
TestingLibraryElementError: Found multiple elements by: [data-testid="tenant-header"]

Here are the matching elements:

Ignored nodes: comments, script, style
<h1
  data-testid="tenant-header"
>
  Tenant:
  tenant-1
</h1>

Ignored nodes: comments, script, style
<h1
  data-testid="tenant-header"
>
  Tenant:
  tenant-2
</h1>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div>
      <h1
        data-testid="tenant-header"
      >
        Tenant:
        tenant-1
      </h1>
      <div
        data-testid="tenant-users"
      >
        Users for
        tenant-1
      </div>
      <div
        data-testid="tenant-roles"
      >
        Roles for
        tenant-1
      </div>
    </div>
  </div>
  <div>
    <div>
      <h1
        data-testid="tenant-header"
      >
        Tenant:
        tenant-2
      </h1>
      <div
        data-testid="tenant-users"
      >
        Users for
        tenant-2
      </div>
      <div
        data-testid="tenant-roles"
      >
        Roles for
        tenant-2
      </div>
    </div>
  </div>
</body>
 ❯ Object.getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ src/test/scenarios/multi-tenant-isolation.test.tsx:34:21
     32|         .render(<TenantDataComponent tenantId="tenant-2" />)
     33|
     34|       expect(screen.getByTestId('tenant-header')).toHaveTextContent('Tenant: tenant-2')
       |                     ^
     35|     })
     36|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[11/24]⎯

 FAIL  src/test/scenarios/multi-tenant-isolation.test.tsx > Multi-Tenant Permission Isolation Scenarios > Tenant-Scoped Permissions > should scope permissions to specific tenants
Error: expect(element).not.toBeInTheDocument()

expected document not to contain element, found <button
  data-testid="view-all-tenants"
>
  View All Tenants
</button> instead
 ❯ src/test/scenarios/multi-tenant-isolation.test.tsx:82:60
     80|
     81|       // But should not see cross-tenant admin features
     82|       expect(screen.queryByTestId('view-all-tenants')).not.toBeInTheDocument()
       |                                                            ^
     83|     })
     84|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[12/24]⎯

 FAIL  src/test/scenarios/multi-tenant-isolation.test.tsx > Multi-Tenant Permission Isolation Scenarios > Tenant Context Switching > should restrict tenant switching for single-tenant users
Error: expect(element).toBeDisabled()

Received element is not disabled:
  <select
  data-testid="tenant-switcher"
/>
 ❯ src/test/scenarios/multi-tenant-isolation.test.tsx:129:26
    127|       const switcher = screen.queryByTestId('tenant-switcher')
    128|       if (switcher) {
    129|         expect(switcher).toBeDisabled()
       |                          ^
    130|       }
    131|     })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[13/24]⎯

 FAIL  src/test/scenarios/permission-component-patterns.test.tsx > RBAC Permission Component Patterns > Conditional UI Rendering > should show/hide UI elements based on user permissions
TestingLibraryElementError: Found multiple elements by: [data-testid="page-title"]

Here are the matching elements:

Ignored nodes: comments, script, style
<h1
  data-testid="page-title"
>
  User Management
</h1>

Ignored nodes: comments, script, style
<h1
  data-testid="page-title"
>
  User Management
</h1>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div>
      <h1
        data-testid="page-title"
      >
        User Management
      </h1>
      <button
        data-testid="create-user-btn"
      >
        Create User
      </button>
      <button
        data-testid="export-users-btn"
      >
        Export Users
      </button>
      <button
        data-testid="delete-user-btn"
      >
        Delete User
      </button>
      <section
        data-testid="admin-settings"
      >
        <h2>
          Admin Settings
        </h2>
      </section>
      <nav
        data-testid="role-nav"
      >
        <a
          href="/roles"
        >
          Manage Roles
        </a>
      </nav>
    </div>
  </div>
  <div>
    <div>
      <h1
        data-testid="page-title"
      >
        User Management
      </h1>
      <button
        data-testid="create-user-btn"
      >
        Create User
      </button>
      <button
        data-testid="export-users-btn"
      >
        Export Users
      </button>
      <button
        data-testid="delete-user-btn"
      >
        Delete User
      </button>
      <section
        data-testid="admin-settings"
      >
        <h2>
          Admin Settings
        </h2>
      </section>
      <nav
        data-testid="role-nav"
      >
        <a
          href="/roles"
        >
          Manage Roles
        </a>
      </nav>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div>
        <h1
          data-testid="page-title"
        >
          User Management
        </h1>
        <button
          data-testid="create-user-btn"
        >
          Create User
        </button>
        <button
          data-testid="export-users-btn"
        >
          Export Users
        </button>
        <button
          data-testid="delete-user-btn"
        >
          Delete User
        </button>
        <section
          data-testid="admin-settings"
        >
          <h2>
            Admin Settings
          </h2>
        </section>
        <nav
          data-testid="role-nav"
        >
          <a
            href="/roles"
          >
            Manage Roles
          </a>
        </nav>
      </div>
    </div>
    <div>
      <div>
        <h1
          data-testid="page-title"
        >
          User Management
        </h1>
        <button
          data-testid="create-user-btn"
        >
          Create User
        </button>
        <button
          data-testid="export-users-btn"
        >
          Export Users
        </button>
        <button
          data-testid="delete-user-btn"
        >
          Delete User
        </button>
        <section
          data-testid="admin-settings"
        >
          <h2>
            Admin Settings
          </h2>
        </section>
        <nav
          data-testid="role-nav"
        >
          <a
            href="/roles"
          >
            Manage Roles
          </a>
        </nav>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ Object.testAllRoles src/test/utils/rbac-test-utils.tsx:315:17
    313|       if (expectVisible) {
    314|         for (const testId of expectVisible) {
    315|           await waitFor(() => {
       |                 ^
    316|             expect(screen.getByTestId(testId)).toBeInTheDocument()
    317|           })
 ❯ src/test/scenarios/permission-component-patterns.test.tsx:37:7

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[14/24]⎯

 FAIL  src/test/scenarios/permission-component-patterns.test.tsx > RBAC Permission Component Patterns > Form Field Permissions > should enable/disable form fields based on permissions
TestingLibraryElementError: Found multiple elements by: [data-testid="first-name"]

Here are the matching elements:

Ignored nodes: comments, script, style
<input
  data-testid="first-name"
  placeholder="First Name"
/>

Ignored nodes: comments, script, style
<input
  data-testid="first-name"
  placeholder="First Name"
/>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <form
      data-testid="user-form"
    >
      <input
        data-testid="first-name"
        placeholder="First Name"
      />
      <input
        data-testid="last-name"
        placeholder="Last Name"
      />
      <input
        data-testid="email"
        placeholder="Email"
      />
      <select
        data-testid="role-select"
      >
        <option
          value="User"
        >
          User
        </option>
        <option
          value="Manager"
        >
          Manager
        </option>
        <option
          value="Admin"
        >
          Admin
        </option>
      </select>
      <fieldset
        data-testid="system-settings"
      >
        <legend>
          System Settings
        </legend>
        <input
          data-testid="api-access"
          type="checkbox"
        />
        <input
          data-testid="debug-mode"
          type="checkbox"
        />
      </fieldset>
      <fieldset
        data-testid="tenant-settings"
      >
        <legend>
          Tenant Configuration
        </legend>
        <input
          data-testid="tenant-name"
          placeholder="Tenant Name"
        />
      </fieldset>
      <button
        data-testid="submit-btn"
        type="submit"
      >
        Save User
      </button>
    </form>
  </div>
  <div>
    <form
      data-testid="user-form"
    >
      <input
        data-testid="first-name"
        placeholder="First Name"
      />
      <input
        data-testid="last-name"
        placeholder="Last Name"
      />
      <input
        data-testid="email"
        placeholder="Email"
      />
      <select
        data-testid="role-select"
      >
        <option
          value="User"
        >
          User
        </option>
        <option
          value="Manager"
        >
          Manager
        </option>
        <option
          value="Admin"
        >
          Admin
        </option>
      </select>
      <fieldset
        data-testid="system-settings"
      >
        <legend>
          System Settings
        </legend>
        <input
          data-testid="api-access"
          type="checkbox"
        />
        <input
          data-testid="debug-mode"
          type="checkbox"
        />
      </fieldset>
      <fieldset
        data-testid="tenant-settings"
      >
        <legend>
          Tenant Configuration
        </legend>
        <input
          data-testid="tenant-name"
          placeholder="Tenant Name"
        />
      </fieldset>
      <button
        data-testid="submit-btn"
        type="submit"
      >
        Save User
      </button>
    </form>
  </div>
</body>
 ❯ Object.getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ Object.testFieldPermissions src/test/utils/rbac-component-helpers.tsx:21:30
     19|         rbacRender.scenario().asRole(role as any).render(formComponent)
     20|
     21|         const field = screen.queryByTestId(fieldTest.fieldTestId)
       |                              ^
     22|
     23|         switch (expectedState) {
 ❯ src/test/scenarios/permission-component-patterns.test.tsx:116:34

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[15/24]⎯

 FAIL  src/test/scenarios/permission-component-patterns.test.tsx > RBAC Permission Component Patterns > Navigation Menu Permissions > should show navigation items based on user permissions
TestingLibraryElementError: Found multiple elements by: [data-testid="dashboard-link"]

Here are the matching elements:

Ignored nodes: comments, script, style
<a
  data-testid="dashboard-link"
  href="/dashboard"
>
  Dashboard
</a>

Ignored nodes: comments, script, style
<a
  data-testid="dashboard-link"
  href="/dashboard"
>
  Dashboard
</a>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <nav
      data-testid="main-nav"
    >
      <ul>
        <li>
          <a
            data-testid="dashboard-link"
            href="/dashboard"
          >
            Dashboard
          </a>
        </li>
        <li>
          <a
            data-testid="users-link"
            href="/users"
          >
            Users
          </a>
        </li>
        <li>
          <a
            data-testid="roles-link"
            href="/roles"
          >
            Roles
          </a>
        </li>
        <li>
          <a
            data-testid="reports-link"
            href="/reports"
          >
            Reports
          </a>
        </li>
        <li>
          <a
            data-testid="tenants-link"
            href="/tenants"
          >
            Tenants
          </a>
        </li>
        <li>
          <a
            data-testid="system-link"
            href="/system"
          >
            System
          </a>
        </li>
      </ul>
    </nav>
  </div>
  <div>
    <nav
      data-testid="main-nav"
    >
      <ul>
        <li>
          <a
            data-testid="dashboard-link"
            href="/dashboard"
          >
            Dashboard
          </a>
        </li>
        <li>
          <a
            data-testid="users-link"
            href="/users"
          >
            Users
          </a>
        </li>
        <li>
          <a
            data-testid="roles-link"
            href="/roles"
          >
            Roles
          </a>
        </li>
        <li>
          <a
            data-testid="reports-link"
            href="/reports"
          >
            Reports
          </a>
        </li>
        <li>
          <a
            data-testid="tenants-link"
            href="/tenants"
          >
            Tenants
          </a>
        </li>
        <li>
          <a
            data-testid="system-link"
            href="/system"
          >
            System
          </a>
        </li>
      </ul>
    </nav>
  </div>
</body>
 ❯ Object.getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ Object.testMenuVisibility src/test/utils/rbac-component-helpers.tsx:85:36
     83|       for (const menuItem of menuItems) {
     84|         const shouldBeVisible = menuItem.visibleForRoles.includes(role)
     85|         const menuElement = screen.queryByTestId(menuItem.testId)
       |                                    ^
     86|
     87|         if (shouldBeVisible) {
 ❯ src/test/scenarios/permission-component-patterns.test.tsx:185:40

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[16/24]⎯

 FAIL  src/test/scenarios/permission-component-patterns.test.tsx > RBAC Permission Component Patterns > Data Table Action Buttons > should show/hide action buttons based on permissions
AssertionError: expected 4 to be +0 // Object.is equality

- Expected
+ Received

- 0
+ 4

 ❯ Object.testTableActionButtons src/test/utils/rbac-component-helpers.tsx:120:41
    118|           expect(buttonElements.length).toBeGreaterThan(0)
    119|         } else {
    120|           expect(buttonElements.length).toBe(0)
       |                                         ^
    121|         }
    122|       }
 ❯ src/test/scenarios/permission-component-patterns.test.tsx:249:35

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[17/24]⎯

 FAIL  src/test/scenarios/role-hierarchy-scenarios.test.tsx > Role Hierarchy Validation Scenarios > Permission Inheritance > should ensure higher roles have more permissions than lower roles
AssertionError: expected [ 'users.view', 'users.create', …(18) ] to include 'tenants.view'
 ❯ src/test/scenarios/role-hierarchy-scenarios.test.tsx:35:37
     33|         // Ensure all lower role permissions are included in higher role
     34|         lowerPermissions.forEach(permission => {
     35|           expect(higherPermissions).toContain(permission)
       |                                     ^
     36|         })
     37|       }
 ❯ src/test/scenarios/role-hierarchy-scenarios.test.tsx:34:26

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[18/24]⎯

 FAIL  src/test/scenarios/role-hierarchy-scenarios.test.tsx > Role Hierarchy Validation Scenarios > System vs Tenant Role Separation > should separate system-level and tenant-level permissions
TypeError: screen.getByTestId is not a function
 ❯ src/test/scenarios/role-hierarchy-scenarios.test.tsx:62:21
     60|       // System Admin should have system permissions but limited tenant management
     61|       rbacRender.asSystemAdmin(<SystemAdminComponent />)
     62|       expect(screen.getByTestId('system-health')).toBeInTheDocument()
       |                     ^
     63|       expect(screen.getByTestId('global-settings')).toBeInTheDocument()
     64|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[19/24]⎯

 FAIL  src/components/__tests__/roles/RoleList.test.tsx > RoleList > handles pagination correctly
AssertionError: expected "getRoles" to be called with arguments: [ { page: 2, pageSize: 10, …(1) } ]

Received:

  1st getRoles call:

  Array [
    Object {
-     "page": 2,
+     "page": 1,
      "pageSize": 10,
-     "searchTerm": "",
+     "searchTerm": undefined,
    },
  ]

  2nd getRoles call:

  Array [
    Object {
-     "page": 2,
+     "page": 1,
      "pageSize": 10,
-     "searchTerm": "",
+     "searchTerm": undefined,
    },
  ]

  3rd getRoles call:

  Array [
    Object {
      "page": 2,
      "pageSize": 10,
-     "searchTerm": "",
+     "searchTerm": undefined,
    },
  ]


Number of calls: 3

 ❯ src/components/__tests__/roles/RoleList.test.tsx:164:34
    162|     await user.click(nextPageButton)
    163|
    164|     expect(roleService.getRoles).toHaveBeenCalledWith({
       |                                  ^
    165|       page: 2,
    166|       pageSize: 10,

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[20/24]⎯

 FAIL  src/components/__tests__/roles/RoleList.test.tsx > RoleList > calls onDeleteRole when delete button is clicked
AssertionError: expected "spy" to be called with arguments: [ 1 ]

Received:



Number of calls: 0

 ❯ src/components/__tests__/roles/RoleList.test.tsx:219:32
    217|     if (deleteButton) {
    218|       await user.click(deleteButton)
    219|       expect(mockOnDeleteRole).toHaveBeenCalledWith(1)
       |                                ^
    220|     }
    221|   })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[21/24]⎯

 FAIL  src/components/__tests__/roles/RoleList.test.tsx > RoleList > shows error state when loading fails
TestingLibraryElementError: Unable to find an element with the text: /Failed to fetch roles/. This could be because the text is broken up by multiple elements. In this case, you can provide a function for your text matcher to make your matcher more flexible.

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      class="MuiBox-root css-19midj6"
    >
      <p
        class="MuiTypography-root MuiTypography-body1 css-lcmie7-MuiTypography-root"
      >
        Failed to load
      </p>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head>
    <meta
      content=""
      name="emotion-insertion-point"
    />
  </head>
  <body>
    <div>
      <div
        class="MuiBox-root css-19midj6"
      >
        <p
          class="MuiTypography-root MuiTypography-body1 css-lcmie7-MuiTypography-root"
        >
          Failed to load
        </p>
      </div>
    </div>
  </body>
</html>...
 ❯ Proxy.waitForWrapper node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/components/__tests__/roles/RoleList.test.tsx:277:11
    275|     )
    276|
    277|     await waitFor(() => {
       |           ^
    278|       expect(screen.getByText(/Failed to fetch roles/)).toBeInTheDocument()
    279|     })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[22/24]⎯

 FAIL  src/components/__tests__/roles/RoleList.test.tsx > RoleList > displays role information correctly
TestingLibraryElementError: Found multiple elements with the text: Custom

Here are the matching elements:

Ignored nodes: comments, script, style
<span
  class="MuiChip-label MuiChip-labelSmall css-eccknh-MuiChip-label"
>
  Custom
</span>

Ignored nodes: comments, script, style
<span
  class="MuiChip-label MuiChip-labelSmall css-eccknh-MuiChip-label"
>
  Custom
</span>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      class="MuiBox-root css-0"
    >
      <div
        class="MuiBox-root css-1qm1lh"
      >
        <div
          class="MuiFormControl-root MuiFormControl-fullWidth MuiTextField-root css-cmpglg-MuiFormControl-root-MuiTextField-root"
        >
          <div
            class="MuiInputBase-root MuiOutlinedInput-root MuiInputBase-colorPrimary MuiInputBase-fullWidth MuiInputBase-formControl MuiInputBase-adornedStart css-ot336b-MuiInputBase-root-MuiOutlinedInput-root"
          >
            <div
              class="MuiInputAdornment-root MuiInputAdornment-positionStart MuiInputAdornment-outlined MuiInputAdornment-sizeMedium css-1nowbqt-MuiInputAdornment-root"
            >
              <span
                aria-hidden="true"
                class="notranslate"
              >
                ​
              </span>
              <svg
                aria-hidden="true"
                class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
                data-testid="SearchIcon"
                focusable="false"
                viewBox="0 0 24 24"
              >
                <path
                  d="M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14"
                />
              </svg>
            </div>
            <input
              aria-invalid="false"
              class="MuiInputBase-input MuiOutlinedInput-input MuiInputBase-inputAdornedStart css-2u11ia-MuiInputBase-input-MuiOutlinedInput-input"
              id="«r2i»"
              placeholder="Search roles..."
              type="text"
              value=""
            />
            <fieldset
              aria-hidden="true"
              class="MuiOutlinedInput-notchedOutline css-18p5xg2-MuiNotchedOutlined-root-MuiOutlinedInput-notchedOutline"
            >
              <legend
                class="css-1nf2c5d-MuiNotchedOutlined-root"
              >
                <span
                  aria-hidden="true"
                  class="notranslate"
                >
                  ​
                </span>
              </legend>
            </fieldset>
          </div>
        </div>
      </div>
      <div
        class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiTableContainer-root css-y2ff7i-MuiPaper-root-MuiTableContainer-root"
        style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
      >
        <table
          class="MuiTable-root css-1xwxv7r-MuiTable-root"
        >
          <thead
            class="MuiTableHead-root css-1a7iywq-MuiTableHead-root"
          >
            <tr
              class="MuiTableRow-root MuiTableRow-head css-1xtozoh-MuiTableRow-root"
            >
              <th
                class="MuiTableCell-root MuiTableCell-head MuiTableCell-sizeMedium css-1orzuox-MuiTableCell-root"
                scope="col"
              >
                Name
              </th>
              <th
                class="MuiTableCell-root MuiTableCell-head MuiTableCell-sizeMedium css-1orzuox-MuiTableCell-root"
                scope="col"
              >
                Description
              </th>
              <th
                class="MuiTableCell-root MuiTableCell-head MuiTableCell-sizeMedium css-1orzuox-MuiTableCell-root"
                scope="col"
              >
                Type
              </th>
              <th
                class="MuiTableCell-root MuiTableCell-head MuiTableCell-sizeMedium css-1orzuox-MuiTableCell-root"
                scope="col"
              >
                Permissions
              </th>
              <th
                class="MuiTableCell-root MuiTableCell-head MuiTableCell-sizeMedium css-1orzuox-MuiTableCell-root"
                scope="col"
              >
                Users
              </th>
              <th
                class="MuiTableCell-root MuiTableCell-head MuiTableCell-sizeMedium css-1orzuox-MuiTableCell-root"
                scope="col"
              >
                Actions
              </th>
            </tr>
          </thead>
          <tbody
            class="MuiTableBody-root css-gmh7jj-MuiTableBody-root"
          >
            <tr
              class="MuiTableRow-root css-1xtozoh-MuiTableRow-root"
            >
              <td
                class="MuiTableCell-root MuiTableCell-body MuiTableCell-sizeMedium css-1dc80h3-MuiTableCell-root"
              >
                Admin
              </td>
              <td
                class="MuiTableCell-root MuiTableCell-body MuiTableCell-sizeMedium css-1dc80h3-MuiTableCell-root"
              >
                Administrator role
              </td>
              <td
                class="MuiTableCell-root MuiTableCell-body MuiTableCell-sizeMedium css-1dc80h3-MuiTableCell-root"
              >
                <div
                  class="MuiChip-root MuiChip-filled MuiChip-sizeSmall MuiChip-colorDefault MuiChip-filledDefault css-1lflu6f-MuiChip-root"
                >...
 ❯ Object.getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ src/components/__tests__/roles/RoleList.test.tsx:296:19
    294|     expect(screen.getByText('2')).toBeInTheDocument() // Permission count for Admin
    295|     expect(screen.getByText('5')).toBeInTheDocument() // User count for Admin
    296|     expect(screen.getByText('Custom')).toBeInTheDocument() // Role type chip
       |                   ^
    297|   })
    298|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[23/24]⎯

 FAIL  src/components/__tests__/roles/RoleList.test.tsx > RoleList > handles page size changes
Error: Unable to perform pointer interaction as the element has `pointer-events: none`:

INPUT
 ❯ assertPointerEvents node_modules/@testing-library/user-event/dist/esm/utils/pointer/cssPointerEvents.js:45:15
 ❯ Object.enter node_modules/@testing-library/user-event/dist/esm/system/pointer/pointer.js:50:17
 ❯ PointerHost.move node_modules/@testing-library/user-event/dist/esm/system/pointer/index.js:51:85
 ❯ pointerAction node_modules/@testing-library/user-event/dist/esm/pointer/index.js:57:39
 ❯ Object.pointer node_modules/@testing-library/user-event/dist/esm/pointer/index.js:25:15
 ❯ src/components/__tests__/roles/RoleList.test.tsx:312:5
    310|     // Find the rows per page selector
    311|     const pageSizeSelect = screen.getByDisplayValue('10')
    312|     await user.click(pageSizeSelect)
       |     ^
    313|
    314|     const option25 = screen.getByRole('option', { name: '25' })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[24/24]⎯

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯ Unhandled Errors ⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯

Vitest caught 1 unhandled error during the test run.
This might cause false positive tests. Resolve unhandled errors to make sure your tests are not affected.

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯ Unhandled Rejection ⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯
InternalError: [MSW] Cannot bypass a request when using the "error" strategy for the "onUnhandledRequest" option.
 ❯ applyStrategy node_modules/msw/src/core/utils/request/onUnhandledRequest.ts:42:15
 ❯ onUnhandledRequest node_modules/msw/src/core/utils/request/onUnhandledRequest.ts:79:5
 ❯ handleRequest node_modules/msw/src/core/utils/handleRequest.ts:79:11
 ❯ _Emitter.<anonymous> node_modules/msw/src/node/SetupServerCommonApi.ts:60:26
 ❯ emitAsync node_modules/@mswjs/interceptors/src/utils/emitAsync.ts:23:5
 ❯ node_modules/@mswjs/interceptors/src/utils/handleRequest.ts:145:5
 ❯ until node_modules/@open-draft/until/src/until.ts:23:18
 ❯ handleRequest node_modules/@mswjs/interceptors/src/utils/handleRequest.ts:134:18
 ❯ globalThis.fetch node_modules/@mswjs/interceptors/src/interceptors/fetch/index.ts:72:32

This error originated in "src/test/scenarios/api-permission-integration.test.tsx" test file. It doesn't mean the error was thrown inside the file itself, but while it was running.
The latest test that might've caused the error is "should filter user data based on role permissions". It might mean one of the following:
- The error was thrown, while Vitest was running this test.
- If the error occurred after the test had been completed, this was the last documented test before it was thrown.
⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯

 Test Files  6 failed | 10 passed (16)
      Tests  24 failed | 103 passed (127)
     Errors  1 error
   Start at  12:08:08
   Duration  29.81s (transform 3.03s, setup 16.94s, collect 185.13s, tests 45.17s, environment 17.84s, prepare 5.65s)
