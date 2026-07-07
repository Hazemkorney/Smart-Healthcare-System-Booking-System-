import { DashboardLayout } from './DashboardLayout';

const navItems = [
  { to: '/doctor/schedule', label: 'My Schedule' },
  { to: '/doctor/working-hours', label: 'Working Hours' },
];

export function DoctorLayout() {
  return <DashboardLayout title="Doctor Portal" navItems={navItems} />;
}
