import type {
  StartTwoFactorSetupRequest,
  TwoFactorSetupResponse,
} from "../types/twoFactor";

const BASE_URL = "/api/v1/auth/2fa";

export async function startTwoFactorSetup(
  request: StartTwoFactorSetupRequest,
): Promise<TwoFactorSetupResponse> {
  const response = await fetch(`${BASE_URL}/setup`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(request),
  });

  if (response.status === 409) {
    throw new Error("Tweestapsverificatie is al ingeschakeld voor dit account.");
  }

  if (!response.ok) {
    throw new Error("Het instellen van tweestapsverificatie is mislukt.");
  }

  return (await response.json()) as TwoFactorSetupResponse;
}
