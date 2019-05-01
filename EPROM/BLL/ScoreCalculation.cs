using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BAL;
using Common;
using System.Data;

namespace BLL
{
    public class ScoreCalculation
    {
        Score objScore = null;
        Factor objFactor = null;

        public static DataSet dsStatic = new DataSet();

        public DataSet ReadXmlFile(string FilePath)
        {
            dsStatic.ReadXml(FilePath);
            return dsStatic;
        }

        public string GetEpromScore(ResponseCustomClass response)
        {
            if (response.Eprom_title.Trim() == "PROMIS Global 10")
            {
                return Create_PROMISG10_ScoreTable(response);
            }
            else if (response.Eprom_title.Trim() == "KOOS-Physical Function Shortform (KOOS-PS)")
            {
                return Calculate_KOOS_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Hip disability and Osteoarthritis Outcome Score (HOOS)")
            {
                return Calculate_HOOS_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Orebro Musculoskeletal Pain Questionnaire")
            {
                return Calculate_OMPQ_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Neck Disability Index - Vernon and Mior Cervical Spine Questionnaire")
            {
                return Calculate_NeckPain_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Kessler Psychological Distress Scale (K10)")
            {
                return Calculate_K10_Score(response);
            }
            else if (response.Eprom_title.Trim() == "QuickDASH Questionnaire")
            {
                return Calculate_QDASH_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Modified Oswestry Low Back Disability Questionnaire")
            {
                return Calculate_Oswestry_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Preventive ePROMs for Population Health Management by GPs ™")
            {
                return Calculate_Preventive_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Cared-For ePROM")
            {
                return Calculate_CFE_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Preventive ePROMs for Population Health Management™")
            {
                return Calculate_Preventive_Score(response);
            }
            else if (response.Eprom_title.Trim() == "PROMIS Global SF10")
            {
                return Create_PROMISG10_ScoreTable(response);
            }
            else if (response.Eprom_title.Trim() == "COPD Assessment Test ©")
            {
                return Calculate_COPD_Score(response);
            }
            else if (response.Eprom_title.Trim() == "PROMIS 29 Profile v1.0")
            {
                return Calculate_PROMISE_29_Score(response);
            }
            else if (response.Eprom_title.Trim() == "K5 ePROM")
            {
                return Calculate_K5_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Kessler 10 Plus (K10+)")
            {
                return Calculate_K10_Plus_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Depression Anxiety Stress Scale")
            {
                return Calculate_DASS_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Strengths and Difficulties Questionnaire YR101")
            {
                return Calculate_SDQ_Score(response);
            }
            else if (response.Eprom_title.Trim() == "Cataracts ePROM")
            {
                return Calculate_Cataracts_Score(response);
            }

            return "";
        }

        #region PROMISG10

        public string Create_PROMISG10_ScoreTable(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int scorehealth = 0, scoreQuality = 0, scorePhysicalHealth = 0, scoreMentalHealth = 0, scoreSatisfaction = 0, scoreRoles = 0, scoreWalking = 0, scoreDepressed = 0, scoreFatigue = 0, scorePainAverage = 0, globalMentalScore = 0, globalPhysicalScore = 0;

                double PhysicalAdjustedScore = 0.0, MentalAdjustedScore = 0.0;

                for (int i = 0; i < List.Count; i++)
                {
                    if (List[i].question_title == "In general, would you say your health is:")
                    {
                        scorehealth = GetScore_PROMISG10(List[i].answer_text);
                    }
                    else if (List[i].question_title == "In general, would you say your quality of life is:")
                    {
                        scoreQuality = GetScore_PROMISG10(List[i].answer_text);
                    }
                    else if (List[i].question_title == "In general, how would you rate your physical health?")
                    {
                        scorePhysicalHealth = GetScore_PROMISG10(List[i].answer_text);
                    }
                    else if (List[i].question_title == "In general, how would you rate your mental health, including your mood and your ability to<br>think?")
                    {
                        scoreMentalHealth = GetScore_PROMISG10(List[i].answer_text);
                    }
                    else if (List[i].question_title == "In general, how would you rate your satisfaction with your social activities and relationships?")
                    {
                        scoreSatisfaction = GetScore_PROMISG10(List[i].answer_text);
                    }
                    else if (List[i].question_title == "In general, please rate how well you carry out your usual social activities and roles. (This<br>includes activities at home, at work and in your community, and responsibilities as a parent,  child, spouse, employee, friend, etc.)")
                    {
                        scoreRoles = GetScore_PROMISG10(List[i].answer_text);
                    }
                    else if (List[i].question_title == "To what extent are you able to carry out your everyday physical activities such as walking,  climbing stairs, carrying groceries, or moving a chair?")
                    {
                        scoreWalking = GetScore_PROMISG10(List[i].answer_text);
                    }
                    else if (List[i].question_title == "In the past 7 days, How often have you been bothered by emotional problems such as feeling anxious  <br>depressed or irritable?")
                    {
                        scoreDepressed = GetScore_PROMISG10(List[i].answer_text);
                    }
                    else if (List[i].question_title == "How would you rate your fatigue on average?")
                    {
                        scoreFatigue = GetScore_PROMISG10(List[i].answer_text);
                    }
                    else if (List[i].question_title == "On a scale of 0 to 10 (where 0 is No Pain, and 10 is Worst Pain Imaginable) how would you rate your pain on average?")
                    {
                        scorePainAverage = GetPainAVGScore_PROMISG10(List[i].answer_text);
                    }
                    else if (List[i].question_title == "What is your gender?")
                    {
                        Entities.SetCookie(List[i].answer_text, "Gender");
                    }
                    else if (List[i].question_title == "What is your age?")
                    {
                        Entities.SetCookie(List[i].answer_text, "Age");
                    }
                    else if (List[i].question_title == "Please provide the postcode of where you live")
                    {
                        Entities.SetCookie(List[i].answer_text, "PostCode");
                    }
                }

                globalPhysicalScore = scorePhysicalHealth + scoreWalking + scoreFatigue + scorePainAverage;

                globalMentalScore = scoreQuality + scoreMentalHealth + scoreSatisfaction + scoreDepressed;

                #region GlobalPhysicalAdjustedScore

                PhysicalAdjustedScore = GetPROMISG10_GlobalPhysicalScore(globalPhysicalScore);

                #endregion GlobalPhysicalAdjustedScore

                #region GlobalMentalAdjustedScore

                MentalAdjustedScore = GetPROMISG10_GlobalMentalScore(globalMentalScore);

                #endregion GlobalMentalAdjustedScore

                List<Score> list = new List<Score>();

                objScore = new Score();
                objScore.Title = "Global Physical Health Adjusted Score";
                objScore.Value = PhysicalAdjustedScore;
                list.Add(objScore);

                objScore = new Score();
                objScore.Title = "Global Mental Health Adjusted Score";
                objScore.Value = MentalAdjustedScore;
                list.Add(objScore);
                string str = JsonConvert.SerializeObject(list);

                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_PROMISG10(string strTitle)
        {
            int ScorePROMISG10 = 0;
            DataTable dtStatic = dsStatic.Tables["PROMISG10"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScorePROMISG10 = Convert.ToInt32(staticValue);
                    }
                }
            }
            return ScorePROMISG10;
        }

        public int GetPainAVGScore_PROMISG10(string strTitle)
        {
            int ScorePROMISG10 = 0;
            DataTable dtStatic = dsStatic.Tables["PROMISG10Score"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScorePROMISG10 = Convert.ToInt32(staticValue);
                    }
                }
            }
            return ScorePROMISG10;
        }

        public double GetPROMISG10_GlobalPhysicalScore(int Title)
        {
            double ScorePhysical = 0;
            DataTable dtStatic = dsStatic.Tables["PROMISG10_GlobalPhysicalScore"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (Title.ToString() == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScorePhysical = Convert.ToDouble(staticValue);
                    }
                }
                else if (Title > 20 && staticText == "20")
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScorePhysical = Convert.ToDouble(staticValue);
                    }
                }
            }
            return ScorePhysical;
        }

        public double GetPROMISG10_GlobalMentalScore(int Title)
        {
            double ScoreMental = 0;
            DataTable dtStatic = dsStatic.Tables["PROMISG10_GlobalMentalScore"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (Title.ToString() == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreMental = Convert.ToDouble(staticValue);
                    }
                }
                else if (Title > 20 && staticText == "20")
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreMental = Convert.ToDouble(staticValue);
                    }
                }
            }
            return ScoreMental;
        }

        #endregion PROMISG10

        #region KOOS       

        public string Calculate_KOOS_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int scoreRising = 0, scoreStockings = 0, scoreSitting = 0, scoreBending = 0, scoreTwisting = 0, scoreKneeling = 0, scoreSquatting = 0, KoosScore = 0;
                double TotalKOOSScore = 0;
                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "Rising from bed")
                        {
                            scoreRising = GetScore_KOOS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Putting on socks/stockings")
                        {
                            scoreStockings = GetScore_KOOS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Rising from sitting")
                        {
                            scoreSitting = GetScore_KOOS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Bending to floor")
                        {
                            scoreBending = GetScore_KOOS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Twisting/pivoting on your injured knee")
                        {
                            scoreTwisting = GetScore_KOOS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Kneeling")
                        {
                            scoreKneeling = GetScore_KOOS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Squatting")
                        {
                            scoreSquatting = GetScore_KOOS(List[i].answer_text);
                        }
                    }
                }

                #region KOOSScore      

                KoosScore = scoreRising + scoreStockings + scoreSitting + scoreBending + scoreTwisting + scoreKneeling + scoreSquatting;

                TotalKOOSScore = GetLevelValue_KOOS(KoosScore);

                #endregion KOOSScore

                objScore = new Score();
                objScore.Title = "KOOS Score";
                objScore.Value = TotalKOOSScore;

                List<Score> list = new List<Score>();
                list.Add(objScore);
                string str = JsonConvert.SerializeObject(list);

                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_KOOS(string strTitle)
        {
            int ScoreKOOS = 0;
            DataTable dtStatic = dsStatic.Tables["KOOS"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreKOOS = Convert.ToInt32(staticValue);
                    }
                }
            }
            return ScoreKOOS;
        }

        public double GetLevelValue_KOOS(int Score)
        {
            double ScoreKOOS = 0;
            DataTable dtStatic = dsStatic.Tables["KoosLevelScore"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                var staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (Score.ToString() == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreKOOS = Convert.ToDouble(staticValue);
                    }
                }
                else if (Score > 20 && staticText == "20")
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreKOOS = Convert.ToDouble(staticValue);
                    }
                }
            }
            return ScoreKOOS;
        }

