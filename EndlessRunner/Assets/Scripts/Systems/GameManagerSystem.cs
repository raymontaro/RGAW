using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Tiny.Input;
using Unity.Tiny.Text;
using Unity.Tiny.UI;

public class GameManagerSystem : SystemBase
{
    public static GameManagerSystem Instance;

    public enum Gamestate { init, instructions, play, win, lose, restart };
    public Gamestate myGameState;

    Entity remindersEntity;
    DynamicBuffer<Reminders> remindersBuffer;

    float waitTimer = 0f;
    float timer = 60f;
    int score = 0;

    bool once = false;

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

        switch (myGameState)
        {
            case Gamestate.init:
                DisableAllReminders();
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
                    currentSpeed += Time.DeltaTime*0.2f;
                }
                
                LevelChunkMoveSystem.Instance.ChangeSpeed(currentSpeed);
                break;
            case Gamestate.win:
                if (waitTimer < 3f)
                {
                    waitTimer += Time.DeltaTime;
                }
                else
                {
                    waitTimer = 3f;

                    Random rand = new Random((uint)math.round(System.DateTime.Now.Millisecond) + 1);
                    int tipsIndex = 0;
                    for (int i = 0; i < System.DateTime.Now.Millisecond; i++)
                    {
                        tipsIndex = rand.NextInt(0, 8);
                    }

                    var tipsTransform = GetComponent<RectTransform>(remindersBuffer[tipsIndex].entity);
                    tipsTransform.Hidden = false;
                    SetComponent(remindersBuffer[tipsIndex].entity, tipsTransform);

                    myGameState = Gamestate.restart;
                }
                break;
            case Gamestate.lose:
                if (waitTimer < 3f)
                {
                    waitTimer += Time.DeltaTime;
                }
                else
                {
                    waitTimer = 3f;

                    Random rand = new Random((uint)math.round(System.DateTime.Now.Millisecond) + 1);
                    int tipsIndex = 0;
                    for (int i = 0; i < System.DateTime.Now.Millisecond; i++)
                    {
                        tipsIndex = rand.NextInt(0, 8);
                    }

                    var tipsTransform = GetComponent<RectTransform>(remindersBuffer[tipsIndex].entity);
                    tipsTransform.Hidden = false;
                    SetComponent(remindersBuffer[tipsIndex].entity, tipsTransform);

                    myGameState = Gamestate.restart;
                }
                break;
            case Gamestate.restart:
                if(waitTimer > 0f)
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
                    score = 0;
                    currentSpeed = initialSpeed;
                    LevelChunkMoveSystem.Instance.ChangeSpeed(currentSpeed);
                    LevelChunkSpawnSystem.Instance.RestartSpawn();
                    myGameState = Gamestate.init;
                }
                break;
        }
    }    

    private void SetUI()
    {
        var uiSys = World.GetExistingSystem<ProcessUIEvents>();

        var gameCompletedEntity = uiSys.GetEntityByUIName("GameCompletedPanel");
        var gameCompletedTransform = GetComponent<RectTransform>(gameCompletedEntity);
        gameCompletedTransform.Hidden = !(myGameState == Gamestate.lose || myGameState == Gamestate.restart);
        SetComponent(gameCompletedEntity, gameCompletedTransform);

        var timesUpEntity = uiSys.GetEntityByUIName("TimesUpPanel");
        var timesUpTransform = GetComponent<RectTransform>(timesUpEntity);
        timesUpTransform.Hidden = !(myGameState == Gamestate.win);
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
        TextLayout.SetEntityTextRendererString(EntityManager, scoreEntity, score.ToString());
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
        myGameState = Gamestate.lose;
    }

    public void AddScore(int value)
    {
        score += value;
    }
}
