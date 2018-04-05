using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.IBusiness
{
	public interface IBookletBusiness
	{
		IEnumerable<Booklet> GetAllByTest(long TestId);
		Booklet GetTestBooklet(long Id);
		Booklet GetBookletByTest(long Id);

		EntityFile SavePdfTest(PDFFilter filter);
		EntityFile SavePdfTest(PDFFilter filter, Guid? EntityId);
		string GetHtmlTest(PDFFilter filter, Guid? EntityId);
		byte[] GetHtmlBytes(PDFFilter filter, Guid? EntityId);
		string GetBookletContent(PDFFilter filter, Guid EntityId);
		EntityFile GetLogo(long ownerId, long parentId);
	}
}
