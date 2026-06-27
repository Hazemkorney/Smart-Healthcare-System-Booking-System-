import { useMemo } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { doctorApi, appointmentApi } from '../../api';
import { DataTable, type Column } from '../../components/DataTable';
import { StatusBadge } from '../../components/StatusBadge';
import { useApiMutation } from '../../hooks/useApiMutation';
import { formatDate, formatTime, todayIso } from '../../utils/format';
import type { Appointment } from '../../types';

export function ReceptionistSchedulePage() {
  const today = todayIso();
  const { data: doctors = [] } = useQuery({ queryKey: ['doctors', 'all'], queryFn: doctorApi.getAllItems });

  const { data: appointments = [], isLoading, refetch } = useQuery({
    queryKey: ['today-schedule', today],
    queryFn: async () => {
      const results: Appointment[] = [];
      for (const doc of doctors) {
        const appts = await appointmentApi.getByDoctorAndDate(doc.id, today);
        results.push(...appts);
      }
      return results.sort((a, b) => a.startTime.localeCompare(b.startTime));
    },
    enabled: doctors.length > 0,
  });

  const checkInMutation = useApiMutation({
    mutationFn: appointmentApi.checkIn,
    successMessage: 'Patient checked in',
    onSuccess: () => refetch(),
  });

  const columns: Column<Appointment>[] = useMemo(
    () => [
      { key: 'time', header: 'Time', render: (r) => formatTime(r.startTime) },
      { key: 'doctor', header: 'Doctor', render: (r) => r.doctorName },
      { key: 'patient', header: 'Patient', render: (r) => r.patientName },
      { key: 'status', header: 'Status', render: (r) => <StatusBadge status={r.status} /> },
      {
        key: 'actions',
        header: 'Actions',
        render: (r) => (
          <div className="flex gap-2">
            <Link to={`/receptionist/appointments/${r.id}`} className="text-sm text-primary-600 hover:underline">View</Link>
            {(r.status === 'Confirmed' || r.status === 0) && (
              <button type="button" onClick={() => checkInMutation.mutate(r.id)} className="text-sm text-green-600 hover:underline">Check-In</button>
            )}
          </div>
        ),
      },
    ],
    [checkInMutation],
  );

  return (
    <div>
      <h2 className="mb-6 text-2xl font-bold">Today&apos;s Schedule</h2>
      <p className="mb-4 text-sm text-slate-500">{formatDate(today)}</p>
      <DataTable columns={columns} data={appointments} loading={isLoading} emptyMessage="No appointments today." />
    </div>
  );
}
