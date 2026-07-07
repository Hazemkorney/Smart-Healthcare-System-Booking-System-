import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from 'react';
import { authApi } from '../api';
import type { CurrentUser, LoginResponse } from '../types';
import { parseRole, roleHomePath } from '../utils/format';

interface AuthContextValue {
  user: CurrentUser | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<string>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

function loadStoredUser(): CurrentUser | null {
  const raw = localStorage.getItem('user');
  if (!raw) return null;
  try {
    const parsed = JSON.parse(raw) as CurrentUser;
    return { ...parsed, role: parseRole(parsed.role as never) };
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem('token'));
  const [user, setUser] = useState<CurrentUser | null>(() => loadStoredUser());

  const login = useCallback(async (email: string, password: string) => {
    const response: LoginResponse = await authApi.login(email, password);
    const currentUser: CurrentUser = {
      userId: response.userId,
      email: response.email,
      role: parseRole(response.role),
    };
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify(currentUser));
    setToken(response.token);
    setUser(currentUser);
    return roleHomePath(currentUser.role);
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setToken(null);
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({ user, token, isAuthenticated: !!token && !!user, login, logout }),
    [user, token, login, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
