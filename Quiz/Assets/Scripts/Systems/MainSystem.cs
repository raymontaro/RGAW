using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Tiny.Text;
using Unity.Tiny.UI;

public class MainSystem : SystemBase
{
    public enum Gamestate {start, question,answer,result,thankyou};
    public Gamestate myGameState;
    bool isCorrect;
    int correctCount;
    int questionCount = 10;
    int currentQuestion = 1;
    float restartWaitTimer = 6f;

    protected override void OnUpdate()
    {
        var uiSys = World.GetExistingSystem<ProcessUIEvents>();

        var restartEntity = uiSys.GetEntityByUIName("Restart");
        //var restartTextEntity = uiSys.GetEntityByUIName("RestartText");

        var trueButonEntity = uiSys.GetEntityByUIName("TrueButton");
        var trueButtonState = GetComponent<UIState>(trueButonEntity);
        var falseButonEntity = uiSys.GetEntityByUIName("FalseButton");
        var falseButtonState = GetComponent<UIState>(falseButonEntity);
        var nextButonEntity = uiSys.GetEntityByUIName("NextButton");
        var nextButtonState = GetComponent<UIState>(nextButonEntity);
        var startButonEntity = uiSys.GetEntityByUIName("StartButton");
        var startButtonState = GetComponent<UIState>(startButonEntity);
        var restartButonEntity = uiSys.GetEntityByUIName("RestartButton");
        var restartButtonState = GetComponent<UIState>(restartButonEntity);

        switch (myGameState)
        {
            case Gamestate.start:
                ShowStart();
                if (startButtonState.IsClicked)
                {                    
                    myGameState = Gamestate.question;
                }
                break;
            case Gamestate.question:
                ShowQuestion();
                if (trueButtonState.IsClicked)
                {
                    CheckAnswer(true);
                    myGameState = Gamestate.answer;
                }
                if (falseButtonState.IsClicked)
                {
                    CheckAnswer(false);
                    myGameState = Gamestate.answer;
                }
                break;
            case Gamestate.answer:
                ShowAnswer();
                if (nextButtonState.IsClicked)
                {                    
                    if (currentQuestion < questionCount)
                    {
                        myGameState = Gamestate.question;
                        currentQuestion++;
                    }
                    else
                    {
                        myGameState = Gamestate.result;
                    }
                }
                break;
            case Gamestate.result:
                ShowResult();
                if (restartWaitTimer > 0f)
                {
                    restartWaitTimer -= Time.DeltaTime;
                }
                else
                {
                    restartWaitTimer = 6f;

                    myGameState = Gamestate.thankyou;
                }

                //if (restartWaitTimer <= 3)
                //{
                //    if (restartWaitTimer > 0)
                //    {
                //        var restartTransform = GetComponent<RectTransform>(restartEntity);
                //        restartTransform.Hidden = false;
                //        SetComponent(restartEntity, restartTransform);

                //        TextLayout.SetEntityTextRendererString(EntityManager, restartTextEntity, "Game will restart in " + math.ceil(restartWaitTimer) + " seconds.");
                //    }
                //    else
                //    {
                //        restartWaitTimer = 6f;

                //        correctCount = 0;
                //        currentQuestion = 1;

                //        var restartTransform = GetComponent<RectTransform>(restartEntity);
                //        restartTransform.Hidden = true;
                //        SetComponent(restartEntity, restartTransform);

                //        myGameState = Gamestate.question;
                //    }
                //}
                break;
            case Gamestate.thankyou:
                ShowThankyou();
                if (restartButtonState.IsClicked)
                {
                    correctCount = 0;
                    currentQuestion = 1;

                    var restartTransform = GetComponent<RectTransform>(restartEntity);
                    restartTransform.Hidden = true;
                    SetComponent(restartEntity, restartTransform);

                    myGameState = Gamestate.question;
                }                                
                break;
        }
    }

    private void CheckAnswer(bool a)
    {
        switch (currentQuestion)
        {
            case 1:
                isCorrect = (a == false);
                break;
            case 2:
                isCorrect = (a == false);
                break;
            case 3:
                isCorrect = (a == false);
                break;
            case 4:
                isCorrect = (a == false);
                break;
            case 5:
                isCorrect = (a == false);
                break;
            case 6:
                isCorrect = (a == false);
                break;
            case 7:
                isCorrect = (a == false);
                break;
            case 8:
                isCorrect = (a == false);
                break;
            case 9:
                isCorrect = (a == false);
                break;
            case 10:
                isCorrect = (a == false);
                break;
        }

        if (isCorrect)
        {
            correctCount++;
        }
    }

