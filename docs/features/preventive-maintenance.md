# Preventive Maintenance (PM) Schedules & Smart Reminders - Feature Design

## Context

**Problem Statement:**
Vehicle owners need to track and receive timely reminders for preventive maintenance tasks (oil changes, tire rotations, inspections, etc.) to ensure vehicle reliability and longevity. Currently, the CarManagement app tracks completed service records but has no proactive system to remind users when maintenance is due.

**User Needs:**
- Track recurring maintenance schedules (e.g., "Oil change every 6 months OR 10,000 km, whichever comes first")
- Receive timely reminders before maintenance is due
- Support multiple interval types: time-based, mileage-based, engine hours-based
- Smart compound rules: "12 months OR 15,000 km, whichever comes first"
- Connection between PM schedules and completed service records
- Both pre-defined templates (common maintenance tasks) and custom schedules

**Success Criteria:**
- Users never miss critical maintenance deadlines
- Accurate calculation of "next due" dates considering compound rules
- Seamless integration with existing ServiceRecord system
- Multi-channel notifications (in-app, email, mobile push)

**User Stories:**
- As a vehicle owner, I want to set up recurring maintenance schedules so that I don't forget important service deadlines
- As a user, I want to receive reminders 30 days before maintenance is due so I can plan ahead
- As a user, I want the system to suggest which PM schedule a completed service satisfies so I can easily reset the schedule
- As a fleet manager, I want to track engine hours for commercial vehicles so I can maintain them properly
- As a user, I want pre-defined templates for common maintenance tasks so I don't have to manually configure everything
- As a user, I want to create custom maintenance schedules for unique vehicle needs

## Implementation Status

**Status:** 🔵 Planning Phase

**Completed:**
- [ ] Database schema (MaintenanceSchedule, MaintenanceTemplate, Reminder, Notification entities)
- [ ] PM schedule calculation engine (compound rule logic)
- [ ] Reminder generation service (background job)
- [ ] Notification delivery system (in-app, email, push)
- [ ] Backend API endpoints
- [ ] Frontend UI for managing PM schedules
- [ ] Integration with existing ServiceRecord system
- [ ] Testing and documentation

## Technology Decisions

### Notification System Architecture

**Choice:** Multi-channel notification system with pluggable providers

**Why:**
- Supports all requested channels: in-app, email, mobile push
- Allows future expansion to SMS or other channels
- Decouples notification logic from delivery mechanism

**Components:**
1. **Notification Queue** - Store pending notifications
2. **Notification Providers** - Pluggable interfaces for each channel (IEmailProvider, IPushNotificationProvider, etc.)
3. **Notification Service** - Orchestrates notification delivery
4. **Background Job** - Daily check for due/upcoming maintenance

**Implementation:**
- Use built-in .NET background service (HostedService) for reminder checks
- Store notifications in database for audit trail and in-app display
- Email via SendGrid or SMTP (configurable)
- Push notifications via Firebase Cloud Messaging (FCM) for mobile

**Alternatives Considered:**
- External notification service (Twilio, OneSignal) - More features but adds external dependency and cost
- Immediate calculation on API calls - Would miss background reminders when users aren't active

### Background Job for Reminder Checks

**Choice:** .NET BackgroundService (Hosted Service)

**Why:**
- Native to .NET, no external dependencies
- Runs daily to check which PM schedules are approaching due dates
- Can scale with the application

**Alternatives Considered:**
- Hangfire - More powerful but adds complexity for this simple use case
- Azure Functions/AWS Lambda - Requires cloud infrastructure

### Compound Rule Logic Engine

**Choice:** Custom rule evaluation engine

**Why:**
- Unique requirement: "12 months OR 10,000 km, whichever comes first"
- Need to evaluate multiple conditions and pick the earliest due date/mileage
- Must support: time-based, mileage-based, engine hours-based, and combinations

