import { useState } from "react";
import type { FormEvent } from "react";
import { QRCodeSVG } from "qrcode.react";
import { enableTwoFactor, startTwoFactorSetup } from "../../services/twoFactor";
import type { TwoFactorSetupResponse } from "../../types/twoFactor";
import "./TwoFactorSetup.css";

type Step = "start" | "scan" | "done";

// TODO: remove the userId and email fields once login exists; use the logged-in user instead.
function TwoFactorSetup() {
  const [step, setStep] = useState<Step>("start");
  const [userId, setUserId] = useState("1");
  const [email, setEmail] = useState("");
  const [setup, setSetup] = useState<TwoFactorSetupResponse | null>(null);
  const [code, setCode] = useState("");
  const [recoveryCodes, setRecoveryCodes] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  async function runRequest(request: () => Promise<void>) {
    setError(null);
    setIsLoading(true);

    try {
      await request();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Er ging iets mis.");
    } finally {
      setIsLoading(false);
    }
  }

  function handleStart(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    void runRequest(async () => {
      const response = await startTwoFactorSetup({ userId: Number(userId), email });
      setSetup(response);
      setStep("scan");
    });
  }

  function handleEnable(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    void runRequest(async () => {
      const response = await enableTwoFactor({ userId: Number(userId), code });
      setRecoveryCodes(response.recoveryCodes);
      setStep("done");
    });
  }

  return (
    <section className="two-factor-setup">
      <h2>Tweestapsverificatie instellen</h2>

      {step === "start" && (
        <form className="two-factor-form" onSubmit={handleStart}>
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
      )}

      {step === "scan" && setup !== null && (
        <div className="two-factor-result">
          <ol>
            <li>Open Microsoft Authenticator op je telefoon.</li>
            <li>
              Tik op <strong>+</strong> en kies{" "}
              <strong>Ander account (Google, Facebook, enz.)</strong>.
            </li>
            <li>Scan de QR-code hieronder.</li>
            <li>Vul de 6-cijferige code uit de app in.</li>
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

          <form className="two-factor-form" onSubmit={handleEnable}>
            <label htmlFor="two-factor-code">Code uit de app</label>
            <input
              id="two-factor-code"
              inputMode="numeric"
              autoComplete="one-time-code"
              pattern="[0-9]{6}"
              maxLength={6}
              placeholder="123456"
              value={code}
              onChange={(event) => setCode(event.target.value.replace(/\D/g, ""))}
              required
            />

            <button type="submit" disabled={isLoading || code.length !== 6}>
              {isLoading ? "Bezig..." : "Activeren"}
            </button>
          </form>
        </div>
      )}

      {step === "done" && (
        <div className="two-factor-result">
          <p className="two-factor-success">Tweestapsverificatie is ingeschakeld.</p>

          <p>
            <strong>Bewaar deze herstelcodes op een veilige plek.</strong> Ben je je telefoon
            kwijt, dan kun je met één van deze codes toch inloggen. Elke code werkt maar één
            keer en je ziet ze hierna niet meer terug.
          </p>

          <ul className="two-factor-recovery-codes">
            {recoveryCodes.map((recoveryCode) => (
              <li key={recoveryCode}>
                <code>{recoveryCode}</code>
              </li>
            ))}
          </ul>

          <button type="button" onClick={() => void navigator.clipboard.writeText(recoveryCodes.join("\n"))}>
            Kopieer codes
          </button>
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