    private void ShowQuestion()
    {
        var uiSys = World.GetExistingSystem<ProcessUIEvents>();

        var questionsEntity = uiSys.GetEntityByUIName("Questions");
        var answersEntity = uiSys.GetEntityByUIName("Answers");
        var resultsEntity = uiSys.GetEntityByUIName("Results");
        var startEntity = uiSys.GetEntityByUIName("Start");
        var restartEntity = uiSys.GetEntityByUIName("Restart");


        var questionsTransform = GetComponent<RectTransform>(questionsEntity);
        questionsTransform.Hidden = false;
        SetComponent(questionsEntity, questionsTransform);
        var answersTransform = GetComponent<RectTransform>(answersEntity);
        answersTransform.Hidden = true;
        SetComponent(answersEntity, answersTransform);
        var resultsTransform = GetComponent<RectTransform>(resultsEntity);
        resultsTransform.Hidden = true;
        SetComponent(resultsEntity, resultsTransform);
        var startTransform = GetComponent<RectTransform>(startEntity);
        startTransform.Hidden = true;
        SetComponent(startEntity, startTransform);
        var restartTransform = GetComponent<RectTransform>(restartEntity);
        restartTransform.Hidden = true;
        SetComponent(restartEntity, restartTransform);

        for (int i = 1; i <= questionCount; i++)
        {
            var questionEntity = uiSys.GetEntityByUIName("Question (" + i + ")");
            var questionTransform = GetComponent<RectTransform>(questionEntity);
            Debug.Log("Question (" + i + ") hidden = " + !(i == currentQuestion));
            questionTransform.Hidden = !(i == currentQuestion);            
            SetComponent(questionEntity, questionTransform);
        }

        //var restartEntity = uiSys.GetEntityByUIName("Restart");

        //var restartTransform = GetComponent<RectTransform>(restartEntity);
        //restartTransform.Hidden = true;
        //SetComponent(restartEntity, restartTransform);
    }

    private void ShowAnswer()
    {
        var uiSys = World.GetExistingSystem<ProcessUIEvents>();

        var questionsEntity = uiSys.GetEntityByUIName("Questions");
        var answersEntity = uiSys.GetEntityByUIName("Answers");
        var resultsEntity = uiSys.GetEntityByUIName("Results");
        var startEntity = uiSys.GetEntityByUIName("Start");
        var restartEntity = uiSys.GetEntityByUIName("Restart");

        var questionsTransform = GetComponent<RectTransform>(questionsEntity);
        questionsTransform.Hidden = true;
        SetComponent(questionsEntity, questionsTransform);
        var answersTransform = GetComponent<RectTransform>(answersEntity);
        answersTransform.Hidden = false;
        SetComponent(answersEntity, answersTransform);
        var resultsTransform = GetComponent<RectTransform>(resultsEntity);
        resultsTransform.Hidden = true;
        SetComponent(resultsEntity, resultsTransform);
        var startTransform = GetComponent<RectTransform>(startEntity);
        startTransform.Hidden = true;
        SetComponent(startEntity, startTransform);
        var restartTransform = GetComponent<RectTransform>(restartEntity);
        restartTransform.Hidden = true;
        SetComponent(restartEntity, restartTransform);

        if (isCorrect)
        {
            for (int i = 1; i <= questionCount; i++)
            {
                var answerEntity = uiSys.GetEntityByUIName("AnswerCorrect (" + i + ")");
                var answerTransform = GetComponent<RectTransform>(answerEntity);
                answerTransform.Hidden = !(i == currentQuestion);
                SetComponent(answerEntity, answerTransform);
            }

            for (int i = 1; i <= questionCount; i++)
            {
                var answerEntity = uiSys.GetEntityByUIName("AnswerIncorrect (" + i + ")");
                var answerTransform = GetComponent<RectTransform>(answerEntity);
                answerTransform.Hidden = true;
                SetComponent(answerEntity, answerTransform);
            }
        }
        else
        {
            for (int i = 1; i <= questionCount; i++)
            {
                var answerEntity = uiSys.GetEntityByUIName("AnswerIncorrect (" + i + ")");
                var answerTransform = GetComponent<RectTransform>(answerEntity);
                answerTransform.Hidden = !(i == currentQuestion);
                SetComponent(answerEntity, answerTransform);
            }

            for (int i = 1; i <= questionCount; i++)
            {
                var answerEntity = uiSys.GetEntityByUIName("AnswerCorrect (" + i + ")");
                var answerTransform = GetComponent<RectTransform>(answerEntity);
                answerTransform.Hidden = true;
                SetComponent(answerEntity, answerTransform);
            }
        }

        //var restartEntity = uiSys.GetEntityByUIName("Restart");

        //var restartTransform = GetComponent<RectTransform>(restartEntity);
        //restartTransform.Hidden = true;
        //SetComponent(restartEntity, restartTransform);
    }

