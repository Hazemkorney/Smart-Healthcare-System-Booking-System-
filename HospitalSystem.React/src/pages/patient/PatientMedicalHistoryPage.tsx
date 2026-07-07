import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { patientApi } from '../../api';
import { formatDate, formatTime } from '../../utils/format';

export function PatientMedicalHistoryPage() {
  const { data: history = [], isLoading, isError } = useQuery({
    queryKey: ['patient-medical-history'],
    queryFn: patientApi.getMedicalHistory,
  });

  if (isError) return <p className="text-red-600">Unable to load medical history.</p>;

  return (
    <div>
      <h2 className="mb-2 text-2xl font-bold">Medical History</h2>
      <p className="mb-6 text-sm text-slate-500">Your completed visits, diagnoses, and prescriptions.</p>

      {isLoading ? (
        <div className="space-y-3">{[...Array(3)].map((_, i) => <div key={i} className="h-24 animate-pulse rounded-xl bg-slate-100" />)}</div>
      ) : history.length === 0 ? (
        <p className="rounded-xl border bg-white p-8 text-center text-slate-500">
          No completed visits on record yet. Medical history appears after your doctor completes the consultation.
        </p>
      ) : (
        <div className="space-y-4">
          {history.map((entry) => (
            <Link
              key={entry.appointmentId}
              to={`/patient/appointments/${entry.appointmentId}`}
              className="block rounded-xl border bg-white p-5 shadow-sm transition hover:border-primary-300"
            >
              <div className="mb-2 flex flex-wrap items-center justify-between gap-2">
                <span className="font-medium text-slate-900">
                  {formatDate(entry.appointmentDate)} · {formatTime(entry.startTime)}
                </span>
                <span className="text-sm text-slate-500">{entry.doctorName}</span>
              </div>
              {entry.diagnosis && (
                <p className="text-sm text-slate-700">
                  <span className="font-medium">Diagnosis:</span> {entry.diagnosis}
                </p>
              )}
              {entry.notes && (
                <p className="mt-1 text-sm text-slate-600">
                  <span className="font-medium">Notes:</span> {entry.notes}
                </p>
              )}
              {entry.prescriptions.length > 0 && (
                <ul className="mt-2 space-y-1 text-sm text-slate-600">
                  {entry.prescriptions.map((p) => (
                    <li key={p.id}>
                      {p.medicationName} — {p.dosage}, {p.frequency}, {p.duration}
                    </li>
                  ))}
                </ul>
              )}
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
