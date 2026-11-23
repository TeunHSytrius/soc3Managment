import { useEffect, useState } from 'react';
import type { FormEvent } from 'react';
import { createOrganization, deactivateOrganization, fetchOrganizations } from '../../services/organizations';
import type { OrganizationResponse } from '../../types/organization';
import './OrganizationManagement.css';

export function OrganizationManagement() {
  const [organizations, setOrganizations] = useState<OrganizationResponse[]>([]);
  const [name, setName] = useState('');
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;
    void (async () => {
      try {
        const data = await fetchOrganizations();
        if (active) {
          setOrganizations(data);
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
      const created = await createOrganization({ name: trimmed });
      setOrganizations((previous) => [created, ...previous]);
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
      const updated = await deactivateOrganization(id);
      setOrganizations((previous) =>
        previous.map((organization) => (organization.id === updated.id ? updated : organization)),
      );
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Onbekende fout.');
    }
  };

  return (
    <section className="organization-management">
      <h2>Organisaties</h2>

      {error !== null && (
        <p className="organization-management__error" role="alert">
          {error}
        </p>
      )}

      <form className="organization-management__form" onSubmit={handleCreate}>
        <label className="organization-management__label" htmlFor="organization-name">
          Naam
        </label>
        <input
          id="organization-name"
          className="organization-management__input"
          type="text"
          value={name}
          onChange={(event) => setName(event.target.value)}
          placeholder="Naam van de organisatie"
          disabled={submitting}
        />
        <button
          className="organization-management__button"
          type="submit"
          disabled={submitting || name.trim() === ''}
        >
          {submitting ? 'Aanmaken…' : 'Organisatie aanmaken'}
        </button>
      </form>

      {loading ? (
        <p>Organisaties laden…</p>
      ) : organizations.length === 0 ? (
        <p>Er zijn nog geen organisaties.</p>
      ) : (
        <table className="organization-management__table">
          <thead>
            <tr>
              <th>Naam</th>
              <th>Status</th>
              <th>Aangemaakt</th>
              <th>Acties</th>
            </tr>
          </thead>
          <tbody>
            {organizations.map((organization) => (
              <tr key={organization.id}>
                <td>{organization.name}</td>
                <td>
                  <span
                    className={
                      organization.isActive
                        ? 'organization-management__badge organization-management__badge--active'
                        : 'organization-management__badge organization-management__badge--inactive'
                    }
                  >
                    {organization.isActive ? 'Actief' : 'Inactief'}
                  </span>
                </td>
                <td>{new Date(organization.createdAt).toLocaleDateString('nl-NL')}</td>
                <td>
                  {organization.isActive && (
                    <button
                      type="button"
                      className="organization-management__button organization-management__button--danger"
                      onClick={() => void handleDeactivate(organization.id)}
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
