export interface OrganizationResponse {
  id: string;
  name: string;
  isActive: boolean;
  createdAt: string;
  deactivatedAt: string | null;
}

export interface CreateOrganizationRequest {
  name: string;
}
