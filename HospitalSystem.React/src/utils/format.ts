import type { AppointmentStatus, Gender, UserRole } from '../types';

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
  return new Date().toISOString().slice(0, 10);
}

export const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

export function roleHomePath(role: UserRole): string {
  switch (role) {
    case 'Admin':
      return '/admin/departments';
    case 'Receptionist':
      return '/receptionist/patients';
    case 'Doctor':
      return '/doctor/schedule';
    case 'Patient':
      return '/patient/profile';
    default:
      return '/login';
  }
}
