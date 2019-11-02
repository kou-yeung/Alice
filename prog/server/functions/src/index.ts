import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';

class ServerTime {
    // サーバー時間を取得する
    // MEMO : JavaScript では ミリ秒まで取得されるため、最後の3桁は削除しています!!
    static get current(): number {
        var str = Date.now().toString().slice(0, -3);
        return parseInt(str);
    }
}
// Guidを生成する
class Guid {
    static NewGuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}

// 乱数生成器
class Random {
    // [min,max)
    static Next(min: number = 0, max: number = Math.pow(2, 31) - 1): number {
        return Math.floor(this.NextDouble(min, max));
    }
    // [min,max)
    static NextDouble(min: number, max: number): number {
        return Math.random() * (max - min) + min;
    }
}

// Firebase へのアクセス用ヘルパー関数
export class Ref {
    static async snapshot<T>(ref: FirebaseFirestore.DocumentReference): Promise<T> {
        return await ref.get().then(snap => <T>snap.data());
    }
    static async collection<T>(ref: FirebaseFirestore.CollectionReference | FirebaseFirestore.Query): Promise<T[]> {
        return await ref.get().then(snap => {
            const array: T[] = [];
            snap.forEach(result => array.push(<T>result.data()));
            return array;
        });
    }
}

class Documents {
    uid: string;

    constructor(uid: string) {
        this.uid = uid;
    }
    player(): FirebaseFirestore.DocumentReference {
        return db.collection('player').doc(this.uid);
    }
    deck(): FirebaseFirestore.DocumentReference {
        return db.collection('deck').doc(this.uid);
    }
    unit(characterId: string): FirebaseFirestore.DocumentReference {
        return this.units().doc(characterId)
    }
    skill(id: string): FirebaseFirestore.DocumentReference {
        return this.skills().doc(id);
    }
    chest(uniq: string): FirebaseFirestore.DocumentReference {
        return this.chests().doc(uniq);
    }
    // 指定ランクの登録データ取得
    colosseum(rank: number): FirebaseFirestore.DocumentReference {
        return this.colosseumGroups().collection(rank.toString()).doc(this.uid);
    }
    colosseumGroups(): FirebaseFirestore.DocumentReference {
        return db.collection('colosseum').doc('groups');
    }
    // Collection
    units(): FirebaseFirestore.CollectionReference {
        return this.player().collection("units");
    }
    unitsWith(ids: string[]): FirebaseFirestore.Query {
        let query : any = undefined;
        for (const id of ids.filter(v => v)) {
            if (!query) query = this.units().where('characterId', '==', id);
            else query.where('characterId', '==', id);
        }
        return query;
    }
    skills(): FirebaseFirestore.CollectionReference {
        return this.player().collection("skills");
    }
    chests(): FirebaseFirestore.CollectionReference {
        return this.player().collection("chests");
    }

    // コロシアムからランダム抽選
    colosseumRandom(rank: number, count: number, max: number): FirebaseFirestore.Query {
        let query: any = undefined;
        for (let i = 0; i < count; ++i) {
            const index = Random.Next(0, max);
            if (!query) query = this.colosseumGroups().collection(rank.toString()).where('index', '==', index);
            else query.where('index', '==', index);
        }
        return query;
    }
}

//==============
// Entity
//==============
// Player情報
class  Player {
    name: string = "";       // ユーザ名
    alarm: number = 0;  // アラーム(時間短縮アイテム
    rank: number = 0;        // プレイヤーランキング
    ads: number = 0;         // 残り広告使用回数
    token: string = "";    // 認証トークン
    stamp: number = 0;      // 最後ログインの日付
    loginCount: number = 0;  // 累計ログイン日数
    totalBattleCount: number = 0;    // 累計バトル回数
    todayBattleCount: number = 0;    // 本日バトルした回数
    todayWinCount: number = 0;       // 本日勝利した回数
}

// スキルx1
class UserSkill {
    id: string = "";
    count: number = 0;
}
// ユニットx1
class UserUnit {
    characterId: string = "";
    skill: string[] = [];
    exp: number = 0;
}
// 宝箱
class UserChest {
    uniq: string = "";    // アクセス用ID
    created: number = 0;    // 生成時間:変更しない
    start: number = 0; // 開始時間:Adsより変更される可能性ある
    end: number = 0;   // 終了時間:Adsより変更される可能性ある
    rate: number = 0;   // レアリティ
}

