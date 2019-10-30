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

// ログインボーナス
async function LoginBonus(context: functions.https.CallableContext, player: any) {
    if (player.stamp) return null;  // タイムスタンプチェック
    player.stamp = "OK";
    player.loginCount = (player.loginCount || 0) + 1;

    const doc = db.collection('player').doc(context.auth!.uid);
    await doc.update(player);
    return null;
}

//FirebaseFirestore.DocumentSnapshot.prototype.Cast = function () {
//};

//class Utils {
//    static Cast<T>(snapshot: FirebaseFirestore.DocumentSnapshot) {
//        const res = new T();
//        for (const key of res.keys()) {
//            res[key] = snapshot.get(key);
//        }
//        return res;
//    }
//}
//======================
// Proto

//class Player {
//    public name: string = "";              // ユーザ名
//    public alarm: number = 0;              // アラーム(時間短縮アイテム
//    public rank: number = 0;               // プレイヤーランキング
//    public ads: number = 0;                // 残り広告使用回数
//    public token: string = "";             // 認証トークン
//    public stamp: string = "";             // 最後ログインの日付
//    public loginCount: number = 0;         // 累計ログイン日数
//    public totalBattleCount: number = 0;   // 累計バトル回数
//    public todayBattleCount: number = 0;   // 本日バトルした回数
//    public todayWinCount: number = 0;      // 本日勝利した回数

//    constructor() {
//        this.name = "ゲスト";
//        this.token = Guid.NewGuid();
//    }
//}

class HomeRecv {
    player:any;
}

//======================

// init admin to use admin function.
admin.initializeApp();
const db = admin.firestore();

exports.helloWorld = functions.https.onRequest((request, response) => {
    response.send("Hello from Firebase!!");
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

async function GenOrGetPlayer(context: functions.https.CallableContext) {
    const doc = db.collection('player').doc(context.auth!.uid);
    const snap = await doc.get();
    if (snap.exists) return snap.data();    // あればそのまま返す

    // 新規ユーザを生成
    const player =
    {
        name: "ゲスト",
        token: Guid.NewGuid(),
    };
    await doc.set(player);
    return player;
}

/**
 * proto:Home:画面の情報を取得する
 */ 
exports.Home = functions.https.onCall(async (data, context) => {
    const s2c = new HomeRecv();
    s2c.player = await GenOrGetPlayer(context);
    await LoginBonus(context, s2c.player);
    return JSON.stringify(s2c);
    //    const s2c = new HomeRecv();
    //    if (doc.exists) {
    //        s2c.player = doc.data();//Utils.Cast<Player>(doc);
    //    } else {
    //        // 新規ユーザ
    //        s2c.player = new Player();
    //    }
    //    return JSON.stringify(s2c);
    //});
});
