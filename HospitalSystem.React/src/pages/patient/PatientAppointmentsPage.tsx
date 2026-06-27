import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { patientApi } from '../../api';
import { StatusBadge } from '../../components/StatusBadge';
import { formatDate, formatTime } from '../../utils/format';

export function PatientAppointmentsPage() {
  const { data: appointments = [], isLoading, isError } = useQuery({
    queryKey: ['patient-appointments'],
    queryFn: patientApi.getAppointments,
  });

  if (isError) return <p className="text-red-600">Unable to load appointments.</p>;

  return (
    <div>
      <h2 className="mb-6 text-2xl font-bold">My Appointments</h2>
      {isLoading ? (
        <div className="space-y-3">{[...Array(3)].map((_, i) => <div key={i} className="h-16 animate-pulse rounded-xl bg-slate-100" />)}</div>
      ) : appointments.length === 0 ? (
        <p className="rounded-xl border bg-white p-8 text-center text-slate-500">No appointments yet.</p>
      ) : (
        <div className="space-y-3">
          {appointments.map((appt) => (
            <Link key={appt.id} to={`/patient/appointments/${appt.id}`} className="flex items-center justify-between rounded-xl border bg-white p-4 shadow-sm hover:border-primary-300">
              <div>
                <p className="font-medium">{appt.doctorName}</p>
                <p className="text-sm text-slate-500">{formatDate(appt.appointmentDate)} at {formatTime(appt.startTime)}</p>
              </div>
              <StatusBadge status={appt.status} />
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
