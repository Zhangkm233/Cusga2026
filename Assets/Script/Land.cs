using System.ComponentModel;
using UnityEngine;

public enum LandType
{
    HILL, //ɽ��
    PLAIN, //ƽԭ
    FOREST, //ɭ��
    MOUNTAIN, //ɽ��
    JUNGLE, //����
    WHEATLAND, //����
    CABIN, //ľ��
    THATCH, //é��
    TOWN, //С��
    WAREHOUSE, //�ֿ�
    TOWER, //��
    WINDMILL, //�糵
    RUIN //����
}

public abstract class Land
{
    private LandType landType; //��������
    private int energyCounter = 0; //����������ʾ�õ������ж��ٸ�����
    private int requiredEnergy; //��������Ч����Ҫ�ĳ�������
    public MaterialType storageCardType = MaterialType.NULL; //�Ѵ洢�Ŀ�Ƭ���
    public int storageCardNum = 0; //�洢�Ŀ�Ƭ����
    private int preference = 0; //ƫ�õ�����ֵ
    private int soild = 0; //���ֵ ���Եֵ�һ�ι�����������Ч��
    private int hunterarea = 0;//��Ȧֵ ��������ĸ�������һ��
    private static int maxSoild = 1; //�����ֵ
    private static int maxHunterarea = 2; //�����Ȧֵ
    private int mapRow; //�������ڵ�ͼ��
    private int mapCol; //�������ڵ�ͼ��
    private int level;

    //public delegate void LandChangeEventHandler(Land oldLand,Land newLand);
    //public event LandChangeEventHandler OnLandChanged; //���α仯�¼�
    
    public int RequiredEnergy {
        get { return requiredEnergy; }
        set { requiredEnergy = value; }
    }
    public LandType LandType {
        get { return landType; }
        set { landType = value; }
    }
    public int Soild {
        get { return soild; }
        set { soild = value; }
    }
    public int Hunterarea {
        get { return hunterarea; }
        set { hunterarea = value; }
    }
    public int MapRow {
        get { return mapRow; }
        set { mapRow = value; }
    }
    public int MapCol {
        get { return mapCol; }
        set { mapCol = value; }
    }

    public int EnergyCounter {
        get { return energyCounter; }
        set { energyCounter = value; }
    }
    public int Preference {
        get { return preference; }
        set { preference = value; }
    }

    public int Level {
        get { return level; }
        set { level = value; }
    }

    public bool IsRandomEventTriggered(int probability) {
        int rand = Random.Range(1,101); // ����1��100�������
        return rand <= probability; // ��������С�ڵ��ڸ���ֵ�����¼�����
    }

    public void AddEnergy(int energy) {
        energyCounter += energy; // ���ӳ���
        PassiveEffect();
        Debug.Log($"���� {landType} �������� {energy}����ǰ���ܣ�{energyCounter}");
    }

    public void SpawnAnimal(AnimalType animalType) {
        MapManager.Instance.AddAnimalToLand(animalType,this);
    }

    public abstract void PassiveEffect(); //����Ч��
    public abstract void MaterialEffect(MaterialCard materialCard);
    public abstract void ExtraEffect(); //����Ч��

    public void StorageIn(MaterialType materialType) {
        if (storageCardType == materialType) {
            storageCardNum += 1; // �洢�Ŀ�Ƭ��������1
        } else {
            storageCardType = materialType; // �洢�Ŀ�Ƭ����Ϊ����
            storageCardNum = 1; // �洢�Ŀ�Ƭ��������Ϊ1
        }
    }

    public void AddSoild(int value) {
        soild += value;
        if (soild > maxSoild) {
            soild = maxSoild; // ���ֵ���ܳ������ֵ
        }
    }
    public void AddHunterarea(int value) {
        hunterarea += value;
        if (hunterarea > maxHunterarea) {
            hunterarea = maxHunterarea; // ��Ȧֵ���ܳ������ֵ
        }
    }

    public void AddCard(Card card,int num) {
        DeckManager.Instance.AddCardToDeckFromLand(card,num,this);
    }
    public void AddExtraDraw(CardType cardType,int num) {
        for (int i = 0;i < num;i++) {
            DeckManager.Instance.extraCertainCardType.Add(cardType);
        }
    }

