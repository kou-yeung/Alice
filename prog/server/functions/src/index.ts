import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';


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
