import { useId, type ReactNode } from 'react';
import { AppMenu } from './AppMenu';
import type { NavigationItem } from './AppMenu';
import '../App.css';

export interface CustomerOption {
  id: string;
  name: string;
}

export interface AppUser {
  displayName: string;
  role: string;
}

interface AppShellProps {
  user: AppUser;
  customer: CustomerOption;
  customerOptions?: CustomerOption[];
  onCustomerChange?: (customerId: string) => void;
  navigationItems: NavigationItem[];
  children: ReactNode;
}

export function AppShell({
  user,
  customer,
  customerOptions = [],
  onCustomerChange,
  navigationItems,
  children,
}: AppShellProps) {
  const customerSelectId = useId();
  const canSwitchCustomer = customerOptions.length > 1 && onCustomerChange !== undefined;

  return (
    <div className="AppShell">
      <header className="AppShell__header">
        <div className="AppShell__topbar">
          <a className="AppShell__brand" href="/" aria-label="SOC3 startpagina">
            <span className="AppShell__brandMark" aria-hidden="true">S3</span>
            <span>SOC3</span>
          </a>
          <AppMenu items={navigationItems} />
        </div>
        <div className="AppShell__context">
          <div className="AppShell__user" aria-label="Ingelogde gebruiker">
            <span className="AppShell__userName">{user.displayName}</span>
            <span className="AppShell__role">{user.role}</span>
          </div>
          <div className="AppShell__customer">
            {canSwitchCustomer ? (
              <>
                <select
                  id={customerSelectId}
                  aria-label="Actieve klant"
                  value={customer.id}
                  onChange={(event) => onCustomerChange(event.currentTarget.value)}
                >
                  {customerOptions.map((option) => (
                    <option key={option.id} value={option.id}>
                      {option.name}
                    </option>
                  ))}
                </select>
              </>
            ) : (
              <span className="AppShell__customerName">{customer.name}</span>
            )}
          </div>
        </div>
      </header>
      <main className="AppShell__content">{children}</main>
    </div>
  );
}
