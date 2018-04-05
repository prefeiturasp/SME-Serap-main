using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class ModelTestRepository : ConnectionReadOnly, IModelTestRepository
	{
		#region ReadOnly
		public ModelTest Get(long id)
		{
			StringBuilder sql = new StringBuilder();
			sql.Append("SELECT m.Id,m.Description,m.DefaultModel,m.ShowCoverPage,m.ShowBorder,m.LogoHeaderPosition,m.LogoHeaderWaterMark,m.MessageHeaderPosition,m.MessageHeader, ");
            sql.Append("m.MessageHeaderWaterMark,m.ShowLineBelowHeader,m.ShowItemLine,m.FileHeader_Id,m.LogoFooterPosition,m.LogoFooterWaterMark,m.MessageFooterPosition,m.MessageFooter, ");
			sql.Append("m.MessageFooterWaterMark,m.ShowLineAboveFooter,m.FileFooter_Id,m.ShowSchool,m.ShowStudentName,m.ShowTeacherName,m.ShowClassName,m.ShowStudentNumber, ");
			sql.Append("m.ShowDate,m.ShowLineBelowStudentInformation,m.CoverPageText,m.ShowStudentInformationsOnCoverPage,m.ShowHeaderOnCoverPage,m.ShowFooterOnCoverPage, ");
			sql.Append("m.CreateDate,m.UpdateDate,m.State,m.EntityId,m.ShowBorderOnCoverPage,m.LogoHeaderSize,m.ShowMessageHeader,m.LogoFooterSize,m.ShowMessageFooter, ");
			sql.Append("m.ShowLogoHeader,m.ShowLogoFooter,m.HeaderHtml,m.FooterHtml,m.StudentInformationHtml, fHeader.Id, fHeader.Name, fHeader.Path, fHeader.CreateDate, fHeader.UpdateDate, ");
			sql.Append("fHeader.State, fHeader.ContentType, fHeader.OwnerId, fHeader.OwnerType, fHeader.ParentOwnerId,fFooter.Id, fFooter.Name, fFooter.Path, fFooter.CreateDate, ");
			sql.Append("fFooter.UpdateDate, fFooter.State, fFooter.ContentType, fFooter.OwnerId, fFooter.OwnerType, fFooter.ParentOwnerId, ");
			sql.Append("fRedactor.Id, fRedactor.Name, fRedactor.Path, fRedactor.CreateDate, ");
			sql.Append("fRedactor.UpdateDate, fRedactor.State, fRedactor.ContentType, fRedactor.OwnerId, fRedactor.OwnerType, fRedactor.ParentOwnerId ");
			sql.Append("FROM dbo.ModelTest m (NOLOCK) ");
			sql.Append("LEFT JOIN [File] fHeader (NOLOCK) ON m.FileHeader_Id = fHeader.Id ");
			sql.Append("LEFT JOIN [File] fFooter (NOLOCK) ON m.FileFooter_Id = fFooter.Id ");
            sql.Append("LEFT JOIN [File] fRedactor (NOLOCK) ON m.Id = fRedactor.OwnerId AND fRedactor.Id <> ISNULL(fFooter.Id, 0) AND fRedactor.Id <> ISNULL(fHeader.Id, 0) AND fRedactor.OwnerType IN (@OwnerType1,@OwnerType2) ");
			sql.Append("WHERE m.Id = @id And m.State = @state ");

			var lookup = new Dictionary<long, ModelTest>();

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				cn.Query<ModelTest, File, File, File, ModelTest>(sql.ToString(),
					(modelTest, fHeader, fFooter, fRedactor) =>
					{
						ModelTest found;
						if (!lookup.TryGetValue(modelTest.Id, out found))
						{
							modelTest.LogoHeader = fHeader;
							modelTest.LogoFooter = fFooter;
							modelTest.Files = new List<File>();
							lookup.Add(modelTest.Id, modelTest);

							found = modelTest;
						}
						if(fRedactor != null && fRedactor.Id > 0)
							found.Files.Add(fRedactor);

						return found;

                    }, new { id = id, state = Convert.ToByte(EnumState.ativo), OwnerType1 = (byte)EnumFileType.ModelTestHeader, OwnerType2 = (byte)EnumFileType.ModelTestFooter });

				return lookup.Values.FirstOrDefault();
			}
		}

		public IEnumerable<ModelTest> Search(ref Pager pager, Guid EntityId, string search)
		{
			StringBuilder sql = new StringBuilder("WITH NumberedModelTest AS ");
			sql.Append("( ");
			sql.Append("SELECT Id, Description, DefaultModel, ");
			sql.Append("ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber ");
			sql.Append("FROM ModelTest (NOLOCK) ");
			sql.Append("WHERE State = @state AND EntityId = @EntityId ");
			if (!string.IsNullOrEmpty(search))
				sql.Append(" AND Description LIKE '%' + @search + '%'");
			sql.Append(") ");

			sql.Append("SELECT Id, Description, DefaultModel ");
			sql.Append("FROM NumberedModelTest ");
			sql.Append("WHERE RowNumber > ( @pageSize * @page ) AND RowNumber <= ( ( @page + 1 ) * @pageSize )");
			sql.Append(" ORDER BY RowNumber");

			

			var countSql = new StringBuilder("SELECT COUNT(id) ");
			countSql.Append("FROM ModelTest (NOLOCK) ");
			countSql.Append("WHERE State = @state AND EntityId = @EntityId");


			if (!string.IsNullOrEmpty(search))
				countSql.Append(" AND Description LIKE '%' + @search + '%'");



			using (IDbConnection cn = Connection)
			{
				cn.Open();

				var modelTest = cn.Query<ModelTest>(sql.ToString(), new { state = (Byte)EnumState.ativo, EntityId = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage, search = search });
				var count = (int)cn.ExecuteScalar(countSql.ToString(), new { state = (Byte)EnumState.ativo, EntityId = EntityId, search = search });

				pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));
				pager.SetTotalItens(count);

				return modelTest;
			}
		}

		public bool ExistsAnotherDefaultModel(Guid EntityId, long Id)
		{
			StringBuilder sql = new StringBuilder("SELECT Count(Id) ");
			sql.Append("FROM dbo.ModelTest (NOLOCK) ");
			sql.Append("WHERE State = @state AND EntityId = @EntityId AND Id <> @id AND DefaultModel = 1");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return (int)cn.ExecuteScalar(sql.ToString(), new { state = (Byte)EnumState.ativo, EntityId = EntityId, id = Id  }) > 0;
			}
		}

		public IEnumerable<ModelTest> FindSimple(Guid EntityId)
		{
			StringBuilder sql = new StringBuilder("SELECT Id, Description ");
			sql.Append("FROM ModelTest (NOLOCK) ");
			sql.Append("WHERE EntityId = @EntityId AND State = @State ");
			sql.Append("ORDER BY DefaultModel DESC, Description ASC ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<ModelTest>(sql.ToString(), new { State = (Byte)EnumState.ativo, EntityId = EntityId });
			}
		}

		public ModelTest GetDefault(Guid EntityId)
		{
			StringBuilder sql = new StringBuilder("SELECT Id,Description,DefaultModel,ShowCoverPage,ShowBorder,LogoHeaderPosition,LogoHeaderWaterMark,MessageHeaderPosition,MessageHeader, ");
            sql.Append("MessageHeaderWaterMark,ShowLineBelowHeader,ShowItemLine,FileHeader_Id,LogoFooterPosition,LogoFooterWaterMark,MessageFooterPosition,MessageFooter,MessageFooterWaterMark, ");
			sql.Append("ShowLineAboveFooter,FileFooter_Id,ShowSchool,ShowStudentName,ShowTeacherName,ShowClassName,ShowStudentNumber,ShowDate,ShowLineBelowStudentInformation, ");
			sql.Append("CoverPageText,ShowStudentInformationsOnCoverPage,ShowHeaderOnCoverPage,ShowFooterOnCoverPage,CreateDate,UpdateDate,State,EntityId,ShowBorderOnCoverPage, ");
			sql.Append("LogoHeaderSize,ShowMessageHeader,LogoFooterSize,ShowMessageFooter,ShowLogoHeader,ShowLogoFooter,HeaderHtml,FooterHtml,StudentInformationHtml ");
			sql.Append("FROM ModelTest (NOLOCK) ");
			sql.Append("WHERE EntityId = @EntityId AND State = @State AND DefaultModel = @DefaultModel ");
			sql.Append("ORDER BY DefaultModel DESC, Description ASC ");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<ModelTest>(sql.ToString(), new { State = (Byte)EnumState.ativo, EntityId = EntityId, DefaultModel = true }).FirstOrDefault();
			}
		}

		public bool ExistsDescriptionNamed(Guid EntityId, long Id, string description)
		{
			StringBuilder sql = new StringBuilder("SELECT Count(Id) ");
			sql.Append("FROM dbo.ModelTest (NOLOCK) ");
			sql.Append("WHERE State = @state AND EntityId = @EntityId AND Id <> @id AND Description COLLATE Latin1_General_CI_AS = @DefaultModel");

			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return (int)cn.ExecuteScalar(sql.ToString(), new { state = (Byte)EnumState.ativo, EntityId = EntityId, id = Id, DefaultModel = description }) > 0;
			}
		}

		#endregion

		#region CRUD
		public ModelTest Save(ModelTest entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				DateTime dateNow = DateTime.Now;

				entity.CreateDate = dateNow;
				entity.UpdateDate = dateNow;
				entity.State = Convert.ToByte(EnumState.ativo);

				GestaoAvaliacaoContext.ModelTest.Add(entity);
				GestaoAvaliacaoContext.SaveChanges();

				return entity;
			}
		}

		public void Update(ModelTest entity)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				ModelTest modelTest = GestaoAvaliacaoContext.ModelTest.FirstOrDefault(a => a.Id == entity.Id);

				#region Atribui Dados Novos

				modelTest.CoverPageText = entity.CoverPageText;
				modelTest.DefaultModel = entity.DefaultModel;
				modelTest.FileFooter_Id = entity.FileFooter_Id;
				modelTest.FileHeader_Id = entity.FileHeader_Id;
				modelTest.LogoFooterSize = entity.LogoFooterSize;
				modelTest.LogoHeaderSize = entity.LogoHeaderSize;
				modelTest.LogoFooterPosition = entity.LogoFooterPosition;
				modelTest.LogoFooterWaterMark = entity.LogoFooterWaterMark;
				modelTest.LogoHeaderPosition = entity.LogoHeaderPosition;
				modelTest.LogoHeaderWaterMark = entity.LogoHeaderWaterMark;
				modelTest.MessageFooter = entity.MessageFooter;
				modelTest.MessageFooterPosition = entity.MessageFooterPosition;
				modelTest.MessageFooterWaterMark = entity.MessageFooterWaterMark;
				modelTest.MessageHeader = entity.MessageHeader;
				modelTest.MessageHeaderPosition = entity.MessageHeaderPosition;
				modelTest.MessageHeaderWaterMark = entity.MessageHeaderWaterMark;
				modelTest.ShowBorder = entity.ShowBorder;
				modelTest.ShowClassName = entity.ShowClassName;
				modelTest.ShowCoverPage = entity.ShowCoverPage;
				modelTest.ShowDate = entity.ShowDate;
				modelTest.ShowFooterOnCoverPage = entity.ShowFooterOnCoverPage;
				modelTest.ShowHeaderOnCoverPage = entity.ShowHeaderOnCoverPage;
				modelTest.ShowLineAboveFooter = entity.ShowLineAboveFooter;
				modelTest.ShowLineBelowHeader = entity.ShowLineBelowHeader;
				modelTest.ShowLineBelowStudentInformation = entity.ShowLineBelowStudentInformation;
                modelTest.ShowItemLine = entity.ShowItemLine;
				modelTest.ShowSchool = entity.ShowSchool;
				modelTest.ShowStudentInformationsOnCoverPage = entity.ShowStudentInformationsOnCoverPage;
				modelTest.ShowStudentName = entity.ShowStudentName;
				modelTest.ShowStudentNumber = entity.ShowStudentNumber;
				modelTest.ShowTeacherName = entity.ShowTeacherName;
				modelTest.State = entity.State;
				modelTest.Description = entity.Description;
				modelTest.ShowBorderOnCoverPage = entity.ShowBorderOnCoverPage;
				modelTest.ShowLogoFooter = entity.ShowLogoFooter;
				modelTest.ShowLogoHeader = entity.ShowLogoHeader;
				modelTest.StudentInformationHtml = entity.StudentInformationHtml;
				modelTest.FooterHtml = entity.FooterHtml;
				modelTest.HeaderHtml = entity.HeaderHtml;

				modelTest.UpdateDate = DateTime.Now;
				#endregion

				GestaoAvaliacaoContext.Entry(modelTest).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();
			}
		}

		public void UnsetDefaultModel(Guid EntityId)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				ModelTest modelTest = GestaoAvaliacaoContext.ModelTest.FirstOrDefault(a => a.EntityId == EntityId && a.DefaultModel);

				modelTest.DefaultModel = false;
				modelTest.UpdateDate = DateTime.Now;

				GestaoAvaliacaoContext.Entry(modelTest).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();
			}
		}

		public void Delete(long id)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				ModelTest modelTest = GestaoAvaliacaoContext.ModelTest.FirstOrDefault(a => a.Id == id);
				modelTest.UpdateDate = DateTime.Now;
				modelTest.State = Convert.ToByte(EnumState.excluido);

				GestaoAvaliacaoContext.Entry(modelTest).State = System.Data.Entity.EntityState.Modified;
				GestaoAvaliacaoContext.SaveChanges();
			}
		}

		public void RemoveFileFromEntity(long fileId)
		{
			using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
			{
				ModelTest modelTest = GestaoAvaliacaoContext.ModelTest.FirstOrDefault(a => a.FileFooter_Id == fileId || a.FileHeader_Id == fileId);

				if (modelTest != null)
				{
					modelTest.UpdateDate = DateTime.Now;
					if (modelTest.FileHeader_Id == fileId)
						modelTest.FileHeader_Id = null;
					else if (modelTest.FileFooter_Id == fileId)
						modelTest.FileFooter_Id = null;

					GestaoAvaliacaoContext.Entry(modelTest).State = System.Data.Entity.EntityState.Modified;
					GestaoAvaliacaoContext.SaveChanges();
				}
			}
		}
		#endregion
	}
}
