import { useState } from 'react';

export interface NavigationItem {
  label: string;
  href: string;
}

interface AppMenuProps {
  items: NavigationItem[];
}

function isCurrentPage(href: string, pathname: string) {
  return pathname === href || (href !== '/' && pathname.startsWith(`${href}/`));
}

export function AppMenu({ items }: AppMenuProps) {
  const [menuOpen, setMenuOpen] = useState(false);
  const pathname = window.location.pathname;

  return (
    <>
      <button
        className="AppShell__mobileToggle"
        type="button"
        aria-expanded={menuOpen}
        aria-controls="primary-navigation"
        onClick={() => setMenuOpen((open) => !open)}
      >
        {menuOpen ? 'Menu sluiten' : 'Menu openen'}
      </button>
      <nav
        id="primary-navigation"
        className={`AppMenu${menuOpen ? ' AppMenu--open' : ''}`}
        aria-label="Hoofdnavigatie"
      >
        {items.map((item) => {
          const current = isCurrentPage(item.href, pathname);

          return (
            <a
              key={item.href}
              className="AppMenu__link"
              href={item.href}
              aria-current={current ? 'page' : undefined}
              onClick={() => setMenuOpen(false)}
            >
              {item.label}
            </a>
          );
        })}
      </nav>
    </>
  );
}
