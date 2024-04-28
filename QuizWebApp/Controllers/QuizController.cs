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
    public class QuizController : Controller
    {
        private QuizDBEntities objQuizDBEntities;
        public QuizController()
        {
            objQuizDBEntities = new QuizDBEntities();
        }
        // GET: Quiz
        public ActionResult Index()
        {
            QuizCategoryViewModel objQuizCategoryViewModel = new QuizCategoryViewModel();
            objQuizCategoryViewModel.CandidateName = User.Identity.Name;
            objQuizCategoryViewModel.ListOfCategory = (from objCat in objQuizDBEntities.Categories
                                                       select new SelectListItem()
                                                       {
                                                           Text = objCat.CategoryName,
                                                           Value = objCat.CategoryId.ToString()
                                                       }).ToList();

            return View(objQuizCategoryViewModel);
        }

        [HttpPost]
        public ActionResult Index(int categoryId)
        {
            if (categoryId != null)
            {
                Session["CandidateName"] = User.Identity.Name;
                Session["CategoryId"] = categoryId;

                return View("QuestionIndex");
            }
            return View();
        }

        public PartialViewResult UserQuestionAnswer(CandidateAnswer objCandidateAnswer)
        {
            bool IsLast = false;
            if(objCandidateAnswer.AnswerText != null)
            {
                List<CandidateAnswer> objCandidateAnswers = Session["CadQuestionAnswer"] as List<CandidateAnswer>;
                if(objCandidateAnswers == null)
                {
                    objCandidateAnswers = new List<CandidateAnswer>();
                }
                objCandidateAnswers.Add(objCandidateAnswer);
                Session["CadQuestionAnswer"] = objCandidateAnswers;
            }

            int pageSize = 1;
            int pageNumber = 0;
            int CategoryId = Convert.ToInt32(Session["CategoryId"]);

            if(Session["CadQuestionAnswer"] == null)
            {
                pageNumber = pageNumber + 1;
            }
            else
            {
                List<CandidateAnswer> canAnswer = Session["CadQuestionAnswer"] as List<CandidateAnswer>;
                pageNumber = canAnswer.Count + 1;
            }

            List<Question> listOfQuestion = new List<Question>();
            listOfQuestion = objQuizDBEntities.Questions.Where(model => model.CategoryId == CategoryId).ToList();

            if(pageNumber == listOfQuestion.Count)
            {
                IsLast = true;
            }

            Question objQuestion = new Question();
            objQuestion = listOfQuestion.Skip((pageNumber - 1) * pageSize).Take(pageSize).FirstOrDefault();            

            QuizQuestionAnswerViewModel objQuizQuestionAnswerViewModel = new QuizQuestionAnswerViewModel();
            objQuizQuestionAnswerViewModel.IsLast = IsLast;
            objQuizQuestionAnswerViewModel.QuestionId = objQuestion.QuestionId;
            objQuizQuestionAnswerViewModel.QuestionName = objQuestion.QuestionName;
            objQuizQuestionAnswerViewModel.ListOfQuizOption = (from obj in objQuizDBEntities.Options
                                                               where obj.QuestionId == objQuestion.QuestionId
                                                               select new QuizOption()
                                                               {
                                                                   OptionName = obj.OptionName,
                                                                   OptionId = obj.OptionId
                                                               }).ToList();

            return PartialView("QuizQuestionOption", objQuizQuestionAnswerViewModel);
        }

        public JsonResult SaveCandidateAnswer(CandidateAnswer objCandidateAnswer)
        {
            
            if (objCandidateAnswer.AnswerText != null)
            {
                List<CandidateAnswer> objCandidateAnswers = Session["CadQuestionAnswer"] as List<CandidateAnswer>;
                if (objCandidateAnswers == null)
                {
                    objCandidateAnswers = new List<CandidateAnswer>();
                }
                objCandidateAnswers.Add(objCandidateAnswer);
                Session["CadQuestionAnswer"] = objCandidateAnswers;
            }
            List<CandidateAnswer> canAnswer = Session["CadQuestionAnswer"] as List<CandidateAnswer>;
            foreach (var item in canAnswer)
            {
                Result objResult = new Result();
                objResult.AnswerText = item.AnswerText;
                objResult.QuestionId = item.QuestionId;
                objResult.UserId = Convert.ToInt32(Session["UserId"]);

                objQuizDBEntities.Results.Add(objResult);
                objQuizDBEntities.SaveChanges();
            }
            return Json( new { message = "Data successfully added", success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFinalResult()
        {
            List<CandidateAnswer> listOfQuestionAnswers = Session["CadQuestionAnswer"] as List<CandidateAnswer>;
            var UserResult = (from objResult in listOfQuestionAnswers
                              join objAnswer in objQuizDBEntities.Answers on objResult.QuestionId equals objAnswer.QuestionId
                              join objQuestion in objQuizDBEntities.Questions on objResult.QuestionId equals objQuestion.QuestionId
                              select new ResultViewModel()
                              {
                                  Question = objQuestion.QuestionName,
                                  Answer = objAnswer.AnswerText,
                                  AnswerByUser = objResult.AnswerText,
                                  Status = objAnswer.AnswerText == objResult.AnswerText ? "Correct" : "Wrong"
                              }
                              ).ToList();

            Session.Remove("CadQuestionAnswer");
            Session.Remove("CategoryId");

            return View(UserResult);
        }
    }
}