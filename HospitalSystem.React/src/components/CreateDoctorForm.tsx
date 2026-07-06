import { useState } from 'react';
import type { ChangeEvent, FormEvent } from 'react';
import { optionalPhoneNumberSchema } from '../utils/validation';

interface DoctorFormData {
  email: string;
  password: string;
  departmentId: string;
  fullName: string;
  specialization: string;
  phone: string;
}

interface FormErrors {
  email?: string;
  password?: string;
  departmentId?: string;
  fullName?: string;
  specialization?: string;
  phone?: string;
}

export default function CreateDoctorForm() {
  const [formData, setFormData] = useState<DoctorFormData>({
    email: '',
    password: '',
    departmentId: '',
    fullName: '',
    specialization: '',
    phone: ''
  });

  const [errors, setErrors] = useState<FormErrors>({});

  const handleChange = (e: ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
    
    if (errors[name as keyof FormErrors]) {
      setErrors({ ...errors, [name]: undefined });
    }
  };

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};
    const textOnlyRegex = /^[\u0600-\u06FFa-zA-Z\s]+$/;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    // 1. Email
    if (!formData.email.trim()) {
      newErrors.email = 'Email is required.';
    } else if (!emailRegex.test(formData.email)) {
      newErrors.email = 'Invalid email format.';
    }

    // 2. Password
    if (!formData.password) {
      newErrors.password = 'Password is required.';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters.';
    }

    // 3. Department ID
    if (!formData.departmentId.trim()) {
      newErrors.departmentId = 'Department ID is required.';
    }

    // 4. Full Name (No Numbers)
    if (!formData.fullName.trim()) {
      newErrors.fullName = 'Full Name is required.';
    } else if (!textOnlyRegex.test(formData.fullName)) {
      newErrors.fullName = 'Name cannot contain numbers or special characters.';
    }

    // 5. Specialization (No Numbers)
    if (!formData.specialization.trim()) {
      newErrors.specialization = 'Specialization is required.';
    } else if (!textOnlyRegex.test(formData.specialization)) {
      newErrors.specialization = 'Specialization cannot contain numbers or special characters.';
    }

    // 6. Phone (Zod Validation)
    const phoneValidation = optionalPhoneNumberSchema.safeParse(formData.phone);
    if (!phoneValidation.success) {
      newErrors.phone = phoneValidation.error.flatten().formErrors[0];
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    
    if (validateForm()) {
      console.log('Form Submitted Successfully!', formData);
    } else {
      console.log('Form has errors.');
    }
  };

  // ستايل ثابت لرسالة الخطأ عشان تظهر بنفس الشكل اللي في صورتك
  const errorStyle = { color: '#dc3545', fontSize: '14px', marginTop: '5px' };

  return (
    <div className="container mt-5 mb-5">
      <h2>Create Doctor Account</h2>
      <form onSubmit={handleSubmit} noValidate>
        
        {/* Email */}
        <div className="mb-3">
          <label className="form-label">Email</label>
          <input
            type="email"
            name="email"
            className="form-control"
            value={formData.email}
            onChange={handleChange}
          />
          {errors.email && <div style={errorStyle}>{errors.email}</div>}
        </div>

        {/* Password */}
        <div className="mb-3">
          <label className="form-label">Password</label>
          <input
            type="password"
            name="password"
            className="form-control"
            value={formData.password}
            onChange={handleChange}
          />
          {errors.password && <div style={errorStyle}>{errors.password}</div>}
        </div>

        {/* Department ID */}
        <div className="mb-3">
          <label className="form-label">Department ID</label>
          <input
            type="text"
            name="departmentId"
            className="form-control"
            value={formData.departmentId}
            onChange={handleChange}
          />
          {errors.departmentId && <div style={errorStyle}>{errors.departmentId}</div>}
        </div>

        {/* Full Name */}
        <div className="mb-3">
          <label className="form-label">Full Name</label>
          <input
            type="text"
            name="fullName"
            className="form-control"
            value={formData.fullName}
            onChange={handleChange}
          />
          {errors.fullName && <div style={errorStyle}>{errors.fullName}</div>}
        </div>

        {/* Specialization */}
        <div className="mb-3">
          <label className="form-label">Specialization</label>
          <input
            type="text"
            name="specialization"
            className="form-control"
            value={formData.specialization}
            onChange={handleChange}
          />
          {errors.specialization && <div style={errorStyle}>{errors.specialization}</div>}
        </div>

        {/* Phone Number */}
        <div className="mb-3">
          <label className="form-label">Phone</label>
          <input
            type="tel"
            name="phone"
            maxLength={11}
            className="form-control"
            value={formData.phone}
            onChange={handleChange}
          />
          {errors.phone && <div style={errorStyle}>{errors.phone}</div>}
        </div>

        <button type="submit" className="btn btn-primary w-100 mt-3">
          Save Doctor
        </button>
      </form>
    </div>
  );
}