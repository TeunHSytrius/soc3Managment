export interface StartTwoFactorSetupRequest {
  userId: number;
  email: string;
}

export interface TwoFactorSetupResponse {
  otpAuthUri: string;
  manualEntryKey: string;
}
