USE GestaoAvaliacao;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Test' 
               AND COLUMN_NAME = 'ShowInReport')
BEGIN
    ALTER TABLE Test
    ADD ShowInReport BIT DEFAULT 0;
END;