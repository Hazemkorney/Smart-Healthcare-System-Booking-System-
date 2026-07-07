import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Search } from 'lucide-react';
import { patientApi } from '../../api';
import { DataTable, type Column } from '../../components/DataTable';
import { Pagination } from '../../components/Pagination';
import { parseGender } from '../../utils/format';
import type { Patient } from '../../types';

export function PatientsListPage() {
  const [query, setQuery] = useState('');
  const [page, setPage] = useState(1);
  const pageSize = 10;

  const { data, isLoading } = useQuery({
    queryKey: ['patients', query, page, pageSize],
    queryFn: () => patientApi.search(query || undefined, page, pageSize),
  });

  const columns: Column<Patient>[] = useMemo(
    () => [
      {
        key: 'name',
        header: 'Name',
        render: (r) => (
          <Link to={`/receptionist/patients/${r.id}`} className="font-medium text-primary-600 hover:underline">
            {r.fullName}
          </Link>
        ),
        sortValue: (r) => r.fullName,
      },
      { key: 'phone', header: 'Phone', render: (r) => r.phone },
      { key: 'email', header: 'Email', render: (r) => r.email ?? '—' },
      { key: 'gender', header: 'Gender', render: (r) => parseGender(r.gender) },
    ],
    [],
  );

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <h2 className="text-2xl font-bold">Patients</h2>
        <Link to="/receptionist/patients/new" className="rounded-lg bg-primary-600 px-4 py-2 text-sm text-white">
          Register Patient
        </Link>
      </div>
      <div className="relative mb-4 max-w-md">
        <Search className="absolute left-3 top-2.5 h-4 w-4 text-slate-400" />
        <input
          value={query}
          onChange={(e) => {
            setQuery(e.target.value);
            setPage(1);
          }}
          placeholder="Search by name, phone, email..."
          className="w-full rounded-lg border border-slate-300 py-2 pl-10 pr-3 text-sm"
        />
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
    </div>
  );
}