        #endregion KOOS

        #region HOOS  

        public string Calculate_HOOS_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int scorestairs = 0, scoreShower = 0, scoreSitting = 0, scoreRunning = 0, scoreTwisting = 0, HoosScore = 0;
                double TotalHOOSScore = 0;

                for (int i = 0; i < List.Count; i++)
                {
                    if (List[i].question_title == "Descending stairs")
                    {
                        scorestairs = GetScore_HOOS(List[i].answer_text);
                    }
                    else if (List[i].question_title == "Getting in/out of bath or shower")
                    {
                        scoreShower = GetScore_HOOS(List[i].answer_text);
                    }
                    else if (List[i].question_title == "Sitting")
                    {
                        scoreSitting = GetScore_HOOS(List[i].answer_text);
                    }
                    else if (List[i].question_title == "Running")
                    {
                        scoreRunning = GetScore_HOOS(List[i].answer_text);
                    }
                    else if (List[i].question_title == "Twisting/pivoting on your loaded leg")
                    {
                        scoreTwisting = GetScore_HOOS(List[i].answer_text);
                    }
                }

                HoosScore = scorestairs + scoreShower + scoreSitting + scoreRunning + scoreTwisting;
                TotalHOOSScore = GetLevelValue_HOOS(HoosScore);

                objScore = new Score();
                objScore.Title = "HOOS Score";
                objScore.Value = TotalHOOSScore;

