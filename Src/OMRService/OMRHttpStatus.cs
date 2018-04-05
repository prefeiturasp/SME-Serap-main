namespace OMRService
{
    public class OMRHttpStatus
	{
		public OMRHttpStatus(int _StatusCode)
		{
			StatusCode = _StatusCode;
			setMessage(StatusCode);
		}

		public int StatusCode { get; set; }

		public string Message { get; private set; }

		public bool isValid()
		{
			return StatusCode == 200;
		}

		public void setMessage(int _StatusCode)
		{
			switch (_StatusCode)
			{
				case 200:
				case 204:
					Message = "Lote enviado para processamento com sucesso.";
					break;
				case 400:
					Message = "Não foi possível atender a solicitação de processamento";
					break;
				case 401:
				case 403:
					Message = "Não foi possível enviar o lote devido ao acesso negado";
					break;
				case 500:
					Message = "Não foi possível enviar o lote devido a um erro interno";
					break;
				default:
					Message = "Erro ao tentar enviar lote para processamento.";
					break;
			}
		}
	}
}
