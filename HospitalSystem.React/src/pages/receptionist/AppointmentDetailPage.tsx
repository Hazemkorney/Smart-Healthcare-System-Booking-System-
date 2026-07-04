import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { appointmentApi } from '../../api';
import { StatusBadge } from '../../components/StatusBadge';
import { ConfirmDialog } from '../../components/ConfirmDialog';
import { Modal } from '../../components/Modal';
import { useApiMutation } from '../../hooks/useApiMutation';
import { formatDate, formatTime, todayIso } from '../../utils/format';

export function AppointmentDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [showCancel, setShowCancel] = useState(false);
  const [showReschedule, setShowReschedule] = useState(false);
  const [newDate, setNewDate] = useState('');
  const [selectedSlot, setSelectedSlot] = useState('');

  const { data: appointment, isLoading, refetch } = useQuery({
    queryKey: ['appointment', id],
    queryFn: () => appointmentApi.getById(id!),
    enabled: !!id,
  });

  const { data: slots = [], isLoading: slotsLoading } = useQuery({
    queryKey: ['reschedule-slots', appointment?.doctorId, appointment?.patientId, newDate, id],
    queryFn: () =>
      appointmentApi.getAvailableSlots(appointment!.doctorId, newDate, appointment!.patientId, id),
    enabled: showReschedule && !!appointment?.doctorId && !!appointment?.patientId && !!newDate && !!id,
  });

  const cancelMutation = useApiMutation({
    mutationFn: () => appointmentApi.cancel(id!),
    successMessage: 'Appointment cancelled',
    onSuccess: () => { setShowCancel(false); refetch(); },
  });

  const rescheduleMutation = useApiMutation({
    mutationFn: () => appointmentApi.reschedule(id!, { newDate, newStartTime: `${selectedSlot}:00` }),
    successMessage: 'Appointment rescheduled',
    onSuccess: () => {
      setShowReschedule(false);
      setNewDate('');
      setSelectedSlot('');
      refetch();
    },
  });

  const openReschedule = () => {
    setNewDate('');
    setSelectedSlot('');
    setShowReschedule(true);
  };

  const closeReschedule = () => {
    setShowReschedule(false);
    setNewDate('');
    setSelectedSlot('');
  };

  if (isLoading) return <div className="h-40 animate-pulse rounded-xl bg-slate-100" />;
  if (!appointment) return <p>Not found</p>;

  const canReschedule = appointment.status !== 'Cancelled' && appointment.status !== 4
    && appointment.status !== 'Completed' && appointment.status !== 3;

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
        {canReschedule && (
          <div className="mt-6 flex gap-3">
            <button type="button" onClick={openReschedule} className="rounded-lg border px-4 py-2 text-sm">Reschedule</button>
            <button type="button" onClick={() => setShowCancel(true)} className="rounded-lg bg-red-600 px-4 py-2 text-sm text-white">Cancel</button>
          </div>
        )}
      </div>

      <ConfirmDialog open={showCancel} title="Cancel Appointment" message="Are you sure you want to cancel this appointment?" onCancel={() => setShowCancel(false)} onConfirm={() => cancelMutation.mutate(undefined)} loading={cancelMutation.isPending} />

      <Modal open={showReschedule} onClose={closeReschedule} title="Reschedule Appointment">
        <div className="space-y-4">
          <p className="text-sm text-slate-600">Doctor: <strong>{appointment.doctorName}</strong></p>
          <div>
            <label className="text-sm font-medium">New Date</label>
            <input
              type="date"
              value={newDate}
              min={todayIso()}
              onChange={(e) => { setNewDate(e.target.value); setSelectedSlot(''); }}
              className="mt-1 w-full rounded-lg border px-3 py-2 text-sm"
            />
          </div>
          {newDate && (
            <div>
              <label className="text-sm font-medium">Available Time Slots</label>
              {slotsLoading ? (
                <p className="mt-2 text-sm text-slate-500">Loading slots...</p>
              ) : (
                <div className="mt-2 grid grid-cols-3 gap-2 sm:grid-cols-4">
                  {slots.map((slot) => {
                    const time = formatTime(slot.startTime);
                    const isSelected = selectedSlot === time;
                    return (
                      <button
                        key={time}
                        type="button"
                        onClick={() => setSelectedSlot(time)}
                        className={`rounded-lg border-2 py-2 text-sm font-medium ${
                          isSelected
                            ? 'border-primary-600 bg-primary-50 text-primary-700'
                            : 'border-slate-200 hover:border-primary-500 hover:bg-primary-50'
                        }`}
                      >
                        {time}
                      </button>
                    );
                  })}
                  {slots.length === 0 && (
                    <p className="col-span-full text-sm text-slate-500">No slots available for this date.</p>
                  )}
                </div>
              )}
            </div>
          )}
          <button
            type="button"
            onClick={() => rescheduleMutation.mutate(undefined)}
            disabled={!newDate || !selectedSlot || rescheduleMutation.isPending}
            className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white disabled:cursor-not-allowed disabled:opacity-50"
          >
            {rescheduleMutation.isPending ? 'Saving...' : 'Save'}
          </button>
        </div>
      </Modal>
    </div>
  );
}
