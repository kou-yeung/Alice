import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';
import * as rp from 'request-promise';

// 定数
class Const {
    static PLAYER_RANK_MAX: number = 10;
    static AdsRewardTimeSecond: number = (10 * 60);     // 10分
    static AlarmTimeSecond: number = (15 * 60);         // 15分
    static LoginBonusAlarm: number = 2;                 // アラーム数
    static LoginBonusAds: number = 15;                  // 観れる広告回数
    static RoomOfFloot: number = 100;                   // １フロアのルーム数
    static FLOOR_MAX: number = 1000;                   // フロア数数
    static ROOM_MAX: number = Const.RoomOfFloot * Const.FLOOR_MAX;// ルームの最大数
    static APP_VERSION: string = "0.0.2";               // アプリバージョン
}

// 宝箱のランクから必要の時間を算出します
function RankToTimeSecond(rank: number) {
    return ((rank + 1) * Math.floor((rank + 1) / 2) * 5 + 5) * 60;
}

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
    admin(): FirebaseFirestore.DocumentReference {
        return db.collection('admin').doc(this.uid);
    }
    configs(): FirebaseFirestore.DocumentReference {
        return db.collection('admin').doc("configs");
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
    product(id: string, platform: string): FirebaseFirestore.DocumentReference {
        return this.products().doc(id + ':' + platform);
    }
    purchase(transaction: string): FirebaseFirestore.DocumentReference {
        return this.purchases().doc(transaction);
    }
    // 指定ランクの登録データ取得
    colosseum(rank: number, index: number): FirebaseFirestore.DocumentReference {
        return this.colosseumGroups().collection(rank.toString()).doc(index.toString());
    }
    colosseumGroups(): FirebaseFirestore.DocumentReference {
        return db.collection('colosseum').doc('groups');
    }
    // シャドウ情報
    shadowInfo(): FirebaseFirestore.DocumentReference {
        return db.collection('shadow').doc('info');
    }
    // シャドウのフロア:ランダムでルームID一覧を保持するデータ
    shadowFloor(floor: number): FirebaseFirestore.DocumentReference {
        return db.collection('shadow').doc(floor.toString());
    }
    // シャドウのルーム取得
    room(id: number): FirebaseFirestore.DocumentReference {
        return db.collection('rooms').doc(id.toString());
    }
    // 自分の最後登録したシャドウ情報
    shadowSelf(): FirebaseFirestore.DocumentReference {
        return this.player().collection('shadow').doc('self');
    }
    // マスタデータ:スキル
    masterdataSkill(lv: number): FirebaseFirestore.DocumentReference {
        return db.collection('masterdata').doc('skills' + lv.toString());
    }
    // マスタデータ:キャラクタ
    masterdataCharacter(lv: number): FirebaseFirestore.DocumentReference {
        return db.collection('masterdata').doc('characters' + lv.toString());
    }
    // マスタデータ:キャラクタ
    masterdataProduct(): FirebaseFirestore.DocumentReference {
        return db.collection('masterdata').doc('product');
    }
    // Collection
    units(): FirebaseFirestore.CollectionReference {
        return this.player().collection("units");
    }
    skills(): FirebaseFirestore.CollectionReference {
        return this.player().collection("skills");
    }
    chests(): FirebaseFirestore.CollectionReference {
        return this.player().collection("chests");
    }
    // 課金アイテム
    products(): FirebaseFirestore.CollectionReference {
        return db.collection("products");
    }
    // 購入履歴
    purchases(): FirebaseFirestore.CollectionReference {
        return db.collection("purchases");
    }
    // ルームID + 指定時間移行のバトルを指定件数のクエリを生成する
    shadowEnemies(roomid: number, beforeTime: number, count: number): FirebaseFirestore.Query {
        const collection = this.player().collection("shadow").doc(roomid.toString()).collection('enemies');
        return collection.where('created', '>=', beforeTime).orderBy('created', 'desc').limit(count);
    }

    // 特別ヘルパー
    // コロシアムからランダム取得する
    async colosseumRandom(rank: number, count: number, max: number): Promise<Colosseum[]> {
        const res: Colosseum[] = [];
        for (let i = 0; i < count; ++i) {
            const index = Random.Next(0, max);
            const colosseum = await Ref.snapshot<Colosseum>(this.colosseum(rank, index));
            if (colosseum !== undefined) res.push(colosseum);
        }
        return res;
    }
}

