
CREATE TABLE znacky (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        nazev VARCHAR(50) NOT NULL
);


CREATE TABLE auta (
                      id INT AUTO_INCREMENT PRIMARY KEY,
                      znacka_id INT,
                      model VARCHAR(100) NOT NULL,
                      najezd_km FLOAT NOT NULL,
                      je_skladem BOOLEAN DEFAULT TRUE,
                      stav ENUM('Nove', 'Ojete', 'Poskozene') NOT NULL,
                      cena DECIMAL(10, 2) NOT NULL,
                      datum_prijeti DATETIME NOT NULL,
                      FOREIGN KEY (znacka_id) REFERENCES znacky(id)
);


INSERT INTO znacky (nazev) VALUES ('Škoda'), ('Volkswagen'), ('Toyota'), ('BMW');