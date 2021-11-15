using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class answerlist : MonoBehaviour //답안지에 들어갈 각 동물별 리스트
{
    [SerializeField] public string answertag;
    [SerializeField] public Type type;
    [SerializeField] public int index;
    [SerializeField] public Image icon;
    [SerializeField] Text count;
    [SerializeField] public int value=0;

    public void addValue() // 수를 증가시켰을때
    {
        SoundManager.instance.playMenuSound2(); //효과음 재생
        value++;
        count.text = "x" + value;
    }

    public void reduceValue() //수를 감소시켰을때
    {
        SoundManager.instance.playMenuSound2(); //효과음 재생

        value--;
        if (value < 0)
        {
            value = 0;
        }
        count.text = "x" + value;
    }


}
