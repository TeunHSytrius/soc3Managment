
const tabel = document.querySelector("table");
const naamInput = document.getElementById("klantNaam");
const toevoegenKnop = document.querySelector(".formulier-knop");

// Klant toevoegen
toevoegenKnop.addEventListener("click", function () {
    const naam = naamInput.value.trim();

    if (naam === "") {
        alert("Vul een klantnaam in!");
        return;
    }

    const rij = tabel.insertRow();

    rij.innerHTML = `
        <td></td>
        <td><span class="status actief">Actief</span></td>
        <td><button class="knop klein">Deactiveren</button></td>
    `;

    rij.cells[0].textContent = naam;
    naamInput.value = "";
});


// Klant deactiveren
tabel.addEventListener("click", function (event) {
    if (event.target.textContent !== "Deactiveren") {
        return;
    }

    // Vraag om een reden
    const reden = prompt("Waarom wil je deze klant deactiveren?");

    // Controleer of een reden is ingevuld
    if (reden === null) {
        return;
    }

    if (reden.trim() === "") {
        alert("Je moet een reden invullen!");
        return;
    }

    // Vraag om bevestiging
    const bevestiging = confirm(
        "Weet je zeker dat je deze klant wilt deactiveren?\n\n" +
        "Reden: " + reden + "\n\n" +
        "Nieuwe alerts worden geweigerd en bestaande gegevens blijven behouden."
    );

    if (bevestiging) {
        const rij = event.target.closest("tr");
        const status = rij.querySelector(".status");

        status.textContent = "Gedeactiveerd";
        status.classList.remove("actief");
        status.classList.add("inactief");

        event.target.textContent = "Gedeactiveerd";
        event.target.disabled = true;

        alert("Klant is gedeactiveerd.");
    }
});