                List<Score> list = new List<Score>();
                list.Add(objScore);
                string str = JsonConvert.SerializeObject(list);

                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_HOOS(string strTitle)
        {
            int ScoreHOOS = 0;
            DataTable dtStatic = dsStatic.Tables["HOOS"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreHOOS = Convert.ToInt32(staticValue);
                    }
                }
                else if (strTitle == "")
                {
                    ScoreHOOS = int.MinValue;
                }
            }
            return ScoreHOOS;
        }

        public double GetLevelValue_HOOS(int Title)
        {
            double ScoreHOOS = 0;
            DataTable dtStatic = dsStatic.Tables["HOOSLevelScore"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                var staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (Title.ToString() == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreHOOS = Convert.ToDouble(staticValue);
                    }
                }
                else if (Title > 20 && staticText == "20")
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreHOOS = Convert.ToDouble(staticValue);
                    }
                }
            }
            return ScoreHOOS;
        }
        #endregion HOOS

        #region OMPQ

        public string Calculate_OMPQ_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int scoreNeck = 0, scoreShoulder = 0, scoreArm = 0, scoreUpperBack = 0, scoreLowerBack = 0, scoreLeg = 0, scoreOther = 0, TotalInjury = 0, scoreDaysOfWork = 0, scoreCurrentPain = 0, scoreMonotonous = 0, scorePainRate = 0, scorePainScale = 0, scorePainExperienced = 0, scorePainDecrease = 0, scoreJobSatisfied = 0, scoreTense = 0, scoreDepressed = 0, scorePainPersistent = 0, scoreWorkChance = 0, scorePhysicalActivity = 0, scorePainIndication = 0, scorePresentPain = 0, scoreLightWork = 0, scoreWalk = 0, scoreOrdinaryHousehold = 0, scoreShopping = 0, scoreSleep = 0, TotalOMPQScore = 0, EmailScore = 0;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        string strTitle = List[i].question_title;

                        if (strTitle == "Where do you have pain? Place a tick for all appropriate sites.")
                        {
                            if (List[i].answer_text.Contains("Neck"))
                                scoreNeck = 1;

                            if (List[i].answer_text.Contains("Shoulder"))
                                scoreShoulder = 1;

                            if (List[i].answer_text.Contains("Arm"))
                                scoreArm = 1;

                            if (List[i].answer_text.Contains("Upper Back"))
                                scoreUpperBack = 1;

                            if (List[i].answer_text.Contains("Lower Back"))
                                scoreLowerBack = 1;

                            if (List[i].answer_text.Contains("Leg"))
                                scoreLeg = 1;


                            if (List[i].answer_text.Contains("Other (state)"))
                                scoreOther = 1;

                        }
                        else if (strTitle == "How many days of work have you missed because of pain during the past 18 months? Tick  one.")
                        {
                            scoreDaysOfWork = GetScore_OMPQ(List[i].answer_text);
                        }
                        else if (strTitle == "How long have you had your current pain problem? Tick  one.")
                        {
                            scoreCurrentPain = GetScore_OMPQ(List[i].answer_text);
                        }
                        else if (strTitle == "Is your work heavy or monotonous? -")
                        {
                            scoreMonotonous = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "How would you rate the pain that you have had during the past week? -")
                        {
                            scorePainRate = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "In the past three months, on average, how bad was your pain on a 0-10 scale? Select one. -")
                        {
                            scorePainScale = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "How often would you say that you have experienced pain episodes, on average, during the past three<br>months? Select one. -")
                        {
                            scorePainExperienced = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "Based on all things you do to cope, or deal with your pain, on an average day, how much are you<br>able to decrease it? Select one. -")
                        {
                            scorePainDecrease = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "If you take into consideration your work routines, management, salary, promotion possibilities and<br>work mates, how satisfied are you with your job? Select one. -")
                        {
                            scoreJobSatisfied = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "How tense or anxious have you felt in the past week? Select one. -")
                        {
                            scoreTense = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "How much have you been bothered by feeling depressed in the past week? Select one. -")
                        {
                            scoreDepressed = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "In your view, how large is the risk that your current pain may become persistent? Select one. -")
                        {
                            scorePainPersistent = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "In your estimation, what are the chances that you will be able to work in six months? Select one. -")
                        {
                            scoreWorkChance = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "Physical activity makes my pain worse.")
                        {
                            scorePhysicalActivity = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "An increase in pain is an indication that I should stop what I’m doing until the pain decreases")
                        {
                            scorePainIndication = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "I should not do my normal work with my present pain.")
                        {
                            scorePresentPain = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "I can do light work for an hour.")
                        {
                            scoreLightWork = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "I can walk for an hour.")
                        {
                            scoreWalk = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "I can do ordinary household chores.")
                        {
                            scoreOrdinaryHousehold = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "I can do the weekly shopping")
                        {
                            scoreShopping = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                        else if (strTitle == "I can sleep at night.")
                        {
                            scoreSleep = GetScore_OMPQQuestions(List[i].answer_text);
                        }
                    }
                }

                TotalInjury = scoreNeck + scoreShoulder + scoreArm + scoreUpperBack + scoreLowerBack + scoreLeg + scoreOther;

                #region OMPQScore
                int InjuryScore = 0;

                if (TotalInjury > 4)
                    InjuryScore = 10;
                else
                    InjuryScore = (TotalInjury * 2);

                TotalOMPQScore = InjuryScore + (scoreDaysOfWork + scoreCurrentPain + scoreMonotonous + scorePainRate + scorePainScale + scorePainExperienced + scoreJobSatisfied + scoreTense + scoreDepressed + scorePhysicalActivity + scorePainIndication + scorePresentPain) + 80 - (scorePainDecrease + scorePainPersistent + scoreWorkChance + scoreLightWork + scoreWalk + scoreOrdinaryHousehold + scoreShopping + scoreSleep);

                EmailScore = TotalOMPQScore;

                #endregion OMPQScore

                objScore = new Score();
                objScore.Title = "OMPQ Score";
                objScore.Value = TotalOMPQScore;

                List<Score> list = new List<Score>();
                list.Add(objScore);
                string str = JsonConvert.SerializeObject(list);
                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_OMPQ(string strTitle)
        {
            int ScoreOMPQ = 0;
            DataTable dtStatic = dsStatic.Tables["OMPQ"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreOMPQ = Convert.ToInt32(staticValue);
                    }
                }
            }
            return ScoreOMPQ;
        }

        public int GetScore_OMPQQuestions(string strTitle)
        {
            int ScoreOMPQ = 0;
            DataTable dtStatic = dsStatic.Tables["OMPQ_Question_Value"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreOMPQ = Convert.ToInt32(staticValue);
                    }
                }
                else if (strTitle.Contains(staticText))
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreOMPQ = Convert.ToInt32(staticValue);
                    }
                }
            }
            return ScoreOMPQ;
        }

        #endregion OMPQ

        #region NeckPain       

        public string Calculate_NeckPain_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int scorePainIntensity = 0, scorePersonalCare = 0, scoreLifting = 0, scoreReading = 0, scoreHeadaches = 0, scoreConcentrating = 0, scoreWork = 0, scoreDriving = 0, scoreSleeping = 0, scoreRecreation = 0, TotalNeckPainScore = 0;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "Section 1 - Pain Intensity")
                        {
                            scorePainIntensity = GetScore_NeckPain(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 2 - Personal Care (e.g., Washing, Dressing)")
                        {
                            scorePersonalCare = GetScore_NeckPain(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 3 - Lifting")
                        {
                            scoreLifting = GetScore_NeckPain(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 4 - Reading")
                        {
                            scoreReading = GetScore_NeckPain(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 5 - Headaches")
                        {
                            scoreHeadaches = GetScore_NeckPain(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 6 - Concentrating")
                        {
                            scoreConcentrating = GetScore_NeckPain(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 7 - Work")
                        {
                            scoreWork = GetScore_NeckPain(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 8 - Driving")
                        {
                            scoreDriving = GetScore_NeckPain(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 9 - Sleeping")
                        {
                            scoreSleeping = GetScore_NeckPain(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 10 - Recreation")
                        {
                            scoreRecreation = GetScore_NeckPain(List[i].answer_text);
                        }
                    }
                }

                TotalNeckPainScore = scorePainIntensity + scorePersonalCare + scoreLifting + scoreReading + scoreHeadaches + scoreConcentrating + scoreWork + scoreDriving + scoreSleeping + scoreRecreation;

                objScore = new Score();
                objScore.Title = "NECKPAIN Score";
                objScore.Value = TotalNeckPainScore;

                List<Score> list = new List<Score>();
                list.Add(objScore);
                string str = JsonConvert.SerializeObject(list);
                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_NeckPain(string strTitle)
        {
            int ScoreNeckPain = 0;
            DataTable dtStatic = dsStatic.Tables["NeckPain"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreNeckPain = Convert.ToInt32(staticValue);
                    }
                }
            }
            return ScoreNeckPain;
        }

        #endregion NeckPain

        #region K10  

        public string Calculate_K10_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int scoreTired = 0, scoreNervous = 0, scoreCalm = 0, scoreHopeless = 0, scoreFidgety = 0, scoreRestless = 0, scoreDepressed = 0, scoreEffort = 0, scoreCheer = 0, K10Score = 0, scoreWorthless = 0;
                double TotalK10Score = 0;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "During the last 30 days, about how often did you feel tired out for no good reason?")
                        {
                            scoreTired = GetScore_K10(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the last 30 days, about how often did you feel nervous?")
                        {
                            scoreNervous = GetScore_K10(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the last 30 days, about how often did you feel so nervous that nothing could calm you<br>down?")
                        {
                            scoreCalm = GetScore_K10(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the last 30 days, about how often did you feel hopeless?")
                        {
                            scoreHopeless = GetScore_K10(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the last 30 days, about how often did you feel restless or fidgety?")
                        {
                            scoreFidgety = GetScore_K10(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the last 30 days, about how often did you feel so restless you could not sit still?")
                        {
                            scoreRestless = GetScore_K10(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the last 30 days, about how often did you feel depressed?")
                        {
                            scoreDepressed = GetScore_K10(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the last 30 days, about how often did you feel that everything was an effort?")
                        {
                            scoreEffort = GetScore_K10(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the last 30 days, about how often did you feel so sad that nothing could cheer you up?")
                        {
                            scoreCheer = GetScore_K10(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the last 30 days, about how often did you feel worthless?")
                        {
                            scoreWorthless = GetScore_K10(List[i].answer_text);
                        }
                    }
                }

                //This below code is commented on 28 oct 2016 as the client requeted to do so.
                //K10Score = scoreTired + scoreNervous + scoreCalm + scoreHopeless + scoreFidgety;
                //TotalK10Score = GetLevelValue_K10(K10Score);

                K10Score = scoreTired + scoreNervous + scoreCalm + scoreHopeless + scoreFidgety + scoreRestless + scoreDepressed + scoreEffort + scoreCheer + scoreWorthless;
                TotalK10Score = K10Score;

                objScore = new Score();
                objScore.Title = "K10 Score";
                objScore.Value = TotalK10Score;

                List<Score> list = new List<Score>();
                list.Add(objScore);
                string str = JsonConvert.SerializeObject(list);
                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_K10(string strTitle)
        {
            int ScoreK10 = 0;
            DataTable dtStatic = dsStatic.Tables["K10"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreK10 = Convert.ToInt32(staticValue);
                    }
                }
            }
            return ScoreK10;
        }

        public double GetLevelValue_K10(int Score)
        {
            double ScoreK10 = 0;
            DataTable dtStatic = dsStatic.Tables["K10LevelScore"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                var staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (Score.ToString() == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreK10 = Convert.ToDouble(staticValue);
                    }
                }
                else if (Score > 20 && staticText == "20")
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreK10 = Convert.ToDouble(staticValue);
                    }
                }
            }
            return ScoreK10;
        }

        #endregion K10

        #region QDASH       

        public string Calculate_QDASH_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int scoreTight = 0, scoreChores = 0, scoreShopping = 0, scoreWash = 0, scoreKnife = 0, scoreRecreational = 0, scoreSocial = 0, scoreDaily = 0, scoreSymptoms = 0, scoreTingling = 0, scoreDifficulty = 0;

                decimal TotalQDashScore = 0;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "Open a tight or new jar")
                        {
                            scoreTight = GetScore_QDASH(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Do heavy household chores (e.g. wash walls, wash floors)")
                        {
                            scoreChores = GetScore_QDASH(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Carry a shopping bag or briefcase")
                        {
                            scoreShopping = GetScore_QDASH(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Wash your back")
                        {
                            scoreWash = GetScore_QDASH(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Use a knife to cut food")
                        {
                            scoreKnife = GetScore_QDASH(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Recreational activities in which you take some force or impact through your arm, shoulder or hand (e.g. golf, hammering, tennis, etc.)")
                        {
                            scoreRecreational = GetScore_QDASH(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the past week, to what extent has your arm, shoulder or hand problem interfered with your normal social activities with family, friends, neighbours or groups?")
                        {
                            scoreSocial = GetScore_QDASH(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the past week, were you limited in your work or other regular daily activities as a result of your arm, shoulder or hand problem?")
                        {
                            scoreDaily = GetScore_QDASH(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Arm, shoulder or hand pain")
                        {
                            scoreSymptoms = GetScore_QDASH(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Tingling (pins and needles) in your arm, shoulder or hand")
                        {
                            scoreTingling = GetScore_QDASH(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the past week, how much difficulty have you had sleeping because of the pain in your arm, shoulder or hand?")
                        {
                            scoreDifficulty = GetScore_QDASH(List[i].answer_text);
                        }
                    }
                }

                decimal Sum = scoreTight + scoreChores + scoreShopping + scoreWash + scoreKnife + scoreRecreational + scoreSocial + scoreDaily + scoreSymptoms + scoreTingling + scoreDifficulty;

                decimal value1 = Sum / 11;

                decimal TotalScore = (value1 - 1) * 25;
                TotalQDashScore = Math.Round(TotalScore, 1, MidpointRounding.ToEven);

                objScore = new Score();
                objScore.Title = "QDASH Score";
                objScore.Value = Convert.ToDouble(TotalQDashScore);

                List<Score> list = new List<Score>();
                list.Add(objScore);
                string str = JsonConvert.SerializeObject(list);
                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_QDASH(string strTitle)
        {
            int ScoreQDASH = 0;
            DataTable dtStatic = dsStatic.Tables["QDASH"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreQDASH = Convert.ToInt32(staticValue);
                    }
                }
            }
            return ScoreQDASH;
        }

        #endregion QDASH

        #region Oswestry       

        public string Calculate_Oswestry_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int scorePainIntensity = 0, scorePersonalCare = 0, scoreLifting = 0, scoreWalking = 0, scoreSitting = 0, scoreStanding = 0, scoreSleeping = 0, scoreSocialLife = 0, scoreTraveling = 0, scoreHomeMaking = 0, TotalOwestreyScore = 0;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "Section 1 - Pain Intensity")
                        {
                            scorePainIntensity = GetScore_Oswestry(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 2 - Personal Care (e.g., Washing, Dressing)")
                        {
                            scorePersonalCare = GetScore_Oswestry(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 3 - Lifting")
                        {
                            scoreLifting = GetScore_Oswestry(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 4 - Walking")
                        {
                            scoreWalking = GetScore_Oswestry(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 5 - Sitting")
                        {
                            scoreSitting = GetScore_Oswestry(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 6 - Standing")
                        {
                            scoreStanding = GetScore_Oswestry(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 7 - Sleeping")
                        {
                            scoreSleeping = GetScore_Oswestry(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 8 - Social Life")
                        {
                            scoreSocialLife = GetScore_Oswestry(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 9 - Traveling")
                        {
                            scoreTraveling = GetScore_Oswestry(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Section 10 - Employment/Home-making")
                        {
                            scoreHomeMaking = GetScore_Oswestry(List[i].answer_text);
                        }
                    }
                }

                TotalOwestreyScore = scorePainIntensity + scorePersonalCare + scoreLifting + scoreWalking + scoreSitting + scoreStanding + scoreSleeping + scoreSocialLife + scoreTraveling + scoreHomeMaking;

                objScore = new Score();
                objScore.Title = "OWESTREY Score";
                objScore.Value = TotalOwestreyScore;

                List<Score> list = new List<Score>();
                list.Add(objScore);
                string str = JsonConvert.SerializeObject(list);
                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_Oswestry(string strTitle)
        {
            int ScoreOswestry = 0;
            DataTable dtStatic = dsStatic.Tables["OSWESTRY"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreOswestry = Convert.ToInt32(staticValue);
                    }
                }
            }
            return ScoreOswestry;
        }
        #endregion Oswestry

        #region Preventive       

        public string Calculate_Preventive_Score(ResponseCustomClass response)
        {
            string kg = "";
            string cm = "";
            string BMI = "no risk factor";
            string HealthyVegetableIntake = "";
            string PhysicalActivity = "";
            string Smoking = "";
            string AlcoholIntake = "";
            string FruitIntake = "";

            try
            {
                var List = response.responselist;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title.Replace("\n", "").ToString() == "How much do you weigh (without clothes) in kilograms?")
                        {
                            kg = List[i].answer_text;
                        }
                        else if (List[i].question_title.Replace("\n", "").ToString() == "How tall are you without shoes in centimetres?")
                        {
                            cm = List[i].answer_text;
                        }
                        else if (List[i].question_title.Replace("\n", "").ToString() == "How many serves of vegetables do you usually eat each day?<br><br>A serve of vegetables is defined as about 75 grams or:<br><ul><li>½ cup cooked green or orange vegetables (for example, broccoli, spinach, carrots or pumpkin)</li><li>½ cup cooked, dried or canned beans, peas or lentils (no added salt preferred)</li><li>1 cup green leafy or raw salad vegetables</li><li>½ cup sweet corn</li><li>½ medium potato or other starchy vegetables (sweet potato, taro or cassava)</li><li>1 medium tomato</li></ul>")
                        {
                            string retVal = List[i].answer_text;

                            if (List[i].answer_text == "1 to less than 5 serves")
                            {
                                retVal = "TEMP1";
                            }

                            HealthyVegetableIntake = List[i].answer_text + "_" + GetScore_Preventive(retVal);
                        }
                        else if (List[i].question_title.Replace("\n", "").ToString() == "Physical Activity<br><br>Over the last week, were you:<br><ol><li>Active on most, preferably all days (at least 10 minutes continuous walking each day)</li><li>Able to accumulate 150 to 300 minutes (2 ½ to 5 hours) of moderate intensity physical activity or 75 to 150 minutes (1 ¼ to 2 ½ hours) of vigorous intensity physical activity, or an equivalent combination of both moderate and vigorous activities, and</li><li>Able to undertake muscle strengthening activities on at least 2 days of the week.</li></ol>")
                        {
                            string retVal = List[i].answer_text;

                            if (List[i].answer_text == "I achieved  at least 150 minutes of moderate intensity physical activity (spread over less than 5 days last week)" || List[i].answer_text == "I was able to achieve items 2 and 3 above")
                            {
                                retVal = "TEMP1";
                            }

                            PhysicalActivity = List[i].answer_text + "_" + GetScore_Preventive(retVal);
                        }
                        else if (List[i].question_title.Replace("\n", "").ToString() == "Current Smoking<br><br>Which of the following best describes your smoking status?")
                        {
                            string retVal = List[i].answer_text;

                            if (List[i].answer_text == "Have tried a few time but never smoked regularly")
                            {
                                retVal = "TEMP2";
                            }

                            Smoking = List[i].answer_text + "_" + GetScore_Preventive(retVal);
                        }
                        else if (List[i].question_title.Replace("\n", "").ToString() == "<p>On a day when you drink alcohol, how many standard drinks do you usually have?</p><p>A standard drink is equal to:</p><ul><li>1 middy of full strength beer</li><li>1 schooner of light beer</li><li>1 small glass of wine or</li><li>one pub-sized nip of spirits</li></ul>")
                        {
                            AlcoholIntake = List[i].answer_text + "_" + GetScore_Preventive(List[i].answer_text);
                        }
                        else if (List[i].question_title.Replace("\n", "").ToString() == "What is your intake of “serves” of fruit that you usually eat each day.<br><br>A  serve of fruit is defined as approximately 150 grams of fresh fruit which is:<br><ul><li>1 piece of medium-sized fruit or 2 pieces of small fruit or</li><li>1 cup diced, cooked or canned fruit or </li><li>½ cup 100% fruit juice (only to be used occasionally as a substitute for other foods in the group) or </li><li>30g dried fruit (only to be used occasionally as a substitute for other foods in the group)</li></ul>")
                        {
                            FruitIntake = List[i].answer_text + "_" + GetScore_Preventive(List[i].answer_text);
                        }
                        else if (List[i].question_title == "What is your gender?")
                        {
                            Entities.SetCookie(List[i].answer_text, "Gender");
                        }
                        else if (List[i].question_title == "What is your age?")
                        {
                            Entities.SetCookie(List[i].answer_text, "Age");
                        }
                        else if (List[i].question_title == "Please provide the postcode of where you live")
                        {
                            Entities.SetCookie(List[i].answer_text, "PostCode");
                        }
                    }
                }

                List<Factor> list = new List<Factor>();

                decimal result = ((Convert.ToDecimal(cm) / 100) * (Convert.ToDecimal(cm) / 100));
                decimal finalResult = Convert.ToDecimal(kg) / result;

                if (finalResult >= 25)
                {
                    BMI = "risk factor";
                }

                objFactor = new Factor();
                objFactor.Title = "BMI";
                objFactor.Value = String.Format("{0:0.00}", finalResult) + "_" + BMI;
                list.Add(objFactor);

                objFactor = new Factor();
                objFactor.Title = "Vegetable Intake";
                objFactor.Value = HealthyVegetableIntake;
                list.Add(objFactor);

                objFactor = new Factor();
                objFactor.Title = "Physical Activity";
                objFactor.Value = PhysicalActivity;
                list.Add(objFactor);

                objFactor = new Factor();
                objFactor.Title = "Smoking";
                objFactor.Value = Smoking;
                list.Add(objFactor);

                objFactor = new Factor();
                objFactor.Title = "Alcohol Intake";
                objFactor.Value = AlcoholIntake;
                list.Add(objFactor);

                objFactor = new Factor();
                objFactor.Title = "Fruit Intake";
                objFactor.Value = FruitIntake;
                list.Add(objFactor);

                string str = JsonConvert.SerializeObject(list);

                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public string GetScore_Preventive(string strTitle)
        {
            string strPreventive = "";
            DataTable dtStatic = dsStatic.Tables["PREVENTICVE"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        strPreventive = staticValue.ToString();
                    }
                }
            }
            return strPreventive;
        }
        #endregion Preventive

        #region CFE

        public string Calculate_CFE_Score(ResponseCustomClass response)
        {
            double finalResult = 0.0;
            int cntCHKBOX = 0;
            int score1 = 0;
            int score2 = 0;
            int score3 = 0;
            int score4 = 0;
            int score5 = 0;
            int score6 = 0;
            int score7 = 0;
            int score8 = 0;
            int score9 = 0;
            int score10 = 0;
            int score11 = 0;
            int score12 = 0;
            int score13 = 0;
            int score14 = 0;
            int score15 = 0;
            int score16 = 0;
            int score17 = 0;

            try
            {
                var List = response.responselist;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "Someone you can count on to listen to you when you need to talk")
                        {
                            score1 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Someone to give you information to help you understand a situation")
                        {
                            score2 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Someone to give you good advice about a crisis")
                        {
                            score3 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Someone to confide in or talk to about yourself or your problems")
                        {
                            score4 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Someone whose advice you really want")
                        {
                            score5 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Someone to share your most private worries and fears with")
                        {
                            score6 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Someone to turn to for suggestions about how to deal with a personal problem")
                        {
                            score7 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Someone who understands your problems")
                        {
                            score8 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Someone to help you if you were confined to bed")
                        {
                            score9 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Someone to take you to the doctor if you needed it")
                        {
                            score10 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Someone to prepare your meals if you were unable to do it yourself")
                        {
                            score11 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Someone to help with daily chores if you were sick")
                        {
                            score12 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Convenience of location, Booking availability, telephone/internet access, Clinic waiting time")
                        {
                            score13 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Time spent, Explanation, Technical Skills, Personnel Manner, Visit overall,")
                        {
                            score14 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Continuity and choice of care")
                        {
                            score15 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "Would you rate whether there is likely to be a Focus including questions that relate to your desired outcomes of the consultation and how you might minimise future illness episodes")
                        {
                            score16 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "How often could you say \"I am confident that the consultation with my medical practitioner will lead to a treatment that will succeed in improving my health\"")
                        {
                            score17 = GetScore_CFE(List[i].answer_text);
                        }
                        else if (List[i].question_title == "How confident are you and how often do you feel the following kinds of support and health services and care will be available to you (Select all that apply)")
                        {
                            cntCHKBOX++;
                        }
                    }
                }

                finalResult = (((score1 + score2 + score3 + score4 + score5 + score6 + score7 + score8 + score9 + score10 + score11 + score12 + score13 + score14 + score15 + score16 + score17 + cntCHKBOX) * 100) / 72);

                List<Score> list = new List<Score>();

                objScore = new Score();
                objScore.Title = "Cared For ePROM";
                objScore.Value = finalResult;
                list.Add(objScore);

                string str = JsonConvert.SerializeObject(list);

                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_CFE(string strTitle)
        {
            int strCFE = 0;
            DataTable dtStatic = dsStatic.Tables["CFE"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        strCFE = Convert.ToInt32(staticValue);
                    }
                }
            }
            return strCFE;
        }

        #endregion

        #region K5

        public string Calculate_K5_Score(ResponseCustomClass response)
        {
            int finalResult = 0;
            int score1 = 0;
            int score2 = 0;
            int score3 = 0;
            int score4 = 0;
            int score5 = 0;

            try
            {
                var List = response.responselist;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "In the last 4 weeks, about how often did you feel nervous?")
                        {
                            score1 = GetScore_K5(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last 4 weeks, about how often did you feel without hope?")
                        {
                            score2 = GetScore_K5(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last 4 weeks, about how often did you feel restless or jumpy?")
                        {
                            score3 = GetScore_K5(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last 4 weeks, about how often did you feel everything was an effort?")
                        {
                            score4 = GetScore_K5(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last 4 weeks, about how often did you feel so sad that nothing could cheer you up?")
                        {
                            score5 = GetScore_K5(List[i].answer_text);
                        }
                    }
                }

                finalResult = score1 + score2 + score3 + score4 + score5;

                List<Score> list = new List<Score>();

                objScore = new Score();
                objScore.Title = "K5 ePROM";
                objScore.Value = finalResult;
                list.Add(objScore);

                string str = JsonConvert.SerializeObject(list);

                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_K5(string strTitle)
        {
            int strK5 = 0;
            DataTable dtStatic = dsStatic.Tables["K5"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle.Contains(staticText))
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        strK5 = Convert.ToInt32(staticValue);
                    }
                }
            }
            return strK5;
        }

        #endregion

        #region K10_Plus

        public string Calculate_K10_Plus_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int scoreTired = 0, scoreNervous = 0, scoreCalm = 0, scoreHopeless = 0, scoreFidgety = 0, scoreRestless = 0, scoreDepressed = 0, scoreEffort = 0, scoreCheer = 0, scoreWorthless = 0;
                int TotalK10PlusScore = 0;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "In the last four weeks, about how often did you feel tired out for no good reason?")
                        {
                            scoreTired = GetScore_K10_Plus(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last four weeks, about how often did you feel nervous?")
                        {
                            scoreNervous = GetScore_K10_Plus(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last four weeks, about how often did you feel so nervous that nothing could calm you down?")
                        {
                            scoreCalm = GetScore_K10_Plus(List[i].answer_text);
                        }
                        else if (List[i].question_title == "During the last 30 days, about how often did you feel hopeless?")
                        {
                            scoreHopeless = GetScore_K10_Plus(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last four weeks, about how often did you feel restless or fidgety?")
                        {
                            scoreFidgety = GetScore_K10_Plus(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last four weeks, about how often did you feel so restless you<br>could not sit still?")
                        {
                            scoreRestless = GetScore_K10_Plus(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last four weeks, about how often did you feel depressed?")
                        {
                            scoreDepressed = GetScore_K10_Plus(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last four weeks, about how often did you feel that everything was an effort?")
                        {
                            scoreEffort = GetScore_K10_Plus(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last four weeks, about how often did you feel so sad that nothing could cheer you up?")
                        {
                            scoreCheer = GetScore_K10_Plus(List[i].answer_text);
                        }
                        else if (List[i].question_title == "In the last four weeks, about how often did you feel worthless?")
                        {
                            scoreWorthless = GetScore_K10_Plus(List[i].answer_text);
                        }
                    }
                }

                TotalK10PlusScore = scoreTired + scoreNervous + scoreCalm + scoreHopeless + scoreFidgety + scoreRestless + scoreDepressed + scoreEffort + scoreCheer + scoreWorthless;

                objScore = new Score();
                objScore.Title = "Kessler 10 Plus (K10+)";
                objScore.Value = TotalK10PlusScore;

                List<Score> list = new List<Score>();
                list.Add(objScore);
                string str = JsonConvert.SerializeObject(list);
                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_K10_Plus(string strTitle)
        {
            int ScoreK10Plus = 0;
            DataTable dtStatic = dsStatic.Tables["K10Plus"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        ScoreK10Plus = Convert.ToInt32(staticValue);
                    }
                }
            }
            return ScoreK10Plus;
        }

        #endregion

        #region PROMISE_29

        public string Calculate_PROMISE_29_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int pf1 = 0, pf2 = 0, pf3 = 0, pf4 = 0;
                int a1 = 0, a2 = 0, a3 = 0, a4 = 0;
                int d1 = 0, d2 = 0, d3 = 0, d4 = 0;
                int f1 = 0, f2 = 0, f3 = 0, f4 = 0;
                int sd1 = 0, sd2 = 0, sd3 = 0, sd4 = 0;
                int s1 = 0, s2 = 0, s3 = 0, s4 = 0;
                int p1 = 0, p2 = 0, p3 = 0, p4 = 0;

                double PhysicalFunction = 0.0, Anxiety = 0.0, Depression = 0.0, Fatigue = 0.0, SleepDisturbance = 0.0, Satisfaction = 0.0, PainInterference = 0.0, PainIntensity = 0.0;

                for (int i = 0; i < List.Count; i++)
                {
                    if (List[i].question_title == "Are you able to do chores such as vacuuming or yard work?")
                    {
                        pf1 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "Are you able to go up and down stairs at a normal pace?")
                    {
                        pf2 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "Are you able to go for a walk of at least 15 minutes?")
                    {
                        pf3 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "Are you able to run errands and shop?")
                    {
                        pf4 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I felt fearful")
                    {
                        a1 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I found it hard to focus on anything other than my anxiety")
                    {
                        a2 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "My worries overwhelmed me")
                    {
                        a3 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I felt uneasy")
                    {
                        a4 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I felt worthless")
                    {
                        d1 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I felt helpless")
                    {
                        d2 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I felt depressed")
                    {
                        d3 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I felt hopeless")
                    {
                        d4 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I feel fatigued")
                    {
                        f1 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I have trouble starting things because I am tired")
                    {
                        f2 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "How run-down did you feel on average?")
                    {
                        f3 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "How fatigued were you on average?")
                    {
                        f4 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "My sleep quality was")
                    {
                        sd1 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "My sleep was refreshing")
                    {
                        sd2 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I had a problem with my sleep ")
                    {
                        sd3 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I had difficulty falling asleep")
                    {
                        sd4 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I am satisfied with how much work I can do (include work at home)")
                    {
                        s1 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I am satisfied with my ability to work (include work at home)")
                    {
                        s2 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I am satisfied with my ability to do regular personal and household responsibilities")
                    {
                        s3 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "I am satisfied with my ability to perform my daily routines")
                    {
                        s4 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "How much did pain interfere with your day to day activities?")
                    {
                        p1 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "How much did pain interfere with work around the home?")
                    {
                        p2 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "How much did pain interfere with your ability to participate in social activities?")
                    {
                        p3 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "How much did pain interfere with your household chores?")
                    {
                        p4 = GetScore_PROMISE_29(List[i].answer_text);
                    }
                    else if (List[i].question_title == "On a scale of 0 to 10 (where 0 is No Pain, and 10 is Worst Pain Imaginable) how would you rate your pain on average?")
                    {
                        PainIntensity = Convert.ToInt32(List[i].answer_text);
                    }
                }

                PhysicalFunction = GetPROMISG29((pf1 + pf2 + pf3 + pf4), "P29PF");
                Anxiety = GetPROMISG29((a1 + a2 + a3 + a4), "P29A");
                Depression = GetPROMISG29((d1 + d2 + d3 + d4), "P29D");
                Fatigue = GetPROMISG29((f1 + f2 + f3 + f4), "P29F");
                SleepDisturbance = GetPROMISG29((sd1 + sd2 + sd3 + sd4), "P29SD");
                Satisfaction = GetPROMISG29((s1 + s2 + s3 + s4), "P29S");
                PainInterference = GetPROMISG29((p1 + p2 + p3 + p4), "P29P");

                List<Score> list = new List<Score>();

                objScore = new Score();
                objScore.Title = "Physical Function";
                objScore.Value = PhysicalFunction;
                list.Add(objScore);

                objScore = new Score();
                objScore.Title = "Anxiety";
                objScore.Value = Anxiety;
                list.Add(objScore);

                objScore = new Score();
                objScore.Title = "Depression";
                objScore.Value = Depression;
                list.Add(objScore);

                objScore = new Score();
                objScore.Title = "Fatigue";
                objScore.Value = Fatigue;
                list.Add(objScore);

                objScore = new Score();
                objScore.Title = "Sleep Disturbance";
                objScore.Value = SleepDisturbance;
                list.Add(objScore);

                objScore = new Score();
                objScore.Title = "Satisfaction with Social Role";
                objScore.Value = Satisfaction;
                list.Add(objScore);

                objScore = new Score();
                objScore.Title = "Pain Interference";
                objScore.Value = PainInterference;
                list.Add(objScore);

                objScore = new Score();
                objScore.Title = "Pain Intensity";
                objScore.Value = PainIntensity;
                list.Add(objScore);

                string str = JsonConvert.SerializeObject(list);

                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_PROMISE_29(string strTitle)
        {
            int strPROMISE29 = 0;
            DataTable dtStatic = dsStatic.Tables["PROMISE29"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        strPROMISE29 = Convert.ToInt32(staticValue);
                    }
                }
            }
            return strPROMISE29;
        }

        public double GetPROMISG29(int val, string title)
        {
            double finalVal = 0;
            DataTable dtStatic = dsStatic.Tables[title];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (val.ToString() == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        finalVal = Convert.ToDouble(staticValue);
                    }
                }
            }
            return finalVal;
        }

        #endregion

        #region SDQ

        public string Calculate_SDQ_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int score1 = 0, score2 = 0, score3 = 0, score4 = 0, score5 = 0, score6 = 0, score7 = 0, score8 = 0, score9 = 0, score10 = 0, score11 = 0, score12 = 0, score13 = 0, score14 = 0, score15 = 0, score16 = 0, score17 = 0, score18 = 0, score19 = 0, score20 = 0, score21 = 0, score22 = 0, score23 = 0, score24 = 0, score25 = 0;
                int TDS = 0, ESS = 0, CPS = 0, HS = 0, PPS = 0, PBS = 0;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "1. I try to be nice to other people. I care about their feelings")
                        {
                            score21 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "2. I am restless, I cannot stay still for long")
                        {
                            score11 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "3. I get a lot of headaches, stomach-aches, or sickness")
                        {
                            score1 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "4. I usually share with others, for example CDs, games, food")
                        {
                            score22 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "5. I get very angry and often lose my temper")
                        {
                            score6 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "6. I would rather be alone than with people of my age")
                        {
                            score16 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "7. I usually do as I am told")
                        {
                            score7 = GetScore_SDQ(List[i].answer_text, "ASDQ");
                        }
                        else if (List[i].question_title == "8. I worry a lot")
                        {
                            score2 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "9. I am helpful if someone is hurt, upset or feeling ill")
                        {
                            score23 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "10. I am constantly fidgeting or squirming")
                        {
                            score12 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "11. I have one good friend or more")
                        {
                            score17 = GetScore_SDQ(List[i].answer_text, "ASDQ");
                        }
                        else if (List[i].question_title == "12. I fight a lot. I can make other people do what I want")
                        {
                            score8 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "13. I am often unhappy, depressed or tearful")
                        {
                            score3 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "14. Other people my age generally like me")
                        {
                            score18 = GetScore_SDQ(List[i].answer_text, "ASDQ");
                        }
                        else if (List[i].question_title == "15. I am easily distracted, I find it difficult to concentrate")
                        {
                            score13 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "16. I am nervous in new situations. I easily lose confidence")
                        {
                            score4 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "17. I am kind to younger children")
                        {
                            score24 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "18. I am often accused of lying or cheating")
                        {
                            score9 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "19. Other children or young people pick on me or bully me")
                        {
                            score19 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "20. I often volunteer to help others (parents, teachers, children)")
                        {
                            score25 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "21. I think before I do things")
                        {
                            score14 = GetScore_SDQ(List[i].answer_text, "ASDQ");
                        }
                        else if (List[i].question_title == "22. I take things that are not mine from home, school or elsewhere")
                        {
                            score10 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "23. I get along better with adults than with people my own age")
                        {
                            score20 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "24. I have many fears, I am easily scared")
                        {
                            score5 = GetScore_SDQ(List[i].answer_text);
                        }
                        else if (List[i].question_title == "25. I finish the work I’m doing. My attention is good")
                        {
                            score15 = GetScore_SDQ(List[i].answer_text, "ASDQ");
                        }
                    }
                }

                ESS = score1 + score2 + score3 + score4 + score5;
                CPS = score6 + score7 + score8 + score9 + score10;
                HS = score11 + score12 + score13 + score14 + score15;
                PPS = score16 + score17 + score18 + score19 + score20;
                PBS = score21 + score22 + score23 + score24 + score25;
                TDS = ESS + CPS + HS + PPS + PBS;

                List<Factor> list = new List<Factor>();

                string strScore = "";
                if (TDS >= 0 && TDS <= 15)
                    strScore = "Normal";
                else if (TDS >= 16 && TDS <= 19)
                    strScore = "Borderline";
                else
                    strScore = "Abnormal";
                objFactor = new Factor();
                objFactor.Title = "Total Difficulties Score";
                objFactor.Value = strScore;
                list.Add(objFactor);

                if (ESS >= 0 && ESS <= 5)
                    strScore = "Normal";
                else if (ESS == 6)
                    strScore = "Borderline";
                else
                    strScore = "Abnormal";
                objFactor = new Factor();
                objFactor.Title = "Emotional Symptoms Score";
                objFactor.Value = strScore;
                list.Add(objFactor);

                if (CPS >= 0 && CPS <= 3)
                    strScore = "Normal";
                else if (CPS == 4)
                    strScore = "Borderline";
                else
                    strScore = "Abnormal";
                objFactor = new Factor();
                objFactor.Title = "Conduct Problems Score";
                objFactor.Value = strScore;
                list.Add(objFactor);

                if (HS >= 0 && HS <= 5)
                    strScore = "Normal";
                else if (HS == 6)
                    strScore = "Borderline";
                else
                    strScore = "Abnormal";
                objFactor = new Factor();
                objFactor.Title = "Hyperactivity Score";
                objFactor.Value = strScore;
                list.Add(objFactor);

                if (PPS >= 0 && PPS <= 3)
                    strScore = "Normal";
                else if (PPS >= 4 && PPS <= 5)
                    strScore = "Borderline";
                else
                    strScore = "Abnormal";
                objFactor = new Factor();
                objFactor.Title = "Peer Problems Score";
                objFactor.Value = strScore;
                list.Add(objFactor);

                if (PBS >= 6 && PBS <= 10)
                    strScore = "Normal";
                else if (PBS == 5)
                    strScore = "Borderline";
                else
                    strScore = "Abnormal";
                objFactor = new Factor();
                objFactor.Title = "Prosocial Behaviour Score";
                objFactor.Value = strScore;
                list.Add(objFactor);

                string str = JsonConvert.SerializeObject(list);

                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_SDQ(string strTitle, string xmlTitle = "SDQ")
        {
            int strSDQ = 0;
            DataTable dtStatic = dsStatic.Tables[xmlTitle];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                strTitle = strTitle.Replace("<br />", " ");

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        strSDQ = Convert.ToInt32(staticValue);
                    }
                }
            }
            return strSDQ;
        }

        #endregion

        #region DASS

        public string Calculate_DASS_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int score1 = 0, score2 = 0, score3 = 0, score4 = 0, score5 = 0, score6 = 0, score7 = 0, score8 = 0, score9 = 0, score10 = 0, score11 = 0, score12 = 0, score13 = 0, score14 = 0, score15 = 0, score16 = 0, score17 = 0, score18 = 0, score19 = 0, score20 = 0, score21 = 0;
                int TotalScore = 0;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "I found it hard to wind down")
                        {
                            score1 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I was aware of dryness of my mouth")
                        {
                            score2 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I couldn't seem to experience any positive feeling at all")
                        {
                            score3 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I experienced breathing difficulty (eg, excessively rapid breathing, breathlessness in the absence of physical exertion)")
                        {
                            score4 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I found it difficult to work up the initiative to do things")
                        {
                            score5 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I tended to over-react to situations")
                        {
                            score6 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I experienced trembling (eg, in the hands)")
                        {
                            score7 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I felt that I was using a lot of nervous energy")
                        {
                            score8 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I was worried about situations in which I might panic and make a fool of myself")
                        {
                            score9 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I felt that I had nothing to look forward to")
                        {
                            score10 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I found myself getting agitated")
                        {
                            score11 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I found it difficult to relax")
                        {
                            score12 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I felt down-hearted and blue")
                        {
                            score13 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I was intolerant of anything that kept me from getting on with what I was doing")
                        {
                            score14 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I felt I was close to panic")
                        {
                            score15 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I was unable to become enthusiastic about anything")
                        {
                            score16 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I felt I wasn't worth much as a person")
                        {
                            score17 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I felt that I was rather touchy")
                        {
                            score18 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I was aware of the action of my heart in the absence of physical exertion (eg, sense of heart rate increase, heart missing a beat)")
                        {
                            score19 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I felt scared without any good reason")
                        {
                            score20 = GetScore_DASS(List[i].answer_text);
                        }
                        else if (List[i].question_title == "I felt that life was meaningless")
                        {
                            score21 = GetScore_DASS(List[i].answer_text);
                        }
                    }
                }

                TotalScore = score1 + score2 + score3 + score4 + score5 + score6 + score7 + score8 + score9 + score10 + score11 + score12 + score13 + score14 + score15 + score16 + score17 + score18 + score19 + score20 + score21;

                List<Factor> list = new List<Factor>();

                string strScore = "";
                if (TotalScore >= 0 && TotalScore <= 4)
                    strScore = "Normal";
                else if (TotalScore >= 5 && TotalScore <= 6)
                    strScore = "Mild";
                else if (TotalScore >= 7 && TotalScore <= 10)
                    strScore = "Moderate";
                else if (TotalScore >= 11 && TotalScore <= 13)
                    strScore = "Severe";
                else
                    strScore = "Extremely Severe";
                objFactor = new Factor();
                objFactor.Title = "DEPRESSION SCORE";
                objFactor.Value = strScore;
                list.Add(objFactor);

                strScore = "";
                if (TotalScore >= 0 && TotalScore <= 3)
                    strScore = "Normal";
                else if (TotalScore >= 4 && TotalScore <= 5)
                    strScore = "Mild";
                else if (TotalScore >= 6 && TotalScore <= 7)
                    strScore = "Moderate";
                else if (TotalScore >= 8 && TotalScore <= 9)
                    strScore = "Severe";
                else
                    strScore = "Extremely Severe";
                objFactor = new Factor();
                objFactor.Title = "ANXIETY SCORE";
                objFactor.Value = strScore;
                list.Add(objFactor);

                strScore = "";
                if (TotalScore >= 0 && TotalScore <= 7)
                    strScore = "Normal";
                else if (TotalScore >= 8 && TotalScore <= 9)
                    strScore = "Mild";
                else if (TotalScore >= 10 && TotalScore <= 12)
                    strScore = "Moderate";
                else if (TotalScore >= 13 && TotalScore <= 16)
                    strScore = "Severe";
                else
                    strScore = "Extremely Severe";
                objFactor = new Factor();
                objFactor.Title = "STRESS SCORE";
                objFactor.Value = strScore;
                list.Add(objFactor);

                string str = JsonConvert.SerializeObject(list);

                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_DASS(string strTitle)
        {
            int strDASS = 0;
            DataTable dtStatic = dsStatic.Tables["DASS"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle == staticText)
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        strDASS = Convert.ToInt32(staticValue);
                    }
                }
            }
            return strDASS;
        }

        #endregion

        #region COPD

        public string Calculate_COPD_Score(ResponseCustomClass response)
        {
            try
            {
                var List = response.responselist;

                int score1 = 0, score2 = 0, score3 = 0, score4 = 0, score5 = 0, score6 = 0, score7 = 0, score8 = 0;
                int TotalScore = 0;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "1")
                        {
                            score1 = Convert.ToInt32(List[i].answer_text);
                        }
                        else if (List[i].question_title == "2")
                        {
                            score2 = Convert.ToInt32(List[i].answer_text);
                        }
                        else if (List[i].question_title == "3")
                        {
                            score3 = Convert.ToInt32(List[i].answer_text);
                        }
                        else if (List[i].question_title == "4")
                        {
                            score4 = Convert.ToInt32(List[i].answer_text);
                        }
                        else if (List[i].question_title == "5")
                        {
                            score5 = Convert.ToInt32(List[i].answer_text);
                        }
                        else if (List[i].question_title == "6")
                        {
                            score6 = Convert.ToInt32(List[i].answer_text);
                        }
                        else if (List[i].question_title == "7")
                        {
                            score7 = Convert.ToInt32(List[i].answer_text);
                        }
                        else if (List[i].question_title == "8")
                        {
                            score8 = Convert.ToInt32(List[i].answer_text);
                        }
                    }
                }

                TotalScore = score1 + score2 + score3 + score4 + score5 + score6 + score7 + score8;

                List<Score> list = new List<Score>();

                objScore = new Score();
                objScore.Title = "COPD Assessment Test ©";
                objScore.Value = TotalScore;
                list.Add(objScore);

                string str = JsonConvert.SerializeObject(list);

                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        #endregion

        #region Cataracts

        public string Calculate_Cataracts_Score(ResponseCustomClass response)
        {
            int score1 = 0;
            int score2 = 0;
            int score3 = 0;
            int score4 = 0;
            int score5 = 0;
            int score6 = 0;
            int score7 = 0;
            int score8 = 0;
            int score9 = 0;

            try
            {
                var List = response.responselist;

                if (List != null)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].question_title == "1. Do you find that your sight at present in some way causes you difficulty in your everyday life?")
                        {
                            score1 = GetScore_Cataracts(List[i].answer_text);
                        }
                        else if (List[i].question_title == "2. Are you satisfied or dissatisfied with your sight at present?")
                        {
                            score2 = GetScore_Cataracts(List[i].answer_text);
                        }
                        else if (List[i].question_title == "3. Do you have difficulty reading text in newspapers because of your sight?")
                        {
                            score3 = GetScore_Cataracts(List[i].answer_text);
                        }
                        else if (List[i].question_title == "4. Do you have difficulty recognizing the faces of people you meet?")
                        {
                            score4 = GetScore_Cataracts(List[i].answer_text);
                        }
                        else if (List[i].question_title == "5. Do you have difficulty seeing the prices of goods when shopping because of your sight?")
                        {
                            score5 = GetScore_Cataracts(List[i].answer_text);
                        }
                        else if (List[i].question_title == "6. Do you have difficulty seeing to walk on uneven surfaces (for example cobblestones) because of your sight?")
                        {
                            score6 = GetScore_Cataracts(List[i].answer_text);
                        }
                        else if (List[i].question_title == "7. Do you have difficulty seeing to do handicrafts, woodwork, etc. because of your sight?")
                        {
                            score7 = GetScore_Cataracts(List[i].answer_text);
                        }
                        else if (List[i].question_title == "8. Do you have difficulty reading subtitles on TV because of your sight?")
                        {
                            score8 = GetScore_Cataracts(List[i].answer_text);
                        }
                        else if (List[i].question_title == "9. Do you have difficulty seeing to engage in activity/hobby that you are interested in because of your sight?")
                        {
                            score9 = GetScore_Cataracts(List[i].answer_text);
                        }
                    }
                }

                double s1 = GetScore_Cataracts_SF(score1.ToString(), "SF1");
                double s2 = GetScore_Cataracts_SF(score2.ToString(), "SF2");
                double s3 = GetScore_Cataracts_SF(score3.ToString(), "SF3");
                double s4 = GetScore_Cataracts_SF(score4.ToString(), "SF4");
                double s5 = GetScore_Cataracts_SF(score5.ToString(), "SF5");
                double s6 = GetScore_Cataracts_SF(score6.ToString(), "SF6");
                double s7 = GetScore_Cataracts_SF(score7.ToString(), "SF7");
                double s8 = GetScore_Cataracts_SF(score8.ToString(), "SF8");
                double s9 = GetScore_Cataracts_SF(score9.ToString(), "SF9");

                double mean = (s1 + s2 + s3 + s4 + s5 + s6 + s7 + s8 + s9) / 9.0;

                double d1 = (s1 - mean) * (s1 - mean);
                double d2 = (s2 - mean) * (s2 - mean);
                double d3 = (s3 - mean) * (s3 - mean);
                double d4 = (s4 - mean) * (s4 - mean);
                double d5 = (s5 - mean) * (s5 - mean);
                double d6 = (s6 - mean) * (s6 - mean);
                double d7 = (s7 - mean) * (s7 - mean);
                double d8 = (s8 - mean) * (s8 - mean);
                double d9 = (s9 - mean) * (s9 - mean);

                double sd = Math.Sqrt(((d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8 + d9) / 9.0));

                List<Score> list = new List<Score>();

                objScore = new Score();
                objScore.Title = "Cataracts ePROM";
                objScore.Value = mean;
                list.Add(objScore);

                string str = JsonConvert.SerializeObject(list);

                return str;
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetScore_Cataracts(string strTitle)
        {
            int strCataracts = 0;
            DataTable dtStatic = dsStatic.Tables["Cataracts"];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle.Contains(staticText))
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        strCataracts = Convert.ToInt32(staticValue);
                    }
                }
            }
            return strCataracts;
        }

        public int GetScore_Cataracts_SF(string strTitle, string tag)
        {
            int strCataracts = 0;
            DataTable dtStatic = dsStatic.Tables[tag];
            for (int s = 0; s < dtStatic.Rows.Count; s++)
            {
                string staticText = dtStatic.Rows[s]["Text"].ToString();
                var staticValue = dtStatic.Rows[s]["Value"];

                if (strTitle.Contains(staticText))
                {
                    if (staticValue != null && staticValue.ToString() != "")
                    {
                        strCataracts = Convert.ToInt32(staticValue);
                    }
                }
            }
            return strCataracts;
        }

        #endregion
    }
}
