using System.Text.Json;

    namespace TxtRPG
    {
        internal class RPG
        {
            // 아이템 인터페이스
            public interface IItem
            {
                string Name { get; } // 아이템 이름
                string Description { get; } // 아이템 설명
                int AttackBonus { get; } // 공격력 보너스
                int DefenseBonus { get; } // 방어력 보너스
                string Type { get; } // 아이템 타입 (무기, 방어구)
                bool IsEquipped { get; set; } // 장착 여부
            }

            // 아이템 클래스
            public class Item : IItem
            {
                public string Name { get; }
                public string Description { get; }
                public int AttackBonus { get; }
                public int DefenseBonus { get; }
                public string Type { get; }
                public bool IsEquipped { get; set; }

                // 생성자
                public Item(string name, string description, int attackBonus, int defenseBonus, string type)
                {
                    Name = name;
                    Description = description;
                    AttackBonus = attackBonus;
                    DefenseBonus = defenseBonus;
                    Type = type;
                    IsEquipped = false;
                }
            }

            // 캐릭터 클래스
            public class Character
            {
                public string Name { get; } // 캐릭터 이름
                public string Job { get; } // 캐릭터 직업
                public int Level { get; set; } // 캐릭터 레벨
                public int BaseAttack { get; set; } // 기본 공격력
                public int BaseDefense { get; set; } // 기본 방어력
                public int Health { get; set; } // 현재 체력
                public int Gold { get; set; } // 보유 골드
                public List<IItem> Inventory { get; set; } // 인벤토리
                public int DungeonClearCount { get; set; } // 던전 클리어 횟수


                // 생성자
                public Character(string name, string job, int baseAttack, int baseDefense, int health, int gold)
                {
                    Name = name;
                    Job = job;
                    Level = 1;
                    BaseAttack = baseAttack;
                    BaseDefense = baseDefense;
                    Health = health;
                    Gold = gold;
                    Inventory = new List<IItem>();
                    DungeonClearCount = 0;
                }

                // 장착 아이템을 포함한 총 공격력 계산
                public int GetTotalAttack()
                {
                    int attackBonus = 0;
                    foreach (var item in Inventory)
                    {
                        if (item.IsEquipped)
                        {
                            attackBonus += item.AttackBonus;
                        }
                    }
                    return BaseAttack + attackBonus;
                }

                // 장착 아이템을 포함한 총 방어력 계산
                public int GetTotalDefense()
                {
                    int defenseBonus = 0;
                    foreach (var item in Inventory)
                    {
                        if (item.IsEquipped)
                        {
                            defenseBonus += item.DefenseBonus;
                        }
                    }
                    return BaseDefense + defenseBonus;
                }
                // 레벨업 처리
                public void LevelUp()
                {
                    Level++;
                    BaseAttack = (int)(BaseAttack + 0.5);
                    BaseDefense += 1;
                    Console.WriteLine($"\n레벨 업!! 현재 레벨: {Level}");
                }
            }

            // 상점 클래스
            public class Shop
            {
                public List<IItem> ItemsForSale { get; } // 판매 아이템 목록

                // 생성자
                public Shop()
                {
                    ItemsForSale = new List<IItem> 
        {
            new Item("가죽 갑옷",          "맨몸보다는 나을지도..?.",         0, 5, "방어구"),
            new Item("쇠 갑옷",            "쇠로 만들어진 갑옷.",             0, 15, "방어구"),
            new Item("튼튼한 갑옷",        "생각보다 튼튼한 갑옷.",           0, 30, "방어구"),
            new Item("갑주",               "아무도 내몸에 손댈 순 없다.",     0, 60, "방어구"),
            new Item("미래식 방어형 슈트", "미래형 최첨단 슈트.",             0, 120, "방어구"),
            new Item("낡은 검",            "맨손 보다야 뭐라도...",           5, 0, "무기"),      
            new Item("도끼",               "손잡이가 헤진 도끼.",             15, 0, "무기"),
            new Item("창",                 "길고 날카로운 창.",               30, 0, "무기"),
            new Item("명검",               "무엇이든 베어버리는 검.",         60, 0,"무기"),
            new Item("광선검",             "무엇이든 베어버리는 광선검.",     120, 0,"무기")
        };
                }
            }
            // 던전 클래스
            public class Dungeon
            {
                public string Name { get; } // 던전 이름
                public int RecommendedDefense { get; } // 권장 방어력
                public int BaseReward { get; } // 기본 보상

                // 생성자
                public Dungeon(string name, int recommendedDefense, int baseReward)
                {
                    Name = name;
                    RecommendedDefense = recommendedDefense;
                    BaseReward = baseReward;
                }
            }
            // 게임 클래스
            public class Game
            {
                public Character _player; // 플레이어 캐릭터
                public Shop _shop; // 상점
                public Dictionary<string, bool> _purchasedItems; // 구매한 아이템 목록
                public List<Dungeon> _dungeons; // 던전 목록
                public const int MaxHealth = 100; // 최대 체력
                public string SaveFilePath = "game_save.json"; // 저장 파일 경로


                // 생성자
                public Game()
                {
                    LoadGame(); // 게임 데이터 로드

                    if (_player == null) // 기존 세이브가 없으면 초기화
                    {
                        _player = new Character("Chad", "전사", 10, 5, 100, 1500);
                    }
                    _shop = new Shop();
                    if (_purchasedItems == null)
                    {
                        _purchasedItems = new Dictionary<string, bool>();
                    }
                    _dungeons = new List<Dungeon>
        {
             new Dungeon("쉬운 던전", 5, 1000),
             new Dungeon("일반 던전", 11, 1700),
             new Dungeon("어려운 던전", 17, 2500),
             new Dungeon("지옥 던전", 50, 5000)
        };
                }
                // 게임 데이터 로드
                public void LoadGame()
                {
                    if (File.Exists(SaveFilePath))
                    {
                        try
                        {
                            string json = File.ReadAllText(SaveFilePath);
                            var save = JsonSerializer.Deserialize<SaveData>(json);

                            _player = save.Player;
                            _purchasedItems = save.PurchasedItems;
                        if (save.Inventory != null) // null 체크 추가
                        {
                            _player.Inventory = save.Inventory;
                        }
                        else
                        {
                            _player.Inventory = new List<IItem>();
                        }

                        foreach (var item in _player.Inventory)
                            {
                                foreach (var shopItem in _shop.ItemsForSale)
                                {
                                    if (item.Name == shopItem.Name)
                                    {
                                        item.IsEquipped = shopItem.IsEquipped;
                                        break;
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"데이터 로드 실패: {ex.Message}");

                        }
                    }
                }
                // 게임 데이터 저장
                public void SaveGame()
                {
                    try
                    {
                        var save = new SaveData { Player = _player, PurchasedItems = _purchasedItems, Inventory = _player.Inventory }; // 수정
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        string json = JsonSerializer.Serialize(save, options);
                        File.WriteAllText(SaveFilePath, json);
                        Console.WriteLine("\n게임 저장 완료!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"데이터 저장 실패: {ex.Message}");

                    }
                }

                // 게임 시작
                public void Start()
                {
                    Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
                    Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");

                    while (true)
                    {
                        ShowMainMenu(); // 메인 메뉴 표시
                        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                        string input = Console.ReadLine();

                        switch (input)
                        {
                            case "1":
                                ShowCharacterStatus(); // 상태 보기
                                break;
                            case "2":
                                ShowInventory(); // 인벤토리
                                break;
                            case "3":
                                ShowShop(); // 상점
                                break;
                            case "4":
                                EnterDungeon(); // 던전 입장
                                break;
                            case "5":
                                Rest(); // 휴식
                                break;
                        case "6":
                            ResetGame(); // 게임 리셋
                            break;
                        case "0":
                                SaveGame(); // 게임 저장
                                return;

                            default:
                                Console.WriteLine("\n잘못된 입력입니다.");
                                break;
                        }
                    }
                }

                // 메인 메뉴 표시
                public void ShowMainMenu()
                {
                    Console.WriteLine("1. 상태 보기");
                    Console.WriteLine("2. 인벤토리");
                    Console.WriteLine("3. 상점");
                    Console.WriteLine("4. 던전입장");
                    Console.WriteLine("5. 휴식하기");
                    Console.WriteLine("6. 게임 리셋"); // 추가
                    Console.WriteLine("0. 게임 종료");
                }

                // 캐릭터 상태 보기
                public void ShowCharacterStatus()
                {
                    Console.WriteLine("\n[상태 보기]");
                    Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");
                    Console.WriteLine($"Lv. {_player.Level:D2}");
                    Console.WriteLine($"{_player.Name} ( {_player.Job} )");
                    Console.WriteLine($"공격력 : {_player.GetTotalAttack()}");
                    Console.WriteLine($"방어력 : {_player.GetTotalDefense()}");
                    Console.WriteLine($"체 력 : {_player.Health}");
                    Console.WriteLine($"Gold : {_player.Gold} G\n");

                    while (true)
                    {
                        Console.WriteLine("0. 나가기");
                        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                        string input = Console.ReadLine();

                        if (input == "0") break;
                        else Console.WriteLine("\n잘못된 입력입니다.");
                    }
                }

                // 인벤토리 보기
                public void ShowInventory()
                {
                    Console.WriteLine("\n[인벤토리]");
                    Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");
                    Console.WriteLine("[아이템 목록]");
                    if (_player.Inventory.Count == 0)
                    {
                        Console.WriteLine("보유 중인 아이템이 없습니다.\n");
                    }
                    else
                    {
                        for (int i = 0; i < _player.Inventory.Count; i++)
                        {
                            string equipped = _player.Inventory[i].IsEquipped ? "[E]" : "";
                            Console.WriteLine($"- {i + 1} {equipped}{_player.Inventory[i].Name,-15} | 공격력 +{_player.Inventory[i].AttackBonus} | 방어력 +{_player.Inventory[i].DefenseBonus} | {_player.Inventory[i].Description}");
                        }
                    }

                    while (true)
                    {
                        Console.WriteLine("\n1. 장착 관리");
                        Console.WriteLine("0. 나가기");
                        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                        string input = Console.ReadLine();

                        if (input == "1")
                        {
                            ManageEquip(); // 장착 관리
                            break;
                        }
                        else if (input == "0")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\n잘못된 입력입니다.");
                        }
                    }
                }

                // 아이템 장착 관리
                public void ManageEquip()
                {
                    Console.WriteLine("\n[인벤토리 - 장착 관리]");
                    Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");
                    if (_player.Inventory.Count == 0)
                    {
                        Console.WriteLine("보유 중인 아이템이 없습니다.\n");
                        return;
                    }
                    Console.WriteLine("[아이템 목록]");
                    for (int i = 0; i < _player.Inventory.Count; i++)
                    {
                        string equipped = _player.Inventory[i].IsEquipped ? "[E]" : "";
                        Console.WriteLine($"- {i + 1} {equipped}{_player.Inventory[i].Name,-15} | 공격력 +{_player.Inventory[i].AttackBonus} | 방어력 +{_player.Inventory[i].DefenseBonus} | {_player.Inventory[i].Description}");
                    }

                    while (true)
                    {
                        Console.WriteLine("\n0. 나가기");
                        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                        string input = Console.ReadLine();

                        if (input == "0") break;

                        if (int.TryParse(input, out int itemIndex) && itemIndex > 0 && itemIndex <= _player.Inventory.Count)
                        {
                            IItem selectedItem = _player.Inventory[itemIndex - 1];
                            EquipItem(selectedItem); // 아이템 장착
                        }
                        else
                        {
                            Console.WriteLine("\n잘못된 입력입니다.");
                        }
                    }
                }
                // 아이템 장착
                public void EquipItem(IItem newItem)
                {
                    if (newItem.Type == "무기")
                    {
                        var existingWeapon = _player.Inventory.FirstOrDefault(item => item.Type == "무기" && item.IsEquipped);
                        if (existingWeapon != null)
                        {
                            existingWeapon.IsEquipped = false;
                        }
                        newItem.IsEquipped = !newItem.IsEquipped;
                        Console.WriteLine($"{newItem.Name} {(newItem.IsEquipped ? "장착" : "장착 해제")} 했습니다.");
                    }
                    else if (newItem.Type == "방어구")
                    {
                        var existingArmor = _player.Inventory.FirstOrDefault(item => item.Type == "방어구" && item.IsEquipped);
                        if (existingArmor != null)
                        {
                            existingArmor.IsEquipped = false;
                        }
                        newItem.IsEquipped = !newItem.IsEquipped;
                        Console.WriteLine($"{newItem.Name} {(newItem.IsEquipped ? "장착" : "장착 해제")} 했습니다.");
                    }
                }


                // 상점 보기
                public void ShowShop()
                {
                    Console.WriteLine("\n[상점]");
                    Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
                    Console.WriteLine($"[보유 골드]\n{_player.Gold} G\n");
                    Console.WriteLine("[아이템 목록]");
                    for (int i = 0; i < _shop.ItemsForSale.Count; i++)
                    {
                        string isPurchased = _purchasedItems.ContainsKey(_shop.ItemsForSale[i].Name) ? "구매완료" : "";
                        Console.WriteLine($"- {i + 1} {_shop.ItemsForSale[i].Name,-15} | 공격력 +{_shop.ItemsForSale[i].AttackBonus} | 방어력 +{_shop.ItemsForSale[i].DefenseBonus} | {_shop.ItemsForSale[i].Description,-40} | {isPurchased,-10} ");
                    }

                    while (true)
                    {
                        Console.WriteLine("\n1. 아이템 구매");
                        Console.WriteLine("2. 아이템 판매");
                        Console.WriteLine("0. 나가기");
                        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                        string input = Console.ReadLine();

                        if (input == "1")
                        {
                            PurchaseItem(); // 아이템 구매
                            break;
                        }
                        else if (input == "2")
                        {
                            SellItem(); // 아이템 판매
                            break;
                        }
                        else if (input == "0")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\n잘못된 입력입니다.");
                        }
                    }
                }
                // 아이템 구매
                public void PurchaseItem()
                {
                    Console.WriteLine("\n[상점 - 아이템 구매]");
                    Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
                    Console.WriteLine($"[보유 골드]\n{_player.Gold} G\n");
                    Console.WriteLine("[아이템 목록]");
                    for (int i = 0; i < _shop.ItemsForSale.Count; i++)
                    {
                        string isPurchased = _purchasedItems.ContainsKey(_shop.ItemsForSale[i].Name) ? "구매완료" : $"{GetItemPrice(_shop.ItemsForSale[i])} G";
                        Console.WriteLine($"- {i + 1} {_shop.ItemsForSale[i].Name,-15} | 공격력 +{_shop.ItemsForSale[i].AttackBonus} | 방어력 +{_shop.ItemsForSale[i].DefenseBonus} | {_shop.ItemsForSale[i].Description,-40} | {isPurchased,-10} ");
                    }


                    while (true)
                    {
                        Console.WriteLine("\n0. 나가기");
                        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                        string input = Console.ReadLine();

                        if (input == "0") break;

                        if (int.TryParse(input, out int itemIndex) && itemIndex > 0 && itemIndex <= _shop.ItemsForSale.Count)
                        {
                            IItem selectedItem = _shop.ItemsForSale[itemIndex - 1];
                            if (_purchasedItems.ContainsKey(selectedItem.Name))
                            {
                                Console.WriteLine("\n이미 구매한 아이템입니다.");
                            }
                            else
                            {
                                int itemPrice = GetItemPrice(selectedItem);
                                if (_player.Gold >= itemPrice)
                                {
                                    _player.Gold -= itemPrice;
                                    _player.Inventory.Add(selectedItem);
                                    _purchasedItems.Add(selectedItem.Name, true);
                                    Console.WriteLine($"\n{selectedItem.Name}을(를) 구매했습니다.");
                                }
                                else
                                {
                                    Console.WriteLine("\nGold 가 부족합니다.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n잘못된 입력입니다.");
                        }
                    }
                }
                // 아이템 판매
                public void SellItem()
                {
                    Console.WriteLine("\n[상점 - 아이템 판매]");
                    Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
                    Console.WriteLine($"[보유 골드]\n{_player.Gold} G\n");
                    if (_player.Inventory.Count == 0)
                    {
                        Console.WriteLine("보유 중인 아이템이 없습니다.\n");
                        return;
                    }
                    Console.WriteLine("[아이템 목록]");
                    for (int i = 0; i < _player.Inventory.Count; i++)
                    {
                        Console.WriteLine($"- {i + 1} {_player.Inventory[i].Name,-15} | 공격력 +{_player.Inventory[i].AttackBonus} | 방어력 +{_player.Inventory[i].DefenseBonus} | {_player.Inventory[i].Description,-40} | {GetSellPrice(_player.Inventory[i])} G");
                    }

                    while (true)
                    {
                        Console.WriteLine("\n0. 나가기");
                        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                        string input = Console.ReadLine();

                        if (input == "0") break;

                        if (int.TryParse(input, out int itemIndex) && itemIndex > 0 && itemIndex <= _player.Inventory.Count)
                        {
                            IItem selectedItem = _player.Inventory[itemIndex - 1];
                            int sellPrice = GetSellPrice(selectedItem);
                            _player.Gold += sellPrice;

                            if (selectedItem.IsEquipped)
                            {
                                selectedItem.IsEquipped = false;
                            }
                            _player.Inventory.Remove(selectedItem);

                            Console.WriteLine($"\n{selectedItem.Name}을(를) {sellPrice} G 에 판매했습니다.");
                        }
                        else
                        {
                            Console.WriteLine("\n잘못된 입력입니다.");
                        }
                    }
                }

                // 아이템 가격 계산
                public int GetItemPrice(IItem item)
                {
                    // 아이템 가격 정책을 설정합니다. (예: 방어력과 공격력에 따라 가격이 달라짐)
                    return (item.AttackBonus * 150) + (item.DefenseBonus * 100);
                }

                // 아이템 판매 가격 계산
                public int GetSellPrice(IItem item)
                {
                    // 아이템 판매 정책 (구매 가격의 85%)
                    return (int)(GetItemPrice(item) * 0.85);
                }
                // 휴식하기
                public void Rest()
                {
                    Console.WriteLine("\n[휴식하기]");
                    Console.WriteLine($"500 G 를 내면 체력을 회복할 수 있습니다. (보유 골드 : {_player.Gold} G)\n");
                    while (true)
                    {
                        Console.WriteLine("1. 휴식하기");
                        Console.WriteLine("0. 나가기");
                        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                        string input = Console.ReadLine();

                        if (input == "1")
                        {
                            if (_player.Gold >= 500)
                            {
                                _player.Gold -= 500;
                                _player.Health = Math.Min(MaxHealth, _player.Health + 100);
                                Console.WriteLine("\n휴식을 완료했습니다.");
                            }
                            else
                            {
                                Console.WriteLine("\nGold 가 부족합니다.");
                            }
                            break;
                        }
                        else if (input == "0")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\n잘못된 입력입니다.");
                        }
                    }
                }

                // 던전 입장
                public void EnterDungeon()
                {
                    Console.WriteLine("\n[던전입장]");
                    Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");
                    while (true)
                    {
                        Console.WriteLine("1. 쉬운 던전     | 방어력 5 이상 권장");
                        Console.WriteLine("2. 일반 던전     | 방어력 11 이상 권장");
                        Console.WriteLine("3. 어려운 던전   | 방어력 17 이상 권장");
                        Console.WriteLine("4. 지옥 던전     | 방어력 ?  이상 권장");
                        Console.WriteLine("0. 나가기");
                        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                        string input = Console.ReadLine();

                        if (input == "0") break;

                        if (int.TryParse(input, out int dungeonLevel) && dungeonLevel > 0 && dungeonLevel <= _dungeons.Count)
                        {
                            StartDungeon(_dungeons[dungeonLevel - 1]); // 던전 시작
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\n잘못된 입력입니다.");
                        }
                    }
                }
                // 던전 시작
                public void StartDungeon(Dungeon dungeon)
                {
                    Console.WriteLine($"\n[던전 입장] {dungeon.Name} 에 입장합니다.");

                    Random random = new Random();
                    int successRate = 60;
                    int healthDecrease = 0;
                    if (_player.GetTotalDefense() < dungeon.RecommendedDefense)
                    {
                        int failRate = random.Next(1, 101);

                        if (failRate > successRate)
                        {
                            Console.WriteLine("\n던전 실패!!");
                            healthDecrease = _player.Health / 2;
                            _player.Health -= healthDecrease;
                            Console.WriteLine($"체력 {healthDecrease} 감소, 현재 체력: {_player.Health}");
                            return;
                        }
                    }

                    Console.WriteLine("\n던전 클리어!!");
                    int baseHealthDecrease = random.Next(20, 36);
                    int healthChange = _player.GetTotalDefense() - dungeon.RecommendedDefense;
                    healthDecrease = Math.Max(0, baseHealthDecrease + healthChange);
                    _player.Health = Math.Max(0, _player.Health - healthDecrease);


                    int reward = dungeon.BaseReward;
                    int attackPercent = random.Next(_player.GetTotalAttack(), _player.GetTotalAttack() * 2 + 1);
                    double additionalReward = (double)reward * (attackPercent / 100.0);
                    int totalReward = (int)(reward + additionalReward);

                    _player.Gold += totalReward;
                    _player.DungeonClearCount += 1;

                    Console.WriteLine($"\n[탐험 결과]\n체력 {_player.Health + healthDecrease} -> {_player.Health}");
                    Console.WriteLine($"Gold {_player.Gold - totalReward} -> {_player.Gold} G\n");

                    CheckLevelUp(); // 레벨업 확인
                }
                // 레벨업 확인
                public void CheckLevelUp()
                {
                    if (_player.DungeonClearCount == _player.Level)
                    {
                        _player.LevelUp(); // 레벨업
                    }
                }


            public void ResetGame()
            {
                Console.WriteLine("\n정말로 게임을 리셋하시겠습니까? (y/n)");
                string input = Console.ReadLine();
                if (input.ToLower() == "y")
                {
                    _player = new Character("Chad", "전사", 10, 5, 100, 1500);
                    _purchasedItems = new Dictionary<string, bool>();
                    if (File.Exists(SaveFilePath))
                    {
                        File.Delete(SaveFilePath);
                    }
                    Console.WriteLine("\n게임이 리셋되었습니다.");
                }
                else
                {
                    Console.WriteLine("\n게임 리셋이 취소되었습니다.");
                }
            }
        }
            // 게임 저장 데이터 클래스
            public class SaveData
            {
                public Character Player { get; set; }
                public Dictionary<string, bool> PurchasedItems { get; set; }
                public List<IItem> Inventory { get; set; } // 추가
            }
            public class Program
            {
                public static void Main(string[] args)
                {
                    Game game = new Game();
                    game.Start();
                }
            }
        }
    }
