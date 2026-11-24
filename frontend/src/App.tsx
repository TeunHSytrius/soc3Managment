import { useState } from 'react';
import { AppShell } from './components/AppShell';
import type { CustomerOption } from './components/AppShell';
import { EmptyState } from './components/PageStates';
import { PageHeader } from './components/PageHeader';

const demoCustomers: CustomerOption[] = [
  { id: 'demo-a', name: 'Voorbeeldklant A' },
  { id: 'demo-b', name: 'Voorbeeldklant B' },
];

const navigationItems = [
  { label: 'Alerts', href: '/alerts' },
  { label: 'Cases', href: '/cases' },
];

function App() {
  const [activeCustomer, setActiveCustomer] = useState(demoCustomers[0]);

  function handleCustomerChange(customerId: string) {
    const customer = demoCustomers.find((item) => item.id === customerId);

    if (!customer) {
      throw new Error(`Unknown demo customer: ${customerId}`);
    }

    setActiveCustomer(customer);
  }

  return (
    <AppShell
      user={{ displayName: 'Voorbeeldgebruiker', role: 'SOC-analist' }}
      customer={activeCustomer}
      customerOptions={demoCustomers}
      onCustomerChange={handleCustomerChange}
      navigationItems={navigationItems}
    >
      <PageHeader
        title="SOC3"
        description="Gedeelde basisinterface voor het SOC-platform."
      />
      <EmptyState
        title="De schermen worden per story opgebouwd"
        description="Dit is een voorbeeldweergave. Gebruiker, rol en klantcontext zijn testgegevens; er is nog geen router, authenticatie of API-koppeling."
      />
    </AppShell>
  );
}

export default App;
