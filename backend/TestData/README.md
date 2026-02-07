# Test Data for Vehicle Registration OCR

This directory contains sample vehicle registration documents for testing the OCR extraction feature.

## Sample Documents

### California DMV Form
- **File:** `SampleRegistrations/ca_dmv_reg343.pdf`
- **Description:** Official California DMV registration form (REG 343)
- **Source:** [California DMV](https://www.dmv.ca.gov/portal/uploads/2020/06/reg343.pdf)

## How to Obtain More Sample Documents

### Official DMV Sources
1. **California DMV**
   - Registration Form: https://www.dmv.ca.gov/portal/vehicle-registration/
   - Sample forms: https://www.dmv.ca.gov/portal/forms/

2. **Texas DMV**
   - Uses windshield stickers instead of traditional registration cards
   - Forms: https://www.txdmv.gov/motorists/register-your-vehicle

3. **Florida DMV (FLHSMV)**
   - Registration info: https://www.flhsmv.gov/motor-vehicles-tags-titles/

4. **New York DMV**
   - Registration: https://dmv.ny.gov/registration/

### Alternative Sources for Sample Images

1. **PDFfiller** - Sample registration templates:
   - https://what-does-vehicle-registration-look-like.pdffiller.com/
   - https://dmv-form-reg-343.pdffiller.com/

2. **DocHub** - Fillable registration templates:
   - https://www.dochub.com/fillable-form/4354-what-does-vehicle-registration-look-like
   - https://www.dochub.com/fillable-form/39755-california-vehicle-registration-template

3. **Stock Images**
   - Shutterstock: https://www.shutterstock.com/search/dmv-registration
   - Getty Images: Search for "vehicle registration card"

## Creating Test Documents

For development/testing, you can:

1. **Fill out the blank forms** with test data:
   - VIN: `1HGBH41JXMN109186` (sample format)
   - License Plate: `ABC1234`
   - Make/Model: `Toyota Camry`
   - Year: `2020`
   - Owner: `John Doe`
   - Address: `123 Main Street, Los Angeles, CA 90001`

2. **Take photos of the filled forms** with your phone camera
   - Test different angles and lighting conditions
   - Test blurry images to check OCR robustness

3. **Scan the forms** at different resolutions
   - 150 DPI (low quality)
   - 300 DPI (optimal for OCR)
   - 600 DPI (high quality)

## Testing the OCR Endpoint

Use cURL to test the extraction:

```bash
# First, register a user and get a JWT token
TOKEN=$(curl -X POST http://localhost:5239/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"password123"}' | jq -r '.token')

# Extract data from registration document
curl -X POST http://localhost:5239/api/vehicle-registration/extract \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@backend/TestData/SampleRegistrations/your_sample.jpg"
```

## Expected OCR Fields

The OCR system attempts to extract:

### Basic Vehicle Info
- Make (e.g., Toyota, Honda, Ford)
- Model (e.g., Camry, Civic, F-150)
- Year (e.g., 2020)
- VIN (17 characters)
- License Plate
- Color

### Registration Info
- Registration Number
- Issue Date
- Expiration Date

### Owner Info
- Owner Name
- Owner Address

### Vehicle Specifications
- Body Type (Sedan, SUV, Truck, etc.)
- Fuel Type (Gasoline, Diesel, Electric, Hybrid)
- Transmission (if available)
- Engine Info (if available)
- Number of Seats (if available)

## Troubleshooting

### Low OCR Accuracy?
1. Ensure image quality is good (clear, well-lit, in focus)
2. Check that text is horizontal (not rotated)
3. Try increasing image resolution
4. Verify Tesseract trained data is installed correctly

### No Text Extracted?
1. Check that the file is a valid image format (JPG, PNG, PDF)
2. Verify the image contains readable text
3. Check application logs for OCR errors
4. Ensure tessdata/eng.traineddata exists

## Sources

- [What Does Car Registration Look Like - PDFfiller](https://what-does-vehicle-registration-look-like.pdffiller.com/)
- [What Does A Car Registration Look Like? - Wheels For Wishes](https://www.wheelsforwishes.org/news/what-does-a-car-registration-look-like/)
- [California DMV Registration Forms](https://www.dmv.ca.gov/portal/forms/)
- [What Does a Car Registration Look Like? - CarParts](https://www.carparts.com/blog/what-does-a-car-registration-look-like-and-other-faq/)
