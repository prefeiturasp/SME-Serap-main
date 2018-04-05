using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.MongoEntities.Projections;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoRepository
{
	public class CorrectionResultsRepository : BaseRepository<CorrectionResults>, ICorrectionResultsRepository
	{
        /// <summary>
        /// Busca as médias(acertos e porcentagem) da SME, DRE, escola e turma da prova passada(test_id)
        /// </summary>
        /// <param name="test_Id">Id da prova</param>
        /// <param name="esc_Id">Id da escola</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="team_id">Id da turma</param>
        /// <returns>Projection com médias(acertos e porcentagem) da SME, DRE, escola e turma em relação a prova passada</returns>
        public List<TestAverageTeamResult> GetTestAveragesHitsAndPercentagesByTest(List<long> test_Id, long? discipline_id)
        {
            var filter = new BsonDocument();
            if (discipline_id.HasValue)
                filter.Add("Discipline_Id", discipline_id);

            BsonArray bArray = new BsonArray();
            bArray.AddRange(test_Id);

            var filterTest = new BsonDocument();
            filterTest.Add("Test_id", new BsonDocument { { "$in", bArray } });

            var allTestDRE = DataBase
                                  .GetCollection<CorrectionResults>("CorrectionResults")
                                  .Aggregate()
                                  .Match(filterTest)
                                  .Unwind("Students")
								  .Project(new BsonDocument {
												{ "Dre_id", "$Dre_id" },
												{ "Esc_id", "$Esc_id" },
												{ "Tur_id", "$Tur_id" },
                                                { "Test_id", "$Test_id" },
                                                { "alu_id", "$alu_id"},
                                                { "NumberAnswers", "$NumberAnswers"},
                                                { "Performance", "$Students.Performance"}
                                        })
                                  .Match(new BsonDocument { { "Performance", new BsonDocument("$ne", BsonNull.Value) } })
                                  .Group(new BsonDocument 
								  {
									{
										"_id", new BsonDocument
										{
											{ "Dre_id", "$Dre_id" },
											{ "Esc_id", "$Esc_id" },
											{ "Tur_id", "$Tur_id" },
                                            { "Test_id", "$Test_id" },
                                            { "NumberAnswers", "$NumberAnswers"}
                                        }
									},
									{"NumberStudents", new BsonDocument("$sum", 1) }
                                  })
                                  .Project(new BsonDocument {
									  { "_id", 0 },
									  { "Dre_id", "$_id.Dre_id" },
									  { "Esc_id", "$_id.Esc_id" },
									  { "Tur_id", "$_id.Tur_id" },
                                      { "Test_id", "$_id.Test_id" },
                                      { "NumberAnswers", "$_id.NumberAnswers"},
                                      { "TotalStudents", "$NumberStudents" }
								  })
								  .As<TestAverageTeamResult>()
								  .ToListAsync();

			var correctTestDRE = DataBase
								  .GetCollection<CorrectionResults>("CorrectionResults")
								  .Aggregate()
								  .Match(filterTest)
								  .Unwind("Students")
								  .Unwind("Students.Alternatives")
								  .Project(new BsonDocument {
												{ "Dre_id", "$Dre_id" },
												{ "Esc_id", "$Esc_id" },
												{ "Tur_id", "$Tur_id" },
                                                { "Test_id", "$Test_id" },
                                                { "Alternatives", "$Students.Alternatives.Correct" },
												{ "Discipline_Id", "$Students.Alternatives.Discipline_Id" },
										})
								  .Match(new BsonDocument { { "Alternatives", true } })
								  .Match(filter)
								  .Group(new BsonDocument
								  {
									{
										"_id", new BsonDocument
										{
											{ "Dre_id", "$Dre_id" },
											{ "Esc_id", "$Esc_id" },
											{ "Tur_id", "$Tur_id" },
                                            { "Test_id", "$Test_id" },
                                        }
									},
									{ "TotalItems", new BsonDocument("$sum", 1) }
								  })
								  .Project(new BsonDocument {
									  { "_id", 0 },
									  { "Dre_id", "$_id.Dre_id" },
									  { "Esc_id", "$_id.Esc_id" },
									  { "Tur_id", "$_id.Tur_id" },
                                      { "Test_id", "$_id.Test_id" },
                                      { "TotalItems", "$TotalItems" },
									  { "TotalCorretItems", "$TotalItems" }
								  })
								  .As<TestAverageTeamResult>()
								  .ToListAsync();

			var lstResultSME = (from all in allTestDRE.Result
								join correct in correctTestDRE.Result on new { all.Dre_id, all.Esc_id, all.Tur_id, all.Test_id } equals new { correct.Dre_id, correct.Esc_id, correct.Tur_id, correct.Test_id } into j1
								from j2 in j1.DefaultIfEmpty()
								select new TestAverageTeamResult { Dre_id = all.Dre_id, Esc_id = all.Esc_id, Tur_id = all.Tur_id, Test_id = all.Test_id, NumberAnswers = all.NumberAnswers, TotalStudents = all.TotalStudents, TotalItems = (j2 != null ? j2.TotalItems : 0), TotalCorretItems = (j2 != null ? j2.TotalCorretItems : 0) }).ToList();
			return lstResultSME;


		}

		/// <summary>
		/// Busca as informações do aluno na prova informada, como nome, número de matricula e desempenho na mesma
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="alu_id">Id do aluno</param>
		/// <returns>Projection com as informações do aluno em determinada prova</returns>
		public async Task<StudentTestInformationProjection> GetStudentTestInformationByTestAndStudent(long test_Id, long alu_id)
		{
			var studentTest = await DataBase
								.GetCollection<BsonDocument>("CorrectionResults")
								.Aggregate()
								.Match(new BsonDocument { { "Test_id", test_Id } })
								.Unwind("Students")
								.Match(new BsonDocument { { "Students.alu_id", alu_id } })
								.Project(new BsonDocument {
									{ "_id", 0 },
									{ "Alu_id", "$Students.alu_id" },
									{ "Mtu_numeroChamada", "$Students.mtu_numeroChamada" },
									{ "Alu_nome", "$Students.alu_nome" },
									{ "Hits", "$Students.Hits" },
									{ "Avg", "$Students.Performance" },
									{ "Items", "$Students.Alternatives" }
								})
								.As<StudentTestInformationProjection>()
								.FirstOrDefaultAsync();

			return studentTest;
		}

		/// <summary>
		/// Busca as médias(porcentagem) de acertos da SME e DRE em relação a cada item da prova
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <returns>Lista de projection com as médias(porcentagem) da SME e DRE em relação a cada item da prova</returns>
		public async Task<List<ItemAvgPercentageSMEAndDREProjection>> GetAvgPercentageSmeAndDrePerItemByTest(long test_Id, Guid dre_id)
		{
			var averagesItem = DataBase
							   .GetCollection<BsonDocument>("CorrectionResults")
							   .Aggregate()
							   .Match(new BsonDocument { { "Test_id", test_Id } })
							   .Unwind("Statistics.Averages")
							   .Group(new BsonDocument {
										{ "_id", "$Statistics.Averages.Item_Id" },
										{ "totalItemsSME", new BsonDocument("$sum", 1) },
										{ "sumAvgItemsSME", new BsonDocument("$sum", "$Statistics.Averages.Average") },
										{ "totalItemsDRE",
												new BsonDocument("$sum",
													new BsonDocument("$cond", new BsonArray {
														new BsonDocument("$eq",
																new BsonArray
																{
																	"$Dre_id",
																	dre_id
																}
															), 1, 0
													})
												)
										},
										{ "sumAvgItemsDRE",
												new BsonDocument("$sum",
													new BsonDocument("$cond", new BsonArray {
														new BsonDocument("$eq",
																new BsonArray
																{
																	"$Dre_id",
																	dre_id
																}
															), "$Statistics.Averages.Average", 0
													})
												)
										}
									})
							   .Project(new BsonDocument {
										{ "_id", 0 },
										{ "Item_Id", "$_id" },
										{ "AvgSME",
											new BsonDocument("$cond", new BsonArray {
												new BsonDocument("$gte",
														new BsonArray
														{
															"$totalItemsSME",
															1
														}
													),new BsonDocument("$divide", new BsonArray {
															"$sumAvgItemsSME","$totalItemsSME"
														}), 0
												})
										},
										{ "AvgDRE",
											new BsonDocument("$cond", new BsonArray {
												new BsonDocument("$gte",
														new BsonArray
														{
															"$totalItemsDRE",
															1
														}
													),new BsonDocument("$divide", new BsonArray {
															"$sumAvgItemsDRE","$totalItemsDRE"
														}), 0
												})
										}
								   })
								.As<ItemAvgPercentageSMEAndDREProjection>()
								.ToListAsync();

			return await averagesItem;
		}

		/// <summary>
		/// Busca as médias de escolha por alternativa de cada item em uma prova
		/// </summary>
		/// <param name="test_Id">Id da prova</param>
		/// <param name="dre_id">Id da DRE</param>
		/// <param name="esc_id">Id da escola</param>
		/// <returns>Lista de projection com as médias(porcentagem) de escolha da DRE por alternativa de cada item da prova</returns>
		public async Task<List<ItemPercentageChoiceByAlternativeProjection>> GetItemPercentageChoiceByAlternative(long test_Id, long? discipline_id, Guid? dre_id, int? esc_id)
		{
			var filter = new BsonDocument();
			if (dre_id.HasValue)
				filter.Add("Dre_id", dre_id);
			else if (esc_id.HasValue)
				filter.Add("Esc_id", esc_id);

			var itemsPercentageByAlternative = DataBase
												.GetCollection<BsonDocument>("CorrectionResults")
												.Aggregate()
												.Match(new BsonDocument("Test_id", test_Id))
												.Match(filter)
												.Unwind("Students")
												.Unwind("Students.Alternatives")
												.Group(new BsonDocument {
														{ "_id",  new BsonDocument {
															{ "Item_Id", "$Students.Alternatives.Item_Id"},
															{ "Alternative_Id", "$Students.Alternatives.Alternative_Id" },
															{ "Discipline_Id", "$Students.Alternatives.Discipline_Id" },
															{ "Numeration", "$Students.Alternatives.Numeration" }
														}},
														{ "totalItems", new BsonDocument("$sum", 1) }
													})
												.Group(new BsonDocument {
														{ "_id",  new BsonDocument {
															{ "Item_Id", "$_id.Item_Id"},
															{ "Discipline_Id", "$_id.Discipline_Id" }
														}},
														{ "totalItems", new BsonDocument("$sum", "$totalItems") },
														{ "alternatives", new BsonDocument("$push", new BsonDocument {
															{ "alternative_Id", "$_id.Alternative_Id" },
															{ "totalChoiceByAlternative", "$totalItems" },
															{ "numeration", "$_id.Numeration" }
														})}
													})

												.Unwind("alternatives")
												.Group(new BsonDocument {
														{ "_id",  new BsonDocument {
															{ "Item_Id", "$_id.Item_Id"},
															{ "Discipline_Id", "$_id.Discipline_Id" }
														}},
														 { "alternatives", new BsonDocument("$push", new BsonDocument {
															 { "Alternative_Id", "$alternatives.alternative_Id" },
															 { "Numeration", "$alternatives.numeration" },
															 { "Avg",
																new BsonDocument("$multiply",
																		new BsonArray
																		{
																			new BsonDocument("$divide",
																				new BsonArray {
																					"$alternatives.totalChoiceByAlternative",
																					"$totalItems"
																				}
																			),100
																		}
																	)
															 }
														 })}
													 })
											   .Project(new BsonDocument {
															{ "_id", 0 },
															{ "Item_Id", "$_id.Item_Id" },
															{ "Discipline_Id", "$_id.Discipline_Id" },
															{ "Alternatives", "$alternatives" },
													})
											   .As<ItemPercentageChoiceByAlternativeProjection>()
											   .ToListAsync();

			return await itemsPercentageByAlternative;
		}

		public List<TestAverageDreDTO> GetTestAverageByTestGroupByDre(long testId, long? discipline_id)
		{
			var filter = new BsonDocument();
			if (discipline_id.HasValue)
				filter.Add("Discipline_Id", discipline_id);

			Task<List<TestAverageDreDTO>> allTestDRE = DataBase
								  .GetCollection<CorrectionResults>("CorrectionResults")
								  .Aggregate()
								  .Match(new BsonDocument { { "Test_id", testId } })
								  .Unwind("Students")
								  .Unwind("Students.Alternatives")
								  .Project(new BsonDocument {
												{ "Dre_id", "$Dre_id" },
												{ "Alternatives", "$Students.Alternatives.Correct" },
												{ "Discipline_Id", "$Students.Alternatives.Discipline_Id" },
										})
								  .Match(filter)
								  .Group(new BsonDocument
								  {
									{ "_id", "$Dre_id" },
									{ "TotalItems", new BsonDocument("$sum", 1) }
								  })
								  .Project(new BsonDocument {
									  { "_id", 0 },
									  { "Dre_id", "$_id" },
									  { "TotalItems", "$TotalItems" },
									  { "TotalCorretItems", "$TotalItems" }
								  })
								  .As<TestAverageDreDTO>()
								  .ToListAsync();

			Task<List<TestAverageDreDTO>> correctTestDRE = DataBase
								  .GetCollection<CorrectionResults>("CorrectionResults")
								  .Aggregate()
								  .Match(new BsonDocument { { "Test_id", testId } })
								  .Unwind("Students")
								  .Unwind("Students.Alternatives")
								  .Project(new BsonDocument {
												{ "Dre_id", "$Dre_id" },
												{ "Alternatives", "$Students.Alternatives.Correct" },
												{ "Discipline_Id", "$Students.Alternatives.Discipline_Id" },
										})
								  .Match(new BsonDocument { { "Alternatives", true } })
								  .Match(filter)
								  .Group(new BsonDocument
								  {
								   { "_id", "$Dre_id" },
								   { "TotalItems", new BsonDocument("$sum", 1) }
								  })
								  .Project(new BsonDocument {
									  { "_id", 0 },
									  { "Dre_id", "$_id" },
									  { "TotalItems", "$TotalItems" },
									  { "TotalCorretItems", "$TotalItems" }
								  })
								  .As<TestAverageDreDTO>()
								  .ToListAsync();

			return (from all in allTestDRE.Result
					join correct in correctTestDRE.Result on all.Dre_id equals correct.Dre_id into j1
					from j2 in j1.DefaultIfEmpty()
					select new TestAverageDreDTO { Dre_id = all.Dre_id, TotalItems = all.TotalItems, TotalCorretItems = (j2 != null ? j2.TotalCorretItems : 0) }).ToList();
		}

		public List<TestAverageSchoolDTO> GetTestAverageByTestDreGroupBySchool(long testId, long? discipline_id, Guid dre_id)
		{
			var filter = new BsonDocument();
			if (discipline_id.HasValue)
				filter.Add("Discipline_Id", discipline_id);

			Task<List<TestAverageSchoolDTO>> allTestDRE = DataBase
								  .GetCollection<CorrectionResults>("CorrectionResults")
								  .Aggregate()
								  .Match(new BsonDocument { { "Test_id", testId }, { "Dre_id", dre_id } })
								  .Unwind("Students")
								  .Unwind("Students.Alternatives")
								  .Project(new BsonDocument {
												{ "Esc_id", "$Esc_id" },
												{ "Alternatives", "$Students.Alternatives.Correct" },
												{ "Discipline_Id", "$Students.Alternatives.Discipline_Id" },
										})
								  .Match(filter)
								  .Group(new BsonDocument
								  {
									{ "_id", "$Esc_id" },
									{ "TotalItems", new BsonDocument("$sum", 1) }
								  })
								  .Project(new BsonDocument {
									  { "_id", 0 },
									  { "Esc_id", "$_id" },
									  { "TotalItems", "$TotalItems" },
									  { "TotalCorretItems", "$TotalItems" }
								  })
								  .As<TestAverageSchoolDTO>()
								  .ToListAsync();

			Task<List<TestAverageSchoolDTO>> correctTestDRE = DataBase
								  .GetCollection<CorrectionResults>("CorrectionResults")
								  .Aggregate()
								  .Match(new BsonDocument { { "Test_id", testId }, { "Dre_id", dre_id } })
								  .Unwind("Students")
								  .Unwind("Students.Alternatives")
								  .Project(new BsonDocument {
												{ "Esc_id", "$Esc_id" },
												{ "Alternatives", "$Students.Alternatives.Correct" },
												{ "Discipline_Id", "$Students.Alternatives.Discipline_Id" },
										})
								  .Match(new BsonDocument { { "Alternatives", true } })
								  .Match(filter)
								  .Group(new BsonDocument
								  {
								   { "_id", "$Esc_id" },
								   { "TotalItems", new BsonDocument("$sum", 1) }
								  })
								  .Project(new BsonDocument {
									  { "_id", 0 },
									  { "Esc_id", "$_id" },
									  { "TotalItems", "$TotalItems" },
									  { "TotalCorretItems", "$TotalItems" }
								  })
								  .As<TestAverageSchoolDTO>()
								  .ToListAsync();

			return (from all in allTestDRE.Result
					join correct in correctTestDRE.Result on all.Esc_id equals correct.Esc_id into j1
					from j2 in j1.DefaultIfEmpty()
					select new TestAverageSchoolDTO { Esc_id = all.Esc_id, TotalItems = all.TotalItems, TotalCorretItems = (j2 != null ? j2.TotalCorretItems : 0) }).ToList();
		}

		public List<TestAveragePerformanceDTO> GetTestAveragePerformanceGeneral(long testId, long? discipline_id)
		{
			var averageTest = DataBase
							  .GetCollection<CorrectionResults>("CorrectionResults")
							  .Aggregate()
							  .Match(new BsonDocument { { "Test_id", testId } })
							  .Unwind(i => i.Statistics.Averages)
							  .Group(new BsonDocument {
										{ "_id", "$Test_id" },
										{ "mediaTurma", new BsonDocument("$avg", "$Statistics.GeneralGrade") }
								   })
							  .Project(new BsonDocument {
								   { "_id", 0 },
								   { "Media", "$mediaTurma" }
							  })
							  .As<TestAveragePerformanceDTO>()
							  .ToListAsync();

			return averageTest.Result;
		}

		public List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceGeneral(long testId)
		{
			var averageTest = DataBase
							  .GetCollection<CorrectionResults>("CorrectionResults")
							  .Aggregate()
							  .Match(new BsonDocument { { "Test_id", testId } })
							  .Unwind(i => i.Statistics.Averages)
							  .Group(new BsonDocument
							  {
								   {
									   "_id", new BsonDocument
									   {
										   {"Item_id", "$Statistics.Averages.Item_Id"},
										   {"Discipline_Id", "$Statistics.Averages.Discipline_Id"}
									   }
								   },
								   { "TotalItens", new BsonDocument("$sum", 1) },
								   { "TotalSomaItem", new BsonDocument("$sum", "$Statistics.Averages.Average") }
							  })
							  .Project(new BsonDocument {
								   { "_id", 0 },
								   { "Item_id", "$_id.Item_id" },
								   { "Discipline_Id", "$_id.Discipline_Id" },

								   { "Media", new BsonDocument()
									   {
										   {
											   "$divide", new BsonArray
											   {
												   "$TotalSomaItem",
												   "$TotalItens"
											   }
										   }
									   }
								   }
							  })
							  .As<TestAverageItemPerformanceDTO>()
							  .ToListAsync();

			return averageTest.Result;
		}

		public List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceByDre(List<long> testId)
		{
            BsonArray bArray = new BsonArray();
            bArray.AddRange(testId);

            var filterTest = new BsonDocument();
            filterTest.Add("Test_id", new BsonDocument { { "$in", bArray } });

            var averageTest = DataBase
							  .GetCollection<CorrectionResults>("CorrectionResults")
							  .Aggregate()
                              .Match(filterTest)
                              .Unwind(i => i.Statistics.Averages)
							  .Group(new BsonDocument
							  {
								   {
									   "_id", new BsonDocument
									   {
										   {"Dre_id", "$Dre_id"},
										   {"Item_id", "$Statistics.Averages.Item_Id"},
										   {"Discipline_Id", "$Statistics.Averages.Discipline_Id"}
									   }
								   },
								   { "TotalItens", new BsonDocument("$sum", 1) },
								   { "TotalSomaItem", new BsonDocument("$sum", "$Statistics.Averages.Average") }
							  })
							  .Project(new BsonDocument {
								   { "_id", 0 },
								   { "Dre_id", "$_id.Dre_id" },
								   { "Item_id", "$_id.Item_id" },
								   { "Discipline_Id", "$_id.Discipline_Id" },
								   { "Media", new BsonDocument()
									   {
										   {
											   "$divide", new BsonArray
											   {
												   "$TotalSomaItem",
												   "$TotalItens"
											   }
										   }
									   }
								   }
							  })
							  .As<TestAverageItemPerformanceDTO>()
							  .ToListAsync();

			return averageTest.Result;
		}

		public List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceBySchool(long test_Id, Guid dre_id)
		{

			var averageTest = DataBase
							  .GetCollection<CorrectionResults>("CorrectionResults")
							  .Aggregate()
							  .Match(new BsonDocument { { "Test_id", test_Id }, { "Dre_id", dre_id } })
							  .Unwind(i => i.Statistics.Averages)
							  .Group(new BsonDocument
							  {
								   {
									   "_id", new BsonDocument
									   {
										   {"Esc_id", "$Esc_id"},
										   {"Item_id", "$Statistics.Averages.Item_Id"},
										   {"Discipline_Id", "$Statistics.Averages.Discipline_Id"}
									   }
								   },
								   { "TotalItens", new BsonDocument("$sum", 1) },
								   { "TotalSomaItem", new BsonDocument("$sum", "$Statistics.Averages.Average") }
							  })
							  .Project(new BsonDocument {
								   { "_id", 0 },
								   { "Esc_id", "$_id.Esc_id" },
								   { "Item_id", "$_id.Item_id" },
								   { "Discipline_Id", "$_id.Discipline_Id" },
								   { "Media", new BsonDocument()
									   {
										   {
											   "$divide", new BsonArray
											   {
												   "$TotalSomaItem",
												   "$TotalItens"
											   }
										   }
									   }
								   }
							  })
							  .As<TestAverageItemPerformanceDTO>()
							  .ToListAsync();

			return averageTest.Result;
		}

        public async Task<List<CorrectionResults>> GetByTest(List<long> testId)
        {
            var filter1 = Builders<CorrectionResults>.Filter.In("Test_id", testId);
            var filter = Builders<CorrectionResults>.Filter.And(filter1);

            var count = await base.Count(filter);

            if (count == 0)
                return new List<CorrectionResults>();
            else
                return await base.Find(filter);
        }

    }
}