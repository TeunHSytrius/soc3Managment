import type { CreateOrganizationRequest, OrganizationResponse } from '../types/organization';

const BASE_URL = '/api/v1/organizations';

export async function fetchOrganizations(): Promise<OrganizationResponse[]> {
  const response = await fetch(BASE_URL);
  if (!response.ok) {
    throw new Error('Organisaties konden niet worden geladen.');
  }
  const data = await response.json();
  return data as OrganizationResponse[];
}

export async function createOrganization(
  request: CreateOrganizationRequest,
): Promise<OrganizationResponse> {
  const response = await fetch(BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });
  if (!response.ok) {
    const problem = await response.json().catch(() => null);
    throw new Error(problem?.detail ?? 'Organisatie aanmaken mislukt.');
  }
  const data = await response.json();
  return data as OrganizationResponse;
}

export async function deactivateOrganization(id: string): Promise<OrganizationResponse> {
  const response = await fetch(`${BASE_URL}/${id}/deactivate`, { method: 'POST' });
  if (!response.ok) {
    const problem = await response.json().catch(() => null);
    throw new Error(problem?.detail ?? 'Organisatie deactiveren mislukt.');
  }
  const data = await response.json();
  return data as OrganizationResponse;
}
