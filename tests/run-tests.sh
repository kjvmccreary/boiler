#!/bin/bash

echo "Running AuthService Unit Tests..."
echo "=================================="

# Navigate to the test directory
cd "$(dirname "$0")/unit/AuthService.Tests"

# Run tests with coverage
dotnet test --logger "trx;LogFileName=TestResults.trx" \
           --logger "console;verbosity=detailed" \
           --collect:"XPlat Code Coverage" \
           --results-directory ./TestResults

echo ""
echo "Test run completed!"
echo "Results saved to: $(pwd)/TestResults"
