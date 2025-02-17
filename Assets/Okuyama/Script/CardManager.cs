using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // 手札の位置調整用パラメータ
    [SerializeField] private Vector3 HandCenter;
    [SerializeField] private float HandRadius;
    [SerializeField] private float HandAngle;

    [SerializeField] private Vector3 CardStartPos; //ドロー時にカードが出現する位置

    [SerializeField] private GameObject CardPrefab;
    private Vector3 CardSize
    {
        get { return CardPrefab.GetComponent<SpriteRenderer>().bounds.size; }
    }


    // ドブルカードの配列
    // この要素数が総カード数、かつ総シンボル数
    private List<int>[] dobbleCardArr;

    private List<CardObject> hand = new List<CardObject>(); // 手札

    // シングルトン(簡易)
    public static CardManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        // ドブルカードの初期化
        InitializeDobbleCardArr(4);
    }

    void Update()
    {

    }

    // カードを引く
    public void Draw()
    {
        GameObject cardObj = Instantiate(CardPrefab, transform);
        cardObj.transform.parent = transform; //子にする
        cardObj.transform.position = CardStartPos; //初期位置に移動

        CardObject card = cardObj.GetComponent<CardObject>();
        card.Initialize(0); //TODO:ランダムに引く

        hand.Add(card);
        AdjustHandPos();
    }

    private void AdjustHandPos()
    {
        Debug.Log(hand.Count);
        List<CardObject.CardPos> cardPosList = CalculateHandPos(hand.Count);
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].SetHandPos(cardPosList[i]);
        }
    }


    // 手札枚数に応じた手札の位置を計算
    // TODO:手札枚数が少ないときの見栄えが悪い。要調整
    List<CardObject.CardPos> CalculateHandPos(int handNum)
    {
        List<CardObject.CardPos> cardPosList = new List<CardObject.CardPos>();

        // 例外処理
        if (handNum == 0) return cardPosList;
        if (handNum == 1)
        {
            CardObject.CardPos cardPos = new CardObject.CardPos();
            cardPos.center = HandCenter + new Vector3(0, HandRadius, 0);
            cardPos.angle = 0;
            cardPosList.Add(cardPos);
            return cardPosList;
        }

        //ほんへ
        for (int i = 0; i < handNum; i++)
        {
            CardObject.CardPos cardPos = new CardObject.CardPos();

            float angle = Mathf.Lerp(-HandAngle, HandAngle, (float)i / (handNum - 1));

            // 中心座標計算
            float CenterX = HandCenter.x + HandRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
            float CenterY = HandCenter.y + HandRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float CenterZ = i * -0.1f; // 右の手札ほど手前に (UIデザイン考慮して検討)

            cardPos.center = new Vector3(CenterX, CenterY, CenterZ);
            cardPos.angle = -angle;
            cardPosList.Add(cardPos);
        }
        return cardPosList;
    }

    // DpbbleCardList (カード番号とマークの対応表)の初期化
    private void InitializeDobbleCardArr(int numOfSymbols)
    {
        int n_1 = numOfSymbols - 1;
        List<List<int>> cards = new List<List<int>>();
        List<int> card;

        //最初の一枚
        card = new List<int>();
        for (int i = 0; i <= n_1; i++)
        {
            card.Add(i);
        }
        cards.Add(card);

        //次のn枚
        for (int j = 0; j < n_1; j++)
        {
            card = new List<int>();
            card.Add(0);
            for (int k = 0; k < n_1; k++)
            {
                card.Add(n_1 + n_1 * j + k + 1);
            }
            cards.Add(card);
        }

        // 残りの n² 枚
        for (int i = 1; i <= n_1; i++)
        {
            for (int j = 1; j <= n_1; j++)
            {
                card = new List<int>();
                card.Add(i);
                for (int k = 1; k <= n_1; k++)
                {
                    card.Add(n_1 + 1 + n_1 * (k - 1) + ((i - 1) * (k - 1) + j - 1) % n_1);
                }
                cards.Add(card);
            }
        }

        dobbleCardArr = new List<int>[cards.Count];
        for (int i = 0; i < cards.Count; i++)
        {
            dobbleCardArr[i] = cards[i];   //配列に写す
        }

        return;
    }

    // 指定カードのシンボルリストを取得するメソッド
    public List<int> GetSymbolList(int cardID)
    {
        return dobbleCardArr[cardID];
    }


    void OnDrawGizmos()
    {
        // デモ用にカード配置を描画
        List<CardObject.CardPos> cardPosList = CalculateHandPos(5);
        Gizmos.color = Color.red;
        foreach (var cardPos in cardPosList)
        {
            DrawCardGizmo(cardPos);
        }

        // ドロー時の出現位置
        Gizmos.DrawSphere(CardStartPos, 0.1f);


    }

    // Gizmoで角度を含めて描画する
    private void DrawCardGizmo(CardObject.CardPos cardPos)
    {
        Matrix4x4 original = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            cardPos.center,
            transform.rotation * Quaternion.Euler(0, 0, cardPos.angle), // オブジェクトの回転に追加
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, CardSize);
        Gizmos.matrix = original;
    }
}

