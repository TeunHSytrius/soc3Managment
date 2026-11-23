export interface StartTwoFactorSetupRequest {
  userId: number;
  email: string;
}

export interface TwoFactorSetupResponse {
  otpAuthUri: string;
  manualEntryKey: string;
}

export interface EnableTwoFactorRequest {
  userId: number;
  code: string;
}

export interface EnableTwoFactorResponse {
  recoveryCodes: string[];
}
