import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { patientApi } from '../../api';
import { parseGender } from '../../utils/format';

export function PatientDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { data: patient, isLoading } = useQuery({
    queryKey: ['patient', id],
    queryFn: () => patientApi.getById(id!),
    enabled: !!id,
  });

  if (isLoading) return <div className="h-40 animate-pulse rounded-xl bg-slate-100" />;
  if (!patient) return <p>Patient not found.</p>;

  return (
    <div>
      <h2 className="mb-6 text-2xl font-bold">{patient.fullName}</h2>
      <div className="mb-8 grid grid-cols-2 gap-4 rounded-xl border bg-white p-6 shadow-sm md:grid-cols-3">
        <div><p className="text-xs text-slate-500">Phone</p><p className="font-medium">{patient.phone}</p></div>
        <div><p className="text-xs text-slate-500">Email</p><p className="font-medium">{patient.email ?? '—'}</p></div>
        <div><p className="text-xs text-slate-500">Gender</p><p className="font-medium">{parseGender(patient.gender)}</p></div>
        <div><p className="text-xs text-slate-500">Blood Type</p><p className="font-medium">{patient.bloodType ?? '—'}</p></div>
        <div><p className="text-xs text-slate-500">DOB</p><p className="font-medium">{patient.dateOfBirth.slice(0, 10)}</p></div>
      </div>
      <Link to="/receptionist/appointments" className="text-sm text-primary-600 hover:underline">Book appointment →</Link>
    </div>
  );
}
