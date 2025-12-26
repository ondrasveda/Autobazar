
INSERT IGNORE INTO znacky (id, nazev) 
VALUES
    (1, 'Škoda'), (2, 'Volkswagen'), (3, 'Toyota'), (4, 'BMW'), (5, 'Audi');


INSERT INTO auta (znacka_id, model, najezd_km, je_skladem, stav, cena, datum_prijeti) 
VALUES
    (1, 'Octavia III', 150000.5, 1, 'Ojete', 250000, '2025-10-10 10:00:00'),
    (1, 'Fabia IV', 1200.0, 1, 'Nove', 420000, '2025-12-01 08:30:00'),
    (2, 'Golf VII', 89000.2, 1, 'Ojete', 310000, '2025-11-15 14:20:00'),
    (3, 'Corolla', 500.8, 1, 'Nove', 650000, '2025-12-20 11:00:00'),
    (4, 'BMW 3', 210000.0, 0, 'Poskozene', 150000, '2025-09-05 16:45:00'),
    (5, 'A4 Avant', 125000.0, 1, 'Ojete', 480000, '2025-12-24 18:00:00');