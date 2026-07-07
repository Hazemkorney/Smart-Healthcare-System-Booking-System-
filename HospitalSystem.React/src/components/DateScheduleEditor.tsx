import type { ReactNode } from 'react';
import { Trash2 } from 'lucide-react';
import { formatDate, formatTime, todayIso } from '../utils/format';
import type { DoctorDateSchedule } from '../types';

export { validateDateScheduleInput } from '../utils/format';

export function monthRangeFrom(dateIso: string): { from: string; to: string } {
  const d = new Date(`${dateIso}T00:00:00`);
  const year = d.getFullYear();
  const month = d.getMonth();
  const from = new Date(year, month, 1);
  const to = new Date(year, month + 1, 0);
  const fmt = (x: Date) =>
    `${x.getFullYear()}-${String(x.getMonth() + 1).padStart(2, '0')}-${String(x.getDate()).padStart(2, '0')}`;
  return { from: fmt(from), to: fmt(to) };
}

interface DateScheduleEditorProps {
  selectedDate: string;
  onDateChange: (date: string) => void;
  startTime: string;
  endTime: string;
  duration: number;
  onStartTimeChange: (value: string) => void;
  onEndTimeChange: (value: string) => void;
  onDurationChange: (value: number) => void;
  scheduledDates: DoctorDateSchedule[];
  onSelectScheduledDate: (date: string) => void;
  onSave: () => void;
  onRemoveDate?: (date: string) => void;
  saving?: boolean;
  saveLabel?: string;
  formTitle?: string;
  listTitle?: string;
  emptyListMessage?: string;
  footer?: ReactNode;
}

export function DateScheduleEditor({
  selectedDate,
  onDateChange,
  startTime,
  endTime,
  duration,
  onStartTimeChange,
  onEndTimeChange,
  onDurationChange,
  scheduledDates,
  onSelectScheduledDate,
  onSave,
  onRemoveDate,
  saving,
  saveLabel = 'Save Hours',
  formTitle = 'Set working hours for a date',
  listTitle = 'Configured dates',
  emptyListMessage = 'No dates configured yet. Pick a date above and save working hours.',
  footer,
}: DateScheduleEditorProps) {
  return (
    <div className="space-y-6">
      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <h3 className="mb-4 text-lg font-semibold">{formTitle}</h3>
        {selectedDate === todayIso() && (
          <p className="mb-4 text-xs text-amber-700">
            For today, start and end times must be later than the current time.
          </p>
        )}
        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">Date</label>
            <input
              type="date"
              value={selectedDate}
              min={todayIso()}
              onChange={(e) => onDateChange(e.target.value)}
              className="w-full rounded-lg border px-3 py-2 text-sm"
            />
          </div>
          <div className="flex flex-wrap items-end gap-3">
            <div>
              <label className="mb-1 block text-sm font-medium text-slate-700">From</label>
              <input
                type="time"
                value={startTime}
                onChange={(e) => onStartTimeChange(e.target.value)}
                className="rounded-lg border px-3 py-2 text-sm"
              />
            </div>
            <div>
              <label className="mb-1 block text-sm font-medium text-slate-700">To</label>
              <input
                type="time"
                value={endTime}
                onChange={(e) => onEndTimeChange(e.target.value)}
                className="rounded-lg border px-3 py-2 text-sm"
              />
            </div>
            <div>
              <label className="mb-1 block text-sm font-medium text-slate-700">Slot</label>
              <select
                value={duration}
                onChange={(e) => onDurationChange(Number(e.target.value))}
                className="rounded-lg border px-3 py-2 text-sm"
              >
                {[15, 30, 45, 60].map((m) => (
                  <option key={m} value={m}>
                    {m} min
                  </option>
                ))}
              </select>
            </div>
          </div>
        </div>
        <div className="mt-6 flex flex-wrap gap-3">
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

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <h3 className="mb-4 text-lg font-semibold">{listTitle}</h3>
        {scheduledDates.length === 0 ? (
          <p className="text-sm text-slate-500">{emptyListMessage}</p>
        ) : (
          <ul className="divide-y">
            {scheduledDates.map((s) => (
              <li key={s.id} className="flex items-center justify-between gap-3 py-3 text-sm">
                <div className="flex min-w-0 flex-1 flex-wrap items-center gap-x-3 gap-y-1">
                  <button
                    type="button"
                    onClick={() => onSelectScheduledDate(s.date)}
                    className={`font-medium ${s.date === selectedDate ? 'text-primary-600' : 'text-slate-800 hover:text-primary-600'}`}
                  >
                    {formatDate(s.date)}
                  </button>
                  <span className="text-slate-600">
                    {formatTime(s.startTime)} – {formatTime(s.endTime)} ({s.appointmentDurationMinutes} min)
                  </span>
                </div>
                {onRemoveDate && (
                  <button
                    type="button"
                    onClick={() => onRemoveDate(s.date)}
                    disabled={saving}
                    title="Remove this date"
                    aria-label={`Remove ${formatDate(s.date)}`}
                    className="shrink-0 rounded p-1 text-red-600 hover:bg-red-50 disabled:opacity-50"
                  >
                    <Trash2 className="h-4 w-4" />
                  </button>
                )}
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
