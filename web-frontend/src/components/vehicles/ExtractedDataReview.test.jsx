import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import ExtractedDataReview from './ExtractedDataReview';

describe('ExtractedDataReview', () => {
  const mockExtractedData = {
    success: true,
    message: 'Data extracted successfully',
    extractedData: {
      registrationNumber: {
        value: 'REG 4008',
        confidence: 0.85, // Numeric confidence
      },
      licensePlate: {
        value: 'ABC123',
        confidence: 0.9,
      },
      color: {
        value: 'Red',
        confidence: 0.7,
      },
      make: {
        value: 'Toyota',
        confidence: 0.6,
      },
      vin: {
        value: '1HGBH41JXMN109186',
        confidence: 0.4, // Low confidence
      },
    },
    rawText: 'Sample OCR text...',
  };

  const mockOnUseData = vi.fn();
  const mockOnCancel = vi.fn();

  it('renders without crashing', () => {
    render(
      <ExtractedDataReview
        extractedData={mockExtractedData}
        onUseData={mockOnUseData}
        onCancel={mockOnCancel}
      />
    );

    expect(screen.getByText(/Review the extracted information/i)).toBeInTheDocument();
  });

  it('handles numeric confidence values correctly', () => {
    render(
      <ExtractedDataReview
        extractedData={mockExtractedData}
        onUseData={mockOnUseData}
        onCancel={mockOnCancel}
      />
    );

    // Check that HIGH confidence (0.85, 0.9) is displayed
    const highChips = screen.getAllByText('HIGH');
    expect(highChips.length).toBeGreaterThanOrEqual(1);

    // Check that MEDIUM confidence (0.7, 0.6) is displayed
    const mediumChips = screen.getAllByText('MEDIUM');
    expect(mediumChips.length).toBeGreaterThan(0);

    // Check that LOW confidence (0.4) is displayed
    expect(screen.getByText('LOW')).toBeInTheDocument();
  });

  it('shows warning for low confidence fields', () => {
    render(
      <ExtractedDataReview
        extractedData={mockExtractedData}
        onUseData={mockOnUseData}
        onCancel={mockOnCancel}
      />
    );

    expect(
      screen.getByText(/Some fields have low confidence/i)
    ).toBeInTheDocument();
  });

  it('displays all extracted field values', () => {
    render(
      <ExtractedDataReview
        extractedData={mockExtractedData}
        onUseData={mockOnUseData}
        onCancel={mockOnCancel}
      />
    );

    expect(screen.getByDisplayValue('REG 4008')).toBeInTheDocument();
    expect(screen.getByDisplayValue('ABC123')).toBeInTheDocument();
    expect(screen.getByDisplayValue('Red')).toBeInTheDocument();
    expect(screen.getByDisplayValue('Toyota')).toBeInTheDocument();
    expect(screen.getByDisplayValue('1HGBH41JXMN109186')).toBeInTheDocument();
  });

  it('allows editing field values', () => {
    render(
      <ExtractedDataReview
        extractedData={mockExtractedData}
        onUseData={mockOnUseData}
        onCancel={mockOnCancel}
      />
    );

    const licensePlateInput = screen.getByDisplayValue('ABC123');
    fireEvent.change(licensePlateInput, { target: { value: 'XYZ789' } });

    expect(screen.getByDisplayValue('XYZ789')).toBeInTheDocument();
  });

  it('calls onUseData with edited values when Use This Data is clicked', () => {
    render(
      <ExtractedDataReview
        extractedData={mockExtractedData}
        onUseData={mockOnUseData}
        onCancel={mockOnCancel}
      />
    );

    // Edit a field
    const colorInput = screen.getByDisplayValue('Red');
    fireEvent.change(colorInput, { target: { value: 'Blue' } });

    // Click Use This Data button
    const useDataButton = screen.getByRole('button', { name: /Use This Data/i });
    fireEvent.click(useDataButton);

    expect(mockOnUseData).toHaveBeenCalledWith(
      expect.objectContaining({
        color: 'Blue',
        licensePlate: 'ABC123',
        registrationNumber: 'REG 4008',
      })
    );
  });

  it('calls onCancel when Try Another Document is clicked', () => {
    render(
      <ExtractedDataReview
        extractedData={mockExtractedData}
        onUseData={mockOnUseData}
        onCancel={mockOnCancel}
      />
    );

    const cancelButton = screen.getByRole('button', { name: /Try Another Document/i });
    fireEvent.click(cancelButton);

    expect(mockOnCancel).toHaveBeenCalledTimes(1);
  });

  it('shows message when no data is extracted', () => {
    const emptyData = {
      success: false,
      extractedData: {},
      rawText: '',
    };

    render(
      <ExtractedDataReview
        extractedData={emptyData}
        onUseData={mockOnUseData}
        onCancel={mockOnCancel}
      />
    );

    expect(
      screen.getByText(/No data could be extracted from the document/i)
    ).toBeInTheDocument();
  });

  it('disables Use This Data button when no data is extracted', () => {
    const emptyData = {
      success: false,
      extractedData: {},
      rawText: '',
    };

    render(
      <ExtractedDataReview
        extractedData={emptyData}
        onUseData={mockOnUseData}
        onCancel={mockOnCancel}
      />
    );

    const useDataButton = screen.getByRole('button', { name: /Use This Data/i });
    expect(useDataButton).toBeDisabled();
  });

  it('handles string confidence values (legacy support)', () => {
    const dataWithStringConfidence = {
      ...mockExtractedData,
      extractedData: {
        make: {
          value: 'Honda',
          confidence: 'high', // String confidence
        },
        model: {
          value: 'Civic',
          confidence: 'medium',
        },
      },
    };

    render(
      <ExtractedDataReview
        extractedData={dataWithStringConfidence}
        onUseData={mockOnUseData}
        onCancel={mockOnCancel}
      />
    );

    // Should render without errors
    expect(screen.getByDisplayValue('Honda')).toBeInTheDocument();
    expect(screen.getByText('HIGH')).toBeInTheDocument();
    expect(screen.getByText('MEDIUM')).toBeInTheDocument();
  });

  it('displays raw OCR text when available', () => {
    render(
      <ExtractedDataReview
        extractedData={mockExtractedData}
        onUseData={mockOnUseData}
        onCancel={mockOnCancel}
      />
    );

    expect(screen.getByText('Sample OCR text...')).toBeInTheDocument();
  });
});
