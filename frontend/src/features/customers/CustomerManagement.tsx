import { useEffect, useState } from 'react';
import type { FormEvent } from 'react';
import { createCustomer, deactivateCustomer, fetchCustomers } from '../../services/customers';
import type { CustomerResponse } from '../../types/customer';
import './CustomerManagement.css';

export function CustomerManagement() {
  const [customers, setCustomers] = useState<CustomerResponse[]>([]);
  const [name, setName] = useState('');
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;
    void (async () => {
      try {
        const data = await fetchCustomers();
        if (active) {
          setCustomers(data);
        }
      } catch (err) {
        if (active) {
          setError(err instanceof Error ? err.message : 'Onbekende fout.');
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    })();
    return () => {
      active = false;
    };
  }, []);

  const handleCreate = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const trimmed = name.trim();
    if (!trimmed) {
      return;
    }
    setSubmitting(true);
    setError(null);
    try {
      const created = await createCustomer({ name: trimmed });
      setCustomers((previous) => [created, ...previous]);
      setName('');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Onbekende fout.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeactivate = async (id: string) => {
    setError(null);
    try {
      const updated = await deactivateCustomer(id);
      setCustomers((previous) =>
        previous.map((customer) => (customer.id === updated.id ? updated : customer)),
      );
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Onbekende fout.');
    }
  };

  return (
    <section className="customer-management">
      <h2>Klanten</h2>

      {error !== null && (
        <p className="customer-management__error" role="alert">
          {error}
        </p>
      )}

      <form className="customer-management__form" onSubmit={handleCreate}>
        <label className="customer-management__label" htmlFor="customer-name">
          Naam
        </label>
        <input
          id="customer-name"
          className="customer-management__input"
          type="text"
          value={name}
          onChange={(event) => setName(event.target.value)}
          placeholder="Naam van de organisatie"
          disabled={submitting}
        />
        <button
          className="customer-management__button"
          type="submit"
          disabled={submitting || name.trim() === ''}
        >
          {submitting ? 'Aanmaken…' : 'Klant aanmaken'}
        </button>
      </form>

      {loading ? (
        <p>Klanten laden…</p>
      ) : customers.length === 0 ? (
        <p>Er zijn nog geen klanten.</p>
      ) : (
        <table className="customer-management__table">
          <thead>
            <tr>
              <th>Naam</th>
              <th>Status</th>
              <th>Aangemaakt</th>
              <th>Acties</th>
            </tr>
          </thead>
          <tbody>
            {customers.map((customer) => (
              <tr key={customer.id}>
                <td>{customer.name}</td>
                <td>
                  <span
                    className={
                      customer.isActive
                        ? 'customer-management__badge customer-management__badge--active'
                        : 'customer-management__badge customer-management__badge--inactive'
                    }
                  >
                    {customer.isActive ? 'Actief' : 'Inactief'}
                  </span>
                </td>
                <td>{new Date(customer.createdAt).toLocaleDateString('nl-NL')}</td>
                <td>
                  {customer.isActive && (
                    <button
                      type="button"
                      className="customer-management__button customer-management__button--danger"
                      onClick={() => void handleDeactivate(customer.id)}
                    >
                      Deactiveren
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </section>
  );
}