class Crypto {
    // 復号する
    static Decrypt(cipher: string): string {
        return Buffer.from(cipher, 'base64').toString();
        //return cipher;
    }
    // 暗号化する
    static Encrypt(text: string): string {
        return Buffer.from(text).toString('base64');
        //return text;
    }
}
class Proto {
    static parse<T>(data: string): T {
        return <T>JSON.parse(Crypto.Decrypt(data));
    }
    static stringify<T>(data: T): string {
        return Crypto.Encrypt(JSON.stringify(data));
    }
}

class Configs {
    maintain: boolean = false;
    app_version: string = "";
}

async function CommonCheck(data: any, context: functions.https.CallableContext) {
    //return "メンテナンス中です";
    const doc = new Documents(context.auth!.uid);
    var configs = await Ref.snapshot<Configs>(doc.configs());

    // メンテナンス中で管理者ではない場合
    if (configs.maintain/* && await Ref.snapshot(doc.admin()) == undefined*/) {
        return "メンテナンス中です";
    }
    return null;
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

    colosseums: any;      // コロシアムにランクと番号のマップ colosseums[rank] = index
    roomid: number = -1;    // 最後に生成したシャドウ番号    

    tutorialFlag: number = 0;   // チュートリアルフラグ
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
    rare: number = 0;
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

// ルーム
class Room {
    uid: string = "";       // 現在このルームを使用中のUID
    seed: number = 0;       // シャドウバトルを挑戦した側のみ発行します
    created: number = 0;    // 生成時間
    name: string = "";      // 名前
    unitJson: string = "";  // ユニットデータのJSON文字列
    deckJson: string = "";  // デッキデータのJSON文字列
}

// 更新されたデータ
class Modified {
    player :Player[] = [];          // プレイヤー情報
    skill: UserSkill[] = [];        // スキルデータ
    unit: UserUnit[] = [];          // ユニットデータ
    chest: UserChest[] = [];        // 宝箱データ
    remove: UserChest[] = [];       // 削除した宝箱
    appVersion: string = Const.APP_VERSION;
}

class Message {
    error?: string;
    warning?: string;

    static Error(str: string): Message {
        return { error: str };
    }
    static Warning(str: string): Message {
        return { warning: str };
    }
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
    player.ads = Const.LoginBonusAds;
    const bonus = { alarm: Const.LoginBonusAlarm, rankup: false };

    player.alarm = (player.alarm || 0) + bonus.alarm;
    // 本日のバトル回数が１０回以上のみチェック
    if (player.todayBattleCount >= 10) {
        const count = player.todayBattleCount || 0;
        const win = player.todayWinCount || 0;

        // 勝率が 65% 以上ならランクアップ
        if (win / count >= 0.65) {
            player.rank = Math.min(Const.PLAYER_RANK_MAX - 1, (player.rank || 0) + 1);
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
    appVersion: string = Const.APP_VERSION;
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
    const player = { name: "", token: Guid.NewGuid(), totalBattleCount: 0, rank: 0, tutorialFlag: 0 };
    batch.set(doc.player(), player);

    // 初期ユニット抽選:レアリティ0の中にランダム
    const mst = await Ref.snapshot<MasterDataIds>(doc.masterdataCharacter(0));
    const id = mst.ids[Random.Next(0, mst.ids.length)];

    // 初期ユニット情報
    const unit = { characterId: id, exp: 0, skill: [] };
    batch.set(doc.unit(unit.characterId), unit);

    // 初期デッキ設定
    const deck = { ids: [unit.characterId, "", "", ""] };
    batch.set(doc.deck(), deck);

    // コミット
    await batch.commit();
});

// 管理者:ルームID登録時に使用します
exports.GenRoomIds = functions.https.onCall(async(data, context) => {
    const doc = new Documents(context.auth!.uid);

    // 権限をチェックする
    if (await Ref.snapshot(doc.admin()) == undefined) {
        return Proto.stringify(Message.Warning('管理者権限レベルが低い'));
    }
    const ids = [];

    let batch = db.batch();

    // 10000個IDを生成する
    for (let i = 0; i < Const.ROOM_MAX; ++i) ids.push(i);
    for (let x = 0; x < Const.FLOOR_MAX; ++x) {
        const rooms = [];
        for (let y = 0; y < Const.RoomOfFloot; ++y) {
            const index = Random.Next(0, ids.length);
            rooms.push(index);
            ids.slice(index, 1);
        }
        batch.set(doc.shadowFloor(x), { ids: rooms });
        if (x % 200 == 0) {
            await batch.commit();
            batch = db.batch();
        }
    }

    batch.set(doc.shadowInfo(), { counter: 0 });
    await batch.commit();

    return Proto.stringify(Message.Warning('登録完了しました'));
});

class MasterDataSkill {
    rare: number = 0;
    id: string = "";
}
class MasterDataCharacter {
    rare: number = 0;
    id: string = "";
}
class MasterDataProduct {
    id: string = "";
    platform: string = "";
    alarm: number = 0;
    bonus: number = 0;
    admin: boolean = true;
}
class MasterDataIds {
    ids: string[] = [];
}
class MasterDataSend {
    skills?: MasterDataSkill[];
    characters?: MasterDataCharacter[];
    products?: MasterDataProduct[];
}
// 管理者:マスタデータ登録
exports.MasterData = functions.https.onCall(async (data, context) => {
    const doc = new Documents(context.auth!.uid);
    const c2s = Proto.parse<MasterDataSend>(data);

    // 権限をチェックする
    if (await Ref.snapshot(doc.admin()) == undefined) {
        return Proto.stringify(Message.Warning('管理者権限レベルが低い'));
    }
    const batch = db.batch();

    // スキル登録
    if (c2s.skills) {
        const map = new Map();
        for (let i = 0; i < c2s.skills.length; ++i) {
            const skill = c2s.skills[i]; 
            if (!map.has(skill.rare)) map.set(skill.rare, []);
            map.get(skill.rare).push(skill.id);
        }
        for (const [k, v] of map) {
            batch.set(doc.masterdataSkill(k), { ids: v });
        }
    }
    // キャラ登録
    if (c2s.characters) {
        const map = new Map();
        for (let i = 0; i < c2s.characters.length; ++i) {
            const character = c2s.characters[i];
            if (!map.has(character.rare)) map.set(character.rare, []);
            map.get(character.rare).push(character.id);
        }
        for (const [k, v] of map) {
            batch.set(doc.masterdataCharacter(k), { ids: v });
        }
    }
    // 課金アイテム
    if (c2s.products) {
        for (const product of c2s.products) {
            batch.set(doc.product(product.id, product.platform),
                {
                    alarm: product.alarm,
                    bonus: product.bonus,
                    admin: product.admin
                });
        }
    }
    await batch.commit();
    return Proto.stringify(Message.Warning('登録完了しました'));
});


class AdminCommandSend {
    command: string = "";
    param: string[] = [];
}

class AdminCommandRecv {
    modified: Modified = new Modified();
}

// 管理者:チートコマンド
exports.AdminCommand = functions.https.onCall(async (data, context) => {
    const doc = new Documents(context.auth!.uid);
    const c2s = Proto.parse<AdminCommandSend>(data);

    // 権限をチェックする
    if (await Ref.snapshot(doc.admin()) == undefined) {
        return Proto.stringify(Message.Warning('管理者権限レベルが低い'));
    }

    const s2c = new AdminCommandRecv();
    const batch = db.batch();

    switch (c2s.command) {
        case "AddCharacter":
            {
                for (let i = 0; i < c2s.param.length; i += 2)
                {
                    let id = c2s.param[i];
                    let rare = parseInt(c2s.param[i + 1]);
                    let add: UserUnit = {
                        characterId: id,
                        skill: [],
                        exp: 0,
                        rare: rare
                    }
                    batch.set(doc.unit(add.characterId), add);
                    s2c.modified.unit.push(add);
                }
            }
            break;

        case "AddSkill":
            {
                for (const id of c2s.param) {
                    // スキル
                    let skill = await Ref.snapshot<UserSkill>(doc.skill(id));
                    // なければ生成する
                    if (!skill) skill = { id: id, count: 0 };
                    // 数を + 1
                    skill.count += 1;
                    // 更新
                    batch.set(doc.skill(id), skill);
                    s2c.modified.skill.push(skill);
                }
            } break;
    }
    await batch.commit();
    return Proto.stringify(s2c);
});

///**
// * proto:Temp:テンプレート
// */
//exports.Temp = functions.https.onCall(async (data, context) => {
//    const doc = new Documents(context.auth!.uid);
//    const c2s = Proto.parse<TempSend>(data); // c2s
//    const s2c = new TempRecv();             // s2c
//    return Proto.stringify(s2c);
//});

/**
 * proto:ping
 */
exports.ping = functions.https.onCall((data, context) => {
    return Proto.stringify({ data:data, uid: context.auth!.uid });
});

/**
 * proto:Home:画面の情報を取得する
 */ 
exports.Home = functions.https.onCall(async (data, context) => {

    // 基本のチェックを行う
    var error = await CommonCheck(data, context);
    if (error) return Proto.stringify(Message.Error(error));

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

    return Proto.stringify(s2c);
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
    type: number = 0;   // バトルタイプ:0 通常 1:シャドウ
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
    counter: any = []; // 該当グループ
}
/**
 * proto:Battle:バトル開始
 */
exports.Battle = functions.https.onCall(async (data, context) => {
    // 基本のチェックを行う
    var error = await CommonCheck(data, context);
    if (error) return Proto.stringify(Message.Error(error));

    const doc = new Documents(context.auth!.uid);

    const c2s = Proto.parse<BattleStartSend>(data);
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
    const groups = await Ref.snapshot<Groups>(doc.colosseumGroups()) || { counter: [] };

    // 自分のデータをコロシアムに登録or更新
    player.colosseums = player.colosseums || {};
    if (player.colosseums[player.rank] != undefined) {
        const colosseum = await Ref.snapshot<Colosseum>(doc.colosseum(player.rank, player.colosseums[player.rank]));
        colosseum.name = player.name;
        colosseum.unitJson = JSON.stringify(c2s.units);
        colosseum.deckJson = JSON.stringify(c2s.deck);
        batch.set(doc.colosseum(player.rank, player.colosseums[player.rank]), colosseum);
    } else {
        const count = groups.counter[player.rank] || 0;
        // なかったので登録する
        let colosseum : Colosseum = {
            index: count,
            uid: doc.uid,
            name: player.name,
            unitJson: JSON.stringify(c2s.units),
            deckJson: JSON.stringify(c2s.deck),
        };

        batch.set(doc.colosseum(player.rank, colosseum.index), colosseum);
        // 所属グループのデータ数をカウントアップする
        groups.counter[player.rank] = count + 1;    // インクリメント

        /// HACK : 本来は必要ないが、テストでランク設定された場合、null ができてしまうための対応です
        for (var i = 0; i < groups.counter.length; i++) {
            if (groups.counter[i] == null) groups.counter[i] = 0;
        }
        batch.set(doc.colosseumGroups(), groups);
        // 自分がこのランクに登録した番号を記録する
        player.colosseums[player.rank] = colosseum.index;
        batch.update(doc.player(), { colosseums: player.colosseums });
    }
    // 一旦同期をとる
    await batch.commit();

    s2c.seed = Random.Next();
    // プレイヤーユニット
    s2c.playerUnit = c2s.units;
    s2c.playerDeck = c2s.deck;


    // 同じランクのユーザランダム取得
    const max = groups.counter[player.rank];
    let enemies = await doc.colosseumRandom(player.rank, 5, max);
    enemies = enemies.filter(v => v.uid != doc.uid); // 自分を除く

    // HACK : １０人以下の場合、おすすめ敵を入れる
    if (max <= 10) {
        const recommend = new Colosseum();
        recommend.name = 'ゲスト';
        recommend.unitJson = JSON.stringify(c2s.recommendUnits);
        recommend.deckJson = JSON.stringify(c2s.recommendDeck);
        enemies.push(recommend);
    }

    // HACK : 初めの２０バトルはおすすめユニットのみ出現します
    if (player.totalBattleCount <= 20) {
        const name = enemies[Random.Next(0, enemies.length)].name;
        const recommend = new Colosseum();
        recommend.name = name;  // 名前を借用する
        recommend.unitJson = JSON.stringify(c2s.recommendUnits);
        recommend.deckJson = JSON.stringify(c2s.recommendDeck);
        enemies = [recommend];
    }

    // 候補から抽選
    const enemy = enemies[Random.Next(0, enemies.length)];
    s2c.enemyUnit = JSON.parse(enemy.unitJson);
    s2c.enemyDeck = JSON.parse(enemy.deckJson);

    // 名前を設定する
    s2c.names = [player.name, enemy.name];

    return Proto.stringify(s2c);
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
    alarm: number = 0;
}
/**
 * proto:GameSet:試合終了
 */
exports.GameSet = functions.https.onCall(async (data, context) => {

    const doc = new Documents(context.auth!.uid);
    const Win = 1;
    const c2s = Proto.parse<GameSetSend>(data);
    const s2c: GameSetRecv = new GameSetRecv();

    // プレイヤーのバトル回数更新
    const player = await Ref.snapshot<Player>(doc.player());
    const batch = db.batch();

    player.totalBattleCount += 1;   // 累計   
    player.todayBattleCount += 1;   // 本日
    if (c2s.result === Win) {
        player.todayWinCount += 1;
    }

    // デッキにセットしたユニットに経験値を与える
    const deck = await Ref.snapshot<UserDeck>(doc.deck());
    s2c.modified.unit = [];
    for (const id of deck.ids.filter(v => v)) {
        const unit = await Ref.snapshot<UserUnit>(doc.unit(id));
        unit.exp += 1;  // 経験値付与する
        batch.update(doc.unit(unit.characterId), { exp: unit.exp });
        s2c.modified.unit.push(unit);
    }

    // レアリティの抽選テーブルを作る
    const rares = [];
    for (let i = player.rank - 3; i < player.rank + 3; ++i) {
        rares.push(Math.floor(Math.max(0, i) / 3));
    }

    // 勝利した場合、確率でアラームドロップ
    if (c2s.result === Win && Random.Next(0, 50) === 0) {
        s2c.alarm = 1;
        player.alarm += s2c.alarm;
    }

    const rare = rares[Random.Next(0, rares.length)];
    
    // 宝箱を追加します
    const chests = await Ref.collection<UserChest>(doc.chests());
    if (chests.length < 3) {
        const start = ServerTime.current;
        const end = start + RankToTimeSecond(rare);

        const chest = {
            uniq: Guid.NewGuid(),
            created: ServerTime.current,
            start: start,
            end: end,
            rate: rare,
        }
        batch.set(doc.chest(chest.uniq), chest);
        s2c.modified.chest = [chest];
    }

    batch.set(doc.player(), player);
    s2c.modified.player = [player];

    // 同期
    await batch.commit();
    return Proto.stringify(s2c);
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
    const c2s = Proto.parse<AdsSend>(data);
    const s2c = new AdsRecv();

    const player = await Ref.snapshot<Player>(doc.player());

    // トークンが無効、あるいは 回数がない
    if (c2s.token !== player.token) {
        return Proto.stringify(Message.Error("トークンが無効です"));
    }
    if(player.ads <= 0) {
        return Proto.stringify(Message.Warning("一日の広告使用回数制限を越えました"));
    }

    const chest = await Ref.snapshot<UserChest>(doc.chest(c2s.chest.uniq));
    chest.start -= Const.AdsRewardTimeSecond;
    chest.end -= Const.AdsRewardTimeSecond;

    const batch = db.batch();
    batch.set(doc.chest(c2s.chest.uniq), chest);

    player.token = Guid.NewGuid();
    player.ads -= 1;    // 回数減らす

    batch.set(doc.player(), player);

    await batch.commit();
    
    s2c.modified.chest = [chest];
    s2c.modified.player = [player];

    return Proto.stringify(s2c);

});


// 宝箱を開く: cl -> sv
class ChestSend {
    chest!: UserChest;          // 宝箱
}

// 宝箱を開く: sv -> cl
class ChestRecv {
    modified: Modified = new Modified();       // 更新したデータ
}

class ChestLots {
    type: string = '';   // skill / character
    id: string = '';     // id
}

exports.ChestTest = functions.https.onCall(async (data, context) => {
    const doc = new Documents(context.auth!.uid);

    const skillIds = await Ref.snapshot<MasterDataIds>(doc.masterdataSkill(0));
    const characterIds = await Ref.snapshot<MasterDataIds>(doc.masterdataCharacter(0));    // マスタデータからキャラID一覧取得
    const units = await Ref.collection<UserUnit>(doc.units().where('rare', '==', 0));      // 自分の所持キャラ一覧取得

    const lots: ChestLots[] = [];
    // スキルの抽選一覧
    for (const skill of skillIds.ids) {
        const lot = new ChestLots();
        lot.id = skill;
        lot.type = 'skill';
        lots.push(lot);
    }
    // キャラの抽選一覧
    const hasUnits = units.map(v => v.characterId);
    for (const character of characterIds.ids) {
        if (hasUnits.includes(character)) continue; // すでに持ってるため抽選から外す
        const lot = new ChestLots();
        lot.id = character;
        lot.type = 'character';
        lots.push(lot);
    }
    // 抽選
    //const res = lots[Random.Next(0, lots.length)];
    return Proto.stringify(Message.Error(JSON.stringify(lots)));
});


/**
 * proto:Chest:宝箱を開く
 */
exports.Chest = functions.https.onCall(async (data, context) => {
    const doc = new Documents(context.auth!.uid);
    const c2s = Proto.parse<ChestSend>(data);
    const s2c = new ChestRecv();             // s2c
    const batch = db.batch();

    const player = await Ref.snapshot<Player>(doc.player());
    const chest = await Ref.snapshot<UserChest>(doc.chest(c2s.chest.uniq));

    if (chest == undefined) {
        return Proto.stringify(Message.Error("指定の宝箱が存在しません"));
    }

    // 残り時間
    const remain = Math.max(0, chest.end - ServerTime.current);

    if (remain > 0) {
        // 必要なアイテム数を算出
        const needItemCount = Math.ceil(remain / Const.AlarmTimeSecond);
        if (needItemCount > player.alarm) {
            return Proto.stringify(Message.Error("アラームが足りません"));
        }
        // アイテムを減らす
        player.alarm -= needItemCount;
        batch.update(doc.player(), { alarm: player.alarm });
        s2c.modified.player = [player];
    }

    const rate = chest.rate;
    const skillIds = await Ref.snapshot<MasterDataIds>(doc.masterdataSkill(rate));
    const characterIds = await Ref.snapshot<MasterDataIds>(doc.masterdataCharacter(rate));    // マスタデータからキャラID一覧取得
    const units = await Ref.collection<UserUnit>(doc.units().where('rare', '==', rate));      // 自分の所持キャラ一覧取得

    const lots: ChestLots[] = [];
    // スキルの抽選一覧
    for (const skill of skillIds.ids) {
        const lot = new ChestLots();
        lot.id = skill;
        lot.type = 'skill';
        lots.push(lot);
    }
    // キャラの抽選一覧
    const hasUnits = units.map(v => v.characterId);
    for (const character of characterIds.ids) {
        if (hasUnits.includes(character)) continue; // すでに持ってるため抽選から外す
        const lot = new ChestLots();
        lot.id = character;
        lot.type = 'character';
        lots.push(lot);
    }

    // 宝箱を消す
    batch.delete(doc.chest(chest.uniq));
    s2c.modified.remove = [chest];

    // 抽選
    const res = lots[Random.Next(0, lots.length)];

    if (res.type === 'character') {
        // キャラ
        var add: UserUnit = {
            characterId: res.id,
            skill:[],
            exp:0,
            rare:chest.rate
        }
        batch.set(doc.unit(add.characterId), add);
        s2c.modified.unit = [add];
    }
    else if (res.type === 'skill')
    {
        // スキル
        const id = res.id;
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
    return Proto.stringify(s2c);
});



class ShadowInfo {
    counter: number = 0;
}
class ShadowFloor {
    ids: number[] = [];
}

// シャドウ生成: c2s
class ShadowCreateSend {
    player!: Player;             // 自分情報
    deck!: UserDeck;             // デッキ情報
    units: UserUnit[] = [];      // バトルに使用するユニット
    edited: UserUnit[] = [];    // 同期必要ユニット
}
// シャドウ生成: s2c
class ShadowCreateRecv {
    roomId: number = 0;              // ルームID
    self: ShadowSelf = new ShadowSelf();    // 自分のシャドウ情報
}

/**
 * proto:CreateShadow:シャドウを生成する
 */
exports.CreateShadow = functions.https.onCall(async (data, context) => {
    const doc = new Documents(context.auth!.uid);
    const c2s = Proto.parse<ShadowCreateSend>(data);
    const s2c = new ShadowCreateRecv();             // s2c

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
    for (const unit of c2s.edited) {
        batch.update(doc.unit(unit.characterId), { skill: unit.skill });
    }

    // 情報からカウンタを取得
    const info = await Ref.snapshot<ShadowInfo>(doc.shadowInfo());
    const floorId = Math.floor(info.counter / Const.RoomOfFloot);    // floor id
    const index = info.counter % Const.RoomOfFloot;    // index
    const floor = await Ref.snapshot<ShadowFloor>(doc.shadowFloor(floorId));
    const roomid = floor.ids[index];    // floor の鍵からルームID取得

    const room: Room = {
        uid: doc.uid,
        seed:0,
        created: ServerTime.current,
        name: c2s.player.name,
        deckJson: JSON.stringify(c2s.deck),
        unitJson: JSON.stringify(c2s.units),
    };
    batch.set(doc.room(roomid), room);
    // カウントアップ
    batch.update(doc.shadowInfo(), { counter: (info.counter + 1) % Const.ROOM_MAX });
    // シャドウIDを保持しておく
    batch.update(doc.player(), { roomid: roomid });

    // 自分の最後に生成したシャドウを記録する
    batch.set(doc.shadowSelf(), room);

    // 情報をそのまま返す
    s2c.self.name = c2s.player.name;
    s2c.self.unit = c2s.units;
    s2c.self.deck = c2s.deck;

    await batch.commit();
    s2c.roomId = roomid;
    return Proto.stringify(s2c);
});

// シャドウバトル]c2s
class ShadowBattleSend {
    roomid:number = 0;       // 
    player!:Player;         // 自分情報
    deck!:UserDeck;         // デッキ情報
    units: UserUnit[] = [];  // バトルに使用するユニット
    edited:UserUnit[] =[];  // 同期必要ユニット
}

/**
 * proto:BattleShadow:とバトルする
 */
exports.BattleShadow = functions.https.onCall(async (data, context) => {
    const doc = new Documents(context.auth!.uid);
    const c2s = Proto.parse<ShadowBattleSend>(data);
    const s2c = new BattleStartRecv();

    const batch = db.batch();
    const room = await Ref.snapshot<Room>(doc.room(c2s.roomid));
    // ルームが存在しない
    if (room === undefined) {
        return Proto.stringify(Message.Warning("指定されたIDが存在しません"));
    }

    // シャドウバトル
    s2c.type = 1;
    s2c.seed = Random.Next();   // シード生成

    const player = await Ref.snapshot<Player>(doc.player());
    // プレイヤー名が変更されたら更新する
    if (c2s.player.name !== player.name) {
        player.name = c2s.player.name;
        batch.update(doc.player(), { name: player.name });
    }
    // デッキ更新
    batch.set(doc.deck(), c2s.deck);
    // 情報更新したユニットを同期する( スキルのみ
    for (const unit of c2s.edited) {
        batch.update(doc.unit(unit.characterId), { skill: unit.skill });
    }
    // TODO : ルームのホストにバトル情報を登録する
    const host = new Documents(room.uid);
    const enemy2host: Room = {
        created: ServerTime.current,
        uid: doc.uid,
        seed: s2c.seed,
        name: player.name,
        unitJson: JSON.stringify(c2s.units),
        deckJson: JSON.stringify(c2s.deck)
    };
    batch.set(host.player().collection('shadow').doc(c2s.roomid.toString()).collection('enemies').doc(), enemy2host);

    // 一旦同期をとる
    await batch.commit();

    // プレイヤーユニット
    s2c.playerUnit = c2s.units;
    s2c.playerDeck = c2s.deck;

    // ルーム内情報
    s2c.enemyUnit = JSON.parse(room.unitJson);
    s2c.enemyDeck = JSON.parse(room.deckJson);

    // 名前を設定する
    s2c.names = [player.name, room.name];

    return Proto.stringify(s2c);

});


class ShadowEnemy {
    seed: number = 0;       // 乱数シード
    name:string = '';       // 名前
    unit:UserUnit[] = [];   // ユニット
    deck!:UserDeck;         // デッキ情報
}

/// <summary>
/// シャドウ自分情報
/// </summary>
class ShadowSelf {
    name:string='';  // 名前
    unit:UserUnit[] = [];
    deck!:UserDeck;
}
// シャドウリスト:c2s
class ShadowListSend {
    roomid: number = 0;
}
// シャドウリスト: s2c
class ShadowListRecv {
    roomid: number = 0;                     // 再確認のため返すだけ
    isActive: boolean = false;              // まだ有効なのか？
    self: ShadowSelf = new ShadowSelf();    // 自分のシャドウ情報
    enemies:ShadowEnemy[] = [];             // 敵のシャドウ情報一覧
}
/**
 * proto:ShadowList:シャドウリスト取得
 */
exports.ShadowList = functions.https.onCall(async (data, context) => {
    const doc = new Documents(context.auth!.uid);
    const c2s = Proto.parse<ShadowListSend>(data);

    const s2c = new ShadowListRecv();             // s2c

    // ルームIDはまだ有効かをチェックする
    const room = await Ref.snapshot<Room>(doc.room(c2s.roomid));
    if (room === undefined) {
        return Proto.stringify(Message.Warning('ルームが存在しませんでした'));
    }
    s2c.roomid = c2s.roomid;
    s2c.isActive = room.uid === doc.uid;    // まだこのルームのオーナーか？

    // 自分が登録した情報取得
    const shadowSelf = await Ref.snapshot<Room>(doc.shadowSelf());
    s2c.self.name = shadowSelf.name;
    s2c.self.unit = JSON.parse(shadowSelf.unitJson);
    s2c.self.deck = JSON.parse(shadowSelf.deckJson);

    // ルーム生成後に登録したバトルのみ取得する
    const enemies = await Ref.collection<Room>(doc.shadowEnemies(c2s.roomid, shadowSelf.created, 50));
    for (const enemy of enemies) {
        const v = new ShadowEnemy();
        v.name = enemy.name;
        v.seed = enemy.seed;
        v.unit = JSON.parse(enemy.unitJson);
        v.deck = JSON.parse(enemy.deckJson);
        s2c.enemies.push(v);
    }
    return Proto.stringify(s2c);
});

// 購入処理:c2s
class PurchasingSend {
    id: string = "";
    platform: string = "";
    receipt: string = "";

}
// 購入処理: s2c
class PurchasingRecv {
    modified: Modified = new Modified();
}

class Product {
    alarm: number = 0;
    bonus: number = 0;
    admin: boolean = true;
}
class Receipt {
    Store: string = "";
    TransactionID: string = "";
    Payload: string = "";
}
/*
 * 購入処理
 */
exports.Purchasing = functions.https.onCall(async (data, context) => {
    const doc = new Documents(context.auth!.uid);
    const c2s = Proto.parse<PurchasingSend>(data); // c2s
    const s2c = new PurchasingRecv();             // s2c

    // 課金アイテムの情報を取得する
    const product = await Ref.snapshot<Product>(doc.product(c2s.id, c2s.platform));
    // 管理者チェック
    if (product.admin) {
        // 権限をチェックする
        if (await Ref.snapshot(doc.admin()) == undefined) {
            return Proto.stringify(Message.Warning('購入できませんでした'));
        }
    }

    // レシートをオブジェクトに
    const receipt: Receipt = JSON.parse(c2s.receipt);

    // TransactionIDはまだ存在していないのを確認する
    if (await Ref.snapshot(doc.purchase(receipt.TransactionID)) != undefined) {
        return Proto.stringify(Message.Warning('使用済のレシートです'));
    }
    const batch = db.batch();

    const now = new Date();
    // レシートを登録
    batch.set(doc.purchase(receipt.TransactionID), {
        date: now.toDateString(),
        time: now.toTimeString(),
        timezoneOffset: now.getTimezoneOffset(),
        id: c2s.id,
        platform: c2s.platform,
        uid: doc.uid,
        receipt: c2s.receipt,
        alarm: product.alarm,
        bonus: product.bonus,
    });

    // アイテム付与
    const player = await Ref.snapshot<Player>(doc.player());
    player.alarm += product.alarm + product.bonus;
    batch.update(doc.player(), { alarm: player.alarm });

    // 同期
    await batch.commit();
    s2c.modified.player = [player];
    return Proto.stringify(s2c);
});

/*
 * request-test
 */
exports.RequestTest = functions.https.onCall(async (data, context) => {
    const html = await rp('http://www.google.com');
    var s2c =
    {
        html: html
    };
    return Proto.stringify(s2c);
});