    public void AddExtraDraw(CardType cardType) {
        DeckManager.Instance.extraCertainCardType.Add(cardType);
    }

    public void AddExtraDraw(int cardId,int num) {
        for (int i = 0;i < num;i++) {
            DeckManager.Instance.extraCertainCardId.Add(cardId);
        }
    }

    public void AddExtraDraw(int cardId) {
        DeckManager.Instance.extraCertainCardId.Add(cardId);
    }


    public void ChangeLandType(LandType landType) {
        //������ת����εĺ��� ��������ϣ�����Ҫ��һ�ĵ���Ϸ����ĵ��ã�Ӧ��UI������Ҫȫ����д
        //��Ҫ���߼����������Ӵ洢 Ȼ������ʾ����Ϸ/UE��

        MapManager.Instance.LandMap[mapRow][mapCol] = null;
        Land newLand = null;
        switch (landType) {
            case LandType.HILL:
                newLand = new HillLand(mapRow,mapCol);
                break;
            case LandType.PLAIN:
                newLand = new PlainLand(mapRow,mapCol);
                break;
            case LandType.FOREST:
                newLand = new ForestLand(mapRow,mapCol);
                break;
            case LandType.THATCH:
                newLand = new ThatchLand(mapRow,mapCol);
                break;
            case LandType.CABIN:
                newLand = new CabinLand(mapRow,mapCol);
                break;
            case LandType.MOUNTAIN:
                newLand = new MountainLand(mapRow,mapCol);
                break;
            case LandType.TOWN:
                newLand = new TownLand(mapRow,mapCol);
                break;
            case LandType.TOWER:
                newLand = new TowerLand(mapRow,mapCol);
                break;
            case LandType.WAREHOUSE:
                newLand = new WarehouseLand(mapRow,mapCol);
                break;
            case LandType.WINDMILL:
                newLand = new WindmillLand(mapRow,mapCol);
                break;
            case LandType.JUNGLE:
                newLand = new JungleLand(mapRow,mapCol);
                break;
            case LandType.WHEATLAND:
                newLand = new WheatLand(mapRow,mapCol);
                break;
        }
        //�̳������������ԣ���̺���Ȧ֮��ģ������ܲ��̳У�
        newLand.soild = this.soild;
        newLand.hunterarea = this.hunterarea;
        MapManager.Instance.LandMap[mapRow][mapCol] = newLand;
        //this.gameObject.AddComponent(newLand.GetType());
        //�����Ҳ�֪��д�ĶԲ��ԣ��д���ȶ
        //Destroy(this);
        //������Ҫ������Ϸ�ڶ�Ӧ��gameobject
        //OnLandChanged.Invoke(this,newLand);
    }
}

public class HillLand : Land 
{
    // ɽ�����
    public HillLand(int row, int col) {
        LandType = LandType.HILL;
        RequiredEnergy = 4;
        Preference = 20;
        Level = 0;
        MapRow = row;
        MapCol = col;
    }

    public override void PassiveEffect() {
        // ɽ����εı���Ч�� 4ʱ��1ʯ
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("ɽ����α���Ч������");
            AddCard(new MaterialCard(MaterialType.STONE),1); //����һ��ʯͷ���Ͽ����ƿ�
            EnergyCounter = 0; // ���ü�����
        }
    }

    public override void MaterialEffect(MaterialCard materialCard) {
        switch(materialCard.materialType) {
            case MaterialType.WOOD:
                //4��ľ�ݼ��
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 4 && storageCardType == MaterialType.WOOD) {
                    AddSoild(1);
                    ChangeLandType(LandType.CABIN);
                }
                break;
            case MaterialType.HAY:
                //��2ľ��
                AddCard(new MaterialCard(MaterialType.WOOD),2);
                break;
            case MaterialType.STONE:
                //1��ɽ��
                StorageIn(MaterialType.STONE);
                if (storageCardNum >= 1 && storageCardType == MaterialType.STONE) {
                    ChangeLandType(LandType.MOUNTAIN);
                }
                break;
            case MaterialType.MEAT:
                //��1��
                SpawnAnimal(AnimalType.BEAR);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //���ﻹûд Ӧ����20%��1ʯ
        if(IsRandomEventTriggered(20)) {
            Debug.Log("ɽ����ζ���Ч������");
            AddCard(new MaterialCard(MaterialType.STONE),1  ); //����һ��ʯͷ���Ͽ����ƿ�
        }
    }
}