**Algorithm:**
1. Calculate next due date for time-based rule (if exists)
2. Calculate next due mileage for mileage-based rule (if exists)
3. Calculate next due engine hours for hours-based rule (if exists)
4. For compound rules with OR logic, determine which condition will trigger first
5. Return the earliest trigger point as "Next Due"

**Example:**
```
Schedule: "Oil Change every 6 months OR 5,000 km"
Last Service: 2026-01-01 at 10,000 km
Current Date: 2026-02-07
Current Mileage: 11,500 km

Time-based: Next due = 2026-07-01 (6 months from 2026-01-01)
Mileage-based: Next due = 15,000 km (10,000 + 5,000)

Current status:
- Time remaining: ~145 days
- Mileage remaining: 3,500 km

Result: Whichever comes first will trigger the reminder
```

### Engine Hours Tracking

**Choice:** Add `EngineHours` field to Vehicle model + tracking via manual input

**Why:**
- Critical for commercial vehicles, heavy equipment, boats
- No automatic way to track engine hours (requires OBD-II integration, out of scope for MVP)
- Users can manually update engine hours when they check the vehicle

**Implementation:**
- Add `CurrentEngineHours` (decimal, nullable) to Vehicle model
- Add `EngineHoursAtService` to ServiceRecord model
- Allow users to update engine hours manually via API/UI
- Calculate hours-based maintenance intervals similar to mileage

**Future Enhancement:**
- OBD-II integration for automatic engine hours tracking

## Database Schema Changes

### New Entities

#### MaintenanceTemplate
Pre-defined maintenance task templates (e.g., "Oil Change", "Tire Rotation")

```csharp
public class MaintenanceTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } // "Oil Change", "Tire Rotation", etc.
    public string Description { get; set; }
    public string Category { get; set; } // "Engine", "Tires", "Brakes", "General", "Inspection"
    public bool IsSystemTemplate { get; set; } // true for pre-defined, false for user-created
    public Guid? UserId { get; set; } // null for system templates, userId for custom templates

    // Default intervals (can be overridden per vehicle)
    public int? DefaultIntervalMonths { get; set; }
    public int? DefaultIntervalKilometers { get; set; }
    public decimal? DefaultIntervalHours { get; set; }
    public bool UseCompoundRule { get; set; } // true = OR logic (whichever first), false = AND logic (all must be met)

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public User User { get; set; }
}
```

#### MaintenanceSchedule
Vehicle-specific PM schedules based on templates

```csharp
public class MaintenanceSchedule
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Guid? TemplateId { get; set; } // null if custom schedule not from template

    public string TaskName { get; set; } // "Oil Change", "Replace Air Filter", etc.
    public string Description { get; set; }
    public string Category { get; set; } // "Engine", "Tires", "Brakes", etc.

    // Interval Configuration
    public int? IntervalMonths { get; set; } // e.g., 6 months
    public int? IntervalKilometers { get; set; } // e.g., 10,000 km
    public decimal? IntervalHours { get; set; } // e.g., 100 hours
    public bool UseCompoundRule { get; set; } // true = OR (whichever first), false = AND (all conditions)

    // Last Completion Tracking
    public DateTime? LastCompletedDate { get; set; }
    public int? LastCompletedMileage { get; set; }
    public decimal? LastCompletedHours { get; set; }
    public Guid? LastServiceRecordId { get; set; } // Link to ServiceRecord if completed

    // Next Due Calculation (computed)
    public DateTime? NextDueDate { get; set; }
    public int? NextDueMileage { get; set; }
    public decimal? NextDueHours { get; set; }

    // Reminder Settings
    public int ReminderDaysBefore { get; set; } // Default: 30 days
    public int ReminderKilometersBefore { get; set; } // Default: 1,000 km
    public decimal? ReminderHoursBefore { get; set; } // Default: 10 hours

    public bool IsActive { get; set; } // Allow users to pause schedules
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Vehicle Vehicle { get; set; }
    public MaintenanceTemplate Template { get; set; }
    public ServiceRecord LastServiceRecord { get; set; }
    public ICollection<Reminder> Reminders { get; set; }
}
```

