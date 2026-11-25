
import { useState, type FormEvent } from 'react';
import './klantenbeheer.css';

type Klant = {
  id: number;
  naam: string;
  actief: boolean;
  reden?: string;
};

function Klantenbeheer() {
  const [klanten, setKlanten] = useState<Klant[]>([
    { id: 1, naam: 'Bedrijf A', actief: true },
    { id: 2, naam: 'Bedrijf B', actief: true },
    { id: 3, naam: 'Bedrijf C', actief: false },
  ]);

  const [naam, setNaam] = useState('');

  function klantToevoegen(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (naam.trim() === '') {
      alert('Vul een klantnaam in!');
      return;
    }

    const nieuweKlant: Klant = {
      id: Date.now(),
      naam: naam.trim(),
      actief: true,
    };

    setKlanten([...klanten, nieuweKlant]);
    setNaam('');
  }

  function klantDeactiveren(id: number) {
    const reden = prompt('Waarom wil je deze klant deactiveren?');

    if (reden === null) {
      return;
    }

    if (reden.trim() === '') {
      alert('Je moet een reden invullen!');
      return;
    }

    const bevestiging = confirm(
      'Weet je zeker dat je deze klant wilt deactiveren?\n\n' +
      'Nieuwe alerts worden geweigerd en bestaande gegevens blijven behouden.'
    );

    if (bevestiging) {
      setKlanten(klanten.map((klant) =>
        klant.id === id
          ? { ...klant, actief: false, reden: reden.trim() }
          : klant
      ));
    }
  }

  return (
    <div className="klantenbeheer">
      <div className="indeling">

        <section className="kaart">
          <h2>Klantenoverzicht</h2>

          <div className="tabel-scroll">
            <table>
              <thead>
                <tr>
                  <th>Naam</th>
                  <th>Status</th>
                  <th>Actie</th>
                </tr>
              </thead>

              <tbody>
                {klanten.map((klant) => (
                  <tr key={klant.id}>
                    <td>{klant.naam}</td>

                    <td>
                      <span className={
                        klant.actief ? 'status actief' : 'status inactief'
                      }>
                        {klant.actief ? 'Actief' : 'Gedeactiveerd'}
                      </span>
                    </td>

                    <td>
                      <button
                        className="knop klein"
                        disabled={!klant.actief}
                        onClick={() => klantDeactiveren(klant.id)}
                      >
                        {klant.actief ? 'Deactiveren' : 'Gedeactiveerd'}
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>

        <section className="kaart">
          <h2>Nieuwe klant</h2>
          <p className="uitleg">Voeg een nieuwe klantorganisatie toe.</p>

          <form onSubmit={klantToevoegen}>
            <label htmlFor="klantNaam">Naam klantorganisatie *</label>

            <input
              id="klantNaam"
              type="text"
              placeholder="Naam klant"
              value={naam}
              onChange={(event) => setNaam(event.target.value)}
              required
            />

            <button type="submit" className="knop primair formulier-knop">
              Klant aanmaken
            </button>
          </form>
        </section>

      </div>
    </div>
  );
}

export default Klantenbeheer;
