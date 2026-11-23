import { useState } from "react";
import type { FormEvent } from "react";
import { QRCodeSVG } from "qrcode.react";
import { startTwoFactorSetup } from "../../services/twoFactor";
import type { TwoFactorSetupResponse } from "../../types/twoFactor";
import "./TwoFactorSetup.css";

// TODO: remove the userId and email fields once login exists; use the logged-in user instead.
function TwoFactorSetup() {
  const [userId, setUserId] = useState("1");
  const [email, setEmail] = useState("");
  const [setup, setSetup] = useState<TwoFactorSetupResponse | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setIsLoading(true);

    try {
      const response = await startTwoFactorSetup({
        userId: Number(userId),
        email,
      });
      setSetup(response);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Er ging iets mis.");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <section className="two-factor-setup">
      <h2>Tweestapsverificatie instellen</h2>

      {setup === null ? (
        <form className="two-factor-form" onSubmit={handleSubmit}>
          <label htmlFor="two-factor-user-id">Gebruikers-ID (tijdelijk)</label>
          <input
            id="two-factor-user-id"
            type="number"
            min="1"
            value={userId}
            onChange={(event) => setUserId(event.target.value)}
            required
          />

          <label htmlFor="two-factor-email">E-mailadres</label>
          <input
            id="two-factor-email"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
          />

          <button type="submit" disabled={isLoading}>
            {isLoading ? "Bezig..." : "QR-code maken"}
          </button>
        </form>
      ) : (
        <div className="two-factor-result">
          <ol>
            <li>Open Microsoft Authenticator op je telefoon.</li>
            <li>
              Tik op <strong>+</strong> en kies{" "}
              <strong>Ander account (Google, Facebook, enz.)</strong>.
            </li>
            <li>Scan de QR-code hieronder.</li>
          </ol>

          <div className="two-factor-qr">
            <QRCodeSVG
              value={setup.otpAuthUri}
              size={200}
              bgColor="#ffffff"
              fgColor="#000000"
            />
          </div>

          <p>Lukt scannen niet? Voer deze sleutel handmatig in:</p>
          <code className="two-factor-key">{setup.manualEntryKey}</code>
        </div>
      )}

      {error !== null && (
        <p className="two-factor-error" role="alert">
          {error}
        </p>
      )}
    </section>
  );
}

export default TwoFactorSetup;