class UserDeck {
    ids: string[] = [];
}
// 更新されたデータ
class Modified {
    player :Player[] = [];                 // プレイヤー情報
    skill: UserSkill[] = [];        // スキルデータ
    unit: UserUnit[] = [];          // ユニットデータ
    chest: UserChest[] = [];        // 宝箱データ
    remove: UserChest[] = [];       // 削除した宝箱
}

/**
 * ログインボーナスチェック
 * return (null)ログインボーナス発生しない 
 * @param context
 * @param player
 */
function LoginBonus(player: Player) {

    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const stamp = player.stamp || 0;
    if (stamp >= today.getTime()) return null;  // 日付同じならまだログインボーナス貰えない

    // タイムスタンプ更新
    player.stamp = today.getTime();
    // 累計ログイン数 + 1
    player.loginCount = (player.loginCount || 0) + 1;

    //========================
    // 以下ログインボーナス付与
    //========================
    // 広告使用回数を回復
    player.ads = 15;
    const bonus = { alarm: 3, rankup : false };

    player.alarm = (player.alarm || 0) + bonus.alarm;
    // 本日のバトル回数が１０回以上のみチェック
    if (player.todayBattleCount >= 10) {
        const count = player.todayBattleCount || 0;
        const win = player.todayWinCount || 0;

        // 勝率が 65% 以上ならランクアップ
        if (win / count >= 0.65) {
            player.rank = (player.rank || 0) + 1;
            bonus.rankup = true;
        }
    }
    // 本日のバトル回数をリセット
    player.todayBattleCount = 0;
    player.todayWinCount = 0;

    // ログインボーナスを返す
    return bonus;
}

class HomeRecv {
    svtime: number = 0;
    player?: Player;
    bonus: any;
    deck?: UserDeck;
    units: UserUnit[] = [];
    skills: UserSkill[] = [];
    chests: UserChest[] = [];
}

//======================

// init admin to use admin function.
admin.initializeApp();
const db = admin.firestore();

/**
 *
 * 新規ユーザ作成時
 */
exports.onCreate = functions.auth.user().onCreate(async (user) => {
    const doc = new Documents(user.uid);
    const batch = db.batch();

    // Player情報
    const player = { name: "ゲスト", token: Guid.NewGuid(), totalBattleCount: 0, rank: 0 };
    batch.set(doc.player(), player);

    // 初期ユニット情報
    const unit = { characterId: "Character_001_001", exp:0, skill: [] };
    batch.set(doc.unit(unit.characterId), unit);

    // 初期デッキ設定
    const deck = { ids: [unit.characterId, "", "", ""] };
    batch.set(doc.deck(), deck);

    // コミット
    await batch.commit();
});

///**
// * proto:Temp:テンプレート
// */
//exports.Temp = functions.https.onCall(async (data, context) => {
//    const uid = context.auth!.uid;
//    const c2s: TempSend = JSON.parse(data); // c2s
//    const s2c = new TempRecv();             // s2c
//    return JSON.stringify(s2c);
//});

/**
 * proto:ping
 */
exports.ping = functions.https.onCall((data, context) => {
    return JSON.stringify({ data:data, uid: context.auth!.uid });
});

/**
 * proto:Home:画面の情報を取得する
 */ 
exports.Home = functions.https.onCall(async (data, context) => {

    const doc = new Documents(context.auth!.uid);

    // プレイヤー情報取得
    const player = await Ref.snapshot<Player>(doc.player());

    const s2c = new HomeRecv();

    s2c.svtime = ServerTime.current;
    // ログインボーナスチェック
    s2c.bonus = await LoginBonus(player);
    if (s2c.bonus) await doc.player().set(player);

    s2c.player = player;
    s2c.deck = await Ref.snapshot<UserDeck>(doc.deck());
    s2c.units = await Ref.collection<UserUnit>(doc.units());
    s2c.skills = await Ref.collection<UserSkill>(doc.skills());
    s2c.chests = await Ref.collection<UserChest>(doc.chests().orderBy('created'));

    return JSON.stringify(s2c);
});

// バトル開始: c2s
class BattleStartSend {
    player!: Player;       // プレイヤー情報
    units!: UserUnit[];    // バトルに使用するユニット
    deck!: UserDeck;       // デッキ情報

    edited!: UserUnit[];  // 情報を更新したユニット

