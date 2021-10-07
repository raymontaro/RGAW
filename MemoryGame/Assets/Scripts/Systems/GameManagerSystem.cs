using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Tiny.UI;
using Unity.Tiny.Input;
using Unity.Tiny.Text;

public class GameManagerSystem : SystemBase
{
    public static GameManagerSystem Instance;

    public enum Gamestate { loading,spawn,instructions,showCard,firstCard,secondCard,check,wrong,right,win,lose,showTips,end};

    public Gamestate myGameState;

    public Entity firstCardSelected;
    public Entity secondCardSelected;

    float checkWaitTimer = 0f;
    float restartWaitTimer = 0f;
    int rightCount = 0;

    public bool isWin = false;
    bool isPlay = false;

    bool isLoading = true;

    float gameDuration = 60f;
    float timer = 60f;

    bool uiIsSet = false;
    Entity tipsEntity;
    DynamicBuffer<Tips> tipsBuffer;    

    protected override void OnStartRunning()
    {
        Instance = this;
    }

    protected override void OnCreate()
    {        
        myGameState = Gamestate.spawn;
        //Entity e = EntityManager.CreateEntity();
        //EntityManager.AddComponentData(e, new GameState { Value = GameStates.None });
        tipsEntity = EntityManager.CreateEntity();            
    }

