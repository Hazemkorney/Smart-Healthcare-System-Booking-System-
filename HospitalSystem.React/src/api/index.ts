import type {
  ApiResponse,
  Appointment,
  AppointmentDetail,
  AvailableSlot,
  Consultation,
  CurrentUser,
  Department,
  Doctor,
  DoctorAppointmentDetail,
  DoctorSchedule,
  LoginResponse,
  PagedResponse,
  Patient,
  Receptionist,
} from '../types';
import { apiClient } from './client';

const ALL_ITEMS_PAGE_SIZE = 500;

async function unwrap<T>(promise: Promise<{ data: ApiResponse<T> }>): Promise<T> {
  const { data } = await promise;
  return data.data;
}

export const authApi = {
  login: (email: string, password: string) =>
    unwrap<LoginResponse>(apiClient.post('/auth/login', { email, password })),
  me: () => unwrap<CurrentUser>(apiClient.get('/auth/me')),
};

export const departmentApi = {
  getAll: (page = 1, pageSize = 10) =>
    unwrap<PagedResponse<Department>>(apiClient.get('/departments', { params: { page, pageSize } })),
  getAllItems: () => departmentApi.getAll(1, ALL_ITEMS_PAGE_SIZE).then((r) => r.data),
  create: (body: { name: string; description?: string }) =>
    unwrap<Department>(apiClient.post('/departments', body)),
  update: (id: string, body: { name: string; description?: string }) =>
    unwrap<Department>(apiClient.put(`/departments/${id}`, body)),
  delete: (id: string) => unwrap<unknown>(apiClient.delete(`/departments/${id}`)),
};

export const doctorApi = {
  getAll: (page = 1, pageSize = 10) =>
    unwrap<PagedResponse<Doctor>>(apiClient.get('/doctors', { params: { page, pageSize } })),
  getAllItems: () => doctorApi.getAll(1, ALL_ITEMS_PAGE_SIZE).then((r) => r.data),
  getByDepartment: (departmentId: string) =>
    unwrap<Doctor[]>(apiClient.get(`/doctors/department/${departmentId}`)),
  create: (body: Record<string, unknown>) => unwrap<Doctor>(apiClient.post('/doctors', body)),
  update: (id: string, body: Record<string, unknown>) =>
    unwrap<Doctor>(apiClient.put(`/doctors/${id}`, body)),
  delete: (id: string) => unwrap<unknown>(apiClient.delete(`/doctors/${id}`)),
  getSchedule: (id: string) => unwrap<DoctorSchedule[]>(apiClient.get(`/doctors/${id}/schedule`)),
  setSchedule: (id: string, schedules: Record<string, unknown>[]) =>
    unwrap<DoctorSchedule[]>(apiClient.post(`/doctors/${id}/schedule`, { schedules })),
};

export const receptionistApi = {
  getAll: (page = 1, pageSize = 10) =>
    unwrap<PagedResponse<Receptionist>>(apiClient.get('/receptionists', { params: { page, pageSize } })),
  create: (body: Record<string, unknown>) =>
    unwrap<Receptionist>(apiClient.post('/receptionists', body)),
  update: (id: string, body: Record<string, unknown>) =>
    unwrap<Receptionist>(apiClient.put(`/receptionists/${id}`, body)),
  delete: (id: string) => unwrap<unknown>(apiClient.delete(`/receptionists/${id}`)),
};

export const patientApi = {
  search: (query?: string, page = 1, pageSize = 10) =>
    unwrap<PagedResponse<Patient>>(apiClient.get('/patients', { params: { query, page, pageSize } })),
  searchAll: (query?: string) => patientApi.search(query, 1, ALL_ITEMS_PAGE_SIZE).then((r) => r.data),
  getById: (id: string) => unwrap<Patient>(apiClient.get(`/patients/${id}`)),
  create: (body: Record<string, unknown>) => unwrap<Patient>(apiClient.post('/patients', body)),
  update: (id: string, body: Record<string, unknown>) =>
    unwrap<Patient>(apiClient.put(`/patients/${id}`, body)),
  getProfile: () => unwrap<Patient>(apiClient.get('/patient/profile')),
  getAppointments: () => unwrap<Appointment[]>(apiClient.get('/patient/appointments')),
  getAppointmentDetail: (id: string) =>
    unwrap<AppointmentDetail>(apiClient.get(`/patient/appointments/${id}`)),
};

export const appointmentApi = {
  getByDoctorAndDate: (doctorId: string, date: string) =>
    unwrap<Appointment[]>(apiClient.get('/appointments', { params: { doctorId, date } })),
  getAvailableSlots: (doctorId: string, date: string) =>
    unwrap<AvailableSlot[]>(apiClient.get('/appointments/available-slots', { params: { doctorId, date } })),
  getById: (id: string) => unwrap<Appointment>(apiClient.get(`/appointments/${id}`)),
  book: (body: Record<string, unknown>) => unwrap<Appointment>(apiClient.post('/appointments', body)),
  reschedule: (id: string, body: { newDate: string; newStartTime: string }) =>
    unwrap<Appointment>(apiClient.put(`/appointments/${id}/reschedule`, body)),
  cancel: (id: string) => unwrap<unknown>(apiClient.put(`/appointments/${id}/cancel`)),
  checkIn: (id: string) => unwrap<Appointment>(apiClient.put(`/appointments/${id}/checkin`)),
};

export const doctorPortalApi = {
  getSchedule: (date: string) =>
    unwrap<Appointment[]>(apiClient.get('/doctor/schedule', { params: { date } })),
  getAppointment: (id: string) =>
    unwrap<DoctorAppointmentDetail>(apiClient.get(`/doctor/appointments/${id}`)),
  startConsultation: (id: string) =>
    unwrap<Consultation>(apiClient.post(`/doctor/appointments/${id}/start`)),
  addDiagnosis: (id: string, body: { diagnosis: string; notes?: string }) =>
    unwrap<Consultation>(apiClient.put(`/doctor/appointments/${id}/diagnosis`, body)),
  addPrescription: (id: string, body: Record<string, unknown>) =>
    unwrap<Consultation>(apiClient.post(`/doctor/appointments/${id}/prescriptions`, body)),
  complete: (id: string) => unwrap<Consultation>(apiClient.put(`/doctor/appointments/${id}/complete`)),
};
