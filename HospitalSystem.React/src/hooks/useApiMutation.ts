import {
  useMutation,
  type UseMutationOptions,
  useQueryClient,
} from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { getApiError } from '../api/client';

export function useApiMutation<TData, TVariables>(
  options: UseMutationOptions<TData, Error, TVariables> & {
    successMessage?: string;
    invalidateKeys?: readonly (readonly string[])[];
  },
) {
  const queryClient = useQueryClient();
  const { successMessage, invalidateKeys, onSuccess, onError, ...rest } = options;

  return useMutation({
    ...rest,
    onSuccess: (data, variables, context, mutation) => {
      if (successMessage) toast.success(successMessage);
      invalidateKeys?.forEach((key) => queryClient.invalidateQueries({ queryKey: key }));
      onSuccess?.(data, variables, context, mutation);
    },
    onError: (error, variables, context, mutation) => {
      toast.error(getApiError(error));
      onError?.(error, variables, context, mutation);
    },
  });
}
