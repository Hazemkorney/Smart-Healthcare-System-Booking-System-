import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { receptionistApi } from '../../api';
import { DataTable, type Column } from '../../components/DataTable';
import { Pagination } from '../../components/Pagination';
import { Modal } from '../../components/Modal';
import { ConfirmDialog } from '../../components/ConfirmDialog';
import { useApiMutation } from '../../hooks/useApiMutation';
import type { Receptionist } from '../../types';
import { optionalPhoneNumberSchema, NAME_REGEX } from '../../utils/validation';

const createSchema = z.object({
  email: z.string().email('Valid email required'),
  password: z.string().min(6).optional().or(z.literal('')),
  fullName: z.string().min(1, 'Full name required').regex(NAME_REGEX, 'Full Name cannot contain numbers or special characters.').max(100, 'Full Name cannot exceed 100 characters'),
  phone: optionalPhoneNumberSchema,
});

const editSchema = z.object({
  fullName: z.string().min(1, 'Full name required').regex(NAME_REGEX, 'Full Name cannot contain numbers or special characters.').max(100, 'Full Name cannot exceed 100 characters'),
  phone: optionalPhoneNumberSchema,
});

// type CreateFormData = z.infer<typeof createSchema>;
// type EditFormData = z.infer<typeof editSchema>;

type FormValues = {
  email?: string;
  password?: string;
  fullName: string;
  phone?: string;
};

export function ReceptionistsPage() {
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Receptionist | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Receptionist | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['receptionists', page, pageSize],
    queryFn: () => receptionistApi.getAll(page, pageSize),
  });
  const form = useForm<FormValues>({
    resolver: async (values, context, options) =>
      zodResolver(editing ? editSchema : createSchema)(values, context, options),
    mode: 'onChange',
  });

  const createMutation = useApiMutation({
    mutationFn: receptionistApi.create,
    successMessage: 'Receptionist created',
    invalidateKeys: [['receptionists']],
    onSuccess: () => { setModalOpen(false); form.reset(); },
  });

  const updateMutation = useApiMutation({
    mutationFn: ({ id, data }: { id: string; data: Record<string, unknown> }) => receptionistApi.update(id, data),
    successMessage: 'Receptionist updated',
    invalidateKeys: [['receptionists']],
    onSuccess: () => { setModalOpen(false); setEditing(null); form.reset(); },
  });

  const deleteMutation = useApiMutation({
    mutationFn: receptionistApi.delete,
    successMessage: 'Receptionist deleted',
    invalidateKeys: [['receptionists']],
    onSuccess: () => setDeleteTarget(null),
  });

  const columns: Column<Receptionist>[] = useMemo(
    () => [
      { key: 'name', header: 'Name', render: (r) => r.fullName },
      { key: 'phone', header: 'Phone', render: (r) => r.phone ?? '—' },
      {
        key: 'actions',
        header: 'Actions',
        render: (r) => (
          <div className="flex gap-2">
            <button type="button" onClick={() => { setEditing(r); form.reset({ fullName: r.fullName, phone: r.phone ?? '' }); setModalOpen(true); }} className="text-primary-600"><Pencil className="h-4 w-4" /></button>
            <button type="button" onClick={() => setDeleteTarget(r)} className="text-red-600"><Trash2 className="h-4 w-4" /></button>
          </div>
        ),
      },
    ],
    [form],
  );

  const onSubmit = (values: FormValues) => {
    if (editing) {
      updateMutation.mutate({ id: editing.id, data: { fullName: values.fullName, phone: values.phone } });
    } else {
      createMutation.mutate({ email: values.email, fullName: values.fullName, phone: values.phone, password: values.password || 'Reception@123' });
    }
  };

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <h2 className="text-2xl font-bold">Receptionists</h2>
        <button type="button" onClick={() => { setEditing(null); form.reset(); setModalOpen(true); }} className="inline-flex items-center gap-2 rounded-lg bg-primary-600 px-4 py-2 text-sm text-white">
          <Plus className="h-4 w-4" /> Add Receptionist
        </button>
      </div>
      <DataTable columns={columns} data={data?.data ?? []} loading={isLoading} />
      {data && (
        <Pagination
          page={data.page}
          pageSize={data.pageSize}
          totalCount={data.totalCount}
          totalPages={data.totalPages}
          onPageChange={setPage}
        />
      )}

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Edit Receptionist' : 'Add Receptionist'}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
          {!editing && (
            <>
              <div>
                <label className="text-sm font-medium">Email</label>
                <input {...form.register('email')} type="email" className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" />
                {form.formState.errors.email && <p className="mt-1 text-xs text-red-600">{form.formState.errors.email.message}</p>}
              </div>
              <div>
                <label className="text-sm font-medium">Password</label>
                <input {...form.register('password')} type="password" className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" placeholder="Reception@123" />
                {form.formState.errors.password && <p className="mt-1 text-xs text-red-600">{form.formState.errors.password.message}</p>}
              </div>
            </>
          )}
          <div>
            <label className="text-sm font-medium">Full Name</label>
            <input {...form.register('fullName')} className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" />
            {form.formState.errors.fullName && <p className="mt-1 text-xs text-red-600">{form.formState.errors.fullName.message}</p>}
          </div>
          <div>
            <label className="text-sm font-medium">Phone</label>
            <input {...form.register('phone')} type="tel" placeholder="01xxxxxxxxx" className="mt-1 w-full rounded-lg border px-3 py-2 text-sm" />
            {form.formState.errors.phone && <p className="mt-1 text-xs text-red-600">{form.formState.errors.phone.message}</p>}
          </div>
          <div className="flex justify-end gap-3">
            <button type="button" onClick={() => setModalOpen(false)} className="rounded-lg border px-4 py-2 text-sm">Cancel</button>
            <button type="submit" className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white">Save</button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog open={!!deleteTarget} title="Delete Receptionist" message={`Delete ${deleteTarget?.fullName}?`} onCancel={() => setDeleteTarget(null)} onConfirm={() => deleteTarget && deleteMutation.mutate(deleteTarget.id)} loading={deleteMutation.isPending} />
    </div>
  );
}
