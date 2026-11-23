export interface CustomerResponse {
  id: string;
  name: string;
  isActive: boolean;
  createdAt: string;
  deactivatedAt: string | null;
}

export interface CreateCustomerRequest {
  name: string;
}
