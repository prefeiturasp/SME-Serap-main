using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class ResponseChangeLogBusiness : IResponseChangeLogBusiness
    {
        private readonly IStudentTestAbsenceReasonRepository _studentTestAbsenceReasonRepository;
        private readonly IResponseChangeLogMongoRepository _responseChangeLogMongoRepository;
        private readonly IResponseChangeLogRepository _responseChangeLogRepository;
        private readonly IAbsenceReasonRepository _absenceReasonRepository;

        public ResponseChangeLogBusiness(IStudentTestAbsenceReasonRepository studentTestAbsenceReasonRepository, IResponseChangeLogMongoRepository responseChangeLogMongoRepository, IResponseChangeLogRepository responseChangeLogRepository,
            IAbsenceReasonRepository absenceReasonRepository)
        {
            _studentTestAbsenceReasonRepository = studentTestAbsenceReasonRepository;
            _responseChangeLogMongoRepository = responseChangeLogMongoRepository;
            _responseChangeLogRepository = responseChangeLogRepository;
            _absenceReasonRepository = absenceReasonRepository;
        }

        public async Task SaveAsync(IEnumerable<Answer> answers, long alu_id, long test_id, long tur_id, Guid ent_id, Guid usuId, bool manual, IEnumerable<StudentCorrectionAnswerGrid> studentsAnswers, long AbsenceReason_IdAnterior, long AbsenceReason_IdAtual)
        {
            var escola = _studentTestAbsenceReasonRepository.GetEscIdDreIdByTeam(tur_id);

            var responsesChangeLog = answers
                .Select(a =>
                {
                    var responseChangeLog = new ResponseChangeLog
                    {
                        Ent_id = ent_id,
                        Usu_id = usuId,
                        Dre_id = escola.dre_id,
                        Esc_id = escola.esc_id,
                        Tur_id = tur_id,
                        Test_id = test_id,
                        Alu_id = alu_id,
                        Item_Id = a != null ? a.Item_Id : 0,
                        Alternative_IdAtual = (a != null ? a.AnswerChoice : 0),
                    };

                    responseChangeLog.Alternative_Atual = (a != null && a.AnswerChoice > 0
                                            ? studentsAnswers.FirstOrDefault(p => p.Item_Id == a.Item_Id).Alternatives.FirstOrDefault(p => p.Id == a.AnswerChoice).Numeration.Replace(")", "")
                                            : (a != null && a.AnswerChoice == 0 && a.Empty
                                                ? "N"
                                                : (a != null && a.AnswerChoice == 0 && a.StrikeThrough
                                                    ? "R"
                                                    : string.Empty))
                                        );

                    responseChangeLog.Alternative_IdAnterior = studentsAnswers != null
                                                    ? (studentsAnswers.FirstOrDefault(p => p.Item_Id == a.Item_Id).Alternatives.Exists(p => p.Selected == true)
                                                        ? studentsAnswers.FirstOrDefault(p => p.Item_Id == a.Item_Id).Alternatives.FirstOrDefault(p => p.Selected == true).Id
                                                        : 0)
                                                    : 0;
                    responseChangeLog.Alternative_Anterior = (studentsAnswers == null
                                                    ? string.Empty
                                                        : studentsAnswers.FirstOrDefault(p => p.Item_Id == a.Item_Id).Null
                                                            ? "N"
                                                            : studentsAnswers.FirstOrDefault(p => p.Item_Id == a.Item_Id).StrikeThrough
                                                                ? "R"
                                                                : studentsAnswers.FirstOrDefault(p => p.Item_Id == a.Item_Id).Alternatives.Exists(p => p.Selected == true)
                                                                    ? studentsAnswers.FirstOrDefault(p => p.Item_Id == a.Item_Id).Alternatives.FirstOrDefault(p => p.Selected == true).Numeration.Replace(")", "")
                                                            : string.Empty);
                    responseChangeLog.Automatic = !manual;
                    responseChangeLog.Absence = a == null;
                    responseChangeLog.AbsenceReason_IdAnterior = AbsenceReason_IdAnterior;
                    responseChangeLog.AbsenceReason_IdAtual = AbsenceReason_IdAtual;

                    return responseChangeLog;
                })
                .ToList();

            await _responseChangeLogMongoRepository.InsertManyAsync(responsesChangeLog);
        }

        public async Task<ResponseChangeLog> Save(Answer answer, long alu_id, long test_id, long tur_id, Guid ent_id, Guid usuId, bool manual, IEnumerable<StudentCorrectionAnswerGrid> studentsAnswers, long AbsenceReason_IdAnterior, long AbsenceReason_IdAtual)
        {
            SchoolDTO escola = _studentTestAbsenceReasonRepository.GetEscIdDreIdByTeam(tur_id);

            ResponseChangeLog responseChangeLog = new ResponseChangeLog();

            responseChangeLog.Ent_id = ent_id;
            responseChangeLog.Usu_id = usuId;
            responseChangeLog.Dre_id = escola.dre_id;
            responseChangeLog.Esc_id = escola.esc_id;
            responseChangeLog.Tur_id = tur_id;
            responseChangeLog.Test_id = test_id;
            responseChangeLog.Alu_id = alu_id;
            responseChangeLog.Item_Id = answer != null ? answer.Item_Id : 0;
            responseChangeLog.Alternative_IdAtual = (answer != null ? answer.AnswerChoice : 0);
            responseChangeLog.Alternative_Atual = (answer != null && answer.AnswerChoice > 0
                                            ? studentsAnswers.FirstOrDefault(p => p.Item_Id == answer.Item_Id).Alternatives.FirstOrDefault(p => p.Id == answer.AnswerChoice).Numeration.Replace(")", "")
                                            : (answer != null && answer.AnswerChoice == 0 && answer.Empty
                                                ? "N"
                                                : (answer != null && answer.AnswerChoice == 0 && answer.StrikeThrough
                                                    ? "R"
                                                    : string.Empty))
                                        );
          
            responseChangeLog.Alternative_IdAnterior = studentsAnswers != null                                             
                                            ? (studentsAnswers.FirstOrDefault(p => p.Item_Id == answer.Item_Id).Alternatives.Exists(p => p.Selected == true) 
                                                ? studentsAnswers.FirstOrDefault(p => p.Item_Id == answer.Item_Id).Alternatives.FirstOrDefault(p => p.Selected == true).Id 
                                                : 0) 
                                            : 0;
            responseChangeLog.Alternative_Anterior = (studentsAnswers == null 
                                            ? string.Empty
                                                : studentsAnswers.FirstOrDefault(p => p.Item_Id == answer.Item_Id).Null 
                                                    ? "N"
                                                    : studentsAnswers.FirstOrDefault(p => p.Item_Id == answer.Item_Id).StrikeThrough
                                                        ? "R"
                                                        : studentsAnswers.FirstOrDefault(p => p.Item_Id == answer.Item_Id).Alternatives.Exists(p => p.Selected == true)
                                                            ? studentsAnswers.FirstOrDefault(p => p.Item_Id == answer.Item_Id).Alternatives.FirstOrDefault(p => p.Selected == true).Numeration.Replace(")", "") 
                                                    : string.Empty);
            responseChangeLog.Automatic = !manual;
            responseChangeLog.Absence = answer == null;
            responseChangeLog.AbsenceReason_IdAnterior = AbsenceReason_IdAnterior;
            responseChangeLog.AbsenceReason_IdAtual = AbsenceReason_IdAtual;

            return await _responseChangeLogMongoRepository.Insert(responseChangeLog);
        }

        public List<ResponseChangeLogDTO> GetResponseChangeLog(long test_id, Guid ent_id, Guid? uad_id, long? esc_id, long? tur_id, DateTime? DateStartChange, DateTime? DateEndChange, ref Pager pager)
        {
            List<ResponseChangeLog> lista = _responseChangeLogMongoRepository.GetResponseChangeLog(test_id, uad_id, esc_id, tur_id)
                        .Where(x => (DateStartChange == null || (DateStartChange.HasValue && x.CreateDate.Date >= DateStartChange.Value.Date)) &&
                            (DateEndChange == null || (DateEndChange.HasValue && x.CreateDate.Date <= DateEndChange.Value.Date))).ToList().OrderByDescending(p => p.CreateDate).ToList();

            pager.SetTotalPages((int)Math.Ceiling(lista.Count() / (double)pager.PageSize));
            pager.SetTotalItens(lista.Count);

            List<ResponseChangeLog> listaPaginada = lista
                                                .Skip(pager.CurrentPage * pager.PageSize)
                                                .Take(pager.PageSize).ToList();

            List <ResponseChangeLogDTO> listaCompleta = new List<ResponseChangeLogDTO>();

            if (listaPaginada.Count > 0)
            {
                List<long> alunos = new List<long>();
                List<string> dres = new List<string>();
                List<int> escolas = new List<int>();
                List<long> turmas = new List<long>();
                List<string> usuarios = new List<string>();
                List<long> itens = new List<long>();

                foreach (ResponseChangeLog responseChangeLog in listaPaginada)
                {
                    alunos.Add(responseChangeLog.Alu_id);
                    dres.Add(responseChangeLog.Dre_id.ToString());
                    escolas.Add(responseChangeLog.Esc_id);
                    turmas.Add(responseChangeLog.Tur_id);
                    usuarios.Add(responseChangeLog.Usu_id.ToString());
                    itens.Add(responseChangeLog.Item_Id);
                }

                List<StudentDTO> students = GetInfoStudents(alunos.Distinct().ToList());

                List<DresDTO> dresSchools = GetInfoDresSchools(dres.Distinct().ToList(), ent_id);

                List<UsersDTO> users = GetInfoUsers(usuarios.Distinct().ToList(), ent_id);

                List<TeamsDTO> teams = GetInfoTeams(turmas.Distinct().ToList());

                List<AbsenceReason> absenceReasons = _absenceReasonRepository.GetAll(ent_id);

                List<BlockItem> blockItens = GetBlockItens(itens.Distinct().ToList(), test_id);

                listaCompleta =
                    (from dados in listaPaginada
                     select new ResponseChangeLogDTO
                     {
                         Ent_id = dados.Ent_id,
                         Usu_id = dados.Usu_id,
                         usu_login = users.Exists(p => p.usu_id == dados.Usu_id) ? users.FirstOrDefault(p => p.usu_id == dados.Usu_id).usu_login + (!string.IsNullOrEmpty(users.FirstOrDefault(p => p.usu_id == dados.Usu_id).pes_nome) ? " - " + users.FirstOrDefault(p => p.usu_id == dados.Usu_id).pes_nome : string.Empty) : string.Empty,
                         Dre_id = dados.Dre_id,
                         dre_nome = dresSchools.FirstOrDefault(p => p.dre_id == dados.Dre_id).dre_nome,
                         Esc_id = dados.Esc_id,
                         esc_nome = dresSchools.FirstOrDefault(p => p.esc_id == dados.Esc_id).esc_nome,
                         Tur_id = dados.Tur_id,
                         tur_nome = teams.FirstOrDefault(p => p.tur_id == dados.Tur_id).tur_codigo,
                         Test_id = dados.Test_id,
                         Alu_id = dados.Alu_id,
                         alu_nome = students.FirstOrDefault(p => p.alu_id == dados.Alu_id).alu_nome,
                         Item_Id = dados.Item_Id,
                         OrderItem = dados.Item_Id > 0 ? string.IsNullOrEmpty(blockItens.FirstOrDefault(p => p.Item_Id == dados.Item_Id).Order.ToString()) ? "-" : (blockItens.FirstOrDefault(p => p.Item_Id == dados.Item_Id).Order + 1).ToString() : "-",
                         valorAnterior = !string.IsNullOrEmpty(dados.Alternative_Anterior) ? dados.Alternative_Anterior : (dados.AbsenceReason_IdAnterior > 0 ? "Ausência (" + absenceReasons.FirstOrDefault(p => p.Id == dados.AbsenceReason_IdAnterior).Description + ")" : "-"),
                         valorAtual = !string.IsNullOrEmpty(dados.Alternative_Atual) ? dados.Alternative_Atual : (dados.AbsenceReason_IdAtual > 0 ? "Ausência (" + absenceReasons.FirstOrDefault(p => p.Id == dados.AbsenceReason_IdAtual).Description + ")" : "-"),
                         Automatic = dados.Automatic,
                         dataCriacao = dados.CreateDate
                     }).ToList();

            }
            return listaCompleta;
        }

        public List<StudentDTO> GetInfoStudents(List<long> alunos)
        {
            return _responseChangeLogRepository.GetInfoStudents(alunos);
        }

        public List<DresDTO> GetInfoDresSchools(List<string> dres, Guid ent_id)
        {
            return _responseChangeLogRepository.GetInfoDresSchools(dres.AsEnumerable().Select(x => string.Concat("'", x, "'")), ent_id);
        }

        public List<UsersDTO> GetInfoUsers(List<string> usuarios, Guid ent_id)
        {
            return _responseChangeLogRepository.GetInfoUsers(usuarios.AsEnumerable().Select(x => string.Concat("'", x, "'")), ent_id);
        }

        public List<TeamsDTO> GetInfoTeams(List<long> turmas)
        {
            return _responseChangeLogRepository.GetInfoTeams(turmas);
        }

        public List<BlockItem> GetBlockItens(List<long> itens, long test_id)
        {
            return _responseChangeLogRepository.GetBlockItens(itens, test_id);
        }
    }
}
