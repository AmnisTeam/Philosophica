using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QuestionLoader
{
    public List<QuestionManager.Question> questions;

    public QuestionLoader()
    {
        questions = new List<QuestionManager.Question>();
    }

    /*
     * ��������� ������� �� ����� questions.txt. ������ ����� ���������:
     * 
     *  ������ ����� ���������:
     *  ������ 1: ������;
     *  ������ 2 - 4: ������;
     *  ������ 5: ����� ����������� ������;
     *  ������ 6: ����� �� ������
     */ 
    public void LoadQuestions()
    {
        //StreamReader reader = new StreamReader("StreamingData/questions.txt");
        StreamReader reader = null;
        
        if (Application.platform == RuntimePlatform.WindowsEditor) {
            reader = new StreamReader("Assets/StreamingAssets/questions.txt");
        } else if (Application.platform == RuntimePlatform.WindowsPlayer) {
            reader = new StreamReader(Application.streamingAssetsPath + "/questions.txt");
        } else if (Application.platform == RuntimePlatform.Android) {
            string path = "jar:file://" + Application.dataPath + "!/assets/questions.txt";
            WWW wwwfile = new WWW(path);

            while (!wwwfile.isDone) { }

            var filepath = string.Format("{0}/{1}", Application.persistentDataPath, "questions.txt");
            File.WriteAllBytes(filepath, wwwfile.bytes);

            reader = new StreamReader(filepath);
        }
        
        string line = reader.ReadLine();
        int id = 0;
        QuestionManager.Question question = null;

        while(line != null)
        {
            if(id == 0)
            {
                question = new QuestionManager.Question();
                question.question = new string(line);
            }
            else if(id >= 1 && id < 5)
            {
                question.answer[id - 1] = new string(line);
            }
            else if(id == 5)
            {
                question.idRightAnswer = int.Parse(line);
            }
            else if(id == 6)
            {
                question.timeToQuestion = float.Parse(line);
                questions.Add(question);
            }
            id = (id + 1) % 8;
            line = reader.ReadLine();
        }
    }
}
