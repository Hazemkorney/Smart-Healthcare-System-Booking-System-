import { z } from 'zod';

export const NAME_REGEX = /^[\u0600-\u06FFa-zA-Z\s]+$/;
export const EGYPT_PHONE_REGEX = /^01[0125][0-9]{8}$/;
export const NATIONAL_ID_REGEX = /^[23][0-9]{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])(0[1-9]|[12][0-9]|3[1-5]|88)[0-9]{4}$/;

export const nameSchema = z
  .string()
  .min(1, 'Full Name is required')
  .regex(NAME_REGEX, 'Full Name cannot contain numbers or special characters.')
  .max(100, 'Full Name cannot exceed 100 characters');

export const phoneNumberSchema = z
  .string()
  .min(1, 'Phone number is required')
  .regex(EGYPT_PHONE_REGEX, 'Phone number must be exactly 11 digits and start with 010, 011, 012, or 015.');

export const optionalPhoneNumberSchema = z
  .string()
  .optional()
  .refine(
    (value) => !value || value.trim() === '' || EGYPT_PHONE_REGEX.test(value),
    'Phone number must be exactly 11 digits and start with 010, 011, 012, or 015.',
  );

export const nationalIdSchema = z
  .string()
  .min(14, 'National ID must be exactly 14 digits.')
  .max(14, 'National ID must be exactly 14 digits.')
  .regex(NATIONAL_ID_REGEX, 'Invalid Egyptian National ID format.');
