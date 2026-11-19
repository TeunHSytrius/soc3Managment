import { useState } from 'react';
import './App.css';

function App() {
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <main>
      <header className="Header">
        <a className="Header__brand" href="/" aria-label="SOC home">
          <img src="https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fd1csarkz8obe9u.cloudfront.net%2Fposterpreviews%2Fprofessional-company-logo-logo-design-template-d13176322c3d0a43692b023a894dbd68_e1e24567-94c7-4552-9ce7-c574e053cb7c_screen.png%3Fts%3D1763534820&f=1&nofb=1&ipt=b99e050e5dab0f48489bf45483b0bf78d27cc9a0c06746261d6cfc471aaa4fed&ipo=images" alt="" className="HeaderLogo" />
          <span>SOC</span>
        </a>

        <button
          className="Header__toggle"
          type="button"
          aria-expanded={menuOpen}
          aria-controls="primary-navigation"
          onClick={() => setMenuOpen(!menuOpen)}
        >
          {menuOpen ? 'Close' : 'Menu'}
        </button>

        <nav id="primary-navigation" className={`Header__nav ${menuOpen ? 'Header__nav--open' : ''}`}>
          <a href="/alerts" onClick={() => setMenuOpen(false)}>Alerts</a>
          <a href="/cases" onClick={() => setMenuOpen(false)}>Cases</a>
          <a href="/account" onClick={() => setMenuOpen(false)}>Account</a>
        </nav>
      </header>

      <section className="PageContent">
        <h1>SOC Casemanagementplatform</h1>
        <p>Frontend succesvol gestart.</p>
      </section>
    </main>
  );
}

export default App;