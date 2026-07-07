import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { patientApi } from '../../api';
import { StatusBadge } from '../../components/StatusBadge';
import { formatDate, formatTime } from '../../utils/format';

export function PatientAppointmentDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { data, isLoading } = useQuery({
    queryKey: ['patient-appointment', id],
    queryFn: () => patientApi.getAppointmentDetail(id!),
    enabled: !!id,
  });

  if (isLoading) return <div className="h-40 animate-pulse rounded-xl bg-slate-100" />;
  if (!data) return <p>Not found</p>;

  const { appointment, consultation } = data;
  const isCompleted = appointment.status === 'Completed' || appointment.status === 3;

  return (
    <div className="max-w-2xl">
      <Link to="/patient/appointments" className="mb-4 inline-block text-sm text-slate-600 hover:underline">← Back</Link>
      <div className="mb-6 flex items-center justify-between">
        <h2 className="text-2xl font-bold">Appointment Detail</h2>
        <StatusBadge status={appointment.status} />
      </div>
      <div className="mb-6 rounded-xl border bg-white p-6 shadow-sm">
        <dl className="space-y-2 text-sm">
          <div className="flex justify-between"><dt className="text-slate-500">Doctor</dt><dd>{appointment.doctorName}</dd></div>
          <div className="flex justify-between"><dt className="text-slate-500">Date</dt><dd>{formatDate(appointment.appointmentDate)}</dd></div>
          <div className="flex justify-between"><dt className="text-slate-500">Time</dt><dd>{formatTime(appointment.startTime)}</dd></div>
        </dl>
      </div>
      {!isCompleted && (
        <p className="mb-6 rounded-lg bg-slate-50 px-4 py-3 text-sm text-slate-600">
          Diagnosis and prescriptions will appear here after your doctor completes the consultation.
        </p>
      )}
      {isCompleted && consultation?.diagnosis && (
        <div className="mb-6 rounded-xl border bg-white p-6 shadow-sm">
          <h3 className="mb-2 font-semibold">Diagnosis</h3>
          <p className="text-sm text-slate-700">{consultation.diagnosis}</p>
          {consultation.notes && <p className="mt-2 text-sm text-slate-600">{consultation.notes}</p>}
        </div>
      )}
      {isCompleted && consultation?.prescriptions && consultation.prescriptions.length > 0 && (
        <div className="rounded-xl border bg-white p-6 shadow-sm">
          <h3 className="mb-3 font-semibold">Prescriptions</h3>
          {consultation.prescriptions.map((p) => (
            <div key={p.id} className="mb-2 rounded-lg bg-slate-50 p-3 text-sm">
              <strong>{p.medicationName}</strong> — {p.dosage}, {p.frequency}, {p.duration}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
