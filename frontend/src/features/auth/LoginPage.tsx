import { useRef, useState } from "react";
import { getAuthErrorMessage } from "./authErrors";
import { simulateLoginRequest } from "./mockAuthService";
import "./LoginPage.css";

function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const requestInProgress = useRef(false);

  async function handleSubmit(event: React.SubmitEvent<HTMLFormElement>) {
    event.preventDefault();

    if (requestInProgress.current) {
      return;
    }

    setError("");

    if (username.trim() === "") {
      setError("Vul je gebruikersnaam of e-mailadres in.");
      return;
    }

    if (password === "") {
      setError("Vul je wachtwoord in.");
      return;
    }

    requestInProgress.current = true;
    setIsLoading(true);

    try {
      const errorCode = await simulateLoginRequest();

      setError(getAuthErrorMessage(errorCode));
    } catch {
      setError(getAuthErrorMessage("networkError"));
    } finally {
      requestInProgress.current = false;
      setIsLoading(false);
    }
  }

  return (
    <main className="login-page">
      <header className="login-header">
        <div className="login-brand">
          <span className="login-brand-icon" aria-hidden="true">
            SOC
          </span>
          <span>SOC Management</span>
        </div>
        <span className="login-header-text">SOC-platform</span>
      </header>

      <section className="login-content">
        <div className="login-intro">
          <h1>Inloggen</h1>
          <p>Log in om beveiligingsmeldingen en onderzoeken te beheren.</p>
        </div>

        <form
          className="login-form"
          onSubmit={handleSubmit}
          noValidate
          aria-busy={isLoading}
        >
          <div className="login-field">
            <label htmlFor="username">Gebruikersnaam of e-mailadres</label>
            <input
              id="username"
              name="username"
              type="text"
              autoComplete="username"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
            />
          </div>

          <div className="login-field">
            <label htmlFor="password">Wachtwoord</label>
            <input
              id="password"
              name="password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
            />
          </div>

          {error && (
            <p className="login-error" role="alert">
              {error}
            </p>
          )}

          <button type="submit" disabled={isLoading}>
            {isLoading ? "Bezig met inloggen..." : "Inloggen"}
          </button>
        </form>

        <p className="login-footer">SOC Casemanagementplatform</p>
      </section>
    </main>
  );
}

export default LoginPage;
