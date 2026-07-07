import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { departmentApi, doctorApi, appointmentApi, patientApi } from '../../api';
import { useApiMutation } from '../../hooks/useApiMutation';
import { formatTime } from '../../utils/format';

export function BookAppointmentPage() {
  const [step, setStep] = useState(1);
  const [departmentId, setDepartmentId] = useState('');
  const [doctorId, setDoctorId] = useState('');
  const [date, setDate] = useState('');
  const [patientId, setPatientId] = useState('');
  const [selectedSlot, setSelectedSlot] = useState('');
  const [notes, setNotes] = useState('');

  const { data: departments = [] } = useQuery({ queryKey: ['departments', 'all'], queryFn: departmentApi.getAllItems });
  const { data: doctors = [] } = useQuery({
    queryKey: ['doctors', departmentId],
    queryFn: () => doctorApi.getByDepartment(departmentId),
    enabled: !!departmentId,
  });
  const { data: patients = [] } = useQuery({ queryKey: ['patients', 'all'], queryFn: () => patientApi.searchAll() });
  const { data: slots = [], isLoading: slotsLoading } = useQuery({
    queryKey: ['slots', doctorId, date, patientId],
    queryFn: () => appointmentApi.getAvailableSlots(doctorId, date, patientId),
    enabled: !!doctorId && !!date && !!patientId,
  });

  const bookMutation = useApiMutation({
    mutationFn: () =>
      appointmentApi.book({
        patientId,
        doctorId,
        appointmentDate: date,
        startTime: `${selectedSlot}:00`,
        notes: notes || undefined,
      }),
    successMessage: 'Appointment booked!',
    onSuccess: () => {
      setStep(1);
      setSelectedSlot('');
    },
  });

  return (
    <div className="max-w-3xl">
      <h2 className="mb-6 text-2xl font-bold">Book Appointment</h2>
      <div className="mb-6 flex gap-2">
        {[1, 2, 3].map((s) => (
          <div key={s} className={`flex-1 rounded-lg py-2 text-center text-sm font-medium ${step >= s ? 'bg-primary-600 text-white' : 'bg-slate-200 text-slate-600'}`}>
            Step {s}
          </div>
        ))}
      </div>

      {step === 1 && (
        <div className="space-y-4 rounded-xl border bg-white p-6 shadow-sm">
          <div><label className="text-sm font-medium">Patient</label><select value={patientId} onChange={(e) => setPatientId(e.target.value)} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm"><option value="">Select patient...</option>{patients.map((p) => <option key={p.id} value={p.id}>{p.fullName}</option>)}</select></div>
          <div><label className="text-sm font-medium">Department</label><select value={departmentId} onChange={(e) => { setDepartmentId(e.target.value); setDoctorId(''); }} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm"><option value="">Select...</option>{departments.map((d) => <option key={d.id} value={d.id}>{d.name}</option>)}</select></div>
          <div><label className="text-sm font-medium">Doctor</label><select value={doctorId} onChange={(e) => setDoctorId(e.target.value)} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" disabled={!departmentId}><option value="">Select...</option>{doctors.map((d) => <option key={d.id} value={d.id}>{d.fullName} — {d.specialization}</option>)}</select></div>
          <div><label className="text-sm font-medium">Date</label><input type="date" value={date} min={new Date().toISOString().slice(0, 10)} onChange={(e) => setDate(e.target.value)} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" /></div>
          <button type="button" disabled={!patientId || !doctorId || !date} onClick={() => setStep(2)} className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white disabled:opacity-50">Next</button>
        </div>
      )}

      {step === 2 && (
        <div className="rounded-xl border bg-white p-6 shadow-sm">
          <h3 className="mb-1 font-semibold">Available Time Slots</h3>
          <p className="mb-4 text-xs text-slate-500">
            Times when this patient is already booked (with any doctor) are hidden.
          </p>
          {slotsLoading ? <p className="text-sm text-slate-500">Loading slots...</p> : (
            <div className="grid grid-cols-3 gap-3 sm:grid-cols-4">
              {slots.map((slot) => {
                const time = formatTime(slot.startTime);
                return (
                  <button key={time} type="button" onClick={() => { setSelectedSlot(time); setStep(3); }} className="rounded-lg border-2 border-slate-200 py-3 text-sm font-medium hover:border-primary-500 hover:bg-primary-50">
                    {time}
                  </button>
                );
              })}
              {slots.length === 0 && <p className="col-span-full text-sm text-slate-500">No slots available for this date.</p>}
            </div>
          )}
          <button type="button" onClick={() => setStep(1)} className="mt-4 text-sm text-slate-600 hover:underline">← Back</button>
        </div>
      )}

      {step === 3 && (
        <div className="space-y-4 rounded-xl border bg-white p-6 shadow-sm">
          <p className="text-sm"><strong>Time:</strong> {selectedSlot}</p>
          <p className="text-sm"><strong>Date:</strong> {date}</p>
          <div><label className="text-sm font-medium">Notes (optional)</label><textarea value={notes} onChange={(e) => setNotes(e.target.value)} rows={3} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" /></div>
          <div className="flex gap-3">
            <button type="button" onClick={() => setStep(2)} className="rounded-lg border px-4 py-2 text-sm">Back</button>
            <button type="button" onClick={() => bookMutation.mutate(undefined)} disabled={bookMutation.isPending} className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white">{bookMutation.isPending ? 'Booking...' : 'Confirm Booking'}</button>
          </div>
        </div>
      )}
    </div>
  );
}
