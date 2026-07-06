import { z } from 'zod';

const PHONE_ALLOWED = /^01[0125][0-9]{8}$/;

function countDigits(value: string): number {
  return [...value].filter((char) => char >= '0' && char <= '9').length;
}

export function isValidPhoneNumber(value: string): boolean {
  if (!PHONE_ALLOWED.test(value))
    return false;

  const digits = countDigits(value);
  return digits === 11;
}

export const phoneNumberSchema = z
  .string()
  .min(1, 'Phone number is required')
  .refine(isValidPhoneNumber, 'Enter a valid phone number (11 digits, e.g. 010xxxxxxxx, 011xxxxxxxx, 012xxxxxxxx, 015xxxxxxxx)');

export const optionalPhoneNumberSchema = z
  .string()
  .optional()
  .refine(
    (value) => !value || value.trim() === '' || isValidPhoneNumber(value),
    'Enter a valid phone number (11 digits, e.g. 010xxxxxxxx, 011xxxxxxxx, 012xxxxxxxx, 015xxxxxxxx)',
  );
  