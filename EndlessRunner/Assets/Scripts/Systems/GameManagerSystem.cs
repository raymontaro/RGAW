using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Tiny.Input;
using Unity.Tiny.Text;
using Unity.Tiny.UI;

public class GameManagerSystem : SystemBase
{
    public static GameManagerSystem Instance;

    public enum Gamestate {start, init, instructions, play,tips, win, lose, restart,wait,thankyou };
    public Gamestate myGameState;

    Entity remindersEntity;
    DynamicBuffer<Reminders> remindersBuffer;    

    float waitTimer = 0f;
    float timer = 60f;
    float score = 0f;
    float showScore = 0f;

    bool once = false;
    bool openTips = false;
    bool wallHit = false;

    float initialSpeed = 5f;
    float maxSpeed = 15f;
    float currentSpeed = 5f;

    bool initSound = false;

    protected override void OnStartRunning()
    {
        Instance = this;
    }

    protected override void OnCreate()
    {
        myGameState = Gamestate.start;
        remindersEntity = EntityManager.CreateEntity();
    }

    protected override void OnUpdate()
    {
        if (!LoadingSystem.isLevelLoaded)
            return;

        var uiSys = World.GetExistingSystem<ProcessUIEvents>();
        var input = World.GetExistingSystem<InputSystem>();

        if (!once)
        {
            once = true;

            remindersBuffer = EntityManager.AddBuffer<Reminders>(remindersEntity);
            for (int i = 1; i <= 9; i++)
            {
                remindersBuffer.Add(new Reminders { entity = uiSys.GetEntityByUIName("Reminder (" + i + ")") });
            }
        }

        SetUI();

        var startButonEntity = uiSys.GetEntityByUIName("StartButton");
        var startButtonState = GetComponent<UIState>(startButonEntity);
        var restartButonEntity = uiSys.GetEntityByUIName("RestartButton");
        var restartButtonState = GetComponent<UIState>(restartButonEntity);
        //var restartEntity = uiSys.GetEntityByUIName("Restart");
        //var restartTextEntity = uiSys.GetEntityByUIName("RestartText");
        var timerEntity = uiSys.GetEntityByUIName("TimerText");

        if (showScore < score-5)
        {
            showScore += Time.DeltaTime * 200;
        }
        else if(showScore > score+5)
        {
            showScore -= Time.DeltaTime * 200;
        }
        else
        {
            showScore = score;
        }

        switch (myGameState)
        {
            case Gamestate.start:
                DisableAllReminders();
                if (startButtonState.IsClicked)
                {
                    if (!initSound)
                    {
                        initSound = true;
                        AudioUtils.PlaySound(EntityManager, AudioTypes.Coin);
                        AudioUtils.PlaySound(EntityManager, AudioTypes.Reminder);
                        AudioUtils.PlaySound(EntityManager, AudioTypes.Wall);
                    }
                    myGameState = Gamestate.init;
                }
                break;
            case Gamestate.init:
                DisableAllReminders();
                TextLayout.SetEntityTextRendererString(EntityManager, timerEntity, math.ceil(timer).ToString());
                //LevelChunkSpawnSystem.Instance.ClearLevel();
                //LevelChunkSpawnSystem.Instance.InitLevel();
                myGameState = Gamestate.instructions;
                break;
            case Gamestate.instructions:                
                if (input.GetMouseButton(0))
                {
                    myGameState = Gamestate.play;
                }
                break;
            case Gamestate.play:
                TextLayout.SetEntityTextRendererString(EntityManager, timerEntity, math.ceil(timer).ToString());

                if (timer > 0)
                {
                    timer -= Time.DeltaTime;
                }
                else
                {
                    myGameState = Gamestate.win;
                }

                if (currentSpeed <= maxSpeed)
                {
                    currentSpeed += Time.DeltaTime * 0.2f;
                }

                LevelChunkMoveSystem.Instance.ChangeSpeed(currentSpeed);
                break;
            case Gamestate.tips:                
                if (!openTips && !wallHit)
                {
                    openTips = true;

                    Random rand = new Random((uint)math.round(System.DateTime.Now.Millisecond) + 1);
                    int tipsIndex = 0;
                    for (int i = 0; i < System.DateTime.Now.Millisecond; i++)
                    {
                        tipsIndex = rand.NextInt(0, 8);
                    }

                    var tipsTransform = GetComponent<RectTransform>(remindersBuffer[tipsIndex].entity);
                    tipsTransform.Hidden = false;
                    SetComponent(remindersBuffer[tipsIndex].entity, tipsTransform);
                }

                if (waitTimer < 2f)
                {
                    waitTimer += Time.DeltaTime;
                }
                else
                {
                    waitTimer = 0f;

                    openTips = false;

                    DisableAllReminders();

                    if (wallHit)
                    {
                        wallHit = false;
                        currentSpeed = initialSpeed;
                        LevelChunkMoveSystem.Instance.ChangeSpeed(currentSpeed);
                        MoveBySwipeSystem.Instance.Respawn();
                        PlayerAnimationSystem.RestartSpeed();
                        waitTimer = 0.5f;
                        myGameState = Gamestate.wait;
                    }
                    else
                    {
                        myGameState = Gamestate.play;
                    }
                }
                break;
            case Gamestate.win:
                if (waitTimer < 3f)
                {
                    waitTimer += Time.DeltaTime;
                }
                else
                {
                    waitTimer = 3f;

                    //if (!openTips)
                    //{
                    //    openTips = true;

                    //    Random rand = new Random((uint)math.round(System.DateTime.Now.Millisecond) + 1);
                    //    int tipsIndex = 0;
                    //    for (int i = 0; i < System.DateTime.Now.Millisecond; i++)
                    //    {
                    //        tipsIndex = rand.NextInt(0, 8);
                    //    }

                    //    var tipsTransform = GetComponent<RectTransform>(remindersBuffer[tipsIndex].entity);
                    //    tipsTransform.Hidden = false;
                    //    SetComponent(remindersBuffer[tipsIndex].entity, tipsTransform);
                    //}

                    myGameState = Gamestate.restart;
                }
                break;
            case Gamestate.lose:
                //if (!openTips)
                //{
                //    openTips = true;

                //    Random rand = new Random((uint)math.round(System.DateTime.Now.Millisecond) + 1);
                //    int tipsIndex = 0;
                //    for (int i = 0; i < System.DateTime.Now.Millisecond; i++)
                //    {
                //        tipsIndex = rand.NextInt(0, 8);
                //    }

                //    var tipsTransform = GetComponent<RectTransform>(remindersBuffer[tipsIndex].entity);
                //    tipsTransform.Hidden = false;
                //    SetComponent(remindersBuffer[tipsIndex].entity, tipsTransform);
                //}

                if (waitTimer < 3f)
                {
                    waitTimer += Time.DeltaTime;
                }
                else
                {
                    waitTimer = 3f;


                    myGameState = Gamestate.restart;
                }
                break;
            case Gamestate.restart:
                if (waitTimer > 0f)
                {
                    waitTimer -= Time.DeltaTime;
                }

                if (waitTimer > 0)
                {
                    //var restartTransform = GetComponent<RectTransform>(restartEntity);
                    //restartTransform.Hidden = false;
                    //SetComponent(restartEntity, restartTransform);

                    //TextLayout.SetEntityTextRendererString(EntityManager, restartTextEntity, "Game will restart in " + math.ceil(waitTimer) + " seconds.");
                }
                else
                {
                    waitTimer = 0f;                    
                    myGameState = Gamestate.thankyou;
                }
                break;
            case Gamestate.wait:
                if (waitTimer > 0f)
                {
                    waitTimer -= Time.DeltaTime;
                }
                else
                {
                    waitTimer = 0f;
                    myGameState = Gamestate.play;
                }
                break;
            case Gamestate.thankyou:
                if (restartButtonState.IsClicked)
                {
                    timer = 60f;
                    score = 0f;
                    showScore = 0f;
                    currentSpeed = initialSpeed;
                    PlayerAnimationSystem.RestartSpeed();
                    LevelChunkMoveSystem.Instance.ChangeSpeed(currentSpeed);
                    LevelChunkSpawnSystem.Instance.RestartSpawn();
                    openTips = false;
                    myGameState = Gamestate.start;
                }                
                break;
        }
    }

