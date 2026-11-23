import type { CreateCustomerRequest, CustomerResponse } from '../types/customer';

const BASE_URL = '/api/v1/customers';

export async function fetchCustomers(): Promise<CustomerResponse[]> {
  const response = await fetch(BASE_URL);
  if (!response.ok) {
    throw new Error('Klanten konden niet worden geladen.');
  }
  const data = await response.json();
  return data as CustomerResponse[];
}

export async function createCustomer(request: CreateCustomerRequest): Promise<CustomerResponse> {
  const response = await fetch(BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });
  if (!response.ok) {
    const problem = await response.json().catch(() => null);
    throw new Error(problem?.detail ?? 'Klant aanmaken mislukt.');
  }
  const data = await response.json();
  return data as CustomerResponse;
}

export async function deactivateCustomer(id: string): Promise<CustomerResponse> {
  const response = await fetch(`${BASE_URL}/${id}/deactivate`, { method: 'POST' });
  if (!response.ok) {
    const problem = await response.json().catch(() => null);
    throw new Error(problem?.detail ?? 'Klant deactiveren mislukt.');
  }
  const data = await response.json();
  return data as CustomerResponse;
}
