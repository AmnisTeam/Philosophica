using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static QuestionManager;

[Serializable]
public class QuestionLoader
{
    private List<QuestionManager.Question> questions;
    public QuestionManager.Question currentQuestion;

    public QuestionLoader()
    {
        questions = new List<QuestionManager.Question>();
    }

    /*
     * Загружает вопросы из файла questions.txt. Формат файла следующий:
     * 
     *  Формат файла следующий:
     *  строка 1: вопрос;
     *  строка 2 - 4: ответы;
     *  строка 5: номер правильного ответа;
     *  строка 6: время на вопрос
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
        //ShuffleQuestions();
    }

    /*public void ShuffleQuestions()
    {
        System.Random sr = new System.Random((int)PhotonNetwork.CurrentRoom.CustomProperties["seed"]);

        for (int i = 0; i < questions.Count; i++)
        {
            int rand1 = sr.Next(0, questions.Count);
            int rand2 = sr.Next(0, questions.Count);
            Question q1 = questions[rand1];
            Question q2 = questions[rand2];

            questions[rand1] = q2;
            questions[rand2] = q1;
        }
    }*/

    public QuestionManager.Question GetRandQuestionWithRemove()
    {
        System.Random sr = new System.Random((int)PhotonNetwork.CurrentRoom.CustomProperties["seed"]);
        int rand = sr.Next(0, questions.Count);
        QuestionManager.Question q = questions[rand];
        currentQuestion = q;

        Debug.Log("Остаось " + questions.Count);

        questions.RemoveAt(rand);

        if (questions.Count == 0)
            LoadQuestions();

        return q;
    }

    /*public QuestionManager.Question GetQuestion()
    {
        if (questions.Count == 0)
            LoadQuestions();

        QuestionManager.Question q = questions[0];
        currentQuestion = q;
        questions.RemoveAt(0);

        return q;
    }*/

    public int GetQuestionsSize()
    {
        return questions.Count;
    }
}
