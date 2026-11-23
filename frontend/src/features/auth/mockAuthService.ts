import type { AuthErrorCode } from "./authErrors";

export async function simulateLoginRequest(
  scenario: AuthErrorCode = "apiUnavailable",
): Promise<AuthErrorCode> {
  if (!import.meta.env.DEV) {
    throw new Error(
      "De testsimulatie is alleen beschikbaar tijdens ontwikkeling.",
    );
  }

  await new Promise<void>((resolve) => {
    setTimeout(resolve, 1200);
  });

  return scenario;
}
