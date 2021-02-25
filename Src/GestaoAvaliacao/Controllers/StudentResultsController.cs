using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.DTO.StudentsTestSent;
using GestaoAvaliacao.Entities.DTO.Tests;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class StudentResultsController : Controller
    {
        private readonly ITestBusiness testBusiness;
        private readonly IAlternativeBusiness alternativeBusiness;
        private readonly IStudentCorrectionBusiness _studentCorrectionBusiness;
        private readonly ICorrectionBusiness correctionBusiness;
        private readonly IItemFileBusiness itemFileBusiness;
        private readonly IItemAudioBusiness itemAudioBusiness;

        public StudentResultsController(ITestBusiness testBusiness, IAlternativeBusiness alternativeBusiness, IStudentCorrectionBusiness _studentCorrectionBusiness, ICorrectionBusiness correctionBusiness,
            IItemFileBusiness itemFileBusiness, IItemAudioBusiness itemAudioBusiness)
        {
            this.testBusiness = testBusiness;
            this.alternativeBusiness = alternativeBusiness;
            this._studentCorrectionBusiness = _studentCorrectionBusiness;
            this.correctionBusiness = correctionBusiness;
            this.itemFileBusiness = itemFileBusiness;
            this.itemAudioBusiness = itemAudioBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }     
    }
}