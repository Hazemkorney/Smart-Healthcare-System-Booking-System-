import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { doctorApi } from '../../api';
import { useApiMutation } from '../../hooks/useApiMutation';
import { dayNames } from '../../utils/format';

const weekdays = [1, 2, 3, 4, 5]; // Mon-Fri

interface DayConfig {
  enabled: boolean;
  startTime: string;
  endTime: string;
  duration: number;
}

const defaultDay = (): DayConfig => ({
  enabled: false,
  startTime: '09:00',
  endTime: '17:00',
  duration: 30,
});

export function DoctorSchedulePage() {
  const { id } = useParams<{ id: string }>();
  const [days, setDays] = useState<Record<number, DayConfig>>(() =>
    Object.fromEntries(weekdays.map((d) => [d, defaultDay()])),
  );

  const { data: doctor } = useQuery({
    queryKey: ['doctors', id],
    queryFn: () => doctorApi.getAllItems().then((list) => list.find((d) => d.id === id)),
    enabled: !!id,
  });

  useQuery({
    queryKey: ['doctor-schedule', id],
    queryFn: async () => {
      const schedules = await doctorApi.getSchedule(id!);
      const next = { ...days };
      schedules.forEach((s) => {
        next[s.dayOfWeek] = {
          enabled: true,
          startTime: s.startTime.slice(0, 5),
          endTime: s.endTime.slice(0, 5),
          duration: s.appointmentDurationMinutes,
        };
      });
      setDays(next);
      return schedules;
    },
    enabled: !!id,
  });

  const saveMutation = useApiMutation({
    mutationFn: () =>
      doctorApi.setSchedule(
        id!,
        weekdays
          .filter((d) => days[d]?.enabled)
          .map((d) => ({
            dayOfWeek: d,
            startTime: `${days[d].startTime}:00`,
            endTime: `${days[d].endTime}:00`,
            appointmentDurationMinutes: days[d].duration,
          })),
      ),
    successMessage: 'Schedule saved',
    invalidateKeys: [['doctor-schedule', id!]],
  });

  return (
    <div>
      <h2 className="mb-2 text-2xl font-bold">Schedule — {doctor?.fullName ?? 'Doctor'}</h2>
      <p className="mb-6 text-sm text-slate-500">Configure weekly working hours and slot duration.</p>

      <div className="space-y-4 rounded-xl border bg-white p-6 shadow-sm">
        {weekdays.map((day) => (
          <div key={day} className="flex flex-wrap items-center gap-4 border-b pb-4 last:border-0">
            <label className="flex w-32 items-center gap-2 text-sm font-medium">
              <input
                type="checkbox"
                checked={days[day]?.enabled ?? false}
                onChange={(e) => setDays({ ...days, [day]: { ...days[day], enabled: e.target.checked } })}
              />
              {dayNames[day]}
            </label>
            {days[day]?.enabled && (
              <>
                <input type="time" value={days[day].startTime} onChange={(e) => setDays({ ...days, [day]: { ...days[day], startTime: e.target.value } })} className="rounded border px-2 py-1 text-sm" />
                <span className="text-slate-400">to</span>
                <input type="time" value={days[day].endTime} onChange={(e) => setDays({ ...days, [day]: { ...days[day], endTime: e.target.value } })} className="rounded border px-2 py-1 text-sm" />
                <select value={days[day].duration} onChange={(e) => setDays({ ...days, [day]: { ...days[day], duration: Number(e.target.value) } })} className="rounded border px-2 py-1 text-sm">
                  {[15, 30, 45, 60].map((m) => <option key={m} value={m}>{m} min slots</option>)}
                </select>
              </>
            )}
          </div>
        ))}
        <button type="button" onClick={() => saveMutation.mutate(undefined)} disabled={saveMutation.isPending} className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white">
          {saveMutation.isPending ? 'Saving...' : 'Save Schedule'}
        </button>
      </div>
    </div>
  );
}