#### Reminder
Tracks reminders sent for PM schedules

```csharp
public class Reminder
{
    public Guid Id { get; set; }
    public Guid MaintenanceScheduleId { get; set; }
    public Guid UserId { get; set; }

    public ReminderStatus Status { get; set; } // Pending, Sent, Dismissed, Completed
    public ReminderType Type { get; set; } // TimeBased, MileageBased, HoursBased

    public DateTime? ScheduledDate { get; set; } // When reminder should be sent
    public DateTime? SentDate { get; set; } // When reminder was actually sent
    public DateTime? DismissedDate { get; set; }

    public string Message { get; set; } // "Oil change due in 5 days or 200 km"

    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public MaintenanceSchedule MaintenanceSchedule { get; set; }
    public User User { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}

public enum ReminderStatus
{
    Pending,
    Sent,
    Dismissed,
    Completed
}

public enum ReminderType
{
    TimeBased,
    MileageBased,
    HoursBased,
    Compound
}
```

#### Notification
Multi-channel notification records

```csharp
public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? ReminderId { get; set; } // null for non-reminder notifications

    public NotificationType Type { get; set; } // MaintenanceDue, MaintenanceOverdue, General
    public NotificationChannel Channel { get; set; } // InApp, Email, Push
    public NotificationStatus Status { get; set; } // Pending, Sent, Failed, Read

    public string Title { get; set; }
    public string Message { get; set; }
    public string ActionUrl { get; set; } // Deep link to relevant page

    public DateTime? ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string ErrorMessage { get; set; } // If failed

    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public User User { get; set; }
    public Reminder Reminder { get; set; }
}

public enum NotificationType
{
    MaintenanceDue,
    MaintenanceOverdue,
    ServiceCompleted,
    General
}

public enum NotificationChannel
{
    InApp,
    Email,
    Push
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
    Read
}
```

### Modified Entities

#### Vehicle Model
Add engine hours tracking

```csharp
public class Vehicle
{
    // ... existing fields ...

    // NEW FIELDS
    public decimal? CurrentEngineHours { get; set; } // For equipment/commercial vehicles
    public DateTime? EngineHoursLastUpdated { get; set; }

    // NEW NAVIGATION PROPERTY
    public ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; }
}
```

#### ServiceRecord Model
Add engine hours tracking

```csharp
public class ServiceRecord
{
    // ... existing fields ...

    // NEW FIELD
    public decimal? EngineHoursAtService { get; set; }
}
```

### Migrations

1. `AddEngineHoursToVehicle` - Add CurrentEngineHours, EngineHoursLastUpdated to Vehicle
2. `AddMaintenanceTemplates` - Create MaintenanceTemplate table with seed data
3. `AddMaintenanceSchedules` - Create MaintenanceSchedule table
4. `AddRemindersAndNotifications` - Create Reminder and Notification tables
5. `AddEngineHoursToServiceRecord` - Add EngineHoursAtService to ServiceRecord

### Seed Data - System Maintenance Templates

Pre-defined templates users can enable:

**Engine Maintenance:**
- Oil Change (6 months OR 10,000 km)
- Oil Filter Replacement (6 months OR 10,000 km)
- Air Filter Replacement (12 months OR 20,000 km)
- Spark Plugs Replacement (24 months OR 50,000 km)
- Timing Belt Replacement (60 months OR 100,000 km)

**Tires & Wheels:**
- Tire Rotation (6 months OR 10,000 km)
- Tire Replacement (48 months OR 60,000 km)
- Wheel Alignment (12 months OR 20,000 km)
- Tire Pressure Check (1 month OR 2,000 km)

**Brakes:**
- Brake Pad Inspection (12 months OR 20,000 km)
- Brake Fluid Replacement (24 months OR 40,000 km)

**Fluids:**
- Coolant Flush (24 months OR 40,000 km)
- Transmission Fluid Change (48 months OR 80,000 km)
- Power Steering Fluid (24 months OR 40,000 km)

