import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { appointmentApi } from '../../api';
import { StatusBadge } from '../../components/StatusBadge';
import { ConfirmDialog } from '../../components/ConfirmDialog';
import { Modal } from '../../components/Modal';
import { useApiMutation } from '../../hooks/useApiMutation';
import { formatDate, formatTime } from '../../utils/format';

export function AppointmentDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [showCancel, setShowCancel] = useState(false);
  const [showReschedule, setShowReschedule] = useState(false);
  const [newDate, setNewDate] = useState('');
  const [newTime, setNewTime] = useState('');

  const { data: appointment, isLoading, refetch } = useQuery({
    queryKey: ['appointment', id],
    queryFn: () => appointmentApi.getById(id!),
    enabled: !!id,
  });

  const cancelMutation = useApiMutation({
    mutationFn: () => appointmentApi.cancel(id!),
    successMessage: 'Appointment cancelled',
    onSuccess: () => { setShowCancel(false); refetch(); },
  });

  const rescheduleMutation = useApiMutation({
    mutationFn: () => appointmentApi.reschedule(id!, { newDate, newStartTime: `${newTime}:00` }),
    successMessage: 'Appointment rescheduled',
    onSuccess: () => { setShowReschedule(false); refetch(); },
  });

  if (isLoading) return <div className="h-40 animate-pulse rounded-xl bg-slate-100" />;
  if (!appointment) return <p>Not found</p>;

  return (
    <div className="max-w-xl">
      <button type="button" onClick={() => navigate(-1)} className="mb-4 text-sm text-slate-600 hover:underline">← Back</button>
      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <div className="mb-4 flex items-center justify-between">
          <h2 className="text-xl font-bold">Appointment Details</h2>
          <StatusBadge status={appointment.status} />
        </div>
        <dl className="space-y-2 text-sm">
          <div className="flex justify-between"><dt className="text-slate-500">Patient</dt><dd>{appointment.patientName}</dd></div>
          <div className="flex justify-between"><dt className="text-slate-500">Doctor</dt><dd>{appointment.doctorName}</dd></div>
          <div className="flex justify-between"><dt className="text-slate-500">Date</dt><dd>{formatDate(appointment.appointmentDate)}</dd></div>
          <div className="flex justify-between"><dt className="text-slate-500">Time</dt><dd>{formatTime(appointment.startTime)} – {formatTime(appointment.endTime)}</dd></div>
          {appointment.notes && <div><dt className="text-slate-500">Notes</dt><dd className="mt-1">{appointment.notes}</dd></div>}
        </dl>
        <div className="mt-6 flex gap-3">
          <button type="button" onClick={() => setShowReschedule(true)} className="rounded-lg border px-4 py-2 text-sm">Reschedule</button>
          <button type="button" onClick={() => setShowCancel(true)} className="rounded-lg bg-red-600 px-4 py-2 text-sm text-white">Cancel</button>
        </div>
      </div>

      <ConfirmDialog open={showCancel} title="Cancel Appointment" message="Are you sure you want to cancel this appointment?" onCancel={() => setShowCancel(false)} onConfirm={() => cancelMutation.mutate(undefined)} loading={cancelMutation.isPending} />

      <Modal open={showReschedule} onClose={() => setShowReschedule(false)} title="Reschedule Appointment">
        <div className="space-y-4">
          <div><label className="text-sm font-medium">New Date</label><input type="date" value={newDate} min={new Date().toISOString().slice(0, 10)} onChange={(e) => setNewDate(e.target.value)} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" /></div>
          <div><label className="text-sm font-medium">New Time</label><input type="time" value={newTime} onChange={(e) => setNewTime(e.target.value)} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" /></div>
          <button type="button" onClick={() => rescheduleMutation.mutate(undefined)} disabled={!newDate || !newTime} className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white">Save</button>
        </div>
      </Modal>
    </div>
  );
}
