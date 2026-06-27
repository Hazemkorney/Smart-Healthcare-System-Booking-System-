import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from 'react-hot-toast';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { PrivateRoute } from './components/PrivateRoute';
import { LoginPage } from './pages/LoginPage';
import { AdminLayout } from './layouts/AdminLayout';
import { ReceptionistLayout } from './layouts/ReceptionistLayout';
import { DoctorLayout } from './layouts/DoctorLayout';
import { PatientLayout } from './layouts/PatientLayout';
import { DepartmentsPage } from './pages/admin/DepartmentsPage';
import { DoctorsPage } from './pages/admin/DoctorsPage';
import { DoctorSchedulePage as AdminDoctorSchedulePage } from './pages/admin/DoctorSchedulePage';
import { ReceptionistsPage } from './pages/admin/ReceptionistsPage';
import { PatientsListPage } from './pages/receptionist/PatientsListPage';
import { RegisterPatientPage } from './pages/receptionist/RegisterPatientPage';
import { PatientDetailPage } from './pages/receptionist/PatientDetailPage';
import { BookAppointmentPage } from './pages/receptionist/BookAppointmentPage';
import { ReceptionistSchedulePage } from './pages/receptionist/ReceptionistSchedulePage';
import { AppointmentDetailPage } from './pages/receptionist/AppointmentDetailPage';
import { DoctorSchedulePage } from './pages/doctor/DoctorSchedulePage';
import { ConsultationPage } from './pages/doctor/ConsultationPage';
import { PatientProfilePage } from './pages/patient/PatientProfilePage';
import { PatientAppointmentsPage } from './pages/patient/PatientAppointmentsPage';
import { PatientAppointmentDetailPage } from './pages/patient/PatientAppointmentDetailPage';
import { roleHomePath } from './utils/format';

const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: 1, staleTime: 30_000 } },
});

function RootRedirect() {
  const { isAuthenticated, user } = useAuth();
  if (!isAuthenticated || !user) return <Navigate to="/login" replace />;
  return <Navigate to={roleHomePath(user.role)} replace />;
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/" element={<RootRedirect />} />

            <Route element={<PrivateRoute role="Admin" />}>
              <Route path="/admin" element={<AdminLayout />}>
                <Route index element={<Navigate to="departments" replace />} />
                <Route path="departments" element={<DepartmentsPage />} />
                <Route path="doctors" element={<DoctorsPage />} />
                <Route path="doctors/:id/schedule" element={<AdminDoctorSchedulePage />} />
                <Route path="receptionists" element={<ReceptionistsPage />} />
              </Route>
            </Route>

            <Route element={<PrivateRoute role="Receptionist" />}>
              <Route path="/receptionist" element={<ReceptionistLayout />}>
                <Route index element={<Navigate to="patients" replace />} />
                <Route path="patients" element={<PatientsListPage />} />
                <Route path="patients/new" element={<RegisterPatientPage />} />
                <Route path="patients/:id" element={<PatientDetailPage />} />
                <Route path="appointments" element={<BookAppointmentPage />} />
                <Route path="appointments/:id" element={<AppointmentDetailPage />} />
                <Route path="schedule" element={<ReceptionistSchedulePage />} />
              </Route>
            </Route>

            <Route element={<PrivateRoute role="Doctor" />}>
              <Route path="/doctor" element={<DoctorLayout />}>
                <Route index element={<Navigate to="schedule" replace />} />
                <Route path="schedule" element={<DoctorSchedulePage />} />
                <Route path="appointments/:id" element={<ConsultationPage />} />
              </Route>
            </Route>

            <Route element={<PrivateRoute role="Patient" />}>
              <Route path="/patient" element={<PatientLayout />}>
                <Route index element={<Navigate to="profile" replace />} />
                <Route path="profile" element={<PatientProfilePage />} />
                <Route path="appointments" element={<PatientAppointmentsPage />} />
                <Route path="appointments/:id" element={<PatientAppointmentDetailPage />} />
              </Route>
            </Route>

            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </BrowserRouter>
        <Toaster position="top-right" />
      </AuthProvider>
    </QueryClientProvider>
  );
}
