import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { doctorPortalApi } from '../../api';
import { getApiError } from '../../api/client';
import { DataTable, type Column } from '../../components/DataTable';
import { formatDate, formatTime } from '../../utils/format';
import type { DoctorDateSchedule } from '../../types';

export function DoctorWorkingHoursPage() {
  const [sortKey, setSortKey] = useState('date');
  const [sortDir, setSortDir] = useState<'asc' | 'desc'>('asc');

  const { data: schedules = [], isLoading, isError, error } = useQuery({
    queryKey: ['doctor-date-schedules'],
    queryFn: () => doctorPortalApi.getDateSchedules(),
    retry: false,
  });
  const sorted = useMemo(() => {
    const copy = [...schedules];
    copy.sort((a, b) => {
      let cmp = 0;
      if (sortKey === 'date') cmp = a.date.localeCompare(b.date);
      else if (sortKey === 'start') cmp = a.startTime.localeCompare(b.startTime);
      else if (sortKey === 'end') cmp = a.endTime.localeCompare(b.endTime);
      else if (sortKey === 'slot') cmp = a.appointmentDurationMinutes - b.appointmentDurationMinutes;
      return sortDir === 'asc' ? cmp : -cmp;
    });
    return copy;
  }, [schedules, sortKey, sortDir]);

  const columns: Column<DoctorDateSchedule>[] = useMemo(
    () => [
      {
        key: 'date',
        header: 'Date',
        sortValue: (r) => r.date,
        render: (r) => (
          <Link to={`/doctor/schedule?date=${r.date}`} className="font-medium text-primary-600 hover:underline">
            {formatDate(r.date)}
          </Link>
        ),
      },
      {
        key: 'start',
        header: 'From',
        sortValue: (r) => r.startTime,
        render: (r) => formatTime(r.startTime),
      },
      {
        key: 'end',
        header: 'To',
        sortValue: (r) => r.endTime,
        render: (r) => formatTime(r.endTime),
      },
      {
        key: 'slot',
        header: 'Slot',
        sortValue: (r) => r.appointmentDurationMinutes,
        render: (r) => `${r.appointmentDurationMinutes} min`,
      },
    ],
    [],
  );

  const handleSort = (key: string) => {
    if (sortKey === key) setSortDir((d) => (d === 'asc' ? 'desc' : 'asc'));
    else {
      setSortKey(key);
      setSortDir('asc');
    }
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="mb-2 text-2xl font-bold">Working Hours</h2>
        <p className="text-sm text-slate-500">
          All dates and hours configured for you by the admin. Click a date to view appointments on that day.
        </p>
      </div>

      {isError ? (
        <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800">
          {getApiError(error)}
        </div>
      ) : (
        <DataTable
          columns={columns}
          data={sorted}
          loading={isLoading}
          emptyMessage="No working hours configured yet. Ask the admin to apply your schedule from Doctor Schedule."
          sortKey={sortKey}
          sortDir={sortDir}
          onSort={handleSort}
        />
      )}
    </div>
  );
}