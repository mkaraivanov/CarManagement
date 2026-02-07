#!/bin/bash

# Script to run the backend with proper library paths for Tesseract/Leptonica
# This ensures the native libraries installed via Homebrew are found

export DYLD_LIBRARY_PATH="/opt/homebrew/lib:$DYLD_LIBRARY_PATH"
export DYLD_FALLBACK_LIBRARY_PATH="/opt/homebrew/lib:$DYLD_FALLBACK_LIBRARY_PATH"

echo "Starting backend with Tesseract native library paths configured..."
echo "DYLD_LIBRARY_PATH=$DYLD_LIBRARY_PATH"

cd "$(dirname "$0")"
dotnet run
