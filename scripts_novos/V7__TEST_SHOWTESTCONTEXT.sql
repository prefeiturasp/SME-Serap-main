ALTER TABLE TEST ADD ShowTestContext BIT NULL;
UPDATE Test SET ShowTestContext=0;
ALTER TABLE TEST ALTER COLUMN ShowTestContext BIT NOT NULL;