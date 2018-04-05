DECLARE
	@clientId INT

SELECT @clientId = id FROM IDS_Clients WHERE ClientId = '$ClientIdMvc$'

IF(NOT EXISTS(SELECT * FROM IDS_ClientCorsOrigins AS icco WHERE icco.ClientId = @clientId AND icco.Origin = '$UrlSerap$'))
BEGIN
	INSERT INTO IDS_ClientCorsOrigins (ClientId, Origin) VALUES (@clientId, '$UrlSerap$')
END

IF(NOT EXISTS(SELECT * FROM IDS_ClientRedirectUris AS icru WHERE icru.ClientId = @clientId AND icru.RedirectUri = '$UrlSerapLogin$'))
BEGIN
	INSERT INTO IDS_ClientRedirectUris (ClientId, RedirectUri) VALUES (@clientId, '$UrlSerapLogin$')
END