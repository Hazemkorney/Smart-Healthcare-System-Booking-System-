export type UserRole = 'Admin' | 'Receptionist' | 'Doctor' | 'Patient';

export type AppointmentStatus =
  | 'Confirmed'
  | 'CheckedIn'
  | 'InProgress'
  | 'Completed'
  | 'Cancelled'
  | 'NoShow';

export type Gender = 'Male' | 'Female' | 'Other';

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
}

export interface PagedResponse<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiError {
  success: false;
  message: string;
  errors?: string[];
}

export interface LoginResponse {
  token: string;
  userId: string;
  email: string;
  role: UserRole | number;
}

export interface CurrentUser {
  userId: string;
  email: string;
  role: UserRole;
}

export interface Department {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
}

export interface Doctor {
  id: string;
  userId: string;
  departmentId: string;
  departmentName: string;
  fullName: string;
  specialization: string;
  phone?: string;
  isActive: boolean;
}

export interface DoctorSchedule {
  id: string;
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  appointmentDurationMinutes: number;
  isActive: boolean;
}

export interface Patient {
  id: string;
  userId?: string;
  fullName: string;
  dateOfBirth: string;
  gender: Gender | number;
  phone: string;
  email?: string;
  address?: string;
  nationalId?: string;
  bloodType?: string;
  createdAt: string;
}

export interface Appointment {
  id: string;
  patientId: string;
  patientName: string;
  doctorId: string;
  doctorName: string;
  appointmentDate: string;
  startTime: string;
  endTime: string;
  status: AppointmentStatus | number;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface AvailableSlot {
  startTime: string;
  endTime: string;
}

export interface Prescription {
  id: string;
  medicationName: string;
  dosage: string;
  frequency: string;
  duration: string;
  notes?: string;
}

export interface Consultation {
  id: string;
  appointmentId: string;
  doctorId: string;
  patientId: string;
  diagnosis?: string;
  notes?: string;
  startedAt: string;
  completedAt?: string;
  prescriptions: Prescription[];
}

export interface AppointmentDetail {
  appointment: Appointment;
  consultation?: Consultation | null;
}

export interface DoctorAppointmentDetail {
  appointment: Appointment;
  patient: Patient;
  consultation?: Consultation | null;
}

export interface Receptionist {
  id: string;
  userId: string;
  fullName: string;
  phone?: string;
  isActive: boolean;
}
