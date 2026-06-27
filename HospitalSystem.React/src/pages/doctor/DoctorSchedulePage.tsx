import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { doctorPortalApi } from '../../api';
import { StatusBadge } from '../../components/StatusBadge';
import { formatDate, formatTime, todayIso } from '../../utils/format';

export function DoctorSchedulePage() {
  const today = todayIso();
  const { data: appointments = [], isLoading } = useQuery({
    queryKey: ['doctor-schedule', today],
    queryFn: () => doctorPortalApi.getSchedule(today),
  });

  return (
    <div>
      <h2 className="mb-2 text-2xl font-bold">My Schedule</h2>
      <p className="mb-6 text-sm text-slate-500">{formatDate(today)}</p>

      {isLoading ? (
        <div className="space-y-3">{[...Array(4)].map((_, i) => <div key={i} className="h-20 animate-pulse rounded-xl bg-slate-100" />)}</div>
      ) : appointments.length === 0 ? (
        <p className="rounded-xl border bg-white p-8 text-center text-slate-500">No appointments today.</p>
      ) : (
        <div className="grid gap-4 md:grid-cols-2">
          {appointments.map((appt) => (
            <Link key={appt.id} to={`/doctor/appointments/${appt.id}`} className="block rounded-xl border bg-white p-5 shadow-sm transition hover:border-primary-300 hover:shadow-md">
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
  );
}
