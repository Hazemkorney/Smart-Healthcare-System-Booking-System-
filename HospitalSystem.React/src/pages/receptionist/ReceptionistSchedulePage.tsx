import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Search } from 'lucide-react';
import { doctorApi, appointmentApi, patientApi } from '../../api';import { DataTable, type Column } from '../../components/DataTable';
import { StatusBadge } from '../../components/StatusBadge';
import { useApiMutation } from '../../hooks/useApiMutation';
import { formatDate, formatTime, findConflictingAppointmentIds, todayIso } from '../../utils/format';
import type { Appointment, Doctor, DoctorDateSchedule } from '../../types';

interface DoctorDaySchedule {
  doctor: Doctor;
  schedule?: DoctorDateSchedule;
}

export function ReceptionistSchedulePage() {
  const [selectedDate, setSelectedDate] = useState(todayIso());
  const [appointmentSearch, setAppointmentSearch] = useState('');

  const { data: doctors = [] } = useQuery({ queryKey: ['doctors', 'all'], queryFn: doctorApi.getAllItems });
  const { data: patients = [] } = useQuery({
    queryKey: ['patients', 'all'],
    queryFn: () => patientApi.searchAll(),
  });

  const { data: daySchedules = [], isLoading: hoursLoading } = useQuery({
    queryKey: ['doctors-date-schedules', selectedDate],
    queryFn: async (): Promise<DoctorDaySchedule[]> => {
      const allDoctors = await doctorApi.getAllItems();
      return Promise.all(
        allDoctors.map(async (doctor) => {
          const schedules = await doctorApi.getDateSchedules(doctor.id, selectedDate, selectedDate);
          return { doctor, schedule: schedules[0] };
        }),
      );
    },
  });

  const { data: appointments = [], isLoading, refetch } = useQuery({
    queryKey: ['receptionist-schedule', selectedDate],
    queryFn: async () => {
      const results: Appointment[] = [];
      for (const doc of doctors) {
        const appts = await appointmentApi.getByDoctorAndDate(doc.id, selectedDate);
        results.push(...appts);
      }
      return results.sort((a, b) => a.startTime.localeCompare(b.startTime));
    },
    enabled: doctors.length > 0,
  });

  const checkInMutation = useApiMutation({
    mutationFn: appointmentApi.checkIn,
    successMessage: 'Patient checked in',
    onSuccess: () => refetch(),
  });

  const conflictingIds = useMemo(
    () => findConflictingAppointmentIds(appointments),
    [appointments],
  );

  const columns: Column<Appointment>[] = useMemo(
    () => [
      { key: 'time', header: 'Time', render: (r) => formatTime(r.startTime) },
      { key: 'doctor', header: 'Doctor', render: (r) => r.doctorName },
      { key: 'patient', header: 'Patient', render: (r) => r.patientName },
      {
        key: 'status',
        header: 'Status',
        render: (r) => (
          <div className="flex flex-wrap items-center gap-2">
            <StatusBadge status={r.status} />
            {conflictingIds.has(r.id) && (
              <span className="rounded bg-red-100 px-2 py-0.5 text-xs font-medium text-red-700">
                Conflict
              </span>
            )}
          </div>
        ),
      },
      {
        key: 'actions',
        header: 'Actions',
        render: (r) => (
          <div className="flex gap-2">
            <Link to={`/receptionist/appointments/${r.id}`} className="text-sm text-primary-600 hover:underline">View</Link>
            {(r.status === 'Confirmed' || r.status === 0) && (
              <button type="button" onClick={() => checkInMutation.mutate(r.id)} className="text-sm text-green-600 hover:underline">Check-In</button>
            )}
          </div>
        ),
      },
    ],
    [checkInMutation, conflictingIds],
  );

  const visibleDaySchedules = useMemo(() => {
    const doctorIdsWithAppointments = new Set(appointments.map((a) => a.doctorId));
    return daySchedules.filter(
      ({ doctor, schedule }) => schedule || doctorIdsWithAppointments.has(doctor.id),
    );
  }, [daySchedules, appointments]);

  const patientPhoneById = useMemo(
    () => new Map(patients.map((p) => [p.id, p.phone])),
    [patients],
  );

  const filteredAppointments = useMemo(() => {
    const query = appointmentSearch.trim().toLowerCase();
    if (!query) return appointments;

    const queryDigits = query.replace(/\D/g, '');
    return appointments.filter((appointment) => {
      if (appointment.patientName.toLowerCase().includes(query)) return true;

      const phone = patientPhoneById.get(appointment.patientId);
      if (!phone) return false;

      const phoneLower = phone.toLowerCase();
      if (phoneLower.includes(query)) return true;

      return queryDigits.length > 0 && phone.replace(/\D/g, '').includes(queryDigits);
    });
  }, [appointments, appointmentSearch, patientPhoneById]);

  return (
    <div className="space-y-8">
      <div>
        <h2 className="mb-2 text-2xl font-bold">Schedule</h2>
        <p className="text-sm text-slate-500">Weekly doctor hours and daily appointments.</p>
      </div>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <div className="mb-4 flex flex-wrap items-end justify-between gap-4">
          <div>
            <h3 className="text-lg font-semibold">Doctors&apos; Hours</h3>
            <p className="text-sm text-slate-500">{formatDate(selectedDate)}</p>
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-slate-500">Select date</label>
            <input
              type="date"
              value={selectedDate}
              onChange={(e) => setSelectedDate(e.target.value)}
              className="rounded-lg border px-3 py-2 text-sm"
            />
          </div>
        </div>
        {hoursLoading ? (
          <div className="h-32 animate-pulse rounded-lg bg-slate-100" />
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full text-sm">
              <thead>
                <tr className="border-b text-left text-slate-500">
                  <th className="pb-3 pr-4 font-medium">Doctor</th>
                  <th className="pb-3 pr-4 font-medium">Working hours</th>
                </tr>
              </thead>
              <tbody>
                {visibleDaySchedules.length === 0 ? (
                  <tr>
                    <td colSpan={2} className="py-6 text-center text-slate-500">
                      No doctor hours configured for this date.
                    </td>
                  </tr>
                ) : (
                  visibleDaySchedules.map(({ doctor, schedule }) => (
                    <tr key={doctor.id} className="border-b last:border-0">
                      <td className="py-3 pr-4 font-medium">
                        {doctor.fullName}
                        <span className="ml-2 text-xs font-normal text-slate-400">{doctor.specialization}</span>
                      </td>
                      <td className="py-3 pr-4 text-slate-600">
                        {schedule
                          ? `${formatTime(schedule.startTime)} – ${formatTime(schedule.endTime)}`
                          : '—'}
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <div>
        <div className="mb-4 flex flex-wrap items-center justify-between gap-4">
          <h3 className="text-lg font-semibold">Appointments</h3>
          <div className="relative w-full max-w-sm">
            <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
            <input
              type="search"
              value={appointmentSearch}
              onChange={(e) => setAppointmentSearch(e.target.value)}
              placeholder="Search by patient name or phone..."
              className="w-full rounded-lg border py-2 pl-9 pr-3 text-sm"
            />
          </div>
        </div>
        {conflictingIds.size > 0 && (
          <p className="mb-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-800">
            Some patients have overlapping appointments with different doctors. Cancel the duplicate
            booking — new bookings at the same time are now blocked.
          </p>
        )}
        <DataTable
          columns={columns}
          data={filteredAppointments}
          loading={isLoading}
          emptyMessage={
            appointmentSearch.trim()
              ? 'No appointments match your search.'
              : 'No appointments on this date.'
          }
        />
      </div>
    </div>
  );
}
