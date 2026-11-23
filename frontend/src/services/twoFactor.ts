import type {
  EnableTwoFactorRequest,
  EnableTwoFactorResponse,
  StartTwoFactorSetupRequest,
  TwoFactorSetupResponse,
} from "../types/twoFactor";

const BASE_URL = "/api/v1/auth/2fa";

function postJson(path: string, body: unknown): Promise<Response> {
  return fetch(`${BASE_URL}${path}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });
}

export async function startTwoFactorSetup(
  request: StartTwoFactorSetupRequest,
): Promise<TwoFactorSetupResponse> {
  const response = await postJson("/setup", request);

  if (response.status === 409) {
    throw new Error("Tweestapsverificatie is al ingeschakeld voor dit account.");
  }

  if (!response.ok) {
    throw new Error("Het instellen van tweestapsverificatie is mislukt.");
  }

  return (await response.json()) as TwoFactorSetupResponse;
}

export async function enableTwoFactor(
  request: EnableTwoFactorRequest,
): Promise<EnableTwoFactorResponse> {
  const response = await postJson("/enable", request);

  if (response.status === 400) {
    throw new Error("De code is onjuist of verlopen. Probeer de nieuwste code uit de app.");
  }

  if (response.status === 409) {
    throw new Error("Tweestapsverificatie is al ingeschakeld voor dit account.");
  }

  if (!response.ok) {
    throw new Error("Het inschakelen van tweestapsverificatie is mislukt.");
  }

  return (await response.json()) as EnableTwoFactorResponse;
}