    // 推薦敵ユニット:サーバ上にヒットした相手がなければこちらを使って対戦させます
    recommendUnits!: UserUnit[];    // 推薦的ユニット
    recommendDeck!: UserDeck;     // 配置
}
// バトル開始: s2c
class BattleStartRecv {
    seed: number = 0;
    type: number = 0;   // バトルタイプ
    names!: string[];   // 名前
    // 味方のユニット情報
    playerUnit!: UserUnit[];
    playerDeck!: UserDeck;
    // 相手のユニット情報
    enemyUnit!: UserUnit[];
    enemyDeck!: UserDeck;
}

class Colosseum {
    index: number = 0;  // 登録番号
    uid: string = "";   // UID
    name: string = "";  // 名前
    unitJson: string = "";  // ユニットデータのJSON文字列
    deckJson: string = "";  // デッキデータのJSON文字列
}

class Groups {
    counter: any; // 該当グループ
}
/**
 * proto:Battle:バトル開始
 */
exports.Battle = functions.https.onCall(async (data, context) => {
    const doc = new Documents(context.auth!.uid);
    const c2s: BattleStartSend = JSON.parse(data);
    const s2c = new BattleStartRecv();
    const batch = db.batch();

    const player = await Ref.snapshot<Player>(doc.player());
    // プレイヤー名が変更されたら更新する
    if (c2s.player.name !== player.name) {
        player.name = c2s.player.name;
        batch.update(doc.player(), { name: player.name });
    }
    // デッキ更新
    batch.set(doc.deck(), c2s.deck);
    // 情報更新したユニットを同期する( スキルのみ
    for(const unit of c2s.edited)
    {
        batch.update(doc.unit(unit.characterId), { skill: unit.skill });
    }

    // コロシアムグループのデータを取得する
    const groups = await Ref.snapshot<Groups>(doc.colosseumGroups());

    // 自分のデータをコロシアムに登録or更新
    let colosseum = await Ref.snapshot<Colosseum>(doc.colosseum(player.rank));
    if (colosseum) {
        colosseum.name = player.name;
        colosseum.unitJson = JSON.stringify(c2s.units);
        colosseum.deckJson = JSON.stringify(c2s.deck);
        batch.set(doc.colosseum(player.rank), colosseum);
    } else {
        const count = groups.counter[player.rank] | 0;
        // なかったので登録する
        colosseum = {
            index: count,
            uid: context.auth!.uid,
            name: player.name,
            unitJson: JSON.stringify(c2s.units),
            deckJson: JSON.stringify(c2s.deck),
        };
        batch.set(doc.colosseum(player.rank), colosseum);
        // 所属グループのデータ数をカウントアップする
        groups.counter[player.rank] = count + 1;    // インクリメント
        batch.set(doc.colosseumGroups(), groups);
    }
    // 一旦同期をとる
    await batch.commit();

    s2c.seed = Random.Next();
    // プレイヤーユニット
    s2c.playerUnit = c2s.units;
    s2c.playerDeck = c2s.deck;

    // 同じランクのユーザランダム取得
    const max = groups.counter[player.rank];
    let enemys = await Ref.collection<Colosseum>(doc.colosseumRandom(player.rank, 5, max));
    enemys = enemys.filter(v => v.uid != context.auth!.uid); // 自分を除く

    // HACK : １０人以下の場合、おすすめ敵を入れる
    if (max <= 10) {
        const recommend = new Colosseum();
        recommend.name = 'ゲスト';
        recommend.unitJson = JSON.stringify(c2s.recommendUnits);
        recommend.deckJson = JSON.stringify(c2s.recommendDeck);
        enemys.push(recommend);
    }

    // 候補から抽選
    const enemy = enemys[Random.Next(0, enemys.length)];
    s2c.enemyUnit = JSON.parse(enemy.unitJson);
    s2c.enemyDeck = JSON.parse(enemy.deckJson);

    // 名前を設定する
    s2c.names = [player.name, enemy.name];

    return JSON.stringify(s2c);
});


export type dataUpdate<T> = Partial<T>;
// c2s
class GameSetSend {
    ID: string = "";
    result: any;
}
// s2c
class GameSetRecv {
    modified: Modified = new Modified();
}
/**
 * proto:GameSet:試合終了
 */
