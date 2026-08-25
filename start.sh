#!/usr/bin/env bash

# Exit immediately if a command exits with a non-zero status
set -e

# Get the root directory of the script
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "=================================================="
echo "🚀 Starting ONES To-Do List Application (Full Stack)"
echo "=================================================="

# 1. Start Backend (.NET Core Web API)
echo "📂 Navigating to TodoListApi and starting .NET API..."
cd "$ROOT_DIR/TodoListApi"
dotnet run &
BACKEND_PID=$!
echo "✅ Backend API started with PID $BACKEND_PID (http://localhost:5000)"

# 2. Start Frontend (Angular)
echo "📂 Navigating to TodoListClient and starting Angular..."
cd "$ROOT_DIR/TodoListClient"

# Ensure dependencies are installed if node_modules is missing
if [ ! -d "node_modules" ]; then
    echo "📦 Installing npm dependencies for Angular client..."
    npm install
fi

npm start &
FRONTEND_PID=$!
echo "✅ Frontend Angular started with PID $FRONTEND_PID (http://localhost:4200)"

echo "=================================================="
echo "✨ Both Backend and Frontend are running!"
echo "Press [CTRL+C] at any time to stop both servers."
echo "=================================================="

# Trap SIGINT (Ctrl+C) to kill both background processes gracefully
trap "echo '🛑 Stopping servers...'; kill $BACKEND_PID $FRONTEND_PID; exit 0" SIGINT

# Wait for background processes to finish
wait
