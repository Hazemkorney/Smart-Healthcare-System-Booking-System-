import { DashboardLayout } from './DashboardLayout';

const navItems = [
  { to: '/patient/medical-history', label: 'Medical History' },
  { to: '/patient/appointments', label: 'Appointments' },
  { to: '/patient/profile', label: 'Profile' },
];

export function PatientLayout() {
  return <DashboardLayout title="Patient Portal" navItems={navItems} />;
}
