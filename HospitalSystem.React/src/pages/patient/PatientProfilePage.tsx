import { useQuery } from '@tanstack/react-query';
import { patientApi } from '../../api';
import { parseGender } from '../../utils/format';

export function PatientProfilePage() {
  const { data: patient, isLoading, isError } = useQuery({
    queryKey: ['patient-profile'],
    queryFn: patientApi.getProfile,
  });

  if (isLoading) return <div className="h-40 animate-pulse rounded-xl bg-slate-100" />;
  if (isError) return <p className="text-red-600">Unable to load profile. Patient account may not be linked.</p>;
  if (!patient) return null;

  return (
    <div className="max-w-2xl">
      <h2 className="mb-6 text-2xl font-bold">My Profile</h2>
      <div className="grid grid-cols-2 gap-4 rounded-xl border bg-white p-6 shadow-sm">
        <div><p className="text-xs text-slate-500">Full Name</p><p className="font-medium">{patient.fullName}</p></div>
        <div><p className="text-xs text-slate-500">Phone</p><p className="font-medium">{patient.phone}</p></div>
        <div><p className="text-xs text-slate-500">Email</p><p className="font-medium">{patient.email ?? '—'}</p></div>
        <div><p className="text-xs text-slate-500">Gender</p><p className="font-medium">{parseGender(patient.gender)}</p></div>
        <div><p className="text-xs text-slate-500">Date of Birth</p><p className="font-medium">{patient.dateOfBirth.slice(0, 10)}</p></div>
        <div><p className="text-xs text-slate-500">Blood Type</p><p className="font-medium">{patient.bloodType ?? '—'}</p></div>
        <div className="col-span-2"><p className="text-xs text-slate-500">Address</p><p className="font-medium">{patient.address ?? '—'}</p></div>
      </div>
    </div>
  );
}
