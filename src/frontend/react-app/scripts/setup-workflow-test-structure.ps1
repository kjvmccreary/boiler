<#
.SYNOPSIS
  Scaffolds new workflow test suite structure and relocates the existing DSL core test.

.DESCRIPTION
  1. Creates test directories:
     src/features/workflow/tests/
       dsl/
       service/
       builder/
       operations/
       integration-lite/
  2. Moves (or optionally copies) the existing dsl.core.test.ts from legacy location:
     src/features/workflow/dsl/__tests__/dsl.core.test.ts
     → src/features/workflow/tests/dsl/dsl.core.test.ts

  Safe to re-run (idempotent). Will not overwrite existing destination file unless -Force is passed.

.PARAMETER ProjectRoot
  Path to react-app root (folder containing src\features). Defaults to current directory.

.PARAMETER CopyInsteadOfMove
  If set, copies the DSL test instead of moving it.

.PARAMETER Force
  Overwrite existing destination test file if present.

.EXAMPLE
  ./scripts/setup-workflow-test-structure.ps1

.EXAMPLE
  ./scripts/setup-workflow-test-structure.ps1 -ProjectRoot "C:\dev\boiler\src\frontend\react-app" -CopyInsteadOfMove
#>

param(
  [string]$ProjectRoot = (Get-Location).Path,
  [switch]$CopyInsteadOfMove,
  [switch]$Force
)

Write-Host "== Workflow Test Architecture Setup ==" -ForegroundColor Cyan
Write-Host "Project root: $ProjectRoot"

# Resolve paths
$srcPath            = Join-Path $ProjectRoot "src"
$workflowBase       = Join-Path $srcPath "features\workflow"
$legacyDslTestDir   = Join-Path $workflowBase "dsl\__tests__"
$legacyDslTestFile  = Join-Path $legacyDslTestDir "dsl.core.test.ts"
$newTestsRoot       = Join-Path $workflowBase "tests"
$newDslDir          = Join-Path $newTestsRoot "dsl"
$serviceDir         = Join-Path $newTestsRoot "service"
$builderDir         = Join-Path $newTestsRoot "builder"
$operationsDir      = Join-Path $newTestsRoot "operations"
$integrationLiteDir = Join-Path $newTestsRoot "integration-lite"

$dirs = @(
  $newTestsRoot,
  $newDslDir,
  $serviceDir,
  $builderDir,
  $operationsDir,
  $integrationLiteDir
)

# 1. Create directories
foreach ($d in $dirs) {
  if (-not (Test-Path $d)) {
    New-Item -ItemType Directory -Path $d | Out-Null
    Write-Host "[Created] $d" -ForegroundColor Green
  } else {
    Write-Host "[Exists ] $d"
  }
}

# 2. Relocate DSL test
$destDslTestFile = Join-Path $newDslDir "dsl.core.test.ts"

if (Test-Path $legacyDslTestFile) {
  if ((Test-Path $destDslTestFile) -and -not $Force) {
    Write-Host "[Skip] Destination test already exists: $destDslTestFile (use -Force to overwrite)" -ForegroundColor Yellow
  } else {
    if ($CopyInsteadOfMove) {
      Copy-Item $legacyDslTestFile $destDslTestFile -Force:$Force
      Write-Host "[Copied] dsl.core.test.ts → tests/dsl" -ForegroundColor Green
    } else {
      Move-Item $legacyDslTestFile $destDslTestFile -Force:$Force
      Write-Host "[Moved ] dsl.core.test.ts → tests/dsl" -ForegroundColor Green

      # Remove legacy __tests__ dir if now empty
      $remaining = Get-ChildItem $legacyDslTestDir -ErrorAction SilentlyContinue
      if (-not $remaining) {
        Remove-Item $legacyDslTestDir -Force -ErrorAction SilentlyContinue
        Write-Host "[Clean ] Removed empty legacy __tests__ directory."
      }
    }
  }
} else {
  Write-Host "[Warn ] Legacy DSL test not found at: $legacyDslTestFile" -ForegroundColor Yellow
}

# 3. (Optional) Create placeholder README files if absent
function Ensure-Readme {
  param(
    [string]$Dir,
    [string]$Content
  )
  $readme = Join-Path $Dir "README.md"
  if (-not (Test-Path $readme)) {
    $Content | Out-File -FilePath $readme -Encoding UTF8
    Write-Host "[Added ] $readme"
  }
}

Ensure-Readme -Dir $newTestsRoot -Content @"
# Workflow Test Suites

Subfolders:
- dsl: Pure DSL serialization/validation tests.
- service: REST client wrapper tests (MSW-backed).
- builder: ReactFlow builder component tests.
- operations: Definitions / Instances / Tasks UI tests.
- integration-lite: Narrow vertical slice (draft → publish → start → complete).

Naming:
  <area>.<purpose>.test.ts
"@

Ensure-Readme -Dir $newDslDir -Content "# DSL Tests\nTests for serialize/deserialize/validate logic."
Ensure-Readme -Dir $serviceDir -Content "# Service Tests\nWorkflow service client & API contract tests."
Ensure-Readme -Dir $builderDir -Content "# Builder Tests\nReactFlow builder interactions."
Ensure-Readme -Dir $operationsDir -Content "# Operations Tests\nDefinitions list, Tasks inbox, Instance details."
Ensure-Readme -Dir $integrationLiteDir -Content "# Integration-Lite Tests\nHigh-value vertical scenario tests using MSW."

Write-Host "`nDone." -ForegroundColor Cyan
if (-not $CopyInsteadOfMove) {
  Write-Host "Verify tests still pass: npm run test:workflow:dsl (after adding script)."
}
