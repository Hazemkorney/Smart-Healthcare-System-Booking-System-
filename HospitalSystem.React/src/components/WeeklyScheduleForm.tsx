import type { ReactNode } from 'react';
import { useEffect, useMemo, useState } from 'react';
import { dayNames } from '../utils/format';

/** All days Mon → Sun (JS getDay(): 0 = Sunday). */
export const scheduleDays = [1, 2, 3, 4, 5, 6, 0] as const;

/** @deprecated Use scheduleDays */
export const scheduleWeekdays = scheduleDays;

export interface DayConfig {
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

export function buildDefaultDays(): Record<number, DayConfig> {
  return Object.fromEntries(scheduleDays.map((d) => [d, defaultDay()]));
}

export function buildAllDaysEnabled(config?: Partial<Omit<DayConfig, 'enabled'>>): Record<number, DayConfig> {
  const base: DayConfig = { ...defaultDay(), enabled: true, ...config };
  return Object.fromEntries(scheduleDays.map((d) => [d, { ...base }]));
}

export function isEveryDaySchedule(days: Record<number, DayConfig>): boolean {
  const enabled = scheduleDays.filter((d) => days[d]?.enabled);
  if (enabled.length !== scheduleDays.length) return false;

  const first = days[enabled[0]!]!;
  return enabled.every(
    (d) =>
      days[d]!.startTime === first.startTime &&
      days[d]!.endTime === first.endTime &&
      days[d]!.duration === first.duration,
  );
}

export function applyEveryDayHours(
  _days: Record<number, DayConfig>,
  hours: Pick<DayConfig, 'startTime' | 'endTime' | 'duration'>,
): Record<number, DayConfig> {
  return buildAllDaysEnabled(hours);
}

interface WeeklyScheduleFormProps {
  days: Record<number, DayConfig>;
  onDaysChange: (days: Record<number, DayConfig>) => void;
  onSave: () => void;
  saving?: boolean;
  saveLabel?: string;
  footer?: ReactNode;
}

export function WeeklyScheduleForm({
  days,
  onDaysChange,
  onSave,
  saving,
  saveLabel = 'Save Schedule',
  footer,
}: WeeklyScheduleFormProps) {
  const everyDay = useMemo(() => isEveryDaySchedule(days), [days]);
  const [customizeByDay, setCustomizeByDay] = useState(false);

  useEffect(() => {
    if (!everyDay && scheduleDays.some((d) => days[d]?.enabled))
      setCustomizeByDay(true);
  }, [days, everyDay]);

  const everyDayHours = everyDay
    ? days[scheduleDays[0]!]!
    : days[scheduleDays.find((d) => days[d]?.enabled) ?? 1] ?? defaultDay();

  const setEveryDayMode = (enabled: boolean) => {
    if (enabled) {
      setCustomizeByDay(false);
      onDaysChange(buildAllDaysEnabled(everyDayHours));
    } else {
      setCustomizeByDay(true);
      onDaysChange(buildDefaultDays());
    }
  };

  const updateEveryDayHours = (patch: Partial<Pick<DayConfig, 'startTime' | 'endTime' | 'duration'>>) => {
    onDaysChange(applyEveryDayHours(days, { ...everyDayHours, ...patch }));
  };

  return (
    <div className="space-y-4 rounded-xl border bg-white p-6 shadow-sm">
      <div className="rounded-lg border border-primary-100 bg-primary-50/50 p-4">
        <label className="flex cursor-pointer items-center gap-2 text-sm font-semibold text-slate-900">
          <input
            type="checkbox"
            checked={everyDay}
            onChange={(e) => setEveryDayMode(e.target.checked)}
          />
          Available every day (same hours all week)
        </label>
        {everyDay && (
          <div className="mt-4 flex flex-wrap items-center gap-3 pl-6">
            <input
              type="time"
              value={everyDayHours.startTime}
              onChange={(e) => updateEveryDayHours({ startTime: e.target.value })}
              className="rounded border px-2 py-1 text-sm"
            />
            <span className="text-slate-400">to</span>
            <input
              type="time"
              value={everyDayHours.endTime}
              onChange={(e) => updateEveryDayHours({ endTime: e.target.value })}
              className="rounded border px-2 py-1 text-sm"
            />
            <select
              value={everyDayHours.duration}
              onChange={(e) => updateEveryDayHours({ duration: Number(e.target.value) })}
              className="rounded border px-2 py-1 text-sm"
            >
              {[15, 30, 45, 60].map((m) => (
                <option key={m} value={m}>
                  {m} min slots
                </option>
              ))}
            </select>
          </div>
        )}
      </div>

      {!everyDay && (
        <div className="flex flex-wrap items-center gap-3">
          <button
            type="button"
            onClick={() => onDaysChange(buildAllDaysEnabled(everyDayHours))}
            className="text-sm font-medium text-primary-600 hover:text-primary-700"
          >
            Enable all days with these hours
          </button>
          <button
            type="button"
            onClick={() => setCustomizeByDay((v) => !v)}
            className="text-sm text-slate-600 hover:text-slate-800"
          >
            {customizeByDay ? 'Hide day list' : 'Customize specific days'}
          </button>
        </div>
      )}

      {!everyDay && customizeByDay && (
        <div className="space-y-4 border-t pt-4">
          <p className="text-xs text-slate-500">Pick which days of the week this schedule applies to.</p>
          {scheduleDays.map((day) => (
            <div key={day} className="flex flex-wrap items-center gap-4 border-b pb-4 last:border-0">
              <label className="flex w-36 items-center gap-2 text-sm font-medium">
                <input
                  type="checkbox"
                  checked={days[day]?.enabled ?? false}
                  onChange={(e) =>
                    onDaysChange({
                      ...days,
                      [day]: { ...(days[day] ?? defaultDay()), enabled: e.target.checked },
                    })
                  }
                />
                {dayNames[day]}
              </label>
              {days[day]?.enabled && (
                <>
                  <input
                    type="time"
                    value={days[day].startTime}
                    onChange={(e) =>
                      onDaysChange({ ...days, [day]: { ...days[day], startTime: e.target.value } })
                    }
                    className="rounded border px-2 py-1 text-sm"
                  />
                  <span className="text-slate-400">to</span>
                  <input
                    type="time"
                    value={days[day].endTime}
                    onChange={(e) =>
                      onDaysChange({ ...days, [day]: { ...days[day], endTime: e.target.value } })
                    }
                    className="rounded border px-2 py-1 text-sm"
                  />
                  <select
                    value={days[day].duration}
                    onChange={(e) =>
                      onDaysChange({ ...days, [day]: { ...days[day], duration: Number(e.target.value) } })
                    }
                    className="rounded border px-2 py-1 text-sm"
                  >
                    {[15, 30, 45, 60].map((m) => (
                      <option key={m} value={m}>
                        {m} min slots
                      </option>
                    ))}
                  </select>
                </>
              )}
            </div>
          ))}
        </div>
      )}

      <div className="flex flex-wrap items-center gap-3 border-t pt-4">
        <button
          type="button"
          onClick={onSave}
          disabled={saving}
          className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white disabled:opacity-50"
        >
          {saving ? 'Saving...' : saveLabel}
        </button>
        {footer}
      </div>
    </div>
  );
}

export function schedulesToDays(
  schedules: Array<{
    dayOfWeek: number | string;
    startTime: string;
    endTime: string;
    appointmentDurationMinutes: number;
  }>,
  parseDayOfWeek: (value: number | string) => number,
): Record<number, DayConfig> {
  const next = buildDefaultDays();
  schedules.forEach((s) => {
    const day = parseDayOfWeek(s.dayOfWeek);
    next[day] = {
      enabled: true,
      startTime: s.startTime.slice(0, 5),
      endTime: s.endTime.slice(0, 5),
      duration: s.appointmentDurationMinutes,
    };
  });
  return next;
}

export function daysToSchedulePayload(days: Record<number, DayConfig>) {
  return scheduleDays
    .filter((d) => days[d]?.enabled)
    .map((d) => ({
      dayOfWeek: d,
      startTime: `${days[d].startTime}:00`,
      endTime: `${days[d].endTime}:00`,
      appointmentDurationMinutes: days[d].duration,
    }));
}
