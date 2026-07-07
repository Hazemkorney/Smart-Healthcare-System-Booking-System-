import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { doctorPortalApi } from '../../api';
import { StatusBadge } from '../../components/StatusBadge';
import { useApiMutation } from '../../hooks/useApiMutation';
import { formatDate, formatTime, isAppointmentDue, parseGender } from '../../utils/format';

export function ConsultationPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [diagnosis, setDiagnosis] = useState('');
  const [notes, setNotes] = useState('');
  const [medication, setMedication] = useState('');
  const [dosage, setDosage] = useState('');
  const [frequency, setFrequency] = useState('');
  const [duration, setDuration] = useState('');
  const [started, setStarted] = useState(false);

  const { data, isLoading, refetch } = useQuery({
    queryKey: ['doctor-appointment', id],
    queryFn: () => doctorPortalApi.getAppointment(id!),
    enabled: !!id,
  });

  const patientId = data?.patient.id;
  const { data: medicalHistory = [], isLoading: historyLoading } = useQuery({
    queryKey: ['doctor-medical-history', patientId, id],
    queryFn: () => doctorPortalApi.getMedicalHistory(patientId!, id),
    enabled: !!patientId && !!id,
  });

  const startMutation = useApiMutation({
    mutationFn: () => doctorPortalApi.startConsultation(id!),
    successMessage: 'Consultation started',
    onSuccess: () => { setStarted(true); refetch(); },
  });

  const diagnosisMutation = useApiMutation({
    mutationFn: () => doctorPortalApi.addDiagnosis(id!, { diagnosis, notes }),
    successMessage: 'Diagnosis saved',
    onSuccess: () => refetch(),
  });

  const prescriptionMutation = useApiMutation({
    mutationFn: () => doctorPortalApi.addPrescription(id!, { medicationName: medication, dosage, frequency, duration }),
    successMessage: 'Prescription added',
    onSuccess: () => { setMedication(''); setDosage(''); setFrequency(''); setDuration(''); refetch(); },
  });

  const completeMutation = useApiMutation({
    mutationFn: () => doctorPortalApi.complete(id!),
    successMessage: 'Appointment completed',
    onSuccess: () => navigate('/doctor/schedule'),
  });

  if (isLoading) return <div className="h-60 animate-pulse rounded-xl bg-slate-100" />;
  if (!data) return <p>Not found</p>;

  const { appointment, patient, consultation } = data;
  const inProgress = started || !!consultation;
  const isCompleted = appointment.status === 'Completed' || appointment.status === 3;
  const canStart = isAppointmentDue(appointment.appointmentDate, appointment.startTime);
  const hasSavedDiagnosis = Boolean(consultation?.diagnosis?.trim());
  const canComplete = hasSavedDiagnosis;

  const handleSaveDiagnosis = () => {
    if (!diagnosis.trim()) return;
    diagnosisMutation.mutate(undefined);
  };

  const handleAddPrescription = () => {
    if (!medication.trim() || !dosage.trim() || !frequency.trim() || !duration.trim()) return;
    prescriptionMutation.mutate(undefined);
  };

  return (
    <div className="max-w-4xl">
      <button type="button" onClick={() => navigate('/doctor/schedule')} className="mb-4 text-sm text-slate-600 hover:underline">← Back to schedule</button>

      <div className="mb-6 flex items-center justify-between">
        <h2 className="text-2xl font-bold">Consultation</h2>
        <StatusBadge status={appointment.status} />
      </div>

      <div className="mb-6 grid gap-4 md:grid-cols-2">
        <div className="rounded-xl border bg-white p-5 shadow-sm">
          <h3 className="mb-3 font-semibold">Patient Info</h3>
          <dl className="space-y-2 text-sm">
            <div className="flex justify-between"><dt className="text-slate-500">Name</dt><dd>{patient.fullName}</dd></div>
            <div className="flex justify-between"><dt className="text-slate-500">DOB</dt><dd>{patient.dateOfBirth.slice(0, 10)}</dd></div>
            <div className="flex justify-between"><dt className="text-slate-500">Gender</dt><dd>{parseGender(patient.gender)}</dd></div>
            <div className="flex justify-between"><dt className="text-slate-500">Blood Type</dt><dd>{patient.bloodType ?? '—'}</dd></div>
          </dl>
        </div>
        <div className="rounded-xl border bg-white p-5 shadow-sm">
          <h3 className="mb-3 font-semibold">Appointment</h3>
          <dl className="space-y-2 text-sm">
            <div className="flex justify-between"><dt className="text-slate-500">Date</dt><dd>{formatDate(appointment.appointmentDate)}</dd></div>
            <div className="flex justify-between"><dt className="text-slate-500">Time</dt><dd>{formatTime(appointment.startTime)}</dd></div>
          </dl>
        </div>
      </div>

      <div className="mb-6 rounded-xl border bg-white p-5 shadow-sm">
        <h3 className="mb-3 font-semibold">Medical History</h3>
        {historyLoading ? (
          <div className="space-y-3">{[...Array(2)].map((_, i) => <div key={i} className="h-16 animate-pulse rounded-lg bg-slate-100" />)}</div>
        ) : medicalHistory.length === 0 ? (
          <p className="text-sm text-slate-500">No previous completed visits on record.</p>
        ) : (
          <div className="space-y-4">
            {medicalHistory.map((entry) => (
              <div key={entry.appointmentId} className="rounded-lg border border-slate-100 bg-slate-50 p-4">
                <div className="mb-2 flex flex-wrap items-center justify-between gap-2 text-sm">
                  <span className="font-medium text-slate-900">
                    {formatDate(entry.appointmentDate)} · {formatTime(entry.startTime)}
                  </span>
                  <span className="text-slate-500">{entry.doctorName}</span>
                </div>
                {entry.diagnosis && (
                  <p className="text-sm text-slate-700"><span className="font-medium">Diagnosis:</span> {entry.diagnosis}</p>
                )}
                {entry.notes && (
                  <p className="mt-1 text-sm text-slate-600"><span className="font-medium">Notes:</span> {entry.notes}</p>
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
              </div>
            ))}
          </div>
        )}
      </div>

      {!inProgress && !isCompleted && (
        <div className="mb-6">
          <button
            type="button"
            onClick={() => startMutation.mutate(undefined)}
            disabled={startMutation.isPending || !canStart}
            className="rounded-lg bg-orange-600 px-4 py-2 text-sm text-white disabled:cursor-not-allowed disabled:opacity-50"
          >
            Start Consultation
          </button>
          {!canStart && (
            <p className="mt-2 text-sm text-amber-700">
              Consultation can start at {formatTime(appointment.startTime)} on {formatDate(appointment.appointmentDate)}.
            </p>
          )}
        </div>
      )}

      {(inProgress || isCompleted) && (
        <div className="space-y-6">
          <div className="rounded-xl border bg-white p-5 shadow-sm">
            <h3 className="mb-3 font-semibold">Diagnosis <span className="text-red-500">*</span></h3>
            <textarea value={diagnosis || consultation?.diagnosis || ''} onChange={(e) => setDiagnosis(e.target.value)} disabled={isCompleted} rows={3} className="w-full rounded-lg border px-3 py-2 text-sm" placeholder="Enter diagnosis..." required />
            <textarea value={notes} onChange={(e) => setNotes(e.target.value)} disabled={isCompleted} rows={2} className="mt-2 w-full rounded-lg border px-3 py-2 text-sm" placeholder="Notes (optional)" />
            {!isCompleted && (
              <button
                type="button"
                onClick={handleSaveDiagnosis}
                disabled={diagnosisMutation.isPending || !(diagnosis || consultation?.diagnosis)?.trim()}
                className="mt-3 rounded-lg bg-primary-600 px-4 py-2 text-sm text-white disabled:cursor-not-allowed disabled:opacity-50"
              >
                Save Diagnosis
              </button>
            )}
          </div>

          <div className="rounded-xl border bg-white p-5 shadow-sm">
            <h3 className="mb-3 font-semibold">Prescriptions</h3>
            {consultation?.prescriptions?.map((p) => (
              <div key={p.id} className="mb-2 rounded-lg bg-slate-50 p-3 text-sm">
                <strong>{p.medicationName}</strong> — {p.dosage}, {p.frequency}, {p.duration}
              </div>
            ))}
            {!isCompleted && (
              <div className="mt-4 grid grid-cols-2 gap-3">
                <input value={medication} onChange={(e) => setMedication(e.target.value)} placeholder="Medication" className="rounded-lg border px-3 py-2 text-sm" />
                <input value={dosage} onChange={(e) => setDosage(e.target.value)} placeholder="Dosage" className="rounded-lg border px-3 py-2 text-sm" />
                <input value={frequency} onChange={(e) => setFrequency(e.target.value)} placeholder="Frequency" className="rounded-lg border px-3 py-2 text-sm" />
                <input value={duration} onChange={(e) => setDuration(e.target.value)} placeholder="Duration" className="rounded-lg border px-3 py-2 text-sm" />
                <button
                  type="button"
                  onClick={handleAddPrescription}
                  disabled={prescriptionMutation.isPending || !medication.trim() || !dosage.trim() || !frequency.trim() || !duration.trim()}
                  className="col-span-2 rounded-lg border border-primary-600 px-4 py-2 text-sm text-primary-600 disabled:cursor-not-allowed disabled:opacity-50"
                >
                  Add Prescription
                </button>
              </div>
            )}
          </div>

          {!isCompleted && (
            <div>
              <button
                type="button"
                onClick={() => completeMutation.mutate(undefined)}
                disabled={completeMutation.isPending || !canComplete}
                className="rounded-lg bg-green-600 px-4 py-2 text-sm text-white disabled:cursor-not-allowed disabled:opacity-50"
              >
                Complete Appointment
              </button>
              {!canComplete && (
                <p className="mt-2 text-sm text-amber-700">
                  Save a diagnosis before completing the appointment.
                </p>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
