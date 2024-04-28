using QuizWebApp.Models;
using QuizWebApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizWebApp.Controllers
{
    [Authorize]
    public class ResultController : Controller
    {
        private QuizDBEntities objQuizDBEntities;
        public ResultController()
        {
            objQuizDBEntities = new QuizDBEntities();
        }
        // GET: Result
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CandidateResult()
        {
            var userName = User.Identity.Name;
            var candidateResult = (from objUser in objQuizDBEntities.Users
                                   where objUser.UserName == userName
                                   join objResult in objQuizDBEntities.Results on objUser.UserId equals objResult.UserId
                                   join objQues in objQuizDBEntities.Questions on objResult.QuestionId equals objQues.QuestionId
                                   join objAns in objQuizDBEntities.Answers on objQues.QuestionId equals objAns.QuestionId
                                   select new CandidateResultViewModel()
                                   {                                     
                                       Question = objQues.QuestionName,
                                       Answer = objAns.AnswerText,
                                       AnswerByUser = objResult.AnswerText,
                                       Status = objAns.AnswerText == objResult.AnswerText ? "Correct" : "Wrong"
                                   }
                                   ).ToList();

            ViewBag.CandidateName = userName;

            return View(candidateResult);
        }
    }
}