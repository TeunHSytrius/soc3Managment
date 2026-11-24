import { useId, type ReactNode } from 'react';
import './PageStates.css';

interface PageStateProps {
  title: string;
  description?: string;
  children?: ReactNode;
}

interface ErrorStateProps extends PageStateProps {
  onRetry?: () => void;
}

export function LoadingState({ label = 'Laden...' }: { label?: string }) {
  return (
    <div className="PageState PageState--loading" role="status" aria-live="polite">
      <span className="PageState__spinner" aria-hidden="true" />
      <span>{label}</span>
    </div>
  );
}

export function EmptyState({ title, description, children }: PageStateProps) {
  const titleId = useId();

  return (
    <section className="PageState" aria-labelledby={titleId}>
      <h2 id={titleId}>{title}</h2>
      {description && <p>{description}</p>}
      {children}
    </section>
  );
}

export function ErrorState({ title, description, children, onRetry }: ErrorStateProps) {
  return (
    <section className="PageState PageState--error" role="alert">
      <h2>{title}</h2>
      {description && <p>{description}</p>}
      {onRetry && (
        <button className="PageState__button" type="button" onClick={onRetry}>
          Opnieuw proberen
        </button>
      )}
      {children}
    </section>
  );
}

export function AccessDenied({
  title = 'Geen toegang',
  description = 'Je hebt geen toegang tot deze pagina.',
  children,
}: Partial<PageStateProps>) {
  return (
    <section className="PageState PageState--denied" role="alert">
      <h2>{title}</h2>
      <p>{description}</p>
      {children}
    </section>
  );
}
