#!/bin/bash

echo "🛑 Stopping Complete Boiler Stack..."

docker-compose -f docker/docker-compose.yml down

echo "✅ Complete stack stopped successfully!"
echo ""
echo "💡 To start again: ./scripts/start-full-stack.sh"
