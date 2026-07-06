import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate } from 'react-router-dom';
import { patientApi } from '../../api';
import { useApiMutation } from '../../hooks/useApiMutation';
import { phoneNumberSchema } from '../../utils/validation';
import { bloodTypes } from '../../utils/bloodTypes';

const schema = z.object({
  fullName: z.string().min(1, 'Full name is required'),
  dateOfBirth: z.string().min(1, 'Date of birth is required'),
  gender: z.enum(['Male', 'Female', 'Other']),
  phone: phoneNumberSchema,
  email: z.string().min(1, 'Email is required').email('Enter a valid email'),
  address: z.string().min(1, 'Address is required'),
  nationalId: z.string().min(1, 'National ID is required'),
  bloodType: z.enum(bloodTypes, { message: 'Blood type is required' }),
});

type FormData = z.infer<typeof schema>;

function FieldError({ message }: { message?: string }) {
  if (!message) return null;
  return <p className="mt-1 text-xs text-red-600">{message}</p>;
}

export function RegisterPatientPage() {
  const navigate = useNavigate();
  const form = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { gender: 'Male' },
  });

  const mutation = useApiMutation({
    mutationFn: patientApi.create,
    successMessage: 'Patient registered',
    invalidateKeys: [['patients']],
    onSuccess: (data) => navigate(`/receptionist/patients/${data.id}`),
  });

  const { errors } = form.formState;

  return (
    <div className="max-w-2xl">
      <h2 className="mb-6 text-2xl font-bold">Register Patient</h2>
      <form onSubmit={form.handleSubmit((v) => mutation.mutate(v))} className="space-y-4 rounded-xl border bg-white p-6 shadow-sm">
        <div className="grid grid-cols-2 gap-4">
          <div className="col-span-2">
            <label className={`text-sm font-medium ${errors.fullName ? 'text-red-600' : ''}`}>Full Name <span className="text-red-500">*</span></label>
            <input {...form.register('fullName')} className={`mt-1 w-full rounded-lg border px-3 py-2 text-sm ${errors.fullName ? 'border-red-600 focus:border-red-600 focus:ring-red-600' : ''}`} />
            <FieldError message={errors.fullName?.message} />
          </div>
          <div>
            <label className={`text-sm font-medium ${errors.dateOfBirth ? 'text-red-600' : ''}`}>Date of Birth <span className="text-red-500">*</span></label>
            <input {...form.register('dateOfBirth')} type="date" className={`mt-1 w-full rounded-lg border px-3 py-2 text-sm ${errors.dateOfBirth ? 'border-red-600 focus:border-red-600 focus:ring-red-600' : ''}`} />
            <FieldError message={errors.dateOfBirth?.message} />
          </div>
          <div>
            <label className={`text-sm font-medium ${errors.gender ? 'text-red-600' : ''}`}>Gender <span className="text-red-500">*</span></label>
            <select {...form.register('gender')} className={`mt-1 w-full rounded-lg border px-3 py-2 text-sm ${errors.gender ? 'border-red-600 focus:border-red-600 focus:ring-red-600' : ''}`}>
              <option value="Male">Male</option>
              <option value="Female">Female</option>
              <option value="Other">Other</option>
            </select>
          </div>
          <div>
            <label className={`text-sm font-medium ${errors.phone ? 'text-red-600' : ''}`}>Phone <span className="text-red-500">*</span></label>
            <input {...form.register('phone')} type="tel" placeholder="01xxxxxxxxx" className={`mt-1 w-full rounded-lg border px-3 py-2 text-sm ${errors.phone ? 'border-red-600 focus:border-red-600 focus:ring-red-600' : ''}`} />
            <FieldError message={errors.phone?.message} />
          </div>
          <div>
            <label className={`text-sm font-medium ${errors.email ? 'text-red-600' : ''}`}>Email <span className="text-red-500">*</span></label>
            <input {...form.register('email')} type="email" className={`mt-1 w-full rounded-lg border px-3 py-2 text-sm ${errors.email ? 'border-red-600 focus:border-red-600 focus:ring-red-600' : ''}`} />
            <FieldError message={errors.email?.message} />
          </div>
          <div>
            <label className={`text-sm font-medium ${errors.bloodType ? 'text-red-600' : ''}`}>Blood Type <span className="text-red-500">*</span></label>
            <select {...form.register('bloodType')} defaultValue="" className={`mt-1 w-full rounded-lg border px-3 py-2 text-sm ${errors.bloodType ? 'border-red-600 focus:border-red-600 focus:ring-red-600' : ''}`}>
              <option value="" disabled>Select blood type...</option>
              {bloodTypes.map((type) => (
                <option key={type} value={type}>{type}</option>
              ))}
            </select>
            <FieldError message={errors.bloodType?.message} />
          </div>
          <div>
            <label className={`text-sm font-medium ${errors.nationalId ? 'text-red-600' : ''}`}>National ID <span className="text-red-500">*</span></label>
            <input {...form.register('nationalId')} className={`mt-1 w-full rounded-lg border px-3 py-2 text-sm ${errors.nationalId ? 'border-red-600 focus:border-red-600 focus:ring-red-600' : ''}`} />
            <FieldError message={errors.nationalId?.message} />
          </div>
          <div className="col-span-2">
            <label className={`text-sm font-medium ${errors.address ? 'text-red-600' : ''}`}>Address <span className="text-red-500">*</span></label>
            <input {...form.register('address')} className={`mt-1 w-full rounded-lg border px-3 py-2 text-sm ${errors.address ? 'border-red-600 focus:border-red-600 focus:ring-red-600' : ''}`} />
            <FieldError message={errors.address?.message} />
          </div>
        </div>
        <button type="submit" disabled={mutation.isPending} className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white">
          {mutation.isPending ? 'Saving...' : 'Register'}
        </button>
      </form>
    </div>
  );
}