    private void SetUI()
    {
        var uiSys = World.GetExistingSystem<ProcessUIEvents>();

        var gameCompletedEntity = uiSys.GetEntityByUIName("GameCompletedPanel");
        var gameCompletedTransform = GetComponent<RectTransform>(gameCompletedEntity);
        gameCompletedTransform.Hidden = !/*(myGameState == Gamestate.lose || myGameState == Gamestate.restart)*/(myGameState == Gamestate.win || myGameState == Gamestate.lose || myGameState == Gamestate.restart);
        SetComponent(gameCompletedEntity, gameCompletedTransform);

        var timesUpEntity = uiSys.GetEntityByUIName("TimesUpPanel");
        var timesUpTransform = GetComponent<RectTransform>(timesUpEntity);
        timesUpTransform.Hidden = /*!(myGameState == Gamestate.win)*/true;
        SetComponent(timesUpEntity, timesUpTransform);

        var instructionEntity = uiSys.GetEntityByUIName("InstructionsPanel");        
        var instructionTransform = GetComponent<RectTransform>(instructionEntity);
        instructionTransform.Hidden = !(myGameState == Gamestate.instructions);
        SetComponent(instructionEntity, instructionTransform);

        var restartEntity = uiSys.GetEntityByUIName("Restart");
        var restartTransform = GetComponent<RectTransform>(restartEntity);
        restartTransform.Hidden = !(myGameState == Gamestate.thankyou);
        SetComponent(restartEntity, restartTransform);

        var startEntity = uiSys.GetEntityByUIName("Start");
        var startTransform = GetComponent<RectTransform>(startEntity);
        startTransform.Hidden = !(myGameState == Gamestate.start);
        SetComponent(startEntity, startTransform);

        var TimerPanelEntity = uiSys.GetEntityByUIName("TimerPanel");
        var TimerPanelTransform = GetComponent<RectTransform>(TimerPanelEntity);
        TimerPanelTransform.Hidden = (myGameState == Gamestate.start || myGameState == Gamestate.thankyou);
        SetComponent(TimerPanelEntity, TimerPanelTransform);

        var ScorePanelEntity = uiSys.GetEntityByUIName("ScorePanel");
        var ScorePanelTransform = GetComponent<RectTransform>(ScorePanelEntity);
        ScorePanelTransform.Hidden = (myGameState == Gamestate.start || myGameState == Gamestate.thankyou);
        SetComponent(ScorePanelEntity, ScorePanelTransform);

        var scoreEntity = uiSys.GetEntityByUIName("ScoreText");
        TextLayout.SetEntityTextRendererString(EntityManager, scoreEntity, math.ceil(showScore).ToString());
    }

    private void DisableAllReminders()
    {
        var uiSys = World.GetExistingSystem<ProcessUIEvents>();        

        RectTransform remindersTransform;

        for (int i = 0; i < remindersBuffer.Length; i++)
        {
            remindersTransform = GetComponent<RectTransform>(remindersBuffer[i].entity);
            remindersTransform.Hidden = true;
            SetComponent(remindersBuffer[i].entity, remindersTransform);
        }
    }

    public void HitObstacle()
    {
        wallHit = true;
        myGameState = Gamestate.tips;
        ReduceScore(50);
    }

    public void AddScore(float value)
    {
        score += value;
    }

    public void ReduceScore(float value)
    {
        if ((score - value) < 0)
        {
            score = 0;
        }
        else
        {
            score -= value;
        }
    }

    public void ShowTips()
    {
        myGameState = Gamestate.tips;
    }
}
