# Incomplete Features

This document tracks features that have partial implementation - typically backend APIs without corresponding frontend UI. Features should be removed from this document only when they are fully implemented end-to-end (backend + frontend + testing).

## Definition

**A feature is considered INCOMPLETE if:**
- Backend API exists but no frontend UI to use it
- Frontend UI exists but no backend API to support it
- Feature is untested or missing critical functionality
- Feature is not usable by end users

**A feature is considered COMPLETE when:**
- Backend API implemented and documented in `backend/API.md`
- Frontend UI implemented and accessible to users
- End-to-end integration tested and working
- Feature documented in appropriate design doc

## Current Incomplete Features

**Note:** Currently no incomplete features. All implemented backend features now have corresponding frontend UI.

The Vehicle Registration Document Upload feature was recently completed with both backend and frontend implementation. It is currently in testing phase.

---

## Recently Completed (In Testing)

### Vehicle Registration Document Upload

**Status:** ✅ Backend + Frontend Complete | ⚠️ Testing Pending

**Design Doc:** [`docs/features/vehicle-registration-ocr.md`](docs/features/vehicle-registration-ocr.md)

**Priority:** Low (nice-to-have feature)

**What's implemented:**

**Backend:**
- ✅ Backend API endpoints in `VehicleRegistrationController.cs`
- ✅ OCR service using Tesseract (`TesseractOcrService.cs`)
- ✅ Registration parser service (`RegistrationParserService.cs`)
- ✅ File storage service (`LocalFileStorageService.cs`)
- ✅ DTOs and database migration
- ✅ Documented in `backend/API.md`

**Frontend:**
- ✅ `RegistrationUploadDialog.jsx` - Modal dialog with drag-and-drop
- ✅ `ExtractedDataReview.jsx` - Review extracted data with confidence indicators
- ✅ `registrationService.js` - API service for OCR operations
- ✅ Integration with `VehicleForm.jsx`
- ✅ Pre-fill form fields from extracted OCR data

**What's pending:**
- ❌ Integration tests for registration endpoints
- ❌ End-to-end testing with real registration documents
- ❌ User acceptance testing

**How to use:**
1. Navigate to "Add New Vehicle" page
2. Click "Upload Registration" button
3. Drag-and-drop or select vehicle registration document
4. Review extracted data (color-coded by confidence)
5. Edit any incorrect fields
6. Click "Use This Data" to pre-fill the form

---

## Future Enhancements (Not Yet Implemented)

These features are planned but have not been started. They will move to "Incomplete Features" above once backend work begins.

### Service Records UI

**Status:** 🔵 Planned

**What exists:**
- Backend API in `ServiceRecordsController.cs`
- Documented in `backend/API.md`

**Missing:**
- No frontend UI for managing service records
- No visual timeline or history view

**Estimated Effort:** 12-16 hours

### Fuel Records UI

**Status:** 🔵 Planned

**What exists:**
- Backend API in `FuelRecordsController.cs`
- Auto-calculation of fuel efficiency
- Documented in `backend/API.md`

**Missing:**
- No frontend UI for adding/viewing fuel records
- No charts/graphs for fuel efficiency over time

**Estimated Effort:** 12-16 hours

### Insurance Management

**Status:** 🔵 Planned

**Missing:**
- Backend API
- Frontend UI
- Database schema

**Estimated Effort:** 20-24 hours (full feature implementation)

### Expense Analytics Dashboard

**Status:** 🔵 Planned

**Missing:**
- Backend aggregation/analytics API
- Frontend dashboard with charts
- Date range filtering

**Estimated Effort:** 16-20 hours

### Reminders and Notifications

**Status:** 🔵 Planned

**Missing:**
- Backend notification system
- Frontend notifications display
- Email/push notification integration

**Estimated Effort:** 24-32 hours (complex feature)

---

## Process for Completing Features

When working on completing an incomplete feature:

1. **Update Design Doc Status**
   - Change phase status from 🟡 In Progress to ⚠️ Backend Only (if backend is done)
   - Or from ⚠️ Backend Only to ✅ Complete (when fully done)

2. **Implement Missing Components**
   - Follow implementation plan in feature design doc
   - Create frontend UI with corresponding service layer
   - Write integration tests
   - Test end-to-end flow

3. **Update Documentation**
   - Remove "⚠️ No Frontend Yet" marker from `backend/API.md`
   - Add frontend usage examples to design doc
   - Update this file to remove the feature from "Incomplete" section

4. **Final Verification**
   - [ ] Backend API working and tested
   - [ ] Frontend UI accessible and functional
   - [ ] End-to-end flow tested manually
   - [ ] Integration tests passing
   - [ ] Documentation updated
   - [ ] No console errors or warnings

5. **Commit and Close**
   ```bash
   git add .
   git commit -m "Complete [feature name] - add frontend UI and tests
   
   - Implement frontend UI components
   - Add service layer integration
   - Update documentation
   - All tests passing
   
   Closes: [feature design doc reference]"
   ```

---

## Adding New Incomplete Features

When you create a backend API without immediate frontend implementation:

1. **Add entry to this document** under "Current Incomplete Features"
2. **Include all sections:**
   - Status (⚠️ Backend Only)
   - Design Doc link
   - Priority (High/Medium/Low)
   - What exists (✅)
   - What's missing (❌)
   - Next steps
   - Estimated effort

3. **Mark in API.md** with "⚠️ No Frontend Yet"

4. **Update design doc** status to ⚠️ Backend Only

5. **Create GitHub issue** (if using issues) to track the work

---

## Notes

- This file should be checked during sprint planning to prioritize completing features
- Features should not stay "Backend Only" for extended periods (>2 weeks) unless explicitly deprioritized
- Consider whether backend-only APIs are really needed, or if they should wait until frontend is ready
- Keep this file up to date - it's a critical project health indicator
