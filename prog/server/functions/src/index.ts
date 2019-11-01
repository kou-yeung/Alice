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
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}

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
    start: number = 0; // 開始時間
    end: number = 0;   // 終了時間
    rate: number = 0;   // レアリティ
}

class UserDeck {
    ids: string[] = [];
}
// 更新されたデータ
class Modified {
    player?:Player;                 // プレイヤー情報
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

class BattleStartRecv {
    seed: any;
    names: any;
    // 味方情報
    playerUnit: any;
    playerDeck: any;
    // 相手情報
    enemyUnit: any;
    enemyDeck: any;
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

    const batch = db.batch();

    // Player情報
    const player = { name: "ゲスト", token: Guid.NewGuid(), totalBattleCount:0 };
    batch.set(db.collection('player').doc(user.uid), player);

    // 初期ユニット情報
    const unit = { characterId: "Character_001_001", skill: [] };
    batch.set(db.collection('player').doc(user.uid).collection("units").doc(unit.characterId), unit);

    // 初期デッキ設定
    const deck = { ids: [unit.characterId, "", "", ""] };
    batch.set(db.collection('deck').doc(user.uid), deck);

    // コミット
    await batch.commit();
});

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

    const uid = context.auth!.uid;

    // プレイヤー情報取得
    const player = await Ref.snapshot<Player>(db.collection('player').doc(uid));

    // ログインボーナスチェック
    const bonus = await LoginBonus(player);
    if (bonus) {
        await db.collection('player').doc(uid).set(player);
    }

    // デッキ情報
    const deck = await Ref.snapshot<UserDeck>(db.collection('deck').doc(uid));
    // ユニット一覧
    const units = await Ref.collection<UserUnit>(db.collection('player').doc(uid).collection('units'));
    // スキル一覧
    const skills = await Ref.collection<UserSkill>(db.collection('player').doc(uid).collection('skills'));
    // 宝箱一覧
    const chests = await Ref.collection<UserChest>(db.collection('player').doc(uid).collection('chests'));

    // 返信を構築
    const s2c = new HomeRecv();
    s2c.svtime = ServerTime.current;
    s2c.bonus = bonus;
    s2c.player = player;
    s2c.deck = deck;
    s2c.units = units;
    s2c.skills = skills;
    s2c.chests = chests;

    return JSON.stringify(s2c);
});


/**
 * proto:Battle:バトル開始
 */
exports.Battle = functions.https.onCall(async (data, context) => {
    const c2s = JSON.parse(data);

    const s2c = new BattleStartRecv();

    s2c.seed = 0;

    s2c.names = ["PLAYER1", "PLAYER2"];
    s2c.playerUnit = c2s.units;
    s2c.playerDeck = c2s.deck;

    s2c.enemyUnit = c2s.recommendUnits;
    s2c.enemyDeck = c2s.recommendDeck;



    return JSON.stringify(s2c);
});


export type dataUpdate<T> = Partial<T>;
// c2s
class GameSetSend {
    ID?: string;
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

    const uid = context.auth!.uid;
    const Win = 1;

    const c2s: GameSetSend = JSON.parse(data);
    const s2c: GameSetRecv = new GameSetRecv();

    // プレイヤーのバトル回数更新
    const player = await Ref.snapshot<Player>(db.collection('player').doc(uid));
    const batch = db.batch();

    player.totalBattleCount += 1;   // 累計   
    player.todayBattleCount += 1;   // 本日
    if (c2s.result == Win) {
        player.todayWinCount += 1;
    }
    batch.set(db.collection('player').doc(uid), player);
    s2c.modified.player = player;

    // 宝箱を追加します
    const chests = await Ref.collection<UserChest>(db.collection('player').doc(uid).collection('chests'));
    if (chests.length < 3) {

        const start = ServerTime.current;
        const end = start + (15 * 60);

        const chest = {
            uniq: Guid.NewGuid(),
            start: start,
            end: end,
            rate: 2
        }
        batch.set(db.collection('player').doc(uid).collection('chests').doc(chest.uniq), chest);
        s2c.modified.chest = [chest]
    }

    // 同期
    await batch.commit();

    return JSON.stringify(s2c);
});