public class PlainLand : Land
{
    // ƽԭ����
    public PlainLand(int row,int col) {
        LandType = LandType.PLAIN;
        RequiredEnergy = 3;
        Preference = 30;
        Level = 0;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        // ƽԭ���εı���Ч�� 3ʱ��1��
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("ƽԭ���α���Ч������");
            AddCard(new MaterialCard(MaterialType.HAY),1); //����һ���ɲݲ��Ͽ����ƿ�
            EnergyCounter = 0; // ���ü�����
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //3��ľ��
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 3 && storageCardType == MaterialType.WOOD) {
                    ChangeLandType(LandType.CABIN);
                }
                break;
            case MaterialType.HAY:
                //4��é��
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 4 && storageCardType == MaterialType.HAY) {
                    ChangeLandType(LandType.THATCH);
                }
                break;
            case MaterialType.STONE:
                //2������
                StorageIn(MaterialType.STONE);
                if (storageCardNum >= 2 && storageCardType == MaterialType.STONE) {
                    ChangeLandType(LandType.WHEATLAND);
                }
                break;
            case MaterialType.MEAT:
                //2��1��1׷��
                StorageIn(MaterialType.MEAT);
                if (storageCardNum >= 2 && storageCardType == MaterialType.MEAT) {
                    SpawnAnimal(AnimalType.GOBLIN);
                    AddCard(new SkillCard(SkillType.STALK),1);
                    storageCardNum = 0;
                    storageCardType = MaterialType.NULL;
                }
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //10%��1��
        if (IsRandomEventTriggered(10)) {
            Debug.Log("ƽԭ���ζ���Ч������");
            SpawnAnimal(AnimalType.GOBLIN);
        }
    }
}

public class ForestLand : Land
{
    // ɭ�ֵ���
    public ForestLand(int row, int col) {
        LandType = LandType.FOREST;
        RequiredEnergy = 2;
        Preference = 35;
        Level = 0;
        MapRow = row;
        MapCol = col;
    }

    public override void PassiveEffect() {
        // ɭ�ֵ��εı���Ч�� 2ʱ��1ľ
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("ɭ�ֵ��α���Ч������");
            AddCard(new MaterialCard(MaterialType.WOOD),1); //����һ��ľ�Ĳ��Ͽ����ƿ�
            EnergyCounter = 0; // ���ü�����
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //2������
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 2 && storageCardType == MaterialType.WOOD) {
                    ChangeLandType(LandType.JUNGLE);
                }
                break;
            case MaterialType.HAY:
                //��1�粼��
                SpawnAnimal(AnimalType.GOBLIN);
                break;
            case MaterialType.STONE:
                //��1ì
                AddCard(new WeaponCard(WeaponType.SPEAR),1);
                break;
            case MaterialType.MEAT:
                //��1׷��
                AddCard(new SkillCard(SkillType.STALK),1);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}

public class MountainLand : Land
{
    public MountainLand(int row,int col) {
        LandType = LandType.MOUNTAIN;
        RequiredEnergy = ProgressionManager.Instance != null ? 
            ProgressionManager.Instance.GetMountainRequiredEnergy() : 4;
        Preference = 21;
        Level = 1;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        // ɽ�����εı���Ч�� 4ʱ��2ʯ
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("ɽ�����α���Ч������");
            AddCard(new MaterialCard(MaterialType.STONE),2);
            EnergyCounter = 0; // ���ü�����
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //��1���
                AddSoild(1);
                break;
            case MaterialType.HAY:
                //2��ɽ���3��
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 2 && storageCardType == MaterialType.HAY) {
                    ChangeLandType(LandType.HILL);
                    DeckManager.Instance.ExtraDrawNum += 3;
                }
                break;
            case MaterialType.STONE:
                //��3��ʯ
                AddCard(new WeaponCard(WeaponType.ROLLROCK),3);
                break;
            case MaterialType.MEAT:
                //��1��Ȧ
                AddHunterarea(1);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //ÿ�غ��С������ĸ���ɽ��������*10%�ĸ��ʻ��1����
        if(IsRandomEventTriggered(10 * MapManager.Instance.CountAdjacentLandType(MapRow,MapCol,LandType.MOUNTAIN))) {
            Debug.Log("ɽ�����ζ���Ч������");
            AddEnergy(1);
        }
    }
}


