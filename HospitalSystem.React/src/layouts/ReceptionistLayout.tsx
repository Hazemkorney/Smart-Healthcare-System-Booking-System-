import { DashboardLayout } from './DashboardLayout';

const navItems = [
  { to: '/receptionist/patients', label: 'Patients' },
  { to: '/receptionist/patients/new', label: 'Register Patient' },
  { to: '/receptionist/appointments', label: 'Book Appointment' },
  { to: '/receptionist/schedule', label: 'Schedule' },
];

export function ReceptionistLayout() {
  return <DashboardLayout title="Receptionist Portal" navItems={navItems} />;
}
