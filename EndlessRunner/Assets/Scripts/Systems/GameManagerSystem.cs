using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Tiny.Input;
using Unity.Tiny.Text;
using Unity.Tiny.UI;

public class GameManagerSystem : SystemBase
{
    public static GameManagerSystem Instance;

    public enum Gamestate { init, instructions, play,tips, win, lose, restart,wait };
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

    protected override void OnStartRunning()
    {
        Instance = this;
    }

    protected override void OnCreate()
    {
        myGameState = Gamestate.init;
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

        var restartEntity = uiSys.GetEntityByUIName("Restart");
        var restartTextEntity = uiSys.GetEntityByUIName("RestartText");
        var timerEntity = uiSys.GetEntityByUIName("TimerText");

        if (showScore < score)
        {
            showScore += Time.DeltaTime * 200;
        }
        else
        {
            showScore = score;
        }

        switch (myGameState)
        {
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
                if (!openTips)
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
                    var restartTransform = GetComponent<RectTransform>(restartEntity);
                    restartTransform.Hidden = false;
                    SetComponent(restartEntity, restartTransform);

                    TextLayout.SetEntityTextRendererString(EntityManager, restartTextEntity, "Game will restart in " + math.ceil(waitTimer) + " seconds.");
                }
                else
                {
                    waitTimer = 0f;
                    timer = 60f;
                    score = 0f;
                    showScore = 0f;
                    currentSpeed = initialSpeed;
                    PlayerAnimationSystem.RestartSpeed();
                    LevelChunkMoveSystem.Instance.ChangeSpeed(currentSpeed);
                    LevelChunkSpawnSystem.Instance.RestartSpawn();
                    openTips = false;
                    myGameState = Gamestate.init;
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
        restartTransform.Hidden = !(myGameState == Gamestate.restart);
        SetComponent(restartEntity, restartTransform);

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
    }

    public void AddScore(float value)
    {
        score += value;
    }

    public void ShowTips()
    {
        myGameState = Gamestate.tips;
    }
}
