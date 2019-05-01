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
    public class EpromScore
    {
        public static DataSet dsStatic = new DataSet();

        public DataSet ReadXmlFile(string FilePath)
        {
            dsStatic.ReadXml(FilePath);
            return dsStatic;
        }

        #region PROMISG10

        public string Create_PROMISG10_ScoreTable(DataTable tblCsv)
        {
            try
            {
                DataTable dtData = new DataTable();

                dtData.Columns.Add("RespondentID");
                dtData.Columns.Add("CollectorID");
                dtData.Columns.Add("GlobalPhysicalHealthRawScore");
                dtData.Columns.Add("GlobalMentalHealthRawScore");
                dtData.Columns.Add("GlobalPhysicalHealthAdjustedScore");
                dtData.Columns.Add("GlobalMentalHealthAdjustedScore");

                for (int i = 0; i < tblCsv.Rows.Count; i++)
                {
                    int scorehealth = 0, scoreQuality = 0, scorePhysicalHealth = 0, scoreMentalHealth = 0, scoreSatisfaction = 0, scoreDepressed = 0, scoreFatigue = 0, scorePainAverage = 0, globalMentalScore = 0, globalPhysicalScore = 0;

                    double PhysicalAdjustedScore = 0.0, MentalAdjustedScore = 0.0;

                    #region Health
                    string strHealth = tblCsv.Rows[i]["Please respond to each item by marking one box per row - In general, would you say your health is:"].ToString();

                    scorehealth = GetScore_PROMISG10(strHealth);
                    #endregion Health

                    #region Quality
                    string strQuality = tblCsv.Rows[i]["Please respond to each item by marking one box per row - In general, would you say your quality of life is:"].ToString();

                    scoreQuality = GetScore_PROMISG10(strQuality);
                    #endregion Quality

                    #region PhysicalHealth
                    string strPhysicalHealth = tblCsv.Rows[i]["Please respond to each item by marking one box per row - In general, how would you rate your physical health?"].ToString();

                    scorePhysicalHealth = GetScore_PROMISG10(strPhysicalHealth);
                    #endregion PhysicalHealth

                    #region MentalHealth
                    string strMentalHealth = tblCsv.Rows[i]["Please respond to each item by marking one box per row - In general, how would you rate your mental health, including your mood and your ability to<br>think?"].ToString();

                    scoreMentalHealth = GetScore_PROMISG10(strMentalHealth);
                    #endregion MentalHealth

                    #region Satisfaction
                    string strSatisfaction = tblCsv.Rows[i]["Please respond to each item by marking one box per row - In general, how would you rate your satisfaction with your social activities and relationships?"].ToString();

                    scoreSatisfaction = GetScore_PROMISG10(strSatisfaction);
                    #endregion Satisfaction                    

                    #region Depressed
                    string strDepressed = tblCsv.Rows[i]["In the past 7 days, How often have you been bothered by emotional problems such as feeling anxious <br>depressed or irritable?"].ToString();

                    scoreDepressed = GetScore_PROMISG10(strDepressed);
                    #endregion Depressed

                    #region Fatigue
                    string strFatigue = tblCsv.Rows[i]["How would you rate your fatigue on average?"].ToString();

                    scoreFatigue = GetScore_PROMISG10(strFatigue);
                    #endregion Fatigue

                    #region PainAverage
                    string strPainAverage = tblCsv.Rows[i]["On a scale of 0 to 10 (where 0 is No Pain, and 10 is Worst Pain Imaginable) how would you rate your pain on average?"].ToString();

                    scorePainAverage = GetPainAVGScore_PROMISG10(strPainAverage);

                    #endregion fatigue

                    globalPhysicalScore = scorehealth + scoreQuality + scorePhysicalHealth + scoreFatigue + scorePainAverage;

                    globalMentalScore = scoreQuality + scoreMentalHealth + scoreSatisfaction + scoreDepressed;

                    #region GlobalPhysicalAdjustedScore

                    PhysicalAdjustedScore = GetPROMISG10_GlobalPhysicalScore(globalPhysicalScore);

                    #endregion GlobalPhysicalAdjustedScore

                    #region GlobalMentalAdjustedScore

                    MentalAdjustedScore = GetPROMISG10_GlobalMentalScore(globalMentalScore);

                    #endregion GlobalMentalAdjustedScore

                    if (tblCsv.Rows[i]["RespondentID"] != null && tblCsv.Rows[i]["RespondentID"].ToString() != "")
                    {
                        dtData.Rows.Add(tblCsv.Rows[i]["RespondentID"], tblCsv.Rows[i]["CollectorID"], globalPhysicalScore, globalMentalScore, PhysicalAdjustedScore, MentalAdjustedScore);
                    }
                }
                return JsonConvert.SerializeObject(dtData);
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

        #region OMPQ

        //public enum OMPQQuestion1
        //{
        //    ZeroDays = 1,
        //    OneTo2Days = 2,
        //    ThreeTo7Days = 3,
        //    EightTo14Days = 4,
        //    FifteenTo30Days = 2,
        //    OneMonth = 6,
        //    TwoMonths = 7,
        //    ThreeTo6Month = 8,
        //    SixTo12Months = 9,
        //    Over1Year = 10
        //};

        //public enum OMPQQuestion2
        //{
        //    ZeroDays = 9,
        //    OneTo2Days = 2,
        //    ThreeTo7Days = 7,
        //    EightTo14Days = 4,
        //    FifteenTo30Days = 2,
        //    OneMonth = 1,
        //    TwoMonths = 2,
        //    ThreeTo6Month = 8,
        //    SixTo12Months = 9,
        //    Over1Year = 10
        //};

        public string Calculate_OMPQ_Score(DataTable tblCsv)
        {
            try
            {
                DataTable dtData = new DataTable();

                #region Add Columns
                dtData.Columns.Add("RespondentID");

                dtData.Columns.Add("CollectorID");

                dtData.Columns.Add("Neck");

                dtData.Columns.Add("Shoulder");

                dtData.Columns.Add("Arm Injury?");

                dtData.Columns.Add("Upper Back");

                dtData.Columns.Add("Lower Back");

                dtData.Columns.Add("Leg");

                dtData.Columns.Add("Other (state)");

                dtData.Columns.Add("Total Injuries");

                dtData.Columns.Add("How many days of work have you missed because of pain during the past 18 months? Tick one.");

                dtData.Columns.Add("How long have you had your current pain problem? Tick one.");

                dtData.Columns.Add("Is your work heavy or monotonous? -");

                dtData.Columns.Add("How would you rate the pain that you have had during the past week? -");

                dtData.Columns.Add("In the past three months, on average, how bad was your pain on a 0-10 scale? Select one. -");

                dtData.Columns.Add("How often would you say that you have experienced pain episodes, on average, during the past three<br>months? Select one. -");

                dtData.Columns.Add("Based on all things you do to cope, or deal with your pain, on an average day, how much are you<br>able to decrease it? Select one. -");

                dtData.Columns.Add("If you take into consideration your work routines, management, salary, promotion possibilities and<br>work mates, how satisfied are you with your job? Select one. -");

                dtData.Columns.Add("How tense or anxious have you felt in the past week? Select one. -");

                dtData.Columns.Add("How much have you been bothered by feeling depressed in the past week? Select one. -");

                dtData.Columns.Add("In your view, how large is the risk that your current pain may become persistent? Select one. -");

                dtData.Columns.Add("In your estimation, what are the chances that you will be able to work in six months? Select one. -");

                dtData.Columns.Add("Here are some of the things that other people have told us about their pain. For each statement, circle<br>one number from 0 to 10 to say how much physical activities, such as bending, lifting, walking or<br>driving, would affect your pain. - Physical activity makes my pain worse.");

                dtData.Columns.Add("Here are some of the things that other people have told us about their pain. For each statement, circle<br>one number from 0 to 10 to say how much physical activities, such as bending, lifting, walking or<br>driving, would affect your pain. - An increase in pain is an indication that I should stop what I’m doing until the pain decreases");

                dtData.Columns.Add("Here are some of the things that other people have told us about their pain. For each statement, circle<br>one number from 0 to 10 to say how much physical activities, such as bending, lifting, walking or<br>driving, would affect your pain. - I should not do my normal work with my present pain.");

                dtData.Columns.Add("Here is a list of five activities. Select the one number that best describes your current ability to<br>participate in each of these activities. - I can do light work for an hour.");

                dtData.Columns.Add("Here is a list of five activities. Select the one number that best describes your current ability to<br>participate in each of these activities. - I can walk for an hour.");

                dtData.Columns.Add("Here is a list of five activities. Select the one number that best describes your current ability to<br>participate in each of these activities. - I can do ordinary household chores.");

                dtData.Columns.Add("Here is a list of five activities. Select the one number that best describes your current ability to<br>participate in each of these activities. - I can do the weekly shopping");

                dtData.Columns.Add("Here is a list of five activities. Select the one number that best describes your current ability to<br>participate in each of these activities. - I can sleep at night.");

                dtData.Columns.Add("OMPQScore");

                dtData.Columns.Add("Email");

                #endregion Add Columns

                for (int i = 0; i < tblCsv.Rows.Count; i++)
                {
                    var test = tblCsv.Columns[56];

                    int scoreNeck = 0, scoreShoulder = 0, scoreArm = 0, scoreUpperBack = 0, scoreLowerBack = 0, scoreLeg = 0, scoreOther = 0, TotalInjury = 0, scoreDaysOfWork = 0, scoreCurrentPain = 0, scoreMonotonous = 0, scorePainRate = 0, scorePainScale = 0, scorePainExperienced = 0, scorePainDecrease = 0, scoreJobSatisfied = 0, scoreTense = 0, scoreDepressed = 0, scorePainPersistent = 0, scoreWorkChance = 0, scorePhysicalActivity = 0, scorePainIndication = 0, scorePresentPain = 0, scoreLightWork = 0, scoreWalk = 0, scoreOrdinaryHousehold = 0, scoreShopping = 0, scoreSleep = 0, TotalOMPQScore = 0, EmailScore = 0;

                    #region Neck

                    string strNeck = tblCsv.Rows[i]["Neck"].ToString();

                    if (strNeck.Contains("Neck"))
                        scoreNeck = 1;
                    else
                        scoreNeck = 0;

                    #endregion Neck                   

                    #region Shoulder

                    string strShoulder = tblCsv.Rows[i]["Shoulder"].ToString();

                    if (strShoulder.Contains("Shoulder"))
                        scoreShoulder = 1;
                    else
                        scoreShoulder = 0;

                    #endregion Shoulder                   

                    #region Arm

                    string strArm = tblCsv.Rows[i]["Arm Injury?"].ToString();

                    if (strArm.Contains("Arm"))
                        scoreArm = 1;
                    else
                        scoreArm = 0;

                    #endregion Arm                   

                    #region UpperBack

                    string strUpperBack = tblCsv.Rows[i]["Upper Back"].ToString();

                    if (strUpperBack.Contains("Upper Back"))
                        scoreUpperBack = 1;
                    else
                        scoreUpperBack = 0;

                    #endregion UpperBack                   

                    #region LowerBack

                    string strLowerBack = tblCsv.Rows[i]["Lower Back"].ToString();

                    if (strLowerBack.Contains("Lower Back"))
                        scoreLowerBack = 1;
                    else
                        scoreLowerBack = 0;

                    #endregion LowerBack                   

                    #region Leg

                    string StrLeg = tblCsv.Rows[i]["Leg"].ToString();

                    if (StrLeg.Contains("Leg"))
                        scoreLeg = 1;
                    else
                        scoreLeg = 0;

                    #endregion Leg

                    #region Other (state)

                    string StrOther = tblCsv.Rows[i]["Other (state)"].ToString();

                    if (StrOther.Contains("Other (state)"))
                        scoreOther = 1;
                    else
                        scoreOther = 0;

                    #endregion Other (state)

                    #region Total Injuries

                    TotalInjury = scoreNeck + scoreShoulder + scoreArm + scoreUpperBack + scoreLowerBack + scoreLeg + scoreOther;

                    #endregion Total Injuries

                    #region DaysOfWork

                    string strDaysofWork = tblCsv.Rows[i]["How many days of work have you missed because of pain during the past 18 months? Tick one."].ToString();

                    scoreDaysOfWork = GetScore_OMPQ(strDaysofWork);

                    #endregion DaysOfWork

                    #region Current Pain

                    string strCurrentPain = tblCsv.Rows[i]["How long have you had your current pain problem? Tick one."].ToString();

                    scoreCurrentPain = GetScore_OMPQ(strCurrentPain);
                    #endregion Current Pain

                    #region Monotonous

                    string strMonotonous = tblCsv.Rows[i]["Is your work heavy or monotonous? -"].ToString();

                    scoreMonotonous = GetScore_OMPQQuestions(strMonotonous);

                    #endregion Monotonous

                    #region PainRate

                    string strPainRate = tblCsv.Rows[i]["How would you rate the pain that you have had during the past week? -"].ToString();

                    scorePainRate = GetScore_OMPQQuestions(strPainRate);

                    #endregion PainRate

                    #region PainScale

                    string strPainScale = tblCsv.Rows[i]["In the past three months, on average, how bad was your pain on a 0-10 scale? Select one. -"].ToString();

                    scorePainScale = GetScore_OMPQQuestions(strPainScale);

                    #endregion PainScale

                    #region PainExperienced

                    string strPainExperienced = tblCsv.Rows[i]["How often would you say that you have experienced pain episodes, on average, during the past three<br>months? Select one. -"].ToString();

                    scorePainExperienced = GetScore_OMPQQuestions(strPainExperienced);

                    #endregion PainExperienced

                    #region PainDecrease

                    string strPainDecrease = tblCsv.Rows[i]["Based on all things you do to cope, or deal with your pain, on an average day, how much are you<br>able to decrease it? Select one. -"].ToString();

                    scorePainDecrease = GetScore_OMPQQuestions(strPainDecrease);

                    #endregion PainDecrease

                    #region JobSatisfied

                    string strJobSatisfied = tblCsv.Rows[i]["If you take into consideration your work routines, management, salary, promotion possibilities and<br>work mates, how satisfied are you with your job? Select one. -"].ToString();

                    scoreJobSatisfied = GetScore_OMPQQuestions(strJobSatisfied);

                    #endregion JobSatisfied

                    #region Tense

                    string strTense = tblCsv.Rows[i]["How tense or anxious have you felt in the past week? Select one. -"].ToString();

                    scoreTense = GetScore_OMPQQuestions(strTense);

                    #endregion Tense

                    #region Depressed

                    string strDepressed = tblCsv.Rows[i]["How much have you been bothered by feeling depressed in the past week? Select one. -"].ToString();

                    scoreDepressed = GetScore_OMPQQuestions(strDepressed);

                    #endregion Depressed

                    #region PainPersistent

                    string strPainPersistent = tblCsv.Rows[i]["In your view, how large is the risk that your current pain may become persistent? Select one. -"].ToString();

                    scorePainPersistent = GetScore_OMPQQuestions(strPainPersistent);

                    #endregion PainPersistent

                    #region WorkChance

                    string strWorkChance = tblCsv.Rows[i]["In your estimation, what are the chances that you will be able to work in six months? Select one. -"].ToString();

                    scoreWorkChance = GetScore_OMPQQuestions(strWorkChance);

                    #endregion WorkChance

                    #region PhysicalActivity

                    string strPhysicalActivity = tblCsv.Rows[i]["Here are some of the things that other people have told us about their pain. For each statement, circle<br>one number from 0 to 10 to say how much physical activities, such as bending, lifting, walking or<br>driving, would affect your pain. - Physical activity makes my pain worse."].ToString();

                    scorePhysicalActivity = GetScore_OMPQQuestions(strPhysicalActivity);

                    #endregion PhysicalActivity

                    #region PainIndication

                    string strPainIndication = tblCsv.Rows[i]["Here are some of the things that other people have told us about their pain. For each statement, circle<br>one number from 0 to 10 to say how much physical activities, such as bending, lifting, walking or<br>driving, would affect your pain. - An increase in pain is an indication that I should stop what I’m doing until the pain decreases"].ToString();

                    scorePainIndication = GetScore_OMPQQuestions(strPainIndication);

                    #endregion PainIndication

                    #region PresentPain

                    string strPresentPain = tblCsv.Rows[i]["Here are some of the things that other people have told us about their pain. For each statement, circle<br>one number from 0 to 10 to say how much physical activities, such as bending, lifting, walking or<br>driving, would affect your pain. - I should not do my normal work with my present pain."].ToString();

                    scorePresentPain = GetScore_OMPQQuestions(strPresentPain);

                    #endregion PresentPain

                    #region LightWork

                    string strLightWork = tblCsv.Rows[i]["Here is a list of five activities. Select the one number that best describes your current ability to<br>participate in each of these activities. - I can do light work for an hour."].ToString();

                    scoreLightWork = GetScore_OMPQQuestions(strLightWork);

                    #endregion LightWork

                    #region Walk

                    string strWalk = tblCsv.Rows[i]["Here is a list of five activities. Select the one number that best describes your current ability to<br>participate in each of these activities. - I can walk for an hour."].ToString();

                    scoreWalk = GetScore_OMPQQuestions(strWalk);

                    #endregion Walk

                    #region OrdinaryHousehold

                    string strOrdinaryHousehold = tblCsv.Rows[i]["Here is a list of five activities. Select the one number that best describes your current ability to<br>participate in each of these activities. - I can do ordinary household chores."].ToString();

                    scoreOrdinaryHousehold = GetScore_OMPQQuestions(strOrdinaryHousehold);

                    #endregion OrdinaryHousehold

                    #region Shopping

                    string strShopping = tblCsv.Rows[i]["Here is a list of five activities. Select the one number that best describes your current ability to<br>participate in each of these activities. - I can do the weekly shopping"].ToString();

                    scoreShopping = GetScore_OMPQQuestions(strShopping);

                    #endregion Shopping

                    #region Sleep

                    string strSleep = tblCsv.Rows[i]["Here is a list of five activities. Select the one number that best describes your current ability to<br>participate in each of these activities. - I can sleep at night."].ToString();

                    scoreSleep = GetScore_OMPQQuestions(strSleep);

                    #endregion Sleep

                    #region OMPQScore
                    int InjuryScore = 0;

                    if (TotalInjury > 4)
                        InjuryScore = 10;
                    else
                        InjuryScore = (TotalInjury * 2);

                    TotalOMPQScore = InjuryScore + (scoreDaysOfWork + scoreCurrentPain + scoreMonotonous + scorePainRate + scorePainScale + scorePainExperienced + scoreJobSatisfied + scoreTense + scoreDepressed + scorePhysicalActivity + scorePainIndication + scorePresentPain) + 80 - (scorePainDecrease + scorePainPersistent + scoreWorkChance + scoreLightWork + scoreWalk + scoreOrdinaryHousehold + scoreShopping + scoreSleep);

                    EmailScore = TotalOMPQScore;
                    #endregion OMPQScore

                    if (tblCsv.Rows[i]["RespondentID"] != null && tblCsv.Rows[i]["RespondentID"].ToString() != "")
                    {
                        dtData.Rows.Add(tblCsv.Rows[i]["RespondentID"], tblCsv.Rows[i]["CollectorID"], scoreNeck, scoreShoulder, scoreArm, scoreUpperBack, scoreLowerBack, scoreLeg, scoreOther, TotalInjury, scoreDaysOfWork, scoreCurrentPain, scoreMonotonous, scorePainRate, scorePainScale, scorePainExperienced, scorePainDecrease, scoreJobSatisfied, scoreTense, scoreDepressed, scorePainPersistent, scoreWorkChance, scorePhysicalActivity, scorePainIndication, scorePresentPain, scoreLightWork, scoreWalk, scoreOrdinaryHousehold, scoreShopping, scoreSleep, TotalOMPQScore, EmailScore);
                    }
                }
                return JsonConvert.SerializeObject(dtData);
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

        #region HOOS       

        public struct HoosLevelScore
        {
            public const double Zero = 0;
            public const double One = 4.6;
            public const double Two = 8.8;
            public const double Three = 12.7;
            public const double Four = 16.4;
            public const double Five = 20;
            public const double Six = 23.4;
            public const double Seven = 26.9;
            public const double Eight = 30.4;
            public const double Nine = 33.9;
            public const double Ten = 37.7;
            public const double Eleven = 41.7;
            public const double Twelve = 46.1;
            public const double Thirteen = 50.8;
            public const double Fourteen = 55.9;
            public const double Fifteen = 61.6;
            public const double Sixteen = 67.9;
            public const double Seventeen = 74.8;
            public const double Eighteen = 82.4;
            public const double Nineteen = 90.8;
            public const double Twenty = 100;
        }

        public string Calculate_HOOS_Score(DataTable tblCsv)
        {
            try
            {
                DataTable dtData = new DataTable();

                #region Add Columns
                dtData.Columns.Add("RespondentID");

                dtData.Columns.Add("CollectorID");

                dtData.Columns.Add("Descending stairs");

                dtData.Columns.Add("Getting in/out of bath or shower");

                dtData.Columns.Add("Sitting");

                dtData.Columns.Add("Running");

                dtData.Columns.Add("Twisting/pivoting on your loaded leg");

                dtData.Columns.Add("HOOSScore");

                #endregion Add Columns

                for (int i = 0; i < tblCsv.Rows.Count; i++)
                {
                    int scorestairs = 0, scoreShower = 0, scoreSitting = 0, scoreRunning = 0, scoreTwisting = 0, HoosScore = 0;
                    double TotalHOOSScore = 0;

                    #region Stairs

                    string strStairs = tblCsv.Rows[i]["Descending stairs"].ToString();
                    scorestairs = GetScore_HOOS(strStairs);

                    #endregion Stairs

                    #region Shower

                    string strShower = tblCsv.Rows[i]["Getting in/out of bath or shower"].ToString();
                    scoreShower = GetScore_HOOS(strShower);

                    #endregion Shower

                    #region Sitting

                    string strSitting = tblCsv.Rows[i]["Sitting"].ToString();
                    scoreSitting = GetScore_HOOS(strSitting);

                    #endregion Sitting

                    #region Running

                    string strRunning = tblCsv.Rows[i]["Running"].ToString();
                    scoreRunning = GetScore_HOOS(strRunning);

                    #endregion Running

                    #region Twisting

                    string strTwisting = tblCsv.Rows[i]["Twisting/pivoting on your loaded leg"].ToString();
                    scoreTwisting = GetScore_HOOS(strTwisting);

                    #endregion Twisting

                    #region HOOSScore                   

                    if (tblCsv.Rows[i]["RespondentID"] != null && tblCsv.Rows[i]["RespondentID"].ToString() != "")
                    {
                        if (scorestairs == int.MinValue || scoreShower == int.MinValue || scoreSitting == int.MinValue || scoreRunning == int.MinValue || scoreTwisting == int.MinValue)
                        {
                            var ScoreMessage = "Something has gone wrong. Please contact eHealthier by email at info@ehealthier.net";

                            dtData.Rows.Add(tblCsv.Rows[i]["RespondentID"], tblCsv.Rows[i]["CollectorID"], scorestairs, scoreShower, scoreSitting, scoreRunning, scoreTwisting, ScoreMessage);
                        }
                        else
                        {
                            HoosScore = scorestairs + scoreShower + scoreSitting + scoreRunning + scoreTwisting;
                            TotalHOOSScore = GetLevelValue_HOOS(HoosScore);

                            dtData.Rows.Add(tblCsv.Rows[i]["RespondentID"], tblCsv.Rows[i]["CollectorID"], scorestairs, scoreShower, scoreSitting, scoreRunning, scoreTwisting, TotalHOOSScore);
                        }
                    }
                    #endregion HOOSScore
                }
                return JsonConvert.SerializeObject(dtData);
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

        #region NeckPain       

        public string Calculate_NeckPain_Score(DataTable tblCsv)
        {
            try
            {
                DataTable dtData = new DataTable();

                #region Add Columns
                dtData.Columns.Add("RespondentID");

                dtData.Columns.Add("CollectorID");

                dtData.Columns.Add("Section 1 - Pain Intensity");

                dtData.Columns.Add("Section 2 - Personal Care (e.g., Washing, Dressing)");

                dtData.Columns.Add("Section 3 - Lifting");

                dtData.Columns.Add("Section 4 - Reading");

                dtData.Columns.Add("Section 5 - Headaches");

                dtData.Columns.Add("Section 6 - Concentrating");

                dtData.Columns.Add("Section 7 - Work");

                dtData.Columns.Add("Section 8 - Driving");

                dtData.Columns.Add("Section 9 - Sleeping");

                dtData.Columns.Add("Section 10 - Recreation");

                dtData.Columns.Add("NeckPainScore");

                #endregion Add Columns

                for (int i = 0; i < tblCsv.Rows.Count; i++)
                {
                    int scorePainIntensity = 0, scorePersonalCare = 0, scoreLifting = 0, scoreReading = 0, scoreHeadaches = 0, scoreConcentrating = 0, scoreWork = 0, scoreDriving = 0, scoreSleeping = 0, scoreRecreation = 0, TotalNeckPainScore = 0;

                    #region Pain Intensity

                    string strPainIntensity = tblCsv.Rows[i]["Section 1 - Pain Intensity"].ToString();
                    scorePainIntensity = GetScore_NeckPain(strPainIntensity);

                    #endregion Pain Intensity

                    #region Personal Care

                    string strPersonalCare = tblCsv.Rows[i]["Section 2 - Personal Care (e.g., Washing, Dressing)"].ToString();
                    scorePersonalCare = GetScore_NeckPain(strPersonalCare);

                    #endregion Personal Care

                    #region Lifting

                    string strLifting = tblCsv.Rows[i]["Section 3 - Lifting"].ToString();
                    scoreLifting = GetScore_NeckPain(strLifting);

                    #endregion Lifting

                    #region Reading

                    string strReading = tblCsv.Rows[i]["Section 4 - Reading"].ToString();
                    scoreReading = GetScore_NeckPain(strReading);

                    #endregion Reading

                    #region Headaches

                    string strHeadaches = tblCsv.Rows[i]["Section 5 - Headaches"].ToString();
                    scoreHeadaches = GetScore_NeckPain(strHeadaches);

                    #endregion Headaches

                    #region Concentrating

                    string strConcentrating = tblCsv.Rows[i]["Section 6 - Concentrating"].ToString();
                    scoreConcentrating = GetScore_NeckPain(strConcentrating);

                    #endregion Concentrating

                    #region Work

                    string strWork = tblCsv.Rows[i]["Section 7 - Work"].ToString();
                    scoreWork = GetScore_NeckPain(strWork);

                    #endregion Work

                    #region Driving

                    string strDriving = tblCsv.Rows[i]["Section 8 - Driving"].ToString();
                    scoreDriving = GetScore_NeckPain(strDriving);

                    #endregion Driving

                    #region Sleeping

                    string strSleeping = tblCsv.Rows[i]["Section 9 - Sleeping"].ToString();
                    scoreSleeping = GetScore_NeckPain(strSleeping);

                    #endregion Sleeping

                    #region Recreation

                    string strRecreation = tblCsv.Rows[i]["Section 10 - Recreation"].ToString();
                    scoreRecreation = GetScore_NeckPain(strRecreation);

                    #endregion Recreation

                    #region NeckPainScore                   

                    if (tblCsv.Rows[i]["RespondentID"] != null && tblCsv.Rows[i]["RespondentID"].ToString() != "")
                    {
                        TotalNeckPainScore = scorePainIntensity + scorePersonalCare + scoreLifting + scoreReading + scoreHeadaches + scoreConcentrating + scoreWork + scoreDriving + scoreSleeping + scoreRecreation;

                        dtData.Rows.Add(tblCsv.Rows[i]["RespondentID"], tblCsv.Rows[i]["CollectorID"], scorePainIntensity, scorePersonalCare, scoreLifting, scoreReading, scoreHeadaches, scoreConcentrating, scoreWork, scoreDriving, scoreSleeping, scoreRecreation, TotalNeckPainScore);
                    }
                    #endregion NeckPainScore
                }
                return JsonConvert.SerializeObject(dtData);
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

        #region KOOS       

        public string Calculate_KOOS_Score(DataTable tblCsv)
        {
            try
            {
                DataTable dtData = new DataTable();

                #region Add Columns
                dtData.Columns.Add("RespondentID");

                dtData.Columns.Add("CollectorID");

                dtData.Columns.Add("Rising from bed");

                dtData.Columns.Add("Putting on socks/stockings");

                dtData.Columns.Add("Rising from sitting");

                dtData.Columns.Add("Bending to floor");

                dtData.Columns.Add("Twisting/pivoting on your injured knee");

                dtData.Columns.Add("Kneeling");

                dtData.Columns.Add("Squatting");

                dtData.Columns.Add("KOOSScore");

                #endregion Add Columns

                for (int i = 0; i < tblCsv.Rows.Count; i++)
                {
                    int scoreRising = 0, scoreStockings = 0, scoreSitting = 0, scoreBending = 0, scoreTwisting = 0, scoreKneeling = 0, scoreSquatting = 0, KoosScore = 0;
                    double TotalKOOSScore = 0;

                    #region Rising

                    string strRising = tblCsv.Rows[i]["Rising from bed"].ToString();
                    scoreRising = GetScore_KOOS(strRising);

                    #endregion Stairs

                    #region Stockings

                    string strStockings = tblCsv.Rows[i]["Putting on socks/stockings"].ToString();
                    scoreStockings = GetScore_KOOS(strStockings);

                    #endregion Stockings

                    #region Sitting

                    string strSitting = tblCsv.Rows[i]["Rising from sitting"].ToString();
                    scoreSitting = GetScore_KOOS(strSitting);

                    #endregion Sitting

                    #region Bending

                    string strBending = tblCsv.Rows[i]["Bending to floor"].ToString();
                    scoreBending = GetScore_KOOS(strBending);

                    #endregion Bending

                    #region Twisting

                    string strTwisting = tblCsv.Rows[i]["Twisting/pivoting on your injured knee"].ToString();
                    scoreTwisting = GetScore_KOOS(strTwisting);

                    #endregion Twisting

                    #region Kneeling

                    string strKneeling = tblCsv.Rows[i]["Kneeling"].ToString();
                    scoreKneeling = GetScore_KOOS(strKneeling);

                    #endregion Kneeling

                    #region Squatting

                    string strSquatting = tblCsv.Rows[i]["Squatting"].ToString();
                    scoreSquatting = GetScore_KOOS(strSquatting);

                    #endregion Squatting

                    #region KOOSScore                   

                    if (tblCsv.Rows[i]["RespondentID"] != null && tblCsv.Rows[i]["RespondentID"].ToString() != "")
                    {
                        KoosScore = scoreRising + scoreStockings + scoreSitting + scoreBending + scoreTwisting + scoreKneeling + scoreSquatting;

                        TotalKOOSScore = GetLevelValue_KOOS(KoosScore);

                        dtData.Rows.Add(tblCsv.Rows[i]["RespondentID"], tblCsv.Rows[i]["CollectorID"], scoreRising, scoreStockings, scoreSitting, scoreBending, scoreTwisting, scoreKneeling, scoreSquatting, TotalKOOSScore);
                    }
                    #endregion KOOSScore
                }
                return JsonConvert.SerializeObject(dtData);
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

        #region K10  

        public string Calculate_K10_Score(DataTable tblCsv)
        {
            try
            {
                DataTable dtData = new DataTable();

                #region Add Columns
                dtData.Columns.Add("RespondentID");

                dtData.Columns.Add("CollectorID");

                dtData.Columns.Add("During the last 30 days, about how often did you feel tired out for no good reason?");

                dtData.Columns.Add("During the last 30 days, about how often did you feel nervous?");

                dtData.Columns.Add("During the last 30 days, about how often did you feel so nervous that nothing could calm you");

                dtData.Columns.Add("During the last 30 days, about how often did you feel hopeless?");

                dtData.Columns.Add("During the last 30 days, about how often did you feel restless or fidgety?");

                dtData.Columns.Add("During the last 30 days, about how often did you feel so restless you could not sit still?");

                dtData.Columns.Add("During the last 30 days, about how often did you feel depressed?");

                dtData.Columns.Add("During the last 30 days, about how often did you feel that everything was an effort?");

                dtData.Columns.Add("During the last 30 days, about how often did you feel so sad that nothing could cheer you up?");

                dtData.Columns.Add("K10Score");

                #endregion Add Columns

                for (int i = 0; i < tblCsv.Rows.Count; i++)
                {
                    int scoreTired = 0, scoreNervous = 0, scoreCalm = 0, scoreHopeless = 0, scoreFidgety = 0, scoreRestless = 0, scoreDepressed = 0, scoreEffort = 0, scoreCheer = 0, K10Score = 0;
                    double TotalK10Score = 0;

                    #region Tired

                    string strTired = tblCsv.Rows[i]["During the last 30 days, about how often did you feel tired out for no good reason?"].ToString();

                    scoreTired = GetScore_K10(strTired);

                    #endregion Tired

                    #region Nervous

                    string strNervous = tblCsv.Rows[i]["During the last 30 days, about how often did you feel nervous?"].ToString();

                    scoreNervous = GetScore_K10(strNervous);

                    #endregion Nervous

                    #region Calm

                    string strCalm = tblCsv.Rows[i]["During the last 30 days, about how often did you feel so nervous that nothing could calm you"].ToString();

                    scoreCalm = GetScore_K10(strCalm);

                    #endregion Calm

                    #region Hopeless

                    string strHopeless = tblCsv.Rows[i]["During the last 30 days, about how often did you feel hopeless?"].ToString();

                    scoreHopeless = GetScore_K10(strHopeless);

                    #endregion Hopeless

                    #region Fidgety

                    string strFidgety = tblCsv.Rows[i]["During the last 30 days, about how often did you feel restless or fidgety?"].ToString();

                    scoreFidgety = GetScore_K10(strFidgety);

                    #endregion Fidgety

                    #region Restless

                    string strRestless = tblCsv.Rows[i]["During the last 30 days, about how often did you feel so restless you could not sit still?"].ToString();

                    scoreRestless = GetScore_K10(strRestless);

                    #endregion Restless

                    #region Depressed

                    string strDepressed = tblCsv.Rows[i]["During the last 30 days, about how often did you feel depressed?"].ToString();

                    scoreDepressed = GetScore_K10(strDepressed);

                    #endregion Depressed

                    #region Effort

                    string strEffort = tblCsv.Rows[i]["During the last 30 days, about how often did you feel that everything was an effort?"].ToString();

                    scoreEffort = GetScore_K10(strEffort);

                    #endregion Effort

                    #region Cheer

                    string strCheer = tblCsv.Rows[i]["During the last 30 days, about how often did you feel so sad that nothing could cheer you up?"].ToString();

                    scoreCheer = GetScore_K10(strCheer);
                    #endregion Cheer                   

                    #region K10Score                   

                    if (tblCsv.Rows[i]["RespondentID"] != null && tblCsv.Rows[i]["RespondentID"].ToString() != "")
                    {
                        K10Score = scoreTired + scoreNervous + scoreCalm + scoreHopeless + scoreFidgety;

                        TotalK10Score = GetLevelValue_K10(K10Score);

                        dtData.Rows.Add(tblCsv.Rows[i]["RespondentID"], tblCsv.Rows[i]["CollectorID"], scoreTired, scoreNervous, scoreCalm, scoreHopeless, scoreFidgety, scoreRestless, scoreDepressed, scoreEffort, scoreCheer, TotalK10Score);
                    }
                    #endregion K10Score
                }
                return JsonConvert.SerializeObject(dtData);
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
            }
            return ScoreK10;
        }

        #endregion K10

        #region QDASH       

        public enum QDASHDifficulty
        {
            NoDifficulty = 1,
            MildDifficulty = 2,
            ModerateDifficulty = 3,
            SevereDifficulty = 4,
            SoMuchDifficulty = 5,
            Unable = 5
        };

        public enum QDASHLimited
        {
            NotLimited = 1,
            SlightlyLimited = 2,
            ModeratelyLimited = 3,
            VeryLimited = 4,
            Unable = 5
        };

        public enum QDASHEnum1
        {
            NotAtAll = 1,
            Slightly = 2,
            Moderately = 3,
            QuiteABit = 4,
            Extremely = 5,
        };

        public enum QDASHEnum2
        {
            None = 1,
            Mild = 2,
            Moderate = 3,
            Severe = 4,
            Extreme = 5
        };

        public string Calculate_QDASH_Score(DataTable tblCsv)
        {
            try
            {
                DataTable dtData = new DataTable();

                #region Add Columns
                dtData.Columns.Add("RespondentID");

                dtData.Columns.Add("CollectorID");

                dtData.Columns.Add("Please rate the claimant's ability to do the following activities in the last week. - Open a tight or new jar");

                dtData.Columns.Add("Please rate the claimant's ability to do the following activities in the last week. - Do heavy household chores (e.g. wash walls, wash floors)");

                dtData.Columns.Add("Please rate the claimant's ability to do the following activities in the last week. - Carry a shopping bag or briefcase");

                dtData.Columns.Add("Please rate the claimant's ability to do the following activities in the last week. - Wash your back");

                dtData.Columns.Add("Please rate the claimant's ability to do the following activities in the last week. - Use a knife to cut food");

                dtData.Columns.Add("Please rate the claimant's ability to do the following activities in the last week. - Recreational activities in which you take some force or impact through your arm, shoulder or hand (e.g. golf, hammering, tennis, etc.)");

                dtData.Columns.Add("During the past week, to what extent has your arm, shoulder or hand problem interfered with your normal social activities with family, friends, neighbours or groups?");

                dtData.Columns.Add("During the past week, were you limited in your work or other regular daily activities as a result of your arm, shoulder or hand problem?");

                dtData.Columns.Add("Please rate the severity of the following symptoms in the last week - Arm, shoulder or hand pain");

                dtData.Columns.Add("Please rate the severity of the following symptoms in the last week - Tingling (pins and needles) in your arm, shoulder or hand");

                dtData.Columns.Add("During the past week, how much difficulty have you had sleeping because of the pain in your arm, shoulder or hand?");

                dtData.Columns.Add("QDASHScore");

                #endregion Add Columns

                for (int i = 0; i < tblCsv.Rows.Count; i++)
                {
                    int scoreTight = 0, scoreChores = 0, scoreShopping = 0, scoreWash = 0, scoreKnife = 0, scoreRecreational = 0, scoreSocial = 0, scoreDaily = 0, scoreSymptoms = 0, scoreTingling = 0, scoreDifficulty = 0;

                    decimal TotalQDashScore = 0;

                    #region Tight

                    string strTight = tblCsv.Rows[i]["Please rate the claimant's ability to do the following activities in the last week. - Open a tight or new jar"].ToString();

                    if (strTight == "No difficulty")
                    {
                        scoreTight = (int)QDASHDifficulty.NoDifficulty;
                    }
                    else if (strTight == "Mild difficulty")
                    {
                        scoreTight = (int)QDASHDifficulty.MildDifficulty;
                    }
                    else if (strTight == "Moderate difficulty")
                    {
                        scoreTight = (int)QDASHDifficulty.ModerateDifficulty;
                    }
                    else if (strTight == "Severe difficulty")
                    {
                        scoreTight = (int)QDASHDifficulty.SevereDifficulty;
                    }
                    else if (strTight == "Unable")
                    {
                        scoreTight = (int)QDASHDifficulty.Unable;
                    }
                    #endregion Tight

                    #region Chores

                    string strChores = tblCsv.Rows[i]["Please rate the claimant's ability to do the following activities in the last week. - Do heavy household chores (e.g. wash walls, wash floors)"].ToString();

                    if (strChores == "No difficulty")
                    {
                        scoreChores = (int)QDASHDifficulty.NoDifficulty;
                    }
                    else if (strChores == "Mild difficulty")
                    {
                        scoreChores = (int)QDASHDifficulty.MildDifficulty;
                    }
                    else if (strChores == "Moderate difficulty")
                    {
                        scoreChores = (int)QDASHDifficulty.ModerateDifficulty;
                    }
                    else if (strChores == "Severe difficulty")
                    {
                        scoreChores = (int)QDASHDifficulty.SevereDifficulty;
                    }
                    else if (strChores == "Unable")
                    {
                        scoreChores = (int)QDASHDifficulty.Unable;
                    }
                    #endregion Chores

                    #region Shopping

                    string strShopping = tblCsv.Rows[i]["Please rate the claimant's ability to do the following activities in the last week. - Carry a shopping bag or briefcase"].ToString();

                    if (strShopping == "No difficulty")
                    {
                        scoreShopping = (int)QDASHDifficulty.NoDifficulty;
                    }
                    else if (strShopping == "Mild difficulty")
                    {
                        scoreShopping = (int)QDASHDifficulty.MildDifficulty;
                    }
                    else if (strShopping == "Moderate difficulty")
                    {
                        scoreShopping = (int)QDASHDifficulty.ModerateDifficulty;
                    }
                    else if (strShopping == "Severe difficulty")
                    {
                        scoreShopping = (int)QDASHDifficulty.SevereDifficulty;
                    }
                    else if (strShopping == "Unable")
                    {
                        scoreShopping = (int)QDASHDifficulty.Unable;
                    }
                    #endregion Shopping

                    #region Wash

                    string strWash = tblCsv.Rows[i]["Please rate the claimant's ability to do the following activities in the last week. - Wash your back"].ToString();

                    if (strWash == "No difficulty")
                    {
                        scoreWash = (int)QDASHDifficulty.NoDifficulty;
                    }
                    else if (strWash == "Mild difficulty")
                    {
                        scoreWash = (int)QDASHDifficulty.MildDifficulty;
                    }
                    else if (strWash == "Moderate difficulty")
                    {
                        scoreWash = (int)QDASHDifficulty.ModerateDifficulty;
                    }
                    else if (strWash == "Severe difficulty")
                    {
                        scoreWash = (int)QDASHDifficulty.SevereDifficulty;
                    }
                    else if (strWash == "Unable")
                    {
                        scoreWash = (int)QDASHDifficulty.Unable;
                    }
                    #endregion Wash

                    #region Knife

                    string strKnife = tblCsv.Rows[i]["Please rate the claimant's ability to do the following activities in the last week. - Use a knife to cut food"].ToString();

                    if (strKnife == "No difficulty")
                    {
                        scoreKnife = (int)QDASHDifficulty.NoDifficulty;
                    }
                    else if (strKnife == "Mild difficulty")
                    {
                        scoreKnife = (int)QDASHDifficulty.MildDifficulty;
                    }
                    else if (strKnife == "Moderate difficulty")
                    {
                        scoreKnife = (int)QDASHDifficulty.ModerateDifficulty;
                    }
                    else if (strKnife == "Severe difficulty")
                    {
                        scoreKnife = (int)QDASHDifficulty.SevereDifficulty;
                    }
                    else if (strKnife == "Unable")
                    {
                        scoreKnife = (int)QDASHDifficulty.Unable;
                    }
                    #endregion Knife

                    #region Recreational

                    string strRecreational = tblCsv.Rows[i]["Please rate the claimant's ability to do the following activities in the last week. - Recreational activities in which you take some force or impact through your arm, shoulder or hand (e.g. golf, hammering, tennis, etc.)"].ToString();

                    if (strRecreational == "No difficulty")
                    {
                        scoreRecreational = (int)QDASHDifficulty.NoDifficulty;
                    }
                    else if (strRecreational == "Mild difficulty")
                    {
                        scoreRecreational = (int)QDASHDifficulty.MildDifficulty;
                    }
                    else if (strRecreational == "Moderate difficulty")
                    {
                        scoreRecreational = (int)QDASHDifficulty.ModerateDifficulty;
                    }
                    else if (strRecreational == "Severe difficulty")
                    {
                        scoreRecreational = (int)QDASHDifficulty.SevereDifficulty;
                    }
                    else if (strRecreational == "Unable")
                    {
                        scoreRecreational = (int)QDASHDifficulty.Unable;
                    }
                    #endregion Recreational

                    #region Social

                    string strSocial = tblCsv.Rows[i]["During the past week, to what extent has your arm, shoulder or hand problem interfered with your normal social activities with family, friends, neighbours or groups?"].ToString();

                    if (strSocial == "Not at all")
                    {
                        scoreSocial = (int)QDASHEnum1.NotAtAll;
                    }
                    else if (strSocial == "Slightly")
                    {
                        scoreSocial = (int)QDASHEnum1.Slightly;
                    }
                    else if (strSocial == "Moderately")
                    {
                        scoreSocial = (int)QDASHEnum1.Moderately;
                    }
                    else if (strSocial == "Quite a bit")
                    {
                        scoreSocial = (int)QDASHEnum1.QuiteABit;
                    }
                    else if (strSocial == "Extremely")
                    {
                        scoreSocial = (int)QDASHEnum1.Extremely;
                    }
                    #endregion Social

                    #region Daily

                    string strDaily = tblCsv.Rows[i]["During the past week, were you limited in your work or other regular daily activities as a result of your arm, shoulder or hand problem?"].ToString();

                    if (strDaily == "Not limited at all")
                    {
                        scoreDaily = (int)QDASHLimited.NotLimited;
                    }
                    else if (strDaily == "Slightly limited")
                    {
                        scoreDaily = (int)QDASHLimited.SlightlyLimited;
                    }
                    else if (strDaily == "Moderately limited")
                    {
                        scoreDaily = (int)QDASHLimited.ModeratelyLimited;
                    }
                    else if (strDaily == "Very limited")
                    {
                        scoreDaily = (int)QDASHLimited.VeryLimited;
                    }
                    else if (strDaily == "Unable")
                    {
                        scoreDaily = (int)QDASHLimited.Unable;
                    }
                    #endregion Daily

                    #region Symptoms

                    string strSymptoms = tblCsv.Rows[i]["Please rate the severity of the following symptoms in the last week - Arm, shoulder or hand pain"].ToString();

                    if (strSymptoms == "None")
                    {
                        scoreSymptoms = (int)QDASHEnum2.None;
                    }
                    else if (strSymptoms == "Mild")
                    {
                        scoreSymptoms = (int)QDASHEnum2.Mild;
                    }
                    else if (strSymptoms == "Moderate")
                    {
                        scoreSymptoms = (int)QDASHEnum2.Moderate;
                    }
                    else if (strSymptoms == "Severe")
                    {
                        scoreSymptoms = (int)QDASHEnum2.Severe;
                    }
                    else if (strSymptoms == "Extreme")
                    {
                        scoreSymptoms = (int)QDASHEnum2.Extreme;
                    }
                    #endregion Symptoms

                    #region Tingling

                    string strTingling = tblCsv.Rows[i]["Please rate the severity of the following symptoms in the last week - Tingling (pins and needles) in your arm, shoulder or hand"].ToString();

                    if (strTingling == "None")
                    {
                        scoreTingling = (int)QDASHEnum2.None;
                    }
                    else if (strTingling == "Mild")
                    {
                        scoreTingling = (int)QDASHEnum2.Mild;
                    }
                    else if (strTingling == "Moderate")
                    {
                        scoreTingling = (int)QDASHEnum2.Moderate;
                    }
                    else if (strTingling == "Severe")
                    {
                        scoreTingling = (int)QDASHEnum2.Severe;
                    }
                    else if (strTingling == "Extreme")
                    {
                        scoreTingling = (int)QDASHEnum2.Extreme;
                    }

                    #endregion Tingling

                    #region Difficulty

                    string strDifficulty = tblCsv.Rows[i]["During the past week, how much difficulty have you had sleeping because of the pain in your arm, shoulder or hand?"].ToString();

                    if (strDifficulty == "No difficulty")
                    {
                        scoreDifficulty = (int)QDASHDifficulty.NoDifficulty;
                    }
                    else if (strDifficulty == "Mild difficulty")
                    {
                        scoreDifficulty = (int)QDASHDifficulty.MildDifficulty;
                    }
                    else if (strDifficulty == "Moderate difficulty")
                    {
                        scoreDifficulty = (int)QDASHDifficulty.ModerateDifficulty;
                    }
                    else if (strDifficulty == "Severe difficulty")
                    {
                        scoreDifficulty = (int)QDASHDifficulty.SevereDifficulty;
                    }
                    else if (strDifficulty == "So much difficulty I can't sleep")
                    {
                        scoreDifficulty = (int)QDASHDifficulty.SoMuchDifficulty;
                    }

                    #endregion Difficulty

                    #region QDASHScore                   

                    if (tblCsv.Rows[i]["RespondentID"] != null && tblCsv.Rows[i]["RespondentID"].ToString() != "")
                    {
                        decimal Sum = scoreTight + scoreChores + scoreShopping + scoreWash + scoreKnife + scoreRecreational + scoreSocial + scoreDaily + scoreSymptoms + scoreTingling + scoreDifficulty;

                        decimal value1 = Sum / 11;

                        decimal TotalScore = (value1 - 1) * 25;
                        TotalQDashScore = Math.Round(TotalScore, 1, MidpointRounding.ToEven);

                        dtData.Rows.Add(tblCsv.Rows[i]["RespondentID"], tblCsv.Rows[i]["CollectorID"], scoreTight, scoreChores, scoreShopping, scoreWash, scoreKnife, scoreRecreational, scoreSocial, scoreDaily, scoreSymptoms, scoreTingling, scoreDifficulty, TotalQDashScore);
                    }
                    #endregion QDASHScore
                }
                return JsonConvert.SerializeObject(dtData);
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

        public enum OswestryScore
        {
            SleepLessThan2Hours = 8,
            SleepLessThan4Hours = 6,
            SleepLessThan6Hours = 4,
            CrawlToToilet = 10,
            Weight_WithoutIncreasePain = 0,
            HeavyWeight_IncreasePain = 2,
            VeryLightWeight = 8,
            StressfulActivities = 4,
            CanSitOnChair = 0,
            FavouriteChair = 2,
            PainMedicine = 2,
            Stand_IncreasePain = 2,
            Stand_WithoutIncreasePain = 0,
            Care_withoutIncreasePain = 0,
            Care_IncreasePain = 2,
            Tolerate_PainWithoutUse_Medicine = 0,
            Travel_WithoutIncreasePain = 0,
            Travel_IncreasePain = 2,
            Walk_crutches = 8,
            CanNotLiftAtAll = 10,
            WashWithDifficulty = 10,
            Hardly_SocialLife = 10,
            Help_AspectOfCare = 8,
            Manage_Personelcare = 6,
            Slow_Careful = 4,
            HomeMaking_NotPain = 0,
            HomeMaking_IncreasePain = 2,
            PainPrevents = 10,
            Travel_1Hour = 6,
            Travel_2Hour = 4,
            Travel_UnderHalfHour = 8,
            Sleep_NeverDisturbed = 0,
            Social_NotIncreasePain = 0,
            Social_IncreasePain = 2,
            NotPrevent_Walking = 0,
            Restricted_SocialLife = 8,
            Medicine_NotEffectOnPain = 10,
            Medicine_CompleteRelief = 4,
            Medicine_LittleRelief = 8,
            Medicine_ModerateRelief = 6,
            PainPrevents_Anything = 6,
            PainPrevents_LightDuties = 8,
            PainPrevents_OutVeryOften = 6,
            PainPrevents_HeaveyWeight_floor = 4,
            PainPrevents_LightToMediumWeight = 6,
            PainPrevents_EnergeticActivities = 4,
            PainPrevents_HomeMakingChores = 10,
            PainPrevents_SittingAtAll = 10,
            PainPrevents_SittingMoreThan_HalfHour = 6,
            PainPrevents_SittingMoreThan_1Hour = 4,
            PainPrevents_SittingMoreThan_10Minute = 8,
            PainPrevents_SleepingAtAll = 10,
            PainPrevents_StandingAtAll = 10,
            PainPrevents_StandingMoreThan_HalfHour = 6,
            PainPrevents_StandingMoreThan_1Hour = 4,
            PainPrevents_StandingMoreThan_10Minute = 8,
            PainPrevents_WalkingMorethan1Mile = 2,
            PainPrevents_WalkingMorethan_HalfMile = 4,
            PainPrevents_WalkingMorethanQuarterMile = 6,
            Manage_WithoutPainMedicine = 2
        };

        public string Calculate_Oswestry_Score(DataTable tblCsv)
        {
            try
            {
                DataTable dtData = new DataTable();

                #region Add Columns
                dtData.Columns.Add("RespondentID");

                dtData.Columns.Add("CollectorID");

                dtData.Columns.Add("Section 1 - Pain Intensity");

                dtData.Columns.Add("Section 2 - Personal Care (e.g., Washing, Dressing)");

                dtData.Columns.Add("Section 3 - Lifting");

                dtData.Columns.Add("Section 4 - Walking");

                dtData.Columns.Add("Section 5 - Sitting");

                dtData.Columns.Add("Section 6 - Standing");

                dtData.Columns.Add("Section 7 - Sleeping");

                dtData.Columns.Add("Section 8 - Social Life");

                dtData.Columns.Add("Section 9 - Traveling");

                dtData.Columns.Add("Section 10 - Employment/Home-making");

                dtData.Columns.Add("OswestryScore");

                #endregion Add Columns

                for (int i = 0; i < tblCsv.Rows.Count; i++)
                {
                    int scorePainIntensity = 0, scorePersonalCare = 0, scoreLifting = 0, scoreWalking = 0, scoreSitting = 0, scoreStanding = 0, scoreSleeping = 0, scoreSocialLife = 0, scoreTraveling = 0, scoreHomeMaking = 0, TotalOwestreyScore = 0;

                    #region Pain Intensity

                    string strPainIntensity = tblCsv.Rows[i]["Section 1 - Pain Intensity"].ToString();

                    scorePainIntensity = GetOswestryScore(strPainIntensity);

                    #endregion Pain Intensity

                    #region Personal Care

                    string strPersonalCare = tblCsv.Rows[i]["Section 2 - Personal Care (e.g., Washing, Dressing)"].ToString();

                    scorePersonalCare = GetOswestryScore(strPersonalCare);

                    #endregion Personal Care

                    #region Lifting

                    string strLifting = tblCsv.Rows[i]["Section 3 - Lifting"].ToString();

                    scoreLifting = GetOswestryScore(strLifting);

                    #endregion Lifting

                    #region Walking

                    string strWalking = tblCsv.Rows[i]["Section 4 - Walking"].ToString();

                    scoreWalking = GetOswestryScore(strWalking);

                    #endregion Walking

                    #region Sitting

                    string strSitting = tblCsv.Rows[i]["Section 5 - Sitting"].ToString();
                    scoreSitting = GetOswestryScore(strSitting);

                    #endregion Sitting

                    #region Standing

                    string strStanding = tblCsv.Rows[i]["Section 6 - Standing"].ToString();
                    scoreStanding = GetOswestryScore(strStanding);

                    #endregion Standing

                    #region Sleeping

                    string strSleeping = tblCsv.Rows[i]["Section 7 - Sleeping"].ToString();
                    scoreSleeping = GetOswestryScore(strSleeping);

                    #endregion Sleeping

                    #region SocialLife

                    string strSocialLife = tblCsv.Rows[i]["Section 8 - Social Life"].ToString();

                    scoreSocialLife = GetOswestryScore(strSocialLife);
                    #endregion SocialLife

                    #region Traveling

                    string strTraveling = tblCsv.Rows[i]["Section 9 - Traveling"].ToString();

                    scoreTraveling = GetOswestryScore(strTraveling);
                    #endregion Traveling

                    #region HomeMaking

                    string strHomeMaking = tblCsv.Rows[i]["Section 10 - Employment/Home-making"].ToString();

                    scoreHomeMaking = GetOswestryScore(strHomeMaking);
                    #endregion HomeMaking

                    #region OswestryScore                   

                    if (tblCsv.Rows[i]["RespondentID"] != null && tblCsv.Rows[i]["RespondentID"].ToString() != "")
                    {
                        TotalOwestreyScore = scorePainIntensity + scorePersonalCare + scoreLifting + scoreWalking + scoreSitting + scoreStanding + scoreSleeping + scoreSocialLife + scoreTraveling + scoreHomeMaking;

                        dtData.Rows.Add(tblCsv.Rows[i]["RespondentID"], tblCsv.Rows[i]["CollectorID"], scorePainIntensity, scorePersonalCare, scoreLifting, scoreWalking, scoreSitting, scoreStanding, scoreSleeping, scoreSocialLife, scoreTraveling, scoreHomeMaking, TotalOwestreyScore);
                    }
                    #endregion OswestryScore
                }
                return JsonConvert.SerializeObject(dtData);
            }
            catch (Exception ex)
            {
                LogEntry.AddLog(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace);
                return "";
            }
        }

        public int GetOswestryScore(string Title)
        {
            int Score = 0;
            try
            {
                if (Title == "Even when I take medication, I sleep less than 2 hours.")
                {
                    Score = (int)OswestryScore.SleepLessThan2Hours;
                }
                else if (Title == "Even when I take medication, I sleep less than 4 hours.")
                {
                    Score = (int)OswestryScore.SleepLessThan4Hours;
                }
                else if (Title == "Even when I take medication, I sleep less than 6 hours.")
                {
                    Score = (int)OswestryScore.SleepLessThan6Hours;
                }
                else if (Title == "I am in bed most of the time and have to crawl to the toilet.")
                {
                    Score = (int)OswestryScore.CrawlToToilet;
                }
                else if (Title == "I can lift heavy weights without increased pain.")
                {
                    Score = (int)OswestryScore.Weight_WithoutIncreasePain;
                }
                else if (Title == "I can lift heavy weights, but it causes increased pain.")
                {
                    Score = (int)OswestryScore.HeavyWeight_IncreasePain;
                }
                else if (Title == "I can lift only very light weights.")
                {
                    Score = (int)OswestryScore.VeryLightWeight;
                }
                else if (Title == "I can perform most of my home-making/job duties, but pain prevents me from performing more physically stressful activities (e.g. lifting, vacuuming).")
                {
                    Score = (int)OswestryScore.StressfulActivities;
                }
                else if (Title == "I can sit in any chair as long as I like")
                {
                    Score = (int)OswestryScore.CanSitOnChair;
                }
                else if (Title == "I can sit in my favourite chair for as long as I like")
                {
                    Score = (int)OswestryScore.FavouriteChair;
                }
                else if (Title == "I can sleep well only using pain medication.")
                {
                    Score = (int)OswestryScore.PainMedicine;
                }
                else if (Title == "I can stand as long as I want but it increases my pain.")
                {
                    Score = (int)OswestryScore.Stand_IncreasePain;
                }
                else if (Title == "I can stand as long as I want without increased pain.")
                {
                    Score = (int)OswestryScore.Stand_WithoutIncreasePain;
                }
                else if (Title == "I can take care of myself normally without causing increased pain.")
                {
                    Score = (int)OswestryScore.Care_withoutIncreasePain;
                }
                else if (Title == "I can take care of myself normally, but it increases my pain.")
                {
                    Score = (int)OswestryScore.Care_IncreasePain;
                }
                else if (Title == "I can tolerate the pain I have without having to use pain medication.")
                {
                    Score = (int)OswestryScore.Tolerate_PainWithoutUse_Medicine;
                }
                else if (Title == "I can travel anywhere without increased pain.")
                {
                    Score = (int)OswestryScore.Travel_WithoutIncreasePain;
                }
                else if (Title == "I can travel anywhere, but it increases my pain..")
                {
                    Score = (int)OswestryScore.Travel_IncreasePain;
                }
                else if (Title == "I can walk only with crutches or a cane.")
                {
                    Score = (int)OswestryScore.Walk_crutches;
                }
                else if (Title == "I cannot lift or carry anything at all.")
                {
                    Score = (int)OswestryScore.CanNotLiftAtAll;
                }
                else if (Title == "I do not get dressed, I wash with difficulty, and stay in bed.")
                {
                    Score = (int)OswestryScore.WashWithDifficulty;
                }
                else if (Title == "I have hardly any social life because of my pain.")
                {
                    Score = (int)OswestryScore.Hardly_SocialLife;
                }
                else if (Title == "I need help every day in most aspects of my care.")
                {
                    Score = (int)OswestryScore.Help_AspectOfCare;
                }
                else if (Title == "I need help, but I am able to manage most of my personal care.")
                {
                    Score = (int)OswestryScore.Manage_Personelcare;
                }
                else if (Title == "It is painful to take care of myself, and I am slow and careful.")
                {
                    Score = (int)OswestryScore.Slow_Careful;
                }
                else if (Title == "My normal home-making/job activities do not cause pain.")
                {
                    Score = (int)OswestryScore.HomeMaking_NotPain;
                }
                else if (Title == "My normal home-making/job activities increase my pain, but I can still perform all that is required of me.")
                {
                    Score = (int)OswestryScore.HomeMaking_IncreasePain;
                }
                else if (Title == "My pain prevents all travel except for visits to the physician/therapist or hospital.")
                {
                    Score = (int)OswestryScore.PainPrevents;
                }
                else if (Title == "My pain restricts my travel over 1 hour.")
                {
                    Score = (int)OswestryScore.Travel_1Hour;
                }
                else if (Title == "My pain restricts my travel over 2 hours.")
                {
                    Score = (int)OswestryScore.Travel_2Hour;
                }
                else if (Title == "My pain restricts my travel to short necessary journeys under 1/2 hour.")
                {
                    Score = (int)OswestryScore.Travel_UnderHalfHour;
                }
                else if (Title == "My sleep is never disturbed by pain.")
                {
                    Score = (int)OswestryScore.Sleep_NeverDisturbed;
                }
                else if (Title == "My social life is normal and does not increase my pain.")
                {
                    Score = (int)OswestryScore.Social_NotIncreasePain;
                }
                else if (Title == "My social life is normal, but it increases my level of pain.")
                {
                    Score = (int)OswestryScore.Social_IncreasePain;
                }
                else if (Title == "Pain does not prevent me from walking any distance.")
                {
                    Score = (int)OswestryScore.NotPrevent_Walking;
                }
                else if (Title == "Pain has restricted my social life to my home.")
                {
                    Score = (int)OswestryScore.Restricted_SocialLife;
                }
                else if (Title == "Pain medication has no effect on my pain")
                {
                    Score = (int)OswestryScore.Medicine_NotEffectOnPain;
                }
                else if (Title == "Pain medication provides me with complete relief from pain.")
                {
                    Score = (int)OswestryScore.Medicine_CompleteRelief;
                }
                else if (Title == "Pain medication provides me with little relief from pain")
                {
                    Score = (int)OswestryScore.Medicine_LittleRelief;
                }
                else if (Title == "Pain medication provides me with moderate relief from pain")
                {
                    Score = (int)OswestryScore.Medicine_ModerateRelief;
                }
                else if (Title == "Pain prevents me from doing anything but light duties")
                {
                    Score = (int)OswestryScore.PainPrevents_Anything;
                }
                else if (Title == "Pain prevents me from doing even light duties.")
                {
                    Score = (int)OswestryScore.PainPrevents_LightDuties;
                }
                else if (Title == "Pain prevents me from going out very often.")
                {
                    Score = (int)OswestryScore.PainPrevents_OutVeryOften;
                }
                else if (Title == "Pain prevents me from lifting heavy weights off the floor, but I can manage if the weights are conveniently positioned (e.g., on a table).")
                {
                    Score = (int)OswestryScore.PainPrevents_HeaveyWeight_floor;
                }
                else if (Title == "Pain prevents me from lifting heavy weights, but I can manage light to medium weights if they are conveniently positioned.")
                {
                    Score = (int)OswestryScore.PainPrevents_LightToMediumWeight;
                }
                else if (Title == "Pain prevents me from participating in more energetic activities (e.g., sports, dancing).")
                {
                    Score = (int)OswestryScore.PainPrevents_EnergeticActivities;
                }
                else if (Title == "Pain prevents me from performing any job or home-making chores.")
                {
                    Score = (int)OswestryScore.PainPrevents_HomeMakingChores;
                }
                else if (Title == "Pain prevents me from sitting at all.")
                {
                    Score = (int)OswestryScore.PainPrevents_SittingAtAll;
                }
                else if (Title == "Pain prevents me from sitting for more than ½ an hour")
                {
                    Score = (int)OswestryScore.PainPrevents_SittingMoreThan_HalfHour;
                }
                else if (Title == "Pain prevents me from sitting for more than 1 hour.")
                {
                    Score = (int)OswestryScore.PainPrevents_SittingMoreThan_1Hour;
                }
                else if (Title == "Pain prevents me from sitting for more that 10 minutes")
                {
                    Score = (int)OswestryScore.PainPrevents_SittingMoreThan_10Minute;
                }
                else if (Title == "Pain prevents me from sleeping at all.")
                {
                    Score = (int)OswestryScore.PainPrevents_SleepingAtAll;
                }
                else if (Title == "Pain prevents me from standing at all.")
                {
                    Score = (int)OswestryScore.PainPrevents_StandingAtAll;
                }
                else if (Title == "Pain prevents me from standing for more than ½ an hour.")
                {
                    Score = (int)OswestryScore.PainPrevents_StandingMoreThan_HalfHour;
                }
                else if (Title == "Pain prevents me from standing for more than 1 hour.")
                {
                    Score = (int)OswestryScore.PainPrevents_StandingMoreThan_1Hour;
                }
                else if (Title == "Pain prevents me from standing for more than 10 minutes.")
                {
                    Score = (int)OswestryScore.PainPrevents_StandingMoreThan_10Minute;
                }
                else if (Title == "Pain prevents me from walking more than 1 mile. (1 mile = 1.6 km)")
                {
                    Score = (int)OswestryScore.PainPrevents_WalkingMorethan1Mile;
                }
                else if (Title == "Pain prevents me from walking more than 1/2 mile.")
                {
                    Score = (int)OswestryScore.PainPrevents_WalkingMorethan_HalfMile;
                }
                else if (Title == "Pain prevents me from walking more than 1/4 mile.")
                {
                    Score = (int)OswestryScore.PainPrevents_WalkingMorethanQuarterMile;
                }
                else if (Title == "The pain is bad, but I can manage without having to take pain medication.")
                {
                    Score = (int)OswestryScore.Manage_WithoutPainMedicine;
                }
            }
            catch (Exception)
            {
            }
            return Score;
        }
        #endregion Oswestry
    }
}
