import { useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { doctorPortalApi } from '../../api';
import { StatusBadge } from '../../components/StatusBadge';
import { formatDate, formatTime, todayIso } from '../../utils/format';

export function DoctorSchedulePage() {
  const [searchParams] = useSearchParams();
  const [selectedDate, setSelectedDate] = useState(() => searchParams.get('date') ?? todayIso());

  useEffect(() => {
    const date = searchParams.get('date');
    if (date) setSelectedDate(date);
  }, [searchParams]);

  const { data: dayHours, isLoading: hoursLoading } = useQuery({
    queryKey: ['doctor-working-hours', selectedDate],
    queryFn: () => doctorPortalApi.getWorkingHours(selectedDate),
    retry: false,
  });

  const { data: appointments = [], isLoading } = useQuery({
    queryKey: ['doctor-appointments', selectedDate],
    queryFn: () => doctorPortalApi.getSchedule(selectedDate),
  });

  const hasHours = !!dayHours;

  return (
    <div className="space-y-6">
      <div>
        <h2 className="mb-2 text-2xl font-bold">My Schedule</h2>
        <p className="text-sm text-slate-500">View your working hours and appointments for each date.</p>
      </div>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <div className="mb-4 flex flex-wrap items-end justify-between gap-4">
          <div>
            <h3 className="text-lg font-semibold">Date</h3>
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
          <div className="h-8 animate-pulse rounded bg-slate-100" />
        ) : hasHours ? (
          <p className="text-sm text-slate-600">
            Working hours: {formatTime(dayHours.startTime)} – {formatTime(dayHours.endTime)}
            <span className="ml-2 text-slate-400">({dayHours.appointmentDurationMinutes} min slots)</span>
          </p>
        ) : (
          <p className="rounded-lg bg-amber-50 px-4 py-3 text-sm text-amber-800">
            No working hours configured for this date. Contact the admin.
          </p>
        )}
      </div>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <h3 className="mb-4 text-lg font-semibold">Appointments</h3>
        {isLoading ? (
          <div className="space-y-3">{[...Array(3)].map((_, i) => <div key={i} className="h-20 animate-pulse rounded-xl bg-slate-100" />)}</div>
        ) : appointments.length === 0 ? (
          <p className="rounded-lg border border-dashed p-8 text-center text-slate-500">
            No appointments on this date.
          </p>
        ) : (
          <div className="grid gap-4 md:grid-cols-2">
            {appointments.map((appt) => (
              <Link
                key={appt.id}
                to={`/doctor/appointments/${appt.id}`}
                className="block rounded-xl border p-5 shadow-sm transition hover:border-primary-300 hover:shadow-md"
              >
                <div className="mb-2 flex items-center justify-between">
                  <span className="font-semibold text-slate-900">{formatTime(appt.startTime)}</span>
                  <StatusBadge status={appt.status} />
                </div>
                <p className="text-sm text-slate-600">{appt.patientName}</p>
              </Link>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
