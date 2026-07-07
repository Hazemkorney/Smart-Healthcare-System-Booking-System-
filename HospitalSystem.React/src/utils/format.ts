import type { Appointment, AppointmentStatus, Gender, UserRole } from '../types';

const roleMap: Record<number, UserRole> = {
  0: 'Admin',
  1: 'Receptionist',
  2: 'Doctor',
  3: 'Patient',
};

const statusMap: Record<number, AppointmentStatus> = {
  0: 'Confirmed',
  1: 'CheckedIn',
  2: 'InProgress',
  3: 'Completed',
  4: 'Cancelled',
  5: 'NoShow',
};

const genderMap: Record<number, Gender> = {
  0: 'Male',
  1: 'Female',
  2: 'Other',
};

export function parseRole(role: UserRole | number): UserRole {
  if (typeof role === 'string') return role;
  return roleMap[role] ?? 'Patient';
}

export function parseStatus(status: AppointmentStatus | number): AppointmentStatus {
  if (typeof status === 'string') return status;
  return statusMap[status] ?? 'Confirmed';
}

export function parseGender(gender: Gender | number): Gender {
  if (typeof gender === 'string') return gender;
  return genderMap[gender] ?? 'Other';
}

export function formatTime(time: string): string {
  return time.slice(0, 5);
}

export function formatDate(date: string): string {
  return new Date(date + 'T00:00:00').toLocaleDateString(undefined, {
    weekday: 'short',
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

export function todayIso(): string {
  const date = new Date();
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

export const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

const dayOfWeekMap: Record<string, number> = {
  Sunday: 0,
  Monday: 1,
  Tuesday: 2,
  Wednesday: 3,
  Thursday: 4,
  Friday: 5,
  Saturday: 6,
};

export function parseDayOfWeek(day: number | string): number {
  if (typeof day === 'number') return day;
  return dayOfWeekMap[day] ?? 0;
}

export function formatDayOfWeek(day: number | string): string {
  return dayNames[parseDayOfWeek(day)] ?? String(day);
}

export function isAppointmentDue(appointmentDate: string, startTime: string): boolean {
  const [hours, minutes] = startTime.split(':').map(Number);
  const start = new Date(`${appointmentDate}T00:00:00`);
  start.setHours(hours, minutes, 0, 0);
  return Date.now() >= start.getTime();
}

export function validateDateScheduleInput(
  date: string,
  startTime: string,
  endTime: string,
): string | null {
  if (startTime >= endTime) return 'Start time must be before end time.';

  const today = todayIso();
  if (date < today) return 'Cannot set schedule for a past date.';

  if (date === today) {
    const now = new Date();
    const nowMinutes = now.getHours() * 60 + now.getMinutes();
    const [sh, sm] = startTime.split(':').map(Number);
    const [eh, em] = endTime.split(':').map(Number);
    const startMinutes = sh * 60 + sm;
    const endMinutes = eh * 60 + em;
    if (startMinutes <= nowMinutes) return 'Start time cannot be in the past for today.';
    if (endMinutes <= nowMinutes) return 'End time cannot be in the past for today.';
  }

  return null;
}

export function patientAppointmentsOverlap(a: Appointment, b: Appointment): boolean {
  if (a.patientId !== b.patientId || a.appointmentDate !== b.appointmentDate) return false;
  if (parseStatus(a.status) === 'Cancelled' || parseStatus(b.status) === 'Cancelled') return false;
  return a.startTime < b.endTime && b.startTime < a.endTime;
}

export function findConflictingAppointmentIds(appointments: Appointment[]): Set<string> {
  const ids = new Set<string>();
  for (let i = 0; i < appointments.length; i++) {
    for (let j = i + 1; j < appointments.length; j++) {
      if (patientAppointmentsOverlap(appointments[i], appointments[j])) {
        ids.add(appointments[i].id);
        ids.add(appointments[j].id);
      }
    }
  }
  return ids;
}

export function roleHomePath(role: UserRole): string {
  switch (role) {
    case 'Admin':
      return '/admin/departments';
    case 'Receptionist':
      return '/receptionist/patients';
    case 'Doctor':
      return '/doctor/schedule';
    case 'Patient':
      return '/patient/medical-history';
    default:
      return '/login';
  }
}
