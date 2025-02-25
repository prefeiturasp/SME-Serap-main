USE GestaoAvaliacao;

UPDATE Test
SET ShowInReport = 0
WHERE ShowInReport IS NULL;