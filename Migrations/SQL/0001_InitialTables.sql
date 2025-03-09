-- Initial migration to create the HelloMessages table
CREATE TABLE IF NOT EXISTS HelloMessages (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NULL,
    Message VARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL
);