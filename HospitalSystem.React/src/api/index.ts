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
  DoctorDateSchedule,
  LoginResponse,
  PagedResponse,
  Patient,
  PatientMedicalHistoryEntry,
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
  getDefaultSchedule: () =>
    unwrap<DoctorSchedule[]>(apiClient.get('/doctors/default-schedule')),
  setDefaultSchedule: (schedules: Record<string, unknown>[]) =>
    unwrap<DoctorSchedule[]>(apiClient.put('/doctors/default-schedule', { schedules })),
  applyDefaultScheduleToAll: () =>
    unwrap<unknown>(apiClient.post('/doctors/default-schedule/apply-all')),
  getDefaultDateSchedules: (from?: string, to?: string) =>
    unwrap<DoctorDateSchedule[]>(
      apiClient.get('/doctors/default-date-schedules', {
        params: {
          ...(from ? { from } : {}),
          ...(to ? { to } : {}),
        },
      }),
    ),
  setDefaultDateSchedule: (body: Record<string, unknown>) =>
    unwrap<DoctorDateSchedule>(apiClient.put('/doctors/default-date-schedules', body)),
  removeDefaultDateSchedule: (date: string) =>
    unwrap<unknown>(apiClient.delete(`/doctors/default-date-schedules/${date}`)),
  applyDefaultDateSchedulesToAll: (from: string, to: string) =>
    unwrap<unknown>(
      apiClient.post('/doctors/default-date-schedules/apply-all', null, { params: { from, to } }),
    ),
  applySelectedDefaultDateSchedulesToAll: (dates: string[]) =>
    unwrap<unknown>(
      apiClient.post('/doctors/default-date-schedules/apply-selected', { dates }),
    ),
  getAppliedDateSchedules: () =>
    unwrap<DoctorDateSchedule[]>(apiClient.get('/doctors/applied-date-schedules')),
  applyDateScheduleToAll: (body: Record<string, unknown>) =>
    unwrap<DoctorDateSchedule>(apiClient.put('/doctors/applied-date-schedules', body)),
  removeAppliedDateSchedule: (date: string) =>
    unwrap<unknown>(apiClient.delete(`/doctors/applied-date-schedules/${date}`)),
  getDateSchedules: (id: string, from?: string, to?: string) =>
    unwrap<DoctorDateSchedule[]>(
      apiClient.get(`/doctors/${id}/date-schedules`, {
        params: {
          ...(from ? { from } : {}),
          ...(to ? { to } : {}),
        },
      }),
    ),
  setDateSchedule: (id: string, body: Record<string, unknown>) =>
    unwrap<DoctorDateSchedule>(apiClient.put(`/doctors/${id}/date-schedules`, body)),
  removeDateSchedule: (id: string, date: string) =>
    unwrap<unknown>(apiClient.delete(`/doctors/${id}/date-schedules/${date}`)),
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
  getMedicalHistory: () => unwrap<PatientMedicalHistoryEntry[]>(apiClient.get('/patient/medical-history')),
  getAppointmentDetail: (id: string) =>
    unwrap<AppointmentDetail>(apiClient.get(`/patient/appointments/${id}`)),
};

export const appointmentApi = {
  getByDoctorAndDate: (doctorId: string, date: string) =>
    unwrap<Appointment[]>(apiClient.get('/appointments', { params: { doctorId, date } })),
  getAvailableSlots: (doctorId: string, date: string, patientId: string, excludeAppointmentId?: string) =>
    unwrap<AvailableSlot[]>(
      apiClient.get('/appointments/available-slots', {
        params: {
          doctorId,
          date,
          patientId,
          ...(excludeAppointmentId ? { excludeAppointmentId } : {}),
        },
      }),
    ),
  getById: (id: string) => unwrap<Appointment>(apiClient.get(`/appointments/${id}`)),
  book: (body: Record<string, unknown>) => unwrap<Appointment>(apiClient.post('/appointments', body)),
  reschedule: (id: string, body: { newDate: string; newStartTime: string }) =>
    unwrap<Appointment>(apiClient.put(`/appointments/${id}/reschedule`, body)),
  cancel: (id: string) => unwrap<unknown>(apiClient.put(`/appointments/${id}/cancel`)),
  checkIn: (id: string) => unwrap<Appointment>(apiClient.put(`/appointments/${id}/checkin`)),
};

export const doctorPortalApi = {
  getWorkingHours: (date: string) =>
    unwrap<DoctorDateSchedule>(apiClient.get('/doctor/working-hours', { params: { date } })),
  getDateSchedules: () =>
    unwrap<DoctorDateSchedule[]>(apiClient.get('/doctor/date-schedules')),
  getSchedule: (date: string) =>
    unwrap<Appointment[]>(apiClient.get('/doctor/schedule', { params: { date } })),
  getAppointment: (id: string) =>
    unwrap<DoctorAppointmentDetail>(apiClient.get(`/doctor/appointments/${id}`)),
  getMedicalHistory: (patientId: string, excludeAppointmentId?: string) =>
    unwrap<PatientMedicalHistoryEntry[]>(
      apiClient.get(`/doctor/patients/${patientId}/medical-history`, {
        params: excludeAppointmentId ? { excludeAppointmentId } : undefined,
      }),
    ),
  startConsultation: (id: string) =>
    unwrap<Consultation>(apiClient.post(`/doctor/appointments/${id}/start`)),
  addDiagnosis: (id: string, body: { diagnosis: string; notes?: string }) =>
    unwrap<Consultation>(apiClient.put(`/doctor/appointments/${id}/diagnosis`, body)),
  addPrescription: (id: string, body: Record<string, unknown>) =>
    unwrap<Consultation>(apiClient.post(`/doctor/appointments/${id}/prescriptions`, body)),
  complete: (id: string) => unwrap<Consultation>(apiClient.put(`/doctor/appointments/${id}/complete`)),
};
