UPDATE Parameter
SET Value = '$StoragePath$'
WHERE
	[Key] = 'STORAGE_PATH'
	AND [State] = 1


UPDATE Parameter
SET Value = '$VirtualPath$'
WHERE
	[Key] = 'VIRTUAL_PATH'
	AND [State] = 1