CREATE TABLE znacky (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nazev VARCHAR(50) NOT NULL
);

CREATE TABLE zamestnanci (
    id INT AUTO_INCREMENT PRIMARY KEY,
    jmeno VARCHAR(50) NOT NULL,
    prijmeni VARCHAR(50) NOT NULL,
    role ENUM('Prodejce', 'Mechanik', 'Manazer') DEFAULT 'Prodejce'
);

CREATE TABLE auta (
    id INT AUTO_INCREMENT PRIMARY KEY,
    znacka_id INT,
    model VARCHAR(100) NOT NULL,
    najezd_km FLOAT NOT NULL,
    je_skladem TINYINT(1) DEFAULT 1,
    stav ENUM('Nove', 'Ojete', 'Poskozene') NOT NULL,
    cena DECIMAL(10, 2) NOT NULL,
    datum_prijeti DATETIME NOT NULL,
    FOREIGN KEY (znacka_id) REFERENCES znacky(id)
);

CREATE TABLE prodeje (
    id INT AUTO_INCREMENT PRIMARY KEY,
    auto_id INT,
    zamestnanec_id INT,
    zakaznik_jmeno VARCHAR(100) NOT NULL,
    prodejni_cena DECIMAL(10, 2) NOT NULL,
    datum_prodeje DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (auto_id) REFERENCES auta(id),
    FOREIGN KEY (zamestnanec_id) REFERENCES zamestnanci(id)
);

CREATE TABLE vybaveni (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nazev VARCHAR(50) NOT NULL
);

CREATE TABLE auto_vybaveni (
    auto_id INT,
    vybaveni_id INT,
    PRIMARY KEY (auto_id, vybaveni_id),
    FOREIGN KEY (auto_id) REFERENCES auta(id) ON DELETE CASCADE,
     FOREIGN KEY (vybaveni_id) REFERENCES vybaveni(id) ON DELETE CASCADE
);

CREATE TABLE servisni_zaznamy (
    id INT AUTO_INCREMENT PRIMARY KEY,
    auto_id INT,
    popis_opravy TEXT NOT NULL,
    datum_servisu DATE NOT NULL,
    cena_opravy DECIMAL(10, 2),
    FOREIGN KEY (auto_id) REFERENCES auta(id) ON DELETE CASCADE
);