public class JungleLand : Land
{
    public JungleLand(int row,int col) {
        LandType = LandType.JUNGLE;
        RequiredEnergy = 1;
        Preference = 36;
        Level = 1;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        //1ʱ��1ľ
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("���ֵ��α���Ч������");
            AddCard(new MaterialCard(MaterialType.WOOD),1); //����һ��ľ�Ĳ��Ͽ����ƿ�
            EnergyCounter = 0; // ���ü�����
        }
    }

    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //2��ľ��
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 2 && storageCardType == MaterialType.WOOD) {
                    ChangeLandType(LandType.CABIN);
                }
                break;
            case MaterialType.HAY:
                //2��1��
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 2 && storageCardType == MaterialType.HAY) {
                    SpawnAnimal(AnimalType.BEAR);
                    storageCardNum = 0;
                    storageCardType = MaterialType.NULL;
                }
                break;
            case MaterialType.STONE:
                //3��С��
                StorageIn(MaterialType.STONE);
                if (storageCardNum >= 3 && storageCardType == MaterialType.STONE) {
                    ChangeLandType(LandType.TOWN);
                }
                break;
            case MaterialType.MEAT:
                //����2��
                AddExtraDraw((int)MaterialType.MEAT,2);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}

public class WheatLand : Land
{
    public WheatLand(int row,int col) {
        LandType = LandType.WHEATLAND;
        RequiredEnergy = 4;
        Preference = 15;
        Level = 1;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        //4ʱ��2��
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("������α���Ч������");
            AddCard(new MaterialCard(MaterialType.HAY),2); //����2���ɲݲ��Ͽ����ƿ�
            EnergyCounter = 0; // ���ü�����
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //��1�ո�
                AddCard(new SkillCard(SkillType.HARVEST),1);
                break;
            case MaterialType.HAY:
                //2����3��
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 2 && storageCardType == MaterialType.HAY) {
                    AddExtraDraw((int)MaterialType.HAY,3);
                    storageCardNum = 0;
                    storageCardType = MaterialType.NULL;
                }
                break;
            case MaterialType.STONE:
                //��1���2����
                AddSoild(1);
                AddEnergy(2);
                break;
            case MaterialType.MEAT:
                //��3����
                AddEnergy(3);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //30%��1��
        if (IsRandomEventTriggered(30)) {
            Debug.Log("������ζ���Ч������");
            AddCard(new MaterialCard(MaterialType.HAY),1); //����һ���ɲݲ��Ͽ����ƿ�
        }
    }
}

public class CabinLand : Land
{
    public CabinLand(int row,int col) {
        LandType = LandType.CABIN;
        RequiredEnergy = 4;
        Preference = 10;
        Level = 1;
        MapRow = row;
        MapCol = col;
    }