    private void ShowResult()
    {
        var uiSys = World.GetExistingSystem<ProcessUIEvents>();

        var questionsEntity = uiSys.GetEntityByUIName("Questions");
        var answersEntity = uiSys.GetEntityByUIName("Answers");
        var resultsEntity = uiSys.GetEntityByUIName("Results");
        var resultTextEntity = uiSys.GetEntityByUIName("resultText");
        var startEntity = uiSys.GetEntityByUIName("Start");
        var restartEntity = uiSys.GetEntityByUIName("Restart");

        var questionsTransform = GetComponent<RectTransform>(questionsEntity);
        questionsTransform.Hidden = true;
        SetComponent(questionsEntity, questionsTransform);
        var answersTransform = GetComponent<RectTransform>(answersEntity);
        answersTransform.Hidden = true;
        SetComponent(answersEntity, answersTransform);
        var resultsTransform = GetComponent<RectTransform>(resultsEntity);
        resultsTransform.Hidden = false;
        SetComponent(resultsEntity, resultsTransform);
        var startTransform = GetComponent<RectTransform>(startEntity);
        startTransform.Hidden = true;
        SetComponent(startEntity, startTransform);
        var restartTransform = GetComponent<RectTransform>(restartEntity);
        restartTransform.Hidden = true;
        SetComponent(restartEntity, restartTransform);

        for (int i = 1; i <= 3; i++)
        {
            var resultEntity = uiSys.GetEntityByUIName("result (" + i + ")");
            var resultTransform = GetComponent<RectTransform>(resultEntity);
            if (correctCount <= 5)
            {
                resultTransform.Hidden = !(i == 1);
            }
            else if (correctCount <= 8)
            {
                resultTransform.Hidden = !(i == 2);
            }
            else
            {
                resultTransform.Hidden = !(i == 3);
            }
            SetComponent(resultEntity, resultTransform);
        }

        TextLayout.SetEntityTextRendererString(EntityManager, resultTextEntity, correctCount.ToString());

        //var restartEntity = uiSys.GetEntityByUIName("Restart");

        //var restartTransform = GetComponent<RectTransform>(restartEntity);
        //restartTransform.Hidden = true;
        //SetComponent(restartEntity, restartTransform);
    }

    private void ShowStart()
    {
        var uiSys = World.GetExistingSystem<ProcessUIEvents>();

        var questionsEntity = uiSys.GetEntityByUIName("Questions");
        var answersEntity = uiSys.GetEntityByUIName("Answers");
        var resultsEntity = uiSys.GetEntityByUIName("Results");
        var startEntity = uiSys.GetEntityByUIName("Start");
        var restartEntity = uiSys.GetEntityByUIName("Restart");

        var questionsTransform = GetComponent<RectTransform>(questionsEntity);
        questionsTransform.Hidden = true;
        SetComponent(questionsEntity, questionsTransform);
        var answersTransform = GetComponent<RectTransform>(answersEntity);
        answersTransform.Hidden = true;
        SetComponent(answersEntity, answersTransform);
        var resultsTransform = GetComponent<RectTransform>(resultsEntity);
        resultsTransform.Hidden = true;
        SetComponent(resultsEntity, resultsTransform);
        var startTransform = GetComponent<RectTransform>(startEntity);
        startTransform.Hidden = false;
        SetComponent(startEntity, startTransform);
        var restartTransform = GetComponent<RectTransform>(restartEntity);
        restartTransform.Hidden = true;
        SetComponent(restartEntity, restartTransform);

    }

    private void ShowThankyou()
    {
        var uiSys = World.GetExistingSystem<ProcessUIEvents>();

        var questionsEntity = uiSys.GetEntityByUIName("Questions");
        var answersEntity = uiSys.GetEntityByUIName("Answers");
        var resultsEntity = uiSys.GetEntityByUIName("Results");
        var startEntity = uiSys.GetEntityByUIName("Start");
        var restartEntity = uiSys.GetEntityByUIName("Restart");

        var questionsTransform = GetComponent<RectTransform>(questionsEntity);
        questionsTransform.Hidden = true;
        SetComponent(questionsEntity, questionsTransform);
        var answersTransform = GetComponent<RectTransform>(answersEntity);
        answersTransform.Hidden = true;
        SetComponent(answersEntity, answersTransform);
        var resultsTransform = GetComponent<RectTransform>(resultsEntity);
        resultsTransform.Hidden = true;
        SetComponent(resultsEntity, resultsTransform);
        var startTransform = GetComponent<RectTransform>(startEntity);
        startTransform.Hidden = true;
        SetComponent(startEntity, startTransform);
        var restartTransform = GetComponent<RectTransform>(restartEntity);
        restartTransform.Hidden = false;
        SetComponent(restartEntity, restartTransform);

    }
}
