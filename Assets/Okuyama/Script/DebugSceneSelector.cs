using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugSceneSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private List<string> sceneNames;

    // 各シーンに遷移するボタンを作成
    private void Start()
    {

        for (int i = 0; i < sceneNames.Count; i++)
        {
            // ローカル変数にコピーを作成
            int index = i;
            
            GameObject button = Instantiate(buttonPrefab, transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = sceneNames[index];

            Button buttonCmp = button.GetComponent<Button>();
            buttonCmp.onClick.AddListener(() =>  SceneManager.LoadScene(sceneNames[index]));


            button.transform.localPosition = new Vector3(0, -index * 80, 0);
        }
    }


}
