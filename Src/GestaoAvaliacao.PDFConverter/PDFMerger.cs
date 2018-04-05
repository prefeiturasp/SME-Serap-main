using EvoPdf;
using GestaoAvaliacao.IPDFConverter;
using System.Collections.Generic;

namespace GestaoAvaliacao.PDFConverter
{
	public class PDFMerger : IPDFMerger
	{
		public byte[] Merge(IEnumerable<string> files)
		{
			Document mergeResultPdfDocument = new Document();

			// Automatically close the merged documents when the document resulted after merge is closed
			mergeResultPdfDocument.AutoCloseAppendedDocs = true;

			// Set license key received after purchase to use the converter in licensed mode
			// Leave it not set to use the converter in demo mode
			mergeResultPdfDocument.LicenseKey = Config.EVOLicenseKey;

			try
			{
				// The documents to merge must remain opened until the PDF document resulted after merge is saved
				// The merged documents can be automatically closed when the document resulted after merge is closed
				// if the AutoCloseAppendedDocs property of the PDF document resulted after merge is set on true like
				// in this demo applcation

				foreach (var item in files)
				{
					Document PdfDocumentToMerge = new Document(item);
					mergeResultPdfDocument.AppendDocument(PdfDocumentToMerge);
				}

				// Save the merge result PDF document in a memory buffer
				return mergeResultPdfDocument.Save();
			}
			finally
			{
				// Close the PDF document resulted after merge
				// When the AutoCloseAppendedDocs property is true the merged PDF documents will also be closed
				mergeResultPdfDocument.Close();
			}
		}
	}
}
