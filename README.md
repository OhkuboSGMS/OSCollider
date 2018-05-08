# Unity Cone Collider
Cone Collider はBoxColliderを使って空洞があるシリンダーのようなコライダーを作成します

![use1](https://github.com/OhkuboSGMS/OSCollider/blob/master/2018-05-08%2020-31-15_1.gif)

# How To Use

![use2](https://github.com/OhkuboSGMS/OSCollider/blob/master/2018-05-08%2020-32-23.gif)

1. GameObjectにスクリプトをアタッチ
2. Updateボタンを押す
3. 値を調整

# その他
Attach ObjectにGameObjectをアタッチするとそのオブジェクトにBoxColliderを追加して並べます。

アタッチしていない場合は自動でGameObjectが生成されます


空洞があるコライダーを作るときにMesh ColliderだとConvexにチェックが入れると空洞ができないため作ってみました。

パフォーマンスは測ってないですが多分いいんじゃないでしょうか
