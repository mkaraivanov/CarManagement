# OCR Investigation Summary

## Problem Statement
Vehicle registration document upload and OCR extraction failing with error: **"Failed to process document"**

## Root Cause Identified

The **Tesseract .NET package (v5.2.0) does not properly support macOS ARM (Apple Silicon)**.

### Technical Details:
- The package includes only x64 (Intel) and x86 (32-bit) Windows DLL files
- No native libraries are included for macOS (Intel or ARM)
- The package expects to find native libraries in platform-specific folders (x64, x86, etc.)
- On macOS ARM, the InteropDotNet library loader looks for "platform x64" libraries, which don't exist for ARM architecture
- The package predates widespread Apple Silicon adoption and hasn't been updated

## What Was Fixed

### ✅ 1. Tesseract Data Path Resolution
**File**: [`Services/TesseractOcrService.cs`](Services/TesseractOcrService.cs)
**Fix**: Updated constructor to use `IWebHostEnvironment.ContentRootPath` to properly resolve relative paths
**Impact**: Tesseract can now find its trained data files (`tessdata/eng.traineddata`)

### ✅ 2. Test Configuration
**File**: [`Backend.Tests/TestWebApplicationFactory.cs`](Backend.Tests/TestWebApplicationFactory.cs)
**Fix**: Configure content root path for test environment
**Impact**: Tests can now properly initialize the application

### ✅ 3. Native Library Installation
**Installed via Homebrew**:
- `tesseract` v5.5.2 (with leptonica v1.87.0)
- `imagemagick` v7.1.2-13

**Location**: `/opt/homebrew/lib/`

### ✅ 4. Package Updates
**File**: [`Backend.csproj`](Backend.csproj)
**Change**: Updated `Magick.NET-Q16-AnyCPU` from v13.6.0 (with vulnerabilities) to v14.10.2
**Impact**: Removed 18 security vulnerability warnings

### ✅ 5. Startup Script
**File**: [`run-backend.sh`](run-backend.sh)
**Purpose**: Sets `DYLD_LIBRARY_PATH` environment variable for native library discovery
**Usage**: `./run-backend.sh` instead of `dotnet run`

### ✅ 6. Comprehensive Integration Tests
**File**: [`Backend.Tests/RegistrationOcrTests.cs`](Backend.Tests/RegistrationOcrTests.cs)
**Tests Created** (5 total):
1. Authentication required
2. File required validation
3. File type validation
4. Simple image OCR extraction
5. PDF OCR extraction

**Test Results**:
- ✅ 3 tests passing (validation and security)
- ❌ 2 tests failing (actual OCR extraction)

## Attempts to Fix Native Library Loading

### Attempt 1: Environment Variables
Set `DYLD_LIBRARY_PATH` and `DYLD_FALLBACK_LIBRARY_PATH` to `/opt/homebrew/lib`
- **Result**: Blocked by macOS System Integrity Protection (SIP) during test execution

### Attempt 2: Global Symlinks
Created symlinks in `/opt/homebrew/lib/`:
```bash
libleptonica-1.82.0.dylib -> libleptonica.6.dylib
```
- **Result**: Library loader still couldn't find libraries (not in search path)

### Attempt 3: NuGet Package Folder Symlinks
Created symlinks in `~/.nuget/packages/tesseract/5.2.0/x64/`:
```bash
libleptonica-1.82.0.dylib -> /opt/homebrew/lib/libleptonica.6.dylib
libtesseract50.dylib -> /opt/homebrew/lib/libtesseract.5.dylib
```
- **Result**: Still not found - InteropDotNet doesn't search NuGet package folders at runtime

## Current Status

✅ **OCR extraction now working!** - Successfully switched to Tesseract CLI wrapper approach.

**Solution Implemented**: Option 3 - Use Tesseract via CLI Wrapper

All 5 integration tests passing:
- ✅ Authentication validation
- ✅ File requirement validation
- ✅ File type validation
- ✅ Image OCR extraction (simple images)
- ✅ PDF OCR extraction (2295 characters from sample registration document)

## Recommended Solutions

### Option 1: Switch to Cloud OCR Service ⭐ **RECOMMENDED**
**Pros:**
- No native library dependencies
- Better accuracy (Google Vision, AWS Textract, Azure Computer Vision)
- Handles multiple document formats natively
- Scalable
- Regular updates and improvements

