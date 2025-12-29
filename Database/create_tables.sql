
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

CREATE TABLE prodeje (
                         id INT AUTO_INCREMENT PRIMARY KEY,
                         auto_id INT,
                         zakaznik_jmeno VARCHAR(100) NOT NULL,
                         datum_prodeje DATETIME DEFAULT CURRENT_TIMESTAMP,
                         prodejni_cena DECIMAL(10, 2),
                         FOREIGN KEY (auto_id) REFERENCES auta(id)
);


