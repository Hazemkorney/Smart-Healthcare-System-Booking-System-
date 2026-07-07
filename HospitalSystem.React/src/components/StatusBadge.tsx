import type { AppointmentStatus } from '../types';
import { parseStatus } from '../utils/format';

const styles: Record<AppointmentStatus, string> = {
  Confirmed: 'bg-blue-100 text-blue-800',
  CheckedIn: 'bg-yellow-100 text-yellow-800',
  InProgress: 'bg-orange-100 text-orange-800',
  Completed: 'bg-green-100 text-green-800',
  Cancelled: 'bg-red-100 text-red-800',
  NoShow: 'bg-gray-100 text-gray-700',
};

export function StatusBadge({ status }: { status: AppointmentStatus | number | string }) {
  const parsed = parseStatus(status as AppointmentStatus | number);
  return (
    <span className={`inline-flex rounded-full px-2.5 py-0.5 text-xs font-medium ${styles[parsed]}`}>
      {parsed}
    </span>
  );
}
