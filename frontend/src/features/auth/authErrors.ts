export type AuthErrorCode =
  | "invalidCredentials"
  | "temporarilyBlocked"
  | "sessionExpired"
  | "networkError"
  | "apiUnavailable";

const authErrorMessages: Record<AuthErrorCode, string> = {
  invalidCredentials: "Gebruikersnaam of wachtwoord is onjuist.",
  temporarilyBlocked:
    "Inloggen is tijdelijk niet mogelijk. Probeer het later opnieuw.",
  sessionExpired: "Je sessie is verlopen. Log opnieuw in.",
  networkError: "Er is een verbindingsprobleem. Probeer het opnieuw.",
  apiUnavailable:
    "Inloggen is nog niet beschikbaar: de authenticatie-API ontbreekt.",
};

export function getAuthErrorMessage(code: AuthErrorCode): string {
  return authErrorMessages[code];
}
