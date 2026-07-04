import { z } from 'zod';

const PHONE_ALLOWED = /^\+?[0-9\s\-().]+$/;

function countDigits(value: string): number {
  return [...value].filter((char) => char >= '0' && char <= '9').length;
}

export function isValidPhoneNumber(value: string): boolean {
  if (!PHONE_ALLOWED.test(value))
    return false;

  const digits = countDigits(value);
  return digits >= 7 && digits <= 15;
}

export const phoneNumberSchema = z
  .string()
  .min(1, 'Phone number is required')
  .refine(isValidPhoneNumber, 'Enter a valid phone number (7–15 digits, e.g. 01xxxxxxxxx)');

export const optionalPhoneNumberSchema = z
  .string()
  .optional()
  .refine(
    (value) => !value || value.trim() === '' || isValidPhoneNumber(value),
    'Enter a valid phone number (7–15 digits, e.g. 01xxxxxxxxx)',
  );
