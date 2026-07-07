import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import type { UserRole } from '../types';
import { roleHomePath } from '../utils/format';

interface PrivateRouteProps {
  role?: UserRole;
}

export function PrivateRoute({ role }: PrivateRouteProps) {
  const { isAuthenticated, user } = useAuth();

  if (!isAuthenticated || !user) {
    return <Navigate to="/login" replace />;
  }

  if (role && user.role !== role) {
    return <Navigate to={roleHomePath(user.role)} replace />;
  }

  return <Outlet />;
}