**Cons:**
- Requires API keys and setup
- Cost per API call (usually very cheap)
- Requires internet connection

**Example**: Google Cloud Vision API, Azure Computer Vision, AWS Textract

### Option 2: Use IronTesseract
**Package**: `IronOcr` or `IronTesseract`
**Pros:**
- Maintained commercial product with macOS ARM support
- Better .NET integration
- No manual native library setup

**Cons:**
- Requires license for production use
- Commercial product

### Option 3: Use Tesseract via CLI Wrapper
Instead of using Tesseract .NET, call the `tesseract` CLI directly via `Process.Start()`

**Pros:**
- Works with already-installed Homebrew Tesseract
- No .NET package compatibility issues
- Full control

**Cons:**
- More complex implementation
- Process overhead
- Text parsing required

**Example**:
```csharp
var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "/opt/homebrew/bin/tesseract",
        Arguments = $"{inputFile} stdout",
        RedirectStandardOutput = true
    }
};
process.Start();
var text = process.StandardOutput.ReadToEnd();
```

### Option 4: Wait for Tesseract .NET Update
Monitor the [Tesseract NuGet package](https://www.nuget.org/packages/Tesseract/) for ARM-native builds

**Pros:**
- Keeps current architecture
- Free and open source

**Cons:**
- Uncertain timeline
- Package appears unmaintained (last update 2022)

## Files Modified

1. [`Services/TesseractOcrService.cs`](Services/TesseractOcrService.cs) - Path resolution, enhanced logging
2. [`Backend.Tests/TestWebApplicationFactory.cs`](Backend.Tests/TestWebApplicationFactory.cs) - Content root configuration
3. [`Backend.csproj`](Backend.csproj) - Updated Magick.NET version
4. [`run-backend.sh`](run-backend.sh) - Created startup script (NEW)
5. [`Backend.Tests/RegistrationOcrTests.cs`](Backend.Tests/RegistrationOcrTests.cs) - Integration tests (NEW)

## Next Steps

1. **Immediate**: Choose one of the recommended solutions above
2. **If choosing Cloud OCR**:
   - Select provider (Google Vision, Azure, AWS)
   - Set up API credentials
   - Update `TesseractOcrService` to call cloud API instead
   - Test with real registration documents
3. **If choosing CLI wrapper**:
   - Implement process-based wrapper
   - Handle output parsing
   - Add error handling for missing tesseract binary
4. **Documentation**: Update CLAUDE.md with chosen solution and setup requirements

## Implementation Details (CLI Wrapper Approach)

### Changes Made

1. **Updated TesseractOcrService.cs**:
   - Removed dependency on Tesseract .NET package
   - Implemented CLI wrapper using `Process.Start()`
   - Saves preprocessed image to temp file
   - Calls tesseract binary directly: `/opt/homebrew/bin/tesseract`
   - Captures stdout for extracted text
   - Maintains ImageMagick preprocessing (working well)

2. **Removed Tesseract NuGet Package**:
   - Removed `Tesseract` v5.2.0 from Backend.csproj
   - No more native library compatibility issues

3. **Added Configuration**:
   - Added `TesseractBinaryPath` to appsettings.json
   - Default: `/opt/homebrew/bin/tesseract`
   - Can be customized for different environments

### Key Benefits

✅ Works on macOS ARM (Apple Silicon)
✅ No native library loading issues
✅ Uses system-installed Tesseract (always up to date)
✅ Simple and maintainable
✅ Full control over tesseract arguments
✅ All existing functionality preserved

### Requirements

- Tesseract must be installed: `brew install tesseract`
- Tesseract trained data files in `backend/tessdata/` directory
- ImageMagick for image preprocessing: `brew install imagemagick`

## Test Commands

```bash
# Run OCR integration tests
cd backend
dotnet test Backend.Tests/Backend.Tests.csproj --filter "RegistrationOcrTests"

# Expected: All 5 tests passing ✅
```

## References

- Tesseract NuGet: https://www.nuget.org/packages/Tesseract/
- Tesseract GitHub: https://github.com/charlesw/tesseract
- Homebrew Tesseract: https://formulae.brew.sh/formula/tesseract
- macOS ARM .NET issues: https://github.com/dotnet/runtime/issues/43313
