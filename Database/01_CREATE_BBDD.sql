CREATE DATABASE IF NOT EXISTS fluff_unity_db;
USE fluff_unity_db;

CREATE TABLE difficulties (
    Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Identificador unico de la dificultad',
    Name VARCHAR(20) NOT NULL UNIQUE COMMENT 'Nombre dificultad',
	EnemySpeed FLOAT NOT NULL COMMENT 'Velocidad de movimiento de las pelusas',
	EnemyLifeTime FLOAT NOT NULL COMMENT 'Tiempo de vida de la pelusa en pantalla. Pasado ese tiempo desaparecerá',
	SpawnRate FLOAT NOT NULL COMMENT 'Frecuencia con la que irán apareciendo pelusas en pantalla por segundo',
	AmountEnemies INT NOT NULL COMMENT 'Cantidad máxima de pelusas en pantalla a la vez',
    LogTimestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) COMMENT 'Dificultades del juego';

-- Insertamos niveles por defecto
-- INSERT INTO difficulties (NAME) VALUES ('easy'), ('medium'), ('advanced');
INSERT INTO `fluff_unity_db`.`difficulties` (`Name`, `EnemySpeed`, `EnemyLifeTime`, `SpawnRate`, `AmountEnemies`) VALUES 
	('easy', 3.0, 10.0, 0.1667, 5),
	('medium', 6.0, 7.0, 0.3334, 8),
	('advanced', 12.0, 2.0, 0.6667, 12);
    
CREATE TABLE users (
    Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Identificador unico del usuario',
    Username VARCHAR(50) NOT NULL UNIQUE COMMENT 'Nombre de usuario',
    FirstName VARCHAR(50) COMMENT 'Nombre',
    LastName VARCHAR(150) COMMENT 'Apellidos',
    BirthDate DATE COMMENT 'Fecha de cumpleaños',
    PasswordHash VARCHAR(255) NOT NULL COMMENT 'Contraseña hasehada',
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Fecha de creacion del usuario',
    LogTimestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP 
) COMMENT 'Usuarios que acceden al juego';

CREATE TABLE scores (
    Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Identificador unico de la puntuacion',
    IdUser INT NOT NULL COMMENT 'Usuario al que pertecene',
    IdDifficulty INT NOT NULL COMMENT 'Modo de dificultad sobre la que se ha obtenido la puntuacion',
    RedPoints INT NOT NULL DEFAULT 0 COMMENT 'Puntuacion pelusas rojas',
    BluePoints INT NOT NULL DEFAULT 0 COMMENT 'Puntuacion pelusas azules',
    GreenPoints INT NOT NULL DEFAULT 0 COMMENT 'Puntuacion pelusas verdes',
    YellowPoints INT NOT NULL DEFAULT 0 COMMENT 'Puntuacion pelusas amarillas',
    MissingPoints INT NOT NULL DEFAULT 0 COMMENT 'Puntuacion pelusas perdidas',
    TotalPoints INT NOT NULL DEFAULT 0 COMMENT 'Puntuacion total',
    LogTimestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (IdUser) REFERENCES users(Id) ON DELETE CASCADE,
    FOREIGN KEY (IdDifficulty) REFERENCES difficulties(Id)
) COMMENT 'Puntuaciones que registra un usuario';

CREATE TABLE user_preferences (
    Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Identificador unico de la preferencia de usuario',
    IdUser INT NOT NULL COMMENT 'Usuario al que pertecenece',
    IdDifficulty INT NOT NULL COMMENT 'Dificultad elegida',
    LogTimestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (IdUser) REFERENCES users(Id) ON DELETE CASCADE,
    FOREIGN KEY (IdDifficulty) REFERENCES difficulties(Id)
) COMMENT 'Preferencias de juego del usuario';