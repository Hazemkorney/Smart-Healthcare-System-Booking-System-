import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate } from 'react-router-dom';
import { patientApi } from '../../api';
import { useApiMutation } from '../../hooks/useApiMutation';

const schema = z.object({
  fullName: z.string().min(1),
  dateOfBirth: z.string().min(1),
  gender: z.enum(['Male', 'Female', 'Other']),
  phone: z.string().min(1),
  email: z.string().email().optional().or(z.literal('')),
  address: z.string().optional(),
  nationalId: z.string().optional(),
  bloodType: z.string().optional(),
});

type FormData = z.infer<typeof schema>;

export function RegisterPatientPage() {
  const navigate = useNavigate();
  const form = useForm<FormData>({ resolver: zodResolver(schema), defaultValues: { gender: 'Male' } });

  const mutation = useApiMutation({
    mutationFn: patientApi.create,
    successMessage: 'Patient registered',
    invalidateKeys: [['patients']],
    onSuccess: (data) => navigate(`/receptionist/patients/${data.id}`),
  });

  return (
    <div className="max-w-2xl">
      <h2 className="mb-6 text-2xl font-bold">Register Patient</h2>
      <form onSubmit={form.handleSubmit((v) => mutation.mutate(v))} className="space-y-4 rounded-xl border bg-white p-6 shadow-sm">
        <div className="grid grid-cols-2 gap-4">
          <div className="col-span-2"><label className="text-sm font-medium">Full Name</label><input {...form.register('fullName')} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" /></div>
          <div><label className="text-sm font-medium">Date of Birth</label><input {...form.register('dateOfBirth')} type="date" className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" /></div>
          <div><label className="text-sm font-medium">Gender</label><select {...form.register('gender')} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm"><option value="Male">Male</option><option value="Female">Female</option><option value="Other">Other</option></select></div>
          <div><label className="text-sm font-medium">Phone</label><input {...form.register('phone')} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" /></div>
          <div><label className="text-sm font-medium">Email</label><input {...form.register('email')} type="email" className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" /></div>
          <div><label className="text-sm font-medium">Blood Type</label><input {...form.register('bloodType')} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" /></div>
          <div><label className="text-sm font-medium">National ID</label><input {...form.register('nationalId')} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" /></div>
          <div className="col-span-2"><label className="text-sm font-medium">Address</label><input {...form.register('address')} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" /></div>
        </div>
        <button type="submit" disabled={mutation.isPending} className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white">{mutation.isPending ? 'Saving...' : 'Register'}</button>
      </form>
    </div>
  );
}
