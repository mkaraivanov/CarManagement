using Backend.Models;
using Backend.Services;
using Xunit;

namespace Backend.Tests;

public class MaintenanceCalculationServiceTests
{
    private readonly MaintenanceCalculationService _calculationService;

    public MaintenanceCalculationServiceTests()
    {
        _calculationService = new MaintenanceCalculationService();
    }

    [Fact]
    public void CalculateNextDue_WithTimeInterval_CalculatesCorrectDate()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            IntervalMonths = 6,
            LastCompletedDate = new DateTime(2026, 1, 15),
            UseCompoundRule = false
        };

        // Act
        var result = _calculationService.CalculateNextDue(schedule, 50000, null);

        // Assert
        Assert.NotNull(result.NextDueDate);
        Assert.Equal(new DateTime(2026, 7, 15), result.NextDueDate);
    }

    [Fact]
    public void CalculateNextDue_WithMileageInterval_CalculatesCorrectMileage()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            IntervalKilometers = 10000,
            LastCompletedMileage = 50000,
            UseCompoundRule = false
        };

        // Act
        var result = _calculationService.CalculateNextDue(schedule, 55000, null);

        // Assert
        Assert.NotNull(result.NextDueMileage);
        Assert.Equal(60000, result.NextDueMileage);
    }

    [Fact]
    public void CalculateNextDue_WithHoursInterval_CalculatesCorrectHours()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            IntervalHours = 250,
            LastCompletedHours = 1000,
            UseCompoundRule = false
        };

        // Act
        var result = _calculationService.CalculateNextDue(schedule, 50000, 1100);

        // Assert
        Assert.NotNull(result.NextDueHours);
        Assert.Equal(1250, result.NextDueHours);
    }

    [Fact]
    public void CalculateNextDue_WithCompoundRule_CalculatesAllIntervals()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            IntervalMonths = 6,
            IntervalKilometers = 10000,
            LastCompletedDate = new DateTime(2026, 1, 15),
            LastCompletedMileage = 50000,
            UseCompoundRule = true
        };

        // Act
        var result = _calculationService.CalculateNextDue(schedule, 55000, null);

        // Assert
        Assert.NotNull(result.NextDueDate);
        Assert.NotNull(result.NextDueMileage);
        Assert.Equal(new DateTime(2026, 7, 15), result.NextDueDate);
        Assert.Equal(60000, result.NextDueMileage);
    }

    [Fact]
    public void IsOverdue_WithCompoundRuleAndTimeOverdue_ReturnsTrue()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(-10), // 10 days overdue
            NextDueMileage = 60000,
            UseCompoundRule = true // OR logic
        };

        // Act
        var result = _calculationService.IsOverdue(schedule, 55000, null);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsOverdue_WithCompoundRuleAndMileageOverdue_ReturnsTrue()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(10), // Not due yet by time
            NextDueMileage = 60000,
            UseCompoundRule = true // OR logic
        };

        // Act
        var result = _calculationService.IsOverdue(schedule, 61000, null); // 1000 km over

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsOverdue_WithCompoundRuleAndNothingOverdue_ReturnsFalse()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(10),
            NextDueMileage = 60000,
            UseCompoundRule = true
        };

        // Act
        var result = _calculationService.IsOverdue(schedule, 55000, null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsOverdue_WithANDLogicAndOnlyTimeOverdue_ReturnsFalse()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(-10), // Time overdue
            NextDueMileage = 60000,
            UseCompoundRule = false // AND logic - all conditions must be met
        };

        // Act
        var result = _calculationService.IsOverdue(schedule, 55000, null); // Mileage NOT overdue

        // Assert
        Assert.False(result); // Not overdue because mileage condition not met
    }

    [Fact]
    public void IsOverdue_WithANDLogicAndAllConditionsOverdue_ReturnsTrue()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(-10), // Time overdue
            NextDueMileage = 60000,
            UseCompoundRule = false // AND logic
        };

        // Act
        var result = _calculationService.IsOverdue(schedule, 61000, null); // Mileage also overdue

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsUpcoming_WithinReminderThreshold_ReturnsTrue()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(20), // 20 days from now
            ReminderDaysBefore = 30,
            UseCompoundRule = false
        };

        // Act
        var result = _calculationService.IsUpcoming(schedule, 50000, null);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsUpcoming_OutsideReminderThreshold_ReturnsFalse()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(40), // 40 days from now
            ReminderDaysBefore = 30,
            UseCompoundRule = false
        };

        // Act
        var result = _calculationService.IsUpcoming(schedule, 50000, null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsUpcoming_WithMileageWithinThreshold_ReturnsTrue()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueMileage = 60000,
            ReminderKilometersBefore = 1000,
            UseCompoundRule = false
        };

        // Act
        var result = _calculationService.IsUpcoming(schedule, 59500, null); // 500 km away

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GetMaintenanceStatus_WhenOverdue_ReturnsOverdue()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(-10),
            UseCompoundRule = true
        };

        // Act
        var result = _calculationService.GetMaintenanceStatus(schedule, 50000, null);

        // Assert
        Assert.Equal("Overdue", result);
    }

    [Fact]
    public void GetMaintenanceStatus_WhenUpcoming_ReturnsDueSoon()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(20),
            ReminderDaysBefore = 30,
            UseCompoundRule = false
        };

        // Act
        var result = _calculationService.GetMaintenanceStatus(schedule, 50000, null);

        // Assert
        Assert.Equal("Due Soon", result);
    }

    [Fact]
    public void GetMaintenanceStatus_WhenNotDueYet_ReturnsOK()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(100),
            NextDueMileage = 70000,
            ReminderDaysBefore = 30,
            ReminderKilometersBefore = 1000,
            UseCompoundRule = false
        };

        // Act
        var result = _calculationService.GetMaintenanceStatus(schedule, 50000, null);

        // Assert
        Assert.Equal("OK", result);
    }

    [Fact]
    public void CalculateRemaining_WithFutureDate_ReturnsPositiveDays()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(45),
            UseCompoundRule = false
        };

        // Act
        var result = _calculationService.CalculateRemaining(schedule, 50000, null);

        // Assert
        Assert.NotNull(result.DaysRemaining);
        Assert.True(result.DaysRemaining > 44 && result.DaysRemaining <= 46);
        Assert.False(result.IsOverdue);
    }

    [Fact]
    public void CalculateRemaining_WithOverdueMileage_SetsOverdueFlag()
    {
        // Arrange
        var schedule = new MaintenanceSchedule
        {
            NextDueMileage = 60000,
            UseCompoundRule = true
        };

        // Act
        var result = _calculationService.CalculateRemaining(schedule, 61000, null);

        // Assert
        Assert.True(result.IsOverdue);
        Assert.Equal(-1000, result.KilometersRemaining);
    }
}
