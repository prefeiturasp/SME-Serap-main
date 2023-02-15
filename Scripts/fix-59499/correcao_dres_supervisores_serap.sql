select ua.* 
from SYS_UsuarioGrupoUA uga
inner join SYS_UnidadeAdministrativa ua on uga.uad_id = ua.uad_id
WHERE uga.gru_id = '66C70452-1A1E-E811-B259-782BCB3D2D76' 

declare @uad_id uniqueidentifier, @novo_uad_id uniqueidentifier,
@gru_id uniqueidentifier = '66C70452-1A1E-E811-B259-782BCB3D2D76',
@uad_codigo varchar(10)
select distinct 
ua.uad_id, 
ua.uad_nome,
ua.uad_codigo,
SUBSTRING(ua.uad_codigo, 0, LEN(ua.uad_codigo) - 1) primeiros_digitos,
SUBSTRING(ua.uad_codigo, LEN(ua.uad_codigo) - 1, LEN(ua.uad_codigo)) dois_ultimos_digitos
into #DADOS
from SYS_UsuarioGrupoUA uga
inner join SYS_UnidadeAdministrativa ua on uga.uad_id = ua.uad_id
WHERE uga.gru_id = @gru_id

select top 1
@uad_id = d.uad_id,
@uad_codigo = d.primeiros_digitos + '00'
from #DADOS d where d.dois_ultimos_digitos = '02'

select @novo_uad_id = ua.uad_id
from SYS_UnidadeAdministrativa ua where ua.uad_codigo = @uad_codigo 

update SYS_UsuarioGrupoUA set uad_id = @novo_uad_id 
where gru_id = @gru_id
and uad_id = @uad_id

drop table #DADOS