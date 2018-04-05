using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business.Adapters
{
    public static class ItemModelAdapter
	{
		public static List<Answer> ToAnswer(List<ItemModelDTO> itemModel)
		{
			var answerList = new List<Answer>();
			foreach (var item in itemModel)
			{
				var answer = new Answer()
				{
					AnswerChoice = item.AlternativeId,
					Item_Id = item.Id,
					Empty = item.EmptyAlternative,
					StrikeThrough = item.DuplicateAlternative,
                    Automatic = true
                };
				answerList.Add(answer);
			}
			return answerList;
		}
	}
}
