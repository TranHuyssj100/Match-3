# Review the project’s organization    

 Tổ chức Folder Project lộn xộn, không đúng chức năng, cần tổ chức lại để dễ quản lý Textures , Data, Scripts, Plugins...

- Suggestion:

    Assets/
    ├── _Project/                        # Toàn bộ nội dung tự làm, tách khỏi asset bên thứ ba
    │   ├── Art/
    │   │   ├── Sprites/
    │   │   │   ├── Items/               # characters_0001..0007, rainbow_fish, fish_*
    │   │   │   ├── Bonuses/             # arrow_horiz, arrow_vert, bombColor
    │   │   │   ├── UI/                  # button*, panel*, music_on/off
    │   │   │   └── Environment/         # bg, piece_bg, stone_square
    │   │   └── Fonts/
    │   ├── Audio/
    │   ├── Prefabs/
    │   │   ├── Board/                   # cellBackground, itemNormal, itemBonus*
    │   │   └── UI/
    │   ├── ScriptableObjects/
    │   │   ├── GameSettings.asset
    │   │   └── ItemDataSO.asset
    │   ├── Scenes/
    │   │   ├── Splash.unity            
    │   │   └── Game.unity
    │   └── Scripts/
    │       ├── Core/                    # Bootstrap, GameManager, GameStateMachine, ServiceContainer
    │       ├── Gameplay/
    │       │   ├── Model/               # Board, Cell(data), Item, NormalItem, BonusItem, MatchFinder
    │       │   ├── View/                # BoardView, CellView, ItemView
    │       │   ├── Input/               # BoardInputHandler
    │       │   └── Conditions/          # LevelCondition, LevelMoves, LevelTime
    │       ├── UI/                      # IMenu, UIMainManager, UIPanel*, UIToggleSound
    │       ├── Data/                    # GameSettings, ItemDataSO, Constants
    │       ├── Infrastructure/
    │       │   ├── Pooling/             # PoolManager, GameObjectPool, PooledObject, IPoolable
    │       │   └── Utilities/           # Utils, Extensions
    │       ├── Editor/                  # MainToolMenu
    │       └── Tests/
    │           ├── EditMode/
    │           └── PlayMode/
    ├── Plugins/
    │   └── Demigiant/                   # DOTween chuyển về đây
    └── Resources/                       # Giữ tối thiểu, tiến tới xoá hẳn (tăng Performance lúc loading vào game )