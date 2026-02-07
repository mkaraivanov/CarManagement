import { useState, useMemo } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  IconButton,
  Chip,
  Grid,
  Alert,
} from '@mui/material';
import { ChevronLeft, ChevronRight } from '@mui/icons-material';
import { format, startOfMonth, endOfMonth, eachDayOfInterval, isSameMonth, isSameDay, addMonths, subMonths, parseISO } from 'date-fns';

const MaintenanceCalendar = ({ schedules }) => {
  const [currentMonth, setCurrentMonth] = useState(new Date());

  const monthStart = startOfMonth(currentMonth);
  const monthEnd = endOfMonth(currentMonth);
  const daysInMonth = eachDayOfInterval({ start: monthStart, end: monthEnd });

  const getStatusColor = (status) => {
    switch (status) {
      case 'OK':
        return 'success';
      case 'Due Soon':
        return 'warning';
      case 'Overdue':
        return 'error';
      default:
        return 'default';
    }
  };

  const getSchedulesForDay = (day) => {
    if (!schedules || schedules.length === 0) return [];

    return schedules.filter(schedule => {
      if (!schedule.nextDueDate) return false;

      try {
        const dueDate = parseISO(schedule.nextDueDate);
        return isSameDay(dueDate, day);
      } catch {
        return false;
      }
    });
  };

  const handlePreviousMonth = () => {
    setCurrentMonth(subMonths(currentMonth, 1));
  };

  const handleNextMonth = () => {
    setCurrentMonth(addMonths(currentMonth, 1));
  };

  const handleToday = () => {
    setCurrentMonth(new Date());
  };

  const weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

  // Get the day of week for the first day of the month (0 = Sunday)
  const firstDayOfWeek = monthStart.getDay();

  // Create array of days including empty slots for proper alignment
  const calendarDays = useMemo(() => {
    const days = [];

    // Add empty slots for days before the first day of the month
    for (let i = 0; i < firstDayOfWeek; i++) {
      days.push(null);
    }

    // Add all days in the month
    days.push(...daysInMonth);

    return days;
  }, [daysInMonth, firstDayOfWeek]);

  if (!schedules || schedules.length === 0) {
    return (
      <Alert severity="info">
        No maintenance schedules to display in calendar.
      </Alert>
    );
  }

  return (
    <Box>
      <Card>
        <CardContent>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
            <IconButton onClick={handlePreviousMonth}>
              <ChevronLeft />
            </IconButton>
            <Box sx={{ display: 'flex', gap: 2, alignItems: 'center' }}>
              <Typography variant="h6">
                {format(currentMonth, 'MMMM yyyy')}
              </Typography>
              <Chip
                label="Today"
                size="small"
                onClick={handleToday}
                sx={{ cursor: 'pointer' }}
              />
            </Box>
            <IconButton onClick={handleNextMonth}>
              <ChevronRight />
            </IconButton>
          </Box>

          {/* Week day headers */}
          <Grid container spacing={1} sx={{ mb: 1 }}>
            {weekDays.map(day => (
              <Grid item xs={12 / 7} key={day}>
                <Typography variant="caption" fontWeight="bold" sx={{ display: 'block', textAlign: 'center' }}>
                  {day}
                </Typography>
              </Grid>
            ))}
          </Grid>

          {/* Calendar grid */}
          <Grid container spacing={1}>
            {calendarDays.map((day, index) => {
              const isToday = day && isSameDay(day, new Date());
              const daySchedules = day ? getSchedulesForDay(day) : [];

              return (
                <Grid item xs={12 / 7} key={index}>
                  {day ? (
                    <Box
                      sx={{
                        minHeight: 80,
                        border: 1,
                        borderColor: isToday ? 'primary.main' : 'divider',
                        borderRadius: 1,
                        p: 0.5,
                        bgcolor: isToday ? 'action.selected' : 'background.paper',
                        opacity: isSameMonth(day, currentMonth) ? 1 : 0.4,
                      }}
                    >
                      <Typography
                        variant="caption"
                        sx={{
                          display: 'block',
                          textAlign: 'right',
                          fontWeight: isToday ? 'bold' : 'normal',
                          color: isToday ? 'primary.main' : 'text.primary',
                        }}
                      >
                        {format(day, 'd')}
                      </Typography>
                      <Box sx={{ mt: 0.5 }}>
                        {daySchedules.map(schedule => (
                          <Chip
                            key={schedule.id}
                            label={schedule.taskName}
                            size="small"
                            color={getStatusColor(schedule.status)}
                            sx={{
                              mb: 0.5,
                              fontSize: '0.65rem',
                              height: 'auto',
                              '& .MuiChip-label': {
                                px: 0.5,
                                py: 0.25,
                                whiteSpace: 'normal',
                              },
                            }}
                          />
                        ))}
                      </Box>
                    </Box>
                  ) : (
                    <Box sx={{ minHeight: 80 }} />
                  )}
                </Grid>
              );
            })}
          </Grid>
        </CardContent>
      </Card>

      {/* Legend */}
      <Box sx={{ display: 'flex', gap: 2, mt: 2, justifyContent: 'center' }}>
        <Chip label="OK" color="success" size="small" />
        <Chip label="Due Soon" color="warning" size="small" />
        <Chip label="Overdue" color="error" size="small" />
      </Box>
    </Box>
  );
};

export default MaintenanceCalendar;
