import { useState } from "react";
import "./LoginPage.css";

function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  function handleSubmit(event: React.SubmitEvent<HTMLFormElement>) {
    event.preventDefault();

    setError("");

    if (username.trim() === "") {
      setError("Vul je gebruikersnaam of e-mailadres in.");
      return;
    }

    if (password === "") {
      setError("Vul je wachtwoord in.");
      return;
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

        <form className="login-form" onSubmit={handleSubmit} noValidate>
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

          <button type="submit">Inloggen</button>
        </form>

        <p className="login-footer">SOC Casemanagementplatform</p>
      </section>
    </main>
  );
}

export default LoginPage;