    protected override void OnUpdate()
    {
        var uiSys = World.GetExistingSystem<ProcessUIEvents>();
        //var gameState = GetSingleton<GameState>();
        //var winPanel = GetSingleton<WinPanel>();

        var input = World.GetExistingSystem<InputSystem>();

        
        //var loadingEntity = uiSys.GetEntityByUIName("Loading");
        var instructionEntity = uiSys.GetEntityByUIName("InstructionButton");
        var instructionButonEntity = uiSys.GetEntityByUIName("InstructionButton");
        if (instructionButonEntity == Entity.Null)
            return;
        var instructionButtonState = GetComponent<UIState>(instructionButonEntity);
        var winEntity = uiSys.GetEntityByUIName("Win");
        var loseEntity = uiSys.GetEntityByUIName("Lose");
        
        
        var restartEntity = uiSys.GetEntityByUIName("Restart");

        tipsBuffer = EntityManager.AddBuffer<Tips>(tipsEntity);
        for (int i = 1; i <= 9; i++)
        {
            tipsBuffer.Add(new Tips { entity = uiSys.GetEntityByUIName("Tips (" + i + ")") });
        }

        if (!uiIsSet)
        {
            uiIsSet = true;

            var winTransform = GetComponent<RectTransform>(winEntity);
            winTransform.Hidden = true;
            SetComponent(winEntity, winTransform);

            var loseTransform = GetComponent<RectTransform>(loseEntity);
            loseTransform.Hidden = true;
            SetComponent(loseEntity, loseTransform);

            RectTransform tipsTransform;

            for(int i = 0; i < tipsBuffer.Length; i++)
            {
                tipsTransform = GetComponent<RectTransform>(tipsBuffer[i].entity);
                tipsTransform.Hidden = true;
                SetComponent(tipsBuffer[i].entity, tipsTransform);
            }

            var restartTransform = GetComponent<RectTransform>(restartEntity);
            restartTransform.Hidden = true;
            SetComponent(restartEntity, restartTransform);

            
        }

        var timerEntity = GetSingletonEntity<TimerText>();
        var restartTextEntity = uiSys.GetEntityByUIName("RestartText");        

        //if (input.GetKeyDown(KeyCode.Space))
        //{
        //    isWin = !isWin;
        //    if (isWin)
        //        gameState.Value = GameStates.Win;
        //    else
        //        gameState.Value = GameStates.None;

        //    SetSingleton(gameState);
        //}

        //if (isWin)
        //{
        //    EntityManager.SetEnabled(winPanel.entity, true);
        //}
        //else
        //{
        //    EntityManager.SetEnabled(winPanel.entity, false);
        //}

        if (isPlay)
        {
            TextLayout.SetEntityTextRendererString(EntityManager, timerEntity, math.ceil(timer).ToString());

            if (timer > 0)
            {
                timer -= Time.DeltaTime;
            }
            else
            {
                myGameState = Gamestate.lose;
            }
        }

        //if (isLoading)
        //{
        //    var isReady = false;
        //    Entities.WithAll<Image2DLoadFromFile>().ForEach((Entity e, ref Image2D img) =>
        //    {
        //        if (img.status == ImageStatus.LoadError)
        //            Debug.Log("Error loading images");

        //        isReady = true;
        //    }).WithStructuralChanges().Run();

        //    if (isReady)
        //    {
        //        isLoading = false;
        //        myGameState = Gamestate.spawn;

        //        var loadingTransform = GetComponent<RectTransform>(loadingEntity);
        //        loadingTransform.Hidden = !isLoading;
        //        SetComponent(loadingEntity, loadingTransform);
        //    }
        //}

        if (instructionButtonState.IsClicked)
        {            
            var instructionTransform = GetComponent<RectTransform>(instructionEntity);
            instructionTransform.Hidden = true;
            SetComponent(instructionEntity, instructionTransform);
            myGameState = Gamestate.showCard;            
        }

        switch (myGameState)
        {
            case Gamestate.spawn:
                SpawnCardSystem.Instance.Spawn();
                myGameState = Gamestate.instructions;
                break;
            case Gamestate.showCard:
                if (restartWaitTimer < 3f)
                {
                    restartWaitTimer += Time.DeltaTime;

                    Entities.ForEach((ref CardEntityComponent cardEntityComponent) =>
                    {
                        EntityManager.SetComponentData(cardEntityComponent.entity, new Rotation { Value = quaternion.EulerXYZ(math.radians(0), math.radians(180), math.radians(0)) });
                    }).WithoutBurst().Run();
                    //var currentCardsBuffer = EntityManager.GetBuffer<Card>(SpawnCardSystem.Instance.currentCardsEntity);
                    //for(int i = 0; i < currentCardsBuffer.Length; i++)
                    //{
                    //    EntityManager.SetComponentData(currentCardsBuffer[i].entity, new Rotation { Value = quaternion.EulerXYZ(math.radians(0), math.radians(180), math.radians(0)) });
                    //}                    
                }
                else
                {
                    restartWaitTimer = 0f;

                    Entities.ForEach((ref CardEntityComponent cardEntityComponent) =>
                    {
                        EntityManager.SetComponentData(cardEntityComponent.entity, new Rotation { Value = quaternion.identity });
                    }).WithoutBurst().Run();
                    //var currentCardsBuffer = EntityManager.GetBuffer<Card>(SpawnCardSystem.Instance.currentCardsEntity);
                    //for (int i = 0; i < currentCardsBuffer.Length; i++)
                    //{
                    //    EntityManager.SetComponentData(currentCardsBuffer[i].entity, new Rotation { Value = quaternion.identity });
                    //}

                    myGameState = Gamestate.firstCard;
                    isPlay = true;
                }                
                break;
            case Gamestate.firstCard:
                if(firstCardSelected != Entity.Null)
                {                    
                    EntityManager.SetComponentData(firstCardSelected, new Rotation { Value = quaternion.EulerXYZ(math.radians(0),math.radians(180),math.radians(0)) });                    
                    myGameState = Gamestate.secondCard;
                }
                break;
            case Gamestate.secondCard:
                if (secondCardSelected != Entity.Null)
                {
                    EntityManager.SetComponentData(secondCardSelected, new Rotation { Value = quaternion.EulerXYZ(math.radians(0), math.radians(180), math.radians(0)) });
                    myGameState = Gamestate.check;
                }
                break;
            case Gamestate.check:
                
                int firstId = EntityManager.GetComponentData<CardEntityComponent>(firstCardSelected).id;
                int secondId = EntityManager.GetComponentData<CardEntityComponent>(secondCardSelected).id;
                if (checkWaitTimer < 0.8f && firstId != secondId)
                {
                    checkWaitTimer += Time.DeltaTime;
                }
                else
                {
                    checkWaitTimer = 0f;

                    if (firstId == secondId)
                        myGameState = Gamestate.right;
                    else
                    {
                        myGameState = Gamestate.wrong;
                        EntityManager.SetComponentData(firstCardSelected, new CardEntityComponent { entity = firstCardSelected, id = firstId, isSelected = false });
                        EntityManager.SetComponentData(secondCardSelected, new CardEntityComponent { entity = secondCardSelected, id = secondId, isSelected = false });
                    }
                }
                break;
            case Gamestate.wrong:
                EntityManager.SetComponentData(firstCardSelected, new Rotation { Value = quaternion.identity });
                EntityManager.SetComponentData(secondCardSelected, new Rotation { Value = quaternion.identity });                

                firstCardSelected = Entity.Null;
                secondCardSelected = Entity.Null;

                myGameState = Gamestate.firstCard;
                break;
            case Gamestate.right:
                rightCount++;
                if (rightCount >= 9)
                    myGameState = Gamestate.win;
                else
                {                    
                    firstCardSelected = Entity.Null;
                    secondCardSelected = Entity.Null;

                    myGameState = Gamestate.firstCard;
                }

                break;
            case Gamestate.win:                
                isWin = true;
                isPlay = false;
                //gameState.Value = GameStates.Win;
                //SetSingleton(gameState);
                var winTransform = GetComponent<RectTransform>(winEntity);
                winTransform.Hidden = false;
                SetComponent(winEntity, winTransform);

                timer = 60f;

                if (restartWaitTimer < 2f)
                {
                    restartWaitTimer += Time.DeltaTime;
                }
                else
                {
                    restartWaitTimer = 0f;

                    //firstCardSelected = Entity.Null;
                    //secondCardSelected = Entity.Null;
                    //rightCount = 0;

                    myGameState = Gamestate.showTips;
                }
                break;
            case Gamestate.lose:
                isWin = false;
                isPlay = false;
                //gameState.Value = GameStates.Win;
                //SetSingleton(gameState);
                var loseTransform = GetComponent<RectTransform>(loseEntity);
                loseTransform.Hidden = false;
                SetComponent(loseEntity, loseTransform);

                timer = 60f;

                if (restartWaitTimer < 2f)
                {
                    restartWaitTimer += Time.DeltaTime;
                }
                else
                {
                    restartWaitTimer = 0f;

                    //firstCardSelected = Entity.Null;
                    //secondCardSelected = Entity.Null;
                    //rightCount = 0;

                    myGameState = Gamestate.showTips;
                }
                break;
            case Gamestate.showTips:
                var winTransform2 = GetComponent<RectTransform>(winEntity);
                winTransform2.Hidden = false;
                SetComponent(winEntity, winTransform2);

                var loseTransform2 = GetComponent<RectTransform>(loseEntity);
                loseTransform2.Hidden = false;
                SetComponent(loseEntity, loseTransform2);                

                Random rand = new Random((uint)math.round(System.DateTime.Now.Millisecond) + 1);
                int tipsIndex = 0;
                for(int i = 0; i < System.DateTime.Now.Millisecond; i++)
                {
                    tipsIndex = rand.NextInt(0, 9);
                }
                //Debug.Log(tipsIndex);

                var tipsTransform = GetComponent<RectTransform>(tipsBuffer[tipsIndex].entity);
                tipsTransform.Hidden = false;
                SetComponent(tipsBuffer[tipsIndex].entity, tipsTransform);

                restartWaitTimer = 6f;

                myGameState = Gamestate.end;

                break;
            case Gamestate.end:
                if (restartWaitTimer > 0f)
                {
                    restartWaitTimer -= Time.DeltaTime;
                }
                
                if(restartWaitTimer <= 3)
                {
                    if (restartWaitTimer > 0)
                    {
                        var restartTransform2 = GetComponent<RectTransform>(restartEntity);
                        restartTransform2.Hidden = false;
                        SetComponent(restartEntity, restartTransform2);
                        
                        TextLayout.SetEntityTextRendererString(EntityManager, restartTextEntity, "Game will restart in "+math.ceil(restartWaitTimer)+" seconds.");
                    }
                    else
                    {
                        restartWaitTimer = 0f;

                        firstCardSelected = Entity.Null;
                        secondCardSelected = Entity.Null;
                        rightCount = 0;

                        var winTransform3 = GetComponent<RectTransform>(winEntity);
                        winTransform3.Hidden = true;
                        SetComponent(winEntity, winTransform3);

                        var loseTransform3 = GetComponent<RectTransform>(loseEntity);
                        loseTransform3.Hidden = true;
                        SetComponent(loseEntity, loseTransform3);

                        RectTransform tipsTransform2;

                        for (int i = 0; i < tipsBuffer.Length; i++)
                        {
                            tipsTransform2 = GetComponent<RectTransform>(tipsBuffer[i].entity);
                            tipsTransform2.Hidden = true;
                            SetComponent(tipsBuffer[i].entity, tipsTransform2);
                        }

                        var instructionTransform = GetComponent<RectTransform>(instructionEntity);
                        instructionTransform.Hidden = false;
                        SetComponent(instructionEntity, instructionTransform);

                        var restartTransform2 = GetComponent<RectTransform>(restartEntity);
                        restartTransform2.Hidden = true;
                        SetComponent(restartEntity, restartTransform2);

                        TextLayout.SetEntityTextRendererString(EntityManager, timerEntity, math.ceil(timer).ToString());

                        myGameState = Gamestate.spawn;
                    }
                }
                                
                break;
        }        
    }


    public void SetGameState(Gamestate state)
    {
        myGameState = state;
    }

    public void SetFirstCardSelected(Entity e)
    {
        firstCardSelected = e;
    }

    public void SetSecondCardSelected(Entity e)
    {
        secondCardSelected = e;
    }
}
