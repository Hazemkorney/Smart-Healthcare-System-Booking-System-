import { DashboardLayout } from './DashboardLayout';

const navItems = [
  { to: '/patient/profile', label: 'Profile' },
  { to: '/patient/appointments', label: 'Appointments' },
];

export function PatientLayout() {
  return <DashboardLayout title="Patient Portal" navItems={navItems} />;
}