    public override void PassiveEffect() {
        // ľ�ݵ��εı���Ч��
        if (EnergyCounter >= RequiredEnergy) {
            //4ʱ��1��
            Debug.Log("ľ�ݵ��α���Ч������");
            AddExtraDraw(CardType.MATERIAL);
            EnergyCounter = 0; // ���ü�����
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //��1��Ȧ
                AddHunterarea(1);
                break;
            case MaterialType.HAY:
                //��1�ո�
                AddCard(new SkillCard(SkillType.HARVEST),1);
                break;
            case MaterialType.STONE:
                //��3��ʯ
                AddCard(new WeaponCard(WeaponType.ROLLROCK),3);
                break;
            case MaterialType.MEAT:
                //4��С��
                StorageIn(MaterialType.MEAT);
                if (storageCardNum >= 4 && storageCardType == MaterialType.MEAT) {
                    ChangeLandType(LandType.TOWN);
                }
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //5%��1����Դ
        if (IsRandomEventTriggered(5)) {
            Debug.Log("ľ�ݵ��ζ���Ч������");
            AddExtraDraw(CardType.MATERIAL);
        }
    }
}

public class ThatchLand : Land
{
    public ThatchLand(int row,int col) {
        LandType = LandType.THATCH;
        RequiredEnergy = 5;
        Preference = 11;
        Level = 1;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        // é�ݵ��εı���Ч��
        if (EnergyCounter >= RequiredEnergy) {
            //5ʱ��2ľ1ʯ
            Debug.Log("é�ݵ��α���Ч������");
            AddCard(new MaterialCard(MaterialType.WOOD),2);
            AddCard(new MaterialCard(MaterialType.STONE),1);
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //��2ì
                AddCard(new WeaponCard(WeaponType.SPEAR),2);
                break;
            case MaterialType.HAY:
                //��1��
                SpawnAnimal(AnimalType.BOAR);
                break;
            case MaterialType.STONE:
                //��1�ӹ̳�1��
                AddSoild(1);
                AddExtraDraw(CardType.SKILL);
                break;
            case MaterialType.MEAT:
                //��2׷��
                AddCard(new SkillCard(SkillType.STALK),2);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //15%��1��
        if (IsRandomEventTriggered(15)) {
            Debug.Log("é�ݵ��ζ���Ч������");
            AddEnergy(1);
        }
    }
}




public class TownLand : Land
{

    bool IsExtraEffectTriggered = false;

