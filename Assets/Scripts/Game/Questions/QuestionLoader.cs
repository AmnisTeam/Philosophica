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
        StreamReader reader = new StreamReader("StreamingData/questions.txt");
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
