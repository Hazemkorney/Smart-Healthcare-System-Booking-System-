import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { doctorApi } from '../../api';
import { useApiMutation } from '../../hooks/useApiMutation';
import { DateScheduleEditor, validateDateScheduleInput } from '../../components/DateScheduleEditor';
import { todayIso } from '../../utils/format';

export function DoctorSchedulePage() {
  const { id } = useParams<{ id: string }>();
  const [selectedDate, setSelectedDate] = useState(todayIso());
  const [startTime, setStartTime] = useState('09:00');
  const [endTime, setEndTime] = useState('17:00');
  const [duration, setDuration] = useState(30);

  const { data: doctor } = useQuery({
    queryKey: ['doctors', id],
    queryFn: () => doctorApi.getAllItems().then((list) => list.find((d) => d.id === id)),
    enabled: !!id,
  });

  const { data: scheduledDates = [], isLoading, refetch } = useQuery({
    queryKey: ['doctor-date-schedules', id],
    queryFn: () => doctorApi.getDateSchedules(id!),
    enabled: !!id,
  });

  useEffect(() => {
    const existing = scheduledDates.find((s) => s.date === selectedDate);
    if (existing) {
      setStartTime(existing.startTime.slice(0, 5));
      setEndTime(existing.endTime.slice(0, 5));
      setDuration(existing.appointmentDurationMinutes);
    }
  }, [scheduledDates, selectedDate]);

  const loadDate = (date: string) => {
    setSelectedDate(date);
    const existing = scheduledDates.find((s) => s.date === date);
    if (existing) {
      setStartTime(existing.startTime.slice(0, 5));
      setEndTime(existing.endTime.slice(0, 5));
      setDuration(existing.appointmentDurationMinutes);
    } else {
      setStartTime('09:00');
      setEndTime('17:00');
      setDuration(30);
    }
  };

  const saveMutation = useApiMutation({
    mutationFn: () =>
      doctorApi.setDateSchedule(id!, {
        date: selectedDate,
        startTime: `${startTime}:00`,
        endTime: `${endTime}:00`,
        appointmentDurationMinutes: duration,
      }),
    successMessage: 'Working hours saved',
    onSuccess: () => refetch(),
  });

  const removeMutation = useApiMutation({
    mutationFn: (date: string) => doctorApi.removeDateSchedule(id!, date),
    successMessage: 'Date removed',
    onSuccess: () => refetch(),
  });

  return (
    <div>
      <h2 className="mb-2 text-2xl font-bold">Schedule — {doctor?.fullName ?? 'Doctor'}</h2>
      <p className="mb-6 text-sm text-slate-500">
        Pick a <strong>date</strong> and set this doctor&apos;s working hours for that day only.
      </p>

      {isLoading ? (
        <div className="h-48 animate-pulse rounded-xl bg-slate-100" />
      ) : (
        <DateScheduleEditor
          selectedDate={selectedDate}
          onDateChange={loadDate}
          startTime={startTime}
          endTime={endTime}
          duration={duration}
          onStartTimeChange={setStartTime}
          onEndTimeChange={setEndTime}
          onDurationChange={setDuration}
          scheduledDates={scheduledDates}
          onSelectScheduledDate={loadDate}
          onSave={() => {
            const error = validateDateScheduleInput(selectedDate, startTime, endTime);
            if (error) {
              toast.error(error);
              return;
            }
            saveMutation.mutate(undefined);
          }}
          onRemoveDate={(date) => removeMutation.mutate(date)}
          saving={saveMutation.isPending || removeMutation.isPending}
        />
      )}
    </div>
  );
}
