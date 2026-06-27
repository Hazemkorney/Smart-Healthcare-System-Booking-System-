import { DashboardLayout } from './DashboardLayout';

const navItems = [{ to: '/doctor/schedule', label: 'My Schedule' }];

export function DoctorLayout() {
  return <DashboardLayout title="Doctor Portal" navItems={navItems} />;
}
