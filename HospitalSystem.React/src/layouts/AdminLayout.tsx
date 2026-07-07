import { DashboardLayout } from './DashboardLayout';

const navItems = [
  { to: '/admin/departments', label: 'Departments' },
  { to: '/admin/doctors', label: 'Doctors' },
  { to: '/admin/doctor-schedule', label: 'Doctor Schedule' },
  { to: '/admin/receptionists', label: 'Receptionists' },
];

export function AdminLayout() {
  return <DashboardLayout title="Admin Portal" navItems={navItems} />;
}
