// ReSharper disable All

using Lumina.Text;
using Lumina.Data;
using Lumina.Data.Structs.Excel;
#pragma warning disable CS8618

namespace Lumina.Excel.GeneratedSheets
{
    [Sheet( "Quest", columnHash: 0x65316a61 )]
    public partial class CustomQuestSheet : ExcelRow
    {
        
        public SeString Name { get; set; }
        public SeString Id { get; set; }
        public LazyRow< ExVersion > Expansion { get; set; }
        public LazyRow< ClassJobCategory > ClassJobCategory0 { get; set; }
        public ushort ClassJobLevel0 { get; set; }
        public byte QuestLevelOffset { get; set; }
        public LazyRow< ClassJobCategory > ClassJobCategory1 { get; set; }
        public ushort ClassJobLevel1 { get; set; }
        public byte PreviousQuestJoin { get; set; }
        public LazyRow< Quest > PreviousQuest0 { get; set; }
        public byte Unknown10 { get; set; }
        public LazyRow< Quest > PreviousQuest1 { get; set; }
        public LazyRow< Quest > PreviousQuest2 { get; set; }
        public byte QuestLockJoin { get; set; }
        public LazyRow< Quest >[] QuestLock { get; set; }
        public ushort Header { get; set; }
        public byte Unknown17 { get; set; }
        public byte Unknown18 { get; set; }
        public LazyRow< ClassJob > ClassJobUnlock { get; set; }
        public LazyRow< GrandCompany > GrandCompany { get; set; }
        public LazyRow< GrandCompanyRank > GrandCompanyRank { get; set; }
        public byte InstanceContentJoin { get; set; }
        public LazyRow< InstanceContent >[] InstanceContent { get; set; }
        public LazyRow< Festival > Festival { get; set; }
        public byte FestivalBegin { get; set; }
        public byte FestivalEnd { get; set; }
        public ushort BellStart { get; set; }
        public ushort BellEnd { get; set; }
        public LazyRow< BeastTribe > BeastTribe { get; set; }
        public LazyRow< BeastReputationRank > BeastReputationRank { get; set; }
        public ushort BeastReputationValue { get; set; }
        public LazyRow< SatisfactionNpc > SatisfactionNpc { get; set; }
        public byte SatisfactionLevel { get; set; }
        public LazyRow< Mount > MountRequired { get; set; }
        public bool IsHouseRequired { get; set; }
        public LazyRow< DeliveryQuest > DeliveryQuest { get; set; }
        public uint IssuerStart { get; set; }
        public LazyRow< Level > IssuerLocation { get; set; }
        public LazyRow< Behavior > ClientBehavior { get; set; }
        public uint TargetEnd { get; set; }
        public bool IsRepeatable { get; set; }
        public byte RepeatIntervalType { get; set; }
        public LazyRow< QuestRepeatFlag > QuestRepeatFlag { get; set; }
        public bool CanCancel { get; set; }
        public byte Type { get; set; }
        public LazyRow< QuestClassJobSupply > QuestClassJobSupply { get; set; }
        public SeString[] ScriptInstruction { get; set; }
        public uint[] ScriptArg { get; set; }
        public byte[] ActorSpawnSeq { get; set; }
        public byte[] ActorDespawnSeq { get; set; }
        public uint[] Listener { get; set; }
        public byte[] QuestUInt8A { get; set; }
        public byte[] QuestUInt8B { get; set; }
        public byte[] ConditionType { get; set; }
        public uint[] ConditionValue { get; set; }
        public byte[] ConditionOperator { get; set; }
        public ushort[] Behavior { get; set; }
        public bool[] VisibleBool { get; set; }
        public bool[] ConditionBool { get; set; }
        public bool[] ItemBool { get; set; }
        public bool[] AnnounceBool { get; set; }
        public bool[] BehaviorBool { get; set; }
        public bool[] AcceptBool { get; set; }
        public bool[] QualifiedBool { get; set; }
        public bool[] CanTargetBool { get; set; }
        public byte[] ToDoCompleteSeq { get; set; }
        public byte[] ToDoQty { get; set; }
        public LazyRow< Level >[] ToDoMainLocation { get; set; }
        public LazyRow<Level> [,] ToDoChildLocation { get; set; }
        public byte[] CountableNum { get; set; }
        public byte LevelMax { get; set; }
        public LazyRow< ClassJob > ClassJobRequired { get; set; }
        public LazyRow< QuestRewardOther > QuestRewardOtherDisplay { get; set; }
        public ushort ExpFactor { get; set; }
        public uint GilReward { get; set; }
        public LazyRow< Item > CurrencyReward { get; set; }
        public uint CurrencyRewardCount { get; set; }
        public LazyRow< Item >[] ItemCatalyst { get; set; }
        public byte[] ItemCountCatalyst { get; set; }
        public byte ItemRewardType { get; set; }
        public uint[] ItemReward { get; set; }
        public byte[] ItemCountReward { get; set; }
        public bool Unknown1465 { get; set; }
        public bool Unknown1466 { get; set; }
        public bool Unknown1467 { get; set; }
        public bool Unknown1468 { get; set; }
        public bool Unknown1469 { get; set; }
        public bool Unknown1470 { get; set; }
        public bool Unknown1471 { get; set; }
        public LazyRow< Stain >[] StainReward { get; set; }
        public LazyRow< Item >[] OptionalItemReward { get; set; }
        public byte[] OptionalItemCountReward { get; set; }
        public bool[] OptionalItemIsHQReward { get; set; }
        public LazyRow< Stain >[] OptionalItemStainReward { get; set; }
        public LazyRow< Emote > EmoteReward { get; set; }
        public LazyRow< Action > ActionReward { get; set; }
        public LazyRow< GeneralAction >[] GeneralActionReward { get; set; }
        public ushort SystemReward0 { get; set; }
        public LazyRow< QuestRewardOther > OtherReward { get; set; }
        public ushort SystemReward1 { get; set; }
        public ushort GCTypeReward { get; set; }
        public LazyRow< InstanceContent > InstanceContentUnlock { get; set; }
        public byte Tomestone { get; set; }
        public byte TomestoneReward { get; set; }
        public byte TomestoneCountReward { get; set; }
        public byte ReputationReward { get; set; }
        public LazyRow< PlaceName > PlaceName { get; set; }
        public LazyRow< JournalGenre > JournalGenre { get; set; }
        public byte Unknown1514 { get; set; }
        public uint Icon { get; set; }
        public uint IconSpecial { get; set; }
        public bool Introduction { get; set; }
        public bool HideOfferIcon { get; set; }
        public LazyRow< EventIconType > EventIconType { get; set; }
        public byte Unknown1520 { get; set; }
        public ushort SortKey { get; set; }
        public bool Unknown1522 { get; set; }
        
