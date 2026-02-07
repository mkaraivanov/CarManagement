# Vehicle Registration Document OCR Feature - Implementation Plan

## Context

The user wants to add a feature that allows creating vehicles by uploading a **vehicle registration document** (not driver's license). The system should:
- Accept uploaded images/PDFs of US vehicle registration documents
- Extract vehicle data using OCR (Optical Character Recognition)
- Pre-populate vehicle creation forms with extracted data
- Store the registration document for audit purposes
- Add registration-specific fields to track registration expiration, owner info, etc.

**Why this is needed:** Currently users must manually type all vehicle information. OCR will speed up vehicle creation and reduce data entry errors.

## Implementation Status

**Status:** ✅ Backend + Frontend Complete (Testing Pending)

**Completed:**
- ✅ Database migration with registration fields
- ✅ OCR service using Tesseract
- ✅ Registration parser service
- ✅ File storage service
- ✅ API endpoints (`/api/vehicle-registration/extract`, `/upload/{vehicleId}`)
- ✅ Web frontend UI for document upload (`RegistrationUploadDialog.jsx`)
- ✅ File upload component with drag-and-drop
- ✅ OCR data preview and review interface (`ExtractedDataReview.jsx`)
- ✅ Form integration with vehicle creation (`VehicleForm.jsx`)
- ✅ Registration service for API calls (`registrationService.js`)

**Remaining:**
- ❌ Integration tests for registration endpoints
- ❌ End-to-end testing with sample documents
- ❌ User acceptance testing with real registration images

## Technology Decisions

### OCR: Tesseract (Local, Free)
- **Library:** Tesseract via .NET NuGet package
- **Why:** Free, open-source, runs locally (no API costs), sufficient accuracy for structured documents
- **Alternative:** Azure Computer Vision (more accurate but requires paid API subscription)

### File Storage: Local File System
- **Structure:** `/uploads/{userId}/registrations/{vehicleId}/registration.jpg`
- **Why:** Simple, no external dependencies, can migrate to cloud storage later
- **Alternative:** Azure Blob Storage / AWS S3 (better for scale, but adds complexity)

### API Design: Two-Step Process
1. **POST /api/vehicle-registration/extract** - Upload document, get extracted data (JSON response)
2. User reviews/edits extracted data in frontend form
3. **POST /api/vehicles** (existing endpoint) - Create vehicle with reviewed data + document URL

**Why two-step:** OCR is never 100% accurate. Users need to review/correct before saving.

## Database Schema Changes

### New Fields for Vehicle Model

Add to [Vehicle.cs](../../backend/Models/Vehicle.cs):

**Registration Fields:**
- `RegistrationNumber` (string, nullable) - Unique registration ID
- `RegistrationIssueDate` (DateTime, nullable) - When registration was issued
- `RegistrationExpiryDate` (DateTime, nullable) - When registration expires
- `RegistrationDocumentUrl` (string, nullable) - Path to uploaded document
- `RegistrationStatus` (enum) - Active, Expired, Suspended, etc.

**Owner Information:**
- `OwnerName` (string, nullable) - Registered owner from document
- `OwnerAddress` (string, nullable) - Owner's registered address

**Vehicle Specifications:**
- `BodyType` (string, nullable) - Sedan, SUV, Truck, etc.
- `EngineInfo` (string, nullable) - Engine size/type
- `FuelType` (string, nullable) - Gasoline, Diesel, Electric, Hybrid
- `Transmission` (string, nullable) - Manual, Automatic, CVT
- `Seats` (int, nullable) - Number of seats

All fields nullable for backward compatibility with existing vehicles.

### New Enum

Create [RegistrationStatus.cs](../../backend/Models/RegistrationStatus.cs):
```csharp
public enum RegistrationStatus
{
    Unknown,
    Active,
    Expired,
    Suspended,
    Revoked
}
```

### Migration

Create migration: `AddRegistrationFieldsToVehicle`

## New Files to Create

### Models
- `backend/Models/RegistrationStatus.cs` - Enum for registration status ✅

### DTOs
- `backend/DTOs/RegistrationExtractResponse.cs` - OCR extraction results with confidence scores ✅
- `backend/DTOs/ExtractedFieldDto.cs` - Individual field (value + confidence level) ✅
- Update `backend/DTOs/CreateVehicleRequest.cs` - Add new registration fields ✅
- Update `backend/DTOs/UpdateVehicleRequest.cs` - Add new registration fields ✅
- Update `backend/DTOs/VehicleDto.cs` - Add new registration fields ✅

### Services
- `backend/Services/IFileStorageService.cs` - Interface for file operations ✅
- `backend/Services/LocalFileStorageService.cs` - Local file system implementation ✅
- `backend/Services/IOcrService.cs` - Interface for OCR operations ✅
- `backend/Services/TesseractOcrService.cs` - Tesseract OCR implementation ✅
- `backend/Services/RegistrationParserService.cs` - Parse OCR text into structured data ✅

### Controllers
- `backend/Controllers/VehicleRegistrationController.cs` - New controller for OCR endpoint ✅

### Configuration
- Update `backend/appsettings.json` - Add FileStorage and Ocr configuration sections ✅

### Test Data
- `backend/TestData/SampleRegistrations/` - Sample registration documents for testing

### Frontend (Not Yet Implemented)
- `web-frontend/src/pages/vehicles/RegistrationUpload.jsx` - Upload page
- `web-frontend/src/components/vehicles/RegistrationUploadForm.jsx` - Upload form component
- `web-frontend/src/components/vehicles/ExtractedDataReview.jsx` - Review extracted data
- `web-frontend/src/services/registrationService.js` - API service methods

## API Endpoint Design

### Extract Registration Data
```http
POST /api/vehicle-registration/extract
Authorization: Bearer {jwt_token}
Content-Type: multipart/form-data

file: [binary image/PDF]
```

**Response:**
```json
{
  "success": true,
  "extractedData": {
    "make": { "value": "Toyota", "confidence": "High" },
    "model": { "value": "Camry", "confidence": "Medium" },
    "year": { "value": 2020, "confidence": "High" },
    "vin": { "value": "1HGBH41JXMN109186", "confidence": "High" },
    "licensePlate": { "value": "ABC1234", "confidence": "High" },
    "ownerName": { "value": "John Doe", "confidence": "Medium" },
    "registrationNumber": { "value": "REG123456", "confidence": "High" },
    "registrationExpiryDate": { "value": "2026-12-31", "confidence": "Low" }
  },
  "rawText": "Full OCR text for debugging..."
}
```

### Upload Registration Document
```http
POST /api/vehicle-registration/upload/{vehicleId}
Authorization: Bearer {jwt_token}
Content-Type: multipart/form-data

file: [binary image/PDF]
```

**Response:**
```json
{
  "success": true,
  "message": "Registration document uploaded successfully",
  "documentUrl": "uploads/user-id/registrations/filename.jpg",
  "publicUrl": "/uploads/user-id/registrations/filename.jpg"
}
```

## Service Layer Design

### IFileStorageService
```csharp
Task<string> SaveFileAsync(Stream fileStream, string fileName, Guid userId, string category);
Task<Stream> GetFileAsync(string filePath);
Task<bool> DeleteFileAsync(string filePath);
string GetFileUrl(string filePath);
```

### IOcrService
```csharp
Task<string> ExtractTextAsync(Stream imageStream, string language = "eng");
Task<string> ExtractTextFromPdfAsync(Stream pdfStream);
```

### RegistrationParserService
```csharp
RegistrationExtractResponse ParseRegistrationText(string ocrText);
```

Uses regex patterns to extract:
- **VIN:** 17-character alphanumeric pattern
- **Dates:** MM/DD/YYYY or DD-MM-YYYY formats
- **License Plates:** 2-8 character alphanumeric
- **Keywords:** Proximity matching for "VIN:", "License Plate:", "Expiration:", etc.

Returns confidence scores based on pattern match quality.

## Configuration Updates

### appsettings.json
```json
{
  "FileStorage": {
    "BasePath": "uploads",
    "MaxFileSizeMB": 10,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".pdf"]
  },
  "Ocr": {
    "TesseractDataPath": "tessdata",
    "Language": "eng"
  }
}
```

### Program.cs Service Registration
```csharp
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IOcrService, TesseractOcrService>();
builder.Services.AddScoped<RegistrationParserService>();

// Configure file upload limits
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});

// Add static file serving for uploads (with auth)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/files"
});
```

## NuGet Packages to Install

```bash
cd backend
dotnet add package Tesseract --version 5.2.0
dotnet add package Magick.NET-Q16-AnyCPU --version 13.6.0  # Image preprocessing
dotnet add package Docnet.Core --version 2.6.0  # PDF rendering
```

### Tesseract Trained Data
- Download `eng.traineddata` from https://github.com/tesseract-ocr/tessdata
- Place in `backend/tessdata/eng.traineddata`

## Implementation Phases

### Phase 1: File Storage Infrastructure ✅ COMPLETED
1. Install NuGet packages (Tesseract, Magick.NET, Docnet.Core)
2. Create IFileStorageService interface + LocalFileStorageService implementation
3. Update appsettings.json with FileStorage configuration
4. Create uploads folder structure
5. Add static file serving middleware in Program.cs
6. Unit tests for file storage

**Verification:** Can upload and retrieve files via service

### Phase 2: OCR Service ✅ COMPLETED
1. Download Tesseract trained data files
2. Create IOcrService interface + TesseractOcrService implementation
3. Add image preprocessing (grayscale, contrast adjustment)
4. Add PDF to image conversion support
5. Unit tests with sample images

**Verification:** Can extract text from sample registration images

### Phase 3: Data Parsing ✅ COMPLETED
1. Create RegistrationParserService
2. Implement regex patterns (VIN, dates, license plates)
3. Implement keyword proximity matching
4. Add confidence scoring logic
5. Unit tests with various OCR text formats

**Verification:** Can parse structured data from raw OCR text

### Phase 4: Database Schema ✅ COMPLETED
1. Add RegistrationStatus enum
2. Extend Vehicle model with 12+ new registration fields
3. Create EF Core migration `AddRegistrationFieldsToVehicle`
4. Update all Vehicle DTOs (CreateVehicleRequest, UpdateVehicleRequest, VehicleDto)
5. Create RegistrationExtractResponse and ExtractedFieldDto
6. Apply migration to database
7. Update VehicleService to handle new fields

**Verification:** Migration applied, new fields visible in database

### Phase 5: API Endpoint ✅ COMPLETED
1. Create VehicleRegistrationController
2. Implement POST /api/vehicle-registration/extract endpoint
   - Accept multipart/form-data with file
   - Validate file type and size
   - Call OCR service
   - Call parser service
   - Return extracted data with confidence scores
3. Add proper error handling and validation
4. Integration tests for endpoint

**Verification:** Can upload registration document and get JSON response with extracted data

### Phase 6: Frontend Implementation ✅ COMPLETED

**Frontend Tasks:**
1. ✅ Create `RegistrationUploadDialog.jsx` modal component
2. ✅ Build file upload component with:
   - Drag-and-drop support
   - File type validation (JPG, PNG, PDF)
   - Progress indicator during OCR processing
   - Preview of uploaded image
3. ✅ Create `ExtractedDataReview.jsx` component
   - Display extracted fields with confidence indicators (High: Green, Medium: Yellow, Low: Red)
   - Allow editing of extracted values
   - Highlight low-confidence fields for review
   - Show raw OCR text for reference
4. ✅ Add service method in `registrationService.js`
   - `extractData()` - Upload and extract data from registration
   - `uploadDocument()` - Upload registration document to vehicle
5. ✅ Integrate with vehicle creation flow:
   - Added "Upload Registration" button to vehicle form (create mode only)
   - Pre-fill form fields from extracted data (with smart make/model matching)
   - Added new registration fields to form (registrationNumber, dates, ownerName)
   - Added vehicle specification fields (bodyType, fuelType, transmission, engineInfo)
   - Allow manual override of all fields
6. ⚠️ No new routes needed - integrated into existing VehicleForm
7. ⚠️ No navigation link needed - button on vehicle form
8. ✅ Material-UI styling consistent with existing pages

**Frontend UX Flow:**
1. User clicks "Upload Registration Document" on vehicle creation page
2. File picker/drag-drop modal opens
3. User selects/drops registration image
4. Loading spinner while OCR processes
5. Extracted data shown in review modal with confidence colors:
   - Green: High confidence
   - Yellow: Medium confidence
   - Red: Low confidence (requires review)
6. User reviews and edits extracted values
7. User clicks "Use This Data" to pre-fill vehicle form
8. User can still edit any field before saving vehicle

### Phase 7: Testing & Documentation ❌ NOT STARTED
1. Find/create sample US vehicle registration documents
2. Test with real registration images from multiple states
3. Document OCR accuracy and limitations
4. Update API.md with new endpoint documentation ✅
5. Create testing guide with sample curl commands
6. Security review (file validation, authorization)
7. Backend integration tests
8. Frontend E2E tests

**Verification:** End-to-end workflow tested with real documents

## Critical Files to Modify

Based on existing patterns, these files are critical:

1. **[backend/Models/Vehicle.cs](../../backend/Models/Vehicle.cs)** - Add 12+ new fields ✅
2. **[backend/Services/VehicleService.cs](../../backend/Services/VehicleService.cs)** - Handle new fields in CRUD operations ✅
3. **[backend/Controllers/VehiclesController.cs](../../backend/Controllers/VehiclesController.cs)** - Reference for auth/DTO patterns
4. **[backend/Data/ApplicationDbContext.cs](../../backend/Data/ApplicationDbContext.cs)** - Configure new fields, create migration ✅
5. **[backend/Program.cs](../../backend/Program.cs)** - Register 3 new services, add file upload middleware ✅

## Error Handling

### File Upload Validation
- File size > 10MB → 413 Payload Too Large
- Invalid file type → 400 Bad Request with message "Unsupported file format"
- No file provided → 400 Bad Request
- Corrupted file → 400 Bad Request

### OCR Processing
- Tesseract initialization failure → 500 Internal Server Error (log details)
- No text extracted → 200 OK with empty results and message
- Partial extraction → 200 OK with extracted fields only

### Authorization
- Always verify userId from JWT token
- Store files in user-specific folders: `/uploads/{userId}/`
- Check vehicle ownership before accepting registration uploads

## Sample Testing Document

Find sample US vehicle registration documents from:
1. **Public DMV websites** - Many states provide example forms
2. **Mock documents** - Create test registrations with known data for unit tests
3. **State variations** - Test CA, TX, FL, NY formats (different layouts)

Store in: `backend/TestData/SampleRegistrations/`

## Verification Steps

After implementation:

1. **Upload Test Document**
   ```bash
   curl -X POST http://localhost:5239/api/vehicle-registration/extract \
     -H "Authorization: Bearer {token}" \
     -F "file=@sample_registration.jpg"
   ```

2. **Verify Extraction Response**
   - Check JSON structure matches RegistrationExtractResponse
   - Verify confidence scores are between 0-1
   - Verify extracted VIN matches document (visual inspection)

3. **Create Vehicle with Extracted Data**
   ```bash
   curl -X POST http://localhost:5239/api/vehicles \
     -H "Authorization: Bearer {token}" \
     -H "Content-Type: application/json" \
     -d '{ "make": "Toyota", "vin": "...", "registrationNumber": "..." }'
   ```

4. **Verify File Storage**
   - Check file exists at `/uploads/{userId}/registrations/`
   - Verify file accessible via static file middleware
   - Verify unauthorized user cannot access file

5. **Database Verification**
   - Query vehicle record, verify new registration fields populated
   - Verify RegistrationDocumentUrl contains correct path

6. **Error Scenarios**
   - Upload invalid file type (should fail gracefully)
   - Upload oversized file (should return 413)
   - Extract with corrupted image (should handle gracefully)

## Future Enhancements

- Cloud OCR fallback (Azure Computer Vision) for low-confidence extractions
- Multi-page PDF support
- State-specific parsing templates
- Auto-alerts for expiring registrations
- Mobile camera optimization (perspective correction)

---

**Estimated Total Implementation Time:**
- Backend: 6-7 days ✅ COMPLETED
- Frontend: 3-4 days ❌ NOT STARTED
- Testing & Documentation: 2 days ❌ NOT STARTED