    public TownLand(int row,int col) {
        LandType = LandType.TOWN;
        RequiredEnergy = 6;
        Preference = 9;
        Level = 2;
        MapRow = row;
        MapCol = col;
        IsExtraEffectTriggered = false;
    }
    public override void PassiveEffect() {
        // ������εı���Ч��
        if (EnergyCounter >= RequiredEnergy) {
            //6ʱ��3��
            Debug.Log("������α���Ч������");
            AddCard(new MaterialCard(MaterialType.MEAT),3); //����3������Ͽ����ƿ�
            EnergyCounter = 0; // ���ü�����
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //5���ֿ�
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 5 && storageCardType == MaterialType.WOOD) {
                    ChangeLandType(LandType.WAREHOUSE);
                }
                break;
            case MaterialType.HAY:
                //4���糵
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 4 && storageCardType == MaterialType.HAY) {
                    ChangeLandType(LandType.WINDMILL);
                }
                break;
            case MaterialType.STONE:
                //1����
                StorageIn(MaterialType.STONE);
                if (storageCardNum >= 1 && storageCardType == MaterialType.STONE) {
                    ChangeLandType(LandType.TOWER);
                }
                break;
            case MaterialType.MEAT:
                //��2ì
                AddCard(new WeaponCard(WeaponType.SPEAR),2);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //�����2��һ��
        if (!IsExtraEffectTriggered) {
            AddExtraDraw(CardType.SKILL,2);
            IsExtraEffectTriggered = true;
        }
    }
}


public class WarehouseLand : Land
{
    public WarehouseLand(int row,int col) {
        LandType = LandType.WAREHOUSE;
        RequiredEnergy = 2;
        Preference = 0;
        Level = 3;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        // �ֿ���εı���Ч��
        if (EnergyCounter >= RequiredEnergy) {
            //2ʱ��1����Դ
            Debug.Log("�ֿ���α���Ч������");
            AddExtraDraw(CardType.MATERIAL);
            EnergyCounter = 0; // ���ü�����
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //��1��ɲ�
                AddExtraDraw((int)MaterialType.HAY,1);
                break;
            case MaterialType.HAY:
                //��1��ľͷ
                AddExtraDraw((int)MaterialType.WOOD,1);
                break;
            case MaterialType.STONE:
                //��1����Դ,�ü��
                AddExtraDraw(CardType.MATERIAL);
                AddSoild(1);
                break;
            case MaterialType.MEAT:
                //��1��ʯͷ
                AddExtraDraw((int)MaterialType.STONE,1);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //8%��1����Դ
        if (IsRandomEventTriggered(8)) {
            Debug.Log("�ֿ���ζ���Ч������");
            AddExtraDraw(CardType.MATERIAL);
        }
    }
}

public class TowerLand : Land
{
    public TowerLand(int row,int col) {
        LandType = LandType.TOWER;
        RequiredEnergy = 3;
        Preference = -1;
        Level = 3;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        // �����εı���Ч��
        if (EnergyCounter >= RequiredEnergy) {
            //3ʱ���̹�
            Debug.Log("�����α���Ч������");
            AddCard(new WeaponCard(WeaponType.BOW),1);
            EnergyCounter = 0; // ���ü�����
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //�ö̹�
                AddCard(new WeaponCard(WeaponType.BOW),1);
                break;
            case MaterialType.HAY:
                //��2�ӹ�
                AddSoild(2);
                break;
            case MaterialType.STONE:
                //��2�̹�2ì
                AddCard(new WeaponCard(WeaponType.BOW),2);
                AddCard(new WeaponCard(WeaponType.SPEAR),2);
                break;
            case MaterialType.MEAT:
                //�ó�2��
                AddExtraDraw(CardType.SKILL,2);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //������Сֵ+1 (���Ч����ս��ϵͳ��ʵ��)
    }
}



public class WindmillLand : Land
{
    public WindmillLand(int row,int col) {
        LandType = LandType.WINDMILL;
        RequiredEnergy = 4;
        Preference = 24;
        Level = 3;
        MapRow = row;
        MapCol = col;

    }
    public override void PassiveEffect() {
        if (EnergyCounter >= RequiredEnergy) {
            //4ʱ��1�ո�
            Debug.Log("�糵���α���Ч������");
            AddCard(new SkillCard(SkillType.HARVEST),1);
            EnergyCounter = 0;
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //��3�ɲ�
                AddCard(new MaterialCard(MaterialType.HAY),3);
                break;
            case MaterialType.HAY:
                //2��3���ո�
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 2 && storageCardType == MaterialType.HAY) {
                    AddExtraDraw((int)SkillType.HARVEST,3);
                    storageCardNum = 0;
                    storageCardType = MaterialType.NULL;
                }
                break;
            case MaterialType.STONE:
                //��2�ո�
                AddCard(new SkillCard(SkillType.HARVEST),2);
                break;
            case MaterialType.MEAT:
                //��1�粼��
                SpawnAnimal(AnimalType.GOBLIN);
                break;
            default:
                break;
        }
    }
    public override void ExtraEffect() {
        //�ո����+1�� (���Ч�����ո����ʵ��)
    }
}

public class RuinLand : Land
{
    public RuinLand(int row,int col) {
        LandType = LandType.RUIN;
        RequiredEnergy = 10;
        Preference = -20;
        Level = 0;
        MapRow = row;
        MapCol = col;

    }
    public override void PassiveEffect() {
        if (EnergyCounter >= RequiredEnergy) {
            //10ʱ��1ʯ
            Debug.Log("������α���Ч������");
            AddCard(new MaterialCard(MaterialType.STONE),1);
            EnergyCounter = 0;
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //3��ɭ��
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 3 && storageCardType == MaterialType.WOOD) {
                    ChangeLandType(LandType.FOREST);
                }
                break;
            case MaterialType.HAY:
                //3��ƽԭ
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 3 && storageCardType == MaterialType.HAY) {
                    ChangeLandType(LandType.PLAIN);
                }
                break;
            case MaterialType.STONE:
                //2��ɽ��
                StorageIn(MaterialType.STONE);
                if (storageCardNum >= 2 && storageCardType == MaterialType.STONE) {
                    ChangeLandType(LandType.MOUNTAIN);
                }
                break;
            case MaterialType.MEAT:
                //��1׷��
                AddCard(new SkillCard(SkillType.STALK),1);
                break;
            default:
                break;
        }
    }
    public override void ExtraEffect() {
        //��
    }
}