**Inspections:**
- Annual Safety Inspection (12 months)
- Emission Test (24 months)
- Battery Check (12 months)

**Equipment/Commercial (Engine Hours):**
- Oil Change - Heavy Equipment (250 hours)
- Hydraulic Fluid Change (500 hours)
- Air Filter - Equipment (100 hours)

## API Design

See full API specification in [backend/API.md](../../backend/API.md) after implementation.

### Key Endpoints

- `GET /api/maintenance-templates` - List system templates
- `GET /api/vehicles/{id}/maintenance-schedules` - List PM schedules for vehicle
- `POST /api/vehicles/{id}/maintenance-schedules` - Create PM schedule
- `PUT /api/maintenance-schedules/{id}` - Update PM schedule
- `POST /api/maintenance-schedules/{id}/complete` - Mark schedule as completed
- `GET /api/service-records/{id}/schedule-suggestions` - Get PM schedule suggestions for service
- `POST /api/maintenance-schedules/{id}/link-service` - Link service record to schedule
- `GET /api/notifications` - Get user's notifications
- `GET /api/notifications/unread-count` - Get unread notification count
- `POST /api/notifications/{id}/read` - Mark notification as read

## Implementation Phases

### Phase 1: Database Schema & Migrations (3-4 days)

Set up database foundation for PM schedules, templates, reminders, and notifications.

### Phase 2: Core Services & Calculation Engine (4-5 days)

Implement business logic for PM schedule management and next due calculations.

### Phase 3: Reminder & Notification System (4-5 days)

Build multi-channel notification system with background job.

### Phase 4: Backend API Endpoints (3-4 days)

Expose PM schedules, templates, reminders, and notifications via REST API.

### Phase 5: Frontend UI (5-6 days)

Build user-facing UI for managing PM schedules and viewing notifications.

### Phase 6: Mobile Push Notifications (Optional, 3-4 days)

Add push notification support for mobile app.

### Phase 7: Testing & Documentation (2-3 days)

Comprehensive testing and documentation updates.

**See full implementation plan in this document for detailed tasks.**

## Files to Create/Modify

### Backend
- 11 new model files (entities + enums)
- 9 new DTO files
- 9 new service files
- 4 new controller files
- Updates to ApplicationDbContext, Program.cs, appsettings.json

### Frontend
- 3 new service files
- 2 new page files
- 8 new component files
- Updates to Dashboard, VehicleDetails, ServiceRecordForm, Navbar, App.jsx

### Tests
- 4 new test files

### Documentation
- Update backend/API.md
- Update CLAUDE.md
- Create user guide

## Dependencies

### NuGet Packages
- SendGrid (or MailKit for SMTP) - Email notifications
- FirebaseAdmin - Push notifications (optional, Phase 6)

### NPM Packages
- date-fns - Date calculations and formatting

### External Services
- SendGrid or SMTP server - Email delivery
- Firebase Cloud Messaging - Push notifications (optional)

## Success Metrics

1. **Adoption Rate:** 70%+ of users create at least one PM schedule within 30 days
2. **Reminder Effectiveness:** 80%+ of reminders lead to completed service within reminder window
3. **Suggestion Accuracy:** 85%+ of service record suggestions match correctly
4. **Performance:** Background job completes within 5 minutes for 10,000+ schedules
5. **User Satisfaction:** Users report reduced missed maintenance deadlines

## Future Enhancements

- OBD-II Integration for automatic mileage/engine hours tracking
- Manufacturer-recommended PM schedules by make/model/year
- Service center integration for booking appointments
- Cost estimates and recurring cost budgeting
- Historical analytics and maintenance completion tracking
- SMS notifications
- Machine learning for interval optimization
- Predictive maintenance based on sensor data

---

**Estimated Total Implementation Time:** 22-27 days (MVP)

**Priority:** High (core feature for vehicle management app)

**Last Updated:** 2026-02-07

**Status:** 🔵 Planning Complete, Awaiting Implementation Approval
