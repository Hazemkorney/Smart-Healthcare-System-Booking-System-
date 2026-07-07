import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { NAME_REGEX } from '../../utils/validation';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { departmentApi } from '../../api';
import { DataTable, type Column } from '../../components/DataTable';
import { Pagination } from '../../components/Pagination';
import { Modal } from '../../components/Modal';
import { ConfirmDialog } from '../../components/ConfirmDialog';
import { useApiMutation } from '../../hooks/useApiMutation';
import type { Department } from '../../types';

const schema = z.object({
  name: z
    .string()
    .min(1, 'Name required')
    .regex(
      NAME_REGEX,
      'Department Name cannot contain numbers or special characters.'
    )
    .max(100, 'Department Name cannot exceed 100 characters'),

  description: z
    .string()
    .max(200, 'Description cannot exceed 200 characters')
    .optional(),
});

type FormData = z.infer<typeof schema>;

export function DepartmentsPage() {
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Department | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Department | null>(null);

  const { data, isLoading, isError } = useQuery({
    queryKey: ['departments', page, pageSize],
    queryFn: () => departmentApi.getAll(page, pageSize),
  });

  const form = useForm<FormData>({ resolver: zodResolver(schema), mode: 'onChange' });

  const createMutation = useApiMutation({
    mutationFn: departmentApi.create,
    successMessage: 'Department created',
    invalidateKeys: [['departments']],
    onSuccess: () => {
      setModalOpen(false);
      form.reset();
    },
  });

  const updateMutation = useApiMutation({
    mutationFn: ({ id, data }: { id: string; data: FormData }) => departmentApi.update(id, data),
    successMessage: 'Department updated',
    invalidateKeys: [['departments']],
    onSuccess: () => {
      setModalOpen(false);
      setEditing(null);
      form.reset();
    },
  });

  const deleteMutation = useApiMutation({
    mutationFn: departmentApi.delete,
    successMessage: 'Department deleted',
    invalidateKeys: [['departments']],
    onSuccess: () => setDeleteTarget(null),
  });

  const openCreate = () => {
    setEditing(null);
    form.reset({ name: '', description: '' });
    setModalOpen(true);
  };

  const openEdit = (dept: Department) => {
    setEditing(dept);
    form.reset({ name: dept.name, description: dept.description ?? '' });
    setModalOpen(true);
  };

  const columns: Column<Department>[] = useMemo(
    () => [
      { key: 'name', header: 'Name', render: (r) => r.name, sortValue: (r) => r.name },
      { key: 'desc', header: 'Description', render: (r) => r.description ?? '—' },
      {
        key: 'actions',
        header: 'Actions',
        render: (r) => (
          <div className="flex gap-2">
            <button type="button" onClick={() => openEdit(r)} className="text-primary-600 hover:text-primary-700">
              <Pencil className="h-4 w-4" />
            </button>
            <button type="button" onClick={() => setDeleteTarget(r)} className="text-red-600 hover:text-red-700">
              <Trash2 className="h-4 w-4" />
            </button>
          </div>
        ),
      },
    ],
    [],
  );

  const onSubmit = (values: FormData) => {
    if (editing) updateMutation.mutate({ id: editing.id, data: values });
    else createMutation.mutate(values);
  };

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <h2 className="text-2xl font-bold text-slate-900">Departments</h2>
        <button
          type="button"
          onClick={openCreate}
          className="inline-flex items-center gap-2 rounded-lg bg-primary-600 px-4 py-2 text-sm font-medium text-white hover:bg-primary-700"
        >
          <Plus className="h-4 w-4" /> Add Department
        </button>
      </div>
      {isError && <p className="mb-4 text-sm text-red-600">Failed to load departments.</p>}
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

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? 'Edit Department' : 'Add Department'}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label className="mb-1 block text-sm font-medium">Name</label>
            <input {...form.register('name')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm" />
            {form.formState.errors.name && <p className="text-xs text-red-600">{form.formState.errors.name.message}</p>}
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium">Description</label>
            <textarea {...form.register('description')} rows={3} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm" />
          </div>
          <div className="flex justify-end gap-3">
            <button type="button" onClick={() => setModalOpen(false)} className="rounded-lg border px-4 py-2 text-sm">Cancel</button>
            <button type="submit" className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white">Save</button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        open={!!deleteTarget}
        title="Delete Department"
        message={`Delete "${deleteTarget?.name}"?`}
        onCancel={() => setDeleteTarget(null)}
        onConfirm={() => deleteTarget && deleteMutation.mutate(deleteTarget.id)}
        loading={deleteMutation.isPending}
      />
    </div>
  );
}
