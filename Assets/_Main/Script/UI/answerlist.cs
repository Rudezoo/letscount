using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class answerlist : MonoBehaviour //������� �� �� ������ ����Ʈ
{
    [SerializeField] public string answertag;
    [SerializeField] public Type type;
    [SerializeField] public int index;
    [SerializeField] public Image icon;
    [SerializeField] Text count;
    [SerializeField] public int value=0;

    public void addValue() // ���� ������������
    {
        SoundManager.instance.playMenuSound2(); //ȿ���� ���
        value++;
        count.text = "x" + value;
    }

    public void reduceValue() //���� ���ҽ�������
    {
        SoundManager.instance.playMenuSound2(); //ȿ���� ���

        value--;
        if (value < 0)
        {
            value = 0;
        }
        count.text = "x" + value;
    }


}
