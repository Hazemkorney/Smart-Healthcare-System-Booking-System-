import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Plus, Pencil, Trash2, Calendar } from 'lucide-react';
import { departmentApi, doctorApi } from '../../api';
import { DataTable, type Column } from '../../components/DataTable';
import { Pagination } from '../../components/Pagination';
import { Modal } from '../../components/Modal';
import { ConfirmDialog } from '../../components/ConfirmDialog';
import { useApiMutation } from '../../hooks/useApiMutation';
import type { Doctor } from '../../types';

const schema = z.object({
  email: z.string().email(),
  password: z.string().min(6).optional().or(z.literal('')),
  fullName: z.string().min(1),
  specialization: z.string().min(1),
  departmentId: z.string().min(1),
  phone: z.string().optional(),
});

type FormData = z.infer<typeof schema>;

export function DoctorsPage() {
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Doctor | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Doctor | null>(null);

  const { data: doctorsPage, isLoading } = useQuery({
    queryKey: ['doctors', page, pageSize],
    queryFn: () => doctorApi.getAll(page, pageSize),
  });
  const { data: departments = [] } = useQuery({
    queryKey: ['departments', 'all'],
    queryFn: departmentApi.getAllItems,
  });

  const form = useForm<FormData>({ resolver: zodResolver(schema) });

  const createMutation = useApiMutation({
    mutationFn: doctorApi.create,
    successMessage: 'Doctor created',
    invalidateKeys: [['doctors']],
    onSuccess: () => { setModalOpen(false); form.reset(); },
  });

  const updateMutation = useApiMutation({
    mutationFn: ({ id, data }: { id: string; data: Record<string, unknown> }) => doctorApi.update(id, data),
    successMessage: 'Doctor updated',
    invalidateKeys: [['doctors']],
    onSuccess: () => { setModalOpen(false); setEditing(null); form.reset(); },
  });

  const deleteMutation = useApiMutation({
    mutationFn: doctorApi.delete,
    successMessage: 'Doctor deleted',
    invalidateKeys: [['doctors']],
    onSuccess: () => setDeleteTarget(null),
  });

  const openCreate = () => {
    setEditing(null);
    form.reset({ email: '', password: '', fullName: '', specialization: '', departmentId: '', phone: '' });
    setModalOpen(true);
  };

  const openEdit = (doc: Doctor) => {
    setEditing(doc);
    form.reset({
      email: '',
      password: '',
      fullName: doc.fullName,
      specialization: doc.specialization,
      departmentId: doc.departmentId,
      phone: doc.phone ?? '',
    });
    setModalOpen(true);
  };

  const columns: Column<Doctor>[] = useMemo(
    () => [
      { key: 'name', header: 'Name', render: (r) => r.fullName },
      { key: 'spec', header: 'Specialization', render: (r) => r.specialization },
      { key: 'dept', header: 'Department', render: (r) => r.departmentName },
      {
        key: 'actions',
        header: 'Actions',
        render: (r) => (
          <div className="flex gap-2">
            <Link to={`/admin/doctors/${r.id}/schedule`} className="text-primary-600"><Calendar className="h-4 w-4" /></Link>
            <button type="button" onClick={() => openEdit(r)} className="text-primary-600"><Pencil className="h-4 w-4" /></button>
            <button type="button" onClick={() => setDeleteTarget(r)} className="text-red-600"><Trash2 className="h-4 w-4" /></button>
          </div>
        ),
      },
    ],
    [],
  );

  const onSubmit = (values: FormData) => {
    if (editing) {
      updateMutation.mutate({
        id: editing.id,
        data: {
          fullName: values.fullName,
          specialization: values.specialization,
          departmentId: values.departmentId,
          phone: values.phone,
        },
      });
    } else {
      createMutation.mutate({ ...values, password: values.password || 'Doctor@123' });
    }
  };

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <h2 className="text-2xl font-bold">Doctors</h2>
        <button type="button" onClick={openCreate} className="inline-flex items-center gap-2 rounded-lg bg-primary-600 px-4 py-2 text-sm text-white">
          <Plus className="h-4 w-4" /> Add Doctor
        </button>
      </div>
      <DataTable columns={columns} data={doctorsPage?.data ?? []} loading={isLoading} />
      {doctorsPage && (
        <Pagination
          page={doctorsPage.page}
          pageSize={doctorsPage.pageSize}
          totalCount={doctorsPage.totalCount}
          totalPages={doctorsPage.totalPages}
          onPageChange={setPage}
        />
      )}

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Edit Doctor' : 'Add Doctor'} size="lg">
        <form onSubmit={form.handleSubmit(onSubmit)} className="grid grid-cols-2 gap-4">
          {!editing && (
            <>
              <div className="col-span-2">
                <label className="mb-1 block text-sm font-medium">Email</label>
                <input {...form.register('email')} type="email" className="w-full rounded-lg border px-3 py-2 text-sm" />
              </div>
              <div className="col-span-2">
                <label className="mb-1 block text-sm font-medium">Password</label>
                <input {...form.register('password')} type="password" className="w-full rounded-lg border px-3 py-2 text-sm" placeholder="Doctor@123" />
              </div>
            </>
          )}
          <div>
            <label className="mb-1 block text-sm font-medium">Full Name</label>
            <input {...form.register('fullName')} className="w-full rounded-lg border px-3 py-2 text-sm" />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium">Specialization</label>
            <input {...form.register('specialization')} className="w-full rounded-lg border px-3 py-2 text-sm" />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium">Department</label>
            <select {...form.register('departmentId')} className="w-full rounded-lg border px-3 py-2 text-sm">
              <option value="">Select...</option>
              {departments.map((d) => <option key={d.id} value={d.id}>{d.name}</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium">Phone</label>
            <input {...form.register('phone')} className="w-full rounded-lg border px-3 py-2 text-sm" />
          </div>
          <div className="col-span-2 flex justify-end gap-3">
            <button type="button" onClick={() => setModalOpen(false)} className="rounded-lg border px-4 py-2 text-sm">Cancel</button>
            <button type="submit" className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white">Save</button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog open={!!deleteTarget} title="Delete Doctor" message={`Delete ${deleteTarget?.fullName}?`} onCancel={() => setDeleteTarget(null)} onConfirm={() => deleteTarget && deleteMutation.mutate(deleteTarget.id)} loading={deleteMutation.isPending} />
    </div>
  );
}