        public override void PopulateData( RowParser parser, GameData gameData, Language language )
        {
            base.PopulateData( parser, gameData, language );

            Name = parser.ReadColumn< SeString >( 0 )!;
            Id = parser.ReadColumn< SeString >( 1 )!;
            Expansion = new LazyRow< ExVersion >( gameData, parser.ReadColumn< byte >( 2 ), language );
            ClassJobCategory0 = new LazyRow< ClassJobCategory >( gameData, parser.ReadColumn< byte >( 3 ), language );
            ClassJobLevel0 = parser.ReadColumn< ushort >( 4 );
            QuestLevelOffset = parser.ReadColumn< byte >( 5 );
            ClassJobCategory1 = new LazyRow< ClassJobCategory >( gameData, parser.ReadColumn< byte >( 6 ), language );
            ClassJobLevel1 = parser.ReadColumn< ushort >( 7 );
            PreviousQuestJoin = parser.ReadColumn< byte >( 8 );
            PreviousQuest0 = new LazyRow< Quest >( gameData, parser.ReadColumn< uint >( 9 ), language );
            Unknown10 = parser.ReadColumn< byte >( 10 );
            PreviousQuest1 = new LazyRow< Quest >( gameData, parser.ReadColumn< uint >( 11 ), language );
            PreviousQuest2 = new LazyRow< Quest >( gameData, parser.ReadColumn< uint >( 12 ), language );
            QuestLockJoin = parser.ReadColumn< byte >( 13 );
            QuestLock = new LazyRow< Quest >[ 2 ];
            for( var i = 0; i < 2; i++ )
                QuestLock[ i ] = new LazyRow< Quest >( gameData, parser.ReadColumn< uint >( 14 + i ), language );
            Header = parser.ReadColumn< ushort >( 16 );
            Unknown17 = parser.ReadColumn< byte >( 17 );
            Unknown18 = parser.ReadColumn< byte >( 18 );
            ClassJobUnlock = new LazyRow< ClassJob >( gameData, parser.ReadColumn< byte >( 19 ), language );
            GrandCompany = new LazyRow< GrandCompany >( gameData, parser.ReadColumn< byte >( 20 ), language );
            GrandCompanyRank = new LazyRow< GrandCompanyRank >( gameData, parser.ReadColumn< byte >( 21 ), language );
            InstanceContentJoin = parser.ReadColumn< byte >( 22 );
            InstanceContent = new LazyRow< InstanceContent >[ 3 ];
            for( var i = 0; i < 3; i++ )
                InstanceContent[ i ] = new LazyRow< InstanceContent >( gameData, parser.ReadColumn< uint >( 23 + i ), language );
            Festival = new LazyRow< Festival >( gameData, parser.ReadColumn< byte >( 26 ), language );
            FestivalBegin = parser.ReadColumn< byte >( 27 );
            FestivalEnd = parser.ReadColumn< byte >( 28 );
            BellStart = parser.ReadColumn< ushort >( 29 );
            BellEnd = parser.ReadColumn< ushort >( 30 );
            BeastTribe = new LazyRow< BeastTribe >( gameData, parser.ReadColumn< byte >( 31 ), language );
            BeastReputationRank = new LazyRow< BeastReputationRank >( gameData, parser.ReadColumn< byte >( 32 ), language );
            BeastReputationValue = parser.ReadColumn< ushort >( 33 );
            SatisfactionNpc = new LazyRow< SatisfactionNpc >( gameData, parser.ReadColumn< byte >( 34 ), language );
            SatisfactionLevel = parser.ReadColumn< byte >( 35 );
            MountRequired = new LazyRow< Mount >( gameData, parser.ReadColumn< int >( 36 ), language );
            IsHouseRequired = parser.ReadColumn< bool >( 37 );
            DeliveryQuest = new LazyRow< DeliveryQuest >( gameData, parser.ReadColumn< byte >( 38 ), language );
            IssuerStart = parser.ReadColumn< uint >( 39 );
            IssuerLocation = new LazyRow< Level >( gameData, parser.ReadColumn< uint >( 40 ), language );
            ClientBehavior = new LazyRow< Behavior >( gameData, parser.ReadColumn< ushort >( 41 ), language );
            TargetEnd = parser.ReadColumn< uint >( 42 );
            IsRepeatable = parser.ReadColumn< bool >( 43 );
            RepeatIntervalType = parser.ReadColumn< byte >( 44 );
            QuestRepeatFlag = new LazyRow< QuestRepeatFlag >( gameData, parser.ReadColumn< byte >( 45 ), language );
            CanCancel = parser.ReadColumn< bool >( 46 );
            Type = parser.ReadColumn< byte >( 47 );
            QuestClassJobSupply = new LazyRow< QuestClassJobSupply >( gameData, parser.ReadColumn< ushort >( 48 ), language );
            ScriptInstruction = new SeString[ 50 ];
            for( var i = 0; i < 50; i++ )
                ScriptInstruction[ i ] = parser.ReadColumn< SeString >( 49 + i )!;
            ScriptArg = new uint[ 50 ];
            for( var i = 0; i < 50; i++ )
                ScriptArg[ i ] = parser.ReadColumn< uint >( 99 + i );
            ActorSpawnSeq = new byte[ 64 ];
            for( var i = 0; i < 64; i++ )
                ActorSpawnSeq[ i ] = parser.ReadColumn< byte >( 149 + i );
            ActorDespawnSeq = new byte[ 64 ];
            for( var i = 0; i < 64; i++ )
                ActorDespawnSeq[ i ] = parser.ReadColumn< byte >( 213 + i );
            Listener = new uint[ 64 ];
            for( var i = 0; i < 64; i++ )
                Listener[ i ] = parser.ReadColumn< uint >( 277 + i );
            QuestUInt8A = new byte[ 32 ];
            for( var i = 0; i < 32; i++ )
                QuestUInt8A[ i ] = parser.ReadColumn< byte >( 341 + i );
            QuestUInt8B = new byte[ 32 ];
            for( var i = 0; i < 32; i++ )
                QuestUInt8B[ i ] = parser.ReadColumn< byte >( 373 + i );
            ConditionType = new byte[ 64 ];
            for( var i = 0; i < 64; i++ )
                ConditionType[ i ] = parser.ReadColumn< byte >( 405 + i );
            ConditionValue = new uint[ 64 ];
            for( var i = 0; i < 64; i++ )
                ConditionValue[ i ] = parser.ReadColumn< uint >( 469 + i );
            ConditionOperator = new byte[ 64 ];
            for( var i = 0; i < 64; i++ )
                ConditionOperator[ i ] = parser.ReadColumn< byte >( 533 + i );
            Behavior = new ushort[ 64 ];
            for( var i = 0; i < 64; i++ )
                Behavior[ i ] = parser.ReadColumn< ushort >( 597 + i );
            VisibleBool = new bool[ 64 ];
            for( var i = 0; i < 64; i++ )
                VisibleBool[ i ] = parser.ReadColumn< bool >( 661 + i );
            ConditionBool = new bool[ 64 ];
            for( var i = 0; i < 64; i++ )
                ConditionBool[ i ] = parser.ReadColumn< bool >( 725 + i );
            ItemBool = new bool[ 64 ];
            for( var i = 0; i < 64; i++ )
                ItemBool[ i ] = parser.ReadColumn< bool >( 789 + i );
            AnnounceBool = new bool[ 64 ];
            for( var i = 0; i < 64; i++ )
                AnnounceBool[ i ] = parser.ReadColumn< bool >( 853 + i );
            BehaviorBool = new bool[ 64 ];
            for( var i = 0; i < 64; i++ )
                BehaviorBool[ i ] = parser.ReadColumn< bool >( 917 + i );
            AcceptBool = new bool[ 64 ];
            for( var i = 0; i < 64; i++ )
                AcceptBool[ i ] = parser.ReadColumn< bool >( 981 + i );
            QualifiedBool = new bool[ 64 ];
            for( var i = 0; i < 64; i++ )
                QualifiedBool[ i ] = parser.ReadColumn< bool >( 1045 + i );
            CanTargetBool = new bool[ 64 ];
            for( var i = 0; i < 64; i++ )
                CanTargetBool[ i ] = parser.ReadColumn< bool >( 1109 + i );
            ToDoCompleteSeq = new byte[ 24 ];
            for( var i = 0; i < 24; i++ )
                ToDoCompleteSeq[ i ] = parser.ReadColumn< byte >( 1173 + i );
            ToDoQty = new byte[ 24 ];
            for( var i = 0; i < 24; i++ )
                ToDoQty[ i ] = parser.ReadColumn< byte >( 1197 + i );
            ToDoMainLocation = new LazyRow< Level >[ 24 ];
            for( var i = 0; i < 24; i++ )
                ToDoMainLocation[ i ] = new LazyRow< Level >( gameData, parser.ReadColumn< uint >( 1221 + i ), language );
            ToDoChildLocation = new LazyRow< Level >[ 24, 7 ];
            for (var i = 0; i < 24; i++)
            for (var j = 0; j < 7; j++)
                ToDoChildLocation[i, j] = new LazyRow< Level >(gameData, parser.ReadColumn< uint >( 1245 + (j * 24) + i ), language);
            CountableNum = new byte[ 24 ];
            for( var i = 0; i < 24; i++ )
                CountableNum[ i ] = parser.ReadColumn< byte >( 1413 + i );
            LevelMax = parser.ReadColumn< byte >( 1437 );
            ClassJobRequired = new LazyRow< ClassJob >( gameData, parser.ReadColumn< byte >( 1438 ), language );
            QuestRewardOtherDisplay = new LazyRow< QuestRewardOther >( gameData, parser.ReadColumn< byte >( 1439 ), language );
            ExpFactor = parser.ReadColumn< ushort >( 1440 );
            GilReward = parser.ReadColumn< uint >( 1441 );
            CurrencyReward = new LazyRow< Item >( gameData, parser.ReadColumn< uint >( 1442 ), language );
            CurrencyRewardCount = parser.ReadColumn< uint >( 1443 );
            ItemCatalyst = new LazyRow< Item >[ 3 ];
            for( var i = 0; i < 3; i++ )
                ItemCatalyst[ i ] = new LazyRow< Item >( gameData, parser.ReadColumn< byte >( 1444 + i ), language );
            ItemCountCatalyst = new byte[ 3 ];
            for( var i = 0; i < 3; i++ )
                ItemCountCatalyst[ i ] = parser.ReadColumn< byte >( 1447 + i );
            ItemRewardType = parser.ReadColumn< byte >( 1450 );
            ItemReward = new uint[ 7 ];
            for( var i = 0; i < 7; i++ )
                ItemReward[ i ] = parser.ReadColumn< uint >( 1451 + i );
            ItemCountReward = new byte[ 7 ];
            for( var i = 0; i < 7; i++ )
                ItemCountReward[ i ] = parser.ReadColumn< byte >( 1458 + i );
            Unknown1465 = parser.ReadColumn< bool >( 1465 );
            Unknown1466 = parser.ReadColumn< bool >( 1466 );
            Unknown1467 = parser.ReadColumn< bool >( 1467 );
            Unknown1468 = parser.ReadColumn< bool >( 1468 );
            Unknown1469 = parser.ReadColumn< bool >( 1469 );
            Unknown1470 = parser.ReadColumn< bool >( 1470 );
            Unknown1471 = parser.ReadColumn< bool >( 1471 );
            StainReward = new LazyRow< Stain >[ 7 ];
            for( var i = 0; i < 7; i++ )
                StainReward[ i ] = new LazyRow< Stain >( gameData, parser.ReadColumn< byte >( 1472 + i ), language );
            OptionalItemReward = new LazyRow< Item >[ 5 ];
            for( var i = 0; i < 5; i++ )
                OptionalItemReward[ i ] = new LazyRow< Item >( gameData, parser.ReadColumn< uint >( 1479 + i ), language );
            OptionalItemCountReward = new byte[ 5 ];
            for( var i = 0; i < 5; i++ )
                OptionalItemCountReward[ i ] = parser.ReadColumn< byte >( 1484 + i );
            OptionalItemIsHQReward = new bool[ 5 ];
            for( var i = 0; i < 5; i++ )
                OptionalItemIsHQReward[ i ] = parser.ReadColumn< bool >( 1489 + i );
            OptionalItemStainReward = new LazyRow< Stain >[ 5 ];
            for( var i = 0; i < 5; i++ )
                OptionalItemStainReward[ i ] = new LazyRow< Stain >( gameData, parser.ReadColumn< byte >( 1494 + i ), language );
            EmoteReward = new LazyRow< Emote >( gameData, parser.ReadColumn< byte >( 1499 ), language );
            ActionReward = new LazyRow< Action >( gameData, parser.ReadColumn< ushort >( 1500 ), language );
            GeneralActionReward = new LazyRow< GeneralAction >[ 2 ];
            for( var i = 0; i < 2; i++ )
                GeneralActionReward[ i ] = new LazyRow< GeneralAction >( gameData, parser.ReadColumn< byte >( 1501 + i ), language );
            SystemReward0 = parser.ReadColumn< ushort >( 1503 );
            OtherReward = new LazyRow< QuestRewardOther >( gameData, parser.ReadColumn< byte >( 1504 ), language );
            SystemReward1 = parser.ReadColumn< ushort >( 1505 );
            GCTypeReward = parser.ReadColumn< ushort >( 1506 );
            InstanceContentUnlock = new LazyRow< InstanceContent >( gameData, parser.ReadColumn< uint >( 1507 ), language );
            Tomestone = parser.ReadColumn< byte >( 1508 );
            TomestoneReward = parser.ReadColumn< byte >( 1509 );
            TomestoneCountReward = parser.ReadColumn< byte >( 1510 );
            ReputationReward = parser.ReadColumn< byte >( 1511 );
            PlaceName = new LazyRow< PlaceName >( gameData, parser.ReadColumn< ushort >( 1512 ), language );
            JournalGenre = new LazyRow< JournalGenre >( gameData, parser.ReadColumn< byte >( 1513 ), language );
            Unknown1514 = parser.ReadColumn< byte >( 1514 );
            Icon = parser.ReadColumn< uint >( 1515 );
            IconSpecial = parser.ReadColumn< uint >( 1516 );
            Introduction = parser.ReadColumn< bool >( 1517 );
            HideOfferIcon = parser.ReadColumn< bool >( 1518 );
            EventIconType = new LazyRow< EventIconType >( gameData, parser.ReadColumn< byte >( 1519 ), language );
            Unknown1520 = parser.ReadColumn< byte >( 1520 );
            SortKey = parser.ReadColumn< ushort >( 1521 );
            Unknown1522 = parser.ReadColumn< bool >( 1522 );
        }
    }
}