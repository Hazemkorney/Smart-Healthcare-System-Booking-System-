import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { doctorApi } from '../../api';
import { useApiMutation } from '../../hooks/useApiMutation';
import { DateScheduleEditor, validateDateScheduleInput } from '../../components/DateScheduleEditor';
import { todayIso } from '../../utils/format';

export function GlobalDoctorSchedulePage() {
  const [selectedDate, setSelectedDate] = useState(todayIso());
  const [startTime, setStartTime] = useState('09:00');
  const [endTime, setEndTime] = useState('17:00');
  const [duration, setDuration] = useState(30);

  const [applyDate, setApplyDate] = useState(todayIso());
  const [applyStartTime, setApplyStartTime] = useState('09:00');
  const [applyEndTime, setApplyEndTime] = useState('17:00');
  const [applyDuration, setApplyDuration] = useState(30);

  const { data: scheduledDates = [], isLoading, refetch } = useQuery({
    queryKey: ['doctor-default-date-schedules'],
    queryFn: () => doctorApi.getDefaultDateSchedules(),
  });

  const {
    data: appliedDates = [],
    isLoading: appliedLoading,
    refetch: refetchApplied,
  } = useQuery({
    queryKey: ['doctor-applied-date-schedules'],
    queryFn: () => doctorApi.getAppliedDateSchedules(),
  });

  const loadDefaultDate = (date: string) => {
    setSelectedDate(date);
    const existing = scheduledDates.find((s) => s.date === date);
    if (existing) {
      setStartTime(existing.startTime.slice(0, 5));
      setEndTime(existing.endTime.slice(0, 5));
      setDuration(existing.appointmentDurationMinutes);
    }
  };

  const loadAppliedDate = (date: string) => {
    setApplyDate(date);
    const existing = appliedDates.find((s) => s.date === date);
    if (existing) {
      setApplyStartTime(existing.startTime.slice(0, 5));
      setApplyEndTime(existing.endTime.slice(0, 5));
      setApplyDuration(existing.appointmentDurationMinutes);
    }
  };

  const saveMutation = useApiMutation({
    mutationFn: () =>
      doctorApi.setDefaultDateSchedule({
        date: selectedDate,
        startTime: `${startTime}:00`,
        endTime: `${endTime}:00`,
        appointmentDurationMinutes: duration,
      }),
    successMessage: 'Working hours saved for this date',
    onSuccess: () => refetch(),
  });

  const removeMutation = useApiMutation({
    mutationFn: (date: string) => doctorApi.removeDefaultDateSchedule(date),
    successMessage: 'Date removed',
    onSuccess: () => refetch(),
  });

  const applyMutation = useApiMutation({
    mutationFn: () =>
      doctorApi.applyDateScheduleToAll({
        date: applyDate,
        startTime: `${applyStartTime}:00`,
        endTime: `${applyEndTime}:00`,
        appointmentDurationMinutes: applyDuration,
      }),
    successMessage: 'Working hours applied to all doctors',
    onSuccess: () => refetchApplied(),
  });

  const removeAppliedMutation = useApiMutation({
    mutationFn: (date: string) => doctorApi.removeAppliedDateSchedule(date),
    successMessage: 'Date removed from all doctors',
    onSuccess: () => refetchApplied(),
  });

  if (isLoading || appliedLoading) {
    return <div className="h-48 animate-pulse rounded-xl bg-slate-100" />;
  }

  return (
    <div className="space-y-10">
      <div>
        <h2 className="mb-2 text-2xl font-bold">Doctor Schedule</h2>
        <p className="mb-6 text-sm text-slate-500">
          Configure default working hours by date, then apply specific dates to all doctors in the section below.
        </p>

        <DateScheduleEditor
          selectedDate={selectedDate}
          onDateChange={(date) => {
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
          }}
          startTime={startTime}
          endTime={endTime}
          duration={duration}
          onStartTimeChange={setStartTime}
          onEndTimeChange={setEndTime}
          onDurationChange={setDuration}
          scheduledDates={scheduledDates}
          onSelectScheduledDate={loadDefaultDate}
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
          saveLabel="Save Default Hours"
        />
      </div>

      <div>
        <h3 className="mb-4 text-xl font-bold">Apply to all doctors</h3>
        <DateScheduleEditor
          selectedDate={applyDate}
          onDateChange={(date) => {
            setApplyDate(date);
            const existing = appliedDates.find((s) => s.date === date);
            if (existing) {
              setApplyStartTime(existing.startTime.slice(0, 5));
              setApplyEndTime(existing.endTime.slice(0, 5));
              setApplyDuration(existing.appointmentDurationMinutes);
            } else {
              setApplyStartTime('09:00');
              setApplyEndTime('17:00');
              setApplyDuration(30);
            }
          }}
          startTime={applyStartTime}
          endTime={applyEndTime}
          duration={applyDuration}
          onStartTimeChange={setApplyStartTime}
          onEndTimeChange={setApplyEndTime}
          onDurationChange={setApplyDuration}
          scheduledDates={appliedDates}
          onSelectScheduledDate={loadAppliedDate}
          onSave={() => {
            const error = validateDateScheduleInput(applyDate, applyStartTime, applyEndTime);
            if (error) {
              toast.error(error);
              return;
            }
            applyMutation.mutate(undefined);
          }}
          onRemoveDate={(date) => removeAppliedMutation.mutate(date)}
          saving={applyMutation.isPending || removeAppliedMutation.isPending}
          formTitle="Apply working hours to all doctors"
          listTitle="Applied dates"
          emptyListMessage="No dates applied yet. Pick a date above and apply working hours to all doctors."
          saveLabel="Apply to all doctors"
        />
      </div>
    </div>
  );
}