exports.GameSet = functions.https.onCall(async (data, context) => {

    const doc = new Documents(context.auth!.uid);
    const Win = 1;

    const c2s: GameSetSend = JSON.parse(data);
    const s2c: GameSetRecv = new GameSetRecv();

    // プレイヤーのバトル回数更新
    const player = await Ref.snapshot<Player>(doc.player());
    const batch = db.batch();

    player.totalBattleCount += 1;   // 累計   
    player.todayBattleCount += 1;   // 本日
    if (c2s.result === Win) {
        player.todayWinCount += 1;
    }
    batch.set(doc.player(), player);
    s2c.modified.player = [player];

    // デッキにセットしたユニットに経験値を与える
    const deck = await Ref.snapshot<UserDeck>(doc.deck());
    const units = await Ref.collection<UserUnit>(doc.unitsWith(deck.ids));
    s2c.modified.unit = [];
    for (const unit of units) {
        unit.exp += 1;  // 経験値付与する
        batch.update(doc.unit(unit.characterId), { exp: unit.exp });
        s2c.modified.unit.push(unit);
    }

    // 宝箱を追加します
    const chests = await Ref.collection<UserChest>(doc.chests());
    if (chests.length < 3) {
        const start = ServerTime.current;
        const end = start + (15 * 60);

        const chest = {
            uniq: Guid.NewGuid(),
            created: ServerTime.current,
            start: start,
            end: end,
            rate: 2
        }
        batch.set(doc.chest(chest.uniq), chest);
        s2c.modified.chest = [chest];
    }

    // 同期
    await batch.commit();

    return JSON.stringify(s2c);
});


// 広告を観ました : c2s
class AdsSend {
    token:string="";        // 認証トークン
    chest!:UserChest;       // 対象
}
// 広告を観ました : s2c
class AdsRecv {
    modified: Modified = new Modified();
}

/**
 * proto:Ads:広告終了
 */
exports.Ads = functions.https.onCall(async (data, context) => {
    const doc = new Documents(context.auth!.uid);
    const c2s: AdsSend = JSON.parse(data);
    const s2c = new AdsRecv();

    const player = await Ref.snapshot<Player>(doc.player());

    // トークンが無効、あるいは 回数がない
    if (c2s.token !== player.token || player.ads <= 0) {
        return JSON.stringify(s2c);
    }

    const chest = await Ref.snapshot<UserChest>(doc.chest(c2s.chest.uniq));
    chest.start -= (10 * 60);
    chest.end -= (10 * 60);

    const batch = db.batch();
    batch.set(doc.chest(c2s.chest.uniq), chest);

    player.token = Guid.NewGuid();
    player.ads -= 1;    // 回数減らす

    batch.set(doc.player(), player);

    await batch.commit();

    s2c.modified.chest = [chest];
    s2c.modified.player = [player];

    return JSON.stringify(s2c);

});


// 宝箱を開く: cl -> sv
class ChestSend {
    chest!: UserChest;          // 宝箱
    useItem: boolean = false;   // アイテム使用
}

// 宝箱を開く: sv -> cl
class ChestRecv {
    modified: Modified = new Modified();       // 更新したデータ
}

/**
 * proto:Chest:テンプレート
 */
exports.Chest = functions.https.onCall(async (data, context) => {
    const doc = new Documents(context.auth!.uid);
    const c2s: ChestSend = JSON.parse(data); // c2s
    const s2c = new ChestRecv();             // s2c

    if (c2s.chest.end > ServerTime.current && !c2s.useItem) {
        return JSON.stringify(s2c);
    }

    const batch = db.batch();
    // 宝箱を消す
    batch.delete(doc.chest(c2s.chest.uniq));
    s2c.modified.remove = [c2s.chest];

    //// MEMO : 現在適当に1/2の確率で[Unit][Skill]分岐
    //if (random.Next() % 2 == 0 && db.units.Length < MasterData.characters.Length) {
    //    var character = MasterData.characters;
    //    var lots = character.Where(v => !Array.Exists(db.units, u => u.characterId == v.ID)).ToArray();
    //    var id = lots[random.Next(0, lots.Length)].ID;
    //    var add = new UserUnit();
    //    add.characterId = id;
    //    add.skill = new string[0];
    //    db.units = db.units.Concat(new [] { add }).ToArray();
    //    s2c.modified.unit = new [] { add };
    //}
    //else
    {
        // スキル
        //var skill = MasterData.skills;
        const id = "Skill_999_002";//skill[random.Next(0, skill.Length)];

        let skill = await Ref.snapshot<UserSkill>(doc.skill(id));
        // なければ生成する
        if (!skill) skill = { id: id, count: 0 };
        // 数を + 1
        skill.count += 1;
        // 更新
        batch.set(doc.skill(id), skill);
        s2c.modified.skill = [skill];
    }
    await batch.commit();
    return JSON.stringify(s2c);
});
