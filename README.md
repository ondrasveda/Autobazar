# Dokumentace k projektu Autobazar

Tato konzolová aplikace slouží pro správu vozidel, prodejů a servisní historie v autobazaru. Je postavená v C# a běží nad MySQL databází. Celý kód je strukturovaný tak, aby splňoval všechny body zadání (snad).

## Co všechno projekt splňuje

* **Repository Pattern:** Veškerá práce s databází je oddělená v samostatné třídě CarRepository. To znamená, že hlavní logika programu se nestará o to, jak se píší SQL dotazy.
* **MySQL Databáze:** Používám skutečnou relační databázi, žádné SQLite nebo ukládání do textu.
* **Struktura DB:** V databázi je celkem 7 tabulek. Je tam i vazební tabulka pro výbavu aut (vazba M:N).
* **SQL Pohledy (Views):** Jsou tam dva. Jeden počítá výkony prodejců a druhý filtruje historii oprav pro auta, co jsou zrovna na skladě.
* **Práce s více tabulkami:**
    * **Vložení:** Když přidáš auto, zapíše se značka, samotné auto i jeho výbava.
    * **Úprava:** Můžeš změnit název modelu i značky v jednom kroku.
    * **Mazání:** Nastavil jsem kaskádové mazání, takže když smažeš auto, databáze sama vyčistí jeho servisní historii i výbavu.
* **Transakce:** Při prodeji auta se používá transakce. Buď se zapíše prodej i změna stavu auta najednou, nebo se při chybě neprovede nic, aby v datech nebyl zmatek.
* **Agregace a reporty:** Aplikace umí vygenerovat report, který spočítá počty aut a celkové tržby pro každou značku pomocí JOINů a GROUP BY.
* **JSON Import:** Umí to hromadně načíst auta a značky ze souboru auta_import.json.
* **Konfigurační soubor:** Connection string k databázi si aplikace bere z appsettings.json.

## Jak to zprovoznit? (to zvládnete já vám věřim) 

### 1. Stažení projektu
Nejdříve musíte mít kód u sebe v počítači.
* Pokud umíte s Gitem, použijte: `git clone https://github.com/ondrasveda/Autobazar_DB_project.git`
* Pokud ne, stáhněte si projekt z GitHubu jako **ZIP archiv** (tlačítko Code -> Download ZIP) a rozbalte ho do libovolné složky.

### 2. Příprava databáze v MySQL Workbench
Před spuštěním programu musí existovat databáze, se kterou bude komunikovat.
1. Otevřete **MySQL Workbench** a připojte se ke svému lokálnímu serveru (Local instance).
2. V horním menu zvolte **File -> Open SQL Script**.
3. Najděte v projektu složku /Database a v ní soubor s názvem `create_tables.sql` a spusťtě ho v MySQL Workbenchi.
4. V levém panelu (Schemas) klikněte pravým tlačítkem a dejte **Refresh All**. Měli byste vidět databázi `autobazar` a v ní tabulky.

### 3. Propojení aplikace s databází
Aplikace musí vědět, jaké máte heslo k MySQL.
1. V projektu běžte do složky `Configuration` a otevřete soubor `appsettings.json`.
2. Najděte řádek s `DefaultConnection`.
3. Přepište hodnotu `user=root` (pokud máte jiného uživatele) a `password=student` (pokud máte jiné heslo).
4. Soubor uložte (Ctrl+S).

### 4. Spuštění v IDE
1. Otevřete si vývojové prostředí (doporučeno **JetBrains Rider**).
2. Otevřete soubor řešení (soubor s příponou `.sln`).
3. Počkejte, až se načtou všechny balíčky (NuGet packages), hlavně `MySql.Data` a `Microsoft.Extensions.Configuration`.
4. Stiskněte zelenou šipku **Run**.
5. Pokud jste vše nastavili správně, otevře se černé okno s hlavním menu aplikace.
