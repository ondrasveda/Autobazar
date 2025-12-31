INSERT INTO znacky (nazev) VALUES ('Škoda'), ('Volkswagen'), ('BMW'), ('Audi'), ('Hyundai');

INSERT INTO zamestnanci (jmeno, prijmeni, role) 
VALUES
    ('Jan', 'Novák', 'Manazer'),
    ('Petr', 'Svoboda', 'Prodejce'),
    ('Karel', 'Dvořák', 'Mechanik');                                             

INSERT INTO auta (znacka_id, model, najezd_km, je_skladem, stav, cena, datum_prijeti)
VALUES
    (1, 'Octavia III', 152000.5, 1, 'Ojete', 250000, '2025-12-01 10:00:00'),
    (2, 'Golf VII', 89000.0, 1, 'Ojete', 320000, '2025-12-05 14:20:00'),
    (3, 'M3', 1500.2, 1, 'Nove', 2100000, '2025-12-10 09:00:00'),
    (1, 'Fabia IV', 10.0, 1, 'Nove', 450000, '2025-12-15 11:30:00'),
    (4, 'A6', 210000.8, 0, 'Ojete', 550000, '2025-11-20 16:45:00');
                                                                                          

INSERT INTO vybaveni (nazev) VALUES ('Klimatizace'), ('Tempomat'), ('Vyhřívaná sedadla'), ('LED světlomety'), ('Parkovací senzory');

INSERT INTO auto_vybaveni (auto_id, vybaveni_id) VALUES (1, 1), (1, 2);
INSERT INTO auto_vybaveni (auto_id, vybaveni_id) VALUES (2, 1), (2, 5);
INSERT INTO auto_vybaveni (auto_id, vybaveni_id) VALUES (3, 1), (3, 2), (3, 3), (3, 4), (3, 5);

INSERT INTO prodeje (auto_id, zamestnanec_id, zakaznik_jmeno, prodejni_cena, datum_prodeje)
VALUES (5, 2, 'Pepa Zdepa', 540000, '2025-12-28 11:00:00');

INSERT INTO servisni_zaznamy (auto_id, popis_opravy, datum_servisu, cena_opravy)
VALUES (1, 'Výměna oleje a filtrů', '2025-12-02', 4500);


INSERT INTO servisni_zaznamy (auto_id, popis_opravy, datum_servisu, cena_opravy) 
VALUES
    (1, 'Výměna rozvodů a vodní pumpy', '2025-10-15', 12500.00),
    (1, 'Pravidelná výměna oleje a filtrů', '2025-12-02', 4200.50),
    (2, 'Výměna brzdových destiček (přední)', '2025-11-20', 3800.00),
    (3, 'Garanční prohlídka po 1000 km', '2025-12-28', 0.00);