import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';

// Guidを生成する
class Guid {
    static NewGuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}

/**
 * ログインボーナスチェック
 * return (null)ログインボーナス発生しない 
 * @param context
 * @param player
 */
function LoginBonus(player: any) {

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


//class Documents {
//    player: FirebaseFirestore.DocumentReference;

//    constructor(uid: string) {
//        this.player = db.collection('player').doc(uid);
//    }
//    async getPlayer(): Promise<FirebaseFirestore.DocumentData | undefined> {
//        const doc = await this.player.get();
//        return doc.data();
//    }
//    async setPlayer(data: any) {
//        await this.player.set(data);
//    }

//    async getUnits(): Promise<any[]> {
//        const doc = await this.player.collection('units').get();
//        const units:any[] = [];
//        doc.forEach(snap => units.push(snap.data()));
//        return units;
//    }
//    async setUnits(units: any[]) {
//        for (const unit of units) {
//            const doc = await this.player.collection('units').doc(unit.characterId);
//            await doc.set(unit);
//        }
//    }

//    async getDeck(): Promise<any> {
//        const doc = await this.player.collection('decks').doc("1").get();
//        return JSON.parse(doc.get('json'));
//    }
//    async setDeck(deck: any) {
//        const doc = this.player.collection('decks').doc("1");
//        await doc.set({ json: JSON.stringify(deck) });
//    }
//}

class HomeRecv {
    debug: string = "";
    player: any;
    bonus: any;
    deck: any;
    units: any;
    skills: any;
    chests: any;
}

//======================

// init admin to use admin function.
admin.initializeApp();
const db = admin.firestore();

exports.helloWorld = functions.https.onRequest((request, response) => {
    response.send("Hello from Firebase!!");
});

/**
 *
 * 新規ユーザ作成時
 */
exports.onCreate = functions.auth.user().onCreate(async (user) => {

    let batch = db.batch();

    // Player情報
    const player = { name: "ゲスト", token: Guid.NewGuid() };
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

exports.getItems = functions.https.onCall(async (data, context) => {

    return await db.collection('items').get().then(snapshot => {

        const res:any[] = [];
        snapshot.forEach(doc => {
            res.push(doc.data());
        });
        return JSON.stringify(res);
    });
});


/**
 * proto:Home:画面の情報を取得する
 */ 
exports.Home = functions.https.onCall(async (data, context) => {

    const uid = context.auth!.uid;

    // プレイヤー情報取得
    const playerDoc = await db.collection('player').doc(uid).get();
    const player : any = playerDoc.data();

    // ログインボーナスチェック
    const bonus = await LoginBonus(player);
    if (bonus) {
        await db.collection('player').doc(uid).set(player);
    }

    // デッキ情報
    const deckDoc = await db.collection('deck').doc(uid).get();

    // ユニット一覧
    const unitsDoc = await db.collection('player').doc(uid).collection('units').get();
    const units: any[] = [];
    unitsDoc.forEach(snap => units.push(snap.data()));

    // スキル一覧
    const skillsDoc = await db.collection('player').doc(uid).collection('skills').get();
    const skills: any[] = [];
    skillsDoc.forEach(snap => skills.push(snap.data()));

    // 宝箱一覧
    const chestsDoc = await db.collection('player').doc(uid).collection('chests').get();
    const chests: any[] = [];
    chestsDoc.forEach(snap => chests.push(snap.data()));

    // 返信を構築
    const s2c = new HomeRecv();
    s2c.bonus = bonus;
    s2c.player = player;
    //s2c.debug = (snap.exists) ? "exists" : "not exists";
    s2c.deck = deckDoc.data();
    s2c.units = units;
    s2c.skills = skills;
    s2c.chests = chests;

    return JSON.stringify(s2c);
});
