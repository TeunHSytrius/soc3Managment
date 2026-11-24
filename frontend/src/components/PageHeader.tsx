import type { ReactNode } from 'react';

interface PageHeaderProps {
  title: string;
  description?: string;
  breadcrumbs?: string[];
  actions?: ReactNode;
}

export function PageHeader({
  title,
  description,
  breadcrumbs = [],
  actions,
}: PageHeaderProps) {
  return (
    <header className="PageHeader">
      {breadcrumbs.length > 0 && (
        <nav className="PageHeader__breadcrumbs" aria-label="Broodkruimelnavigatie">
          {breadcrumbs.map((breadcrumb) => (
            <span className="PageHeader__breadcrumb" key={breadcrumb}>
              {breadcrumb}
            </span>
          ))}
        </nav>
      )}
      <div className="PageHeader__row">
        <div>
          <h1 className="PageHeader__title">{title}</h1>
          {description && <p className="PageHeader__description">{description}</p>}
        </div>
        {actions && <div className="PageHeader__actions">{actions}</div>}
      </div>
    </header>
  );
}
